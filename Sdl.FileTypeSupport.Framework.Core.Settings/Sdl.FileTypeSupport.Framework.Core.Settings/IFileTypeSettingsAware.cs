namespace Sdl.FileTypeSupport.Framework.Core.Settings
{
	public interface IFileTypeSettingsAware<SettingsType> where SettingsType : FileTypeSettingsBase
	{
		SettingsType Settings
		{
			get;
			set;
		}
	}
}
