using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace Sdl.Common.Licensing.Provider.Core.UI
{
	internal class Helpers
	{
		public static bool MultipleInstancesRunning
		{
			get
			{
				try
				{
					using (Process process = Process.GetCurrentProcess())
					{
						string processName = process.ProcessName;
						return !string.IsNullOrEmpty(processName) && Process.GetProcessesByName(processName).Length > 1;
					}
				}
				catch
				{
					return false;
				}
			}
		}

		public static int GetDaysRemainingOnLicense(IProductLicense license)
		{
			try
			{
				DateTime value = license.ExpirationDate ?? DateTime.Today;
				return GetRemainingDays(value);
			}
			catch
			{
				return 0;
			}
		}

		public static int GetRemainingDays(DateTime? dateTime)
		{
			try
			{
				DateTime d = dateTime ?? DateTime.Today;
				TimeSpan timeSpan = d - DateTime.Today;
				if (timeSpan.TotalDays < 0.0)
				{
					return 0;
				}
				return (int)Math.Ceiling(timeSpan.TotalDays);
			}
			catch
			{
				return 0;
			}
		}

		public static bool ValidateLicenseCode(ref string activationCode)
		{
			activationCode = activationCode.Trim();
			string code = activationCode;
			return LicensingProviderManager.Instance.LicensingProviderFactories.Any((ILicensingProviderFactory lf) => lf.IsActivationCode(code));
		}

		public static bool isValidNetBiosName(string serverName)
		{
			if (serverName.Contains("\\") || serverName.Contains("/") || serverName.Contains(":") || serverName.Contains("*") || serverName.Contains("?") || serverName.Contains(";") || serverName.Contains('"') || serverName.Contains("+") || serverName.Contains(" ") || Regex.IsMatch(serverName, "[\\s]"))
			{
				return false;
			}
			return true;
		}

		public static string TrimPasteStringToMaxLength(string stringToPaste, int length)
		{
			if (string.IsNullOrEmpty(stringToPaste))
			{
				return null;
			}
			string text = Regex.Replace(stringToPaste, "\\s", "");
			return (text.Length <= length) ? text : text.Substring(0, length);
		}

		public static MessageBoxResult ShowInformation(string text, string title)
		{
			return MessageBox.Show(text, string.IsNullOrEmpty(title) ? StringResources.DesktopLicensing_MessageBox_Title_Information : title, MessageBoxButton.OK, MessageBoxImage.Asterisk);
		}

		public static MessageBoxResult ShowWarning(string text)
		{
			return MessageBox.Show(text, StringResources.DesktopLicensing_MessageBox_Title_Warning, MessageBoxButton.OK, MessageBoxImage.Exclamation);
		}

		public static MessageBoxResult ShowError(string text)
		{
			return ShowError(text, MessageBoxButton.OK);
		}

		public static MessageBoxResult ShowError(string text, string title)
		{
			return MessageBox.Show(text, title, MessageBoxButton.OK, MessageBoxImage.Hand);
		}

		public static MessageBoxResult ShowError(string text, MessageBoxButton buttons)
		{
			return MessageBox.Show(text, StringResources.DesktopLicensing_MessageBox_Title_Error, buttons, MessageBoxImage.Hand);
		}

		public static MessageBoxResult ShowException(Exception ex)
		{
			return ShowException(null, ex);
		}

		public static MessageBoxResult ShowException(string message, Exception ex)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (!string.IsNullOrEmpty(message))
			{
				stringBuilder.Append(message + Environment.NewLine + Environment.NewLine);
			}
			stringBuilder.Append(ex.Message);
			if (ex.InnerException != null)
			{
				stringBuilder.Append(Environment.NewLine + Environment.NewLine);
				stringBuilder.Append("[");
				stringBuilder.Append(ex.InnerException.Message);
				stringBuilder.Append("]");
			}
			return MessageBox.Show(stringBuilder.ToString(), StringResources.DesktopLicensing_MessageBox_Title_Error, MessageBoxButton.OK, MessageBoxImage.Hand);
		}

		public static MessageBoxResult ShowQuestion(string message, string title, MessageBoxButton button)
		{
			return MessageBox.Show(message, title, button, MessageBoxImage.Question);
		}

		public static void ShowActivationCodeDialog(string licenseCode)
		{
			MessageBox.Show(string.Format(StringResources.YourActivationCode, licenseCode), StringResources.ActivationCode);
		}
	}
}
