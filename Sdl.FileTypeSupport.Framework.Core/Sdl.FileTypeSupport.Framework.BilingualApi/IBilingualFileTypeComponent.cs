namespace Sdl.FileTypeSupport.Framework.BilingualApi
{
	public interface IBilingualFileTypeComponent
	{
		IDocumentItemFactory ItemFactory
		{
			get;
			set;
		}

		IBilingualContentMessageReporter MessageReporter
		{
			get;
			set;
		}
	}
}
