using System;

namespace Sdl.LanguagePlatform.Stat
{
	public class SpecialTokenIDs
	{
		public enum SpecialTokenType
		{
			BeginningOfSegment = 0,
			EndOfSegment = 1,
			Date = 2,
			Measurement = 3,
			Number = 4,
			Time = 5,
			Punctuation = 6,
			Variable = 7,
			Unknown = 9999
		}

		public int BOS;

		public int EOS;

		public int DAT;

		public int MSR;

		public int NUM;

		public int TIM;

		public int PCT;

		public int VAR;

		public SpecialTokenIDs()
		{
			Reset();
		}

		public SpecialTokenIDs(SpecialTokenIDs other)
		{
			if (other == null)
			{
				throw new ArgumentNullException();
			}
			BOS = other.BOS;
			EOS = other.EOS;
			DAT = other.DAT;
			MSR = other.MSR;
			NUM = other.NUM;
			TIM = other.TIM;
			PCT = other.PCT;
			VAR = other.VAR;
		}

		public bool HasAllSpecialIDs()
		{
			if (BOS >= 0 && EOS >= 0 && DAT >= 0 && MSR >= 0 && NUM >= 0 && TIM >= 0 && PCT >= 0)
			{
				return VAR >= 0;
			}
			return false;
		}

		public void Reset()
		{
			BOS = -1;
			EOS = -1;
			DAT = -1;
			MSR = -1;
			NUM = -1;
			TIM = -1;
			PCT = -1;
			VAR = -1;
		}

		public bool IsSpecial(int w)
		{
			if (w < 0)
			{
				return false;
			}
			if (w != BOS && w != EOS && w != DAT && w != MSR && w != NUM && w != TIM && w != PCT)
			{
				return w == VAR;
			}
			return true;
		}

		public SpecialTokenType GetSpecialTokenType(int w)
		{
			if (w >= 0)
			{
				if (w != BOS)
				{
					if (w != EOS)
					{
						if (w != DAT)
						{
							if (w != MSR)
							{
								if (w != NUM)
								{
									if (w != TIM)
									{
										if (w != PCT)
										{
											if (w != VAR)
											{
												return SpecialTokenType.Unknown;
											}
											return SpecialTokenType.Variable;
										}
										return SpecialTokenType.Punctuation;
									}
									return SpecialTokenType.Time;
								}
								return SpecialTokenType.Number;
							}
							return SpecialTokenType.Measurement;
						}
						return SpecialTokenType.Date;
					}
					return SpecialTokenType.EndOfSegment;
				}
				return SpecialTokenType.BeginningOfSegment;
			}
			return SpecialTokenType.Unknown;
		}
	}
}
