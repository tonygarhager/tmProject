using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public interface INativeSubContentGenerator
	{
		INativeGenerationContentHandler Input
		{
			get;
		}

		IEnumerable<INativeGenerationContentProcessor> ContentProcessors
		{
			get;
		}

		INativeFileWriter Writer
		{
			get;
			set;
		}

		void AddProcessor(INativeGenerationContentProcessor processor);

		void InsertProcessor(int index, INativeGenerationContentProcessor processor);

		bool RemoveProcessor(INativeGenerationContentProcessor processor);
	}
}
