namespace Sdl.FileTypeSupport.Framework.Core.Settings.QuickInserts
{
	public class MarkupDataFactory
	{
		public BaseMarkupDataType CreateMarkupDataType(string type)
		{
			if (type == typeof(TextMarkup).Name)
			{
				return new TextMarkup();
			}
			if (type == typeof(TextPairMarkup).Name)
			{
				return new TextPairMarkup();
			}
			if (type == typeof(PlaceholderTagMarkup).Name)
			{
				return new PlaceholderTagMarkup();
			}
			if (type == typeof(TagPairMarkup).Name)
			{
				return new TagPairMarkup();
			}
			return null;
		}
	}
}
