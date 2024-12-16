using Sdl.Core.Bcm.BcmModel.Operations.Merge;
using System;
using System.Linq;

namespace Sdl.Core.Bcm.BcmModel.Operations
{
	internal class SegmentSplitter
	{
		private readonly Segment _sourceSegment;

		private readonly MarkupData _location;

		private readonly int _splitIndex;

		private readonly string _prevSegmentNumber;

		public SegmentSplitter(Segment sourceSegment, MarkupData location, int splitIndex)
		{
			_sourceSegment = sourceSegment;
			_location = location;
			_splitIndex = splitIndex;
			_prevSegmentNumber = _sourceSegment.SegmentNumber;
		}

		public Segment Split()
		{
			if (!AllParentsSplittable(_location, _splitIndex))
			{
				throw new InvalidOperationException("Locked content and revision can not be split!");
			}
			if (!CanSplit(_location, _splitIndex))
			{
				throw new InvalidOperationException("Splitting would result in empty segments!");
			}
			Segment segment = (_splitIndex > -1) ? SplitSegmentText() : SplitSegmentParent();
			int indexInParent = _sourceSegment.IndexInParent;
			_sourceSegment.Parent.InsertAt(indexInParent + 1, segment);
			SplitTargetSegment(segment);
			CleanupSegment(_sourceSegment, moveLeftOutside: false, moveRightOutside: true);
			CleanupSegment(segment, moveLeftOutside: true, moveRightOutside: false);
			return segment;
		}

		private Segment SplitSegmentText()
		{
			TextMarkup textMarkup = _location as TextMarkup;
			if (textMarkup == null)
			{
				throw new InvalidOperationException("Invalid split location");
			}
			if (_splitIndex == 0)
			{
				return SplitSegmentParent();
			}
			textMarkup.SetMetadata("SDL:AutoCloned", true.ToString());
			TextMarkup textMarkup2 = textMarkup.Clone();
			textMarkup2.Id = Guid.NewGuid().ToString();
			textMarkup.Text = textMarkup.Text.Substring(0, _splitIndex);
			textMarkup2.Text = textMarkup2.Text.Substring(_splitIndex);
			if (!string.IsNullOrEmpty(textMarkup2.Text))
			{
				textMarkup.Parent.InsertAt(textMarkup.IndexInParent + 1, textMarkup2);
			}
			return SplitMarkupDataContainer(textMarkup.Parent, textMarkup.IndexInParent + 1);
		}

		private Segment SplitSegmentParent()
		{
			return SplitMarkupDataContainer(_location.Parent, _location.IndexInParent);
		}

		private void SplitTargetSegment(Segment splitSourceSegment)
		{
			Segment segment = _sourceSegment.ParentParagraphUnit.Target.AllSubItems.OfType<Segment>().First((Segment x) => x.SegmentNumber == _prevSegmentNumber);
			segment.SegmentNumber = _sourceSegment.SegmentNumber;
			Segment item = new Segment(splitSourceSegment.SegmentNumber);
			segment.Parent.InsertAt(segment.IndexInParent + 1, item);
		}

		private static Segment SplitMarkupDataContainer(MarkupDataContainer container, int splitIndex)
		{
			Segment segment = container as Segment;
			if (segment != null)
			{
				Segment segment2 = segment.Clone();
				segment2.Reset();
				segment2.Id = Guid.NewGuid().ToString();
				segment.SegmentNumber += " a";
				segment2.SegmentNumber += " b";
				segment.MoveToContainer(segment2, splitIndex, 0, segment.Children.Count - splitIndex);
				return segment2;
			}
			MarkupDataContainer item = container.SplitAt(splitIndex);
			container.Parent.InsertAt(container.IndexInParent + 1, item);
			return SplitMarkupDataContainer(container.Parent, container.IndexInParent + 1);
		}

		private static void CleanupSegment(Segment segment, bool moveLeftOutside, bool moveRightOutside)
		{
			ContainerCleaner containerCleaner = new ContainerCleaner(segment);
			containerCleaner.PerformCleanup(moveLeftOutside, moveRightOutside);
		}

		private bool CanSplit(MarkupData markupData, int splitIndex)
		{
			TextMarkup textMarkup = markupData as TextMarkup;
			if (textMarkup == null)
			{
				if (!ContainsValidMarkupAtLeft(markupData))
				{
					return false;
				}
				if (IsValidMarkup(markupData, checkRootText: false))
				{
					return true;
				}
				return ContainsValidMarkupAtRight(markupData);
			}
			if (string.IsNullOrEmpty(textMarkup.Text))
			{
				if (ContainsValidMarkupAtLeft(textMarkup))
				{
					return ContainsValidMarkupAtRight(textMarkup);
				}
				return false;
			}
			if (splitIndex <= 0)
			{
				if (ContainsValidMarkupAtLeft(markupData))
				{
					if (textMarkup.Text.All(char.IsWhiteSpace))
					{
						return ContainsValidMarkupAtRight(markupData);
					}
					return true;
				}
				return false;
			}
			if (textMarkup.Text.Length <= splitIndex)
			{
				if (ContainsValidMarkupAtRight(markupData))
				{
					if (textMarkup.Text.All(char.IsWhiteSpace))
					{
						return ContainsValidMarkupAtRight(markupData);
					}
					return true;
				}
				return false;
			}
			string source = textMarkup.Text.Substring(0, splitIndex);
			string source2 = textMarkup.Text.Substring(splitIndex);
			if (!source.All(char.IsWhiteSpace) || ContainsValidMarkupAtLeft(markupData))
			{
				if (source2.All(char.IsWhiteSpace))
				{
					return ContainsValidMarkupAtRight(markupData);
				}
				return true;
			}
			return false;
		}

		private bool ContainsValidMarkupAtLeft(MarkupData markupData)
		{
			return ContainsValidMarkup(markupData);
		}

		private bool ContainsValidMarkupAtRight(MarkupData markupData)
		{
			return ContainsValidMarkup(markupData, isLeft: false);
		}

		private bool ContainsValidMarkup(MarkupData markupData, bool isLeft = true)
		{
			if (markupData == null)
			{
				return false;
			}
			if (markupData == _sourceSegment)
			{
				return false;
			}
			int num = 0;
			int num2 = markupData.IndexInParent;
			if (!isLeft)
			{
				num = markupData.IndexInParent + 1;
				num2 = markupData.Parent.Children.Count;
			}
			for (int i = num; i < num2; i++)
			{
				MarkupData markupData2 = markupData.Parent.Children[i];
				if (IsValidMarkup(markupData2, checkRootText: true))
				{
					return true;
				}
			}
			return ContainsValidMarkup(markupData.Parent, isLeft);
		}

		private bool IsValidMarkup(MarkupData markupData, bool checkRootText)
		{
			if (checkRootText)
			{
				TextMarkup textMarkup = markupData as TextMarkup;
				if (!string.IsNullOrEmpty(textMarkup?.Text) && !textMarkup.Text.All(char.IsWhiteSpace))
				{
					return true;
				}
			}
			MarkupDataContainer markupDataContainer = markupData as MarkupDataContainer;
			if (markupDataContainer == null)
			{
				return true;
			}
			if (markupDataContainer.AllSubItems.Any((MarkupData child) => IsValidMarkup(child, checkRootText: true)))
			{
				return true;
			}
			return false;
		}

		private bool AllParentsSplittable(MarkupData markupData, int splitIndex)
		{
			if (splitIndex != 0)
			{
				return markupData.Ancestors.All((MarkupDataContainer p) => !(p is LockedContentContainer) && !(p is RevisionContainer));
			}
			MarkupDataContainer[] array = markupData.Ancestors.ToArray();
			MarkupData markupData2 = markupData;
			MarkupDataContainer[] array2 = array;
			foreach (MarkupDataContainer markupDataContainer in array2)
			{
				if (markupDataContainer is LockedContentContainer)
				{
					return false;
				}
				if (markupDataContainer is RevisionContainer && markupData2.IndexInParent > 0)
				{
					return false;
				}
				markupData2 = markupDataContainer;
			}
			return true;
		}
	}
}
