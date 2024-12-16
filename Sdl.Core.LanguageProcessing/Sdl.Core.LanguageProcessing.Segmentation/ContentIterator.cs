using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;

namespace Sdl.Core.LanguageProcessing.Segmentation
{
	internal class ContentIterator
	{
		private int _countTagParents;

		private int _countMayExcludeTagParents;

		private int _countIncludeTagParents;

		private int _countTagStops;

		private int _countRevisionParents;

		private int _countCommentParents;

		private readonly Dictionary<int, SegmentationInfo> _segmentationHints;

		private readonly bool _skipSegments;

		public Location CurrentLocation
		{
			get;
		}

		public IAbstractMarkupData CurrentData => CurrentLocation?.ItemAtLocation;

		public ContentIterator(Location startLocation, Dictionary<int, SegmentationInfo> segmentationHints = null, bool skipsegments = true)
		{
			_skipSegments = skipsegments;
			if (startLocation == null)
			{
				throw new ArgumentNullException("startLocation");
			}
			_countRevisionParents = 0;
			_countCommentParents = 0;
			_countTagStops = 0;
			_countTagParents = 0;
			_countIncludeTagParents = 0;
			CurrentLocation = (Location)startLocation.Clone();
			_segmentationHints = segmentationHints;
			if (skipsegments && CurrentLocation.ItemAtLocation is ISegment)
			{
				CurrentLocation.MoveNextSibling();
			}
		}

		public bool NextPotentialInlineElement()
		{
			while (Next())
			{
				if (CurrentLocation.ItemAtLocation != null && _segmentationHints.ContainsKey(CurrentLocation.ItemAtLocation.UniqueId) && _segmentationHints[CurrentLocation.ItemAtLocation.UniqueId].IsInlineElement)
				{
					return true;
				}
			}
			return false;
		}

		public void NextSibbling()
		{
			int num = ++CurrentLocation.BottomLevel.Index;
		}

		public bool Next()
		{
			if (!CurrentLocation.IsValid)
			{
				return false;
			}
			int num;
			if (CurrentLocation.BottomLevel.IsAtEndOfParent)
			{
				CurrentLocation.Levels.Remove(CurrentLocation.BottomLevel);
				if (!CurrentLocation.IsValid)
				{
					return false;
				}
				num = ++CurrentLocation.BottomLevel.Index;
				return true;
			}
			IAbstractMarkupDataContainer abstractMarkupDataContainer = CurrentData as IAbstractMarkupDataContainer;
			if (abstractMarkupDataContainer != null && (!(CurrentData is ISegment) || !_skipSegments))
			{
				CurrentLocation.Levels.Add(new LevelLocation(abstractMarkupDataContainer, 0));
				return true;
			}
			num = ++CurrentLocation.BottomLevel.Index;
			return true;
		}

		public bool NextWithCounts()
		{
			if (!CurrentLocation.IsValid)
			{
				return false;
			}
			int num;
			if (CurrentLocation.BottomLevel.IsAtEndOfParent)
			{
				RemoveContainer();
				CurrentLocation.Levels.Remove(CurrentLocation.BottomLevel);
				if (!CurrentLocation.IsValid)
				{
					return false;
				}
				num = ++CurrentLocation.BottomLevel.Index;
				return true;
			}
			IAbstractMarkupDataContainer abstractMarkupDataContainer = CurrentData as IAbstractMarkupDataContainer;
			if (abstractMarkupDataContainer != null && !(CurrentData is ISegment))
			{
				AddContainerCount();
				CurrentLocation.Levels.Add(new LevelLocation(abstractMarkupDataContainer, 0));
				return true;
			}
			num = ++CurrentLocation.BottomLevel.Index;
			return true;
		}

		private void AddContainerCount()
		{
			UpdateContainerCounters(CurrentLocation.ItemAtLocation, 1);
		}

		private void RemoveContainer()
		{
			UpdateContainerCounters(CurrentLocation.BottomLevel.Parent as IAbstractMarkupData, -1);
		}

		private void UpdateContainerCounters(IAbstractMarkupData container, int delta)
		{
			ITagPair tagPair = container as ITagPair;
			if (tagPair == null)
			{
				if (!(container is IRevisionMarker))
				{
					if (container is ICommentMarker)
					{
						_countCommentParents += delta;
					}
				}
				else
				{
					_countRevisionParents += delta;
				}
				return;
			}
			_countTagParents += delta;
			switch (tagPair.StartTagProperties.SegmentationHint)
			{
			case SegmentationHint.Include:
			case SegmentationHint.IncludeWithText:
				_countIncludeTagParents += delta;
				break;
			case SegmentationHint.MayExclude:
				_countMayExcludeTagParents += delta;
				break;
			}
			if (tagPair.StartTagProperties.IsWordStop)
			{
				_countTagStops += delta;
			}
		}

		public bool IsInsideRevisionContainer()
		{
			return _countRevisionParents > 0;
		}

		public bool IsInsideCommentContainer()
		{
			return _countCommentParents > 0;
		}

		public bool IsInsideTagIncludeContainer()
		{
			return _countIncludeTagParents > 0;
		}

		public bool IsInsideTagContainer()
		{
			return _countTagParents > 0;
		}

		public bool IsInsideMayExcludeTagIncludeContainer()
		{
			return _countMayExcludeTagParents > 0;
		}

		public bool IsInsideTagStopContainer()
		{
			return _countTagStops > 0;
		}
	}
}
