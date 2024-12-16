using Sdl.FileTypeSupport.Framework.Core.Settings.Serialization;

namespace Sdl.FileTypeSupport.Bilingual.SdlXliff
{
	public class SdlXliffGeneralSettings : AbstractSettingsClass
	{
		private const bool DefaultValidateXliff = true;

		private const int DefaultMaxEmbeddedFileSize = 20;

		private const string SettingValidateXliff = "ValidateXliff";

		private const string SettingMaxFileEmbed = "MaximumFileEmbed";

		public override string SettingName => "GeneralSettings";

		public bool ValidateXliff
		{
			get;
			set;
		}

		public int MaxEmbeddedFileSize
		{
			get;
			set;
		}

		public SdlXliffGeneralSettings()
		{
			ResetToDefaults();
		}

		internal long GetMaximumFileEmbedSizeInBytes()
		{
			return MaxEmbeddedFileSize * 1048576;
		}

		public override void ResetToDefaults()
		{
			ValidateXliff = true;
			MaxEmbeddedFileSize = 20;
		}

		public override void Read(IValueGetter valueGetter)
		{
			ValidateXliff = valueGetter.GetValue("ValidateXliff", defaultValue: true);
			MaxEmbeddedFileSize = valueGetter.GetValue("MaximumFileEmbed", 20);
		}

		public override void Save(IValueProcessor valueProcessor)
		{
			valueProcessor.Process("ValidateXliff", ValidateXliff, defaultValue: true);
			valueProcessor.Process("MaximumFileEmbed", MaxEmbeddedFileSize, 20);
		}

		public override object Clone()
		{
			return new SdlXliffGeneralSettings
			{
				ValidateXliff = ValidateXliff,
				MaxEmbeddedFileSize = MaxEmbeddedFileSize
			};
		}

		public override bool Equals(ISettingsClass other)
		{
			SdlXliffGeneralSettings sdlXliffGeneralSettings = other as SdlXliffGeneralSettings;
			if (sdlXliffGeneralSettings != null && sdlXliffGeneralSettings.ValidateXliff == ValidateXliff)
			{
				return sdlXliffGeneralSettings.MaxEmbeddedFileSize == MaxEmbeddedFileSize;
			}
			return false;
		}
	}
}
