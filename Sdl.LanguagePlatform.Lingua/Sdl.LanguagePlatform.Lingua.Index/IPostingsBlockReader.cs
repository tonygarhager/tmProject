using System.Collections.Generic;

namespace Sdl.LanguagePlatform.Lingua.Index
{
	public interface IPostingsBlockReader
	{
		PostingsInfo GetPostingsInfo(int feature);

		List<PostingsInfo> GetPostingsInfo(IntFeatureVector features);

		PostingsBlock ReadBlockAfter(int feature, int lastKey);

		PostingsBlock ReadBlockBefore(int feature, int nextKey);

		PostingsBlock ReadBlockContaining(int feature, int key);
	}
}
