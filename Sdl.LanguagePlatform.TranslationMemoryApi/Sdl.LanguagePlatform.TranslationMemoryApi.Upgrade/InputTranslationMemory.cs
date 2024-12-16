using System;
using System.IO;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	internal class InputTranslationMemory : IInputTranslationMemory
	{
		private ILegacyTranslationMemory _tm;

		private string _tmxFilePath;

		private ILegacyTranslationMemorySetup _lazySetup;

		private TmxFileStatus _tmxFileStatus;

		public ILegacyTranslationMemory TranslationMemory => _tm;

		public ILegacyTranslationMemorySetup Setup
		{
			get
			{
				if (_lazySetup == null)
				{
					_lazySetup = _tm.GetSetup();
				}
				return _lazySetup;
			}
		}

		public string TmxFilePath
		{
			get
			{
				return _tmxFilePath;
			}
			set
			{
				if (_tm is ITmxLegacyTranslationMemory)
				{
					throw new InvalidOperationException(string.Format(StringResources.EMSG_InputIsTMX));
				}
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentNullException("value");
				}
				_tmxFilePath = value;
				_tmxFileStatus = (File.Exists(_tmxFilePath) ? TmxFileStatus.Available : TmxFileStatus.None);
			}
		}

		public TmxFileStatus TmxFileStatus => _tmxFileStatus;

		private bool IsTmx => _tm is ITmxLegacyTranslationMemory;

		public InputTranslationMemory(ILegacyTranslationMemory tm)
		{
			if (tm == null)
			{
				throw new ArgumentNullException("tm");
			}
			_tm = tm;
			ITmxLegacyTranslationMemory tmxLegacyTranslationMemory = _tm as ITmxLegacyTranslationMemory;
			if (tmxLegacyTranslationMemory != null)
			{
				_tmxFilePath = tmxLegacyTranslationMemory.TmxFilePath;
				_tmxFileStatus = TmxFileStatus.Available;
			}
		}

		public void ExportTmxFile(EventHandler<BatchExportedEventArgs> progressEventHandler)
		{
			if (IsTmx)
			{
				throw new InvalidOperationException(StringResources.EMSG_CannotExportTMX);
			}
			if (string.IsNullOrEmpty(_tmxFilePath))
			{
				throw new InvalidOperationException(StringResources.EMSG_TMXPathUnspecified);
			}
			IExportableLegacyTranslationMemory exportableLegacyTranslationMemory = _tm as IExportableLegacyTranslationMemory;
			if (exportableLegacyTranslationMemory == null)
			{
				throw new InvalidOperationException(string.Format(StringResources.EMSG_CannotExport, _tm.GetType()));
			}
			ITranslationMemoryExporter translationMemoryExporter = exportableLegacyTranslationMemory.CreateExporter(_tmxFilePath);
			translationMemoryExporter.BatchExported += progressEventHandler;
			try
			{
				translationMemoryExporter.Export();
				if (File.Exists(_tmxFilePath))
				{
					_tmxFileStatus = TmxFileStatus.Available;
				}
			}
			catch (Exception)
			{
				_tmxFileStatus = TmxFileStatus.None;
				try
				{
					if (File.Exists(_tmxFilePath))
					{
						File.Delete(_tmxFilePath);
					}
				}
				catch
				{
				}
				throw;
			}
		}
	}
}
