using Sdl.LanguagePlatform.Core.EditDistance;

namespace Sdl.LanguagePlatform.Lingua
{
	public class PairedTag
	{
		public int Start;

		public int End;

		public int Anchor;

		public EditOperation StartTagOperation = EditOperation.Undefined;

		public EditOperation EndTagOperation = EditOperation.Undefined;

		public PairedTag(int s, int e, int anchor)
		{
			Start = s;
			End = e;
			Anchor = anchor;
		}

		public override string ToString()
		{
			return $"({Start},{End};i={Anchor})";
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj.GetType() != GetType())
			{
				return false;
			}
			PairedTag pairedTag = obj as PairedTag;
			if (pairedTag != null && pairedTag.Start == Start && pairedTag.End == End)
			{
				return pairedTag.Anchor == Anchor;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
