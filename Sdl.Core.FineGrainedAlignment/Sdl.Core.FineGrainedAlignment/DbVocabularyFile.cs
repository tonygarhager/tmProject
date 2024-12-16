using System;
using System.Collections.Generic;

namespace Sdl.Core.FineGrainedAlignment
{
	internal class DbVocabularyFile : VocabularyFile3
	{
		private readonly IChiSquaredTranslationModelStoreReadOnly _store;

		private readonly bool _target;

		public event EventHandler<TranslationModelProgressEventArgs> Progress;

		public DbVocabularyFile(IChiSquaredTranslationModelStoreReadOnly store, bool target)
		{
			if (store == null)
			{
				throw new ArgumentNullException("store");
			}
			_store = store;
			_target = target;
		}

		public override void Load()
		{
			if (_KeyWordMap.Count <= 0)
			{
				List<TranslationModelVocabEntry> list = _store.LoadVocab(_target, null);
				foreach (TranslationModelVocabEntry item in list)
				{
					Add(item.Key, item.Token);
				}
				LookupSpecialTokenIDs();
				_Dirty = false;
				_contiguousKeys = false;
			}
		}

		private IEnumerable<TranslationModelVocabEntry> Entries()
		{
			int count = 0;
			foreach (KeyValuePair<int, TokenWithCount> item in _KeyWordMap)
			{
				TranslationModelVocabEntry translationModelVocabEntry = new TranslationModelVocabEntry();
				translationModelVocabEntry.Key = item.Key;
				translationModelVocabEntry.Token = item.Value.Token;
				translationModelVocabEntry.Occurrences = item.Value.Count;
				count++;
				if (count % 1000 == 0)
				{
					OnProgress(TranslationModelProgressStage.Saving, count);
				}
				yield return translationModelVocabEntry;
			}
		}

		public void IncFrequency(int key)
		{
			_KeyWordMap[key].Count++;
		}

		public void Save(IChiSquaredTranslationModelStore store)
		{
			if (_Dirty)
			{
				store.SaveVocab(_target, Entries());
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
