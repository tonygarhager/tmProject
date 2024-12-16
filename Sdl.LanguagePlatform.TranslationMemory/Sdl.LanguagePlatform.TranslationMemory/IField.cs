using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	public interface IField
	{
		string Name
		{
			get;
			set;
		}

		FieldValueType ValueType
		{
			get;
			set;
		}

		IList<string> PicklistItemNames
		{
			get;
		}
	}
}
