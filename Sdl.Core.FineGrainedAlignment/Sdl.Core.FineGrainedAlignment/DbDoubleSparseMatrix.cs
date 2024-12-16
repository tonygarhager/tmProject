using Sdl.LanguagePlatform.Stat;
using System;
using System.Collections.Generic;

namespace Sdl.Core.FineGrainedAlignment
{
	internal class DbDoubleSparseMatrix : SparseMatrix<double>
	{
		public event EventHandler<TranslationModelProgressEventArgs> Progress;

		public DbDoubleSparseMatrix()
		{
		}

		public DbDoubleSparseMatrix(SparseMatrix<double> matrix)
			: base(matrix)
		{
		}

		public static DbDoubleSparseMatrix Load(IChiSquaredTranslationModelStoreReadOnly store, bool isReversedMatrix)
		{
			int startAfter = -1;
			DbDoubleSparseMatrix dbDoubleSparseMatrix = new DbDoubleSparseMatrix();
			do
			{
				List<TranslationModelMatrixEntry> list = store.LoadMatrixData(ref startAfter, 1000, isReversedMatrix);
				foreach (TranslationModelMatrixEntry item in list)
				{
					dbDoubleSparseMatrix[item.SourceKey, item.TargetKey] = item.Value;
				}
			}
			while (startAfter != -1);
			return dbDoubleSparseMatrix;
		}

		public void Save(IChiSquaredTranslationModelStore store, bool isReversedMatrix)
		{
			store.WriteMatrixData(Entries(), isReversedMatrix);
		}

		private IEnumerable<TranslationModelMatrixEntry> Entries()
		{
			int count = 0;
			foreach (int rowkey in base.RowKeys)
			{
				foreach (int item in ColumnKeys(rowkey))
				{
					double num = base[rowkey, item];
					if (num > 0.0)
					{
						TranslationModelMatrixEntry translationModelMatrixEntry = new TranslationModelMatrixEntry();
						translationModelMatrixEntry.SourceKey = rowkey;
						translationModelMatrixEntry.TargetKey = item;
						translationModelMatrixEntry.Value = num;
						count++;
						if (count % 1000 == 0)
						{
							OnProgress(TranslationModelProgressStage.Saving, count);
						}
						yield return translationModelMatrixEntry;
					}
				}
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
