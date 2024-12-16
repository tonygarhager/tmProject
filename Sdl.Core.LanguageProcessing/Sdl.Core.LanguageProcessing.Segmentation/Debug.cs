using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Core.LanguageProcessing.Segmentation
{
	internal class Debug
	{
		public static string ToString(Location item)
		{
			if (item != null)
			{
				return $"Location: valid: {item.IsValid}, depth: {item.Depth}, index: {item.BottomLevel.Index} item: {ToString(item.ItemAtLocation)}";
			}
			return "(null)";
		}

		public static string ToString(LevelLocation item)
		{
			if (item != null)
			{
				return $"LevelLocation: valid: {item.IsValid}, index: {item.Index}, at parent start: {item.IsAtStartOfParent}, at parent end: {item.IsAtEndOfParent}, item: {ToString(item.ItemAtLocation)}";
			}
			return "(null)";
		}

		public static string ToString(IAbstractDataContent item)
		{
			if (item != null)
			{
				if (item is IAbstractTag)
				{
					return "AT:" + item?.ToString();
				}
				IText text = item as IText;
				if (text == null)
				{
					return item.GetType().ToString();
				}
				return ToString(text);
			}
			return "(null)";
		}

		public static string ToString(IAbstractMarker item)
		{
			if (item == null)
			{
				return "(null)";
			}
			return item.GetType().ToString();
		}

		public static string ToString(IAbstractMarkupDataContainer item)
		{
			if (item != null)
			{
				ISegment segment = item as ISegment;
				if (segment != null)
				{
					return ToString(segment);
				}
				ITagPair tagPair = item as ITagPair;
				if (tagPair == null)
				{
					return item.GetType().ToString();
				}
				return ToString(tagPair);
			}
			return "(null)";
		}

		public static string ToString(IParagraph item)
		{
			if (item == null)
			{
				return "(null)";
			}
			return $"Paragraph ({item.Count} elements)";
		}

		public static string ToString(ISegment item)
		{
			if (item == null)
			{
				return "(null)";
			}
			return $"Segment ({item.Count} elements), id={item.Properties.Id.Id}, origin={item.Properties.TranslationOrigin}";
		}

		public static string ToString(IStructureTag item)
		{
			if (item == null)
			{
				return "(null)";
			}
			return "StructureTag (id=" + item.Properties.TagId.Id + "), content=\"" + item.Properties.TagContent + "\"";
		}

		public static string ToString(IPlaceholderTag item)
		{
			if (item == null)
			{
				return "(null)";
			}
			return "PlaceholderTag (id=" + item.Properties.TagId.Id + "), content=\"" + item.Properties.TagContent + "\"";
		}

		public static string ToString(ITagPair item)
		{
			if (item == null)
			{
				return "(null)";
			}
			return $"TagPair (id={item.TagProperties.TagId.Id}), {item.Count} elements. Start={item.StartTagProperties.TagContent} end={item.EndTagProperties.TagContent}";
		}

		public static string ToString(IText item)
		{
			if (item != null)
			{
				return "Text: \"" + item.Properties.Text + "\"";
			}
			return "(null)";
		}

		public static string ToString(IAbstractMarkupData item)
		{
			if (item != null)
			{
				ISegment segment = item as ISegment;
				if (segment == null)
				{
					ITagPair tagPair = item as ITagPair;
					if (tagPair == null)
					{
						IAbstractDataContent abstractDataContent = item as IAbstractDataContent;
						if (abstractDataContent != null)
						{
							return ToString(abstractDataContent);
						}
						IAbstractMarker abstractMarker = item as IAbstractMarker;
						if (abstractMarker == null)
						{
							return item.GetType().ToString();
						}
						return ToString(abstractMarker);
					}
					return ToString(tagPair);
				}
				return ToString(segment);
			}
			return "(null)";
		}

		public static string ToString(IParagraphUnit item)
		{
			if (item != null)
			{
				return $"TransUnit (id={item.Properties.ParagraphUnitId.Id}, locktype={item.Properties.LockType} structure={item.IsStructure})";
			}
			return "(null)";
		}

		public static void Dump(IAbstractMarkupData data, int level)
		{
			IAbstractMarkupDataContainer abstractMarkupDataContainer = data as IAbstractMarkupDataContainer;
			if (abstractMarkupDataContainer != null)
			{
				Dump(abstractMarkupDataContainer, level);
				return;
			}
			for (int i = 0; i < level; i++)
			{
			}
		}

		public static void Dump(IAbstractMarkupDataContainer data, string msg)
		{
			Dump(data, 0);
		}

		public static void Dump(IAbstractMarkupDataContainer data, int level)
		{
			for (int i = 0; i < level; i++)
			{
			}
			foreach (IAbstractMarkupData datum in data)
			{
				Dump(datum, level + 1);
			}
		}
	}
}
