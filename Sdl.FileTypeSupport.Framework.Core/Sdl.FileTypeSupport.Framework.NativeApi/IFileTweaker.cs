namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public interface IFileTweaker
	{
		INativeTextLocationMessageReporter MessageReporter
		{
			get;
			set;
		}

		bool Enabled
		{
			get;
			set;
		}
	}
}
