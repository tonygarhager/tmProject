using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public class TranslationProviderCredentialStore : ITranslationProviderCredentialStore, IXmlSerializable
	{
		private readonly IDictionary<Uri, TranslationProviderCredential> _credentials = new Dictionary<Uri, TranslationProviderCredential>();

		private readonly IdentityInfoCacheCredentialStore _identityInfoCacheCredentialStore = new IdentityInfoCacheCredentialStore();

		public event EventHandler CredentialsChanged;

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

		public void Clear()
		{
			_credentials.Clear();
			OnCredentialsChanged();
		}

		private void OnCredentialsChanged()
		{
			if (this.CredentialsChanged != null)
			{
				this.CredentialsChanged(this, EventArgs.Empty);
			}
		}

		public XmlSchema GetSchema()
		{
			return null;
		}

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

		private static bool UseIdentityInfoCacheCredentialStore(Uri uri)
		{
			return ServerBasedTranslationMemory.IsServerBasedTranslationMemory(uri);
		}
	}
}
