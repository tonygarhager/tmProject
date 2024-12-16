using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.FileTypeSupport.Framework.Bilingual
{
	public class NativeToBilingualConverterImpl : AbstractBilingualFileTypeComponent, INativeToBilingualConverter, INativeExtractionContentHandler, IAbstractNativeContentHandler, INativeContentCycleAware
	{
		private IBilingualContentHandler _Output;

		private IFileProperties _FileProperties;

		private IDocumentProperties _DocumentInfo;

		private IParagraphUnit _CurrentParagraphUnit;

		private Stack<IAbstractMarkupDataContainer> _CurrentContainers = new Stack<IAbstractMarkupDataContainer>();

		private List<IParagraphUnit> _SubSegmentParagraphUnits = new List<IParagraphUnit>();

		private IContextProperties _Contexts;

		private ParagraphUnitBuffer _ParagraphUnitBuffer = new ParagraphUnitBuffer();

		private Stack<List<IAbstractMarkupData>> _UnclosedContentPairs = new Stack<List<IAbstractMarkupData>>();

		private bool _hasLocalizableParagraph;

		private bool _hasStructureParagraph;

		private bool _structureMode;

		public const string KEY_CONTEXT_ADDED_BY_FRAMEWORK = "SDL:AddedByFramework";

		public const string VALUE_CONTEXTPROPERTIES_CREATED_BY_FRAMEWORK = "SDL:CreatedByFramework";

		public virtual IBilingualContentHandler Output
		{
			get
			{
				return _Output;
			}
			set
			{
				_Output = value;
				_ParagraphUnitBuffer.Output = value;
			}
		}

		public virtual IDocumentProperties DocumentInfo
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

		public virtual IFileProperties FileInfo
		{
			get
			{
				return _FileProperties;
			}
			set
			{
				_FileProperties = value;
			}
		}

		private IAbstractMarkupDataContainer CurrentContainer
		{
			get
			{
				if (_CurrentContainers.Count == 0)
				{
					return null;
				}
				return _CurrentContainers.Peek();
			}
		}

		protected virtual void OutputSubSegments()
		{
			foreach (IParagraphUnit subSegmentParagraphUnit in _SubSegmentParagraphUnits)
			{
				CallProcessParagraphUnit(subSegmentParagraphUnit);
			}
			_SubSegmentParagraphUnits.Clear();
		}

		protected virtual void CallInitialize(IDocumentProperties documentInfo)
		{
			if (_Output != null)
			{
				_Output.Initialize(documentInfo);
			}
		}

		protected virtual void CallComplete()
		{
			if (_Output != null)
			{
				_Output.Complete();
			}
		}

		protected virtual void CallSetFileProperties(IFileProperties fileInfo)
		{
			if (_Output != null)
			{
				_Output.SetFileProperties(fileInfo);
			}
		}

		protected virtual void CallFileComplete()
		{
			if (_Output != null)
			{
				if (!_ParagraphUnitBuffer.IsEmpty || _UnclosedContentPairs.Count > 0)
				{
					throw new FileTypeSupportException(StringResources.NativeToBilingialConverter_UnmatchedTagPairError);
				}
				_Output.FileComplete();
			}
		}

		protected virtual void CallProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			if (_Output != null)
			{
				_ParagraphUnitBuffer.ProcessParagraphUnit(paragraphUnit);
			}
		}

		public virtual void StructureTag(IStructureTagProperties tagInfo)
		{
			if (!_structureMode)
			{
				SwitchMode(switchToStructure: true);
			}
			if (tagInfo != null && tagInfo.TagContent != null && tagInfo.TagContent.Contains("#SDL_SUBCONTENT_BOUNDARY#"))
			{
				OutputCurrentParagraphUnit();
				return;
			}
			EnsureHasParagraphUnit();
			IStructureTagProperties tagInfo2 = tagInfo.Clone() as IStructureTagProperties;
			IStructureTag structureTag = ItemFactory.CreateStructureTag(tagInfo2);
			AddContentItem(structureTag);
			ProcessSubSegments(structureTag);
		}

		public virtual void InlineStartTag(IStartTagProperties tagInfo)
		{
			if (_structureMode)
			{
				SwitchMode(switchToStructure: false);
			}
			EnsureHasParagraphUnit();
			IStartTagProperties startTagInfo = tagInfo.Clone() as IStartTagProperties;
			ITagPair tagPair = ItemFactory.CreateTagPair(startTagInfo, null);
			AddContentItem(tagPair);
			PushUnclosedContentItem(tagPair);
			PushContainer(tagPair);
			ProcessSubSegments(tagPair);
		}

		public virtual void InlineEndTag(IEndTagProperties tagInfo)
		{
			if (_structureMode)
			{
				SwitchMode(switchToStructure: false);
			}
			EnsureHasParagraphUnit();
			IEndTagProperties tagInfo2 = tagInfo.Clone() as IEndTagProperties;
			CloseLastOpenTag(tagInfo2);
			PopContainer();
		}

		public virtual void InlinePlaceholderTag(IPlaceholderTagProperties tagInfo)
		{
			if (_structureMode)
			{
				SwitchMode(switchToStructure: false);
			}
			EnsureHasParagraphUnit();
			IPlaceholderTagProperties tagInfo2 = tagInfo.Clone() as IPlaceholderTagProperties;
			IPlaceholderTag placeholderTag = ItemFactory.CreatePlaceholderTag(tagInfo2);
			AddContentItem(placeholderTag);
			ProcessSubSegments(placeholderTag);
		}

		public virtual void Text(ITextProperties textInfo)
		{
			EnsureHasParagraphUnit();
			ITextProperties textInfo2 = textInfo.Clone() as ITextProperties;
			IText item = ItemFactory.CreateText(textInfo2);
			AddContentItem(item);
		}

		public virtual void CustomInfo(ICustomInfoProperties info)
		{
			if (info.NamespaceUri == "http://sdl.com/FileTypeSupport/StructureBoundary")
			{
				SwitchMode(info.ValueXml == "start");
			}
		}

		public virtual void LocationMark(LocationMarkerId markerId)
		{
			EnsureHasParagraphUnit();
			ILocationMarker locationMarker = ItemFactory.CreateLocationMarker();
			locationMarker.MarkerId = (markerId.Clone() as LocationMarkerId);
			AddContentItem(locationMarker);
		}

		public virtual void ChangeContext(IContextProperties contexts)
		{
			if (contexts != null || _Contexts != null)
			{
				if (_CurrentParagraphUnit != null && _CurrentParagraphUnit.Source.Count != 0)
				{
					OutputCurrentParagraphUnit();
				}
				if (contexts == null)
				{
					_Contexts = null;
				}
				else
				{
					_Contexts = (IContextProperties)contexts.Clone();
				}
			}
		}

		public virtual void LockedContentStart(ILockedContentProperties lockedContentInfo)
		{
			if (_structureMode)
			{
				SwitchMode(switchToStructure: false);
			}
			EnsureHasParagraphUnit();
			ILockedContentProperties properties = lockedContentInfo.Clone() as ILockedContentProperties;
			ILockedContent lockedContent = ItemFactory.CreateLockedContent(properties);
			AddContentItem(lockedContent);
			PushUnclosedContentItem(lockedContent);
			PushContainer(lockedContent.Content);
		}

		public virtual void LockedContentEnd()
		{
			if (_structureMode)
			{
				SwitchMode(switchToStructure: false);
			}
			EnsureHasParagraphUnit();
			CloseLastOpenLockedContent();
			PopContainer();
		}

		public virtual void RevisionStart(IRevisionProperties revisionInfo)
		{
			if (_structureMode)
			{
				SwitchMode(switchToStructure: false);
			}
			EnsureHasParagraphUnit();
			IRevisionProperties properties = revisionInfo.Clone() as IRevisionProperties;
			IRevisionMarker revisionMarker = ItemFactory.CreateRevision(properties);
			AddContentItem(revisionMarker);
			PushUnclosedContentItem(revisionMarker);
			PushContainer(revisionMarker);
		}

		public virtual void RevisionEnd()
		{
			if (_structureMode)
			{
				SwitchMode(switchToStructure: false);
			}
			EnsureHasParagraphUnit();
			CloseLastOpenRevision();
			PopContainer();
		}

		public virtual void CommentStart(ICommentProperties commentInfo)
		{
			if (_structureMode)
			{
				SwitchMode(switchToStructure: false);
			}
			EnsureHasParagraphUnit();
			ICommentProperties comments = commentInfo.Clone() as ICommentProperties;
			ICommentMarker commentMarker = ItemFactory.CreateCommentMarker(comments);
			AddContentItem(commentMarker);
			PushUnclosedContentItem(commentMarker);
			PushContainer(commentMarker);
		}

		public virtual void CommentEnd()
		{
			if (_structureMode)
			{
				SwitchMode(switchToStructure: false);
			}
			EnsureHasParagraphUnit();
			CloseLastOpenComment();
			PopContainer();
		}

		public virtual void ParagraphComments(ICommentProperties commentInfo)
		{
			if (_structureMode)
			{
				SwitchMode(switchToStructure: false);
			}
			EnsureHasParagraphUnit();
			_CurrentParagraphUnit.Properties.Comments = commentInfo;
		}

		public virtual void SetFileProperties(IFileProperties properties)
		{
			_FileProperties = properties;
		}

		public virtual void StartOfInput()
		{
			if (_DocumentInfo == null)
			{
				CreateDocumentProperties();
				InitializeDocumentProperties();
			}
			CallInitialize(_DocumentInfo);
			if (_FileProperties == null)
			{
				CreateFileInfo();
			}
			_FileProperties.FileConversionProperties.SetMetaData("SDL:AutoClonedFlagSupported", true.ToString());
			CallSetFileProperties(_FileProperties);
		}

		public virtual void EndOfInput()
		{
			OutputCurrentParagraphUnit();
			CallFileComplete();
			CallComplete();
		}

		private void SwitchMode(bool switchToStructure)
		{
			if (_structureMode != switchToStructure)
			{
				OutputCurrentParagraphUnit();
				_structureMode = switchToStructure;
			}
		}

		private void EnsureHasParagraphUnit()
		{
			if (_DocumentInfo == null)
			{
				throw new FileTypeSupportException(StringResources.NativeToBilingualConverter_ProcessContentBeforeStartError);
			}
			if (_structureMode && !_hasStructureParagraph)
			{
				OutputCurrentParagraphUnit();
				_CurrentParagraphUnit = ItemFactory.CreateParagraphUnit(LockTypeFlags.Structure);
				_CurrentParagraphUnit.Properties.Contexts = _Contexts;
				_hasStructureParagraph = true;
			}
			else if (!_structureMode && !_hasLocalizableParagraph)
			{
				OutputCurrentParagraphUnit();
				_CurrentParagraphUnit = ItemFactory.CreateParagraphUnit(LockTypeFlags.Unlocked);
				_CurrentParagraphUnit.Properties.Contexts = _Contexts;
				_hasLocalizableParagraph = true;
				SetOpenContainers();
			}
		}

		private void SetOpenContainers()
		{
			_CurrentContainers.Clear();
			PushContainer(_CurrentParagraphUnit.Source);
			foreach (List<IAbstractMarkupData> item in _UnclosedContentPairs.Reverse())
			{
				ITagPair tagPair = item[0] as ITagPair;
				if (tagPair != null)
				{
					IStartTagProperties startTagProperties = tagPair.StartTagProperties;
					if (!startTagProperties.MetaDataContainsKey("SDL:AutoCloned"))
					{
						startTagProperties.SetMetaData("SDL:AutoCloned", true.ToString());
					}
					ITagPair tagPair2 = ItemFactory.CreateTagPair(tagPair.StartTagProperties, null);
					if (tagPair.HasSubSegmentReferences)
					{
						tagPair2.AddSubSegmentReferences(tagPair.SubSegments);
					}
					item.Add(tagPair2);
					AddContentItem(tagPair2);
					PushContainer(tagPair2);
				}
				else
				{
					ILockedContent lockedContent = item[0] as ILockedContent;
					if (lockedContent != null)
					{
						ILockedContent lockedContent2 = ItemFactory.CreateLockedContent(lockedContent.Properties);
						AddContentItem(lockedContent2);
						PushContainer(lockedContent2.Content);
					}
					else
					{
						IRevisionMarker revisionMarker = item[0] as IRevisionMarker;
						if (revisionMarker != null)
						{
							IRevisionMarker revisionMarker2 = ItemFactory.CreateRevision(revisionMarker.Properties);
							AddContentItem(revisionMarker2);
							PushContainer(revisionMarker2);
						}
						else
						{
							ICommentMarker commentMarker = item[0] as ICommentMarker;
							if (commentMarker == null)
							{
								throw new FileTypeSupportException(StringResources.NativeToBilingualConverter_UnknownItemError);
							}
							ICommentMarker commentMarker2 = ItemFactory.CreateCommentMarker(commentMarker.Comments);
							AddContentItem(commentMarker2);
							PushContainer(commentMarker2);
						}
					}
				}
			}
		}

		private void ProcessSubSegments(IAbstractTag tag)
		{
			if (_DocumentInfo == null)
			{
				throw new FileTypeSupportException(StringResources.NativeToBilingualConverter_ProcessContentBeforeStartError);
			}
			if (tag.TagProperties.HasLocalizableContent)
			{
				foreach (ISubSegmentProperties item in tag.TagProperties.LocalizableContent)
				{
					IParagraphUnit paragraphUnit = ItemFactory.CreateParagraphUnit(LockTypeFlags.Unlocked);
					ITextProperties textInfo = base.PropertiesFactory.CreateTextProperties(tag.TagProperties.TagContent.Substring(item.StartOffset, item.Length));
					paragraphUnit.Source.Add(ItemFactory.CreateText(textInfo));
					IContextProperties contextProperties;
					if (item.Contexts != null)
					{
						contextProperties = item.Contexts;
					}
					else
					{
						contextProperties = (item.Contexts = base.PropertiesFactory.CreateContextProperties());
						IContextInfo contextInfo = base.PropertiesFactory.CreateContextInfo("x-tm-tag");
						contextInfo.Purpose = ContextPurpose.Match;
						contextInfo.Description = tag.TagProperties.TagContent;
						contextInfo.SetMetaData("SDL:AddedByFramework", "SDL:CreatedByFramework");
						contextProperties.Contexts.Add(contextInfo);
					}
					IContextInfo contextInfo2 = base.PropertiesFactory.CreateContextInfo("sdl:transunit-ref");
					contextInfo2.Purpose = ContextPurpose.Location;
					contextInfo2.Description = _CurrentParagraphUnit.Properties.ParagraphUnitId.Id;
					contextInfo2.SetMetaData("SDL:AddedByFramework", true.ToString());
					contextProperties.Contexts.Add(contextInfo2);
					paragraphUnit.Properties.Contexts = contextProperties;
					ISubSegmentReference subSegmentReference = ItemFactory.CreateSubSegmentReference(item, paragraphUnit.Properties.ParagraphUnitId);
					tag.AddSubSegmentReference(subSegmentReference);
					_SubSegmentParagraphUnits.Add(paragraphUnit);
				}
			}
		}

		private void OutputCurrentParagraphUnit()
		{
			if (_CurrentParagraphUnit != null)
			{
				if (_CurrentParagraphUnit.IsStructure)
				{
					CallProcessParagraphUnit(_CurrentParagraphUnit);
				}
				else
				{
					if (HasUnclosedTagPair())
					{
						_ParagraphUnitBuffer.Hold();
					}
					else if (_ParagraphUnitBuffer.IsHolding)
					{
						_ParagraphUnitBuffer.Release();
					}
					CallProcessParagraphUnit(_CurrentParagraphUnit);
					_CurrentContainers.Clear();
				}
				_CurrentParagraphUnit = null;
			}
			OutputSubSegments();
			_hasLocalizableParagraph = false;
			_hasStructureParagraph = false;
		}

		private bool HasUnclosedTagPair()
		{
			return _UnclosedContentPairs.Any((List<IAbstractMarkupData> list) => list[0] is ITagPair);
		}

		private void PushUnclosedContentItem(IAbstractMarkupData item)
		{
			List<IAbstractMarkupData> list = new List<IAbstractMarkupData>();
			list.Add(item);
			_UnclosedContentPairs.Push(list);
		}

		private void CreateFileInfo()
		{
			_FileProperties = ItemFactory.CreateFileProperties();
		}

		private void InitializeDocumentProperties()
		{
			if (_FileProperties != null)
			{
				_DocumentInfo.SourceLanguage = _FileProperties.FileConversionProperties.SourceLanguage;
				_DocumentInfo.TargetLanguage = _FileProperties.FileConversionProperties.TargetLanguage;
			}
		}

		private void CreateDocumentProperties()
		{
			_DocumentInfo = ItemFactory.CreateDocumentProperties();
		}

		private void CloseLastOpenTag(IEndTagProperties tagInfo)
		{
			if (_UnclosedContentPairs.Count == 0)
			{
				throw new FileTypeSupportException(string.Format(StringResources.NativeToBilingualConverter_NoMatchingStartError, tagInfo.TagContent));
			}
			if (!(_UnclosedContentPairs.Peek()[0] is ITagPair))
			{
				throw new FileTypeSupportException(string.Format(StringResources.NativeToBilingualConverter_EndTagNestedLockedError, tagInfo.TagContent));
			}
			List<IAbstractMarkupData> list = _UnclosedContentPairs.Pop();
			foreach (ITagPair item in list)
			{
				item.EndTagProperties = tagInfo;
			}
		}

		private void CloseLastOpenLockedContent()
		{
			if (_UnclosedContentPairs.Count == 0)
			{
				throw new FileTypeSupportException(StringResources.NativeToBilingualConverter_MismatchLockedContentError);
			}
			if (!(_UnclosedContentPairs.Peek()[0] is ILockedContent))
			{
				throw new FileTypeSupportException(StringResources.NativeToBilingualConverter_LockedNestedTagError);
			}
			_UnclosedContentPairs.Pop();
		}

		private void CloseLastOpenRevision()
		{
			if (_UnclosedContentPairs.Count == 0)
			{
				throw new FileTypeSupportException(StringResources.NativeToBilingualConverter_MismatchRevisionError);
			}
			if (!(_UnclosedContentPairs.Peek()[0] is IRevisionMarker))
			{
				throw new FileTypeSupportException(StringResources.NativeToBilingualConverter_RevisionNestedTagError);
			}
			_UnclosedContentPairs.Pop();
		}

		private void CloseLastOpenComment()
		{
			if (_UnclosedContentPairs.Count == 0)
			{
				throw new FileTypeSupportException(StringResources.NativeToBilingualConverter_MismatchCommentError);
			}
			if (!(_UnclosedContentPairs.Peek()[0] is ICommentMarker))
			{
				throw new FileTypeSupportException(StringResources.NativeToBilingualConverter_CommentNestedTagError);
			}
			_UnclosedContentPairs.Pop();
		}

		private void AddContentItem(IAbstractMarkupData item)
		{
			if (_hasStructureParagraph)
			{
				_CurrentParagraphUnit.Source.Add(item);
				return;
			}
			if (CurrentContainer == null)
			{
				throw new FileTypeSupportException(StringResources.NativeToBilingualConverter_NoLocalizablePUPresentError);
			}
			CurrentContainer.Add(item);
		}

		private void PushContainer(IAbstractMarkupDataContainer container)
		{
			_CurrentContainers.Push(container);
		}

		private IAbstractMarkupDataContainer PopContainer()
		{
			if (_CurrentContainers.Count == 0)
			{
				throw new FileTypeSupportException(StringResources.NativeToBilingualConverter_RemoveFromEmptyStackError);
			}
			return _CurrentContainers.Pop();
		}
	}
}
