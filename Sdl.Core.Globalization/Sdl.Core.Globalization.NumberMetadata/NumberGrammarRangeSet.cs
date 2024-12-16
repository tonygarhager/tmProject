using Newtonsoft.Json;
using System.Collections.Generic;

namespace Sdl.Core.Globalization.NumberMetadata
{
	public class NumberGrammarRangeSet
	{
		[JsonProperty("ranges")]
		public List<IntNumberGrammarRange> IntNumberGrammarRanges
		{
			get;
			set;
		}

		[JsonProperty("floatranges")]
		public List<DoubleNumberGrammarRange> DoubleNumberGrammarRanges
		{
			get;
			set;
		}

		public string ID
		{
			get;
			set;
		}
	}
}
