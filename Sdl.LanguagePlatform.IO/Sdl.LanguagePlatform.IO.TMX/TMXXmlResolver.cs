using System;
using System.IO;
using System.Xml;

namespace Sdl.LanguagePlatform.IO.TMX
{
	internal class TMXXmlResolver : XmlUrlResolver
	{
		public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
		{
			if (absoluteUri.AbsoluteUri.EndsWith("dtd", StringComparison.OrdinalIgnoreCase))
			{
				return new MemoryStream(new byte[0]);
			}
			return base.GetEntity(absoluteUri, role, ofObjectToReturn);
		}
	}
}
