namespace Sdl.LanguagePlatform.Core.Serialization
{
	internal class SerializationToken
	{
		public TokenType TokenType
		{
			get;
			set;
		}

		public string Text
		{
			get;
			set;
		}

		public override string ToString()
		{
			switch (TokenType)
			{
			case TokenType.Text:
				return Text;
			case TokenType.PlaceholderTag:
				return TokenType.ToString() + " " + Text;
			default:
				return TokenType.ToString();
			}
		}
	}
}
