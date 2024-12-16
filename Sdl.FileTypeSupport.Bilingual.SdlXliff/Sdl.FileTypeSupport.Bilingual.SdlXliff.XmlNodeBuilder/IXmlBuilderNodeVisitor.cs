namespace Sdl.FileTypeSupport.Bilingual.SdlXliff.XmlNodeBuilder
{
	public interface IXmlBuilderNodeVisitor
	{
		void VisitElement(IXmlBuilderElement element);

		void VisitText(IXmlText text);

		void VisitXmlDeclaration(IXmlBuilderXmlDeclaration declaration);
	}
}
