using Sdl.LanguagePlatform.Core;
using System;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public abstract class AbstractMachineTranslationProvider : ITranslationProvider
	{
		private ProviderStatusInfo _StatusInfo;

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

		public abstract Uri Uri
		{
			get;
		}

		public abstract string Name
		{
			get;
		}

		public abstract IList<LanguagePair> SupportedLanguageDirections
		{
			get;
		}

		public virtual bool SupportsTaggedInput => false;

		public virtual bool SupportsScoring => false;

		public virtual bool SupportsSearchForTranslationUnits => false;

		public virtual bool SupportsMultipleResults => false;

		public virtual bool SupportsFilters => false;

		public virtual bool SupportsPenalties => false;

		public virtual bool SupportsStructureContext => false;

		public virtual bool SupportsDocumentSearches => false;

		public virtual bool SupportsUpdate => false;

		public virtual bool SupportsPlaceables => false;

		public virtual bool SupportsTranslation => true;

		public virtual bool SupportsFuzzySearch => false;

		public virtual bool SupportsConcordanceSearch => false;

		public virtual bool SupportsSourceConcordanceSearch => false;

		public virtual bool SupportsTargetConcordanceSearch => false;

		public virtual bool SupportsWordCounts => false;

		public virtual TranslationMethod TranslationMethod => TranslationMethod.MachineTranslation;

		public virtual bool IsReadOnly => true;

		public void RefreshStatusInfo()
		{
			_StatusInfo = GetStatusInfo();
		}

		protected abstract ProviderStatusInfo GetStatusInfo();

		public abstract bool SupportsLanguageDirection(LanguagePair languageDirection);

		public abstract ITranslationProviderLanguageDirection GetLanguageDirection(LanguagePair languageDirection);

		public virtual string SerializeState()
		{
			return null;
		}

		public virtual void LoadState(string translationProviderState)
		{
		}
	}
}
