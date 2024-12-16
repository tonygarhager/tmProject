using Sdl.LanguagePlatform.TranslationMemory;
using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	public interface ITranslationMemoryExporter
	{
		int BatchSize
		{
			get;
			set;
		}

		FilterExpression Filter
		{
			get;
			set;
		}

		string TmxFilePath
		{
			get;
		}

		event EventHandler<BatchExportedEventArgs> BatchExported;

		void Export();
	}
}
