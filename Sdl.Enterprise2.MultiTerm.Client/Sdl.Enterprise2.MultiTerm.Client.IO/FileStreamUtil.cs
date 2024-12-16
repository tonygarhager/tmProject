using log4net;
using System;
using System.Configuration;
using System.Globalization;
using System.IO;

namespace Sdl.Enterprise2.MultiTerm.Client.IO
{
	public static class FileStreamUtil
	{
		private const int defaultBufferSize = 524288;

		private static readonly ILog Log;

		private static readonly int _bufferSize;

		public static int DefaultBufferSize => _bufferSize;

		static FileStreamUtil()
		{
			Log = LogManager.GetLogger(typeof(FileStreamUtil));
			_bufferSize = GetBuffersize();
		}

		private static int GetBuffersize()
		{
			string name = "FileStreamBufferSize";
			string text = ConfigurationManager.AppSettings[name];
			if (text != null && text.StartsWith("0x") && int.TryParse(text.Substring(2), NumberStyles.AllowHexSpecifier, null, out int result))
			{
				return result;
			}
			Log.Error($"Incorrect value in configuration tag <FileStreamBufferSize>: {text}");
			return 524288;
		}

		public static void CopyFromStream(Stream sourceStream, string destinationPath, EventHandler<FileOperationEventArgs> progressEventHandler)
		{
			CopyFromStream(sourceStream, destinationPath, _bufferSize, progressEventHandler);
		}

		public static void CopyFromStream(Stream sourceStream, string destinationPath, int bufferSize, EventHandler<FileOperationEventArgs> progressEventHandler)
		{
			if (sourceStream == null)
			{
				throw new ArgumentNullException("sourceStream");
			}
			if (string.IsNullOrEmpty(destinationPath))
			{
				throw new ArgumentNullException("destinationPath");
			}
			if (File.Exists(destinationPath))
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Error, destination file {0} exists!", destinationPath));
			}
			using (Stream destinationStream = File.OpenWrite(destinationPath))
			{
				CopyStream(sourceStream, destinationStream, bufferSize, progressEventHandler);
			}
		}

		public static void CopyToStream(string sourcePath, Stream destinationStream, EventHandler<FileOperationEventArgs> progressEventHandler)
		{
			CopyToStream(sourcePath, destinationStream, _bufferSize, progressEventHandler);
		}

		public static void CopyToStream(string sourcePath, Stream destinationStream, int bufferSize, EventHandler<FileOperationEventArgs> progressEventHandler)
		{
			if (string.IsNullOrEmpty(sourcePath))
			{
				throw new ArgumentNullException("sourcePath");
			}
			if (destinationStream == null)
			{
				throw new ArgumentNullException("destinationStream");
			}
			if (!File.Exists(sourcePath))
			{
				throw new FileNotFoundException(sourcePath);
			}
			using (Stream sourceStream = File.OpenRead(sourcePath))
			{
				CopyStream(sourceStream, destinationStream, bufferSize, progressEventHandler);
			}
		}

		public static void CopyStream(Stream sourceStream, Stream destinationStream)
		{
			CopyStream(sourceStream, destinationStream, _bufferSize, null);
		}

		public static void CopyStream(Stream sourceStream, Stream destinationStream, EventHandler<FileOperationEventArgs> progressEventHandler)
		{
			CopyStream(sourceStream, destinationStream, _bufferSize, progressEventHandler);
		}

		public static void CopyStream(Stream sourceStream, Stream destinationStream, int bufferSize, EventHandler<FileOperationEventArgs> progressEventHandler)
		{
			if (sourceStream == null)
			{
				throw new ArgumentNullException("sourceStream");
			}
			if (destinationStream == null)
			{
				throw new ArgumentNullException("destinationStream");
			}
			if (bufferSize < 1)
			{
				throw new ArgumentOutOfRangeException("bufferSize", "Error, buffer size must >= 1.");
			}
			byte[] buffer = new byte[bufferSize];
			int num = 0;
			int num2 = 0;
			while ((num2 = sourceStream.Read(buffer, 0, bufferSize)) > 0)
			{
				destinationStream.Write(buffer, 0, num2);
				if (progressEventHandler != null)
				{
					num += num2;
					FileOperationEventArgs fileOperationEventArgs = new FileOperationEventArgs(num, sourceStream.Length);
					progressEventHandler(null, fileOperationEventArgs);
					if (fileOperationEventArgs.Cancel)
					{
						break;
					}
				}
			}
		}
	}
}
