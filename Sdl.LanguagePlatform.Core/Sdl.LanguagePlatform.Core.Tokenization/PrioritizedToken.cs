using System;

namespace Sdl.LanguagePlatform.Core.Tokenization
{
	public class PrioritizedToken
	{
		public Token Token;

		public int Priority;

		public PrioritizedToken(Token t, int priority)
		{
			Token = (t ?? throw new ArgumentNullException());
			Priority = priority;
		}
	}
}
