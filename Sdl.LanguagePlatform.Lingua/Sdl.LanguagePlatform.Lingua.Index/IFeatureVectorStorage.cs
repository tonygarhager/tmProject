namespace Sdl.LanguagePlatform.Lingua.Index
{
	public interface IFeatureVectorStorage
	{
		void Add(int key, IntFeatureVector fv);

		void Delete(int key);

		IntFeatureVector GetFeatureVector(int key);
	}
}
