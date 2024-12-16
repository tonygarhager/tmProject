using System;
using System.Collections.Generic;
using System.IO;

namespace Sdl.Core.FineGrainedAlignment
{
	internal class TempFileRemover : IDisposable
	{
		private readonly List<string> _filenames = new List<string>();

		private readonly List<string> _directories = new List<string>();

		public void AddFile(string filename)
		{
			_filenames.Add(filename);
		}

		public void AddDirectory(string dirname)
		{
			_directories.Add(dirname);
		}

		public void Dispose()
		{
			foreach (string filename in _filenames)
			{
				try
				{
					File.Delete(filename);
				}
				catch (Exception)
				{
				}
			}
			foreach (string directory in _directories)
			{
				try
				{
					Directory.Delete(directory);
				}
				catch (Exception)
				{
				}
			}
		}
	}
}
