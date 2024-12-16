namespace Sdl.FileTypeSupport.Bilingual.SdlXliff.XmlNodeBuilder
{
	public class XmlBuilderText : IXmlText, IXmlBuilderNode
	{
		public string Text
		{
			get;
			set;
		}

		public IXmlBuilderNode Parent
		{
			get;
			set;
		}

		public void AcceptVisitor(IXmlBuilderNodeVisitor visitor)
		{
			visitor.VisitText(this);
		}
	}
}
