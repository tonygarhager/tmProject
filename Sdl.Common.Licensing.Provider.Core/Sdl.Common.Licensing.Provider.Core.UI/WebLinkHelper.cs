using System.Diagnostics;

namespace Sdl.Common.Licensing.Provider.Core.UI
{
	internal class WebLinkHelper
	{
		public static void DisplayBrowser(string webAddress)
		{
			ProcessStartInfo processStartInfo = new ProcessStartInfo(webAddress);
			processStartInfo.UseShellExecute = true;
			Process.Start(processStartInfo);
		}
	}
}
