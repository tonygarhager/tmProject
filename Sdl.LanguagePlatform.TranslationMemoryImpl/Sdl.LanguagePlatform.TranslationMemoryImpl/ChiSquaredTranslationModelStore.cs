using Sdl.Core.FineGrainedAlignment;
using System;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	internal class ChiSquaredTranslationModelStore : IChiSquaredTranslationModelStore, IChiSquaredTranslationModelStoreReadOnly
	{
		private readonly int _modelId;

		private ITranslationModelStorage ModelStorage
		{
			get;
		}

		public DateTime? TranslationModelDate => ModelStorage.GetTranslationModelDate(_modelId);

		public int SampleCount => ModelStorage.GetSampleCount(_modelId);

		public ChiSquaredTranslationModelStore(ITranslationModelStorage modelStorage, int modelId)
		{
			ModelStorage = modelStorage;
			_modelId = modelId;
		}

		public void ClearTranslationModel()
		{
			ModelStorage.ClearTranslationModel(_modelId);
		}

		public void SetTranslationModelDate(DateTime? date)
		{
			ModelStorage.SetTranslationModelDate(_modelId, date);
		}

		public void SetSampleCount(int count)
		{
			ModelStorage.SetSampleCount(_modelId, count);
		}

		public List<TranslationModelVocabEntry> LoadVocab(bool target, IEnumerable<string> tokensToLoad)
		{
			return ModelStorage.LoadTranslationModelVocab(_modelId, target, tokensToLoad);
		}

		public int TotalVocabSize(bool target)
		{
			return ModelStorage.TotalVocabSize(_modelId, target);
		}

		public void SaveVocab(bool target, IEnumerable<TranslationModelVocabEntry> vocab)
		{
			List<TranslationModelVocabEntry> list = new List<TranslationModelVocabEntry>();
			int num = 0;
			foreach (TranslationModelVocabEntry item in vocab)
			{
				list.Add(item);
				num++;
				if (num % 1000 == 0)
				{
					ModelStorage.SaveTranslationModelVocab(_modelId, target, list);
					ModelStorage.Flush();
					ModelStorage.CommitTransaction();
					list.Clear();
				}
			}
			ModelStorage.SaveTranslationModelVocab(_modelId, target, list);
			ModelStorage.Flush();
			ModelStorage.CommitTransaction();
		}

		public List<TranslationModelMatrixEntry> LoadMatrixData(HashSet<int> sourceKeys, HashSet<int> targetKeys, bool isReversedMatrix)
		{
			return ModelStorage.ReadTranslationModelData(_modelId, sourceKeys, targetKeys, isReversedMatrix);
		}

		public List<TranslationModelMatrixEntry> LoadMatrixData(ref int startAfter, int count, bool isReversedMatrix)
		{
			return ModelStorage.ReadTranslationModelData(_modelId, ref startAfter, count, isReversedMatrix);
		}

		public void WriteMatrixData(IEnumerable<TranslationModelMatrixEntry> entries, bool isReversedMatrix)
		{
			int num = 0;
			List<TranslationModelMatrixEntry> list = new List<TranslationModelMatrixEntry>();
			foreach (TranslationModelMatrixEntry entry in entries)
			{
				list.Add(entry);
				num++;
				if (num % 1000 == 0)
				{
					ModelStorage.WriteTranslationModelData(_modelId, list, isReversedMatrix);
					ModelStorage.Flush();
					ModelStorage.CommitTransaction();
					list.Clear();
				}
			}
			ModelStorage.WriteTranslationModelData(_modelId, list, isReversedMatrix);
			ModelStorage.Flush();
			ModelStorage.CommitTransaction();
		}
	}
}
