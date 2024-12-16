using Sdl.LanguagePlatform.Core;
using System;
using System.Globalization;
using System.IO;

namespace Sdl.LanguagePlatform.Stat
{
	public class LTPComputer
	{
		private readonly DataFileInfo _info;

		public LTPComputer(DataLocation location, CultureInfo srcCulture, CultureInfo trgCulture)
		{
			_info = location.FindComponent(DataFileType.SimpleTranslationProbabilitiesFile, srcCulture, trgCulture);
		}

		public void Compute(double cutoff, int maxIterations)
		{
			Compute(cutoff, maxIterations, (TextWriter)null);
		}

		public void Compute(double cutoff, int maxIterations, string logFileName)
		{
			using (TextWriter logStream = File.CreateText(logFileName))
			{
				Compute(cutoff, maxIterations, logStream);
			}
		}

		public void Compute(double cutoff, int maxIterations, TextWriter logStream)
		{
			VocabularyFile vocabulary = _info.Location.GetVocabulary(_info.SourceCulture);
			vocabulary.Load();
			VocabularyFile vocabulary2 = _info.Location.GetVocabulary(_info.TargetCulture);
			vocabulary2.Load();
			TokenFileReader tokenFileReader = new TokenFileReader(_info.Location, _info.SourceCulture);
			tokenFileReader.Open();
			TokenFileReader tokenFileReader2 = new TokenFileReader(_info.Location, _info.TargetCulture);
			if (!tokenFileReader2.Exists)
			{
				throw new LanguagePlatformException(ErrorCode.DataComponentMissing, "Target sentence file");
			}
			tokenFileReader2.Open();
			int count = vocabulary.Count;
			int count2 = vocabulary2.Count;
			SparseMatrix<float> sparseMatrix = new SparseMatrix<float>();
			for (int i = 0; i < Math.Max(5, maxIterations); i++)
			{
				if (logStream != null)
				{
					logStream.WriteLine("========================== Iteration {0}", i);
					logStream.Flush();
				}
				float[] array = new float[vocabulary.Count];
				SparseArray<float>[] array2 = new SparseArray<float>[vocabulary.Count];
				if (tokenFileReader.Segments != tokenFileReader2.Segments)
				{
					throw new LanguagePlatformException(ErrorCode.CorruptData);
				}
				int j;
				for (j = 0; j < tokenFileReader.Segments; j++)
				{
					IntSegment segmentAt = tokenFileReader.GetSegmentAt(j);
					IntSegment segmentAt2 = tokenFileReader2.GetSegmentAt(j);
					if (i == 0)
					{
						for (int k = 0; k < segmentAt.Count; k++)
						{
							if (segmentAt[k] >= count)
							{
								throw new LanguagePlatformException(ErrorCode.DataComponentIncompatible);
							}
						}
						for (int l = 0; l < segmentAt2.Count; l++)
						{
							if (segmentAt2[l] >= count2)
							{
								throw new LanguagePlatformException(ErrorCode.DataComponentIncompatible);
							}
						}
					}
					segmentAt.Elements.Sort();
					segmentAt2.Elements.Sort();
					for (int m = 0; m < segmentAt2.Count; m++)
					{
						int num = 1;
						int num2;
						for (num2 = segmentAt2[m]; m + 1 < segmentAt2.Count && segmentAt2[m + 1] == num2; m++)
						{
							num++;
						}
						float num3 = 0f;
						for (int n = 0; n < segmentAt.Count; n++)
						{
							int num4;
							for (num4 = segmentAt[n]; n + 1 < segmentAt.Count && segmentAt[n + 1] == num4; n++)
							{
							}
							if (i == 0)
							{
								if (array2[num4] == null)
								{
									array2[num4] = new SparseArray<float>();
								}
								array2[num4][num2] += num;
								array[num4] += num;
							}
							else
							{
								num3 += sparseMatrix[num4, num2] * (float)num;
							}
						}
						if (i <= 0)
						{
							continue;
						}
						for (int num5 = 0; num5 < segmentAt.Elements.Count; num5++)
						{
							int num6 = 1;
							int num7;
							for (num7 = segmentAt[num5]; num5 + 1 < segmentAt.Count && segmentAt[num5 + 1] == num7; num5++)
							{
								num6++;
							}
							float num8 = sparseMatrix[num7, num2] * (float)num * (float)num6 / num3;
							if ((double)num8 > 0.0)
							{
								if (array2[num7] == null)
								{
									array2[num7] = new SparseArray<float>();
								}
								array2[num7][num2] += num8;
								array[num7] += num8;
							}
						}
					}
					if (j % 1000 == 0 && logStream != null)
					{
						logStream.Write(".");
						if (j % 50000 == 0)
						{
							logStream.WriteLine(j);
						}
						logStream.Flush();
					}
				}
				sparseMatrix = new SparseMatrix<float>();
				for (int num9 = 0; num9 < vocabulary.Count; num9++)
				{
					float num10 = array[num9];
					if (num10 > 0f)
					{
						foreach (int key in array2[num9].Keys)
						{
							sparseMatrix[num9, key] = array2[num9][key] / num10;
						}
					}
				}
				if (logStream != null)
				{
					logStream.WriteLine();
					logStream.WriteLine("Iteration {0}: {1} segment pairs read", i, j);
					logStream.Flush();
				}
			}
			tokenFileReader.Close();
			tokenFileReader2.Close();
			if (cutoff > 0.0)
			{
				sparseMatrix.DeleteIf((float v) => (double)v < cutoff);
			}
			SparseMatrixIO.Write(sparseMatrix, _info.Location.GetComponentFileName(DataFileType.SimpleTranslationProbabilitiesFile, _info.SourceCulture, _info.TargetCulture));
		}
	}
}
