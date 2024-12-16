using Sdl.LanguagePlatform.Stat;
using System;
using System.Collections.Generic;
using System.IO;

namespace Sdl.Core.FineGrainedAlignment
{
	internal class DiskBasedMergeCounter2<T> where T : IComparable<T>, ICountable
	{
		private readonly IBinaryReaderWriter2<T> _ReaderWriter;

		private int _Count;

		private readonly int _MaxMemoryItems;

		private readonly TempFileGenerator2 _TempFileGenerator;

		private bool _FinishedCounting;

		private readonly List<T> _Items;

		private const int _ReportProgressPeriod = 100000;

		public int Items => _Count;

		public event EventHandler<TranslationModelProgressEventArgs> Progress;

		public DiskBasedMergeCounter2(IBinaryReaderWriter2<T> readerWriter, string tempFilePrefix, string tempFileLocation, int maxMemoryItems)
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
			_MaxMemoryItems = maxMemoryItems;
			_ReaderWriter = readerWriter;
			_TempFileGenerator = new TempFileGenerator2(tempFileLocation, tempFilePrefix ?? string.Empty);
			_Count = 0;
			_Items = new List<T>();
			_FinishedCounting = false;
		}

		public void Count(T item)
		{
			if (_FinishedCounting)
			{
				throw new Exception("Finished counting");
			}
			_Items.Add(item);
			_Count++;
			try
			{
				if (_Items.Count >= _MaxMemoryItems)
				{
					Flush();
				}
			}
			catch
			{
				foreach (string fileName in _TempFileGenerator.FileNames)
				{
					SafeDelete(fileName);
				}
				throw;
			}
		}

		public string FinishCounting()
		{
			_FinishedCounting = true;
			try
			{
				Flush();
				Merge();
				if (_TempFileGenerator.FileNames.Count > 0)
				{
					return _TempFileGenerator.FileNames[0];
				}
				return null;
			}
			catch
			{
				foreach (string fileName in _TempFileGenerator.FileNames)
				{
					SafeDelete(fileName);
				}
				throw;
			}
		}

		private void Flush()
		{
			if (_Items.Count > 0)
			{
				_Items.Sort();
				using (BinaryWriter writer = new BinaryWriter(File.Create(_TempFileGenerator.GetNextTempFileName())))
				{
					int j;
					for (int i = 0; i < _Items.Count; i += j)
					{
						T item = _Items[i];
						for (j = 1; i + j < _Items.Count && item.CompareTo(_Items[i + j]) == 0; j++)
						{
						}
						item.Count = j;
						_ReaderWriter.Write(writer, item);
					}
				}
				_Items.Clear();
			}
		}

		private void Merge()
		{
			int num = 0;
			while (_TempFileGenerator.FileNames.Count > 1)
			{
				string path = _TempFileGenerator.FileNames[0];
				string path2 = _TempFileGenerator.FileNames[1];
				_TempFileGenerator.FileNames.RemoveRange(0, 2);
				string nextTempFileName = _TempFileGenerator.GetNextTempFileName();
				using (FileStream fileStream = File.OpenRead(path))
				{
					long length = fileStream.Length;
					using (FileStream fileStream2 = File.OpenRead(path2))
					{
						long length2 = fileStream2.Length;
						using (FileStream output = File.Create(nextTempFileName))
						{
							BinaryReader reader = new BinaryReader(fileStream);
							BinaryReader reader2 = new BinaryReader(fileStream2);
							BinaryWriter writer = new BinaryWriter(output);
							T item;
							bool flag = _ReaderWriter.Read(reader, out item);
							T item2;
							bool flag2 = _ReaderWriter.Read(reader2, out item2);
							while (flag && flag2)
							{
								int num2 = item.CompareTo(item2);
								if (num2 < 0)
								{
									_ReaderWriter.Write(writer, item);
									flag = _ReaderWriter.Read(reader, out item);
								}
								else if (num2 > 0)
								{
									_ReaderWriter.Write(writer, item2);
									flag2 = _ReaderWriter.Read(reader2, out item2);
								}
								else
								{
									item.Count += item2.Count;
									_ReaderWriter.Write(writer, item);
									flag = _ReaderWriter.Read(reader, out item);
									flag2 = _ReaderWriter.Read(reader2, out item2);
								}
								num++;
								if (num % 100000 == 0)
								{
									OnProgress(TranslationModelProgressStage.Merging, num);
								}
							}
							while (flag)
							{
								_ReaderWriter.Write(writer, item);
								flag = _ReaderWriter.Read(reader, out item);
								num++;
								if (num % 100000 == 0)
								{
									OnProgress(TranslationModelProgressStage.Merging, num);
								}
							}
							while (flag2)
							{
								_ReaderWriter.Write(writer, item2);
								flag2 = _ReaderWriter.Read(reader2, out item2);
								num++;
								if (num % 100000 == 0)
								{
									OnProgress(TranslationModelProgressStage.Merging, num);
								}
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

		private void OnProgress(TranslationModelProgressStage progressStage)
		{
			OnProgress(progressStage, 0);
		}

		private void OnProgress(TranslationModelProgressStage progressStage, int progressNumber)
		{
			TranslationModelProgressEventArgs progressEventArgs = new TranslationModelProgressEventArgs(progressStage, progressNumber, -1);
			OnProgress(progressEventArgs);
		}

		private void OnProgress(TranslationModelProgressStage progressStage, int progressNumber, int progressLimit)
		{
			TranslationModelProgressEventArgs progressEventArgs = new TranslationModelProgressEventArgs(progressStage, progressNumber, progressLimit);
			OnProgress(progressEventArgs);
		}

		private void OnProgress(TranslationModelProgressEventArgs progressEventArgs)
		{
			OnProgress(this, progressEventArgs);
		}

		private void OnProgress(object sender, TranslationModelProgressEventArgs progressEventArgs)
		{
			if (this.Progress != null)
			{
				this.Progress(this, progressEventArgs);
				if (progressEventArgs.Cancel)
				{
					throw new TranslationModelCancelException();
				}
			}
		}
	}
}
