using System;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	public class PlaceableAssociation
	{
		public PlaceableType Type
		{
			get
			{
				if (Memory != null)
				{
					return Memory.Type;
				}
				return Document?.Type ?? PlaceableType.None;
			}
		}

		[DataMember]
		public Placeable Document
		{
			get;
			set;
		}

		[DataMember]
		public Placeable Memory
		{
			get;
			set;
		}

		public PlaceableAssociation(Placeable docPlaceable, Placeable memPlaceable)
		{
			Document = docPlaceable;
			Memory = memPlaceable;
			VerifyTypeCompatibility(Document, Memory);
		}

		private static void VerifyTypeCompatibility(Placeable a, Placeable b)
		{
			if (!AreAssociable(a, b))
			{
				throw new ArgumentException("Placeable types differ");
			}
		}

		public static bool AreAssociable(Placeable a, Placeable b)
		{
			if (a == null || b == null)
			{
				return false;
			}
			if (a.Type == b.Type)
			{
				return true;
			}
			if (a.Type != PlaceableType.StandaloneTag || b.Type != PlaceableType.TextPlaceholder)
			{
				if (a.Type == PlaceableType.TextPlaceholder)
				{
					return b.Type == PlaceableType.StandaloneTag;
				}
				return false;
			}
			return true;
		}
	}
}
