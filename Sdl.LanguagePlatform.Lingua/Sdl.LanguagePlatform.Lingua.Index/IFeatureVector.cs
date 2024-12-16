namespace Sdl.LanguagePlatform.Lingua.Index
{
	public interface IFeatureVector
	{
		int Count
		{
			get;
		}

		int GetFeatureAt(int idx);

		double GetWeightAt(int idx);

		void Clear();
	}
}
