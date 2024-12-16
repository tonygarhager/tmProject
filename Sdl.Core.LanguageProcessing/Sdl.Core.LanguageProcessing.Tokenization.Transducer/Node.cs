namespace Sdl.Core.LanguageProcessing.Tokenization.Transducer
{
	internal abstract class Node
	{
		public Node Owner
		{
			get;
			set;
		}

		protected Node()
		{
			Owner = null;
		}

		public abstract FST GetFst();

		public abstract string GetExpression();

		public override string ToString()
		{
			return GetExpression();
		}
	}
}
