using Sdl.Core.FineGrainedAlignment;
using System;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	public class TranslationModelCallContext : IDisposable
	{
		private readonly Dictionary<string, string> _StorageOptions;

		private readonly Container _Container;

		private ITranslationModelStorage _TranslationModelStorage;

		private ITranslationModelManager _TranslationModelManager;

		public ITranslationModelStorage TranslationModelStorage => _TranslationModelStorage ?? (_TranslationModelStorage = ((_Container != null) ? TranslationModelStorageFactory.Create(_Container) : TranslationModelStorageFactory.Create(_StorageOptions)));

		public ITranslationModelManager TranslationModelManager => _TranslationModelManager ?? (_TranslationModelManager = DefaultTranslationModelManagerFactory.GetTranslationModelManager(TranslationModelStorage));

		public bool IsFileBasedModel => TranslationModelStorage.IsFileBased();

		public TranslationModelCallContext(Container container, Dictionary<string, string> apiOptions)
		{
			_StorageOptions = apiOptions;
			_Container = container;
		}

		public void Complete()
		{
			if (_TranslationModelStorage != null)
			{
				_TranslationModelStorage.Flush();
				_TranslationModelStorage.CommitTransaction();
			}
		}

		public void Dispose()
		{
			if (_TranslationModelStorage != null)
			{
				_TranslationModelStorage.AbortTransaction();
				_TranslationModelStorage.Dispose();
				_TranslationModelStorage = null;
			}
		}
	}
}
