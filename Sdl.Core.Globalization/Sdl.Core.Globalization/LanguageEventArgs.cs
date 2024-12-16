using System;

namespace Sdl.Core.Globalization
{
	public class LanguageEventArgs : EventArgs
	{
		public Language Language
		{
			get;
		}

		public LanguageEventArgs(Language language)
		{
			Language = language;
		}
	}
}
