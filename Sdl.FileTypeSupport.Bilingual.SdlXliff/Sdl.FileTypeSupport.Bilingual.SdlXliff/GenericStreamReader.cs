using Sdl.FileTypeSupport.Bilingual.SdlXliff.Util;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Xml;

namespace Sdl.FileTypeSupport.Bilingual.SdlXliff
{
	public abstract class GenericStreamReader : ISdlXliffStreamContentHandler
	{
		public IBasicMessageReporter MessageReporter
		{
			get;
			set;
		}

		public void ProcessStream(string xliffInputFilePath, string encryptionKey)
		{
			AesCryptoServiceProvider aesCryptoServiceProvider = null;
			if (!string.IsNullOrEmpty(encryptionKey))
			{
				byte[] decodedKey = CryptographicHelper.GetDecodedKey(encryptionKey);
				aesCryptoServiceProvider = new AesCryptoServiceProvider
				{
					Mode = CipherMode.CBC,
					Padding = PaddingMode.PKCS7,
					Key = decodedKey
				};
			}
			using (FileStream fileStream = File.OpenRead(xliffInputFilePath))
			{
				if (aesCryptoServiceProvider != null)
				{
					if (!CryptographicHelper.ReadHeader(fileStream, out byte[] IV))
					{
						throw new Exception(StringResources.Encryption_AttemptingToReadInvalidEncryptedFile);
					}
					aesCryptoServiceProvider.IV = IV;
					using (CryptoStream stream = new CryptoStream(fileStream, aesCryptoServiceProvider.CreateDecryptor(), CryptoStreamMode.Read))
					{
						ProcessInternalStream(stream);
					}
				}
				else
				{
					ProcessInternalStream(fileStream);
				}
			}
		}

		public void ProcessInternalStream(Stream stream)
		{
			using (SdlXliffFeeder sdlXliffFeeder = new SdlXliffFeeder(stream))
			{
				sdlXliffFeeder.RegisterSubscriber(this);
				try
				{
					while (sdlXliffFeeder.ContinueScanning())
					{
					}
				}
				finally
				{
					sdlXliffFeeder.UnregisterSubscriber(this);
					sdlXliffFeeder.Close();
				}
			}
		}

		public virtual void OnDocInfo(XmlElement docInfo)
		{
		}

		public virtual bool OnStartFile(List<XmlAttribute> fileAttributes)
		{
			return true;
		}

		public virtual void OnFileInfo(XmlElement fileInfo)
		{
		}

		public virtual void OnTagDefinition(XmlElement tagDefinition)
		{
		}

		public virtual void OnContextDefinition(XmlElement contextDefinition)
		{
		}

		public virtual void OnNodeDefinition(XmlElement nodeDefinition)
		{
		}

		public virtual void OnFormattingDefinition(XmlElement formattingDefintion)
		{
		}

		public virtual void OnFileTypeInfo(XmlElement fileTypeInfo)
		{
		}

		public virtual void OnCommentReference(XmlElement commentReference)
		{
		}

		public virtual void OnExternalFile(XmlElement externalFile)
		{
		}

		public virtual void OnInternalFile(XmlElement internalFile)
		{
		}

		public virtual void OnDependencyFiles(XmlElement dependencyFiles)
		{
		}

		public virtual void OnGroup(XmlElement group)
		{
		}

		public virtual void OnTranslationUnit(XmlElement translationUnit)
		{
		}

		public virtual void OnBinaryUnit(XmlElement binaryUnit)
		{
		}

		public virtual void OnStartFileHeader()
		{
		}

		public virtual void OnEndFileHeader()
		{
		}

		public virtual void OnEndFile()
		{
		}
	}
}
