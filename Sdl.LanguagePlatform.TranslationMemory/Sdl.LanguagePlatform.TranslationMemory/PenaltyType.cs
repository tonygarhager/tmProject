using System;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	public enum PenaltyType
	{
		Unknown,
		TagMismatch,
		MemoryTagsDeleted,
		[Obsolete("unused penalty")]
		TargetSegmentMismatch,
		FilterPenalty,
		ProviderPenalty,
		MultipleTranslations,
		AutoLocalization,
		TextReplacement,
		Alignment,
		CharacterWidthDifference,
		NotTranslated,
		Draft,
		Translated,
		RejectedTranslation,
		ApprovedTranslation,
		RejectedSignOff,
		ApprovedSignOff
	}
}
