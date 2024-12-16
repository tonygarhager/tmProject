using Sdl.Core.Globalization.LanguageRegistry;
using System.Collections.Generic;

namespace Sdl.Core.Globalization.NumberMetadata
{
	public class NumberMetadataRegistry
	{
		internal Dictionary<string, NumberMetadata> LanguageNumberMetadata = new Dictionary<string, NumberMetadata>();

		public NumberMetadata FindMetadata(string languageCode)
		{
			Language language = LanguageRegistryApi.Instance.GetLanguage(languageCode);
			NumberMetadata value = null;
			while (languageCode != null)
			{
				if (LanguageNumberMetadata.TryGetValue(languageCode, out value))
				{
					return value;
				}
				language = language.ParentLanguage;
				languageCode = language?.LanguageCode;
			}
			LanguageNumberMetadata.TryGetValue(string.Empty, out value);
			return value;
		}

		internal void Validate()
		{
			if (LanguageNumberMetadata == null)
			{
				throw new NumberMetadataRegistryException("NumberMetadata has not been initialised");
			}
			if (!LanguageNumberMetadata.ContainsKey(""))
			{
				throw new NumberMetadataRegistryException("Top-level NumberMetadata has not been initialised");
			}
			foreach (KeyValuePair<string, NumberMetadata> languageNumberMetadatum in LanguageNumberMetadata)
			{
				string text = languageNumberMetadatum.Value.Validate();
				if (text != null)
				{
					throw new NumberMetadataRegistryException("Number metadata is invalid for language code '" + languageNumberMetadatum.Key + "': " + text);
				}
			}
		}
	}
}
