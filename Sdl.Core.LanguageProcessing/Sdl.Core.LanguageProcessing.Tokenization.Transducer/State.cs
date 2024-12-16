using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.LanguageProcessing.Tokenization.Transducer
{
	public class State
	{
		public List<FSTTransition> Transitions
		{
			get;
		}

		public bool IsInitial
		{
			get;
			set;
		}

		public bool IsFinal
		{
			get;
			set;
		}

		public int Id
		{
			get;
			set;
		}

		public int TransitionCount => Transitions.Count;

		public bool TransitionsSorted
		{
			get;
			private set;
		}

		public State()
		{
			Transitions = new List<FSTTransition>();
			TransitionsSorted = true;
		}

		public void AddTransition(int target, Label input, Label output)
		{
			if (HasTransition(target, input, output))
			{
				return;
			}
			FSTTransition item = new FSTTransition(Id, target, input, output);
			Transitions.Add(item);
			if (TransitionsSorted && Transitions.Count > 1)
			{
				int count = Transitions.Count;
				if (Transitions[count - 1].CompareTo(Transitions[count - 2]) <= 0)
				{
					TransitionsSorted = false;
				}
			}
		}

		public void RemoveTransition(int target, Label input, Label output)
		{
			int num = FindTransitionInternal(target, input, output);
			if (num >= 0)
			{
				Transitions.RemoveAt(num);
			}
		}

		public void RemoveTransitionAt(int p)
		{
			Transitions.RemoveAt(p);
		}

		public bool HasTransition(int target, Label input, Label output)
		{
			return FindTransition(target, input, output) != null;
		}

		public FSTTransition FindTransition(int target, Label input, Label output)
		{
			int num = FindTransitionInternal(target, input, output);
			if (num >= 0)
			{
				return Transitions[num];
			}
			return null;
		}

		private int FindTransitionInternal(int target, Label input, Label output)
		{
			for (int i = 0; i < Transitions.Count; i++)
			{
				if (Transitions[i] != null && Transitions[i].Target == target && Transitions[i].Input.Equals(input) && Transitions[i].Output.Equals(output))
				{
					return i;
				}
			}
			return -1;
		}

		public void SortTransitions()
		{
			if (Transitions.Count > 1)
			{
				Transitions.Sort();
			}
			TransitionsSorted = true;
		}

		private bool EnsureSourceState()
		{
			foreach (FSTTransition transition in Transitions)
			{
				if (transition != null && transition.Source != Id)
				{
					return false;
				}
			}
			return true;
		}

		public bool IsDeterministic()
		{
			if (Transitions == null || Transitions.Count == 0)
			{
				return true;
			}
			SortTransitions();
			if (Transitions[0].IsEpsilon)
			{
				return false;
			}
			for (int i = 1; i < Transitions.Count; i++)
			{
				if ((Transitions[i].Input.Symbol == Transitions[i - 1].Input.Symbol && Transitions[i].Output.Symbol == Transitions[i - 1].Output.Symbol) || Transitions[i].IsEpsilon)
				{
					return false;
				}
			}
			return true;
		}

		public bool HasEpsilonTransitions()
		{
			return Transitions.Any((FSTTransition t) => t.IsEpsilon);
		}
	}
}
