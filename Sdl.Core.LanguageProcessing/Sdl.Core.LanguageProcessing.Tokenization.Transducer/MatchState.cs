using System;
using System.Collections.Generic;
using System.Text;

namespace Sdl.Core.LanguageProcessing.Tokenization.Transducer
{
	public class MatchState
	{
		public int ConsumedSymbols
		{
			get;
			private set;
		}

		public int InputPosition
		{
			get;
			set;
		}

		public List<Label> Output
		{
			get;
		}

		public int State
		{
			get;
			private set;
		}

		public MatchState(int s)
		{
			State = s;
			Output = new List<Label>();
			InputPosition = 0;
			ConsumedSymbols = 0;
		}

		public MatchState(MatchState other)
		{
			State = other.State;
			Output = new List<Label>(other.Output);
			InputPosition = other.InputPosition;
			ConsumedSymbols = other.ConsumedSymbols;
		}

		public string GetOutputAsString()
		{
			if (Output == null || Output.Count == 0)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Label item in Output)
			{
				if (item.IsCharLabel)
				{
					stringBuilder.Append((char)item.Symbol);
				}
				else if (item.Symbol == -7)
				{
					stringBuilder.Append(' ');
				}
			}
			return stringBuilder.ToString();
		}

		public void AppendOutput(Label l)
		{
			if (Output != null && l.IsConsuming)
			{
				Output.Add(l);
			}
		}

		public MatchState Traverse(string input, FSTTransition t, Matcher.MatchMode mode, bool ignoreCase)
		{
			Label label;
			Label label2;
			switch (mode)
			{
			case Matcher.MatchMode.Analyse:
				label = t.Input;
				label2 = t.Output;
				break;
			case Matcher.MatchMode.Generate:
				label = t.Output;
				label2 = t.Input;
				break;
			default:
				throw new Exception("Illegal case constant");
			}
			if (!label.Matches(input, InputPosition, ignoreCase))
			{
				return null;
			}
			MatchState matchState = new MatchState(this)
			{
				State = t.Target
			};
			if (label2.IsConsuming)
			{
				matchState.Output.Add(label2);
			}
			if (!label.IsConsuming)
			{
				return matchState;
			}
			int num = ++matchState.ConsumedSymbols;
			num = ++matchState.InputPosition;
			return matchState;
		}
	}
}
