using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.FileTypeSupport.Framework.Native
{
	public class NativeTextNormalizer : INativeExtractionContentProcessor, INativeExtractionContentHandler, IAbstractNativeContentHandler, INativeGenerationContentProcessor, INativeGenerationContentHandler, INativeContentCycleAware
	{
		private IAbstractNativeContentHandler _output;

		private ITextProperties _precedingText;

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

		private void OutputPrecedingText()
		{
			if (_precedingText != null)
			{
				if (_output != null)
				{
					_output.Text(_precedingText);
				}
				_precedingText = null;
			}
		}

		public void SetFileProperties(IFileProperties properties)
		{
		}

		public void StartOfInput()
		{
		}

		public void EndOfInput()
		{
			OutputPrecedingText();
		}

		public void StructureTag(IStructureTagProperties tagInfo)
		{
			OutputPrecedingText();
			if (_output != null)
			{
				_output.StructureTag(tagInfo);
			}
		}

		public void InlineStartTag(IStartTagProperties tagInfo)
		{
			OutputPrecedingText();
			if (_output != null)
			{
				_output.InlineStartTag(tagInfo);
			}
		}

		public void InlineEndTag(IEndTagProperties tagInfo)
		{
			OutputPrecedingText();
			if (_output != null)
			{
				_output.InlineEndTag(tagInfo);
			}
		}

		public void InlinePlaceholderTag(IPlaceholderTagProperties tagInfo)
		{
			OutputPrecedingText();
			if (_output != null)
			{
				_output.InlinePlaceholderTag(tagInfo);
			}
		}

		public void Text(ITextProperties textInfo)
		{
			if (_precedingText != null)
			{
				_precedingText.Text += textInfo.Text;
			}
			else
			{
				_precedingText = (ITextProperties)textInfo.Clone();
			}
		}

		public void ChangeContext(IContextProperties newContexts)
		{
			OutputPrecedingText();
			if (_output != null)
			{
				_output.ChangeContext(newContexts);
			}
		}

		public void CustomInfo(ICustomInfoProperties info)
		{
			OutputPrecedingText();
			if (_output != null)
			{
				_output.CustomInfo(info);
			}
		}

		public void LocationMark(LocationMarkerId markerId)
		{
			OutputPrecedingText();
			if (_output != null)
			{
				_output.LocationMark(markerId);
			}
		}

		public void LockedContentStart(ILockedContentProperties lockedContentInfo)
		{
			OutputPrecedingText();
			if (_output != null)
			{
				_output.LockedContentStart(lockedContentInfo);
			}
		}

		public void LockedContentEnd()
		{
			OutputPrecedingText();
			if (_output != null)
			{
				_output.LockedContentEnd();
			}
		}

		public void RevisionStart(IRevisionProperties revisionInfo)
		{
			OutputPrecedingText();
			if (_output != null)
			{
				_output.RevisionStart(revisionInfo);
			}
		}

		public void RevisionEnd()
		{
			OutputPrecedingText();
			if (_output != null)
			{
				_output.RevisionEnd();
			}
		}

		public void CommentStart(ICommentProperties commentInfo)
		{
			OutputPrecedingText();
			if (_output != null)
			{
				_output.CommentStart(commentInfo);
			}
		}

		public void CommentEnd()
		{
			OutputPrecedingText();
			if (_output != null)
			{
				_output.CommentEnd();
			}
		}

		public void ParagraphComments(ICommentProperties commentInfo)
		{
			OutputPrecedingText();
			if (_output != null)
			{
				_output.ParagraphComments(commentInfo);
			}
		}

		public void ParagraphUnitStart(IParagraphUnitProperties properties)
		{
			OutputPrecedingText();
			if (GenerationOutput != null)
			{
				GenerationOutput.ParagraphUnitStart(properties);
			}
		}

		public void ParagraphUnitEnd()
		{
			OutputPrecedingText();
			if (GenerationOutput != null)
			{
				GenerationOutput.ParagraphUnitEnd();
			}
		}

		public void SegmentStart(ISegmentPairProperties properties)
		{
			OutputPrecedingText();
			if (GenerationOutput != null)
			{
				GenerationOutput.SegmentStart(properties);
			}
		}

		public void SegmentEnd()
		{
			OutputPrecedingText();
			if (GenerationOutput != null)
			{
				GenerationOutput.ParagraphUnitEnd();
			}
		}
	}
}
