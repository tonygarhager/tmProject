using Sdl.LanguagePlatform.TranslationMemory.EditScripts;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	public class ExportSettings
	{
		[DataMember]
		public FilterExpression Filter
		{
			get;
			set;
		}

		[DataMember]
		public EditScript EditScript
		{
			get;
			set;
		}
	}
}
