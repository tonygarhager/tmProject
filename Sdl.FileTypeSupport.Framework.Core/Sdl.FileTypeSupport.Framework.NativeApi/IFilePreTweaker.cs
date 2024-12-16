namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public interface IFilePreTweaker : IFileTweaker
	{
		void TweakFilePreParsing(IPersistentFileConversionProperties properties, IPropertiesFactory propertiesFactory);
	}
}
