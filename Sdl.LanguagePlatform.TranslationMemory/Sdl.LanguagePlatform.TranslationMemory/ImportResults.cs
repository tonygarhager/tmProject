using Sdl.LanguagePlatform.Core;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	public class ImportResults : ImportStatistics, IEnumerable<ImportResult>, IEnumerable
	{
		private readonly IList<ImportResult> _importResults;

		public ImportResult this[int index]
		{
			get
			{
				return _importResults[index];
			}
			set
			{
				_importResults[index] = value;
			}
		}

		public int Count
		{
			get
			{
				IList<ImportResult> importResults = _importResults;
				if (importResults == null)
				{
					return 0;
				}
				return importResults.Count;
			}
		}

		public ImportResults()
		{
			_importResults = new List<ImportResult>();
		}

		public ImportResults(IList<ImportResult> importResults)
		{
			_importResults = importResults;
			if (_importResults == null)
			{
				return;
			}
			base.TotalRead = _importResults.Count;
			for (int i = 0; i < _importResults.Count; i++)
			{
				if (importResults[i] != null)
				{
					switch (importResults[i].Action)
					{
					case Action.Discard:
					{
						int num = ++base.DiscardedTranslationUnits;
						break;
					}
					case Action.Add:
					{
						int num = ++base.AddedTranslationUnits;
						break;
					}
					case Action.Merge:
					{
						int num = ++base.MergedTranslationUnits;
						break;
					}
					case Action.Overwrite:
					{
						int num = ++base.OverwrittenTranslationUnits;
						break;
					}
					case Action.Error:
					{
						int num = ++base.Errors;
						break;
					}
					case Action.Delete:
					{
						int num = ++base.DeletedTranslationUnits;
						break;
					}
					default:
						throw new LanguagePlatformException(ErrorCode.InternalError);
					}
				}
			}
		}

		public IEnumerator<ImportResult> GetEnumerator()
		{
			return _importResults?.GetEnumerator() ?? throw new InvalidOperationException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _importResults?.GetEnumerator() ?? throw new InvalidOperationException();
		}
	}
}
