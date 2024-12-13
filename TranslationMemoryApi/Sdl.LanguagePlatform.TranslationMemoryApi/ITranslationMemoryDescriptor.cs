using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryImpl;
using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Combines the name of a translation memory, the container it is in, and the service
	/// to use to access its data.
	/// </summary>
	internal interface ITranslationMemoryDescriptor
	{
		/// <summary>
		/// Gets the name of the translation memory.
		/// </summary>
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

		/// <summary>
		/// Gets the service through which the translation memory can be accessed.
		/// </summary>
		ITranslationMemoryService Service
		{
			get;
		}

		/// <summary>
		/// Gets the container the translation memory resides in.
		/// </summary>
		Container Container
		{
			get;
		}

		/// <summary>
		/// Returns the translation memory's setup information. 
		/// </summary>
		/// <returns>The translation memory setup of the TM described by this descriptor, or <c>null</c>
		/// if the TM does not exist. An exception may be thrown if the service is not properly 
		/// instantiated or the container does not exist.</returns>
		TranslationMemorySetup GetTranslationMemorySetup();
	}
}
