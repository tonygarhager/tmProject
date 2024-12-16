using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;

namespace Sdl.FileTypeSupport.Framework.PreviewControls
{
	/// <summary>
	/// Microsoft Office applications try to manage resources, processes on its own, so 
	/// Process.Start() will return valid process id only if there are no other instances of MS office running.
	/// In other case Process.Start() call will return immediately
	/// without returning valid process id, because existing instance of MS office application will
	/// take over process creation and opening the file.
	/// To avoid this, special DDE switch has to be passed to MS Office application when it is started.
	/// Class below customizes starting of MS Office application.
	/// see: http://support.microsoft.com/kb/210565 for command line switch and 
	/// see: http://social.msdn.microsoft.com/Forums/hu-HU/csharpgeneral/thread/21014f97-1732-4261-afe5-2127b2e64f68 for
	/// </summary>
	public class MsOfficeExternalPreviewApplication : GenericExteralPreviewApplication
	{
		/// <summary>
		/// Relative path in system registry to key describing default command for file extension
		/// </summary>
		private const string ShellCommandRelativePath = "Shell\\Open\\Command";

		/// <summary>
		/// Text used to locate program path in command string
		/// </summary>
		private const string AnchorText = ".EXE";

		/// <summary>
		/// Gets or sets dde switch allowing to open every file
		/// in separate instance of MS Office Application directly.
		/// </summary>
		public string DdeSwitch
		{
			get;
			set;
		}

		/// <summary>
		/// Get default shell command for extension
		/// </summary>
		/// <param name="extension">file extension</param>
		/// <returns>command to run or null</returns>
		private static string GetDefaultCommand(string extension)
		{
			RegistryKey registryKey = null;
			try
			{
				registryKey = Registry.ClassesRoot.OpenSubKey(extension);
				if (registryKey == null)
				{
					return null;
				}
				string text = registryKey.GetValue(null).ToString();
				registryKey.Close();
				if (string.IsNullOrEmpty(text))
				{
					return null;
				}
				registryKey = Registry.ClassesRoot.OpenSubKey(text + "\\Shell\\Open\\Command");
				if (registryKey == null)
				{
					return null;
				}
				string text2 = registryKey.GetValue(null).ToString();
				if (!string.IsNullOrEmpty(text2))
				{
					string text3 = text2.Substring(0, text2.IndexOf(".EXE", 0, StringComparison.CurrentCultureIgnoreCase) + ".EXE".Length);
					if (text3.StartsWith("\""))
					{
						text3 += "\"";
					}
					return text3;
				}
				registryKey.Close();
				return null;
			}
			catch (Exception)
			{
				return null;
			}
			finally
			{
				registryKey?.Close();
			}
		}

		/// <summary>
		/// Start external application for given file
		/// </summary>
		/// <param name="managedTempFile">File desriptor to open with application</param>
		/// <returns>Process id of external application</returns>
		protected override Process LaunchApplicationForFile(TempFileManager managedTempFile)
		{
			Process result = null;
			if (managedTempFile != null)
			{
				string defaultCommand = GetDefaultCommand(Path.GetExtension(managedTempFile.FilePath));
				result = (string.IsNullOrEmpty(defaultCommand) ? base.LaunchApplicationForFile(managedTempFile) : Process.Start(defaultCommand, DdeSwitch + " \"" + managedTempFile.FilePath + "\""));
			}
			return result;
		}
	}
}
