using Sdl.FileTypeSupport.Bilingual.SdlXliff.XmlNodeBuilder;
using System;
using System.IO;
using System.Xml;

namespace Sdl.FileTypeSupport.Bilingual.SdlXliff
{
	internal class FileManager
	{
		public static void WriteZippedFile(XmlBuilder builder, string filePath)
		{
			Stream stream = new MemoryStream();
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.ConformanceLevel = ConformanceLevel.Fragment;
			XmlWriter xmlWriter = XmlWriter.Create(stream, xmlWriterSettings);
			using (FileStream fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				byte[] array = new byte[57];
				bool flag = true;
				int num = 0;
				do
				{
					if (!flag)
					{
						xmlWriter.WriteString(Environment.NewLine);
					}
					else
					{
						flag = false;
					}
					num = fileStream.Read(array, 0, array.Length);
					xmlWriter.WriteBase64(array, 0, num);
				}
				while (num > 0);
				fileStream.Close();
			}
			xmlWriter.Flush();
			stream.Seek(0L, SeekOrigin.Begin);
			TextReader textReader = new StreamReader(stream);
			builder.AddText(textReader.ReadToEnd());
			xmlWriter.Close();
		}
	}
}
