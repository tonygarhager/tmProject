using Sdl.Core.LanguageProcessing.Resources;
using Sdl.LanguagePlatform.Lingua;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	internal class AnnotatedTranslationMemory
	{
		private LanguageTools _sourceTools;

		private LanguageTools _targetTools;

		private CompositeResourceDataAccessor _languageResourceAccessor;

		public TranslationMemorySetup Tm
		{
			get;
		}

		public int ResourcesWriteCount
		{
			get;
			private set;
		}

		public LanguageTools SourceTools
		{
			get
			{
				if (_sourceTools != null)
				{
					return _sourceTools;
				}
				bool useAlternateStemmers = !Tm.UsesLegacyHashes;
				LanguageResources resources = new LanguageResources(Tm.LanguageDirection.SourceCulture, _languageResourceAccessor);
				_sourceTools = new LanguageTools(resources, Tm.Recognizers, Tm.TokenizerFlags, useAlternateStemmers, Tm.NormalizeCharWidths);
				return _sourceTools;
			}
		}

		public LanguageTools TargetTools
		{
			get
			{
				if (_targetTools != null)
				{
					return _targetTools;
				}
				bool useAlternateStemmers = !Tm.UsesLegacyHashes;
				LanguageResources resources = new LanguageResources(Tm.LanguageDirection.TargetCulture, _languageResourceAccessor);
				_targetTools = new LanguageTools(resources, Tm.Recognizers, Tm.TokenizerFlags, useAlternateStemmers, Tm.NormalizeCharWidths);
				return _targetTools;
			}
		}

		public AnnotatedTranslationMemory(IList<LanguageResource> tmResources, int resourcesWriteCount, TranslationMemorySetup tm)
		{
			Tm = tm;
			SetResources(tmResources, resourcesWriteCount);
		}

		private void SetResources(IList<LanguageResource> tmResources, int resourcesWriteCount)
		{
			ResourcesWriteCount = resourcesWriteCount;
			_languageResourceAccessor = new CompositeResourceDataAccessor(addDefaultAccessor: true);
			if (tmResources != null && tmResources.Count > 0)
			{
				CachedResourceDataAccessor racc = new CachedResourceDataAccessor(tmResources);
				_languageResourceAccessor.Insert(0, racc);
			}
		}

		public void UpdateResources(IList<LanguageResource> tmResources, int resourcesWriteCount)
		{
			SetResources(tmResources, resourcesWriteCount);
			_sourceTools = null;
			_targetTools = null;
		}
	}
}
