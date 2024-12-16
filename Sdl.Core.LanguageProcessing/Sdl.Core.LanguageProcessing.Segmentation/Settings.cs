using System.Runtime.Serialization;

namespace Sdl.Core.LanguageProcessing.Segmentation
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Sdl.LanguagePlatform.TranslationMemoryTools.Segmentation")]
	public class Settings
	{
		[field: DataMember]
		public TargetSegmentCreationMode TargetSegmentCreationMode
		{
			get;
			set;
		}

		[field: DataMember]
		public Mode Mode
		{
			get;
			set;
		}

		[field: DataMember]
		public bool DontSegmentIfTargetExists
		{
			get;
			set;
		}

		public Settings()
		{
			Mode = Mode.SentenceSegmentation;
			TargetSegmentCreationMode = TargetSegmentCreationMode.CreateEmptyTarget;
			DontSegmentIfTargetExists = true;
		}
	}
}
