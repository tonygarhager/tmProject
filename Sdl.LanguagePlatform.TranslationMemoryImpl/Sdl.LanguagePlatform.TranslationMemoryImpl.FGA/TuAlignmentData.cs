using Sdl.Core.FineGrainedAlignment.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using System;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl.FGA
{
	public class TuAlignmentData
	{
		public PersistentObjectToken tuId;

		public LiftAlignedSpanPairSet AlignmentData;

		public DateTime? AlignModelDate;

		public DateTime InsertDate;
	}
}
