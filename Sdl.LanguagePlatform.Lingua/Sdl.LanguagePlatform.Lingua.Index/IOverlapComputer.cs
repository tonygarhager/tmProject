namespace Sdl.LanguagePlatform.Lingua.Index
{
	public interface IOverlapComputer
	{
		double ComputeOverlap(IFeatureVector v1, IFeatureVector v2);
	}
}
