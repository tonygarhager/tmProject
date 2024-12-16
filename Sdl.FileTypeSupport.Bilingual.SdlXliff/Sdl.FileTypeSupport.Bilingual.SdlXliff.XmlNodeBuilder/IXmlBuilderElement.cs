using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Sdl.FileTypeSupport.Bilingual.SdlXliff.XmlNodeBuilder
{
	public interface IXmlBuilderElement : IXmlBuilderContainer, IList<IXmlBuilderNode>, ICollection<IXmlBuilderNode>, IEnumerable<IXmlBuilderNode>, IEnumerable, IXmlBuilderNode
	{
		List<IXmlBuilderAttribute> Attributes
		{
			get;
			set;
		}

		List<IXmlBuilderNamespace> Namespaces
		{
			get;
			set;
		}

		string Name
		{
			get;
			set;
		}

		string Prefix
		{
			get;
			set;
		}

		string NsUri
		{
			get;
			set;
		}

		bool IsPlaceholder
		{
			get;
		}

		string StartTag
		{
			get;
		}

		string EndTag
		{
			get;
		}

		string StartTagNonPlaceholder
		{
			get;
		}

		string EndTagNonPlaceholder
		{
			get;
		}

		void AppendStartTag(StringBuilder content);

		void AppendEndTag(StringBuilder content);

		void AppendStartTagNonPlaceholder(StringBuilder content);

		void AppendEndTagNonPlaceholder(StringBuilder content);
	}
}
