using System;
using System.Collections.Generic;
using System.IO;

namespace Sdl.LanguagePlatform.Stat
{
	internal class DiskBasedMergeSorter<T> where T : IComparable<T>
	{
		private readonly IBinaryReaderWriter<T> _readerWriter;

		private readonly int _maxMemoryItems;

		private readonly TempFileGenerator _tempFileGenerator;

		private bool _finishedCounting;

		private readonly bool _unique;

		private readonly List<T> _items;

		public int Items
		{
			get;
			private set;
		}

		public DiskBasedMergeSorter(IBinaryReaderWriter<T> readerWriter, bool unique, string tempFilePrefix, string tempFileLocation, int maxMemoryItems)
		{
			if (readerWriter == null)
			{
				throw new ArgumentNullException("readerWriter");
			}
			if (string.IsNullOrEmpty(tempFileLocation) || !Directory.Exists(tempFileLocation))
			{
				throw new DirectoryNotFoundException(tempFileLocation);
			}
			if (maxMemoryItems <= 0)
			{
				maxMemoryItems = 16384;
			}
			_unique = unique;
			_maxMemoryItems = maxMemoryItems;
			_readerWriter = readerWriter;
			_tempFileGenerator = new TempFileGenerator(tempFileLocation, tempFilePrefix ?? string.Empty);
			Items = 0;
			_items = new List<T>();
			_finishedCounting = false;
		}

		public void Add(T item)
		{
			if (_finishedCounting)
			{
				throw new Exception("Finished counting");
			}
			_items.Add(item);
			int num = ++Items;
			try
			{
				if (_items.Count >= _maxMemoryItems)
				{
					Flush();
				}
			}
			catch
			{
				foreach (string fileName in _tempFileGenerator.FileNames)
				{
					SafeDelete(fileName);
				}
				throw;
			}
		}

		public string FinishCounting()
		{
			_finishedCounting = true;
			try
			{
				Flush();
				Merge();
				return (_tempFileGenerator.FileNames.Count > 0) ? _tempFileGenerator.FileNames[0] : null;
			}
			catch
			{
				foreach (string fileName in _tempFileGenerator.FileNames)
				{
					SafeDelete(fileName);
				}
				throw;
			}
		}

		private void Flush()
		{
			if (_items.Count > 0)
			{
				_items.Sort();
				using (BinaryWriter writer = new BinaryWriter(File.Create(_tempFileGenerator.GetNextTempFileName())))
				{
					int j;
					for (int i = 0; i < _items.Count; i += j)
					{
						T item = _items[i];
						j = 1;
						if (_unique)
						{
							for (; i + j < _items.Count && item.CompareTo(_items[i + j]) == 0; j++)
							{
							}
						}
						_readerWriter.Write(writer, item);
					}
				}
				_items.Clear();
			}
		}

		private void Merge()
		{
			while (_tempFileGenerator.FileNames.Count > 1)
			{
				string path = _tempFileGenerator.FileNames[0];
				string path2 = _tempFileGenerator.FileNames[1];
				_tempFileGenerator.FileNames.RemoveRange(0, 2);
				string nextTempFileName = _tempFileGenerator.GetNextTempFileName();
				using (FileStream input = File.OpenRead(path))
				{
					using (FileStream input2 = File.OpenRead(path2))
					{
						using (FileStream output = File.Create(nextTempFileName))
						{
							BinaryReader reader = new BinaryReader(input);
							BinaryReader reader2 = new BinaryReader(input2);
							BinaryWriter writer = new BinaryWriter(output);
							T item;
							bool flag = _readerWriter.Read(reader, out item);
							T item2;
							bool flag2 = _readerWriter.Read(reader2, out item2);
							while (flag && flag2)
							{
								int num = item.CompareTo(item2);
								if (num < 0)
								{
									_readerWriter.Write(writer, item);
									flag = _readerWriter.Read(reader, out item);
								}
								else if (num > 0)
								{
									_readerWriter.Write(writer, item2);
									flag2 = _readerWriter.Read(reader2, out item2);
								}
								else
								{
									if (_unique)
									{
										_readerWriter.Write(writer, item);
									}
									else
									{
										_readerWriter.Write(writer, item);
										_readerWriter.Write(writer, item2);
									}
									flag = _readerWriter.Read(reader, out item);
									flag2 = _readerWriter.Read(reader2, out item2);
								}
							}
							while (flag)
							{
								_readerWriter.Write(writer, item);
								flag = _readerWriter.Read(reader, out item);
							}
							while (flag2)
							{
								_readerWriter.Write(writer, item2);
								flag2 = _readerWriter.Read(reader2, out item2);
							}
						}
					}
				}
				File.Delete(path);
				File.Delete(path2);
			}
		}

		private static void SafeDelete(string fn)
		{
			try
			{
				File.Delete(fn);
			}
			catch
			{
			}
		}
	}
}
