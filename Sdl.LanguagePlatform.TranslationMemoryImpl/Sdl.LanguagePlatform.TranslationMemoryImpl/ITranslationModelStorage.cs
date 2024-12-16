using Sdl.Core.FineGrainedAlignment;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	public interface ITranslationModelStorage : IDisposable
	{
		void CreateTranslationModelContainerSchema();

		void DropTranslationModelContainerSchema();

		void WriteTranslationModelData(int modelId, IEnumerable<TranslationModelMatrixEntry> entries, bool isReversedMatrix);

		List<TranslationModelMatrixEntry> ReadTranslationModelData(int modelId, ref int startAfter, int count, bool isReversedMatrix);

		List<TranslationModelMatrixEntry> ReadTranslationModelData(int modelId, HashSet<int> sourceKeys, HashSet<int> targetKeys, bool isReversedMatrix);

		void DeleteTranslationModel(int modelId);

		int TotalVocabSize(int modelId, bool target);

		int AddTranslationModel(string name, List<AlignableCorpusId> corpusIds, CultureInfo sourceCulture, CultureInfo targetCulture);

		void UpdateTranslationModel(int modelId, string name, List<AlignableCorpusId> corpusIds);

		void GetTranslationModelDetails(int modelId, out string name, List<AlignableCorpusId> corpusIds, out CultureInfo sourceCulture, out CultureInfo targetCulture, out DateTime? modelDate, out int sampleCount, out int version);

		void GetAllTranslationModelDetails(List<string> names, List<int> modelIds, List<List<AlignableCorpusId>> corpusIdLists, List<CultureInfo> sourceCultures, List<CultureInfo> targetCultures, List<DateTime?> modelDates, List<int> sampleCounts, List<int> versions);

		void ClearTranslationModel(int modelId);

		List<TranslationModelVocabEntry> LoadTranslationModelVocab(int modelId, bool target, IEnumerable<string> tokensToLoad);

		void SaveTranslationModelVocab(int modelId, bool target, IEnumerable<TranslationModelVocabEntry> vocab);

		void Flush();

		void CommitTransaction();

		void AbortTransaction();

		DateTime? GetTranslationModelDate(int modelId);

		void SetTranslationModelDate(int modelId, DateTime? dateTime);

		int GetSampleCount(int modelId);

		void SetSampleCount(int modelId, int sampleCount);

		bool IsFileBased();
	}
}
