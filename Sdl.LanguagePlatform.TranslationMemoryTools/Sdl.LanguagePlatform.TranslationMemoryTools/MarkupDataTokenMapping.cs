using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.LanguagePlatform.TranslationMemoryTools
{
	public class MarkupDataTokenMapping
	{
		private class CharacterItem<T>
		{
			public int CharacterOffset
			{
				get;
			}

			public char Character
			{
				get;
			}

			public T Item
			{
				get;
			}

			public CharacterItem(int characterOffset, char character, T item)
			{
				CharacterOffset = characterOffset;
				Character = character;
				Item = item;
			}

			public override string ToString()
			{
				return CharacterOffset.ToString() + ",'" + Character.ToString() + "',[" + Item.GetType().Name + ";" + Item?.ToString() + "]";
			}
		}

		private class MarkupItemsVisitor : IMarkupDataVisitor
		{
			private readonly IList<CharacterItem<IAbstractMarkupData>> _markupItems = new List<CharacterItem<IAbstractMarkupData>>();

			private readonly bool _excludeTagsInLockedContentText;

			public MarkupItemsVisitor(bool excludeTagsInLockedContentText)
			{
				_excludeTagsInLockedContentText = excludeTagsInLockedContentText;
			}

			public IList<CharacterItem<IAbstractMarkupData>> GetMarkupItems()
			{
				return _markupItems;
			}

			public void VisitCommentMarker(ICommentMarker commentMarker)
			{
				VisitChildren(commentMarker);
			}

			public void VisitLocationMarker(ILocationMarker locationMarker)
			{
			}

			public void VisitLockedContent(ILockedContent lockedContent)
			{
				if (!_excludeTagsInLockedContentText)
				{
					AddText(lockedContent, lockedContent.Content.ToString());
				}
				else
				{
					VisitChildren(lockedContent.Content);
				}
			}

			public void VisitOtherMarker(IOtherMarker otherMarker)
			{
				VisitChildren(otherMarker);
			}

			public void VisitPlaceholderTag(IPlaceholderTag placeholderTag)
			{
			}

			public void VisitRevisionMarker(IRevisionMarker revisionMarker)
			{
				VisitChildren(revisionMarker);
			}

			public void VisitSegment(ISegment segment)
			{
				VisitChildren(segment);
			}

			public void VisitTagPair(ITagPair tagPair)
			{
				VisitChildren(tagPair);
			}

			public void VisitText(IText text)
			{
				AddText(text, text.Properties.Text);
			}

			private void VisitChildren(IAbstractMarkupDataContainer container)
			{
				foreach (IAbstractMarkupData item in container)
				{
					item.AcceptVisitor(this);
				}
			}

			private void AddText(IAbstractMarkupData markupData, string text)
			{
				int num = 0;
				foreach (char character in text)
				{
					_markupItems.Add(new CharacterItem<IAbstractMarkupData>(num++, character, markupData));
				}
			}
		}

		private readonly Segment _linguaSegment;

		private readonly bool _excludeTagsInLockedContentText;

		private readonly IList<CharacterItem<IAbstractMarkupData>> _markupItems;

		private readonly IList<CharacterItem<Token>> _linguaItems;

		private IList<MarkupDataRange> _tokenRanges;

		public MarkupDataTokenMapping(Segment linguaSegment, ISegment markupSegment, bool? acceptTrackChanges)
			: this(linguaSegment, markupSegment, acceptTrackChanges, excludeTagsInLockedContentText: true)
		{
		}

		public MarkupDataTokenMapping(Segment linguaSegment, ISegment markupSegment, bool? acceptTrackChanges, bool excludeTagsInLockedContentText)
		{
			_linguaSegment = (linguaSegment ?? throw new ArgumentNullException("linguaSegment"));
			if (markupSegment == null)
			{
				throw new ArgumentNullException("markupSegment");
			}
			_excludeTagsInLockedContentText = excludeTagsInLockedContentText;
			_markupItems = CalculateMarkupItems(markupSegment, acceptTrackChanges).ToList();
			_linguaItems = CalculateLinguaItems(_linguaSegment).ToList();
			if (_markupItems.Count != _linguaItems.Count)
			{
				throw new InvalidSegmentContentException("Different number of items in the markup segment than in the lingua segment.");
			}
			if (_markupItems.Where((CharacterItem<IAbstractMarkupData> t, int i) => t.Character != _linguaItems[i].Character).Any())
			{
				throw new InvalidSegmentContentException("Different text in the markup segment than in the lingua segment.");
			}
		}

		private IEnumerable<CharacterItem<IAbstractMarkupData>> CalculateMarkupItems(IAbstractMarkupData markupSegment, bool? acceptTrackChanges)
		{
			return RemoveWhitespaceItems(RemoveRevisionItems(GetMarkupItems(markupSegment), GetRevisionTypeToRemove(acceptTrackChanges)));
		}

		private IList<CharacterItem<IAbstractMarkupData>> GetMarkupItems(IAbstractMarkupData markupSegment)
		{
			MarkupItemsVisitor markupItemsVisitor = new MarkupItemsVisitor(_excludeTagsInLockedContentText);
			markupSegment.AcceptVisitor(markupItemsVisitor);
			return markupItemsVisitor.GetMarkupItems();
		}

		private static RevisionType GetRevisionTypeToRemove(bool? acceptTrackChanges)
		{
			if (!acceptTrackChanges.HasValue || acceptTrackChanges.Value)
			{
				return RevisionType.Delete;
			}
			return RevisionType.Insert;
		}

		private static IEnumerable<CharacterItem<IAbstractMarkupData>> RemoveRevisionItems(IEnumerable<CharacterItem<IAbstractMarkupData>> markupItems0, RevisionType revisionType)
		{
			return markupItems0.Where((CharacterItem<IAbstractMarkupData> markupItem0) => !InRevisionMarker(markupItem0.Item, revisionType));
		}

		private static IEnumerable<CharacterItem<T>> RemoveWhitespaceItems<T>(IEnumerable<CharacterItem<T>> items0)
		{
			return items0.Where((CharacterItem<T> item0) => !char.IsWhiteSpace(item0.Character));
		}

		private static bool InRevisionMarker(IAbstractMarkupData markupData, RevisionType revisionType)
		{
			if (markupData != null)
			{
				IRevisionMarker revisionMarker = markupData as IRevisionMarker;
				if (revisionMarker != null && revisionMarker.Properties.RevisionType == revisionType)
				{
					return true;
				}
				ILockedContainer lockedContainer = markupData.Parent as ILockedContainer;
				if (lockedContainer != null && lockedContainer.LockedContent != null)
				{
					return InRevisionMarker(lockedContainer.LockedContent.Parent as IAbstractMarkupData, revisionType);
				}
				return InRevisionMarker(markupData.Parent as IAbstractMarkupData, revisionType);
			}
			return false;
		}

		private static IEnumerable<CharacterItem<Token>> CalculateLinguaItems(Segment linguaSegment)
		{
			return RemoveWhitespaceItems(RemoveTagTokens(GetLinguaItems(linguaSegment)));
		}

		private static IEnumerable<CharacterItem<Token>> GetLinguaItems(Segment linguaSegment)
		{
			IList<CharacterItem<Token>> list = new List<CharacterItem<Token>>();
			foreach (Token token in linguaSegment.Tokens)
			{
				string tokenText = GetTokenText(token);
				int num = 0;
				string text = tokenText;
				foreach (char character in text)
				{
					list.Add(new CharacterItem<Token>(num++, character, token));
				}
			}
			return list;
		}

		private static string GetTokenText(Token token)
		{
			string result = token.Text;
			TagToken tagToken = token as TagToken;
			if (tagToken != null)
			{
				result = (tagToken.Tag?.TextEquivalent ?? "");
			}
			return result;
		}

		private static IEnumerable<CharacterItem<Token>> RemoveTagTokens(IEnumerable<CharacterItem<Token>> tokenItems0)
		{
			return tokenItems0.Where((CharacterItem<Token> tokenItem0) => !(tokenItem0.Item is TagToken) || IsLockedContentTagToken((TagToken)tokenItem0.Item));
		}

		private static bool IsLockedContentTagToken(TagToken tagToken)
		{
			if (tagToken?.Tag != null)
			{
				return tagToken.Tag.Type == TagType.LockedContent;
			}
			return false;
		}

		public IList<MarkupDataRange> GetTokenRanges()
		{
			return _tokenRanges ?? (_tokenRanges = CalculateTokenRanges(_linguaSegment.Tokens, _markupItems, _linguaItems));
		}

		private static IList<MarkupDataRange> CalculateTokenRanges(IEnumerable<Token> tokens, IList<CharacterItem<IAbstractMarkupData>> markupItems, IList<CharacterItem<Token>> linguaItems)
		{
			IList<MarkupDataRange> list = new List<MarkupDataRange>();
			int searchFromIndex = 0;
			foreach (Token token in tokens)
			{
				int tokenStartIndex = GetTokenStartIndex(token, linguaItems, searchFromIndex);
				int tokenFinishIndex = GetTokenFinishIndex(token, linguaItems, tokenStartIndex);
				if (tokenStartIndex != -1 && tokenFinishIndex != -1)
				{
					MarkupDataPosition from = new MarkupDataPosition(markupItems[tokenStartIndex].Item, markupItems[tokenStartIndex].CharacterOffset);
					MarkupDataPosition into = new MarkupDataPosition(markupItems[tokenFinishIndex].Item, markupItems[tokenFinishIndex].CharacterOffset);
					MarkupDataRange item = new MarkupDataRange(from, into);
					list.Add(item);
					searchFromIndex = tokenFinishIndex + 1;
				}
				else
				{
					list.Add(null);
				}
			}
			return list;
		}

		private static int GetTokenStartIndex(Token token, IList<CharacterItem<Token>> linguaItems, int searchFromIndex)
		{
			if (searchFromIndex < 0)
			{
				return -1;
			}
			for (int i = searchFromIndex; i < linguaItems.Count; i++)
			{
				if (linguaItems[i].Item == token)
				{
					return i;
				}
			}
			return -1;
		}

		private static int GetTokenFinishIndex(Token startToken, IList<CharacterItem<Token>> linguaItems, int startTokenIndex)
		{
			if (startTokenIndex < 0)
			{
				return -1;
			}
			for (int i = startTokenIndex + 1; i < linguaItems.Count; i++)
			{
				if (linguaItems[i].Item != startToken)
				{
					return i - 1;
				}
			}
			return linguaItems.Count - 1;
		}

		public MarkupDataPosition GetMarkupDataPosition(SegmentPosition segmentPosition)
		{
			Token token = GetToken(segmentPosition);
			if (token == null)
			{
				return null;
			}
			int position = token.Span.From.Position;
			int num = segmentPosition.Position - position;
			int tokenStartIndex = GetTokenStartIndex(token, _linguaItems, 0);
			int tokenFinishIndex = GetTokenFinishIndex(token, _linguaItems, tokenStartIndex);
			if (tokenStartIndex == -1 || tokenFinishIndex == -1)
			{
				return null;
			}
			for (int i = tokenStartIndex; i <= tokenFinishIndex; i++)
			{
				if (_linguaItems[i].CharacterOffset >= num)
				{
					return new MarkupDataPosition(_markupItems[i].Item, _markupItems[i].CharacterOffset);
				}
			}
			return null;
		}

		private Token GetToken(SegmentPosition segmentPosition)
		{
			return _linguaSegment.Tokens.FirstOrDefault((Token token) => IsInside(segmentPosition, token.Span));
		}

		private static bool IsInside(SegmentPosition position, SegmentRange range)
		{
			if (position.Index < range.From.Index || position.Index > range.Into.Index)
			{
				return false;
			}
			if (position.Index == range.From.Index && position.Position < range.From.Position)
			{
				return false;
			}
			if (position.Index == range.Into.Index)
			{
				return position.Position <= range.Into.Position;
			}
			return true;
		}
	}
}
