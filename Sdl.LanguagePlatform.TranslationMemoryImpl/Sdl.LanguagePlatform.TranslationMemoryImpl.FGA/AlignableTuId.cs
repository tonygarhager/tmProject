using Sdl.Core.FineGrainedAlignment;
using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl.FGA
{
	internal class AlignableTuId : AlignableContentPairId
	{
		public PersistentObjectToken ResourceId
		{
			get;
			set;
		}

		public override string ToString()
		{
			return "AlignableTuId (" + ((ResourceId == null) ? "null" : (ResourceId.Guid.ToString() + ":" + ResourceId.Id.ToString())) + ")";
		}
	}
}
