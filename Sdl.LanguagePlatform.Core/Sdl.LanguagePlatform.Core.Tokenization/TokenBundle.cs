using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.LanguagePlatform.Core.Tokenization
{
	public class TokenBundle : Token, IEnumerable<PrioritizedToken>, IEnumerable
	{
		private readonly List<PrioritizedToken> _Alternatives;

		public int Count => _Alternatives.Count;

		public PrioritizedToken this[int index] => _Alternatives[index];

		public override bool IsPlaceable => _Alternatives.Any((PrioritizedToken x) => x.Token.IsPlaceable);

		public override bool IsSubstitutable => _Alternatives.Any((PrioritizedToken x) => x.Token.IsSubstitutable);

		public TokenBundle(Token t, int priority)
			: base(t.Text)
		{
			_Alternatives = new List<PrioritizedToken>
			{
				new PrioritizedToken(t, priority)
			};
			base.Culture = t.Culture;
		}

		public TokenBundle(IList<PrioritizedToken> items)
		{
			_Alternatives = new List<PrioritizedToken>();
			if (items != null && items.Count > 0)
			{
				base.Culture = items[0].Token.Culture;
				_Alternatives.AddRange(items);
			}
		}

		public void Add(Token t, int priority)
		{
			Add(t, priority, keepDuplicates: true);
		}

		public void Add(Token t, int priority, bool keepDuplicates)
		{
			if (keepDuplicates || _Alternatives.Count == 0)
			{
				_Alternatives.Add(new PrioritizedToken(t, priority));
				return;
			}
			int num = -1;
			for (int i = 0; i < _Alternatives.Count; i++)
			{
				if (t.GetSimilarity(_Alternatives[i].Token) == Similarity.IdenticalValueAndType && (num < 0 || _Alternatives[i].Priority >= _Alternatives[num].Priority))
				{
					num = i;
				}
			}
			if (num >= 0)
			{
				if (_Alternatives[num].Priority < priority)
				{
					_Alternatives.RemoveAt(num);
					_Alternatives.Add(new PrioritizedToken(t, priority));
				}
			}
			else
			{
				_Alternatives.Add(new PrioritizedToken(t, priority));
			}
		}

		public IEnumerator<PrioritizedToken> GetEnumerator()
		{
			return _Alternatives.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _Alternatives.GetEnumerator();
		}

		public Token GetBest()
		{
			if (_Alternatives == null || _Alternatives.Count == 0)
			{
				throw new InvalidOperationException();
			}
			int index = 0;
			int priority = _Alternatives[0].Priority;
			for (int i = 1; i < _Alternatives.Count; i++)
			{
				if (priority < _Alternatives[i].Priority)
				{
					index = i;
					priority = _Alternatives[i].Priority;
				}
			}
			return _Alternatives[index].Token;
		}

		public void SortByDecreasingPriority()
		{
			if (_Alternatives != null && _Alternatives.Count >= 2)
			{
				_Alternatives.Sort((PrioritizedToken x, PrioritizedToken y) => y.Priority - x.Priority);
			}
		}

		public bool Contains(Token t)
		{
			if (t == null)
			{
				throw new ArgumentNullException();
			}
			if (_Alternatives == null || _Alternatives.Count == 0)
			{
				return false;
			}
			return _Alternatives.Any((PrioritizedToken x) => t.Equals(x.Token));
		}

		protected override TokenType GetTokenType()
		{
			if (_Alternatives == null || _Alternatives.Count == 0)
			{
				throw new InvalidOperationException();
			}
			return _Alternatives[0].Token.Type;
		}

		public override void AcceptSegmentElementVisitor(ISegmentElementVisitor visitor)
		{
			throw new InvalidOperationException();
		}

		public override SegmentElement Duplicate()
		{
			throw new InvalidOperationException();
		}

		public override Similarity GetSimilarity(SegmentElement other)
		{
			if (!(other is Token))
			{
				return Similarity.None;
			}
			Similarity similarity = Similarity.None;
			foreach (PrioritizedToken alternative in _Alternatives)
			{
				Similarity similarity2 = other.GetSimilarity(alternative.Token);
				if (similarity2 > similarity)
				{
					similarity = similarity2;
				}
			}
			return similarity;
		}
	}
}
