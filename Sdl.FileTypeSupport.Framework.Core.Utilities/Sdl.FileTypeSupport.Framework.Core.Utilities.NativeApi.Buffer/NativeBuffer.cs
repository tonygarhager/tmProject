using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi.Buffer
{
	public class NativeBuffer : INativeExtractionContentHandler, IAbstractNativeContentHandler, INativeGenerationContentHandler, INativeExtractionContentProcessor, INativeGenerationContentProcessor, INativeContentCycleAware
	{
		private IAbstractNativeContentHandler _output;

		private bool _IsHolding;

		private List<AbstractContentItem> _BufferedCalls = new List<AbstractContentItem>();

		private bool _IsEndOfInput;

		public virtual IAbstractNativeContentHandler Output
		{
			get
			{
				return _output;
			}
			set
			{
				_output = value;
			}
		}

		public virtual INativeExtractionContentHandler ExtractionOutput
		{
			get
			{
				return _output as INativeExtractionContentHandler;
			}
			set
			{
				_output = value;
			}
		}

		public virtual INativeGenerationContentHandler GenerationOutput
		{
			get
			{
				return _output as INativeGenerationContentHandler;
			}
			set
			{
				_output = value;
			}
		}

		public virtual bool IsEndOfInput
		{
			get
			{
				return _IsEndOfInput;
			}
			set
			{
				_IsEndOfInput = value;
			}
		}

		public List<AbstractContentItem> BufferedCalls => _BufferedCalls;

		public AbstractContentItem LastBufferedCall
		{
			get
			{
				if (_BufferedCalls.Count > 0)
				{
					return _BufferedCalls[_BufferedCalls.Count - 1];
				}
				return null;
			}
		}

		public bool IsHolding => _IsHolding;

		INativeExtractionContentHandler INativeExtractionContentProcessor.Output
		{
			get
			{
				return ExtractionOutput;
			}
			set
			{
				ExtractionOutput = value;
			}
		}

		INativeGenerationContentHandler INativeGenerationContentProcessor.Output
		{
			get
			{
				return GenerationOutput;
			}
			set
			{
				GenerationOutput = value;
			}
		}

		public virtual void Hold()
		{
			_IsHolding = true;
		}

		public virtual void Release()
		{
			if (_IsEndOfInput && _BufferedCalls.Count > 0)
			{
				throw new FileTypeSupportException("Buffer cannot release content after EndOfInput.");
			}
			foreach (AbstractContentItem bufferedCall in _BufferedCalls)
			{
				if (Output != null)
				{
					bufferedCall.Invoke(Output);
				}
			}
			_BufferedCalls.Clear();
			_IsHolding = false;
		}

		public virtual void StartOfInput()
		{
		}

		public virtual void EndOfInput()
		{
			_IsEndOfInput = true;
		}

		public virtual void SetFileProperties(IFileProperties properties)
		{
		}

		public virtual void StructureTag(IStructureTagProperties tagInfo)
		{
			if (_IsHolding)
			{
				_BufferedCalls.Add(new StructureTagContentItem(tagInfo));
			}
			else if (Output != null)
			{
				Output.StructureTag(tagInfo);
			}
		}

		public virtual void InlineStartTag(IStartTagProperties tagInfo)
		{
			if (_IsHolding)
			{
				_BufferedCalls.Add(new InlineStartTagContentItem(tagInfo));
			}
			else if (Output != null)
			{
				Output.InlineStartTag(tagInfo);
			}
		}

		public virtual void InlineEndTag(IEndTagProperties tagInfo)
		{
			if (_IsHolding)
			{
				_BufferedCalls.Add(new InlineEndTagContentItem(tagInfo));
			}
			else if (Output != null)
			{
				Output.InlineEndTag(tagInfo);
			}
		}

		public virtual void InlinePlaceholderTag(IPlaceholderTagProperties tagInfo)
		{
			if (_IsHolding)
			{
				_BufferedCalls.Add(new InlinePlaceholderTagContentItem(tagInfo));
			}
			else if (Output != null)
			{
				Output.InlinePlaceholderTag(tagInfo);
			}
		}

		public virtual void Text(ITextProperties textInfo)
		{
			if (_IsHolding)
			{
				_BufferedCalls.Add(new TextContentItem(textInfo));
			}
			else if (Output != null)
			{
				Output.Text(textInfo);
			}
		}

		public virtual void CustomInfo(ICustomInfoProperties info)
		{
			if (_IsHolding)
			{
				_BufferedCalls.Add(new CustomInfoContentItem(info));
			}
			else if (Output != null)
			{
				Output.CustomInfo(info);
			}
		}

		public virtual void LocationMark(LocationMarkerId markerId)
		{
			if (_IsHolding)
			{
				_BufferedCalls.Add(new LocationMarkContentItem(markerId));
			}
			else if (Output != null)
			{
				Output.LocationMark(markerId);
			}
		}

		public virtual void ChangeContext(IContextProperties contexts)
		{
			if (_IsHolding)
			{
				_BufferedCalls.Add(new ChangeContextContentItem(contexts));
			}
			else if (Output != null)
			{
				Output.ChangeContext(contexts);
			}
		}

		public virtual void LockedContentStart(ILockedContentProperties lockedContentInfo)
		{
			if (_IsHolding)
			{
				_BufferedCalls.Add(new LockedContentStartContentItem(lockedContentInfo));
			}
			else if (Output != null)
			{
				Output.LockedContentStart(lockedContentInfo);
			}
		}

		public virtual void LockedContentEnd()
		{
			if (_IsHolding)
			{
				_BufferedCalls.Add(new LockedContentEndContentItem());
			}
			else if (Output != null)
			{
				Output.LockedContentEnd();
			}
		}

		public virtual void RevisionStart(IRevisionProperties revisionInfo)
		{
			if (_IsHolding)
			{
				_BufferedCalls.Add(new RevisionStartContentItem(revisionInfo));
			}
			else if (Output != null)
			{
				Output.RevisionStart(revisionInfo);
			}
		}

		public virtual void RevisionEnd()
		{
			if (_IsHolding)
			{
				_BufferedCalls.Add(new RevisionEndContentItem());
			}
			else if (Output != null)
			{
				Output.RevisionEnd();
			}
		}

		public virtual void CommentStart(ICommentProperties commentInfo)
		{
			if (_IsHolding)
			{
				_BufferedCalls.Add(new CommentStartContentItem(commentInfo));
			}
			else if (Output != null)
			{
				Output.CommentStart(commentInfo);
			}
		}

		public virtual void CommentEnd()
		{
			if (_IsHolding)
			{
				_BufferedCalls.Add(new CommentEndContentItem());
			}
			else if (Output != null)
			{
				Output.CommentEnd();
			}
		}

		public virtual void ParagraphComments(ICommentProperties commentInfo)
		{
			if (_IsHolding)
			{
				_BufferedCalls.Add(new ParagraphComments(commentInfo));
			}
			else if (Output != null)
			{
				Output.ParagraphComments(commentInfo);
			}
		}

		public virtual void ParagraphUnitStart(IParagraphUnitProperties properties)
		{
			if (_IsHolding)
			{
				_BufferedCalls.Add(new ParagraphUnitStartContentItem(properties));
			}
			else if (GenerationOutput != null)
			{
				GenerationOutput.ParagraphUnitStart(properties);
			}
		}

		public virtual void ParagraphUnitEnd()
		{
			if (_IsHolding)
			{
				_BufferedCalls.Add(new ParagraphUnitEndContentItem());
			}
			else if (GenerationOutput != null)
			{
				GenerationOutput.ParagraphUnitEnd();
			}
		}

		public virtual void SegmentStart(ISegmentPairProperties properties)
		{
			if (_IsHolding)
			{
				_BufferedCalls.Add(new SegmentStartContentItem(properties));
			}
			else if (GenerationOutput != null)
			{
				GenerationOutput.SegmentStart(properties);
			}
		}

		public virtual void SegmentEnd()
		{
			if (_IsHolding)
			{
				_BufferedCalls.Add(new SegmentEndContentItem());
			}
			else if (GenerationOutput != null)
			{
				GenerationOutput.SegmentEnd();
			}
		}
	}
}
