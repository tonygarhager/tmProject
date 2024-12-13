using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryImpl;
using System;
using System.Globalization;
using System.Linq;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	internal class InMemoryTranslationMemoryDescriptor : ITranslationMemoryDescriptor
	{
		private string _tmName;

		private API _service;

		private InMemoryContainer _container;

		private TranslationMemorySetup _setup;

		public Uri Uri => new Uri("tm.inmemory://" + _setup.LanguageDirection.SourceCultureName + "_" + _setup.LanguageDirection.TargetCultureName);

		public PersistentObjectToken Id => _setup.ResourceId;

		public Container Container => _container;

		public string Name => _tmName;

		public ITranslationMemoryService Service => _service;

		public InMemoryTranslationMemoryDescriptor(string name, CultureInfo sourceLanguage, CultureInfo targetLanguage, FuzzyIndexes indexes, BuiltinRecognizers recognizers)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			if (sourceLanguage == null)
			{
				throw new ArgumentNullException("sourceLanguage");
			}
			if (targetLanguage == null)
			{
				throw new ArgumentNullException("targetLanguage");
			}
			if (sourceLanguage.IsNeutralCulture)
			{
				throw new ArgumentException("Neutral cultures are not supported as the source language of a translation memory.");
			}
			if (targetLanguage.IsNeutralCulture)
			{
				throw new ArgumentException("Neutral cultures are not supported as the target language of a translation memory.");
			}
			_tmName = name;
			_service = new API();
			_container = new InMemoryContainer();
			if (!_service.SchemaExists(_container))
			{
				_service.CreateSchema(_container);
			}
			_setup = new TranslationMemorySetup();
			_setup.Name = name;
			_setup.Description = name;
			_setup.LanguageDirection = new LanguagePair(sourceLanguage, targetLanguage);
			_setup.FuzzyIndexes = indexes;
			_setup.Recognizers = recognizers;
			_setup.ResourceId = _service.CreateTranslationMemory(_container, _setup);
		}

		public TranslationMemorySetup GetTranslationMemorySetup()
		{
			TranslationMemorySetup translationMemorySetup = null;
			TranslationMemorySetup[] translationMemories = _service.GetTranslationMemories(Container, checkPermissions: true);
			if (translationMemories != null && translationMemories.Length != 0)
			{
				try
				{
					translationMemorySetup = translationMemories.First((TranslationMemorySetup setup) => setup.Name == Name);
				}
				catch (Exception)
				{
					throw new LanguagePlatformException(ErrorCode.TMNotFound);
				}
			}
			if (translationMemorySetup == null)
			{
				throw new LanguagePlatformException(ErrorCode.TMNotFound);
			}
			return translationMemorySetup;
		}
	}
}
