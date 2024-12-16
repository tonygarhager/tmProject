using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public interface IFileGenerator : IBilingualProcessorContainer, IAbstractGenerator, IFileTypeDefinitionAware
	{
		IDocumentItemFactory ItemFactory
		{
			get;
			set;
		}

		INativeOutputFileProperties NativeOutputProperties
		{
			get;
			set;
		}

		IBilingualContentHandler Input
		{
			get;
		}

		INativeGenerator NativeGenerator
		{
			get;
			set;
		}

		IBilingualToNativeConverter ToNativeConverter
		{
			get;
			set;
		}

		IBilingualWriter BilingualWriter
		{
			get;
			set;
		}

		IEnumerable<object> AllComponents
		{
			get;
		}

		IEnumerable<IFilePostTweaker> FileTweakers
		{
			get;
		}

		IEnumerable<INativeFileVerifier> NativeVerifiers
		{
			get;
		}

		event EventHandler<MessageEventArgs> Message;

		void AddFileTweaker(IFilePostTweaker tweaker);

		void RemoveFileTweaker(IFilePostTweaker tweakerToRemove);

		void AddNativeVerifier(INativeFileVerifier verifier);
	}
}
