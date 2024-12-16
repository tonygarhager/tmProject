using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO;
using System.Text;

namespace Sdl.FileTypeSupport.Framework.Core.Settings
{
	public static class FileSerializer
	{
		private const int BufferSize = 1867776;

		public static string SerializeFile(string filePath)
		{
			return SerializeFile(new FileInfo(filePath));
		}

		public static string SerializeFile(FileInfo file)
		{
			if (!file.Exists)
			{
				throw new FileNotFoundException($"Dependency file not found: '{file.Name}'");
			}
			return EncodeFile(ZipFile(file));
		}

		public static DeserializeFileInfo DeserializeFile(string encodedFile)
		{
			return UnzipFile(DecodeFile(encodedFile));
		}

		private static byte[] ZipFile(FileInfo file)
		{
			FileStream fileStream = file.OpenRead();
			MemoryStream memoryStream = new MemoryStream();
			ZipOutputStream zipOutputStream = new ZipOutputStream(memoryStream);
			zipOutputStream.SetLevel(9);
			EnsureBackwardsCompatibility(zipOutputStream);
			ZipEntry zipEntry = new ZipEntry(Guid.NewGuid().ToString() + file.Extension);
			zipEntry.DateTime = GetFileLastWriteTime(file);
			zipOutputStream.PutNextEntry(zipEntry);
			try
			{
				byte[] buffer = new byte[1867776];
				int num = 0;
				do
				{
					num = fileStream.Read(buffer, 0, 1867776);
					zipOutputStream.Write(buffer, 0, num);
				}
				while (num > 0);
			}
			finally
			{
				fileStream?.Close();
			}
			zipOutputStream.Finish();
			zipOutputStream.Close();
			return memoryStream.ToArray();
		}

		private static DateTime GetFileLastWriteTime(FileInfo file)
		{
			try
			{
				return file.LastWriteTime;
			}
			catch
			{
				return DateTime.Now;
			}
		}

		private static void EnsureBackwardsCompatibility(ZipOutputStream zipOutputStream)
		{
			zipOutputStream.UseZip64 = UseZip64.Off;
		}

		public static DeserializeFileInfo UnzipExternalFile(string filePath)
		{
			if (!File.Exists(filePath))
			{
				throw new FileNotFoundException();
			}
			DeserializeFileInfo deserializeFileInfo = null;
			using (FileStream zipStream = File.OpenRead(filePath))
			{
				return UnzipFile(zipStream);
			}
		}

		private static DeserializeFileInfo UnzipFile(Stream zipStream)
		{
			bool isFileCreated = false;
			bool isDirectoryCreated = false;
			string text = null;
			ZipFile zipFile = new ZipFile(zipStream);
			if (zipFile != null)
			{
				foreach (ZipEntry item in zipFile)
				{
					if (item.IsFile)
					{
						text = Path.Combine(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()), item.Name);
						if (!Directory.Exists(Path.GetDirectoryName(text)))
						{
							Directory.CreateDirectory(Path.GetDirectoryName(text));
							isDirectoryCreated = true;
						}
						using (FileStream fileStream = File.OpenWrite(text))
						{
							byte[] array = new byte[1867776];
							Stream inputStream = zipFile.GetInputStream(item);
							int num = 0;
							while (true)
							{
								num = inputStream.Read(array, 0, array.Length);
								if (num <= 0)
								{
									break;
								}
								fileStream.Write(array, 0, num);
							}
							fileStream.Close();
						}
						File.SetCreationTimeUtc(text, item.DateTime);
						File.SetLastWriteTimeUtc(text, item.DateTime);
						isFileCreated = true;
					}
					zipFile.Close();
				}
			}
			return new DeserializeFileInfo(isFileCreated, isDirectoryCreated, text);
		}

		public static string ZipExternalFile(string filepath)
		{
			string tempFileName = Path.GetTempFileName();
			FileStream fileStream = null;
			ZipOutputStream zipOutputStream = null;
			try
			{
				fileStream = File.Create(tempFileName, 1867776);
				zipOutputStream = new ZipOutputStream(fileStream);
				zipOutputStream.SetLevel(9);
				EnsureBackwardsCompatibility(zipOutputStream);
				ZipEntry zipEntry = new ZipEntry(Path.GetRandomFileName() + Path.GetExtension(filepath));
				zipEntry.DateTime = File.GetLastWriteTimeUtc(filepath);
				zipOutputStream.PutNextEntry(zipEntry);
				FileStream fileStream2 = File.OpenRead(filepath);
				try
				{
					byte[] buffer = new byte[1867776];
					int num = 0;
					do
					{
						num = fileStream2.Read(buffer, 0, 1867776);
						zipOutputStream.Write(buffer, 0, num);
					}
					while (num > 0);
					return tempFileName;
				}
				finally
				{
					fileStream2?.Close();
				}
			}
			finally
			{
				if (zipOutputStream != null)
				{
					zipOutputStream.Finish();
					zipOutputStream.Close();
					zipOutputStream = null;
				}
				if (fileStream != null)
				{
					fileStream.Close();
					fileStream = null;
				}
			}
		}

		private static string EncodeFile(byte[] zipBuffer)
		{
			MemoryStream memoryStream = new MemoryStream(zipBuffer);
			StringBuilder stringBuilder = new StringBuilder();
			byte[] array = new byte[1867776];
			int num = 0;
			do
			{
				num = memoryStream.Read(array, 0, 1867776);
				stringBuilder.Append(Convert.ToBase64String(array, 0, num));
			}
			while (num > 0);
			return stringBuilder.ToString();
		}

		private static Stream DecodeFile(string encodedFile)
		{
			return new MemoryStream(Convert.FromBase64String(encodedFile));
		}
	}
}
