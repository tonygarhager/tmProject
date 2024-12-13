using Sdl.LanguagePlatform.TranslationMemory;
using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	/// <summary>
	/// Represents an engine which can be used to export translation memory data.
	/// </summary>
	public interface ITranslationMemoryExporter
	{
		/// <summary>
		/// Gets or sets the number of translation units to retrieve from the TM in one roundtrip.
		/// <remarks>TODO default, max?</remarks>
		/// </summary>
		int BatchSize
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a filter expression which can be used to export only those translation 
		/// units which satisfy the filter condition.
		/// </summary>
		FilterExpression Filter
		{
			get;
			set;
		}

		/// <summary>
		/// The output path to write the exported data to. The data will be stored compressed if this
		/// path ends in a ".gz" extension.
		/// </summary>
		string TmxFilePath
		{
			get;
		}

		/// <summary>
		/// An event which can be subscribed to in order to get progress information or
		/// to cancel the export process.
		/// </summary>
		event EventHandler<BatchExportedEventArgs> BatchExported;

		/// <summary>
		/// Synchronously starts the export process. To cancel the process, you should subscribe
		/// to the <see cref="E:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.ITranslationMemoryExporter.BatchExported" /> event.
		/// </summary>
		void Export();
	}
}
