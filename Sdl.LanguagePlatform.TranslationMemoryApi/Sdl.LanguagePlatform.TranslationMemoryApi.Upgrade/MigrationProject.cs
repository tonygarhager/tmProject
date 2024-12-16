using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	internal class MigrationProject : IMigrationProject
	{
		private InputTranslationMemoryCollection _inputTranslationMemories;

		private OutputTranslationMemoryCollection _outputTranslationMemories;

		public IInputTranslationMemoryCollection InputTranslationMemories => _inputTranslationMemories;

		public IOutputTranslationMemoryCollection OutputTranslationMemories => _outputTranslationMemories;

		public MigrationProject()
		{
			_inputTranslationMemories = new InputTranslationMemoryCollection();
			_outputTranslationMemories = new OutputTranslationMemoryCollection();
		}

		public void ProcessAll(EventHandler<ProgressEventArgs> progressEventHandler)
		{
			if (OutputTranslationMemories.Count == 0)
			{
				throw new InvalidOperationException(StringResources.EMSG_NoTranslationMemoriesToProcess);
			}
			double exportTTMsPercentage = 0.4;
			double inputTMsPercentage = 1.0 - exportTTMsPercentage;
			int j;
			for (j = 0; j < InputTranslationMemories.Count; j++)
			{
				if (InputTranslationMemories[j].TmxFileStatus != TmxFileStatus.Available)
				{
					InputTranslationMemories[j].ExportTmxFile(delegate(object sender, BatchExportedEventArgs e)
					{
						if (progressEventHandler != null)
						{
							ProgressEventArgs e3 = new ProgressEventArgs((int)Math.Round(exportTTMsPercentage * 100.0 * (double)j / (double)InputTranslationMemories.Count), $"Exporting {InputTranslationMemories[j].TranslationMemory.Url} ({e.TotalExported} tus exported)");
							progressEventHandler(this, e3);
						}
					});
				}
			}
			int i;
			for (i = 0; i < OutputTranslationMemories.Count; i++)
			{
				OutputTranslationMemories[i].Process(delegate(object sender, ProgressEventArgs e)
				{
					if (progressEventHandler != null)
					{
						ProgressEventArgs e2 = new ProgressEventArgs((int)Math.Round(100.0 * exportTTMsPercentage + inputTMsPercentage * (100.0 * (double)i / (double)OutputTranslationMemories.Count + (double)e.PercentComplete)), e.InfoMessage);
						progressEventHandler(this, e2);
					}
				});
			}
		}
	}
}
