using Sdl.Core.Globalization;
using Sdl.Core.Settings;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public interface INativeFileSniffer
	{
		SniffInfo Sniff(string nativeFilePath, Language suggestedSourceLanguage, Codepage suggestedCodepage, INativeTextLocationMessageReporter messageReporter, ISettingsGroup settingsGroup);
	}
}
