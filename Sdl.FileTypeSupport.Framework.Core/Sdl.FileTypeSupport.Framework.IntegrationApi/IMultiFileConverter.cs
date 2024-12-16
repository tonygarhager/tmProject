using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public interface IMultiFileConverter : IBilingualProcessorContainer
	{
		IPropertiesFactory PropertiesFactory
		{
			get;
		}

		IDocumentItemFactory ItemFactory
		{
			get;
			set;
		}

		IDocumentProperties DocumentInfo
		{
			get;
		}

		ISharedObjects SharedObjects
		{
			get;
		}

		IEnumerable<IFileExtractor> Extractors
		{
			get;
		}

		IBilingualDocumentGenerator BilingualDocumentGenerator
		{
			get;
			set;
		}

		NativeGeneratorProvider NativeGeneratorProvider
		{
			get;
			set;
		}

		BilingualVerifiersProvider BilingualVerifiersProvider
		{
			get;
			set;
		}

		OutputPropertiesProvider OutputPropertiesProvider
		{
			get;
			set;
		}

		DependencyFileLocator DependencyFileLocator
		{
			get;
			set;
		}

		BilingualDocumentOutputPropertiesProvider BilingualDocumentOutputPropertiesProvider
		{
			get;
			set;
		}

		IBilingualDocumentOutputProperties BilingualDocumentOutputProperties
		{
			get;
		}

		bool DetectedLanguagesCorrespondToDocumentProperties
		{
			get;
		}

		Pair<Language, DetectionLevel> DetectedSourceLanguage
		{
			get;
		}

		Pair<Language, DetectionLevel> DetectedTargetLanguage
		{
			get;
		}

		event EventHandler<MessageEventArgs> Message;

		event EventHandler<BatchProgressEventArgs> Progress;

		void SetDocumentInfo(IDocumentProperties newDocumentInfo, bool applyToAllExtractors);

		bool UpdateDocumentPropertiesFromExtractors();

		void ApplyDocumentPropertiesToExtractors();

		void SynchronizeDocumentProperties();

		void Parse();

		bool ParseNext();

		void AddExtractor(IFileExtractor extractor);

		void InsertExtractor(int index, IFileExtractor extractor);

		bool RemoveExtractor(IFileExtractor extractor);
	}
}
