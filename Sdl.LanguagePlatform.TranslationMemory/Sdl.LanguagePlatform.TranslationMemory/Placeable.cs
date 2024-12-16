using Sdl.LanguagePlatform.Core;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	public class Placeable
	{
		[DataMember]
		public int SourceTokenIndex
		{
			get;
			set;
		}

		[DataMember]
		public int TargetTokenIndex
		{
			get;
			set;
		}

		[DataMember]
		public PlaceableType Type
		{
			get;
			set;
		}

		public bool IsTag
		{
			get
			{
				if (Type != PlaceableType.PairedTagStart && Type != PlaceableType.PairedTagEnd && Type != PlaceableType.StandaloneTag && Type != PlaceableType.TextPlaceholder)
				{
					return Type == PlaceableType.LockedContent;
				}
				return true;
			}
		}

		public Placeable()
		{
			Type = PlaceableType.None;
			SourceTokenIndex = (TargetTokenIndex = -1);
		}

		public Placeable(PlaceableType t, int sourceTokenIndex, int targetTokenIndex)
		{
			Type = t;
			SourceTokenIndex = sourceTokenIndex;
			TargetTokenIndex = targetTokenIndex;
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
			Placeable placeable = obj as Placeable;
			if (placeable == null)
			{
				return false;
			}
			if (Type == placeable.Type && SourceTokenIndex == placeable.SourceTokenIndex)
			{
				return TargetTokenIndex == placeable.TargetTokenIndex;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public bool IsTagCompatible(TagType tagType)
		{
			switch (Type)
			{
			case PlaceableType.StandaloneTag:
				return tagType == TagType.Standalone;
			case PlaceableType.PairedTagStart:
				return tagType == TagType.Start;
			case PlaceableType.PairedTagEnd:
				return tagType == TagType.End;
			case PlaceableType.TextPlaceholder:
				return tagType == TagType.TextPlaceholder;
			case PlaceableType.LockedContent:
				return tagType == TagType.LockedContent;
			default:
				return false;
			}
		}
	}
}
