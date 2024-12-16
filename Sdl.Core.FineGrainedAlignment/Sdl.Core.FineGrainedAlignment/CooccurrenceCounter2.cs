using Sdl.LanguagePlatform.Stat;
using System;
using System.Collections.Generic;
using System.IO;

namespace Sdl.Core.FineGrainedAlignment
{
	public class CooccurrenceCounter2 : IDisposable
	{
		private readonly DiskBasedMergeCounter2<CooccurrenceCount> _Counter;

		private int _Samples;

		public int Samples => _Samples;

		public event EventHandler<TranslationModelProgressEventArgs> Progress;

		public CooccurrenceCounter2(string tempFolder)
			: this(tempFolder, 4194304)
		{
		}

		public CooccurrenceCounter2(string tempFolder, int itemsPerFile)
		{
			if (itemsPerFile < 1024)
			{
				itemsPerFile = 4194304;
			}
			_Counter = new DiskBasedMergeCounter2<CooccurrenceCount>(new CooccurrenceCountBinaryReaderWriter2(), "coc", tempFolder, itemsPerFile);
			_Counter.Progress += delegate(object sender, TranslationModelProgressEventArgs args)
			{
				OnProgress(args.ProgressStage, args.ProgressNumber);
			};
			_Samples = 0;
		}

		public void Count(int v1, int v2)
		{
			_Counter.Count(new CooccurrenceCount(v1, v2, 1));
			_Samples++;
		}

		public IEnumerable<CooccurrenceCount> Merge(int minCooc)
		{
			string fn = _Counter.FinishCounting();
			return ComputeResult(minCooc, fn);
		}

		private IEnumerable<CooccurrenceCount> ComputeResult(int minCooc, string fn1)
		{
			try
			{
				long num = 12L;
				using (FileStream f = File.Open(fn1, FileMode.Open, FileAccess.Read))
				{
					_ = f.Length;
					int items = (int)(f.Length / num);
					BinaryReader rf = new BinaryReader(f);
					int p = 0;
					while (p < items)
					{
						int s = rf.ReadInt32();
						int t = rf.ReadInt32();
						int num2 = rf.ReadInt32();
						if (num2 >= minCooc)
						{
							yield return new CooccurrenceCount(s, t, num2);
						}
						int num3 = p + 1;
						p = num3;
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
