using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Native;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.IO;
using System.Runtime.Serialization;

namespace Sdl.FileTypeSupport.Framework.Bilingual
{
	public class DocumentItemFactory : AbstractNativeFileTypeComponent, IDocumentItemFactory
	{
		private long _NextSegmentId = 1L;

		public DocumentItemFactory()
		{
			PropertiesFactory = new PropertiesFactory();
		}

		protected virtual ParagraphUnitId GetNextParagraphUnitId()
		{
			return new ParagraphUnitId(Guid.NewGuid().ToString());
		}

		protected virtual SegmentId GetNextSegmentId()
		{
			string id = _NextSegmentId.ToString();
			_NextSegmentId++;
			return new SegmentId(id);
		}

		public IFileProperties CreateFileProperties()
		{
			return new FileProperties();
		}

		public IParagraphUnit CreateParagraphUnit(LockTypeFlags flags)
		{
			ParagraphUnit paragraphUnit = new ParagraphUnit();
			paragraphUnit.Properties.ParagraphUnitId = GetNextParagraphUnitId();
			paragraphUnit.Properties.LockType = flags;
			return paragraphUnit;
		}

		public IPlaceholderTag CreatePlaceholderTag(IPlaceholderTagProperties tagInfo)
		{
			if (tagInfo == null)
			{
				throw new ArgumentNullException("startTagInfo");
			}
			PlaceholderTag placeholderTag = new PlaceholderTag();
			placeholderTag.Properties = tagInfo;
			return placeholderTag;
		}

		public IText CreateText(ITextProperties textInfo)
		{
			if (textInfo == null)
			{
				throw new ArgumentNullException("textInfo");
			}
			Text text = new Text();
			text.Properties = textInfo;
			return text;
		}

		public IStructureTag CreateStructureTag(IStructureTagProperties tagInfo)
		{
			if (tagInfo == null)
			{
				throw new ArgumentNullException("startTagInfo");
			}
			StructureTag structureTag = new StructureTag();
			structureTag.Properties = tagInfo;
			return structureTag;
		}

		public ITagPair CreateTagPair(IStartTagProperties startTagInfo, IEndTagProperties endTagInfo)
		{
			if (startTagInfo == null)
			{
				throw new ArgumentNullException("startTagInfo");
			}
			TagPair tagPair = new TagPair();
			tagPair.StartTagProperties = startTagInfo;
			tagPair.EndTagProperties = endTagInfo;
			return tagPair;
		}

		public ISubSegmentReference CreateSubSegmentReference(ISubSegmentProperties subSegment, ParagraphUnitId paragraphUnitId)
		{
			SubSegmentReference subSegmentReference = new SubSegmentReference();
			subSegmentReference.Properties = subSegment;
			subSegmentReference.ParagraphUnitId = paragraphUnitId;
			return subSegmentReference;
		}

		public IOtherMarker CreateOtherMarker()
		{
			return new OtherMarker();
		}

		public ICommentMarker CreateCommentMarker(ICommentProperties comments)
		{
			CommentMarker commentMarker = new CommentMarker();
			commentMarker.Comments = comments;
			return commentMarker;
		}

		public IDocumentProperties CreateDocumentProperties()
		{
			return new DocumentProperties();
		}

		public ILocationMarker CreateLocationMarker()
		{
			return new LocationMarker();
		}

		public ISegmentPairProperties CreateSegmentPairProperties()
		{
			ISegmentPairProperties segmentPairProperties = new SegmentPairProperties();
			segmentPairProperties.Id = GetNextSegmentId();
			return segmentPairProperties;
		}

		public ISegment CreateSegment(ISegmentPairProperties properties)
		{
			Segment segment = new Segment();
			segment.Properties = properties;
			return segment;
		}

		public ISegmentPair CreateSegmentPair(ISegment source, ISegment target)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (target != null && target.Properties != source.Properties)
			{
				throw new FileTypeSupportException(StringResources.FileAdministrator_IncorrectFormatError);
			}
			SegmentPair segmentPair = new SegmentPair();
			segmentPair.Source = source;
			segmentPair.Target = target;
			return segmentPair;
		}

		public ILockedContent CreateLockedContent(ILockedContentProperties properties)
		{
			ILockedContent lockedContent = new LockedContent();
			lockedContent.Properties = properties;
			return lockedContent;
		}

		[Obsolete("Please use IPropertiesFactory.CreateRevisionProperties(RevisionType) instead.")]
		public IRevisionProperties CreateRevisionProperties(RevisionType type)
		{
			return PropertiesFactory.CreateRevisionProperties(type);
		}

		public IRevisionMarker CreateRevision(IRevisionProperties properties)
		{
			IRevisionMarker revisionMarker = new RevisionMarker();
			revisionMarker.Properties = properties;
			return revisionMarker;
		}

		public IRevisionMarker CreateFeedback(IRevisionProperties properties)
		{
			IRevisionMarker revisionMarker = new FeedbackMarker();
			revisionMarker.Properties = properties;
			return revisionMarker;
		}

		public ITranslationOrigin CreateTranslationOrigin()
		{
			return new TranslationOrigin();
		}

		public ISerializableMarkupDataContainer CreateSerializableContainer()
		{
			return new SerializableMarkupDataContainer();
		}

		public ISerializableMarkupDataContainer DeserialzeContainer(IFormatter formatter, Stream serializationStream)
		{
			return formatter.Deserialize(serializationStream) as SerializableMarkupDataContainer;
		}
	}
}
