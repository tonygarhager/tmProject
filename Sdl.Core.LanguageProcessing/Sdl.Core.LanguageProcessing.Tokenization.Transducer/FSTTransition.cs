using System;

namespace Sdl.Core.LanguageProcessing.Tokenization.Transducer
{
	public class FSTTransition : IComparable<FSTTransition>
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

		public Label Output
		{
			get;
			set;
		}

		public bool IsEpsilon
		{
			get
			{
				if (Input.IsEpsilon)
				{
					return Output.IsEpsilon;
				}
				return false;
			}
		}

		public FSTTransition(int source, int target, Label input, Label output)
		{
			Source = source;
			Target = target;
			Input = input;
			Output = output;
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
			FSTTransition other = obj as FSTTransition;
			return CompareTo(other) == 0;
		}

		public override int GetHashCode()
		{
			int num = 17;
			num = num * 31 + Source;
			num = num * 31 + Input.Symbol;
			num = num * 31 + Output.Symbol;
			return num * 31 + Target;
		}

		public int CompareTo(FSTTransition other)
		{
			int num = Source - other.Source;
			if (num == 0)
			{
				num = Input.Symbol - other.Input.Symbol;
			}
			if (num == 0)
			{
				num = Output.Symbol - other.Output.Symbol;
			}
			if (num == 0)
			{
				num = Target - other.Target;
			}
			return num;
		}

		public bool CanTraverse(Matcher.MatchMode mode, string input, int position, bool ignoreCase, out Label output, out bool consumedInput)
		{
			consumedInput = false;
			output = null;
			Label label;
			Label label2;
			switch (mode)
			{
			case Matcher.MatchMode.Analyse:
				label = Input;
				label2 = Output;
				break;
			case Matcher.MatchMode.Generate:
				label = Output;
				label2 = Input;
				break;
			default:
				throw new Exception("Illegal case constant");
			}
			if (!label.Matches(input, position, ignoreCase))
			{
				return false;
			}
			consumedInput = label.IsConsuming;
			output = label2;
			return true;
		}
	}
}
