using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public interface INativeExtractor
	{
		INativeFileParser Parser
		{
			get;
			set;
		}

		IEnumerable<INativeExtractionContentProcessor> ContentProcessors
		{
			get;
		}

		INativeExtractionContentHandler Output
		{
			get;
			set;
		}

		void AddProcessor(INativeExtractionContentProcessor processor);

		void InsertProcessor(int index, INativeExtractionContentProcessor processor);

		bool RemoveProcessor(INativeExtractionContentProcessor processor);
	}
}
