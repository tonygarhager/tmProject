using Newtonsoft.Json.Linq;

namespace Sdl.FileTypeSupport.Framework.Core.Settings.JsonSettings
{
	public class FileTypeSettingsJson
	{
		public string FileTypeId
		{
			get;
			set;
		}

		public string ComponentBuilderId
		{
			get;
			set;
		}

		public JObject FilterSettings
		{
			get;
			set;
		}

		public JObject VerificationSettings
		{
			get;
			set;
		}

		public JObject PreviewSettings
		{
			get;
			set;
		}

		public JObject EmbeddedContentProcessors
		{
			get;
			set;
		}
	}
}
