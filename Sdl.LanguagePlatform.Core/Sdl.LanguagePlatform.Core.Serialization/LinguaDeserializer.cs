namespace Sdl.LanguagePlatform.Core.Serialization
{
	public class LinguaDeserializer
	{
		private static int _anchor;

		public Segment GetSegmentFromString(string content)
		{
			TokenEnumerator contentTokens = new TokenEnumerator(content);
			Segment result = new Segment();
			ParseContent(contentTokens, result);
			return result;
		}

		private void ParseContent(TokenEnumerator contentTokens, Segment result)
		{
			do
			{
				switch (contentTokens.Current.TokenType)
				{
				case TokenType.StartCommentMarker:
				case TokenType.StartLockedContent:
				case TokenType.StartRevisionMarkerInclude:
				case TokenType.StartRevisionMarkerDelete:
				case TokenType.StartTagPairContent:
					ParseContentTagPair(contentTokens, result);
					break;
				case TokenType.PlaceholderTag:
					ParseContentPlaceholder(contentTokens, result);
					break;
				case TokenType.EndCommentMarker:
				case TokenType.EndLockedContent:
				case TokenType.EndRevisionMarker:
				case TokenType.EndTagPairContent:
					contentTokens.MovePrevious();
					return;
				case TokenType.Text:
					result.Elements.Add(ParseContentText(contentTokens));
					break;
				}
			}
			while (contentTokens.MoveNext());
		}

		private static Text ParseContentText(TokenEnumerator contentTokens)
		{
			return new Text(contentTokens.Current.Text);
		}

		private void ParseContentTagPair(TokenEnumerator contentTokens, Segment result)
		{
			Tag tag = new Tag
			{
				Type = TagType.Start,
				Anchor = ++_anchor,
				TagID = _anchor.ToString()
			};
			result.Elements.Add(tag);
			contentTokens.MoveNext();
			ParseContent(contentTokens, result);
			Tag item = new Tag
			{
				Type = TagType.End,
				Anchor = tag.Anchor,
				TagID = tag.TagID
			};
			result.Elements.Add(item);
			contentTokens.MoveNext();
		}

		private static void ParseContentPlaceholder(TokenEnumerator contentTokens, Segment result)
		{
			Tag item = new Tag
			{
				Type = TagType.Standalone,
				Anchor = ++_anchor,
				TagID = contentTokens.Current.Text,
				TextEquivalent = contentTokens.Current.Text
			};
			result.Elements.Add(item);
		}
	}
}
