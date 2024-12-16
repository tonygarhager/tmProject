using Sdl.Core.PluginFramework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public class TranslationProviderManager
	{
		private static IList<ITranslationProviderFactory> _translationProviderFactories;

		public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
		{
			IList<ITranslationProviderFactory> translationProviderFactories = GetTranslationProviderFactories();
			if (translationProviderFactories != null)
			{
				foreach (ITranslationProviderFactory item in translationProviderFactories)
				{
					if (item != null && item.SupportsTranslationProviderUri(translationProviderUri))
					{
						return item.CreateTranslationProvider(translationProviderUri, translationProviderState, credentialStore);
					}
				}
			}
			throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "No translation provider factory found for uri '{0}'.", translationProviderUri));
		}

		public static IList<ITranslationProviderFactory> GetTranslationProviderFactories()
		{
			if (_translationProviderFactories == null)
			{
				ObjectRegistry<TranslationProviderFactoryAttribute, ITranslationProviderFactory> objectRegistry = new ObjectRegistry<TranslationProviderFactoryAttribute, ITranslationProviderFactory>(PluginManager.DefaultPluginRegistry);
				_translationProviderFactories = objectRegistry.CreateObjects();
			}
			return _translationProviderFactories;
		}

		public static ITranslationProviderFactory GetTranslationProviderFactory(Uri translationProviderUri)
		{
			return GetTranslationProviderFactories()?.FirstOrDefault((ITranslationProviderFactory provider) => provider?.SupportsTranslationProviderUri(translationProviderUri) ?? false);
		}

		public static ITranslationProviderWinFormsUI GetTranslationProviderWinFormsUI(Uri translationProviderUri)
		{
			return GetTranslationProviderWinFormsUIs()?.FirstOrDefault((ITranslationProviderWinFormsUI provider) => provider?.SupportsTranslationProviderUri(translationProviderUri) ?? false);
		}

		public static IList<ITranslationProviderWinFormsUI> GetTranslationProviderWinFormsUIs()
		{
			ObjectRegistry<TranslationProviderWinFormsUiAttribute, ITranslationProviderWinFormsUI> objectRegistry = new ObjectRegistry<TranslationProviderWinFormsUiAttribute, ITranslationProviderWinFormsUI>(PluginManager.DefaultPluginRegistry);
			return objectRegistry.CreateObjects();
		}
	}
}
