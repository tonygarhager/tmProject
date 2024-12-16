using Sdl.LanguagePlatform.Core.Tokenization;

namespace Sdl.LanguagePlatform.Core
{
	public interface ISegmentElementVisitor
	{
		void VisitText(Text text);

		void VisitTag(Tag tag);

		void VisitDateTimeToken(DateTimeToken token);

		void VisitNumberToken(NumberToken token);

		void VisitMeasureToken(MeasureToken token);

		void VisitSimpleToken(SimpleToken token);

		void VisitTagToken(TagToken token);
	}
}
