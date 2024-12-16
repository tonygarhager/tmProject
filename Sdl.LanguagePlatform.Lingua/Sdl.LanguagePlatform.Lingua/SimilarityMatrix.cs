using Sdl.LanguagePlatform.Core.Tokenization;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.Lingua
{
	internal class SimilarityMatrix
	{
		public delegate double TokenSimilarityComputer(Token a, Token b);

		private readonly double[,] _sim;

		private readonly bool _useStringEditDistance;

		private readonly BuiltinRecognizers _disabledAutoSubstitutions;

		private readonly bool _charactersNormalizeSafely;

		private readonly bool _applySmallChangeAdjustment;

		private const double Uncomputed = -1000.0;

		public IList<Token> SourceTokens
		{
			get;
		}

		public IList<Token> TargetTokens
		{
			get;
		}

		public double this[int s, int t]
		{
			get
			{
				double num = _sim[s, t];
				if (num != -1000.0)
				{
					return num;
				}
				num = SimilarityComputers.GetTokenSimilarity(SourceTokens[s], TargetTokens[t], _useStringEditDistance, _disabledAutoSubstitutions, _charactersNormalizeSafely, _applySmallChangeAdjustment);
				_sim[s, t] = num;
				return num;
			}
			set
			{
				_sim[s, t] = value;
			}
		}

		public SimilarityMatrix(IList<Token> sourceTokens, IList<Token> targetTokens, bool useStringEditDistance, BuiltinRecognizers disabledAutoSubstitutions, bool charactersNormalizeSafely, bool applySmallChangeAdjustment)
		{
			SourceTokens = sourceTokens;
			TargetTokens = targetTokens;
			_useStringEditDistance = useStringEditDistance;
			_disabledAutoSubstitutions = disabledAutoSubstitutions;
			_charactersNormalizeSafely = charactersNormalizeSafely;
			_applySmallChangeAdjustment = applySmallChangeAdjustment;
			_sim = new double[SourceTokens.Count, TargetTokens.Count];
			for (int i = 0; i < SourceTokens.Count; i++)
			{
				for (int j = 0; j < TargetTokens.Count; j++)
				{
					_sim[i, j] = -1000.0;
				}
			}
		}

		public bool IsAssigned(int s, int t)
		{
			return _sim[s, t] != -1000.0;
		}

		public int CountUncomputedValues()
		{
			int num = 0;
			for (int i = 0; i < SourceTokens.Count; i++)
			{
				for (int j = 0; j < TargetTokens.Count; j++)
				{
					if (_sim[i, j] == -1000.0)
					{
						num++;
					}
				}
			}
			return num;
		}

		public void Compute(bool computeDiagonalOnly)
		{
			for (int i = 0; i < SourceTokens.Count; i++)
			{
				for (int j = 0; j < TargetTokens.Count; j++)
				{
					if (_sim[i, j] != -1000.0)
					{
						continue;
					}
					if (computeDiagonalOnly)
					{
						if (i == j)
						{
							_sim[i, j] = SimilarityComputers.GetTokenSimilarity(SourceTokens[i], TargetTokens[j], _useStringEditDistance, _disabledAutoSubstitutions, _charactersNormalizeSafely, _applySmallChangeAdjustment);
						}
						else
						{
							_sim[i, j] = -1.0;
						}
					}
					else
					{
						_sim[i, j] = SimilarityComputers.GetTokenSimilarity(SourceTokens[i], TargetTokens[j], _useStringEditDistance, _disabledAutoSubstitutions, _charactersNormalizeSafely, _applySmallChangeAdjustment);
					}
				}
			}
		}
	}
}
