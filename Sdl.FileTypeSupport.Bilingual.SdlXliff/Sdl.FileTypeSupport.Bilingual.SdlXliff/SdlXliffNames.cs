using System.Xml;

namespace Sdl.FileTypeSupport.Bilingual.SdlXliff
{
	public static class SdlXliffNames
	{
		public const string SdlXliffNamespaceUri = "http://sdl.com/FileTypes/SdlXliff/1.0";

		public const string SdlNamespacePrefix = "sdl";

		public static string Prefixed(string name, XmlNamespaceManager nsManager)
		{
			string text = nsManager.LookupPrefix("http://sdl.com/FileTypes/SdlXliff/1.0");
			if (string.IsNullOrEmpty(text))
			{
				return name;
			}
			return text + ":" + name;
		}
	}
}
