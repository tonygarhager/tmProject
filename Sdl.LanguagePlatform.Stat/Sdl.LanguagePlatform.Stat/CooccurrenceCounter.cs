using System;
using System.Collections.Generic;
using System.IO;

namespace Sdl.LanguagePlatform.Stat
{
	public class CooccurrenceCounter : IDisposable
	{
		private readonly DiskBasedMergeCounter<CooccurrenceCount> _counter;

		public int Samples
		{
			get;
			private set;
		}

		public CooccurrenceCounter(string tempFolder)
			: this(tempFolder, 4194304)
		{
		}

		public CooccurrenceCounter(string tempFolder, int itemsPerFile)
		{
			if (itemsPerFile < 1024)
			{
				itemsPerFile = 4194304;
			}
			_counter = new DiskBasedMergeCounter<CooccurrenceCount>(new CooccurrenceCountBinaryReaderWriter(), "coc", tempFolder, itemsPerFile);
			Samples = 0;
		}

		public void Count(int v1, int v2)
		{
			_counter.Count(new CooccurrenceCount(v1, v2, 1));
			int num = ++Samples;
		}

		public IEnumerable<CooccurrenceCount> Merge(int minCooc)
		{
			string fn = _counter.FinishCounting();
			return ComputeResult(minCooc, fn);
		}

		private static IEnumerable<CooccurrenceCount> ComputeResult(int minCooc, string fn1)
		{
			try
			{
				using (FileStream f = File.Open(fn1, FileMode.Open, FileAccess.Read))
				{
					_ = f.Length;
					int items = (int)(f.Length / 12);
					BinaryReader rf = new BinaryReader(f);
					int p = 0;
					while (p < items)
					{
						int s = rf.ReadInt32();
						int t = rf.ReadInt32();
						int num = rf.ReadInt32();
						if (num >= minCooc)
						{
							yield return new CooccurrenceCount(s, t, num);
						}
						int num2 = p + 1;
						p = num2;
					}
				}
			}
			finally
			{
				File.Delete(fn1);
			}
		}

		public void Dispose()
		{
		}
	}
}
