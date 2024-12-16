using System.ComponentModel;

namespace Sdl.Core.Globalization
{
	public class LanguageCancelEventArgs : CancelEventArgs
	{
		public Language Language
		{
			get;
		}

		public LanguageCancelEventArgs(Language language)
		{
			Language = language;
		}
	}
}
