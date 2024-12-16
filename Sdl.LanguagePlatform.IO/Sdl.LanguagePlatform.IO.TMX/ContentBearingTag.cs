using Sdl.LanguagePlatform.Core;

namespace Sdl.LanguagePlatform.IO.TMX
{
	internal class ContentBearingTag : Tag
	{
		private string _Content;

		public string Content
		{
			get
			{
				return _Content;
			}
			set
			{
				_Content = value;
			}
		}

		public ContentBearingTag(Tag other, string content)
			: base(other)
		{
			_Content = content;
		}

		public override SegmentElement Duplicate()
		{
			return new ContentBearingTag(this, _Content);
		}
	}
}
