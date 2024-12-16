using Sdl.Core.Bcm.BcmModel.Common;
using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmModel
{
	[DataContract]
	public class LocalizableSubContent : ExtensionDataContainer, ICloneable<LocalizableSubContent>
	{
		[DataMember(Name = "sourceTagContentOffset")]
		public int SourceTagContentOffset
		{
			get;
			set;
		}

		[DataMember(Name = "length")]
		public int Length
		{
			get;
			set;
		}

		[DataMember(Name = "paragraphUnitId")]
		public string ParagraphUnitId
		{
			get;
			set;
		}

		public LocalizableSubContent Clone()
		{
			return (LocalizableSubContent)MemberwiseClone();
		}

		protected bool Equals(LocalizableSubContent other)
		{
			if (SourceTagContentOffset == other.SourceTagContentOffset && Length == other.Length)
			{
				return string.Equals(ParagraphUnitId, other.ParagraphUnitId);
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
			if (obj.GetType() == GetType())
			{
				return Equals((LocalizableSubContent)obj);
			}
			return false;
		}

		public override int GetHashCode()
		{
			int sourceTagContentOffset = SourceTagContentOffset;
			sourceTagContentOffset = ((sourceTagContentOffset * 397) ^ Length);
			return (sourceTagContentOffset * 397) ^ ((ParagraphUnitId != null) ? ParagraphUnitId.GetHashCode() : 0);
		}
	}
}
