using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Native;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Sdl.FileTypeSupport.Framework.Bilingual
{
	internal class WhitespaceBetweenSegmentsBilingualProcessor : AbstractBilingualContentProcessor
	{
		private readonly IPersistentFileConversionProperties _fileProperties;

		private IAbstractMarkupData _currentNode;

		private IAbstractMarkupData _alreadyVisitedNode;

		public WhitespaceBetweenSegmentsBilingualProcessor(IPersistentFileConversionProperties fileProperties)
		{
			_fileProperties = fileProperties;
		}

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			Action<IAbstractMarkupData> action = DecideSegmentProcessing();
			if (action == null)
			{
				Output.ProcessParagraphUnit(paragraphUnit);
				return;
			}
			IParagraphUnit paragraphUnit2 = ProcessParagraphUnitWhitespaces(paragraphUnit, action);
			paragraphUnit2.Properties.Contexts = paragraphUnit.Properties.Contexts;
			Output.ProcessParagraphUnit(paragraphUnit2);
		}

		private IParagraphUnit ProcessParagraphUnitWhitespaces(IParagraphUnit paragraphUnit, Action<IAbstractMarkupData> processSegment)
		{
			IParagraphUnit paragraphUnit2 = paragraphUnit.Clone() as IParagraphUnit;
			int num = 0;
			foreach (ISegmentPair segmentPair in paragraphUnit2.SegmentPairs)
			{
				if (num == 0 && IsSpecialAsianLanguage() && !IsCopiedFromSourceToTarget(segmentPair.Target))
				{
					RemoveFirstSpaceInsideSegment(segmentPair.Target);
				}
				if (num < paragraphUnit2.SegmentPairs.Count() - 1)
				{
					if (!NeedToSkipWhitespaceProcessing(segmentPair.Target))
					{
						processSegment(segmentPair.Target);
					}
					num++;
				}
			}
			return paragraphUnit2;
		}

		private bool NeedToSkipWhitespaceProcessing(ISegment segment)
		{
			bool flag = IsCopiedFromSourceToTarget(segment);
			bool flag2 = IsCopiedFromSourceToTarget(GetNextSegment(segment));
			if (flag && flag2)
			{
				return true;
			}
			bool flag3 = flag | flag2;
			bool flag4 = LanguageUsesSpaces(_fileProperties.SourceLanguage);
			return flag3 && flag4;
		}

		private bool IsCopiedFromSourceToTarget(ISegment targetSegment)
		{
			if (targetSegment == null)
			{
				return false;
			}
			bool flag = targetSegment.Properties.TranslationOrigin?.OriginType == "source";
			bool flag2 = !targetSegment.Any();
			bool flag3 = SegmentIsNotConfirmed(targetSegment);
			bool flag4 = flag2 && flag3;
			return flag | flag4;
		}

		private bool SegmentIsNotConfirmed(ISegment segment)
		{
			ConfirmationLevel confirmationLevel = segment.Properties.ConfirmationLevel;
			if (confirmationLevel != ConfirmationLevel.ApprovedSignOff && confirmationLevel != ConfirmationLevel.ApprovedTranslation)
			{
				return confirmationLevel != ConfirmationLevel.Translated;
			}
			return false;
		}

		private bool IsSpecialAsianLanguage()
		{
			CultureInfo cultureInfo = _fileProperties.SourceLanguage.CultureInfo;
			CultureInfo cultureInfo2 = _fileProperties.TargetLanguage.CultureInfo;
			if (cultureInfo == null || cultureInfo2 == null)
			{
				return false;
			}
			if (cultureInfo2.Name.Contains("ja") || cultureInfo2.Name.Contains("zh"))
			{
				return true;
			}
			return false;
		}

		private Action<IAbstractMarkupData> DecideSegmentProcessing()
		{
			CultureInfo cultureInfo = _fileProperties.SourceLanguage.CultureInfo;
			CultureInfo cultureInfo2 = _fileProperties.TargetLanguage.CultureInfo;
			if (cultureInfo == null || cultureInfo2 == null)
			{
				return null;
			}
			if (cultureInfo2.Name.Contains("ja") || cultureInfo2.Name.Contains("zh"))
			{
				return HandleJapaneseWhitespaceRequirements;
			}
			bool flag = LanguageUsesSpaces(_fileProperties.SourceLanguage);
			bool flag2 = LanguageUsesSpaces(_fileProperties.TargetLanguage);
			if (flag && !flag2)
			{
				return HandleNonAsianToAsianRequirements;
			}
			if (!flag && flag2)
			{
				return HandleAsianToNonAsianRequirements;
			}
			return null;
		}

		private bool LanguageUsesSpaces(Language language)
		{
			if (!language.UseBlankAsSentenceSeparator)
			{
				return language.UseBlankAsWordSeparator;
			}
			return true;
		}

		private void HandleNonAsianToAsianRequirements(IAbstractMarkupData markupData)
		{
			RemoveFirstSpaceInsideSegment(markupData);
			RemoveLastSpaceInsideSegment(markupData);
			RemoveAllWhitespaceBetweenSegments(markupData);
		}

		private void HandleAsianToNonAsianRequirements(IAbstractMarkupData markupData)
		{
			int totalWhitespaceCountAtSegmentBoundary = GetTotalWhitespaceCountAtSegmentBoundary(markupData);
			bool flag = IsAutoclonedTagPairBetweenOrInNextSegment(markupData);
			if (totalWhitespaceCountAtSegmentBoundary == 0)
			{
				if (flag)
				{
					AddWhitespaceInsideEndOfSegment(markupData);
				}
				else
				{
					AddWhitespaceAfterCurrentSegmentOrInsideIText(markupData);
				}
			}
		}

		private void HandleJapaneseWhitespaceRequirements(IAbstractMarkupData markupData)
		{
			int totalWhitespaceCountAtSegmentBoundary = GetTotalWhitespaceCountAtSegmentBoundary(markupData);
			IText text = IsSpecialCharacter(markupData);
			bool flag = IsAutoclonedTagPairBetweenOrInNextSegment(markupData);
			int num = CountWhitespaceBetweenSegments(markupData);
			switch (totalWhitespaceCountAtSegmentBoundary)
			{
			case 1:
				if (text != null)
				{
					if (flag && num > 0)
					{
						RemoveAllWhitespaceBetweenSegments(markupData);
						AddWhitespaceAfterCurrentSegmentOrInsideIText(text);
					}
				}
				else
				{
					RemoveFirstSpaceInsideSegment(markupData);
					RemoveLastSpaceInsideSegment(markupData);
					RemoveAllWhitespaceBetweenSegments(markupData);
				}
				return;
			case 0:
				if (text != null)
				{
					if (flag)
					{
						AddWhitespaceAfterCurrentSegmentOrInsideIText(text);
					}
					else
					{
						AddWhitespaceAfterCurrentSegmentOrInsideIText(markupData);
					}
				}
				return;
			}
			if (totalWhitespaceCountAtSegmentBoundary > 1 && flag && num > 0)
			{
				RemoveAllWhitespaceBetweenSegments(markupData);
				for (int i = 0; i < num; i++)
				{
					AddWhitespaceInsideEndOfSegment(markupData);
				}
			}
		}

		private int GetTotalWhitespaceCountAtSegmentBoundary(IAbstractMarkupData markupData)
		{
			int num = CountWhitespaceInsideEndOfSegment(markupData);
			ISegment nextSegment = GetNextSegment(markupData);
			int num2 = (nextSegment != null) ? CountWhitespaceInsideStartOfSegment(nextSegment) : 0;
			int num3 = CountWhitespaceBetweenSegments(markupData);
			return num + num2 + num3;
		}

		private ISegment GetNextSegment(IAbstractMarkupData markupData)
		{
			if (!(markupData is ISegment))
			{
				return null;
			}
			_currentNode = markupData;
			while (Next() != null && !(_currentNode is ISegment))
			{
			}
			return _currentNode as ISegment;
		}

		private bool IsAutoclonedTagPairBetweenOrInNextSegment(IAbstractMarkupData markupData)
		{
			ISegment nextSegment = GetNextSegment(markupData);
			if (nextSegment == null)
			{
				return false;
			}
			_currentNode = markupData;
			while (Next() != null && _currentNode != nextSegment)
			{
				if (_currentNode is ITagPair)
				{
					string text = (_currentNode as ITagPair)?.StartTagProperties.GetMetaData("SDL:AutoCloned");
					if (text != null)
					{
						return true;
					}
				}
			}
			ITagPair tagPair = (nextSegment == null || nextSegment.Count == 0) ? null : (nextSegment[0] as ITagPair);
			return IsAutoClonedTagPair(tagPair);
		}

		private bool IsAutoClonedTagPair(ITagPair tagPair)
		{
			string text = tagPair?.StartTagProperties.GetMetaData("SDL:AutoCloned");
			if (text != null)
			{
				return true;
			}
			return false;
		}

		private void RemoveLastSpaceInsideSegment(IAbstractMarkupData markupData)
		{
			if (!(markupData is ISegment))
			{
				return;
			}
			IAbstractMarkupDataContainer abstractMarkupDataContainer = markupData as IAbstractMarkupDataContainer;
			if (abstractMarkupDataContainer.Count == 0)
			{
				return;
			}
			IText lastTextItemInSegment = GetLastTextItemInSegment(markupData);
			if (lastTextItemInSegment != null && lastTextItemInSegment.Properties.Text.Length == 1)
			{
				char c = lastTextItemInSegment.Properties.Text[0];
				if (char.GetUnicodeCategory(c) == UnicodeCategory.SpaceSeparator)
				{
					lastTextItemInSegment.Parent.Remove(lastTextItemInSegment);
				}
			}
			else if (lastTextItemInSegment != null && lastTextItemInSegment.Properties.Text.Length > 0)
			{
				char c2 = lastTextItemInSegment.Properties.Text[lastTextItemInSegment.Properties.Text.Length - 1];
				if (char.GetUnicodeCategory(c2) == UnicodeCategory.SpaceSeparator)
				{
					lastTextItemInSegment.Properties.Text = lastTextItemInSegment.Properties.Text.Substring(0, lastTextItemInSegment.Properties.Text.Length - 1);
				}
			}
		}

		private void RemoveFirstSpaceInsideSegment(IAbstractMarkupData markupData)
		{
			if (!(markupData is ISegment))
			{
				return;
			}
			IAbstractMarkupDataContainer abstractMarkupDataContainer = markupData as IAbstractMarkupDataContainer;
			if (abstractMarkupDataContainer.Count == 0)
			{
				return;
			}
			IText firstTextItemInSegment = GetFirstTextItemInSegment(markupData);
			if (firstTextItemInSegment != null && firstTextItemInSegment.Properties.Text.Length == 1)
			{
				char c = firstTextItemInSegment.Properties.Text[0];
				if (char.GetUnicodeCategory(c) == UnicodeCategory.SpaceSeparator)
				{
					firstTextItemInSegment.Parent.Remove(firstTextItemInSegment);
				}
			}
			else if (firstTextItemInSegment != null && firstTextItemInSegment.Properties.Text.Length > 0)
			{
				char c2 = firstTextItemInSegment.Properties.Text[0];
				if (char.GetUnicodeCategory(c2) == UnicodeCategory.SpaceSeparator)
				{
					firstTextItemInSegment.Properties.Text = firstTextItemInSegment.Properties.Text.Substring(1, firstTextItemInSegment.Properties.Text.Length - 1);
				}
			}
		}

		private IText IsSpecialCharacter(IAbstractMarkupData markupData)
		{
			if (markupData is ISegment)
			{
				IAbstractMarkupDataContainer abstractMarkupDataContainer = markupData as IAbstractMarkupDataContainer;
				if (abstractMarkupDataContainer.Count == 0)
				{
					return null;
				}
				int num = abstractMarkupDataContainer.Count - 1;
				IText text = abstractMarkupDataContainer[num] as IText;
				if (text != null)
				{
					while (num >= 0 && text != null)
					{
						num--;
						IText text2 = CheckForSpecialCharacter(text);
						if (text2 != null)
						{
							return text2;
						}
						if (num < 0)
						{
							return null;
						}
						text = (abstractMarkupDataContainer[num] as IText);
					}
				}
				else
				{
					IAbstractMarkupData abstractMarkupData = abstractMarkupDataContainer[abstractMarkupDataContainer.Count - 1];
					IAbstractMarkupDataContainer abstractMarkupDataContainer2 = null;
					while (abstractMarkupData is IAbstractMarkupDataContainer && ((IAbstractMarkupDataContainer)abstractMarkupData).Count > 0)
					{
						abstractMarkupDataContainer2 = (abstractMarkupData as IAbstractMarkupDataContainer);
						abstractMarkupData = abstractMarkupDataContainer2[abstractMarkupDataContainer2.Count - 1];
					}
					if (abstractMarkupDataContainer2 != null)
					{
						int num2 = abstractMarkupDataContainer2.Count - 1;
						abstractMarkupData = abstractMarkupDataContainer2[num2];
						text = (abstractMarkupData as IText);
						if (text != null)
						{
							while (num2 >= 0 && text != null)
							{
								num2--;
								IText text3 = CheckForSpecialCharacter(text);
								if (text3 != null)
								{
									return text3;
								}
								if (num2 < 0)
								{
									return null;
								}
								text = (abstractMarkupDataContainer2[num2] as IText);
							}
						}
					}
				}
			}
			return null;
		}

		private IText CheckForSpecialCharacter(IText text)
		{
			for (int num = text.Properties.Text.Length - 1; num >= 0; num--)
			{
				char c = text.Properties.Text[num];
				if (c == ':' || c == ';' || c == '?' || c == '!')
				{
					return text;
				}
				if (char.GetUnicodeCategory(c) != UnicodeCategory.SpaceSeparator)
				{
					return null;
				}
			}
			return null;
		}

		private void RemoveAllWhitespaceBetweenSegments(IAbstractMarkupData markupData)
		{
			List<IText> list = new List<IText>();
			_currentNode = markupData;
			while (Next() != null && !(_currentNode is ISegment))
			{
				IText text = _currentNode as IText;
				if (text != null && MarkupDataContainsOnlySpaces(text))
				{
					list.Add(text);
				}
			}
			foreach (IText item in list)
			{
				item.Parent.Remove(item);
			}
		}

		private int CountWhitespaceInsideEndOfSegment(IAbstractMarkupData markupData)
		{
			int num = 0;
			IText lastTextItemInSegment = GetLastTextItemInSegment(markupData);
			if (lastTextItemInSegment != null)
			{
				if (!MarkupDataContainsOnlySpaces(lastTextItemInSegment))
				{
					return num + CountSpacesAtEndOfIText(lastTextItemInSegment);
				}
				num += lastTextItemInSegment.Properties.Text.Length;
				lastTextItemInSegment = GetPreviousITextInBranch(lastTextItemInSegment);
			}
			return num;
		}

		private int CountWhitespaceInsideStartOfSegment(IAbstractMarkupData markupData)
		{
			int num = 0;
			IText firstTextItemInSegment = GetFirstTextItemInSegment(markupData);
			if (firstTextItemInSegment != null)
			{
				if (!MarkupDataContainsOnlySpaces(firstTextItemInSegment))
				{
					return num + CountSpacesAtStartOfIText(firstTextItemInSegment);
				}
				num += firstTextItemInSegment.Properties.Text.Length;
				firstTextItemInSegment = GetNextITextInBranch(firstTextItemInSegment);
			}
			return num;
		}

		private int CountSpacesAtEndOfIText(IText text)
		{
			int num = 0;
			int length = text.Properties.Text.Length;
			for (int num2 = length - 1; num2 >= 0; num2--)
			{
				char c = text.Properties.Text[num2];
				if (char.GetUnicodeCategory(c) != UnicodeCategory.SpaceSeparator)
				{
					break;
				}
				num++;
			}
			return num;
		}

		private int CountSpacesAtStartOfIText(IText text)
		{
			int num = 0;
			int length = text.Properties.Text.Length;
			for (int i = 0; i <= length - 1; i++)
			{
				char c = text.Properties.Text[i];
				if (char.GetUnicodeCategory(c) != UnicodeCategory.SpaceSeparator)
				{
					break;
				}
				num++;
			}
			return num;
		}

		private IText GetPreviousITextInBranch(IAbstractMarkupData markupData)
		{
			if (markupData.IndexInParent == 0)
			{
				return null;
			}
			IAbstractMarkupData abstractMarkupData = markupData.Parent[markupData.IndexInParent - 1];
			return abstractMarkupData as IText;
		}

		private IText GetNextITextInBranch(IAbstractMarkupData markupData)
		{
			int indexInParent = markupData.IndexInParent;
			if (indexInParent >= markupData.Parent.Count - 1)
			{
				return null;
			}
			IAbstractMarkupData abstractMarkupData = markupData.Parent[markupData.IndexInParent + 1];
			return abstractMarkupData as IText;
		}

		private int CountWhitespaceBetweenSegments(IAbstractMarkupData markupData)
		{
			List<IText> list = new List<IText>();
			_currentNode = markupData;
			while (Next() != null && !(_currentNode is ISegment))
			{
				if (_currentNode is IText)
				{
					IText text = _currentNode as IText;
					if (text != null && MarkupDataContainsOnlySpaces(text))
					{
						list.Add(text);
					}
				}
			}
			return list.Sum((IText item) => item.Properties.Text.Length);
		}

		private void AddWhitespaceAfterCurrentSegmentOrInsideIText(IAbstractMarkupData markupData)
		{
			IAbstractMarkupDataContainer abstractMarkupDataContainer = markupData as IAbstractMarkupDataContainer;
			IText text = markupData as IText;
			IText item = ItemFactory.CreateText(new TextProperties
			{
				Text = ' '.ToString(CultureInfo.InvariantCulture)
			});
			if (abstractMarkupDataContainer != null)
			{
				markupData.Parent.Insert(markupData.IndexInParent + 1, item);
			}
			if (text != null)
			{
				text.Properties.Text += ' '.ToString(CultureInfo.InvariantCulture);
			}
		}

		private void AddWhitespaceInsideEndOfSegment(IAbstractMarkupData markupData)
		{
			IAbstractMarkupDataContainer abstractMarkupDataContainer = markupData as IAbstractMarkupDataContainer;
			if (abstractMarkupDataContainer is ISegment && abstractMarkupDataContainer.Count != 0)
			{
				ITagPair tagPair = abstractMarkupDataContainer[abstractMarkupDataContainer.Count - 1] as ITagPair;
				if (IsAutoClonedTagPair(tagPair))
				{
					abstractMarkupDataContainer = tagPair;
				}
				IText item = ItemFactory.CreateText(new TextProperties
				{
					Text = ' '.ToString(CultureInfo.InvariantCulture)
				});
				abstractMarkupDataContainer?.Insert(abstractMarkupDataContainer.Count, item);
			}
		}

		private bool MarkupDataContainsOnlySpaces(IText text)
		{
			return text.Properties.Text.All((char c) => char.GetUnicodeCategory(c) == UnicodeCategory.SpaceSeparator);
		}

		private IText GetFirstTextItemInSegment(IAbstractMarkupData segment)
		{
			IAbstractMarkupDataContainer abstractMarkupDataContainer = segment as IAbstractMarkupDataContainer;
			if (abstractMarkupDataContainer != null && abstractMarkupDataContainer.Count > 0)
			{
				_currentNode = abstractMarkupDataContainer[0];
				if (abstractMarkupDataContainer.Count == 1 && _currentNode is IText)
				{
					return _currentNode as IText;
				}
				while (_currentNode != null && !(_currentNode is IText))
				{
					Next();
				}
				if (_currentNode != null && _currentNode.IndexInParent == 0)
				{
					return _currentNode as IText;
				}
			}
			return null;
		}

		private IText GetLastTextItemInSegment(IAbstractMarkupData segment)
		{
			IAbstractMarkupDataContainer abstractMarkupDataContainer = segment as IAbstractMarkupDataContainer;
			if (abstractMarkupDataContainer != null && abstractMarkupDataContainer.Count > 0)
			{
				_currentNode = abstractMarkupDataContainer[abstractMarkupDataContainer.Count - 1];
				if (abstractMarkupDataContainer.Count == 1 && _currentNode is IText)
				{
					return _currentNode as IText;
				}
				while (_currentNode != null && !(_currentNode is IText))
				{
					Previous();
				}
				if (_currentNode != null && _currentNode.Parent != null && _currentNode.IndexInParent == _currentNode.Parent.Count - 1)
				{
					return _currentNode as IText;
				}
			}
			return null;
		}

		private IAbstractMarkupData Next()
		{
			if (_currentNode.IndexInParent == _currentNode.Parent.Count - 1 && (!(_currentNode is IAbstractMarkupDataContainer) || ((IAbstractMarkupDataContainer)_currentNode).Count <= 0))
			{
				_currentNode = null;
				return null;
			}
			if (_currentNode is IAbstractMarkupDataContainer && !(_currentNode is ISegment) && ((IAbstractMarkupDataContainer)_currentNode).Count > 0)
			{
				_currentNode = ((IAbstractMarkupDataContainer)_currentNode)[0];
			}
			else if (_currentNode.IndexInParent + 1 < _currentNode.Parent.Count)
			{
				_currentNode = _currentNode.Parent[_currentNode.IndexInParent + 1];
			}
			else if (_currentNode.Parent != null && !(_currentNode.Parent is IParagraph))
			{
				while (_currentNode.Parent != null && !(_currentNode.Parent is IParagraph) && _currentNode.IndexInParent + 1 > _currentNode.Parent.Count - 1)
				{
					_currentNode = (_currentNode.Parent as IAbstractMarkupData);
				}
				if (_currentNode.IndexInParent + 1 > _currentNode.Parent.Count - 1)
				{
					_currentNode = null;
					return null;
				}
				_currentNode = _currentNode.Parent[_currentNode.IndexInParent + 1];
			}
			return _currentNode;
		}

		private void Previous()
		{
			if (_currentNode.IndexInParent == 0 || _currentNode is ISegment)
			{
				_currentNode = null;
				return;
			}
			if (_alreadyVisitedNode == null && !(_currentNode is IAbstractMarkupDataContainer) && _currentNode.Parent != null)
			{
				_alreadyVisitedNode = (_currentNode.Parent as IAbstractMarkupData);
			}
			IAbstractMarkupDataContainer abstractMarkupDataContainer = _currentNode as IAbstractMarkupDataContainer;
			if (abstractMarkupDataContainer != null && abstractMarkupDataContainer.Count > 0 && _alreadyVisitedNode == null)
			{
				while (_currentNode is IAbstractMarkupDataContainer && ((IAbstractMarkupDataContainer)_currentNode).Count > 0)
				{
					_currentNode = ((IAbstractMarkupDataContainer)_currentNode)[((IAbstractMarkupDataContainer)_currentNode).Count - 1];
				}
				_alreadyVisitedNode = _currentNode;
			}
			else if (_currentNode.Parent != null && _currentNode.IndexInParent > 0)
			{
				_currentNode = _currentNode.Parent[_currentNode.IndexInParent - 1];
				while (_currentNode is IAbstractMarkupDataContainer && ((IAbstractMarkupDataContainer)_currentNode).Count > 0)
				{
					_currentNode = ((IAbstractMarkupDataContainer)_currentNode)[((IAbstractMarkupDataContainer)_currentNode).Count - 1];
				}
			}
			else if (!(_currentNode.Parent is IParagraph))
			{
				_currentNode = (_currentNode.Parent as IAbstractMarkupData);
			}
		}
	}
}
