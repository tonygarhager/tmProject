using Sdl.FileTypeSupport.Framework.BilingualApi;
using System;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public abstract class AbstractNativeFileWriter : INativeFileWriter, INativeGenerationContentHandler, IAbstractNativeContentHandler, INativeOutputSettingsAware, IDisposable
	{
		private INativeOutputFileProperties _outputProperties;

		private INativeTextLocationMessageReporter _messageReporter;

		private INativeLocationTracker _locationTracker;

		public virtual INativeOutputFileProperties OutputProperties
		{
			get
			{
				return _outputProperties;
			}
			protected set
			{
				_outputProperties = value;
			}
		}

		public virtual INativeTextLocationMessageReporter MessageReporter
		{
			get
			{
				return _messageReporter;
			}
			set
			{
				_messageReporter = value;
			}
		}

		public virtual INativeLocationTracker LocationTracker
		{
			get
			{
				return _locationTracker;
			}
			set
			{
				_locationTracker = value;
			}
		}

		public virtual void SetOutputProperties(INativeOutputFileProperties properties)
		{
			_outputProperties = properties;
		}

		public virtual void GetProposedOutputFileInfo(IPersistentFileConversionProperties fileProperties, IOutputFileInfo proposedFileInfo)
		{
		}

		public virtual void StructureTag(IStructureTagProperties tagInfo)
		{
		}

		public virtual void InlineStartTag(IStartTagProperties tagInfo)
		{
		}

		public virtual void InlineEndTag(IEndTagProperties tagInfo)
		{
		}

		public virtual void InlinePlaceholderTag(IPlaceholderTagProperties tagInfo)
		{
		}

		public virtual void Text(ITextProperties textInfo)
		{
		}

		public virtual void CustomInfo(ICustomInfoProperties info)
		{
		}

		public virtual void LocationMark(LocationMarkerId markerId)
		{
		}

		public virtual void ChangeContext(IContextProperties contexts)
		{
		}

		public virtual void LockedContentStart(ILockedContentProperties lockedContentInfo)
		{
		}

		public virtual void LockedContentEnd()
		{
		}

		public virtual void RevisionStart(IRevisionProperties revisionInfo)
		{
		}

		public virtual void RevisionEnd()
		{
		}

		public virtual void CommentStart(ICommentProperties commentInfo)
		{
		}

		public virtual void CommentEnd()
		{
		}

		public virtual void ParagraphComments(ICommentProperties commentInfo)
		{
		}

		public virtual void ParagraphUnitStart(IParagraphUnitProperties properties)
		{
		}

		public virtual void ParagraphUnitEnd()
		{
		}

		public virtual void SegmentStart(ISegmentPairProperties properties)
		{
		}

		public virtual void SegmentEnd()
		{
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
		}

		~AbstractNativeFileWriter()
		{
			Dispose(disposing: false);
		}
	}
}
