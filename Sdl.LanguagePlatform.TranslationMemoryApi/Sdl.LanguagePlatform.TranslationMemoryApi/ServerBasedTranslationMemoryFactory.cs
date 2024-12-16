using Sdl.Enterprise2.Studio.Platform.Client.IdentityModel;
using System;
using System.Collections.Specialized;
using System.Web;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	[TranslationProviderFactory(Id = "ServerBasedTranslationMemoryFactory", Name = "ServerBasedTranslationMemoryFactory", Description = "ServerBasedTranslationMemoryFactory")]
	internal class ServerBasedTranslationMemoryFactory : ITranslationProviderFactory, ITranslationProviderSummaryFactory
	{
		private TranslationProviderServer _cachedServer;

		public bool SupportsTranslationProviderUri(Uri translationProviderUri)
		{
			return ServerBasedTranslationMemory.IsServerBasedTranslationMemory(translationProviderUri);
		}

		public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
		{
			UserCredentials userCredentials = GetUserCredentials(translationProviderUri, credentialStore);
			string serverBasedTranslationMemoryPath = ServerBasedTranslationMemory.GetServerBasedTranslationMemoryPath(translationProviderUri);
			TranslationProviderServer byUri = GetByUri(translationProviderUri, userCredentials);
			return byUri.GetTranslationMemory(serverBasedTranslationMemoryPath, TranslationMemoryProperties.LanguageDirections);
		}

		public ITranslationProvider GetTranslationProviderSummary(Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
		{
			UserCredentials userCredentials = GetUserCredentials(translationProviderUri, credentialStore);
			string serverBasedTranslationMemoryPath = ServerBasedTranslationMemory.GetServerBasedTranslationMemoryPath(translationProviderUri);
			TranslationProviderServer byUri = GetByUri(translationProviderUri, userCredentials);
			return byUri.GetTranslationMemorySummary(serverBasedTranslationMemoryPath, TranslationMemoryProperties.LanguageDirections);
		}

		private UserCredentials GetUserCredentials(Uri translationProviderUri, ITranslationProviderCredentialStore credentialStore)
		{
			TranslationProviderCredential credential = credentialStore.GetCredential(translationProviderUri);
			if (credential == null)
			{
				throw new TranslationProviderAuthenticationException($"No credentials specified to server '{IdentityInfoCacheCredentialStore.GetServerUri(translationProviderUri)}' (TM Uri='{translationProviderUri}').");
			}
			return IdentityInfoCacheCredentialStore.ToUserCredentials(credential);
		}

		private TranslationProviderServer GetByUri(Uri translationProvider, UserCredentials userCredentials)
		{
			string text;
			if (translationProvider.Scheme.EndsWith(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
			{
				text = Uri.UriSchemeHttps;
			}
			else if (translationProvider.Scheme.EndsWith(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase))
			{
				text = Uri.UriSchemeHttp;
			}
			else
			{
				if (!translationProvider.Scheme.EndsWith(Uri.UriSchemeNetTcp, StringComparison.OrdinalIgnoreCase))
				{
					throw new NotSupportedException("Uri Scheme " + translationProvider.Scheme + " is not supported.");
				}
				text = Uri.UriSchemeNetTcp;
			}
			string text2 = translationProvider.IsDefaultPort ? (text + "://" + translationProvider.Host) : (text + "://" + translationProvider.Host + ":" + translationProvider.Port.ToString());
			if (!string.IsNullOrWhiteSpace(translationProvider.Query))
			{
				text2 += translationProvider.AbsolutePath;
			}
			return GetCachedServer(text2, userCredentials);
		}

		private TranslationProviderServer GetCachedServer(string serverUri, UserCredentials userCredentials)
		{
			if (_cachedServer != null && _cachedServer.Uri.AbsoluteUri.Equals(serverUri))
			{
				return _cachedServer;
			}
			if (userCredentials.UserType == UserManagerTokenType.Saml2User)
			{
				_cachedServer = new TranslationProviderServer(new Uri(serverUri), userCredentials.UserName, userCredentials.SsoCredentials.AuthToken, userCredentials.SsoCredentials.ExpirationDate);
			}
			else
			{
				_cachedServer = new TranslationProviderServer(new Uri(serverUri), userCredentials);
			}
			_cachedServer = new TranslationProviderServer(new Uri(serverUri), userCredentials);
			return _cachedServer;
		}

		public TranslationProviderInfo GetTranslationProviderInfo(Uri translationProviderUri, string translationProviderState)
		{
			TranslationProviderInfo translationProviderInfo = new TranslationProviderInfo();
			if (string.IsNullOrEmpty(translationProviderUri.LocalPath.Replace("/", "")))
			{
				NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(translationProviderUri.Query);
				translationProviderInfo.Name = nameValueCollection["tmName"];
			}
			else
			{
				translationProviderInfo.Name = translationProviderUri.LocalPath;
			}
			translationProviderInfo.TranslationMethod = TranslationMethod.TranslationMemory;
			return translationProviderInfo;
		}
	}
}
