using Sdl.Enterprise2.Studio.Platform.Client.IdentityModel;
using System;
using System.Globalization;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// IdentityInfoCacheCredentialStore class represents a translation provider credential store wrapper around IdentityInfoCache.
	/// </summary>
	internal class IdentityInfoCacheCredentialStore : ITranslationProviderCredentialStore
	{
		public const char Delimiter = '|';

		/// <summary>
		/// Converts the given user credentials to translation provider credential.
		/// </summary>
		/// <param name="userCredentials">user credentials</param>
		/// <returns>translation provider credential</returns>
		public static TranslationProviderCredential ToTranslationProviderCredential(UserCredentials userCredentials)
		{
			GenericCredentials genericCredentials = new GenericCredentials(userCredentials.UserName, userCredentials.Password);
			genericCredentials["type"] = userCredentials.UserType.ToString();
			if (userCredentials.UserType == UserManagerTokenType.Saml2User)
			{
				genericCredentials["SsoSamlToken"] = userCredentials.SsoCredentials.SamlToken;
				genericCredentials["SsoAuthToken"] = userCredentials.SsoCredentials.AuthToken;
				genericCredentials["SsoExp"] = userCredentials.SsoCredentials.ExpirationDate.ToString(CultureInfo.CurrentCulture);
			}
			return new TranslationProviderCredential(genericCredentials.ToCredentialString(), persist: false);
		}

		/// <summary>
		/// Converts the given translation provider credential to user credentials.
		/// </summary>
		/// <param name="translationProviderCredential">translation provider credential</param>
		/// <returns>user credentials</returns>
		public static UserCredentials ToUserCredentials(TranslationProviderCredential translationProviderCredential)
		{
			GenericCredentials genericCredentials = new GenericCredentials(translationProviderCredential.Credential);
			string text = genericCredentials["type"];
			if (text != null)
			{
				UserManagerTokenType userType = (UserManagerTokenType)Enum.Parse(typeof(UserManagerTokenType), text, ignoreCase: true);
				UserCredentials userCredentials = new UserCredentials(genericCredentials.UserName, genericCredentials.Password, userType);
				if (userCredentials.UserType == UserManagerTokenType.Saml2User)
				{
					userCredentials.SsoCredentials = new UserCredentials.SsoData
					{
						SamlToken = genericCredentials["SsoSamlToken"],
						AuthToken = genericCredentials["SsoAuthToken"],
						ExpirationDate = DateTime.Parse(genericCredentials["SsoExp"])
					};
				}
				return userCredentials;
			}
			throw new Exception("IdentityInfoCacheCredentialStore.ToUserCredentials((): unable to parse credentials string");
		}

		/// <summary>
		/// Gets the server uri from the given translation provider uri.
		/// </summary>
		/// <remarks>
		/// The server uri is needed by the IdentityInfoCache and not the translation provider uri.
		/// </remarks>
		/// <param name="translationProviderUri">translation provider uri</param>
		/// <returns>server uri</returns>
		public static Uri GetServerUri(Uri translationProviderUri)
		{
			if (translationProviderUri.Scheme.StartsWith("sdltm."))
			{
				if (!string.IsNullOrWhiteSpace(translationProviderUri.Query))
				{
					int num = translationProviderUri.OriginalString.IndexOf('?');
					int length = "sdltm.".Length;
					string uriString = translationProviderUri.AbsoluteUri.Substring(length, num - length);
					return new Uri(uriString);
				}
				string[] array = translationProviderUri.Scheme.Split('.');
				UriBuilder uriBuilder = new UriBuilder(array[1], translationProviderUri.Host, translationProviderUri.Port);
				return uriBuilder.Uri;
			}
			return translationProviderUri;
		}

		/// <summary>
		/// Gets the credential with the given uri.
		/// </summary>
		/// <remarks>
		/// If the credential does not exist then it returns null.
		/// </remarks>
		/// <param name="uri">uri</param>
		/// <returns>credential</returns>
		public TranslationProviderCredential GetCredential(Uri uri)
		{
			string identityKey = IdentityInfoCache.GetIdentityKey(GetServerUri(uri));
			try
			{
				UserCredentials userCredentials = IdentityInfoCache.Default.GetUserCredentials(identityKey);
				return (userCredentials != null) ? ToTranslationProviderCredential(userCredentials) : null;
			}
			catch (InvalidOperationException)
			{
				return null;
			}
		}

		/// <summary>
		/// Adds the given credential associated with the given uri.
		/// </summary>
		/// <param name="uri">uri</param>
		/// <param name="credential">credential</param>
		public void AddCredential(Uri uri, TranslationProviderCredential credential)
		{
			UserCredentials userCredentials = ToUserCredentials(credential);
			switch (userCredentials.UserType)
			{
			case UserManagerTokenType.CustomUser:
				IdentityInfoCache.Default.SetCustomIdentity(GetServerUri(uri), userCredentials.UserName, userCredentials.Password);
				break;
			case UserManagerTokenType.WindowsUser:
				IdentityInfoCache.Default.SetWindowsIdentity(GetServerUri(uri));
				break;
			case UserManagerTokenType.Saml2User:
				IdentityInfoCache.Default.SetCustomIdentity(GetServerUri(uri), userCredentials.UserName, userCredentials.SsoCredentials);
				break;
			default:
				throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "User type '{0}' not supported.", userCredentials.UserType));
			}
		}

		/// <summary>
		/// Removes the credential associated with the given uri.
		/// </summary>
		/// <param name="uri">uri</param>
		public void RemoveCredential(Uri uri)
		{
			string identityKey = IdentityInfoCache.GetIdentityKey(GetServerUri(uri));
			IdentityInfoCache.Default.RemoveUserCredentials(identityKey);
		}

		/// <summary>
		/// Clears all the credentials.
		/// </summary>
		public void Clear()
		{
			foreach (string identityKey in IdentityInfoCache.Default.IdentityKeys)
			{
				IdentityInfoCache.Default.RemoveUserCredentials(identityKey);
			}
		}
	}
}
