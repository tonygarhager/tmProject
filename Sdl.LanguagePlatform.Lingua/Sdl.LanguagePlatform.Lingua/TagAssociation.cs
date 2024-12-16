using Sdl.LanguagePlatform.Core.EditDistance;

namespace Sdl.LanguagePlatform.Lingua
{
	public class TagAssociation
	{
		public PairedTag SourceTag;

		public PairedTag TargetTag;

		public EditOperation Operation;

		public TagAssociation(PairedTag sourceTag, PairedTag targetTag)
			: this(sourceTag, targetTag, EditOperation.Undefined)
		{
		}

		public TagAssociation(PairedTag sourceTag, PairedTag targetTag, EditOperation op)
		{
			SourceTag = sourceTag;
			TargetTag = targetTag;
			Operation = op;
		}

		public override string ToString()
		{
			return string.Format("{0} <-> {1}, {2}", (SourceTag == null) ? "(null)" : SourceTag.ToString(), (TargetTag == null) ? "(null)" : TargetTag.ToString(), Operation);
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
			TagAssociation tagAssociation = obj as TagAssociation;
			bool flag = tagAssociation != null && Operation == tagAssociation.Operation;
			if (flag)
			{
				flag = (SourceTag?.Equals(tagAssociation.SourceTag) ?? (tagAssociation.SourceTag == null));
			}
			if (flag)
			{
				flag = (TargetTag?.Equals(tagAssociation.TargetTag) ?? (tagAssociation.TargetTag == null));
			}
			return flag;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
