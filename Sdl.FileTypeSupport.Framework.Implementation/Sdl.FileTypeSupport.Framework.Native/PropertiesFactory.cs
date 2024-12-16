using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.Formatting;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.Formatting;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Globalization;

namespace Sdl.FileTypeSupport.Framework.Native
{
	public class PropertiesFactory : IPropertiesFactory
	{
		private uint _NextTagId;

		private IFormattingItemFactory _FormattingItemFactory = new FormattingItemFactory();

		public IFormattingItemFactory FormattingItemFactory
		{
			get
			{
				return _FormattingItemFactory;
			}
			set
			{
				_FormattingItemFactory = value;
			}
		}

		public ITextProperties CreateTextProperties(string text)
		{
			TextProperties textProperties = new TextProperties();
			textProperties.Text = text;
			return textProperties;
		}

		public IPlaceholderTagProperties CreatePlaceholderTagProperties(string tagContent)
		{
			IPlaceholderTagProperties placeholderTagProperties = new PlaceholderTagProperties();
			placeholderTagProperties.TagId = new TagId(_NextTagId.ToString(CultureInfo.InvariantCulture));
			_NextTagId++;
			placeholderTagProperties.TagContent = tagContent;
			return placeholderTagProperties;
		}

		public IStructureTagProperties CreateStructureTagProperties(string tagContent)
		{
			IStructureTagProperties structureTagProperties = new StructureTagProperties();
			structureTagProperties.TagId = new TagId(_NextTagId.ToString(CultureInfo.InvariantCulture));
			_NextTagId++;
			structureTagProperties.TagContent = tagContent;
			return structureTagProperties;
		}

		public ICustomInfoProperties CreateCustomInfoProperties()
		{
			return new CustomInfoProperties();
		}

		public IStartTagProperties CreateStartTagProperties(string tagContent)
		{
			IStartTagProperties startTagProperties = new StartTagProperties();
			startTagProperties.TagId = new TagId(_NextTagId.ToString(CultureInfo.InvariantCulture));
			_NextTagId++;
			startTagProperties.TagContent = tagContent;
			startTagProperties.DisplayText = "";
			return startTagProperties;
		}

		public IEndTagProperties CreateEndTagProperties(string tagContent)
		{
			IEndTagProperties endTagProperties = new EndTagProperties();
			endTagProperties.TagContent = tagContent;
			endTagProperties.DisplayText = "";
			return endTagProperties;
		}

		public ISubSegmentProperties CreateSubSegmentProperties(int offset, int length)
		{
			SubSegmentProperties subSegmentProperties = new SubSegmentProperties();
			subSegmentProperties.StartOffset = offset;
			subSegmentProperties.Length = length;
			return subSegmentProperties;
		}

		public IContextProperties CreateContextProperties()
		{
			return new ContextProperties();
		}

		public IContextInfo CreateContextInfo(string contextType)
		{
			IContextInfo contextInfo = new ContextInfo();
			contextInfo.ContextType = contextType;
			StandardContextTypes.ContextData contextData = null;
			if (contextType != null && StandardContextTypes.StandardContextData.ContainsKey(contextType))
			{
				contextData = StandardContextTypes.StandardContextData[contextType];
				contextInfo.DisplayName = contextData.Name;
				contextInfo.DisplayCode = contextData.Code;
				contextInfo.Description = contextData.Description;
				contextInfo.DisplayColor = contextData.Color;
			}
			return contextInfo;
		}

		public ILockedContentProperties CreateLockedContentProperties(LockTypeFlags lockType)
		{
			ILockedContentProperties lockedContentProperties = new LockedContentProperties();
			lockedContentProperties.LockType = lockType;
			return lockedContentProperties;
		}

		public IStructureInfo CreateStructureInfo(IContextInfo contextInfo, bool mustUseDisplayName, IStructureInfo parentStructure)
		{
			return new StructureInfo(contextInfo, mustUseDisplayName, parentStructure);
		}

		public IStructureInfo CreateStructureInfo()
		{
			return new StructureInfo();
		}

		public IDependencyFileProperties CreateDependencyFileProperties(string currentFilePath)
		{
			return new DependencyFileProperties(currentFilePath);
		}

		public ICommentProperties CreateCommentProperties()
		{
			return new CommentProperties();
		}

		public IComment CreateComment(string text, string author, Severity severity)
		{
			Comment comment = new Comment();
			comment.Text = text;
			comment.Author = author;
			comment.Severity = severity;
			comment.Date = DateTime.Now;
			return comment;
		}

		public IRevisionProperties CreateRevisionProperties(RevisionType type)
		{
			IRevisionProperties revisionProperties = new RevisionProperties();
			revisionProperties.RevisionType = type;
			return revisionProperties;
		}

		public IRevisionProperties CreateFeedbackProperties(RevisionType type)
		{
			IRevisionProperties revisionProperties = new FeedbackProperties();
			revisionProperties.RevisionType = type;
			return revisionProperties;
		}
	}
}
