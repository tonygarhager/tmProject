using Sdl.Core.PluginFramework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// This class exposes functionality to work with translation provider plug-ins (<see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider" />) in a generic way.
	/// It allows creating translation providers in a generic way and also allows creating user interface components to manipulate 
	/// translation provider settings (<see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderWinFormsUI" />).
	/// </summary>
	public class TranslationProviderManager
	{
		private static IList<ITranslationProviderFactory> _translationProviderFactories;

		/// <summary>
		/// Creates a new translation provider for a given URI by selecting the appropriate factory (<see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderFactory" />).
		/// </summary>
		/// <param name="translationProviderUri">A translation provider URI (see <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.Uri" />).</param>
		/// <param name="translationProviderState">Serialized state information to be loaded into the translation provider after instantiation
		/// (see <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.LoadState(System.String)" /> and <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SerializeState" />).</param>
		/// <param name="credentialStore">A credential store object, which provides the relevant translation provider factory 
		/// with the required credentials need to instantiate the translation provider.</param>
		/// <exception cref="T:System.ArgumentNullException">Thrown if <paramref name="translationProviderUri" />, <paramref name="translationProviderState" /> or <paramref name="credentialStore" /> 
		/// i null.</exception>
		/// <exception cref="T:System.ArgumentException">Thrown if <paramref name="translationProviderUri" /> is not supported by any of the available factories.</exception>
		/// <exception cref="T:System.ArgumentException">Thrown if <paramref name="translationProviderState" /> is invalid.</exception>
		/// <exception cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.TranslationProviderAuthenticationException">Thrown when no credentials are available and no UI can be shown, or if the credentials provided by the user 
		/// are invalid.</exception>
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

		/// <summary>
		/// Gets a list of all available translation provider factories.
		/// </summary>
		/// <remarks>The factories returned are those that have been registered with the plug-in registry using the <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.TranslationProviderFactoryAttribute" /> attribute.</remarks>
		/// <returns>A list of all available translation provider factories.</returns>
		public static IList<ITranslationProviderFactory> GetTranslationProviderFactories()
		{
			if (_translationProviderFactories == null)
			{
				ObjectRegistry<TranslationProviderFactoryAttribute, ITranslationProviderFactory> objectRegistry = new ObjectRegistry<TranslationProviderFactoryAttribute, ITranslationProviderFactory>(PluginManager.DefaultPluginRegistry);
				_translationProviderFactories = objectRegistry.CreateObjects();
			}
			return _translationProviderFactories;
		}

		/// <summary>
		/// Gets the translation provider factory that supports the given translation provider uri.
		/// </summary>
		/// <param name="translationProviderUri">translation provider uri</param>
		/// <returns>translation provider factory</returns>
		public static ITranslationProviderFactory GetTranslationProviderFactory(Uri translationProviderUri)
		{
			return GetTranslationProviderFactories()?.FirstOrDefault((ITranslationProviderFactory provider) => provider?.SupportsTranslationProviderUri(translationProviderUri) ?? false);
		}

		/// <summary>
		/// Gets the translation provider win forms ui that supports the given translation provider uri.
		/// </summary>
		/// <param name="translationProviderUri">translation provider uri</param>
		/// <returns>translation provider win forms ui</returns>
		public static ITranslationProviderWinFormsUI GetTranslationProviderWinFormsUI(Uri translationProviderUri)
		{
			return GetTranslationProviderWinFormsUIs()?.FirstOrDefault((ITranslationProviderWinFormsUI provider) => provider?.SupportsTranslationProviderUri(translationProviderUri) ?? false);
		}

		/// <summary>
		/// Gets a list of all available translation provider win form uis.
		/// </summary>
		/// <remarks>The win form uis returned are those that have been registered with the plug-in registry using the <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.TranslationProviderWinFormsUiAttribute" /> attribute.</remarks>
		/// <returns>A list of all available translation provider win form uis.</returns>
		public static IList<ITranslationProviderWinFormsUI> GetTranslationProviderWinFormsUIs()
		{
			ObjectRegistry<TranslationProviderWinFormsUiAttribute, ITranslationProviderWinFormsUI> objectRegistry = new ObjectRegistry<TranslationProviderWinFormsUiAttribute, ITranslationProviderWinFormsUI>(PluginManager.DefaultPluginRegistry);
			return objectRegistry.CreateObjects();
		}
	}
}
