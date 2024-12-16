using System;
using System.Runtime.Serialization;

namespace Sdl.Core.LanguageProcessing.Tokenization
{
	[Serializable]
	public class SeparatorException : Exception
	{
		protected SeparatorException(SerializationInfo i, StreamingContext c)
			: base(i, c)
		{
		}

		public SeparatorException()
			: base("DecimalSeparators and GroupSeparators must both be strings of length 1")
		{
		}
	}
}
