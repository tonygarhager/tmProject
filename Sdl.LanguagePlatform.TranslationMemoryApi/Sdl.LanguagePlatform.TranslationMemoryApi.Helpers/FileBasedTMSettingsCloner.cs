using Sdl.LanguagePlatform.TranslationMemory;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Helpers
{
	public static class FileBasedTMSettingsCloner
	{
		private static readonly List<PenaltyType> tuPenaltiesToRemove = new List<PenaltyType>
		{
			PenaltyType.ApprovedSignOff,
			PenaltyType.ApprovedTranslation,
			PenaltyType.Draft,
			PenaltyType.NotTranslated,
			PenaltyType.RejectedSignOff,
			PenaltyType.RejectedTranslation,
			PenaltyType.Translated
		};

		public static SearchSettings CloneSettings(SearchSettings settings)
		{
			return new SearchSettings
			{
				MaxResults = settings.MaxResults,
				MinScore = settings.MinScore,
				CurrentStructureContext = settings.CurrentStructureContext,
				ComputeTranslationProposal = settings.ComputeTranslationProposal,
				SortSpecification = settings.SortSpecification,
				HardFilter = settings.HardFilter,
				AutoLocalizationSettings = settings.AutoLocalizationSettings,
				Filters = settings.Filters,
				AdvancedTokenizationLegacyScoring = settings.AdvancedTokenizationLegacyScoring,
				CheckMatchingSublanguages = settings.CheckMatchingSublanguages,
				Penalties = settings.Penalties.Where((Penalty p) => !tuPenaltiesToRemove.Contains(p.PenaltyType)).ToList(),
				IsDocumentSearch = settings.IsDocumentSearch,
				MachineTranslationLookup = settings.MachineTranslationLookup,
				Mode = settings.Mode,
				AdaptiveMachineTranslationLookupMode = settings.AdaptiveMachineTranslationLookupMode
			};
		}
	}
}
