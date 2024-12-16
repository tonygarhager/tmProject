using Sdl.Enterprise2.Studio.Platform.Client.IdentityModel;
//using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Client;
using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	internal class TranslationMemoryAdminstrationFactory
	{
		public static TranslationMemoryAdministrationClient Create(Uri serverUri, UserCredentials credentials)
		{
			return null; //new TranslationMemoryAdministrationClient(serverUri.ToString(), credentials);
		}
	}
}
