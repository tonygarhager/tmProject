namespace Sdl.FileTypeSupport.Bilingual.SdlXliff.XmlNodeBuilder
{
	public class XmlBuilderXmlDeclaration : IXmlBuilderXmlDeclaration, IXmlBuilderNode
	{
		public string Content => "<?xml version=\"1.0\" encoding=\"utf-8\"?>";

		public IXmlBuilderNode Parent
		{
			get;
			set;
		}

		public void AcceptVisitor(IXmlBuilderNodeVisitor visitor)
		{
			visitor.VisitXmlDeclaration(this);
		}
	}
}
