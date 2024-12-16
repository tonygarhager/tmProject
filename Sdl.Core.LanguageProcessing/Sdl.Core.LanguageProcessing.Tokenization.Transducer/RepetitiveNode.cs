using System;
using System.Collections.Generic;

namespace Sdl.Core.LanguageProcessing.Tokenization.Transducer
{
	internal class RepetitiveNode : Node
	{
		private readonly int _lower;

		private readonly int _upper;

		private Node _content;

		public const int INFINITY = -1;

		public Node Content
		{
			get
			{
				return _content;
			}
			set
			{
				value.Owner = this;
				_content = value;
			}
		}

		public RepetitiveNode(int lower, int upper)
		{
			_lower = lower;
			_upper = upper;
		}

		public override string GetExpression()
		{
			int lower = _lower;
			char c;
			if (lower != 0)
			{
				if (lower != 1 || _upper != -1)
				{
					goto IL_003a;
				}
				c = '+';
			}
			else if (_upper == 1)
			{
				c = '?';
			}
			else
			{
				if (_upper != -1)
				{
					goto IL_003a;
				}
				c = '*';
			}
			goto IL_003d;
			IL_003a:
			c = '!';
			goto IL_003d;
			IL_003d:
			return $"({_content.GetExpression()}){c}";
		}

		public override FST GetFst()
		{
			FST fst = _content.GetFst();
			if ((_lower == 0 && _upper == 1) || (_lower == 1 && _upper == -1) || (_lower == 0 && _upper == -1))
			{
				int startState = fst.GetStartState();
				if (_upper == -1)
				{
					List<int> finalStates = fst.GetFinalStates();
					foreach (int state in fst.GetStates())
					{
						IList<FSTTransition> transitions = fst.GetTransitions(state);
						for (int num = transitions.Count - 1; num >= 0; num--)
						{
							FSTTransition fSTTransition = transitions[num];
							if (finalStates.Contains(fSTTransition.Target))
							{
								fst.AddTransition(state, startState, new Label(fSTTransition.Input), new Label(fSTTransition.Output));
							}
						}
					}
				}
				if (_lower == 0)
				{
					fst.SetFinal(startState, flag: true);
				}
				return fst;
			}
			throw new NotImplementedException();
		}
	}
}
