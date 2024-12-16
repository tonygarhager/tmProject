using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sdl.Core.LanguageProcessing.Segmentation
{
	public class WordStopStringBuilder
	{
		public const char WordStop = ' ';

		private readonly StringBuilder _stringBuilder = new StringBuilder();

		private readonly IList<int> _wordStopIndexes = new List<int>();

		public void Append(string text)
		{
			_stringBuilder.Append(text);
		}

		public void AppendWordStop()
		{
			_stringBuilder.Append(' ');
			int item = _stringBuilder.Length - 1;
			_wordStopIndexes.Add(item);
		}

		public int GetIndexWithoutWordStops(int indexWithWordStops)
		{
			return indexWithWordStops - _wordStopIndexes.Count((int wordStopIndex) => wordStopIndex <= indexWithWordStops);
		}

		public override string ToString()
		{
			return _stringBuilder.ToString();
		}
	}
}
