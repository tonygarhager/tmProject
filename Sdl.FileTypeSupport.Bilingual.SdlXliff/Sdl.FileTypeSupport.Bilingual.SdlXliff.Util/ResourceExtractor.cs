using System.IO;
using System.Reflection;

namespace Sdl.FileTypeSupport.Bilingual.SdlXliff.Util
{
	public static class ResourceExtractor
	{
		public static Stream GetResourceStream(string resourceId)
		{
			return Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceId);
		}

		public static void CreateFileFromResource(string resourceId, string filePath)
		{
			Stream resourceStream = GetResourceStream(resourceId);
			int num = checked((int)resourceStream.Length);
			byte[] array = new byte[num];
			resourceStream.Read(array, 0, num);
			File.WriteAllBytes(filePath, array);
		}
	}
}
