using System;
using System.Threading;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Represents an upgraded file-based TM 
	/// </summary>
	public interface IReindexableTranslationMemory
	{
		/// <summary>
		///  Returns true if any TUs require reindexing, based on the value of their tokenization_signature_hash column, false otherwise, or null if the TM is a legacy file-based TM that does not have this column.
		/// </summary>
		bool? ReindexRequired
		{
			get;
		}

		/// <summary>
		///  Returns the number of TUs that require reindexing, based on the value of their tokenization_signature_hash column, or -1 if the TM is a legacy file-based TM that does not have this column.
		/// </summary>
		int TuCountForReindex
		{
			get;
		}

		/// <summary>
		/// Returns true for file-based TMs capable of reporting whether TUs require reindexing, or false for legacy TMs that do not support this capability.
		/// </summary>
		bool CanReportReindexRequired
		{
			get;
			set;
		}

		/// <summary>
		/// Provides similar functionality to ReindexTranslationUnits, except that only TUs that require reindexing are reindexed, based on the value of their tokenization_signature_hash column, or no TUs if the TM is a legacy file-based TM that does not have this column.
		/// </summary>
		/// <param name="token"></param>
		/// <param name="progress"></param>
		void SelectiveReindexTranslationUnits(CancellationToken token, IProgress<int> progress);
	}
}
