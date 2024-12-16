using Sdl.LanguagePlatform.Core;
using System;
using System.Globalization;
using System.IO;

namespace Sdl.LanguagePlatform.Stat
{
	public class DataFileInfo2
	{
		private readonly DataFileType _dataType;

		public DataLocation2 Location
		{
			get;
			set;
		}

		public string FileName => Location.GetComponentFileName(_dataType, SourceCulture, TargetCulture);

		public bool Exists => File.Exists(FileName);

		public DataFileType DataType => _dataType;

		public CultureInfo SourceCulture
		{
			get;
		}

		public CultureInfo TargetCulture
		{
			get;
		}

		public DataFileInfo2(DataLocation2 location, DataFileType t, CultureInfo srcCulture, CultureInfo trgCulture)
		{
			_dataType = t;
			SourceCulture = srcCulture;
			TargetCulture = trgCulture;
			Location = location;
		}

		public DataFileInfo2(string fileName)
			: this(new FileInfo(fileName))
		{
		}

		public DataFileInfo2(FileInfo fileInfo)
		{
			_dataType = DataFileType.Unknown;
			SourceCulture = (TargetCulture = null);
			if (!fileInfo.Exists)
			{
				throw new FileNotFoundException(fileInfo.FullName);
			}
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.Name);
			string text = null;
			if (!DataLocation.ExtensionToTypeMapping.TryGetValue(fileInfo.Extension.ToLowerInvariant(), out _dataType))
			{
				throw new LanguagePlatformException(ErrorCode.InternalError);
			}
			Location = new DataLocation2(fileInfo.Directory);
			string name;
			if (DataLocation.IsBilingualDataType(_dataType))
			{
				int num = fileNameWithoutExtension.IndexOf("_", StringComparison.Ordinal);
				if (num <= 0)
				{
					throw new LanguagePlatformException(ErrorCode.InternalError);
				}
				name = fileNameWithoutExtension.Substring(0, num);
				text = fileNameWithoutExtension.Substring(num + 1);
			}
			else
			{
				name = fileNameWithoutExtension;
			}
			SourceCulture = CultureInfoExtensions.GetCultureInfo(name);
			if (text != null)
			{
				TargetCulture = CultureInfoExtensions.GetCultureInfo(text);
			}
			if (DataLocation.IsBilingualDataType(_dataType) && TargetCulture == null)
			{
				throw new ArgumentException("Bilingual data file type requires target culture to be set");
			}
		}
	}
}
