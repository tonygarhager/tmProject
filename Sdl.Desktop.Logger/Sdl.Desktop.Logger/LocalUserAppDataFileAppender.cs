using log4net.Appender;
using log4net.Layout;
using Sdl.Versioning;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Sdl.Desktop.Logger
{
	public class LocalUserAppDataFileAppender : RollingFileAppender
	{
		public static string LogFilePath => Path.Combine(VersionedPaths.UserAppDataPath, "logs", string.Format(CultureInfo.InvariantCulture, "{0}_{1}.log", Application.ProductName, Process.GetCurrentProcess().Id));

		public LocalUserAppDataFileAppender()
		{
			base.Name = "RollingFile";
			base.AppendToFile = false;
			base.Encoding = Encoding.Unicode;
			File = LogFilePath;
			base.MaximumFileSize = "8MB";
			base.MaxSizeRollBackups = 5;
			Layout = new PatternLayout("%p [%d] %c - %m%n");
		}

		protected override void OpenFile(string fileName, bool append)
		{
			base.OpenFile(fileName, append);
		}
	}
}
