using Sdl.Core.FineGrainedAlignment.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Collections.Generic;

namespace Sdl.Core.FineGrainedAlignment
{
	internal class AlignableContentPairWrapper : IAlignableContentPair
	{
		private readonly IAlignableContentPair _pair;

		public AlignableContentPairId Id => _pair.Id;

		public LiftAlignedSpanPairSet AlignmentData
		{
			get
			{
				return _pair.AlignmentData;
			}
			set
			{
				_pair.AlignmentData = value;
			}
		}

		public DateTime? TranslationModelDate
		{
			get
			{
				return _pair.TranslationModelDate;
			}
			set
			{
				_pair.TranslationModelDate = value;
			}
		}

		public List<Token> SourceTokens
		{
			get;
			set;
		}

		public List<Token> TargetTokens
		{
			get;
			set;
		}

		public AlignableContentPairWrapper(IAlignableContentPair pair)
		{
			SourceTokens = DuplicateList(pair.SourceTokens);
			TargetTokens = DuplicateList(pair.TargetTokens);
			_pair = pair;
		}

		private List<Token> DuplicateList(List<Token> list)
		{
			if (list == null)
			{
				return null;
			}
			List<Token> list2 = new List<Token>();
			foreach (Token item in list)
			{
				Token token = item;
				try
				{
					if (token is SimpleToken)
					{
						SimpleToken simpleToken = item.Duplicate() as SimpleToken;
						if (simpleToken != null)
						{
							simpleToken.Stem = null;
							token = simpleToken;
						}
					}
				}
				catch (Exception)
				{
				}
				list2.Add(token);
			}
			return list2;
		}

		public static List<IAlignableContentPair> WrapList(IEnumerable<IAlignableContentPair> list)
		{
			if (list == null)
			{
				return null;
			}
			List<IAlignableContentPair> list2 = new List<IAlignableContentPair>();
			foreach (IAlignableContentPair item in list)
			{
				list2.Add(new AlignableContentPairWrapper(item));
			}
			return list2;
		}

		public static IEnumerable<IAlignableContentPair> WrapEnumerable(IEnumerable<IAlignableContentPair> pairs)
		{
			if (pairs != null)
			{
				foreach (IAlignableContentPair p in pairs)
				{
					if (p == null)
					{
						yield return null;
					}
					yield return new AlignableContentPairWrapper(p);
				}
			}
		}

		public static IAlignableContentPair GetWrapper(IAlignableContentPair pair)
		{
			if (pair == null)
			{
				return null;
			}
			return new AlignableContentPairWrapper(pair);
		}
	}
}
