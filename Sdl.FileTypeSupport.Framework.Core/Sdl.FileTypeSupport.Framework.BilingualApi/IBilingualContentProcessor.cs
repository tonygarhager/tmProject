namespace Sdl.FileTypeSupport.Framework.BilingualApi
{
	public interface IBilingualContentProcessor : IBilingualContentHandler
	{
		IBilingualContentHandler Output
		{
			get;
			set;
		}
	}
}
