using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public interface IMessageLocation
	{
		FileId? FileId
		{
			get;
			set;
		}

		ParagraphUnitId? ParagrahUnitId
		{
			get;
			set;
		}

		SegmentId? SegmentId
		{
			get;
			set;
		}

		ContentRestriction SourceOrTarget
		{
			get;
			set;
		}

		int CharactersIntoParagraph
		{
			get;
			set;
		}

		int CharactersIntoSegment
		{
			get;
			set;
		}

		TextLocation ParagraphLocation
		{
			get;
			set;
		}

		string LocationDescription
		{
			get;
			set;
		}
	}
}
