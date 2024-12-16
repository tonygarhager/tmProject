using System.Collections.Generic;

namespace Sdl.LanguagePlatform.Lingua.Index
{
	public interface ICentroidComputer
	{
		void Add(WeightedFeatureVector centroid, WeightedFeatureVector fv, double minWeight);

		void Remove(WeightedFeatureVector centroid, WeightedFeatureVector fv);

		void Compute(WeightedFeatureVector centroid, IEnumerable<IntFeatureVector> vectors, double minWeight);
	}
}
