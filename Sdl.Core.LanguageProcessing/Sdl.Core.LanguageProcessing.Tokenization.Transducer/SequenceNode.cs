using System;
using System.Collections.Generic;
using System.Text;

namespace Sdl.Core.LanguageProcessing.Tokenization.Transducer
{
	internal class SequenceNode : Node
	{
		private readonly List<Node> _sequence;

		public SequenceNode()
		{
			_sequence = new List<Node>();
		}

		public override string GetExpression()
		{
			StringBuilder stringBuilder = new StringBuilder("(");
			foreach (Node item in _sequence)
			{
				stringBuilder.Append(item.GetExpression());
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		public void Add(Node n)
		{
			n.Owner = this;
			_sequence.Add(n);
		}

		public override FST GetFst()
		{
			if (_sequence.Count == 0)
			{
				throw new Exception("Unexpected");
			}
			Node node = _sequence[0];
			FST fst = node.GetFst();
			for (int i = 1; i < _sequence.Count; i++)
			{
				FST fst2 = _sequence[i].GetFst();
				fst.Concatenate(fst2);
			}
			return fst;
		}
	}
}
