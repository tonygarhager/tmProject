using System;
using System.Threading;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public interface IReindexableTranslationMemory
	{
		bool? ReindexRequired
		{
			get;
		}

		int TuCountForReindex
		{
			get;
		}

		bool CanReportReindexRequired
		{
			get;
			set;
		}

		void SelectiveReindexTranslationUnits(CancellationToken token, IProgress<int> progress);
	}
}
