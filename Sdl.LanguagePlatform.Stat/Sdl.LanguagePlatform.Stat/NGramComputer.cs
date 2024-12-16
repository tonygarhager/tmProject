using System;
using System.Collections.Generic;
using System.IO;

namespace Sdl.LanguagePlatform.Stat
{
	internal class NGramComputer
	{
		private readonly DiskBasedMergeCounter<NGram> _counter;

		public int N
		{
			get;
		}

		public NGramComputer(int n, string tempDataFolder, string prefix)
		{
			if (n <= 1)
			{
				throw new ArgumentException("n must be > 1");
			}
			if (string.IsNullOrEmpty(prefix))
			{
				prefix = "ng";
			}
			N = n;
			NGramReaderWriter readerWriter = new NGramReaderWriter();
			_counter = new DiskBasedMergeCounter<NGram>(readerWriter, prefix, tempDataFolder, 4194304);
		}

		public void Count(IList<int> ngram)
		{
			if (ngram.Count != N)
			{
				throw new ArgumentException("Invalid ngram width");
			}
			NGram item = new NGram(ngram);
			_counter.Count(item);
		}

		public IEnumerable<NGram> FinishCounting(int minCooc)
		{
			string finalFile = _counter.FinishCounting();
			try
			{
				long num = (N + 2) * 4;
				using (FileStream f = File.Open(finalFile, FileMode.Open, FileAccess.Read))
				{
					_ = f.Length;
					int items = (int)(f.Length / num);
					BinaryReader rf = new BinaryReader(f);
					NGramReaderWriter rw = new NGramReaderWriter();
					int p = 0;
					while (p < items)
					{
						rw.Read(rf, out NGram item);
						if (item.Count >= minCooc)
						{
							yield return item;
						}
						int num2 = p + 1;
						p = num2;
					}
				}
			}
			finally
			{
				File.Delete(finalFile);
			}
		}

		public void FinishAndSave(int minCooc, string outputFileName)
		{
			string path = _counter.FinishCounting();
			try
			{
				long num = (N + 2) * 4;
				using (FileStream fileStream = File.Open(path, FileMode.Open, FileAccess.Read))
				{
					using (FileStream output = File.Create(outputFileName))
					{
						long length = fileStream.Length;
						int num2 = (int)(fileStream.Length / num);
						BinaryReader reader = new BinaryReader(fileStream);
						BinaryWriter writer = new BinaryWriter(output);
						NGramReaderWriter nGramReaderWriter = new NGramReaderWriter();
						for (int i = 0; i < num2; i++)
						{
							NGram item;
							bool flag = nGramReaderWriter.Read(reader, out item);
							if (item.Count >= minCooc)
							{
								nGramReaderWriter.Write(writer, item);
							}
						}
					}
				}
			}
			finally
			{
				File.Delete(path);
			}
		}
	}
}
