namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public interface IFilePostTweaker : IFileTweaker
	{
		void TweakFilePostWriting(INativeOutputFileProperties outputFileProperties);
	}
}
