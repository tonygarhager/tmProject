using System;
using System.Net;
using System.Runtime.InteropServices;

namespace Sdl.Common.Licensing.Provider.Core.UI
{
	internal class NetworkConnectivity
	{
		[Flags]
		private enum InternetConnectionState_e
		{
			INTERNET_CONNECTION_MODEM = 0x1,
			INTERNET_CONNECTION_LAN = 0x2,
			INTERNET_CONNECTION_PROXY = 0x4,
			INTERNET_RAS_INSTALLED = 0x10,
			INTERNET_CONNECTION_OFFLINE = 0x20,
			INTERNET_CONNECTION_CONFIGURED = 0x40
		}

		private static string _testServerAddress = "http://www.google.com/";

		[DllImport("wininet.dll", CharSet = CharSet.Auto)]
		private static extern bool InternetGetConnectedState(ref InternetConnectionState_e lpdwFlags, int dwReserved);

		public static bool IsConnectedToInternet()
		{
			InternetConnectionState_e lpdwFlags = (InternetConnectionState_e)0;
			return InternetGetConnectedState(ref lpdwFlags, 0);
		}

		public static bool TryToConnectToWebServer(string proxyAddress, int proxyPort)
		{
			string requestUriString = string.Format(_testServerAddress);
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUriString);
			int num3 = httpWebRequest.ReadWriteTimeout = (httpWebRequest.Timeout = 30000);
			WebProxy webProxy = new WebProxy();
			if (!string.IsNullOrEmpty(proxyAddress))
			{
				Uri uri2 = webProxy.Address = new Uri(string.Format("{0}{1}", proxyAddress, (proxyPort > 0) ? (":" + proxyPort.ToString()) : ""));
				httpWebRequest.Proxy = webProxy;
			}
			httpWebRequest.Proxy = webProxy;
			using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
			{
				if (httpWebResponse.StatusCode == HttpStatusCode.OK)
				{
					return true;
				}
				return false;
			}
		}

		public static Uri GetProxyDetails()
		{
			string text = string.Format(_testServerAddress);
			IWebProxy systemWebProxy = WebRequest.GetSystemWebProxy();
			if (systemWebProxy != null)
			{
				Uri proxy = systemWebProxy.GetProxy(new Uri(text));
				if (!proxy.AbsoluteUri.Equals(text))
				{
					return proxy;
				}
			}
			return null;
		}

		public static void GetProxyDetails(ref string proxyUrl, ref int port)
		{
			string text = string.Format(_testServerAddress);
			IWebProxy systemWebProxy = WebRequest.GetSystemWebProxy();
			if (systemWebProxy != null)
			{
				Uri proxy = systemWebProxy.GetProxy(new Uri(text));
				if (!proxy.AbsoluteUri.Equals(text))
				{
					proxyUrl = proxy.Host;
					port = proxy.Port;
				}
			}
		}
	}
}
