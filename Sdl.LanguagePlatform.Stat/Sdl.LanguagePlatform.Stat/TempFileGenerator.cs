using System;
using System.Collections.Generic;
using System.IO;

namespace Sdl.LanguagePlatform.Stat
{
	internal class TempFileGenerator
	{
		private readonly string _tempFileNameBase;

		private readonly string _tempFolder;

		public List<string> FileNames
		{
			get;
		}

		public int NextFileNumber
		{
			get;
			private set;
		}

		public TempFileGenerator(string location, string tempFilePrefix)
			: this(new DirectoryInfo(location), tempFilePrefix)
		{
		}

		public TempFileGenerator(FileSystemInfo location, string tempFilePrefix)
		{
			if (location == null)
			{
				throw new ArgumentNullException();
			}
			if (!location.Exists)
			{
				throw new DirectoryNotFoundException(location.FullName);
			}
			FileNames = new List<string>();
			_tempFolder = location.FullName;
			_tempFileNameBase = tempFilePrefix + Math.Abs(Environment.TickCount).ToString();
			NextFileNumber = 0;
		}

		public void Clear()
		{
			FileNames.Clear();
			NextFileNumber = 0;
		}

		public string GetNextTempFileName()
		{
			string text = $"{_tempFolder}{Path.DirectorySeparatorChar}{_tempFileNameBase}{NextFileNumber:00000000}.tmp";
			NextFileNumber++;
			FileNames.Add(text);
			return text;
		}
	}
}
