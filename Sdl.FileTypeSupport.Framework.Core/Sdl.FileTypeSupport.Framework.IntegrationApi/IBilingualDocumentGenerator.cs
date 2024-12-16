using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public interface IBilingualDocumentGenerator : IBilingualProcessorContainer, IAbstractGenerator, IFileTypeDefinitionAware
	{
		IBilingualContentHandler Input
		{
			get;
		}

		IBilingualDocumentWriter Writer
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

		void AddFileTweaker(IFilePostTweaker tweaker);

		void RemoveFileTweaker(IFilePostTweaker tweakerToRemove);
	}
}
