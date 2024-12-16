using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.IO;
using System.Runtime.Serialization;

namespace Sdl.FileTypeSupport.Framework.BilingualApi
{
	public interface IDocumentItemFactory
	{
		IPropertiesFactory PropertiesFactory
		{
			get;
			set;
		}

		IDocumentProperties CreateDocumentProperties();

		IFileProperties CreateFileProperties();

		IStructureTag CreateStructureTag(IStructureTagProperties tagInfo);

		IParagraphUnit CreateParagraphUnit(LockTypeFlags flags);

		IText CreateText(ITextProperties textInfo);

		ITagPair CreateTagPair(IStartTagProperties startTagInfo, IEndTagProperties endTagInfo);

		IPlaceholderTag CreatePlaceholderTag(IPlaceholderTagProperties tagInfo);

		ISubSegmentReference CreateSubSegmentReference(ISubSegmentProperties subSegment, ParagraphUnitId paragraphUnitId);

		ILocationMarker CreateLocationMarker();

		IOtherMarker CreateOtherMarker();

		ICommentMarker CreateCommentMarker(ICommentProperties comments);

		ISegmentPairProperties CreateSegmentPairProperties();

		ITranslationOrigin CreateTranslationOrigin();

		ISegment CreateSegment(ISegmentPairProperties properties);

		ISegmentPair CreateSegmentPair(ISegment source, ISegment target);

		ILockedContent CreateLockedContent(ILockedContentProperties properties);

		[Obsolete("Please use IPropertiesFactory.CreateRevisionProperties(RevisionType) instead.")]
		IRevisionProperties CreateRevisionProperties(RevisionType type);

		IRevisionMarker CreateRevision(IRevisionProperties properties);

		IRevisionMarker CreateFeedback(IRevisionProperties properties);

		ISerializableMarkupDataContainer CreateSerializableContainer();

		ISerializableMarkupDataContainer DeserialzeContainer(IFormatter formatter, Stream serializationStream);
	}
}
