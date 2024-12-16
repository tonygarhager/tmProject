using System;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	public interface IFieldDefinitions
	{
		IField LookupIField(string name);

		IField LookupIField(Guid guid);
	}
}
