using Sdl.LanguagePlatform.Core.Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;

namespace Sdl.Core.LanguageProcessing.Resources
{
	public class ResourceFileResourceAccessor : ResourceStorage
	{
		private class ResourceFileData
		{
			public List<string> ResourceNames;

			public Dictionary<string, byte[]> Data;
		}

		private static readonly Dictionary<string, ResourceFileData> Cache;

		private static readonly object CacheLock;

		private readonly string _fileName;

		private ResourceFileData _resourceData;

		public static readonly string DefaultResourceFileName;

		static ResourceFileResourceAccessor()
		{
			CacheLock = new object();
			DefaultResourceFileName = "Sdl.LanguagePlatform.NLP.resources";
			Cache = new Dictionary<string, ResourceFileData>();
		}

		public ResourceFileResourceAccessor(string resourceFileName)
			: base(null)
		{
			_fileName = resourceFileName;
			_resourceData = null;
		}

		public ResourceFileResourceAccessor()
			: base(null)
		{
			try
			{
				string text = _fileName = GetResourceFile();
				_resourceData = null;
			}
			catch (FileNotFoundException)
			{
				throw new LanguageProcessingException(ErrorMessages.EMSG_LanguageResourceFileNotFound);
			}
		}

		public static string GetResourceFile()
		{
			string text = string.Empty;
			Assembly assembly = typeof(ResourceFileResourceAccessor).Assembly;
			if (!assembly.GlobalAssemblyCache)
			{
				if (assembly.Location != assembly.CodeBase && !string.IsNullOrEmpty(assembly.CodeBase))
				{
					Uri uri = new Uri(assembly.CodeBase);
					string localPath = uri.LocalPath;
					FileInfo fileInfo = new FileInfo(localPath);
					text = GetResourceLocation(fileInfo.DirectoryName);
				}
				if (text == string.Empty)
				{
					text = GetResourceLocation(assembly.Location);
				}
			}
			if (text == string.Empty)
			{
				Environment.SpecialFolder[] array = new Environment.SpecialFolder[3]
				{
					Environment.SpecialFolder.CommonApplicationData,
					Environment.SpecialFolder.ApplicationData,
					Environment.SpecialFolder.LocalApplicationData
				};
				Environment.SpecialFolder[] array2 = array;
				foreach (Environment.SpecialFolder folder in array2)
				{
					string loadPath = Path.Combine(Environment.GetFolderPath(folder), string.Format("SDL International{0}Lingua{0}", Path.DirectorySeparatorChar));
					text = GetResourceLocation(loadPath);
					if (text != string.Empty)
					{
						break;
					}
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				throw new FileNotFoundException("Could not find language resource file", DefaultResourceFileName);
			}
			return text;
		}

		private static string GetResourceLocation(string loadPath)
		{
			string path = Path.Combine(loadPath, DefaultResourceFileName);
			string fullPath = Path.GetFullPath(path);
			FileInfo fileInfo = new FileInfo(fullPath);
			string result = string.Empty;
			if (fileInfo.Exists)
			{
				result = fileInfo.FullName;
			}
			return result;
		}

		public override List<string> GetAllResourceKeys()
		{
			lock (CacheLock)
			{
				if (_resourceData != null)
				{
					return _resourceData.ResourceNames;
				}
				if (Cache.TryGetValue(_fileName, out _resourceData))
				{
					return _resourceData.ResourceNames;
				}
				if (!File.Exists(_fileName))
				{
					throw new LanguageProcessingException(ErrorMessages.EMSG_LanguageResourceFileNotFound);
				}
				_resourceData = new ResourceFileData
				{
					ResourceNames = new List<string>(),
					Data = new Dictionary<string, byte[]>()
				};
				ResourceReader resourceReader = null;
				try
				{
					resourceReader = new ResourceReader(_fileName);
					foreach (DictionaryEntry item in resourceReader)
					{
						_resourceData.ResourceNames.Add((string)item.Key);
						_resourceData.Data.Add((string)item.Key, (byte[])item.Value);
					}
					Cache.Add(_fileName, _resourceData);
				}
				catch
				{
					throw new LanguageProcessingException(ErrorMessages.EMSG_InvalidLanguageResourceFile);
				}
				finally
				{
					resourceReader?.Close();
				}
			}
			return _resourceData.ResourceNames;
		}

		public override Stream ReadResourceData(CultureInfo culture, LanguageResourceType t, bool fallback)
		{
			byte[] resourceData = GetResourceData(culture, t, fallback);
			if (resourceData == null)
			{
				return null;
			}
			return new MemoryStream(resourceData, writable: false);
		}

		public override byte[] GetResourceData(CultureInfo culture, LanguageResourceType t, bool fallback)
		{
			lock (CacheLock)
			{
				if (_resourceData == null)
				{
					GetAllResourceKeys();
					if (_resourceData == null)
					{
						return null;
					}
				}
			}
			string resourceName = GetResourceName(culture, t, fallback);
			if (resourceName != null && _resourceData.Data.TryGetValue(resourceName, out byte[] value))
			{
				return value;
			}
			return null;
		}
	}
}
