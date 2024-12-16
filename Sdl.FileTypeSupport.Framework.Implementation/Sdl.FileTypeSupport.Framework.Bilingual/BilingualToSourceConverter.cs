using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Native;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.Bilingual
{
	public class BilingualToSourceConverter : AbstractBilingualFileTypeComponent, IBilingualToNativeConverter, IBilingualContentHandler, IMarkupDataVisitor
	{
		private AdjacentClonedPairedTagsMerger _PairedTagMerger = new AdjacentClonedPairedTagsMerger();

		private CommentMerger _CommentMerger = new CommentMerger();

		private INativeGenerationContentHandler _internalOutput;

		private INativeGenerationContentHandler _publicOutput;

		private IDocumentProperties _DocumentInfo;

		private IFileProperties _FileInfo;

		protected SortedList<ParagraphUnitId, object> _SubSegmentParagraphUnits = new SortedList<ParagraphUnitId, object>();

		private IContextProperties _CurrentContext;

		public INativeGenerationContentHandler PublicOutput
		{
			get
			{
				return _publicOutput;
			}
			set
			{
				_publicOutput = value;
			}
		}

		protected INativeGenerationContentHandler InternalOutput
		{
			get
			{
				return _internalOutput;
			}
			set
			{
				_internalOutput = value;
			}
		}

		protected SortedList<ParagraphUnitId, object> ParagraphUnitsToSkip
		{
			get
			{
				return _SubSegmentParagraphUnits;
			}
			set
			{
				_SubSegmentParagraphUnits = value;
			}
		}

		protected IDocumentProperties DocumentInfo
		{
			get
			{
				return _DocumentInfo;
			}
			set
			{
				_DocumentInfo = value;
			}
		}

		protected IFileProperties FileInfo
		{
			get
			{
				return _FileInfo;
			}
			set
			{
				_FileInfo = value;
			}
		}

		public INativeGenerationContentHandler Output
		{
			get
			{
				return _publicOutput;
			}
			set
			{
				_publicOutput = value;
			}
		}

		public virtual void Initialize(IDocumentProperties documentInfo)
		{
			ConnectOutputHandlers();
			_DocumentInfo = documentInfo;
		}

		protected virtual void ConnectOutputHandlers()
		{
			_CommentMerger.Output = _publicOutput;
			_PairedTagMerger.Output = _CommentMerger;
			_internalOutput = _PairedTagMerger;
		}

		public virtual void Complete()
		{
		}

		public virtual void SetFileProperties(IFileProperties fileInfo)
		{
			if (fileInfo == null)
			{
				throw new ArgumentNullException("fileInfo");
			}
			_FileInfo = fileInfo;
			_PairedTagMerger.SetFileProperties(fileInfo);
			_PairedTagMerger.StartOfInput();
			_CommentMerger.SetFileProperties(fileInfo);
			_CommentMerger.StartOfInput();
		}

		public virtual void FileComplete()
		{
			if (base.PropertiesFactory == null)
			{
				throw new FileTypeSupportException(StringResources.BilingualToSourceConverter_NoPropertiesFactoryError);
			}
			if (_FileInfo == null)
			{
				throw new FileTypeSupportException(StringResources.BilingualToSourceConverter_PropertiesNotSetError);
			}
			_PairedTagMerger.EndOfInput();
			_CommentMerger.EndOfInput();
		}

		protected virtual void ProcessSubSegmentParagraphUnit(IParagraphUnit paragraphUnit)
		{
		}

		protected virtual IEnumerable<IAbstractMarkupData> GetContentToProcess(IParagraphUnit paragraphUnit)
		{
			return paragraphUnit.Source;
		}

		protected virtual void ProcessParagraphUnitContent(IParagraphUnit paragraphUnit)
		{
			VisitContainerItems(GetContentToProcess(paragraphUnit));
		}

		public virtual void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			if (paragraphUnit == null)
			{
				throw new ArgumentNullException("paragraphUnit");
			}
			if (_SubSegmentParagraphUnits.ContainsKey(paragraphUnit.Properties.ParagraphUnitId))
			{
				ProcessSubSegmentParagraphUnit(paragraphUnit);
				_SubSegmentParagraphUnits.Remove(paragraphUnit.Properties.ParagraphUnitId);
				return;
			}
			_internalOutput.ParagraphUnitStart(paragraphUnit.Properties);
			ManageContext(paragraphUnit);
			ProcessParagraphUnitContent(paragraphUnit);
			_internalOutput.ParagraphUnitEnd();
		}

		protected virtual void ManageContext(IParagraphUnit paragraphUnit)
		{
			if (_CurrentContext != paragraphUnit.Properties.Contexts)
			{
				_internalOutput.ChangeContext(paragraphUnit.Properties.Contexts);
				_CurrentContext = paragraphUnit.Properties.Contexts;
			}
		}

		protected virtual void ProcessSubSegmentReferences(IAbstractTag tag)
		{
			if (tag.HasSubSegmentReferences)
			{
				foreach (ISubSegmentReference subSegment in tag.SubSegments)
				{
					if (!_SubSegmentParagraphUnits.ContainsKey(subSegment.ParagraphUnitId))
					{
						_SubSegmentParagraphUnits.Add(subSegment.ParagraphUnitId, new object());
					}
				}
			}
		}

		protected virtual bool RequiresCloning(IAbstractTag tag)
		{
			if (!tag.TagProperties.HasLocalizableContent)
			{
				return false;
			}
			return true;
		}

		protected virtual IPlaceholderTag CloneAsNecessary(IPlaceholderTag tag)
		{
			if (!RequiresCloning(tag))
			{
				return tag;
			}
			IPlaceholderTag placeholderTag = (IPlaceholderTag)tag.Clone();
			placeholderTag.Properties = (IPlaceholderTagProperties)tag.Properties.Clone();
			return placeholderTag;
		}

		protected virtual ITagPair CloneAsNecessary(ITagPair tag)
		{
			if (!RequiresCloning(tag))
			{
				return tag;
			}
			ITagPair tagPair = (ITagPair)tag.Clone();
			tagPair.StartTagProperties = (IStartTagProperties)tag.StartTagProperties.Clone();
			tagPair.EndTagProperties = (IEndTagProperties)tag.EndTagProperties.Clone();
			return tagPair;
		}

		protected virtual IStructureTag CloneAsNecessary(IStructureTag tag)
		{
			if (!RequiresCloning(tag))
			{
				return tag;
			}
			IStructureTag structureTag = (IStructureTag)tag.Clone();
			structureTag.Properties = (IStructureTagProperties)tag.Properties.Clone();
			return structureTag;
		}

		protected void VisitContainerItems(IEnumerable<IAbstractMarkupData> container)
		{
			foreach (IAbstractMarkupData item in container)
			{
				IStructureTag structureTag = item as IStructureTag;
				if (structureTag != null)
				{
					VisitStructureTag(structureTag);
				}
				else
				{
					item.AcceptVisitor(this);
				}
			}
		}

		public virtual void VisitStructureTag(IStructureTag tag)
		{
			if (tag == null || tag.TagProperties == null || tag.TagProperties.TagContent == null || !tag.TagProperties.TagContent.Contains("#SDL_SUBCONTENT_BOUNDARY#"))
			{
				IStructureTag structureTag = CloneAsNecessary(tag);
				ProcessSubSegmentReferences(structureTag);
				_internalOutput.StructureTag(structureTag.Properties);
			}
		}

		public virtual void VisitTagPair(ITagPair tag)
		{
			ITagPair tagPair = CloneAsNecessary(tag);
			ProcessSubSegmentReferences(tagPair);
			_internalOutput.InlineStartTag(tagPair.StartTagProperties);
			VisitContainerItems(tagPair);
			_internalOutput.InlineEndTag(tagPair.EndTagProperties);
		}

		public virtual void VisitPlaceholderTag(IPlaceholderTag tag)
		{
			IPlaceholderTag placeholderTag = CloneAsNecessary(tag);
			ProcessSubSegmentReferences(placeholderTag);
			_internalOutput.InlinePlaceholderTag(placeholderTag.Properties);
		}

		public virtual void VisitText(IText text)
		{
			_internalOutput.Text(text.Properties);
		}

		public virtual void VisitSegment(ISegment segment)
		{
			_internalOutput.SegmentStart(segment.Properties);
			VisitContainerItems(segment);
			_internalOutput.SegmentEnd();
		}

		public virtual void VisitCommentMarker(ICommentMarker commentMarker)
		{
			_internalOutput.CommentStart(commentMarker.Comments);
			VisitContainerItems(commentMarker);
			_internalOutput.CommentEnd();
		}

		public virtual void VisitOtherMarker(IOtherMarker marker)
		{
			VisitContainerItems(marker);
		}

		public virtual void VisitLocationMarker(ILocationMarker location)
		{
			_internalOutput.LocationMark(location.MarkerId);
		}

		public virtual void VisitLockedContent(ILockedContent lockedContent)
		{
			_internalOutput.LockedContentStart(lockedContent.Properties);
			VisitContainerItems(lockedContent.Content);
			_internalOutput.LockedContentEnd();
		}

		public virtual void VisitRevisionMarker(IRevisionMarker revisionMarker)
		{
			_internalOutput.RevisionStart(revisionMarker.Properties);
			VisitContainerItems(revisionMarker);
			_internalOutput.RevisionEnd();
		}

		public void Flush()
		{
			_PairedTagMerger.StopHolding();
		}
	}
}
