namespace Sdl.LanguagePlatform.Lingua.Index
{
	public class DiceFeatureVectorOverlapComputer : IOverlapComputer
	{
		public double ComputeOverlap(IFeatureVector v1, IFeatureVector v2)
		{
			double num = 0.0;
			double num2 = 0.0;
			int count = v1.Count;
			int count2 = v2.Count;
			double num3 = 0.0;
			int num4 = 0;
			int num5 = 0;
			while (num4 < count || num5 < count2)
			{
				bool flag = num4 < count;
				bool flag2 = num5 < count2;
				if (flag)
				{
					num += v1.GetWeightAt(num4);
				}
				if (flag2)
				{
					num2 += v2.GetWeightAt(num5);
				}
				if (flag && flag2)
				{
					int featureAt = v1.GetFeatureAt(num4);
					int featureAt2 = v2.GetFeatureAt(num5);
					if (featureAt < featureAt2)
					{
						num4++;
						continue;
					}
					if (featureAt2 < featureAt)
					{
						num5++;
						continue;
					}
					num3 = num3 + v1.GetWeightAt(num4) + v2.GetWeightAt(num5);
					num4++;
					num5++;
				}
				else if (flag)
				{
					num4++;
				}
				else
				{
					num5++;
				}
			}
			return num3 / (num + num2);
		}
	}
}
