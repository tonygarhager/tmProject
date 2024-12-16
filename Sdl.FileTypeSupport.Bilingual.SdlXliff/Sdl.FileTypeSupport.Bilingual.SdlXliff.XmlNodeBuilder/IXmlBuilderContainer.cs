using System.Collections;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Bilingual.SdlXliff.XmlNodeBuilder
{
	public interface IXmlBuilderContainer : IList<IXmlBuilderNode>, ICollection<IXmlBuilderNode>, IEnumerable<IXmlBuilderNode>, IEnumerable
	{
	}
}
