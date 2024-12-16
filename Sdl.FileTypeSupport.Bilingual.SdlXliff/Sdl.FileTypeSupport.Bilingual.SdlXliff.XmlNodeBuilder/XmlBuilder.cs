using Sdl.FileTypeSupport.Framework;
using System.Linq;
using System.Xml;

namespace Sdl.FileTypeSupport.Bilingual.SdlXliff.XmlNodeBuilder
{
	public class XmlBuilder
	{
		private IXmlBuilderElement _rootNode;

		private IXmlBuilderElement _currentNode;

		private static XmlDocument _sXmlDocument = new XmlDocument();

		public IXmlBuilderNode CurrentNode => _currentNode;

		public IXmlBuilderElement RootElement => _rootNode;

		public bool HasContent => _rootNode.Count > 0;

		public static string XmlEscape(string unescaped)
		{
			if (string.IsNullOrEmpty(unescaped))
			{
				return unescaped;
			}
			XmlElement xmlElement = _sXmlDocument.CreateElement("root");
			xmlElement.InnerText = unescaped;
			return xmlElement.InnerXml.Replace("\"", "&quot;").Replace("'", "&apos;");
		}

		public XmlBuilder()
		{
			_rootNode = new XmlBuilderElement();
			_currentNode = _rootNode;
		}

		public void StartElement(string prefix, string name)
		{
			IXmlBuilderElement xmlBuilderElement = new XmlBuilderElement
			{
				Prefix = prefix,
				Name = name,
				Parent = _currentNode
			};
			_currentNode.Add(xmlBuilderElement);
			_currentNode = xmlBuilderElement;
		}

		public void StartElement(string prefix, string name, string namespaceUri)
		{
			IXmlBuilderElement xmlBuilderElement = new XmlBuilderElement
			{
				Prefix = prefix,
				NsUri = namespaceUri,
				Name = name,
				Parent = _currentNode
			};
			_currentNode.Add(xmlBuilderElement);
			_currentNode = xmlBuilderElement;
		}

		public void StartElement(string name)
		{
			IXmlBuilderElement xmlBuilderElement = new XmlBuilderElement
			{
				Name = name,
				Parent = _currentNode
			};
			_currentNode.Add(xmlBuilderElement);
			_currentNode = xmlBuilderElement;
		}

		public void AddAttribute(string name, string value)
		{
			if (_currentNode.Attributes.Where((IXmlBuilderAttribute attr) => attr.Name == name).Count() == 1)
			{
				throw new FileTypeSupportException("Attribute with the same name cannot be added multiple times");
			}
			value = XmlEscape(value);
			_currentNode.Attributes.Add(new XmlBuilderAttribute
			{
				Name = name,
				Value = value
			});
		}

		public void SetAttribute(string name, string value)
		{
			IXmlBuilderAttribute xmlBuilderAttribute = _currentNode.Attributes.FirstOrDefault((IXmlBuilderAttribute a) => a.Name == name);
			if (xmlBuilderAttribute != null)
			{
				value = XmlEscape(value);
				xmlBuilderAttribute.Value = value;
			}
			else
			{
				AddAttribute(name, value);
			}
		}

		public void AddText(string text)
		{
			_currentNode.Add(new XmlBuilderText
			{
				Text = XmlEscape(text)
			});
		}

		public void AddRawText(string text)
		{
			_currentNode.Add(new XmlBuilderText
			{
				Text = text
			});
		}

		public void EndElement()
		{
			if (_currentNode != _rootNode)
			{
				_currentNode = (_currentNode.Parent as IXmlBuilderElement);
			}
		}

		public string BuildXmlString(TreeGeneration treeGeneration)
		{
			XmlBuilderNodeVisitor xmlBuilderNodeVisitor = new XmlBuilderNodeVisitor(treeGeneration);
			xmlBuilderNodeVisitor.VisitNodes(_rootNode);
			return xmlBuilderNodeVisitor.Content;
		}

		public void Clear()
		{
			_rootNode = new XmlBuilderElement();
			_currentNode = _rootNode;
		}

		public void SetCurrentElementFromRoot(string prefix, string name)
		{
			XmlFinderNodeVisitor xmlFinderNodeVisitor = new XmlFinderNodeVisitor();
			xmlFinderNodeVisitor.VisitNodes(_rootNode, prefix, name);
			if (xmlFinderNodeVisitor.CurrentElement != null)
			{
				_currentNode = xmlFinderNodeVisitor.CurrentElement;
			}
		}

		public IXmlBuilderElement GetCurrentElementFromRoot(string prefix, string name)
		{
			XmlFinderNodeVisitor xmlFinderNodeVisitor = new XmlFinderNodeVisitor();
			xmlFinderNodeVisitor.VisitNodes(_rootNode, prefix, name);
			return xmlFinderNodeVisitor.CurrentElement;
		}

		public void AddDeclaration()
		{
			if (_rootNode.Count > 0 && _rootNode[0] is IXmlBuilderXmlDeclaration)
			{
				throw new FileTypeSupportException("XML Declaration already exists!");
			}
			_rootNode.Insert(0, new XmlBuilderXmlDeclaration());
		}

		public void AddNodesToBuilder(XmlBuilder builder)
		{
			foreach (IXmlBuilderNode item in builder._rootNode)
			{
				_currentNode.Add(item);
			}
		}

		public void AddNamespace(string prefix, string uri)
		{
			if (_currentNode.Namespaces.Where((IXmlBuilderNamespace ns) => ns.Prefix == prefix).Count() == 1)
			{
				throw new FileTypeSupportException("Namespace with same prefix cannot be added multiple times");
			}
			_currentNode.Namespaces.Add(new XmlBuilderNamespace
			{
				Prefix = prefix,
				Uri = uri
			});
		}

		public void SetNamespace(string prefix, string uri)
		{
			IXmlBuilderNamespace xmlBuilderNamespace = _currentNode.Namespaces.FirstOrDefault((IXmlBuilderNamespace ns) => ns.Prefix == prefix);
			if (xmlBuilderNamespace != null)
			{
				xmlBuilderNamespace.Uri = uri;
			}
			else
			{
				AddNamespace(prefix, uri);
			}
		}
	}
}
