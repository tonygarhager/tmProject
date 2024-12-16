using Sdl.Core.Processing.Alignment.Tokenization;
using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.Core.Processing.Alignment.Core
{
	internal class InvertedIndex
	{
		internal IDictionary<string, HashSet<AlignmentElement>> TokenTextElementsIndex = new Dictionary<string, HashSet<AlignmentElement>>();

		private readonly TokensContainer _tokensContainer;

		public InvertedIndex(TokensContainer tokensContainer)
		{
			if (tokensContainer == null)
			{
				throw new ArgumentNullException("tokensContainer");
			}
			_tokensContainer = tokensContainer;
		}

		public void AddElements(IEnumerable<AlignmentElement> elements)
		{
			if (elements == null)
			{
				throw new ArgumentNullException("elements");
			}
			foreach (AlignmentElement element in elements)
			{
				AddElement(element);
			}
		}

		public void AddElement(AlignmentElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			IEnumerable<Token> enumerable = _tokensContainer[element.Content];
			foreach (Token item in enumerable)
			{
				if (!item.IsWhitespace && !item.IsPunctuation)
				{
					HashSet<AlignmentElement> hashSet = GetElementsFromIndex(item) ?? AddEmptyElementsToIndex(item);
					hashSet.Add(element);
				}
			}
		}

		public IEnumerable<AlignmentElement> GetElements(Token token)
		{
			if (token == null)
			{
				throw new ArgumentNullException("token");
			}
			HashSet<AlignmentElement> elementsFromIndex = GetElementsFromIndex(token);
			if (elementsFromIndex == null)
			{
				return new HashSet<AlignmentElement>();
			}
			return new HashSet<AlignmentElement>(elementsFromIndex);
		}

		private HashSet<AlignmentElement> GetElementsFromIndex(Token token)
		{
			if (!TokenTextElementsIndex.TryGetValue(GetTokenCanonicalString(token), out HashSet<AlignmentElement> value))
			{
				return null;
			}
			return value;
		}

		private HashSet<AlignmentElement> AddEmptyElementsToIndex(Token token)
		{
			HashSet<AlignmentElement> hashSet = new HashSet<AlignmentElement>();
			TokenTextElementsIndex.Add(GetTokenCanonicalString(token), hashSet);
			return hashSet;
		}

		private string GetTokenCanonicalString(Token token)
		{
			if (token == null)
			{
				throw new ArgumentNullException("token");
			}
			MeasureToken measureToken = token as MeasureToken;
			if (measureToken != null)
			{
				return measureToken.Value.ToString(CultureInfo.InvariantCulture) + measureToken.Unit;
			}
			NumberToken numberToken = token as NumberToken;
			if (numberToken != null)
			{
				return numberToken.Value.ToString(CultureInfo.InvariantCulture);
			}
			DateTimeToken dateTimeToken = token as DateTimeToken;
			if (dateTimeToken != null)
			{
				return dateTimeToken.Value.ToString(CultureInfo.InvariantCulture);
			}
			return token.Text;
		}
	}
}
