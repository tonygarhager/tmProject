using System;

namespace Sdl.Core.LanguageProcessing.Tokenization.Transducer
{
	public class FSATransition : IComparable<FSATransition>
	{
		public int Source
		{
			get;
			set;
		}

		public int Target
		{
			get;
			set;
		}

		public Label Input
		{
			get;
			set;
		}

		public bool IsEpsilon => Input.IsEpsilon;

		public FSATransition(int source, int target, Label input)
		{
			Source = source;
			Target = target;
			Input = input;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj.GetType() != GetType())
			{
				return false;
			}
			FSATransition other = obj as FSATransition;
			return CompareTo(other) == 0;
		}

		public override int GetHashCode()
		{
			int num = 17;
			num = num * 31 + Source;
			num = num * 31 + Input.Symbol;
			return num * 31 + Target;
		}

		public int CompareTo(FSATransition other)
		{
			int num = Source - other.Source;
			if (num == 0)
			{
				num = Input.Symbol - other.Input.Symbol;
			}
			if (num == 0)
			{
				num = Target - other.Target;
			}
			return num;
		}
	}
}
