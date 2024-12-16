using Sdl.Core.FineGrainedAlignment;
using System;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl.FGA
{
	internal class StorageBasedAlignableCorpusManager : IAlignableCorpusManager
	{
		private readonly CallContext _context;

		public StorageBasedAlignableCorpusManager(CallContext context)
		{
			_context = context;
		}

		public IAlignableCorpus GetAlignableCorpus(AlignableCorpusId id)
		{
			StorageBasedAlignableCorpusId storageBasedAlignableCorpusId = id as StorageBasedAlignableCorpusId;
			if (storageBasedAlignableCorpusId == null)
			{
				throw new ArgumentException("id");
			}
			AnnotatedTranslationMemory annotatedTranslationMemory = _context.GetAnnotatedTranslationMemory(storageBasedAlignableCorpusId.TmId);
			return new StorageBasedAlignableCorpus(_context.Storage, annotatedTranslationMemory, _context);
		}
	}
}
