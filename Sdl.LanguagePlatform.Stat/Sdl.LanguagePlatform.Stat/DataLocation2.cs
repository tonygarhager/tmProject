using Sdl.LanguagePlatform.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Sdl.LanguagePlatform.Stat
{
	public class DataLocation2
	{
		public static readonly Dictionary<string, DataFileType> ExtensionToTypeMapping;

		public static readonly Dictionary<DataFileType, string> TypeToExtensionMapping;

		public DirectoryInfo Directory
		{
			get;
			set;
		}

		static DataLocation2()
		{
			ExtensionToTypeMapping = new Dictionary<string, DataFileType>();
			TypeToExtensionMapping = new Dictionary<DataFileType, string>();
			ExtensionToTypeMapping.Add(".p", DataFileType.PlainFile);
			ExtensionToTypeMapping.Add(".px", DataFileType.PlainFileIndex);
			ExtensionToTypeMapping.Add(".t", DataFileType.TokenFile);
			ExtensionToTypeMapping.Add(".tx", DataFileType.TokenFileIndex);
			ExtensionToTypeMapping.Add(".ti", DataFileType.TokenInvertedFile);
			ExtensionToTypeMapping.Add(".tix", DataFileType.TokenInvertedFileIndex);
			ExtensionToTypeMapping.Add(".v", DataFileType.VocabularyFile);
			ExtensionToTypeMapping.Add(".f", DataFileType.FrequencyCountsFile);
			ExtensionToTypeMapping.Add(".2", DataFileType.NGram2CountsFile);
			ExtensionToTypeMapping.Add(".3", DataFileType.NGram3CountsFile);
			ExtensionToTypeMapping.Add(".pd", DataFileType.PhraseDictionaryFile);
			ExtensionToTypeMapping.Add(".sa", DataFileType.SuffixArray);
			ExtensionToTypeMapping.Add(".c2m", DataFileType.MonolingualChiSquareScores);
			ExtensionToTypeMapping.Add(".wa", DataFileType.WordAlignmentFile);
			ExtensionToTypeMapping.Add(".wax", DataFileType.WordAlignmentFileIndex);
			ExtensionToTypeMapping.Add(".t1", DataFileType.SimpleTranslationProbabilitiesFile);
			ExtensionToTypeMapping.Add(".pm", DataFileType.PhraseMappingFile);
			ExtensionToTypeMapping.Add(".c2b", DataFileType.BilingualChiSquareScores);
			foreach (KeyValuePair<string, DataFileType> item in ExtensionToTypeMapping)
			{
				TypeToExtensionMapping.Add(item.Value, item.Key);
			}
		}

		public static bool IsBilingualDataType(DataFileType t)
		{
			if (t != DataFileType.SimpleTranslationProbabilitiesFile && t != DataFileType.WordAlignmentFile && t != DataFileType.WordAlignmentFileIndex && t != DataFileType.PhraseMappingFile)
			{
				return t == DataFileType.BilingualChiSquareScores;
			}
			return true;
		}

		public DataLocation2(string folderName)
			: this(new DirectoryInfo(folderName))
		{
		}

		public DataLocation2(DirectoryInfo folder)
		{
			if (folder == null)
			{
				throw new ArgumentNullException("folder");
			}
			if (!folder.Exists)
			{
				throw new DirectoryNotFoundException(folder.Name);
			}
			Directory = folder;
		}

		public List<DataFileInfo> GetComponents()
		{
			List<DataFileInfo> list = new List<DataFileInfo>();
			FileInfo[] files = Directory.GetFiles();
			foreach (FileInfo fileInfo in files)
			{
				try
				{
					DataFileInfo item = new DataFileInfo(fileInfo);
					list.Add(item);
				}
				catch
				{
				}
			}
			return list;
		}

		public bool HasComponent(DataFileType type, CultureInfo srcCulture)
		{
			return HasComponent(type, srcCulture, null);
		}

		public bool HasComponent(DataFileType type, CultureInfo srcCulture, CultureInfo trgCulture)
		{
			if (DataLocation.IsBilingualDataType(type) && trgCulture == null)
			{
				throw new ArgumentException("Need target culture for bilingual data type " + type.ToString());
			}
			string componentFileName = GetComponentFileName(type, srcCulture, trgCulture);
			return File.Exists(componentFileName);
		}

		public DataFileInfo2 ExpectComponent(DataFileType type, CultureInfo srcCulture)
		{
			return ExpectComponent(type, srcCulture, null);
		}

		public DataFileInfo2 ExpectComponent(DataFileType type, CultureInfo srcCulture, CultureInfo trgCulture)
		{
			if (!HasComponent(type, srcCulture, trgCulture))
			{
				throw new LanguagePlatformException(ErrorCode.DataComponentMissing);
			}
			return new DataFileInfo2(this, type, srcCulture, trgCulture);
		}

		public DataFileInfo2 FindComponent(DataFileType type, CultureInfo srcCulture)
		{
			return FindComponent(type, srcCulture, null);
		}

		public DataFileInfo2 FindComponent(DataFileType type, CultureInfo srcCulture, CultureInfo trgCulture)
		{
			if (IsBilingualDataType(type) && trgCulture == null)
			{
				throw new ArgumentNullException("trgCulture");
			}
			if (srcCulture == null)
			{
				throw new ArgumentNullException("srcCulture");
			}
			return new DataFileInfo2(this, type, srcCulture, trgCulture);
		}

		public string GetComponentFileName(DataFileType t, CultureInfo srcCulture)
		{
			return GetComponentFileName(t, srcCulture, null);
		}

		public string GetComponentFileName(DataFileType t, CultureInfo srcCulture, CultureInfo trgCulture)
		{
			if (!TypeToExtensionMapping.TryGetValue(t, out string value))
			{
				throw new LanguagePlatformException(ErrorCode.InternalError);
			}
			if (srcCulture == null)
			{
				throw new ArgumentNullException("srcCulture required for t=" + t.ToString());
			}
			string format;
			if (DataLocation.IsBilingualDataType(t))
			{
				if (trgCulture == null)
				{
					throw new ArgumentNullException("trgCulture required for t=" + t.ToString());
				}
				format = "{0}{1}{2}_{3}{4}";
			}
			else
			{
				format = "{0}{1}{2}{4}";
			}
			return string.Format(format, Directory.FullName, Path.DirectorySeparatorChar, srcCulture.Name, (trgCulture == null) ? string.Empty : trgCulture.Name, value);
		}
	}
}
