using System;
using System.Collections.Generic;

namespace Sdl.Core.LanguageProcessing.Tokenization.Transducer
{
	public class Matcher
	{
		public delegate bool MatchFoundCallback(MatchState ms);

		public delegate bool ContinueIterationCallback(int ticks);

		public enum MatchMode
		{
			Analyse,
			Generate
		}

		private readonly FST _fst;

		public Matcher(FST fst)
		{
			_fst = fst;
		}

		public void Match(string s, bool matchWholeInput, MatchMode mode, bool ignoreCase, MatchFoundCallback matchFoundCallback, ContinueIterationCallback continueIterationCallback)
		{
			Match(s, matchWholeInput, mode, 0, ignoreCase, matchFoundCallback, continueIterationCallback);
		}

		public List<MatchState> Match(string input, bool matchWholeInput, MatchMode mode, int startOffset, bool ignoreCase)
		{
			List<MatchState> result = null;
			Match(input, matchWholeInput, mode, startOffset, ignoreCase, delegate(MatchState ms)
			{
				if (result == null)
				{
					result = new List<MatchState>();
				}
				result.Add(ms);
				return true;
			}, null);
			return result;
		}

		public void Match(string input, bool matchWholeInput, MatchMode mode, int startOffset, bool ignoreCase, MatchFoundCallback matchFoundCallback, ContinueIterationCallback continueIterationCallback)
		{
			if (startOffset < 0)
			{
				throw new ArgumentOutOfRangeException("startOffset");
			}
			if (matchFoundCallback == null)
			{
				throw new ArgumentNullException("matchFoundCallback");
			}
			int num = (!string.IsNullOrEmpty(input)) ? input.Length : 0;
			if (startOffset > num)
			{
				return;
			}
			int startState = _fst.GetStartState();
			if (startState < 0)
			{
				return;
			}
			List<MatchState> list = new List<MatchState>();
			bool flag = true;
			MatchState matchState = new MatchState(startState)
			{
				InputPosition = startOffset
			};
			list.Add(matchState);
			if (_fst.IsFinal(matchState.State) && (!matchWholeInput || matchState.InputPosition >= num))
			{
				flag = matchFoundCallback(matchState);
			}
			int num2 = 0;
			while (list.Count > 0 && flag)
			{
				List<MatchState> list2 = new List<MatchState>();
				for (int num3 = list.Count - 1; num3 >= 0; num3--)
				{
					MatchState matchState2 = list[num3];
					State state = _fst.GetState(matchState2.State);
					foreach (FSTTransition transition in state.Transitions)
					{
						num2++;
						MatchState matchState3;
						if ((matchState3 = matchState2.Traverse(input, transition, mode, ignoreCase)) != null)
						{
							list2.Add(matchState3);
							if (_fst.IsFinal(matchState3.State) && (!matchWholeInput || matchState3.InputPosition >= num))
							{
								flag = matchFoundCallback(matchState3);
							}
						}
					}
				}
				list = list2;
				if (num2 % 1000 == 0 && flag && continueIterationCallback != null)
				{
					flag = continueIterationCallback(num2);
				}
			}
		}
	}
}
