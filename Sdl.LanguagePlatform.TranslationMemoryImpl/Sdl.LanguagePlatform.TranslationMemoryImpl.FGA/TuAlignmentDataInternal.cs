using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl.FGA
{
	internal class TuAlignmentDataInternal
	{
		public PersistentObjectToken tuId;

		public byte[] alignmentData;

		public DateTime? alignModelDate;

		public HashSet<long> hashes;

		public List<byte> lengths;

		public List<byte> sigwordCounts;

		public DateTime insertDate;
	}
}
