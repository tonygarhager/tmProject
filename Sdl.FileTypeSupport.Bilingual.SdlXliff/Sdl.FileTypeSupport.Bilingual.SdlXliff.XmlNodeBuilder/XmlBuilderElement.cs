using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Sdl.FileTypeSupport.Bilingual.SdlXliff.XmlNodeBuilder
{
	public class XmlBuilderElement : AbstractXmlContainer, IXmlBuilderElement, IXmlBuilderContainer, IList<IXmlBuilderNode>, ICollection<IXmlBuilderNode>, IEnumerable<IXmlBuilderNode>, IEnumerable, IXmlBuilderNode
	{
		public string Name
		{
			get;
			set;
		}

		public string Prefix
		{
			get;
			set;
		}

		public bool IsPlaceholder => base.Count == 0;

		public IXmlBuilderNode Parent
		{
			get;
			set;
		}

		public List<IXmlBuilderAttribute> Attributes
		{
			get;
			set;
		} = new List<IXmlBuilderAttribute>();


		public string StartTag
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				AppendStartTag(stringBuilder);
				return stringBuilder.ToString();
			}
		}

		public string EndTag
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				AppendEndTag(stringBuilder);
				return stringBuilder.ToString();
			}
		}

		public string StartTagNonPlaceholder
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				AppendStartTagNonPlaceholder(stringBuilder);
				return stringBuilder.ToString();
			}
		}

		public string EndTagNonPlaceholder
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				AppendEndTagNonPlaceholder(stringBuilder);
				return stringBuilder.ToString();
			}
		}

		public string NsUri
		{
			get;
			set;
		}

		public List<IXmlBuilderNamespace> Namespaces
		{
			get;
			set;
		} = new List<IXmlBuilderNamespace>();


		public void AcceptVisitor(IXmlBuilderNodeVisitor visitor)
		{
			visitor.VisitElement(this);
		}

		public void AppendStartTag(StringBuilder content)
		{
			content.Append("<");
			if (!string.IsNullOrEmpty(Prefix))
			{
				content.Append(Prefix);
				content.Append(":");
			}
			content.Append(Name);
			if (!string.IsNullOrEmpty(NsUri))
			{
				if (!string.IsNullOrEmpty(Prefix))
				{
					content.Append(" xmlns:" + Prefix + "=");
				}
				else
				{
					content.Append(" xmlns=");
				}
				content.Append("\"" + NsUri + "\"");
			}
			if (Namespaces.Count > 0)
			{
				AppendNamespaces(content);
			}
			if (Attributes.Count > 0)
			{
				foreach (IXmlBuilderAttribute attribute in Attributes)
				{
					content.Append(" ");
					content.Append(attribute.Name);
					content.Append("=");
					content.Append("\"");
					content.Append(attribute.Value);
					content.Append("\"");
				}
			}
			content.Append(IsPlaceholder ? "/>" : ">");
		}

		private void AppendNamespaces(StringBuilder content)
		{
			foreach (IXmlBuilderNamespace @namespace in Namespaces)
			{
				if (!string.IsNullOrEmpty(@namespace.Prefix))
				{
					content.Append(" xmlns:" + @namespace.Prefix + "=");
				}
				else
				{
					content.Append(" xmlns=");
				}
				content.Append("\"" + @namespace.Uri + "\"");
			}
		}

		public void AppendEndTag(StringBuilder content)
		{
			if (!IsPlaceholder)
			{
				content.Append("</");
				if (!string.IsNullOrEmpty(Prefix))
				{
					content.Append(Prefix);
					content.Append(":");
				}
				content.Append(Name);
				content.Append(">");
			}
		}

		public void AppendStartTagNonPlaceholder(StringBuilder content)
		{
			content.Append("<");
			if (!string.IsNullOrEmpty(Prefix))
			{
				content.Append(Prefix);
				content.Append(":");
			}
			content.Append(Name);
			if (!string.IsNullOrEmpty(NsUri))
			{
				if (!string.IsNullOrEmpty(Prefix))
				{
					content.Append(" xmlns:" + Prefix + "=");
				}
				else
				{
					content.Append(" xmlns=");
				}
				content.Append("\"" + NsUri + "\"");
			}
			if (Namespaces.Count > 0)
			{
				foreach (IXmlBuilderNamespace @namespace in Namespaces)
				{
					if (!string.IsNullOrEmpty(@namespace.Prefix))
					{
						content.Append(" xmlns:" + @namespace.Prefix + "=");
					}
					else
					{
						content.Append(" xmlns=");
					}
					content.Append("\"" + @namespace.Uri + "\"");
				}
			}
			if (Attributes.Count > 0)
			{
				foreach (IXmlBuilderAttribute attribute in Attributes)
				{
					content.Append(" ");
					content.Append(attribute.Name);
					content.Append("=");
					content.Append("\"");
					content.Append(attribute.Value);
					content.Append("\"");
				}
			}
			content.Append(">");
		}

		public void AppendEndTagNonPlaceholder(StringBuilder content)
		{
			content.Append("</");
			if (!string.IsNullOrEmpty(Prefix))
			{
				content.Append(Prefix);
				content.Append(":");
			}
			content.Append(Name);
			content.Append(">");
		}

		public new IEnumerator GetEnumerator()
		{
			throw new NotImplementedException();
		}
	}
}
