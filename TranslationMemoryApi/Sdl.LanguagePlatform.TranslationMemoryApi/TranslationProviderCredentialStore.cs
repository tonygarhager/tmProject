using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// TranslationProviderCredentialStore class represents a standard implementation of a translation provider credential store.
	/// </summary>
	public class TranslationProviderCredentialStore : ITranslationProviderCredentialStore, IXmlSerializable
	{
		private readonly IDictionary<Uri, TranslationProviderCredential> _credentials = new Dictionary<Uri, TranslationProviderCredential>();

		private readonly IdentityInfoCacheCredentialStore _identityInfoCacheCredentialStore = new IdentityInfoCacheCredentialStore();

		/// <summary>
		/// CredentialsChanged event is fired whenever the credentials are added to or removed from the store.
		/// </summary>
		public event EventHandler CredentialsChanged;

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
			if (UseIdentityInfoCacheCredentialStore(uri))
			{
				return _identityInfoCacheCredentialStore.GetCredential(uri);
			}
			if (_credentials.ContainsKey(uri))
			{
				return _credentials[uri];
			}
			return null;
		}

		/// <summary>
		/// Adds the given credential associated with the given uri.
		/// </summary>
		/// <param name="uri">uri</param>
		/// <param name="credential">credential</param>
		public void AddCredential(Uri uri, TranslationProviderCredential credential)
		{
			if (UseIdentityInfoCacheCredentialStore(uri))
			{
				_identityInfoCacheCredentialStore.AddCredential(uri, credential);
				return;
			}
			_credentials[uri] = credential;
			OnCredentialsChanged();
		}

		/// <summary>
		/// Removes the credential associated with the given uri.
		/// </summary>
		/// <param name="uri">uri</param>
		public void RemoveCredential(Uri uri)
		{
			if (UseIdentityInfoCacheCredentialStore(uri))
			{
				_identityInfoCacheCredentialStore.RemoveCredential(uri);
				return;
			}
			if (_credentials.ContainsKey(uri))
			{
				_credentials.Remove(uri);
			}
			OnCredentialsChanged();
		}

		/// <summary>
		/// Clears all the credentials.
		/// </summary>
		public void Clear()
		{
			_credentials.Clear();
			OnCredentialsChanged();
		}

		/// <summary>
		/// Fires the credentials changed event.
		/// </summary>
		private void OnCredentialsChanged()
		{
			if (this.CredentialsChanged != null)
			{
				this.CredentialsChanged(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets the xml schema.
		/// </summary>
		/// <returns>xml schema</returns>
		public XmlSchema GetSchema()
		{
			return null;
		}

		/// <summary>
		/// Reads the xml from the given reader.
		/// </summary>
		/// <param name="reader">reader</param>
		public void ReadXml(XmlReader reader)
		{
			Clear();
			reader.ReadStartElement();
			while (reader.Name == "CredentialEntry")
			{
				reader.ReadStartElement();
				string uriString = reader.ReadElementContentAsString();
				string credential = reader.ReadElementContentAsString();
				AddCredential(new Uri(uriString), new TranslationProviderCredential(credential, persist: true));
				reader.ReadEndElement();
			}
			reader.ReadEndElement();
		}

		/// <summary>
		/// Writes the xml to the given writer.
		/// </summary>
		/// <param name="writer">writer</param>
		public void WriteXml(XmlWriter writer)
		{
			foreach (KeyValuePair<Uri, TranslationProviderCredential> credential in _credentials)
			{
				if (credential.Value.Persist)
				{
					writer.WriteStartElement("CredentialEntry");
					writer.WriteElementString("Uri", credential.Key.AbsoluteUri);
					writer.WriteElementString("Credential", credential.Value.Credential);
					writer.WriteEndElement();
				}
			}
		}

		/// <summary>
		/// Determines whether to use the identity info cache credential store.
		/// </summary>
		/// <param name="uri">uri</param>
		/// <returns>whether to use the identity info cache credential store</returns>
		private static bool UseIdentityInfoCacheCredentialStore(Uri uri)
		{
			return ServerBasedTranslationMemory.IsServerBasedTranslationMemory(uri);
		}
	}
}
