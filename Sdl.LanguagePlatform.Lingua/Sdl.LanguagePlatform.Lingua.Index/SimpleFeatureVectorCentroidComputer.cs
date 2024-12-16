using System.Collections.Generic;

namespace Sdl.LanguagePlatform.Lingua.Index
{
	public class SimpleFeatureVectorCentroidComputer : ICentroidComputer
	{
		public void Add(WeightedFeatureVector centroid, WeightedFeatureVector fv, double minWeight)
		{
			if (centroid.Count == 0)
			{
				centroid.Set(fv);
				centroid.Denominator = 1;
				return;
			}
			int num = centroid.Denominator + fv.Denominator;
			int num2 = 0;
			int num3 = 0;
			while (num2 < centroid.Count || num3 < fv.Count)
			{
				bool flag = num2 < centroid.Count;
				bool flag2 = num3 < fv.Count;
				if (flag && flag2)
				{
					int featureAt = centroid.GetFeatureAt(num2);
					int featureAt2 = fv.GetFeatureAt(num3);
					double w;
					if (featureAt == featureAt2)
					{
						w = (centroid.GetWeightAt(num2) * (double)centroid.Denominator + fv.GetWeightAt(num3) * (double)fv.Denominator) / (double)num;
						centroid.SetWeightAt(num2, w);
						num2++;
						num3++;
						continue;
					}
					if (featureAt < featureAt2)
					{
						w = centroid.GetWeightAt(num2) * (double)centroid.Denominator / (double)num;
						centroid.SetWeightAt(num2, w);
						num2++;
						continue;
					}
					w = fv.GetWeightAt(num3) * (double)fv.Denominator / (double)num;
					if (w >= minWeight)
					{
						centroid.Add(fv.GetFeatureAt(num3), w);
						num2++;
					}
					num3++;
				}
				else if (flag)
				{
					double w = centroid.GetWeightAt(num2) * (double)centroid.Denominator / (double)num;
					centroid.SetWeightAt(num2, w);
					num2++;
				}
				else
				{
					double w = fv.GetWeightAt(num3) * (double)fv.Denominator / (double)num;
					if (w >= minWeight)
					{
						centroid.Add(fv.GetFeatureAt(num3), w);
						num2++;
					}
					num3++;
				}
			}
			centroid.Clean(minWeight);
			centroid.Denominator = num;
		}

		public void Remove(WeightedFeatureVector centroid, WeightedFeatureVector fv)
		{
			if (centroid.Count == 0 || fv.Count == 0)
			{
				return;
			}
			int num = centroid.Denominator - fv.Denominator;
			int num2 = 0;
			int num3 = 0;
			while (num2 < centroid.Count || num3 < fv.Count)
			{
				bool flag = num2 < centroid.Count;
				bool flag2 = num3 < fv.Count;
				if (flag && flag2)
				{
					int featureAt = centroid.GetFeatureAt(num2);
					int featureAt2 = fv.GetFeatureAt(num3);
					if (featureAt == featureAt2)
					{
						double w = (centroid.GetWeightAt(num2) * (double)centroid.Denominator - fv.GetWeightAt(num3) * (double)fv.Denominator) / (double)num;
						centroid.SetWeightAt(num2, w);
						num2++;
						num3++;
					}
					else if (featureAt < featureAt2)
					{
						double w = centroid.GetWeightAt(num2) * (double)centroid.Denominator / (double)num;
						centroid.SetWeightAt(num2, w);
						num2++;
					}
					else
					{
						num3++;
					}
				}
				else
				{
					if (!flag)
					{
						break;
					}
					double w = centroid.GetWeightAt(num2) * (double)centroid.Denominator / (double)num;
					centroid.SetWeightAt(num2, w);
					num2++;
				}
			}
			centroid.Denominator = num;
		}

		public void Compute(WeightedFeatureVector centroid, IEnumerable<IntFeatureVector> vectors, double minWeight)
		{
			centroid.Clear();
			int num = 0;
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			foreach (IntFeatureVector vector in vectors)
			{
				num++;
				foreach (int feature in vector.Features)
				{
					if (dictionary.ContainsKey(feature))
					{
						dictionary[feature]++;
					}
					else
					{
						dictionary.Add(feature, 1);
					}
				}
			}
			foreach (KeyValuePair<int, int> item in dictionary)
			{
				double num2 = (double)item.Value / (double)num;
				if (num2 >= minWeight)
				{
					centroid.Add(item.Key, num2);
				}
			}
			centroid.Denominator = num;
		}
	}
}
