using System;
using System.Collections.Generic;
using System.Text;

namespace Sdl.Core.LanguageProcessing.Tokenization.Transducer
{
	internal class DisjunctionNode : Node
	{
		public List<Node> Alternatives
		{
			get;
		}

		public DisjunctionNode()
		{
			Alternatives = new List<Node>();
		}

		public override string GetExpression()
		{
			StringBuilder stringBuilder = new StringBuilder("(");
			for (int i = 0; i < Alternatives.Count; i++)
			{
				if (i > 0)
				{
					stringBuilder.Append("|");
				}
				stringBuilder.Append(Alternatives[i].GetExpression());
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		public void Add(Node n)
		{
			n.Owner = this;
			Alternatives.Add(n);
		}

		public override FST GetFst()
		{
			if (Alternatives.Count == 0)
			{
				throw new Exception("Internal error");
			}
			FST fst = Alternatives[0].GetFst();
			if (Alternatives.Count <= 1)
			{
				return fst;
			}
			List<FST> list = new List<FST>();
			for (int i = 1; i < Alternatives.Count; i++)
			{
				list.Add(Alternatives[i].GetFst());
			}
			fst.Disjunct(list);
			return fst;
		}
	}
}
