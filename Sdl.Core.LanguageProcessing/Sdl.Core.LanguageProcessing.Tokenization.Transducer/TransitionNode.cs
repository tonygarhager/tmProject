namespace Sdl.Core.LanguageProcessing.Tokenization.Transducer
{
	internal class TransitionNode : Node
	{
		private readonly Label _input;

		private readonly Label _output;

		public TransitionNode(Label input, Label output)
		{
			_input = input;
			_output = output;
		}

		public override string GetExpression()
		{
			if (!_input.Equals(_output))
			{
				return $"<{_input}:{_output}>";
			}
			return _input.ToString();
		}

		public override FST GetFst()
		{
			FST fST = new FST();
			int num = fST.AddState();
			fST.SetInitial(num);
			if (_input.IsEpsilon && _output.IsEpsilon)
			{
				fST.SetFinal(num, flag: true);
			}
			else
			{
				int num2 = fST.AddState();
				fST.SetFinal(num2, flag: true);
				fST.AddTransition(num, num2, _input, _output);
			}
			return fST;
		}
	}
}
