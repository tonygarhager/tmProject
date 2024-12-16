using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	public class ExportStatistics
	{
		[DataMember]
		public int Processed
		{
			get;
			set;
		}

		[DataMember]
		public int Exported
		{
			get;
			set;
		}
	}
}
