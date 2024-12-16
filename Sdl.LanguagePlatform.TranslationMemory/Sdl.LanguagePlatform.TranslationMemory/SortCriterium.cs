using System.Globalization;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	public class SortCriterium
	{
		private string _FieldName;

		[DataMember]
		public string FieldName
		{
			get
			{
				return _FieldName;
			}
			set
			{
				_FieldName = value.ToLower(CultureInfo.InvariantCulture);
			}
		}

		[DataMember]
		public SortDirection Direction
		{
			get;
			set;
		}

		public SortCriterium()
		{
		}

		public SortCriterium(string fieldName, SortDirection dir)
		{
			_FieldName = fieldName.ToLower(CultureInfo.InvariantCulture);
			Direction = dir;
		}
	}
}
