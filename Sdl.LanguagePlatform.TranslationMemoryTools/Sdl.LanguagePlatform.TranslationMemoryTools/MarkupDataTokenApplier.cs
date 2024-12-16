using Sdl.FileTypeSupport.Framework;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemoryTools
{
	public class MarkupDataTokenApplier
	{
		public const string TOKEN_MARKER_TYPE = "x-sdl-tm-token";

		private readonly List<Pair<IAbstractMarkupData, IAbstractMarkupData>> _tokenRanges = new List<Pair<IAbstractMarkupData, IAbstractMarkupData>>();

		public IList<Pair<IAbstractMarkupData, IAbstractMarkupData>> TokenRanges => _tokenRanges;

		public IDocumentItemFactory ItemFactory
		{
			get;
			set;
		}

		public ISegment MarkupDataSegment
		{
			get;
			private set;
		}

		public Segment LinguaSegment
		{
			get;
			set;
		}

		public MarkupDataTokenApplier(IDocumentItemFactory factory)
		{
			ItemFactory = (factory ?? throw new ArgumentNullException());
		}

		public Pair<IAbstractMarkupData, IAbstractMarkupData> GetRange(int tokenIndex)
		{
			if (tokenIndex < 0 || tokenIndex >= LinguaSegment.Tokens.Count)
			{
				throw new ArgumentOutOfRangeException();
			}
			return _tokenRanges[tokenIndex];
		}

		public void Execute(ISegment filterFrameworkSegment, Segment languagePlatformSegment)
		{
			LinguaSegment = (languagePlatformSegment ?? throw new ArgumentNullException("languagePlatformSegment"));
			Execute(filterFrameworkSegment);
		}

		public void Execute(ISegment markupDataSegment)
		{
			MarkupDataSegment = markupDataSegment;
			int num = -1;
			if (LinguaSegment.Tokens != null)
			{
				num = LinguaSegment.Tokens.Count - 1;
			}
			while (num >= 0)
			{
				Token token = LinguaSegment.Tokens[num];
				Location location = SplitBefore(token.Span.From);
				IAbstractMarkupData itemAtLocation = location.ItemAtLocation;
				IAbstractMarkupDataContainer abstractMarkupDataContainer = itemAtLocation as IAbstractMarkupDataContainer;
				IAbstractMarkupData abstractMarkupData;
				if (abstractMarkupDataContainer != null)
				{
					abstractMarkupData = ItemFactory.CreateLocationMarker();
					abstractMarkupDataContainer.Insert(0, abstractMarkupData);
					itemAtLocation = ItemFactory.CreateLocationMarker();
					location.BottomLevel.Parent.Insert(location.BottomLevel.Index, itemAtLocation);
				}
				else if (location.BottomLevel.IsAtEndOfParent && location.BottomLevel.Parent != null)
				{
					itemAtLocation = ItemFactory.CreateLocationMarker();
					location.BottomLevel.Parent.Add(itemAtLocation);
					abstractMarkupData = ItemFactory.CreateLocationMarker();
					LevelLocation levelLocation = location.Levels[location.Levels.Count - 2];
					levelLocation.Parent.Insert(levelLocation.Index + 1, abstractMarkupData);
				}
				else
				{
					ILocationMarker locationMarker = ItemFactory.CreateLocationMarker();
					itemAtLocation.Parent.Insert(itemAtLocation.IndexInParent, locationMarker);
					itemAtLocation = locationMarker;
					SegmentPosition segmentPosition = ConvertToUptoPosition(token.Span.Into);
					Location location2 = SplitBefore(segmentPosition);
					if (location.BottomLevel.Parent != location2.BottomLevel.Parent)
					{
						throw new InvalidSegmentContentException("Unexpected: first and last item in a token are on different levels!");
					}
					IAbstractMarkupDataContainer parent = location.BottomLevel.Parent;
					IOtherMarker otherMarker = ItemFactory.CreateOtherMarker();
					otherMarker.Id = num.ToString();
					otherMarker.MarkerType = "x-sdl-tm-token";
					for (int i = location.BottomLevel.Index; i < location2.BottomLevel.Index; i++)
					{
						IAbstractMarkupData itemAtLocation2 = location.ItemAtLocation;
						itemAtLocation2.RemoveFromParent();
						otherMarker.Add(itemAtLocation2);
					}
					abstractMarkupData = location.ItemAtLocation;
					parent.Insert(location.BottomLevel.Index, otherMarker);
					if (abstractMarkupData == null)
					{
						abstractMarkupData = ItemFactory.CreateLocationMarker();
						parent.Add(abstractMarkupData);
					}
					else
					{
						ILocationMarker locationMarker2 = ItemFactory.CreateLocationMarker();
						abstractMarkupData.Parent.Insert(abstractMarkupData.IndexInParent, locationMarker2);
						abstractMarkupData = locationMarker2;
					}
				}
				_tokenRanges.Insert(0, new Pair<IAbstractMarkupData, IAbstractMarkupData>(itemAtLocation, abstractMarkupData));
				num--;
			}
			ILocationMarker locationMarker3 = ItemFactory.CreateLocationMarker();
			MarkupDataSegment.Add(locationMarker3);
			_tokenRanges.Add(new Pair<IAbstractMarkupData, IAbstractMarkupData>(locationMarker3, null));
		}

		public static void RemoveAllTokenMarkup(ISegment markupDataSegment)
		{
			List<IOtherMarker> list = new List<IOtherMarker>();
			foreach (IAbstractMarkupData allSubItem in markupDataSegment.AllSubItems)
			{
				IOtherMarker otherMarker = allSubItem as IOtherMarker;
				if (otherMarker != null && otherMarker.MarkerType == "x-sdl-tm-token")
				{
					list.Add(otherMarker);
				}
			}
			foreach (IOtherMarker item in list)
			{
				item.MoveAllItemsTo(item.Parent, item.IndexInParent);
				item.RemoveFromParent();
			}
		}

		private Location SplitBefore(SegmentPosition segmentPosition)
		{
			MarkupDataSegmentPositionLocator markupDataSegmentPositionLocator = new MarkupDataSegmentPositionLocator(MarkupDataSegment);
			for (int i = 0; i < segmentPosition.Index; i++)
			{
				markupDataSegmentPositionLocator.StepOver(LinguaSegment.Elements[i]);
			}
			if (segmentPosition.Index >= LinguaSegment.Elements.Count)
			{
				return markupDataSegmentPositionLocator.Location;
			}
			if (segmentPosition.Position > 0)
			{
				markupDataSegmentPositionLocator.StepInto(LinguaSegment.Elements[segmentPosition.Index], segmentPosition.Position);
			}
			if (markupDataSegmentPositionLocator.OffsetIntoTextField == 0)
			{
				return markupDataSegmentPositionLocator.Location;
			}
			IText text = markupDataSegmentPositionLocator.ItemAtLocation as IText;
			IText text2 = text.Split(markupDataSegmentPositionLocator.OffsetIntoTextField);
			text.Parent.Insert(text.IndexInParent + 1, text2);
			return new Location(MarkupDataSegment, text2);
		}

		private SegmentPosition ConvertToUptoPosition(SegmentPosition intoPosition)
		{
			SegmentPosition segmentPosition = new SegmentPosition();
			Text text = LinguaSegment.Elements[intoPosition.Index] as Text;
			if (text != null)
			{
				if (text.Value.Length == intoPosition.Position + 1)
				{
					segmentPosition.Index = intoPosition.Index + 1;
					segmentPosition.Position = 0;
				}
				else
				{
					segmentPosition.Index = intoPosition.Index;
					segmentPosition.Position = intoPosition.Position + 1;
				}
			}
			else
			{
				segmentPosition.Index = intoPosition.Index + 1;
				segmentPosition.Position = 0;
			}
			return segmentPosition;
		}
	}
}
