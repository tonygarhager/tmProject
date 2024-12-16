namespace Sdl.LanguagePlatform.Lingua.Index
{
	public interface IFuzzyDataReader
	{
		AbstractPostingsIterator GetIterator(int feature, bool orderDescending);

		bool ContainsFeature(int feature);

		int GetPostingsCount(int feature);

		IntFeatureVector GetFeatureVector(int key);
	}
}
