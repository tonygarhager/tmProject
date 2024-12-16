using System;

namespace Snowball
{
	public sealed class Among
	{
		public string SearchString
		{
			get;
			private set;
		}

		public int MatchIndex
		{
			get;
			private set;
		}

		public int Result
		{
			get;
			private set;
		}

		public Func<bool> Action
		{
			get;
			private set;
		}

		public Among(string str, int index, int result)
			: this(str, index, result, null)
		{
		}

		public Among(string str, int index, int result, Func<bool> action)
		{
			SearchString = str;
			MatchIndex = index;
			Result = result;
			Action = action;
		}

		public override string ToString()
		{
			return SearchString;
		}
	}
}
