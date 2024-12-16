using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Sdl.LanguagePlatform.IO
{
	public class MultiFileTextReader : TextReader, IDisposable
	{
		private List<FileInfo> _Files;

		private int _CurrentFile;

		private TextReader _CurrentReader;

		public FileInfo CurrentFile
		{
			get
			{
				if (_CurrentFile < _Files.Count)
				{
					return _Files[_CurrentFile];
				}
				return null;
			}
		}

		public MultiFileTextReader(params FileInfo[] files)
		{
			_Files = new List<FileInfo>();
			_Files.AddRange(files);
			NextFile();
		}

		public MultiFileTextReader(DirectoryInfo rootDirectory, string fileFilter, bool recurse)
		{
			_Files = new List<FileInfo>();
			CollectFiles(rootDirectory, fileFilter, recurse);
			NextFile();
		}

		public MultiFileTextReader(params string[] filesOrDirectories)
		{
			_Files = new List<FileInfo>();
			foreach (string obj in filesOrDirectories)
			{
				FileInfo fileInfo = new FileInfo(obj);
				DirectoryInfo directoryInfo = new DirectoryInfo(obj);
				if (fileInfo.Exists)
				{
					_Files.Add(fileInfo);
				}
				else if (directoryInfo.Exists)
				{
					CollectFiles(directoryInfo, "*.txt", recurse: true);
				}
			}
			NextFile();
		}

		private void CollectFiles(DirectoryInfo root, string fileFilter, bool recurse)
		{
			if (string.IsNullOrEmpty(fileFilter))
			{
				_Files.AddRange(root.GetFiles());
			}
			else
			{
				_Files.AddRange(root.GetFiles(fileFilter));
			}
			if (recurse)
			{
				DirectoryInfo[] directories = root.GetDirectories();
				foreach (DirectoryInfo root2 in directories)
				{
					CollectFiles(root2, fileFilter, recurse);
				}
			}
		}

		private bool NextFile()
		{
			if (_CurrentReader != null)
			{
				_CurrentReader.Close();
				_CurrentReader.Dispose();
				_CurrentReader = null;
			}
			if (_CurrentFile < _Files.Count)
			{
				_CurrentReader = new StreamReader(_Files[_CurrentFile].FullName, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
				_CurrentFile++;
				return true;
			}
			return false;
		}

		public override int Peek()
		{
			throw new NotImplementedException("This method should not be called on this class");
		}

		public override int Read()
		{
			if (_CurrentReader == null)
			{
				throw new EndOfStreamException();
			}
			int result;
			while ((result = _CurrentReader.Read()) < 0)
			{
				if (!NextFile())
				{
					return -1;
				}
			}
			return result;
		}

		public override int Read(char[] buffer, int index, int count)
		{
			if (_CurrentReader == null)
			{
				throw new EndOfStreamException();
			}
			while (_CurrentReader.Peek() < 0)
			{
				if (!NextFile())
				{
					return 0;
				}
			}
			return _CurrentReader.Read(buffer, index, count);
		}

		public override int ReadBlock(char[] buffer, int index, int count)
		{
			if (_CurrentReader == null)
			{
				throw new EndOfStreamException();
			}
			int num = 0;
			while (count > 0)
			{
				int num2 = Read(buffer, index + num, count);
				count -= num2;
				num += num2;
				if (count > 0)
				{
					NextFile();
					if (_CurrentReader == null)
					{
						break;
					}
				}
			}
			return num;
		}

		public override string ReadLine()
		{
			if (_CurrentReader == null)
			{
				throw new EndOfStreamException();
			}
			while (_CurrentReader.Peek() < 0)
			{
				if (!NextFile())
				{
					return null;
				}
			}
			return _CurrentReader.ReadLine();
		}

		public override string ReadToEnd()
		{
			throw new NotImplementedException("This method should not be called on this class");
		}

		public override void Close()
		{
			if (_CurrentReader != null)
			{
				_CurrentReader.Close();
				_CurrentReader.Dispose();
				_CurrentReader = null;
			}
			_CurrentFile = _Files.Count;
		}

		void IDisposable.Dispose()
		{
			if (_CurrentReader != null)
			{
				Close();
			}
		}
	}
}
