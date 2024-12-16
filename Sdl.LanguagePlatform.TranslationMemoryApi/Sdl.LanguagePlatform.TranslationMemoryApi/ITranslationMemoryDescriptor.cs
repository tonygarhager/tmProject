using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryImpl;
using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	internal interface ITranslationMemoryDescriptor
	{
		string Name
		{
			get;
		}

		Uri Uri
		{
			get;
		}

		PersistentObjectToken Id
		{
			get;
		}

		ITranslationMemoryService Service
		{
			get;
		}

		Container Container
		{
			get;
		}

		TranslationMemorySetup GetTranslationMemorySetup();
	}
}
