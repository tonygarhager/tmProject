using System;
using System.Collections.Generic;
using System.IO;

namespace Sdl.Core.FineGrainedAlignment
{
	internal class TempFileGenerator2
	{
		private readonly DirectoryInfo _Location;

		private readonly List<string> _FileNames;

		private readonly string _TempFileNameBase;

		private readonly string _TempFolder;

		private int _NextFile;

		public List<string> FileNames => _FileNames;

		public int NextFileNumber => _NextFile;

		public TempFileGenerator2(string location, string tempFilePrefix)
			: this(new DirectoryInfo(location), tempFilePrefix)
		{
		}

		public TempFileGenerator2(DirectoryInfo location, string tempFilePrefix)
		{
			if (location == null)
			{
				throw new ArgumentNullException();
			}
			if (!location.Exists)
			{
				throw new DirectoryNotFoundException(location.FullName);
			}
			_FileNames = new List<string>();
			_Location = location;
			_TempFolder = _Location.FullName;
			_TempFileNameBase = tempFilePrefix + Math.Abs(Environment.TickCount).ToString();
			_NextFile = 0;
		}

		public void Clear()
		{
			_FileNames.Clear();
			_NextFile = 0;
		}

		public string GetNextTempFileName()
		{
			string text = $"{_TempFolder}{Path.DirectorySeparatorChar}{_TempFileNameBase}{_NextFile:00000000}.tmp";
			_NextFile++;
			_FileNames.Add(text);
			return text;
		}
	}
}
