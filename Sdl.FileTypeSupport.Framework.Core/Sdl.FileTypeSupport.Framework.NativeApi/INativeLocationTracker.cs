namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public interface INativeLocationTracker
	{
		NativeTextLocation GetLocationBeforeCurrentContent();

		NativeTextLocation GetLocationAfterCurrentContent();
	}
}
