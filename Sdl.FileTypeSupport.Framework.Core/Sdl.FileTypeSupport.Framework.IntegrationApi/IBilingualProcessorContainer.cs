using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public interface IBilingualProcessorContainer
	{
		Predicate<IPersistentFileConversionProperties> FileRestriction
		{
			get;
			set;
		}

		Predicate<IParagraphUnitProperties> ParagraphUnitRestriction
		{
			get;
			set;
		}

		void AddBilingualProcessor(IBilingualContentProcessor processor);

		void InsertBilingualProcessor(int index, IBilingualContentProcessor processor);

		bool RemoveBilingualProcessor(IBilingualContentProcessor processor);

		IEnumerable<IBilingualContentProcessor> GetBilingualProcessors();
	}
}
