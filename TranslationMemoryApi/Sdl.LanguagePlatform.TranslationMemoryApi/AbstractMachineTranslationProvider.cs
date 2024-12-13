using Sdl.LanguagePlatform.Core;
using System;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Implements an abstract base class for machine translation providers, and provides
	/// overridable default implementations for the most common properties and methods
	/// of <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider" />.
	/// </summary>
	public abstract class AbstractMachineTranslationProvider : ITranslationProvider
	{
		/// <summary>
		/// The cached value used to return the provider's status information in 
		/// <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.AbstractMachineTranslationProvider.StatusInfo" />. The value is refreshed by <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.AbstractMachineTranslationProvider.RefreshStatusInfo" />.
		/// </summary>
		private ProviderStatusInfo _StatusInfo;

		/// <summary>
		/// See <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.StatusInfo" />. The default implementation 
		/// only refreshes the status information if it is <c>null</c>, and returns the
		/// cached value.
		/// </summary>
		public virtual ProviderStatusInfo StatusInfo
		{
			get
			{
				if (_StatusInfo == null)
				{
					RefreshStatusInfo();
				}
				return _StatusInfo;
			}
			set
			{
				_StatusInfo = value;
			}
		}

		/// <summary>
		/// See <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.Uri" />. This method is abstract
		/// and must be implemented by derived classes.
		/// </summary>
		public abstract Uri Uri
		{
			get;
		}

		/// <summary>
		/// See <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.Name" />. This method is abstract
		/// and must be implemented by derived classes.
		/// </summary>
		public abstract string Name
		{
			get;
		}

		/// <summary>
		/// Gets the list of language directions which are supported by this machine translation
		/// engine. Note that the list may include region-neutral cultures.
		/// </summary>
		public abstract IList<LanguagePair> SupportedLanguageDirections
		{
			get;
		}

		/// <summary>
		/// See <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsTaggedInput" />. The default implementation returns <c>false</c>.
		/// </summary>
		public virtual bool SupportsTaggedInput => false;

		/// <summary>
		/// See <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsScoring" />. The default implementation returns <c>false</c>.
		/// </summary>
		public virtual bool SupportsScoring => false;

		/// <summary>
		/// See <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsSearchForTranslationUnits" />. The default implementation returns <c>false</c>.
		/// </summary>
		public virtual bool SupportsSearchForTranslationUnits => false;

		/// <summary>
		/// See <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsMultipleResults" />. The default implementation returns <c>false</c>.
		/// </summary>
		public virtual bool SupportsMultipleResults => false;

		/// <summary>
		/// See <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsFilters" />. The default implementation returns <c>false</c>.
		/// </summary>
		public virtual bool SupportsFilters => false;

		/// <summary>
		/// See <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsPenalties" />. The default implementation returns <c>false</c>.
		/// </summary>
		public virtual bool SupportsPenalties => false;

		/// <summary>
		/// See <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsStructureContext" />. The default implementation returns <c>false</c>.
		/// </summary>
		public virtual bool SupportsStructureContext => false;

		/// <summary>
		/// See <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsDocumentSearches" />. The default implementation returns <c>false</c>.
		/// </summary>
		public virtual bool SupportsDocumentSearches => false;

		/// <summary>
		/// See <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsUpdate" />. The default implementation returns <c>false</c>.
		/// </summary>
		public virtual bool SupportsUpdate => false;

		/// <summary>
		/// See <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsPlaceables" />. The default implementation returns <c>false</c>.
		/// </summary>
		public virtual bool SupportsPlaceables => false;

		/// <summary>
		/// See <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsTranslation" />. The default implementation returns <c>true</c>.
		/// </summary>
		public virtual bool SupportsTranslation => true;

		/// <summary>
		/// See <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsFuzzySearch" />. The default implementation returns <c>false</c>.
		/// </summary>
		public virtual bool SupportsFuzzySearch => false;

		/// <summary>
		/// See <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsConcordanceSearch" />. The default implementation returns <c>false</c>.
		/// </summary>
		public virtual bool SupportsConcordanceSearch => false;

		/// <summary>
		/// See <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsSourceConcordanceSearch" />. The default implementation returns <c>false</c>.
		/// </summary>
		public virtual bool SupportsSourceConcordanceSearch => false;

		/// <summary>
		/// See <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsTargetConcordanceSearch" />. The default implementation returns <c>false</c>.
		/// </summary>
		public virtual bool SupportsTargetConcordanceSearch => false;

		/// <summary>
		/// See <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsWordCounts" />. The default implementation returns <c>false</c>.
		/// </summary>
		public virtual bool SupportsWordCounts => false;

		/// <summary>
		/// See <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.TranslationMethod" />. The default implementation returns <see cref="F:Sdl.LanguagePlatform.TranslationMemoryApi.TranslationMethod.MachineTranslation" />.
		/// </summary>
		public virtual TranslationMethod TranslationMethod => TranslationMethod.MachineTranslation;

		/// <summary>
		/// See <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.IsReadOnly" />. The default implementation returns <c>true</c>.
		/// </summary>
		public virtual bool IsReadOnly => true;

		/// <summary>
		/// See <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.RefreshStatusInfo" />. This method is abstract
		/// and must be implemented by derived classes.
		/// </summary>
		public void RefreshStatusInfo()
		{
			_StatusInfo = GetStatusInfo();
		}

		/// <summary>
		/// Gets up-to-date status info for this translation provider.
		/// </summary>
		/// <remarks>This method is called by <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.AbstractMachineTranslationProvider.RefreshStatusInfo" />.</remarks>
		/// <returns>Status information.</returns>
		protected abstract ProviderStatusInfo GetStatusInfo();

		/// <summary>
		/// Checks whether this machine translation provider supports the specified language direction.
		/// </summary>
		/// <param name="languageDirection">The language direction</param>
		/// <returns>True if the specified language direction is supported, and false otherwise.</returns>
		public abstract bool SupportsLanguageDirection(LanguagePair languageDirection);

		/// <summary>
		/// Obtains a translation provider for the specified language direction.
		/// </summary>
		/// <param name="languageDirection">The language direction</param>
		/// <returns>A translation provider language direction which matches the provided language direction.</returns>
		public abstract ITranslationProviderLanguageDirection GetLanguageDirection(LanguagePair languageDirection);

		/// <summary>
		/// Serializes any meaningful state information for this translation provider that can be stored in projects
		/// and sent around the supply chain.
		/// </summary>
		/// <returns>
		/// A string representing the state of this translation provider that can later be passed into
		/// the <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.AbstractMachineTranslationProvider.LoadState(System.String)" /> method to restore the state after creating a new translation provider.
		/// </returns>
		/// <remarks>
		/// <para>The format of this string can be decided upon by the translation provider implementation.</para>
		/// <para>The default implementation just returns <c>null</c>.</para>
		/// </remarks>
		public virtual string SerializeState()
		{
			return null;
		}

		/// <summary>
		/// Loads previously serialized state information into this translation provider instance.
		/// </summary>
		/// <param name="translationProviderState">A string representing the state of translation provider that was previously saved
		/// using <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.AbstractMachineTranslationProvider.SerializeState" />.</param>
		/// <remarks>
		/// <para>The format of this string can be decided upon by the translation provider implementation.</para>
		/// <para>The default implementation does not load any state.</para>
		/// </remarks>
		public virtual void LoadState(string translationProviderState)
		{
		}
	}
}
