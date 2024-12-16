using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public interface IFileExtractor : IBilingualProcessorContainer, IFileTypeDefinitionAware
	{
		IDocumentItemFactory ItemFactory
		{
			get;
			set;
		}

		IDocumentProperties DocumentInfo
		{
			get;
			set;
		}

		IPersistentFileConversionProperties FileConversionProperties
		{
			get;
			set;
		}

		IBilingualContentHandler Output
		{
			get;
			set;
		}

		IBilingualParser BilingualParser
		{
			get;
			set;
		}

		INativeExtractor NativeExtractor
		{
			get;
			set;
		}

		INativeToBilingualConverter ToBilingualConverter
		{
			get;
			set;
		}

		IEnumerable<object> AllComponents
		{
			get;
		}

		IEnumerable<IFilePreTweaker> FileTweakers
		{
			get;
		}

		ISettingsBundle SettingsBundle
		{
			get;
			set;
		}

		event EventHandler<ProgressEventArgs> Progress;

		event EventHandler<MessageEventArgs> Message;

		void Parse();

		bool ParseNext();

		void AddFileTweaker(IFilePreTweaker tweaker);

		void RemoveFileTweaker(IFilePreTweaker tweakerToRemove);
	}
}
