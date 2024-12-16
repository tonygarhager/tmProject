using Oasis.Xliff12;
using Sdl.FileTypeSupport.Bilingual.SdlXliff.Util;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Sdl.FileTypeSupport.Bilingual.SdlXliff
{
	public class XliffFileWriter : XliffWriter, IBilingualDocumentFileWriter, IBilingualDocumentWriter, IBilingualWriter, IBilingualContentHandler, IDisposable, IBilingualDocumentOutputPropertiesAware, ISharedObjectsAware
	{
		private static XmlSerializer _xliffXmlSerializer;

		private XmlSerializerNamespaces _Namespaces = new XmlSerializerNamespaces();

		private bool _validateAfterWriting;

		private bool _createValidationLogger;

		private ISharedObjects _sharedObjects;

		private IBilingualDocumentOutputProperties _outputProperties;

		public static XmlSerializer XliffXmlSerializer
		{
			get
			{
				return _xliffXmlSerializer;
			}
			set
			{
				_xliffXmlSerializer = value;
			}
		}

		public bool CreateValidationLogger
		{
			get
			{
				return _createValidationLogger;
			}
			set
			{
				_createValidationLogger = value;
			}
		}

		public bool ValidateAfterWriting
		{
			get
			{
				return _validateAfterWriting;
			}
			set
			{
				_validateAfterWriting = value;
			}
		}

		public XmlSerializerNamespaces Namespaces
		{
			get
			{
				return _Namespaces;
			}
			set
			{
				_Namespaces = value;
			}
		}

		public event EventHandler<SdlXliffValidationEventArgs> ValidationIssue;

		static XliffFileWriter()
		{
			_xliffXmlSerializer = new XmlSerializer(typeof(xliff));
		}

		public XliffFileWriter()
		{
			_Namespaces.Add("sdl", "http://sdl.com/FileTypes/SdlXliff/1.0");
		}

		public XliffFileWriter(string xliffFilePath)
			: this()
		{
			base.OutputFilePath = xliffFilePath;
		}

		protected virtual void OnValidationIssue(SdlXliffValidationEventArgs args)
		{
			this.ValidationIssue?.Invoke(this, args);
		}

		public override void Complete()
		{
			base.Complete();
			string tempFileName = GetTempFileName();
			base.DocumentProperties.LastSavedAsPath = base.OutputFilePath;
			string text = "";
			if (File.Exists(base.OutputFilePath))
			{
				Random random = new Random();
				do
				{
					text = Path.GetDirectoryName(base.OutputFilePath) + "\\" + Path.GetFileNameWithoutExtension(base.OutputFilePath) + "_" + random.Next(100000, 999999).ToString() + Path.GetExtension(base.OutputFilePath);
				}
				while (File.Exists(text));
				if (File.Exists(text))
				{
					File.Delete(text);
				}
				File.Move(base.OutputFilePath, text);
			}
			try
			{
				WriteSdlXliffToFile(tempFileName);
			}
			catch
			{
				if (File.Exists(text))
				{
					File.Move(text, base.OutputFilePath);
				}
				throw;
			}
			finally
			{
				if (File.Exists(text))
				{
					File.Delete(text);
				}
			}
			Close();
			File.Move(tempFileName, base.OutputFilePath);
			if (_validateAfterWriting)
			{
				ValidateWrittenFile();
			}
		}

		private void WriteSdlXliffToFile(string tempOutputFile)
		{
			string text = _sharedObjects?.GetSharedObject<string>("SDL-ENC:EncryptionKey");
			if (!string.IsNullOrEmpty(text))
			{
				byte[] decodedKey = CryptographicHelper.GetDecodedKey(text);
				AesCryptoServiceProvider aesCryptoServiceProvider = new AesCryptoServiceProvider
				{
					Mode = CipherMode.CBC,
					Padding = PaddingMode.PKCS7,
					Key = decodedKey,
					IV = CryptographicHelper.GenerateRandomNumber(16)
				};
				using (FileStream fileStream = new FileStream(tempOutputFile, FileMode.Create))
				{
					CryptographicHelper.WriteHeader(fileStream, aesCryptoServiceProvider.IV);
					CryptoStream cryptoStream = new CryptoStream(fileStream, aesCryptoServiceProvider.CreateEncryptor(), CryptoStreamMode.Write);
					using (XliffFormattingXmlTextWriter writer = new XliffFormattingXmlTextWriter(new StreamWriter(cryptoStream, Encoding.UTF8, 5242880)))
					{
						WriteSdlXliff(writer);
						cryptoStream.Flush();
						fileStream.Flush();
					}
				}
			}
			else
			{
				using (XliffFormattingXmlTextWriter writer2 = new XliffFormattingXmlTextWriter(new StreamWriter(tempOutputFile, append: false, Encoding.UTF8, 5242880)))
				{
					WriteSdlXliff(writer2);
				}
			}
		}

		private void ValidateWrittenFile()
		{
			try
			{
				SdlXliffValidator sdlXliffValidator = new SdlXliffValidator();
				XmlValidationLogger logger = null;
				if (_createValidationLogger)
				{
					string xmlLogFilePath = base.OutputFilePath + "_log.xml";
					logger = new XmlValidationLogger(xmlLogFilePath);
				}
				sdlXliffValidator.ValidationIssue += delegate(object o, SdlXliffValidationEventArgs args)
				{
					string message = string.Format(StringResources.XLIFF_ReaderValidationMessage, args.Message, args.Line.ToString(CultureInfo.CurrentCulture), args.Offset.ToString(CultureInfo.CurrentCulture));
					string locationDescription = string.Format(StringResources.LineOffsetLocationInFile, args.Line.ToString(CultureInfo.CurrentCulture), args.Offset.ToString(CultureInfo.CurrentCulture), args.FilePath);
					ReportMessage(this, StringResources.XliffFilterName, (args.Severity == XmlSeverityType.Warning) ? ErrorLevel.Warning : ErrorLevel.Error, message, locationDescription);
					logger?.LogEntries.Add(new XmlValidationLogEntry(args.Message, args.Line, args.Offset));
				};
				if (!sdlXliffValidator.Validate(base.OutputFilePath))
				{
					logger?.WriteLogFile();
				}
			}
			catch (Exception ex)
			{
				ReportMessage(this, StringResources.XliffFilterName, ErrorLevel.Warning, ex.Message, string.Empty);
			}
		}

		public void GetProposedFileInfo(IDocumentProperties documentInfo, IOutputFileInfo proposedFileInfo)
		{
		}

		public void SetOutputProperties(IBilingualDocumentOutputProperties outputProperties)
		{
			_outputProperties = outputProperties;
			base.OutputFilePath = _outputProperties.OutputFilePath;
			base.LinkedDependencyFiles = outputProperties.LinkedDependencyFiles;
		}

		protected override void Dispose(bool isDisposing)
		{
			Close();
			Dispose();
		}

		private void Close()
		{
			base.Xliff = null;
		}

		public void SetSharedObjects(ISharedObjects sharedObjects)
		{
			_sharedObjects = sharedObjects;
		}
	}
}
