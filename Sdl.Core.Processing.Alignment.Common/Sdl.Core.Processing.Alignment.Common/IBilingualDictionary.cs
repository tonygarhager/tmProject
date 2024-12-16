using System.Collections.Generic;
using System.Globalization;

namespace Sdl.Core.Processing.Alignment.Common
{
	public interface IBilingualDictionary
	{
		CultureInfo SourceCulture
		{
			get;
		}

		CultureInfo TargetCulture
		{
			get;
		}

		bool IsReadOnly
		{
			get;
		}

		IEnumerable<string> GetSourceWords();

		IEnumerable<string> GetTargetWords(string sourceWord);

		bool CanAddEntry(string sourceWord, string targetWord);

		void AddEntry(string sourceWord, string targetWord);
	}
}
