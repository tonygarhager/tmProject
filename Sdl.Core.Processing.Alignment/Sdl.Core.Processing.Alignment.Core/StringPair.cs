using System;

namespace Sdl.Core.Processing.Alignment.Core
{
	internal class StringPair
	{
		public string LeftString
		{
			get;
			private set;
		}

		public string RightString
		{
			get;
			private set;
		}

		public StringPair(string leftString, string rightString)
		{
			if (leftString == null)
			{
				throw new ArgumentNullException("leftString");
			}
			if (rightString == null)
			{
				throw new ArgumentNullException("rightString");
			}
			LeftString = leftString;
			RightString = rightString;
		}

		public override bool Equals(object obj)
		{
			StringPair stringPair = obj as StringPair;
			if (stringPair != null && object.Equals(stringPair.LeftString, LeftString))
			{
				return object.Equals(stringPair.RightString, RightString);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return 13 + 157 * LeftString.GetHashCode() + 9137 * RightString.GetHashCode();
		}

		public override string ToString()
		{
			return LeftString + "|" + RightString;
		}
	}
}
