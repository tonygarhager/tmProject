using Sdl.Core.Bcm.BcmModel.Collections;
using Sdl.Core.Bcm.BcmModel.Common;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmModel.Skeleton
{
	[DataContract]
	public class FormattingGroup : SkeletonItem, ICloneable<FormattingGroup>, IEquatable<FormattingGroup>
	{
		[DataMember(Name = "items")]
		public DictionaryEx<string, string> Items
		{
			get;
			set;
		}

		public FormattingGroup()
		{
			Items = new DictionaryEx<string, string>();
		}

		public FormattingGroup Clone()
		{
			FormattingGroup formattingGroup = (FormattingGroup)MemberwiseClone();
			formattingGroup.ReplaceMetadataWith(base.Metadata);
			if (Items != null)
			{
				formattingGroup.Items = new DictionaryEx<string, string>(Items);
			}
			return formattingGroup;
		}

		public bool Equals(FormattingGroup other)
		{
			if (other == null)
			{
				return false;
			}
			if (this == other)
			{
				return true;
			}
			if (Equals((SkeletonItem)other))
			{
				return object.Equals(Items, other.Items);
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (this == obj)
			{
				return true;
			}
			if (obj.GetType() != GetType())
			{
				return false;
			}
			return Equals((FormattingGroup)obj);
		}

		public override int GetHashCode()
		{
			return (base.GetHashCode() * 397) ^ ((Items != null) ? Items.GetHashCode() : 0);
		}

		public bool ShouldSerializeItems()
		{
			if (Items != null)
			{
				return Items.Any();
			}
			return false;
		}
	}
}
