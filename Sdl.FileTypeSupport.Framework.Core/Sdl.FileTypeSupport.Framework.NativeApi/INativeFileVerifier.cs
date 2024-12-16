namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public interface INativeFileVerifier : INativeOutputSettingsAware
	{
		INativeTextLocationMessageReporter MessageReporter
		{
			get;
			set;
		}

		void Verify();
	}
}
