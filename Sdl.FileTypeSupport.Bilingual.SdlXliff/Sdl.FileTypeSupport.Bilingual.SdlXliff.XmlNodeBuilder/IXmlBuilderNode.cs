namespace Sdl.FileTypeSupport.Bilingual.SdlXliff.XmlNodeBuilder
{
	public interface IXmlBuilderNode
	{
		IXmlBuilderNode Parent
		{
			get;
			set;
		}

		void AcceptVisitor(IXmlBuilderNodeVisitor visitor);
	}
}
