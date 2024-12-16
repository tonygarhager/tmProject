using System;
using System.Windows.Forms;

namespace Sdl.Common.Licensing.Provider.Core.UI
{
	internal static class ClipboardAccess
	{
		public static string GetClipboardText()
		{
			try
			{
				string text = Clipboard.GetText();
				if (string.IsNullOrEmpty(text))
				{
					return null;
				}
				return text;
			}
			catch (Exception)
			{
				return null;
			}
		}

		public static void SetClipboardText(string text)
		{
			try
			{
				Clipboard.SetText(text);
			}
			catch (Exception)
			{
			}
		}
	}
}
