namespace Sdl.Core.LanguageProcessing.Stemming
{
	public class StemmingRule
	{
		public enum StemAction
		{
			None,
			Prefix,
			Suffix,
			Infix,
			ProperInfix,
			Circumfix,
			Form,
			MapToLower,
			StripDiacritics,
			DeleteLastDoubleConsonants,
			DeleteLastDoubleVowels,
			TestOnBaseWord,
			PrefixedInfix,
			Version
		}

		public enum StemContinuation
		{
			Continue,
			Restart,
			Stop
		}

		public StemContinuation ContinuationOnSuccess
		{
			get;
			set;
		}

		public StemAction Action
		{
			get;
			set;
		}

		public StemContinuation ContinuationOnFail
		{
			get;
			set;
		}

		public int Length
		{
			get;
			set;
		}

		public string Affix
		{
			get;
			set;
		}

		public string Replacement
		{
			get;
			set;
		}

		public int Priority
		{
			get;
			set;
		}

		public int ContinuationPriority
		{
			get;
			set;
		}

		public static int Compare(StemmingRule a, StemmingRule b)
		{
			int num = b.Priority - a.Priority;
			if (num == 0)
			{
				num = b.Length - a.Length;
			}
			return num;
		}

		public StemmingRule()
		{
			Action = StemAction.None;
		}

		public override string ToString()
		{
			return $"stemAction={Action};Affix={Affix};replacement={Replacement};priority={Priority} ";
		}
	}
}
