namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public interface INativeFileTypeComponent
	{
		IPropertiesFactory PropertiesFactory
		{
			get;
			set;
		}

		INativeContentStreamMessageReporter MessageReporter
		{
			get;
			set;
		}
	}
}
