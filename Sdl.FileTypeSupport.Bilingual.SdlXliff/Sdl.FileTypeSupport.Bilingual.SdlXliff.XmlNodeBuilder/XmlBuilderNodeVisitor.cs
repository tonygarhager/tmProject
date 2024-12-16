using System.Text;

namespace Sdl.FileTypeSupport.Bilingual.SdlXliff.XmlNodeBuilder
{
	public class XmlBuilderNodeVisitor : IXmlBuilderNodeVisitor
	{
		private readonly StringBuilder _content = new StringBuilder();

		private bool _reachedMidPoint;

		private readonly TreeGeneration _treeGeneration;

		public string Content => _content.ToString();

		public XmlBuilderNodeVisitor(TreeGeneration treeGeneration)
		{
			_treeGeneration = treeGeneration;
		}

		public void VisitNodes(IXmlBuilderContainer container)
		{
			foreach (IXmlBuilderNode item in container)
			{
				item.AcceptVisitor(this);
			}
		}

		public void VisitElement(IXmlBuilderElement element)
		{
			if (_treeGeneration != TreeGeneration.ClosingTags || element.IsPlaceholder)
			{
				if (_treeGeneration == TreeGeneration.FullTree)
				{
					element.AppendStartTag(_content);
				}
				else if (_treeGeneration != TreeGeneration.ClosingTags)
				{
					element.AppendStartTagNonPlaceholder(_content);
				}
			}
			if (!element.IsPlaceholder)
			{
				foreach (IXmlBuilderNode item in element)
				{
					item.AcceptVisitor(this);
				}
			}
			_reachedMidPoint = true;
			if (_treeGeneration != TreeGeneration.OpeningTags)
			{
				if (_treeGeneration == TreeGeneration.FullTree)
				{
					element.AppendEndTag(_content);
				}
				else
				{
					element.AppendEndTagNonPlaceholder(_content);
				}
			}
		}

		public void VisitText(IXmlText text)
		{
			if (!_reachedMidPoint && _treeGeneration != TreeGeneration.ClosingTags)
			{
				_content.Append(text.Text);
			}
			else if (_treeGeneration == TreeGeneration.FullTree)
			{
				_content.Append(text.Text);
			}
		}

		public void VisitXmlDeclaration(IXmlBuilderXmlDeclaration declaration)
		{
			_content.Append(declaration.Content);
		}
	}
}
