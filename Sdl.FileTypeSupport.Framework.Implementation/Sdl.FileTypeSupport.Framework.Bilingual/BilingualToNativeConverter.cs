using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi.Buffer;
using Sdl.FileTypeSupport.Framework.Native;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.FileTypeSupport.Framework.Bilingual
{
	public class BilingualToNativeConverter : BilingualToSourceConverter, INativeOutputSettingsAware
	{
		private NativeBuffer _Buffer = new NativeBuffer();

		private List<SubSegmentReplacements> _UnresolvedSubSegmentTags = new List<SubSegmentReplacements>();

		private INativeOutputFileProperties _OutputProperties = new NativeOutputFileProperties();

		private Dictionary<ParagraphUnitId, string> _FoundSubSegments = new Dictionary<ParagraphUnitId, string>();

		private IParagraph _TagPairTargetParentParagraph;

		public BilingualToNativeConverter()
		{
			_OutputProperties.ContentRestriction = ContentRestriction.Target;
		}

		protected override void ConnectOutputHandlers()
		{
			base.ConnectOutputHandlers();
			_Buffer.GenerationOutput = base.InternalOutput;
			base.InternalOutput = _Buffer;
		}

		protected override void ProcessSubSegmentReferences(IAbstractTag tag)
		{
			if (tag.HasSubSegmentReferences)
			{
				SubSegmentReplacements subSegmentReplacements = new SubSegmentReplacements();
				subSegmentReplacements.Tag = tag;
				if (AttemptResolveSubSegments(subSegmentReplacements))
				{
					subSegmentReplacements.ApplyReplacements();
				}
				else
				{
					_UnresolvedSubSegmentTags.Add(subSegmentReplacements);
					_Buffer.Hold();
				}
			}
			base.ProcessSubSegmentReferences(tag);
		}

		private bool AttemptResolveSubSegments(SubSegmentReplacements sub)
		{
			if (sub.Tag.HasSubSegmentReferences)
			{
				foreach (ISubSegmentReference subSegment in sub.Tag.SubSegments)
				{
					if (_FoundSubSegments.ContainsKey(subSegment.ParagraphUnitId))
					{
						sub.AddReplacement(subSegment.Properties, _FoundSubSegments[subSegment.ParagraphUnitId]);
					}
				}
				return sub.ReplacementsCount == sub.Tag.SubSegments.Count();
			}
			return sub.ReplacementsCount == 0;
		}

		protected bool IsInTarget(IAbstractTag tag)
		{
			IAbstractMarkupDataContainer parent = tag.Parent;
			while (parent != null)
			{
				IParagraph paragraph = parent as IParagraph;
				if (paragraph != null)
				{
					return paragraph.IsTarget;
				}
				if (parent is ILockedContainer)
				{
					ILockedContainer lockedContainer = parent as ILockedContainer;
					parent = lockedContainer.LockedContent.Parent;
				}
				else
				{
					parent = ((IAbstractMarkupData)parent).Parent;
				}
			}
			return false;
		}

		protected override ITagPair CloneAsNecessary(ITagPair tag)
		{
			if (!RequiresCloning(tag))
			{
				return tag;
			}
			ITagPair tagPair = (ITagPair)tag.Clone();
			tagPair.StartTagProperties = (IStartTagProperties)tag.StartTagProperties.Clone();
			tagPair.EndTagProperties = (IEndTagProperties)tag.EndTagProperties.Clone();
			if (tag.ParentParagraph != null && tag.ParentParagraph.Parent != null)
			{
				_TagPairTargetParentParagraph = tag.ParentParagraph;
			}
			return tagPair;
		}

		protected override void ProcessSubSegmentParagraphUnit(IParagraphUnit paragraphUnit)
		{
			NativeStringWriter nativeStringWriter = new NativeStringWriter();
			SubSegmentParagraphUnitConverter subSegmentParagraphUnitConverter = new SubSegmentParagraphUnitConverter();
			subSegmentParagraphUnitConverter._OutputProperties = _OutputProperties;
			subSegmentParagraphUnitConverter.DocumentInfo = base.DocumentInfo;
			subSegmentParagraphUnitConverter.FileInfo = base.FileInfo;
			subSegmentParagraphUnitConverter.MessageReporter = MessageReporter;
			subSegmentParagraphUnitConverter.Output = nativeStringWriter;
			subSegmentParagraphUnitConverter.ItemFactory = ItemFactory;
			subSegmentParagraphUnitConverter.ParagraphUnitsToSkip = base.ParagraphUnitsToSkip;
			subSegmentParagraphUnitConverter._UnresolvedSubSegmentTags = _UnresolvedSubSegmentTags;
			subSegmentParagraphUnitConverter.ConnectOutputHandlers();
			subSegmentParagraphUnitConverter.Initialize(base.DocumentInfo);
			subSegmentParagraphUnitConverter.SetFileProperties(base.FileInfo);
			foreach (IAbstractMarkupData item in GetContentToProcess(paragraphUnit))
			{
				item.AcceptVisitor(subSegmentParagraphUnitConverter);
			}
			subSegmentParagraphUnitConverter._UnresolvedSubSegmentTags = new List<SubSegmentReplacements>();
			subSegmentParagraphUnitConverter.FileComplete();
			subSegmentParagraphUnitConverter.Complete();
			string output = nativeStringWriter.Output;
			_FoundSubSegments[paragraphUnit.Properties.ParagraphUnitId] = output;
			foreach (SubSegmentReplacements unresolvedSubSegmentTag in _UnresolvedSubSegmentTags)
			{
				foreach (ISubSegmentReference subSegment in unresolvedSubSegmentTag.Tag.SubSegments)
				{
					if (subSegment.ParagraphUnitId == paragraphUnit.Properties.ParagraphUnitId)
					{
						unresolvedSubSegmentTag.AddReplacement(subSegment.Properties, output);
					}
				}
			}
			foreach (SubSegmentReplacements unresolvedSubSegmentTag2 in _UnresolvedSubSegmentTags)
			{
				if (unresolvedSubSegmentTag2.Tag.SubSegments.Count() != unresolvedSubSegmentTag2.ReplacementsCount)
				{
					return;
				}
			}
			foreach (SubSegmentReplacements unresolvedSubSegmentTag3 in _UnresolvedSubSegmentTags)
			{
				unresolvedSubSegmentTag3.ApplyReplacements();
			}
			_Buffer.Release();
			_UnresolvedSubSegmentTags.Clear();
		}

		protected override IEnumerable<IAbstractMarkupData> GetContentToProcess(IParagraphUnit paragraphUnit)
		{
			switch (_OutputProperties.ContentRestriction)
			{
			case ContentRestriction.NoContent:
				return new List<IAbstractMarkupData>();
			case ContentRestriction.Source:
				return paragraphUnit.Source;
			case ContentRestriction.Target:
				if (paragraphUnit.IsStructure && (paragraphUnit.Target == null || paragraphUnit.Target.Count == 0))
				{
					return paragraphUnit.Source;
				}
				return paragraphUnit.Target;
			default:
				throw new FileTypeSupportException(StringResources.BilingualToNativeConverter_UnrecognizedValue);
			}
		}

		protected IAbstractMarkupDataContainer GetSegmentContentToProcess(ISegment segment)
		{
			if (_OutputProperties.ContentRestriction == ContentRestriction.Target && !HasDataContent(segment) && segment.Properties.ConfirmationLevel < ConfirmationLevel.Translated)
			{
				if (segment.ParentParagraphUnit != null)
				{
					return segment.ParentParagraphUnit.GetSourceSegment(segment.Properties.Id);
				}
				if (_TagPairTargetParentParagraph.Parent != null)
				{
					return _TagPairTargetParentParagraph.Parent.GetSourceSegment(segment.Properties.Id);
				}
			}
			return segment;
		}

		private bool HasDataContent(IAbstractMarkupDataContainer container)
		{
			foreach (IAbstractMarkupData allSubItem in container.AllSubItems)
			{
				if (allSubItem is IAbstractDataContent)
				{
					return true;
				}
			}
			return false;
		}

		public override void SetFileProperties(IFileProperties fileInfo)
		{
			_FoundSubSegments.Clear();
			base.SetFileProperties(fileInfo);
		}

		public override void VisitSegment(ISegment segment)
		{
			base.InternalOutput.SegmentStart(segment.Properties);
			VisitContainerItems(GetSegmentContentToProcess(segment));
			base.InternalOutput.SegmentEnd();
		}

		public override void VisitCommentMarker(ICommentMarker commentMarker)
		{
			bool flag = false;
			if (commentMarker.ParentParagraph != null)
			{
				flag = commentMarker.ParentParagraph.IsSource;
			}
			else if (_TagPairTargetParentParagraph != null)
			{
				flag = _TagPairTargetParentParagraph.IsSource;
			}
			bool flag2 = _OutputProperties.ContentRestriction != ContentRestriction.Target || !flag;
			if (flag2)
			{
				base.InternalOutput.CommentStart(commentMarker.Comments);
			}
			VisitContainerItems(commentMarker);
			if (flag2)
			{
				base.InternalOutput.CommentEnd();
			}
		}

		public override void FileComplete()
		{
			if (_Buffer.IsHolding)
			{
				MessageReporter.ReportMessage(this, StringResources.FileProcessing, ErrorLevel.Error, StringResources.BilingualToNativeConverter_PUMissingMessage, null);
				foreach (SubSegmentReplacements unresolvedSubSegmentTag in _UnresolvedSubSegmentTags)
				{
					unresolvedSubSegmentTag.ApplyReplacements();
				}
				_Buffer.Release();
				_UnresolvedSubSegmentTags.Clear();
			}
			_FoundSubSegments.Clear();
			base.FileComplete();
		}

		public void SetOutputProperties(INativeOutputFileProperties properties)
		{
			_OutputProperties = properties;
		}

		public void GetProposedOutputFileInfo(IPersistentFileConversionProperties fileProperties, IOutputFileInfo proposedFileInfo)
		{
		}
	}
}
