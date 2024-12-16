using Sdl.Core.Bcm.BcmModel.Skeleton;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.Bcm.BcmModel.Operations.Merge
{
	internal class ContainerCleaner
	{
		private MarkupDataContainer _container;

		public ContainerCleaner(MarkupDataContainer container)
		{
			_container = container;
		}

		public void PerformCleanup(bool moveLeftOutside, bool moveRightOutside)
		{
			int num;
			int num2;
			do
			{
				num = RemoveEmptyContainers(ref _container);
				num2 = MergeAdjacentClonedContainers(_container);
			}
			while (num != 0 || num2 != 0);
			MergeAdjacentText();
			bool flag;
			do
			{
				MoveTagPairsOutsideBoundary();
				flag = false;
				if (moveLeftOutside)
				{
					flag = MoveLeftWhiteSpacesOutsideBoundary();
				}
				if (moveRightOutside)
				{
					flag = (flag || MoveRightWhiteSpacesOutsideBoundary());
				}
			}
			while (flag);
			FixParents();
		}

		private void FixParents()
		{
			foreach (MarkupDataContainer item in _container.AllSubItems.OfType<MarkupDataContainer>())
			{
				foreach (MarkupData item2 in item)
				{
					item2.Parent = item;
				}
			}
		}

		private static int RemoveEmptyContainers(ref MarkupDataContainer container)
		{
			int num = 0;
			for (int i = 0; i < container.Count; i++)
			{
				MarkupDataContainer container2 = container[i] as MarkupDataContainer;
				if (container2 != null)
				{
					if (container2.Count == 0 && !(container2 is Segment))
					{
						container.RemoveAt(i);
						num++;
					}
					num += RemoveEmptyContainers(ref container2);
				}
			}
			return num;
		}

		private static int MergeAdjacentClonedContainers(MarkupDataContainer container)
		{
			int i = 0;
			int num = 0;
			for (; i < container.Count - 1; i++)
			{
				MarkupDataContainer first = container[i] as MarkupDataContainer;
				if (first != null)
				{
					MarkupDataContainer second = container[i + 1] as MarkupDataContainer;
					if (second != null && CanBeMerged(first, second))
					{
						num += MergeContainers(ref first, ref second);
						num += MergeAdjacentClonedContainers(first);
					}
				}
			}
			return num;
		}

		private static bool CanBeMerged(MarkupDataContainer first, MarkupDataContainer second)
		{
			if (first.MetaDataContainsKey("SDL:AutoCloned") && second.MetaDataContainsKey("SDL:AutoCloned"))
			{
				return first.FrameworkId == second.FrameworkId;
			}
			return false;
		}

		private static int MergeContainers(ref MarkupDataContainer first, ref MarkupDataContainer second)
		{
			List<MarkupData> elementsInBetween = first.GetElementsInBetween(second);
			if (elementsInBetween.Any((MarkupData x) => !(x is TextMarkup)))
			{
				return 0;
			}
			foreach (MarkupData item in elementsInBetween)
			{
				item.RemoveFromParent();
				first.Add(item);
			}
			second.RemoveFromParent();
			if (second.Count > 0)
			{
				first.AddRange(second.Children);
			}
			return 1;
		}

		private void MergeAdjacentText()
		{
			AdjacentTextMergerVisitor adjacentTextMergerVisitor = new AdjacentTextMergerVisitor();
			adjacentTextMergerVisitor.VisitChildren(_container);
		}

		private void MoveTagPairsOutsideBoundary()
		{
			while (_container.Count == 1)
			{
				TagPair tagPair = _container[0] as TagPair;
				if (tagPair != null)
				{
					TagPairDefinition definition = tagPair.Definition;
					if (definition == null || definition.SegmentationHint != SegmentationHint.MayExclude)
					{
						break;
					}
					while (tagPair.Count > 0)
					{
						MarkupData markupData = tagPair[0];
						markupData.RemoveFromParent();
						_container.Add(markupData);
					}
					MarkupDataContainer parent = _container.Parent;
					int indexInParent = _container.IndexInParent;
					tagPair.RemoveFromParent();
					_container.RemoveFromParent();
					tagPair.Add(_container);
					parent.InsertAt(indexInParent, tagPair);
					continue;
				}
				break;
			}
		}

		private bool MoveLeftWhiteSpacesOutsideBoundary()
		{
			return MoveWhiteSpacesOutsideBoundary(left: true, () => 0, (TextMarkup textMarkup) => 0);
		}

		private bool MoveRightWhiteSpacesOutsideBoundary()
		{
			return MoveWhiteSpacesOutsideBoundary(left: false, () => _container.Children.Count - 1, (TextMarkup textMarkup) => textMarkup.Text.Length - 1);
		}

		private bool MoveWhiteSpacesOutsideBoundary(bool left, Func<int> containerIndex, Func<TextMarkup, int> textIndex)
		{
			if (_container.Children.Count > 0)
			{
				TextMarkup textMarkup = _container[containerIndex()] as TextMarkup;
				if (textMarkup == null)
				{
					return false;
				}
				string text = null;
				while (!string.IsNullOrEmpty(textMarkup.Text))
				{
					int num = textIndex(textMarkup);
					char c = textMarkup.Text[num];
					if (!char.IsWhiteSpace(c))
					{
						break;
					}
					text += textMarkup.Text[num].ToString();
					textMarkup.Text = (left ? textMarkup.Text.Substring(1) : textMarkup.Text.Substring(0, num));
				}
				if (string.IsNullOrEmpty(text))
				{
					return false;
				}
				MoveTextOutside(text, textMarkup, left);
				return true;
			}
			return false;
		}

		private void MoveTextOutside(string extractedText, TextMarkup textmarkup, bool left)
		{
			TextMarkup item = new TextMarkup
			{
				Text = extractedText
			};
			int indexInParent = _container.IndexInParent;
			_container.Parent.InsertAt(left ? indexInParent : (indexInParent + 1), item);
			if (string.IsNullOrEmpty(textmarkup.Text))
			{
				_container.Remove(textmarkup);
			}
		}
	}
}
