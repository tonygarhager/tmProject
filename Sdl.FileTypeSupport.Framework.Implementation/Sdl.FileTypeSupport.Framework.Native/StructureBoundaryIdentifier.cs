using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi.Buffer;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Globalization;

namespace Sdl.FileTypeSupport.Framework.Native
{
	internal class StructureBoundaryIdentifier : AbstractNativeFileTypeComponent, INativeExtractionContentProcessor, INativeExtractionContentHandler, IAbstractNativeContentHandler, INativeContentCycleAware
	{
		public const string StructureBoundaryNamespaceUri = "http://sdl.com/FileTypeSupport/StructureBoundary";

		public const string Start = "start";

		public const string End = "end";

		private NativeBuffer _outputBuffer = new NativeBuffer();

		private ITextProperties _textThatMayNeedSplitting;

		private bool _structureMode;

		public INativeExtractionContentHandler Output
		{
			get
			{
				return _outputBuffer.ExtractionOutput;
			}
			set
			{
				_outputBuffer.ExtractionOutput = value;
			}
		}

		private void SetStructureMode(bool isStructure)
		{
			if (isStructure != _structureMode)
			{
				ICustomInfoProperties customInfoProperties = PropertiesFactory.CreateCustomInfoProperties();
				customInfoProperties.NamespaceUri = "http://sdl.com/FileTypeSupport/StructureBoundary";
				customInfoProperties.ValueXml = (isStructure ? "start" : "end");
				if (!_structureMode)
				{
					if (_textThatMayNeedSplitting != null)
					{
						Pair<ITextProperties, ITextProperties> pair = SplitBeforeEndingStructureContent(_textThatMayNeedSplitting);
						if (pair.First != null)
						{
							_outputBuffer.Output.Text(pair.First);
						}
						if (pair.Second != null)
						{
							((TextContentItem)_outputBuffer.BufferedCalls[0]).Properties = pair.Second;
						}
						else
						{
							_outputBuffer.BufferedCalls.RemoveAt(0);
						}
					}
					_outputBuffer.Output.CustomInfo(customInfoProperties);
				}
				else
				{
					_outputBuffer.CustomInfo(customInfoProperties);
				}
			}
			_textThatMayNeedSplitting = null;
			_outputBuffer.Release();
			_structureMode = isStructure;
		}

		public void EndOfInput()
		{
			if (_structureMode)
			{
				ICustomInfoProperties customInfoProperties = PropertiesFactory.CreateCustomInfoProperties();
				customInfoProperties.NamespaceUri = "http://sdl.com/FileTypeSupport/StructureBoundary";
				customInfoProperties.ValueXml = "end";
				_outputBuffer.CustomInfo(customInfoProperties);
			}
			_outputBuffer.Release();
			_outputBuffer.EndOfInput();
		}

		public void SetFileProperties(IFileProperties properties)
		{
			_outputBuffer.SetFileProperties(properties);
		}

		public void StartOfInput()
		{
			_outputBuffer.StartOfInput();
			_structureMode = true;
			ICustomInfoProperties customInfoProperties = PropertiesFactory.CreateCustomInfoProperties();
			customInfoProperties.NamespaceUri = "http://sdl.com/FileTypeSupport/StructureBoundary";
			customInfoProperties.ValueXml = "start";
			_outputBuffer.Hold();
			_outputBuffer.CustomInfo(customInfoProperties);
		}

		public void ChangeContext(IContextProperties newContexts)
		{
			_outputBuffer.ChangeContext(newContexts);
		}

		public void CustomInfo(ICustomInfoProperties info)
		{
			_outputBuffer.CustomInfo(info);
		}

		public void InlineEndTag(IEndTagProperties tagInfo)
		{
			SetStructureMode(isStructure: false);
			_outputBuffer.InlineEndTag(tagInfo);
		}

		public void InlinePlaceholderTag(IPlaceholderTagProperties tagInfo)
		{
			SetStructureMode(isStructure: false);
			_outputBuffer.InlinePlaceholderTag(tagInfo);
		}

		public void InlineStartTag(IStartTagProperties tagInfo)
		{
			SetStructureMode(isStructure: false);
			_outputBuffer.InlineStartTag(tagInfo);
		}

		public void LocationMark(LocationMarkerId markerId)
		{
			_outputBuffer.LocationMark(markerId);
		}

		public void LockedContentEnd()
		{
			SetStructureMode(isStructure: false);
			_outputBuffer.LockedContentEnd();
		}

		public void LockedContentStart(ILockedContentProperties lockedContentInfo)
		{
			SetStructureMode(isStructure: false);
			_outputBuffer.LockedContentStart(lockedContentInfo);
		}

		public void RevisionStart(IRevisionProperties revisionInfo)
		{
			SetStructureMode(isStructure: false);
			_outputBuffer.RevisionStart(revisionInfo);
		}

		public void RevisionEnd()
		{
			SetStructureMode(isStructure: false);
			_outputBuffer.RevisionEnd();
		}

		public void CommentStart(ICommentProperties commentInfo)
		{
			SetStructureMode(isStructure: false);
			_outputBuffer.CommentStart(commentInfo);
		}

		public void CommentEnd()
		{
			SetStructureMode(isStructure: false);
			_outputBuffer.CommentEnd();
		}

		public void ParagraphComments(ICommentProperties commentInfo)
		{
			SetStructureMode(isStructure: false);
			_outputBuffer.ParagraphComments(commentInfo);
		}

		public void StructureTag(IStructureTagProperties tagInfo)
		{
			SetStructureMode(isStructure: true);
			_outputBuffer.StructureTag(tagInfo);
		}

		public void Text(ITextProperties textInfo)
		{
			if (string.IsNullOrEmpty(textInfo.Text))
			{
				return;
			}
			if (_structureMode)
			{
				Pair<ITextProperties, ITextProperties> pair = SplitAtFirstNonStructureContent(textInfo);
				if (pair.First != null)
				{
					_outputBuffer.Text(pair.First);
				}
				if (pair.Second == null)
				{
					return;
				}
				textInfo = pair.Second;
				SetStructureMode(isStructure: false);
			}
			if (string.IsNullOrEmpty(textInfo.Text))
			{
				return;
			}
			if (_outputBuffer.IsHolding)
			{
				if (!TextHasNonStructureContentCharacters(textInfo))
				{
					_outputBuffer.Text(textInfo);
					return;
				}
				SetStructureMode(isStructure: false);
			}
			if (TextEndsInStructureContentCharacter(textInfo))
			{
				_outputBuffer.Hold();
				_textThatMayNeedSplitting = textInfo;
			}
			_outputBuffer.Text(textInfo);
		}

		private static bool TextHasNonStructureContentCharacters(ITextProperties textInfo)
		{
			string text = textInfo.Text;
			foreach (char c in text)
			{
				if (!CharacterCanBePartOfStructureContent(c))
				{
					return true;
				}
			}
			return false;
		}

		private static bool TextEndsInStructureContentCharacter(ITextProperties textInfo)
		{
			return CharacterCanBePartOfStructureContent(textInfo.Text[textInfo.Text.Length - 1]);
		}

		private static Pair<ITextProperties, ITextProperties> SplitAtFirstNonStructureContent(ITextProperties textInfo)
		{
			int i;
			for (i = 0; i < textInfo.Text.Length && CharacterCanBePartOfStructureContent(textInfo.Text[i]); i++)
			{
			}
			return SplitText(textInfo, i);
		}

		private static Pair<ITextProperties, ITextProperties> SplitBeforeEndingStructureContent(ITextProperties textInfo)
		{
			int num;
			for (num = textInfo.Text.Length - 1; num >= 0; num--)
			{
				if (!CharacterCanBePartOfStructureContent(textInfo.Text[num]))
				{
					num++;
					break;
				}
			}
			return SplitText(textInfo, num);
		}

		private static Pair<ITextProperties, ITextProperties> SplitText(ITextProperties textInfo, int splitIndex)
		{
			if (splitIndex <= 0)
			{
				return new Pair<ITextProperties, ITextProperties>(null, textInfo);
			}
			if (splitIndex >= textInfo.Text.Length)
			{
				return new Pair<ITextProperties, ITextProperties>(textInfo, null);
			}
			ITextProperties textProperties = (ITextProperties)textInfo.Clone();
			textProperties.Text = textInfo.Text.Substring(0, splitIndex);
			ITextProperties textProperties2 = (ITextProperties)textInfo.Clone();
			textProperties2.Text = textInfo.Text.Substring(splitIndex);
			return new Pair<ITextProperties, ITextProperties>(textProperties, textProperties2);
		}

		private static bool CharacterCanBePartOfStructureContent(char c)
		{
			switch (CharUnicodeInfo.GetUnicodeCategory(c))
			{
			case UnicodeCategory.SpaceSeparator:
				return true;
			case UnicodeCategory.LineSeparator:
				return true;
			case UnicodeCategory.ParagraphSeparator:
				return true;
			case UnicodeCategory.Control:
				return true;
			default:
				return false;
			}
		}
	}
}
