namespace Sdl.FileTypeSupport.Bilingual.SdlXliff.XmlNodeBuilder
{
	public class XmlFinderNodeVisitor : IXmlBuilderNodeVisitor
	{
		private IXmlBuilderNode _currentNode;

		private bool _found;

		private string _name;

		private string _prefix;

		public IXmlBuilderElement CurrentElement => _currentNode as IXmlBuilderElement;

		public void VisitNodes(IXmlBuilderContainer container, string prefix, string name)
		{
			_prefix = prefix;
			_name = name;
			foreach (IXmlBuilderNode item in container)
			{
				item.AcceptVisitor(this);
				if (_found)
				{
					break;
				}
			}
		}

		public void VisitElement(IXmlBuilderElement element)
		{
			if (!_found && element.Prefix == _prefix && element.Name == _name)
			{
				_found = true;
				_currentNode = element;
			}
			else if (!element.IsPlaceholder)
			{
				foreach (IXmlBuilderNode item in element)
				{
					item.AcceptVisitor(this);
				}
			}
		}

		public void VisitText(IXmlText text)
		{
		}

		public void VisitXmlDeclaration(IXmlBuilderXmlDeclaration declaration)
		{
		}
	}
}
