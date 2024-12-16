using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace Sdl.Core.LanguageProcessing.Tokenization.Transducer
{
	public class FST
	{
		private delegate bool TransitionProperty(FSTTransition t);

		private Dictionary<int, State> _states;

		private int _startState;

		private int _maxState;

		public static readonly string ReservedCharacters;

		private static readonly char[] _SpecialCharsArray;

		private const int Magic = 2007110601;

		public FST(string alpha)
		{
			_states = new Dictionary<int, State>();
			State state = new State
			{
				Id = 1,
				IsInitial = true,
				IsFinal = false
			};
			_states.Add(state.Id, state);
			_startState = state.Id;
			State state2 = new State
			{
				Id = 2,
				IsInitial = false,
				IsFinal = false
			};
			_states.Add(state2.Id, state2);
			State state3 = new State
			{
				Id = 3,
				IsInitial = false,
				IsFinal = false
			};
			_states.Add(state3.Id, state3);
			State state4 = new State
			{
				Id = 4,
				IsInitial = false,
				IsFinal = false
			};
			_states.Add(state4.Id, state4);
			State state5 = new State
			{
				Id = 5,
				IsInitial = false,
				IsFinal = false
			};
			_states.Add(state5.Id, state5);
			State state6 = new State
			{
				Id = 6,
				IsInitial = false,
				IsFinal = true
			};
			_states.Add(state6.Id, state6);
			_maxState = 5;
			AddTranzitions(state, state2, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ");
			AddTranzitions(state, state3, "0123456789");
			AddTranzitions(state2, state2, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ");
			AddTranzitions(state2, state2, "-_");
			AddTranzitions(state3, state3, "0123456789");
			AddTranzitions(state3, state3, "-_");
			AddTranzitions(state2, state4, "0123456789");
			AddTranzitions(state3, state4, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ");
			AddTranzitions(state2, state5, "0123456789");
			AddTranzitions(state3, state5, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ");
			AddTranzitions(state4, state4, "0123456789");
			AddTranzitions(state4, state4, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ");
			AddTranzitions(state4, state4, "-_");
			AddTranzitions(state4, state5, "0123456789");
			AddTranzitions(state4, state5, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ");
			state5.AddTransition(6, new Label(-4), new Label(-4));
		}

		private static void AddTranzitions(State start, State end, string tranzitions)
		{
			foreach (char symbol in tranzitions)
			{
				start.AddTransition(end.Id, new Label(symbol), new Label(symbol));
			}
		}

		static FST()
		{
			ReservedCharacters = "<>()[]{}<>*+?\\^$|#:";
			_SpecialCharsArray = ReservedCharacters.ToCharArray();
		}

		public static bool IsSpecial(char c)
		{
			return ReservedCharacters.IndexOf(c) >= 0;
		}

		public static string EscapeSpecial(char c)
		{
			if (_SpecialCharsArray.Contains(c))
			{
				return "\\" + c.ToString();
			}
			return string.Empty + c.ToString();
		}

		public static string EscapeSpecial(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return s;
			}
			if (s.IndexOfAny(_SpecialCharsArray) < 0)
			{
				return s;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (char value in s)
			{
				if (_SpecialCharsArray.Contains(value))
				{
					stringBuilder.Append("\\");
				}
				stringBuilder.Append(value);
			}
			return stringBuilder.ToString();
		}

		public static FST Create(string expression)
		{
			Parser parser = new Parser();
			return parser.Parse(expression);
		}

		public static FST Create(byte[] data)
		{
			FST fST = new FST();
			fST.FromBinary(data);
			return fST;
		}

		public FST()
		{
			_states = new Dictionary<int, State>();
			_startState = -1;
		}

		internal State GetState(int s)
		{
			if (!_states.TryGetValue(s, out State value))
			{
				return null;
			}
			return value;
		}

		internal IList<int> GetStates()
		{
			return _states.Select((KeyValuePair<int, State> kvp) => kvp.Key).ToList();
		}

		public int GetStateCount()
		{
			return _states.Count;
		}

		public IList<FSTTransition> GetTransitions(int state)
		{
			if (!StateExists(state))
			{
				throw new Exception("State doesn't exist");
			}
			return _states[state].Transitions;
		}

		public int AddState()
		{
			State state = new State
			{
				Id = _maxState
			};
			_maxState++;
			_states.Add(state.Id, state);
			return state.Id;
		}

		public bool StateExists(int s)
		{
			return _states.ContainsKey(s);
		}

		public List<int> GetFirstSet(bool forOutput)
		{
			List<int> result = new List<int>();
			ComputeStateClosure(_startState, includeStartState: true, delegate(FSTTransition t)
			{
				Label label = forOutput ? t.Output : t.Input;
				if (!label.IsConsuming)
				{
					return true;
				}
				int num = result.BinarySearch(label.Symbol);
				if (num < 0)
				{
					result.Insert(~num, label.Symbol);
				}
				return false;
			});
			return result;
		}

		public bool RemoveState(int s)
		{
			if (!StateExists(s))
			{
				throw new Exception("State doesn't exist");
			}
			if (s == _startState)
			{
				_startState = -1;
			}
			RemoveTransitionsInto(s);
			return _states.Remove(s);
		}

		public void Concatenate(FST rhs)
		{
			List<int> finalStates = GetFinalStates();
			if (finalStates.Count == 0)
			{
				throw new Exception("Automaton does not have any final states");
			}
			if (!StateExists(_startState))
			{
				throw new Exception("Automaton does not have a start state");
			}
			if (!rhs.StateExists(rhs._startState))
			{
				throw new Exception("RHS Automaton does not have a start state");
			}
			bool flag = IsFinal(_startState);
			bool flag2 = rhs.IsFinal(rhs._startState);
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			if (!flag2)
			{
				foreach (int item in finalStates)
				{
					SetFinal(item, flag: false);
				}
			}
			foreach (KeyValuePair<int, State> state in rhs._states)
			{
				int num = AddState();
				dictionary.Add(state.Value.Id, num);
				if (state.Value.IsFinal)
				{
					SetFinal(num, flag: true);
				}
			}
			foreach (KeyValuePair<int, State> state2 in rhs._states)
			{
				foreach (FSTTransition transition in state2.Value.Transitions)
				{
					AddTransition(dictionary[transition.Source], dictionary[transition.Target], new Label(transition.Input), new Label(transition.Output));
				}
			}
			int target = dictionary[rhs._startState];
			foreach (KeyValuePair<int, State> state3 in _states)
			{
				State value = state3.Value;
				List<FSTTransition> transitions = value.Transitions;
				for (int num2 = transitions.Count - 1; num2 >= 0; num2--)
				{
					FSTTransition fSTTransition = transitions[num2];
					if (finalStates.Contains(fSTTransition.Target))
					{
						value.AddTransition(target, new Label(fSTTransition.Input), new Label(fSTTransition.Output));
					}
				}
			}
			if (flag)
			{
				AddTransition(_startState, target, new Label(-1), new Label(-1));
			}
			Clean();
		}

		public void Disjunct(IList<FST> alternatives)
		{
			if (alternatives != null && alternatives.Count != 0)
			{
				int startState = _startState;
				int num = AddState();
				SetInitial(num);
				AddTransition(num, startState, new Label(-1), new Label(-1));
				foreach (FST alternative in alternatives)
				{
					Dictionary<int, int> dictionary = new Dictionary<int, int>();
					int startState2 = alternative._startState;
					foreach (KeyValuePair<int, State> state in alternative._states)
					{
						int num2 = AddState();
						dictionary.Add(state.Key, num2);
						if (state.Value.IsFinal)
						{
							SetFinal(num2, flag: true);
						}
						if (state.Key == startState2)
						{
							AddTransition(num, num2, new Label(-1), new Label(-1));
						}
					}
					foreach (KeyValuePair<int, State> state2 in alternative._states)
					{
						foreach (FSTTransition transition in state2.Value.Transitions)
						{
							AddTransition(dictionary[transition.Source], dictionary[transition.Target], new Label(transition.Input), new Label(transition.Output));
						}
					}
				}
				MergeSimpleFinalStates();
				EliminateEpsilonTransitions();
				Clean();
			}
		}

		public List<int> ComputeReachableStates()
		{
			if (!StateExists(_startState))
			{
				return new List<int>();
			}
			return ComputeStateClosure(_startState, includeStartState: true, (FSTTransition _003Cp0_003E) => true);
		}

		public bool IsCyclic()
		{
			List<int> list = new List<int>();
			List<int> list2 = new List<int>
			{
				_startState
			};
			while (list2.Count > 0)
			{
				int num = list2[0];
				list2.RemoveAt(0);
				int num2 = list.BinarySearch(num);
				list.Insert(~num2, num);
				foreach (FSTTransition transition in _states[num].Transitions)
				{
					int target = transition.Target;
					num2 = list.BinarySearch(target);
					if (num2 >= 0)
					{
						return true;
					}
					num2 = list2.BinarySearch(target);
					if (num2 < 0)
					{
						list2.Insert(~num2, target);
					}
				}
			}
			return false;
		}

		private void MergeSimpleFinalStates()
		{
			List<int> finalStates = GetFinalStates();
			if (finalStates == null || finalStates.Count < 2)
			{
				return;
			}
			List<int> list = new List<int>();
			int num = -1;
			bool flag = false;
			foreach (int item in finalStates)
			{
				if (_states[item].Transitions.Count == 0)
				{
					if (item == _startState)
					{
						flag = true;
					}
					if (num < 0)
					{
						num = item;
					}
					else
					{
						list.Add(item);
					}
				}
			}
			if (list.Count != 0)
			{
				list.Sort();
				if (flag)
				{
					SetInitial(num);
				}
				foreach (KeyValuePair<int, State> state in _states)
				{
					foreach (FSTTransition transition in state.Value.Transitions)
					{
						if (list.BinarySearch(transition.Target) >= 0)
						{
							transition.Target = num;
						}
					}
				}
				foreach (int item2 in list)
				{
					_states.Remove(item2);
				}
			}
		}

		public int EliminateEpsilonTransitions()
		{
			int num = 0;
			Dictionary<int, List<int>> dictionary = new Dictionary<int, List<int>>();
			foreach (KeyValuePair<int, State> state2 in _states)
			{
				if (HasEpsilonTransitions(state2.Key))
				{
					List<int> list = ComputeStateClosure(state2.Key, includeStartState: false, (FSTTransition t) => t.IsEpsilon);
					dictionary.Add(state2.Key, list);
					if (!state2.Value.IsFinal)
					{
						state2.Value.IsFinal = list.Any(IsFinal);
					}
				}
			}
			if (dictionary.Count > 0)
			{
				if (dictionary.ContainsKey(_startState))
				{
					State state = _states[_startState];
					foreach (int item in dictionary[_startState])
					{
						foreach (FSTTransition transition in _states[item].Transitions)
						{
							if (!transition.IsEpsilon)
							{
								state.AddTransition(transition.Target, new Label(transition.Input), new Label(transition.Output));
							}
						}
					}
				}
				foreach (KeyValuePair<int, State> state3 in _states)
				{
					List<FSTTransition> transitions = state3.Value.Transitions;
					int count = transitions.Count;
					for (int num2 = count - 1; num2 >= 0; num2--)
					{
						FSTTransition fSTTransition = transitions[num2];
						if (!fSTTransition.IsEpsilon && dictionary.TryGetValue(fSTTransition.Target, out List<int> value))
						{
							foreach (int item2 in value)
							{
								state3.Value.AddTransition(item2, new Label(fSTTransition.Input), new Label(fSTTransition.Output));
							}
						}
					}
					for (int num3 = count - 1; num3 >= 0; num3--)
					{
						if (transitions[num3].IsEpsilon)
						{
							transitions.RemoveAt(num3);
							num++;
						}
					}
				}
				DeleteNonreachableStates();
			}
			return num;
		}

		private void SortTransitions()
		{
			foreach (KeyValuePair<int, State> state in _states)
			{
				state.Value.SortTransitions();
			}
		}

		public void MakeDeterministic()
		{
			if (!StateExists(_startState))
			{
				throw new Exception("No start state");
			}
			if (IsDeterministic())
			{
				return;
			}
			EliminateEpsilonTransitions();
			SortTransitions();
			List<int> list = new List<int>
			{
				_startState
			};
			list.AddRange(from kvp in _states
				where kvp.Key != _startState
				select kvp into s
				select s.Key);
			int num = 0;
			Trie<int, int> trie = new Trie<int, int>();
			while (num < list.Count)
			{
				int key = list[num];
				num++;
				State state = _states[key];
				if (state.Transitions.Count < 2)
				{
					continue;
				}
				bool flag = false;
				int num2 = 0;
				int transitionCount = state.TransitionCount;
				while (num2 < transitionCount)
				{
					int i;
					for (i = num2 + 1; i < transitionCount && state.Transitions[i].Input.Equals(state.Transitions[num2].Input) && state.Transitions[i].Output.Equals(state.Transitions[num2].Output); i++)
					{
					}
					if (i > num2 + 1)
					{
						List<int> list2 = new List<int>();
						for (int j = num2; j < i; j++)
						{
							list2.Add(state.Transitions[j].Target);
						}
						list2.Sort();
						int num3;
						if (!trie.Contains(list2, out int value))
						{
							num3 = AddState();
							State state2 = _states[num3];
							list.Add(num3);
							trie.Add(list2, num3);
							state2.IsFinal = list2.Any(IsFinal);
							foreach (int item in list2)
							{
								State state3 = _states[item];
								foreach (FSTTransition transition in state3.Transitions)
								{
									state2.AddTransition(transition.Target, new Label(transition.Input), new Label(transition.Output));
								}
							}
							state2.SortTransitions();
						}
						else
						{
							num3 = value;
						}
						state.AddTransition(num3, new Label(state.Transitions[num2].Input), new Label(state.Transitions[num2].Output));
						for (int k = num2; k < i; k++)
						{
							state.Transitions[k] = null;
						}
						flag = true;
					}
					num2 = i;
				}
				if (flag)
				{
					state.Transitions.RemoveAll((FSTTransition t) => t == null);
				}
			}
			Clean();
		}

		public bool IsDeterministic()
		{
			return _states.All((KeyValuePair<int, State> kvp) => kvp.Value.IsDeterministic());
		}

		public bool HasEpsilonTransitions()
		{
			return _states.Any((KeyValuePair<int, State> kvp) => kvp.Value.HasEpsilonTransitions());
		}

		public bool HasEpsilonTransitions(int state)
		{
			if (!_states.TryGetValue(state, out State value))
			{
				throw new Exception("State doesn't exist");
			}
			return value.HasEpsilonTransitions();
		}

		public List<int> ComputeProductiveStates()
		{
			List<int> finalStates = GetFinalStates();
			bool flag;
			do
			{
				flag = false;
				foreach (KeyValuePair<int, State> state in _states)
				{
					int num = finalStates.BinarySearch(state.Key);
					if (num < 0)
					{
						foreach (FSTTransition transition in state.Value.Transitions)
						{
							if (finalStates.BinarySearch(transition.Target) >= 0)
							{
								finalStates.Insert(~num, state.Key);
								flag = true;
								break;
							}
						}
					}
				}
			}
			while (flag);
			return finalStates;
		}

		public void Clean()
		{
			DeleteUnproductiveStates();
			DeleteNonreachableStates();
		}

		public void DeleteUnproductiveStates()
		{
			List<int> list = ComputeProductiveStates();
			if (list.Count != _states.Count)
			{
				List<int> list2 = new List<int>();
				foreach (KeyValuePair<int, State> state in _states)
				{
					if (list.BinarySearch(state.Key) < 0)
					{
						list2.Add(state.Key);
					}
				}
				foreach (int item in list2)
				{
					RemoveState(item);
				}
			}
		}

		public void DeleteNonreachableStates()
		{
			List<int> list = ComputeReachableStates();
			if (list.Count != _states.Count)
			{
				List<int> list2 = new List<int>();
				foreach (KeyValuePair<int, State> state in _states)
				{
					if (list.BinarySearch(state.Key) < 0)
					{
						list2.Add(state.Key);
					}
				}
				foreach (int item in list2)
				{
					RemoveState(item);
				}
			}
		}

		public void SetStartState(int s)
		{
			if (s == _startState)
			{
				return;
			}
			State state = null;
			if (_startState >= 0)
			{
				state = GetState(_startState);
				if (state == null)
				{
					throw new Exception("State doesn't exist");
				}
			}
			State state2 = GetState(s);
			if (state2 == null)
			{
				throw new Exception("State doesn't exist");
			}
			if (state != null)
			{
				state.IsInitial = false;
			}
			state2.IsInitial = true;
			_startState = s;
		}

		public int GetStartState()
		{
			State state = GetState(_startState);
			if (state == null)
			{
				return -1;
			}
			return _startState;
		}

		public bool IsInitial(int s)
		{
			if (!_states.TryGetValue(s, out State value))
			{
				throw new Exception("State doesn't exist");
			}
			return value.IsInitial;
		}

		public bool IsFinal(int s)
		{
			if (!_states.TryGetValue(s, out State value))
			{
				throw new Exception("State doesn't exist");
			}
			return value.IsFinal;
		}

		public void SetInitial(int s)
		{
			SetStartState(s);
		}

		public void SetFinal(int s, bool flag)
		{
			if (!_states.TryGetValue(s, out State value))
			{
				throw new Exception("State doesn't exist");
			}
			value.IsFinal = flag;
		}

		public List<int> GetFinalStates()
		{
			List<int> list = new List<int>();
			foreach (KeyValuePair<int, State> state in _states)
			{
				if (state.Value.IsFinal)
				{
					list.Add(state.Value.Id);
				}
			}
			list.Sort();
			return list;
		}

		public void RemoveTransitionsInto(int s)
		{
			if (!StateExists(s))
			{
				throw new Exception("State doesn't exist");
			}
			foreach (KeyValuePair<int, State> state in _states)
			{
				State value = state.Value;
				for (int num = value.Transitions.Count - 1; num >= 0; num--)
				{
					if (value.Transitions[num].Target == s)
					{
						value.RemoveTransitionAt(num);
					}
				}
			}
		}

		public bool AddTransition(int start, int target, Label input, Label output)
		{
			if (!_states.TryGetValue(start, out State value))
			{
				throw new Exception("State doesn't exist");
			}
			if (!_states.TryGetValue(target, out State _))
			{
				throw new Exception("State doesn't exist");
			}
			if (value.HasTransition(target, input, output))
			{
				return false;
			}
			value.AddTransition(target, input, output);
			return true;
		}

		public int IncomingTransitionCount(int s)
		{
			if (!StateExists(s))
			{
				throw new Exception("State doesn't exist");
			}
			return GetIncomingTransitions(s).Count;
		}

		public List<FSTTransition> GetIncomingTransitions(int targetState)
		{
			List<FSTTransition> list = new List<FSTTransition>();
			foreach (KeyValuePair<int, State> state in _states)
			{
				State value = state.Value;
				foreach (FSTTransition transition in value.Transitions)
				{
					if (transition.Target == targetState)
					{
						list.Add(transition);
					}
				}
			}
			return list;
		}

		public bool IsConsistent()
		{
			int num = 0;
			if (_states.Count == 0)
			{
				num++;
			}
			int num2 = 0;
			int num3 = 0;
			foreach (KeyValuePair<int, State> state in _states)
			{
				State value = state.Value;
				if (value.IsInitial)
				{
					num2++;
				}
				if (value.IsFinal)
				{
					num3++;
				}
				foreach (FSTTransition transition in value.Transitions)
				{
					if (!StateExists(transition.Target))
					{
						num++;
					}
				}
			}
			if (num2 != 1)
			{
				num++;
			}
			if (num3 == 0)
			{
				num++;
			}
			return num == 0;
		}

		private List<int> ComputeStateClosure(int startState, bool includeStartState, TransitionProperty prop)
		{
			List<int> list = new List<int>();
			List<int> list2 = new List<int>
			{
				startState
			};
			while (list2.Count > 0)
			{
				int num = list2[0];
				list2.RemoveAt(0);
				int num2 = list.BinarySearch(num);
				if ((num != _startState) | includeStartState)
				{
					list.Insert(~num2, num);
				}
				foreach (FSTTransition transition in _states[num].Transitions)
				{
					if (prop(transition))
					{
						int target = transition.Target;
						if (list.BinarySearch(target) < 0 && (num2 = list2.BinarySearch(target)) < 0)
						{
							list2.Insert(~num2, target);
						}
					}
				}
			}
			return list;
		}

		public void Dump(string fileName)
		{
			using (StreamWriter wtr = new StreamWriter(fileName, append: false, Encoding.UTF8))
			{
				Dump(wtr);
			}
		}

		private void FromBinary(byte[] data)
		{
			_states = new Dictionary<int, State>();
			_startState = -1;
			_maxState = -1;
			using (MemoryStream stream = new MemoryStream(data))
			{
				using (GZipStream input = new GZipStream(stream, CompressionMode.Decompress))
				{
					using (BinaryReader binaryReader = new BinaryReader(input))
					{
						int num = binaryReader.ReadInt32();
						if (num != 2007110601)
						{
							throw new Exception("Inconsistent FST data - version number mismatch");
						}
						int num2 = binaryReader.ReadInt32();
						if (num2 < 0)
						{
							throw new Exception("Inconsistent FST data - invalid state count");
						}
						_startState = binaryReader.ReadInt32();
						_maxState = binaryReader.ReadInt32();
						for (int i = 0; i < num2; i++)
						{
							State state = new State
							{
								Id = binaryReader.ReadInt32()
							};
							_states.Add(state.Id, state);
							if (state.Id == _startState)
							{
								state.IsInitial = true;
							}
						}
						int num3 = binaryReader.ReadInt32();
						if (num3 < 0)
						{
							throw new Exception("Inconsistent FST data - invalid number of final states");
						}
						for (int j = 0; j < num3; j++)
						{
							int s = binaryReader.ReadInt32();
							SetFinal(s, flag: true);
						}
						for (int num4 = binaryReader.ReadInt32(); num4 > 0; num4--)
						{
							int start = binaryReader.ReadInt32();
							int target = binaryReader.ReadInt32();
							int symbol = binaryReader.ReadInt32();
							int symbol2 = binaryReader.ReadInt32();
							AddTransition(start, target, new Label(symbol), new Label(symbol2));
						}
					}
				}
			}
		}

		public byte[] GetBinary()
		{
			using (MemoryStream memoryStream = new MemoryStream(4096))
			{
				using (GZipStream output = new GZipStream(memoryStream, CompressionMode.Compress))
				{
					using (BinaryWriter binaryWriter = new BinaryWriter(output))
					{
						binaryWriter.Write(2007110601);
						binaryWriter.Write(_states.Count);
						binaryWriter.Write(_startState);
						binaryWriter.Write(_maxState);
						int num = 0;
						foreach (KeyValuePair<int, State> state in _states)
						{
							binaryWriter.Write(state.Key);
							num += state.Value.TransitionCount;
						}
						List<int> finalStates = GetFinalStates();
						binaryWriter.Write(finalStates.Count);
						foreach (int item in finalStates)
						{
							binaryWriter.Write(item);
						}
						binaryWriter.Write(num);
						foreach (KeyValuePair<int, State> state2 in _states)
						{
							foreach (FSTTransition transition in state2.Value.Transitions)
							{
								binaryWriter.Write(transition.Source);
								binaryWriter.Write(transition.Target);
								binaryWriter.Write(transition.Input.Symbol);
								binaryWriter.Write(transition.Output.Symbol);
							}
						}
					}
				}
				return memoryStream.ToArray();
			}
		}

		public bool IsIdentical(FST other)
		{
			if (other == null)
			{
				throw new ArgumentNullException();
			}
			if (_maxState != other._maxState || _startState != other._startState || _states.Count != other._states.Count)
			{
				return false;
			}
			SortTransitions();
			other.SortTransitions();
			foreach (KeyValuePair<int, State> state in _states)
			{
				if (!other._states.TryGetValue(state.Key, out State value))
				{
					return false;
				}
				if (state.Value.IsFinal != value.IsFinal || state.Value.IsInitial != value.IsInitial || state.Value.TransitionCount != value.TransitionCount)
				{
					return false;
				}
				for (int i = 0; i < state.Value.TransitionCount; i++)
				{
					FSTTransition fSTTransition = state.Value.Transitions[i];
					FSTTransition fSTTransition2 = value.Transitions[i];
					if (fSTTransition.Source != fSTTransition2.Source || fSTTransition.Target != fSTTransition2.Target || fSTTransition.Input.Symbol != fSTTransition2.Input.Symbol || fSTTransition.Output.Symbol != fSTTransition2.Output.Symbol)
					{
						return false;
					}
				}
			}
			return true;
		}

		public void Compare(TextWriter wtr, FST other)
		{
			wtr.WriteLine("================================================");
			wtr.WriteLine();
			if (_states.Count != other._states.Count)
			{
				wtr.WriteLine("This FST has " + _states.Count.ToString() + " states, other has " + other._states.Count.ToString());
				wtr.WriteLine("================================================");
			}
		}

		public void Dump(TextWriter wtr)
		{
			wtr.WriteLine("================================================");
			wtr.WriteLine();
			wtr.WriteLine("{0} States, start state is {1}", _states.Count, _startState);
			wtr.WriteLine();
			foreach (KeyValuePair<int, State> state in _states)
			{
				State value = state.Value;
				wtr.WriteLine("State {0} ({1}{2} - {3} transitions)", value.Id, value.IsInitial ? " start " : string.Empty, value.IsFinal ? " final " : string.Empty, (value.Transitions != null) ? value.TransitionCount : 0);
				if (value.Transitions != null)
				{
					foreach (FSTTransition transition in value.Transitions)
					{
						wtr.WriteLine("\t{0,-5} -> {1,-5} on ({2}:{3})", transition.Source, transition.Target, transition.Input, transition.Output);
					}
				}
			}
			wtr.WriteLine("================================================");
		}
	}
}
