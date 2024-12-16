using Sdl.Core.FineGrainedAlignment;
using Sdl.Core.FineGrainedAlignment.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.Lingua;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryImpl.Storage;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl.FGA
{
	internal class StorageBasedAlignableCorpus : IAlignableCorpus
	{
		private readonly CallContext _context;

		private readonly AnnotatedTranslationMemory _tm;

		private readonly StorageBasedAlignableCorpusId _id;

		private IAlignableStorage AlignableStorage
		{
			get;
		}

		[Obsolete("replaced by UnalignedCount")]
		public int UnalignedContentPairCount => AlignableStorage.GetUnalignedCount(_id.TmId.Id, null);

		public int PairCount => AlignableStorage.GetPairCount(_id.TmId.Id);

		public AlignableCorpusId Id => _id;

		public CultureInfo SourceCulture => _tm.Tm.LanguageDirection.SourceCulture;

		public CultureInfo TargetCulture => _tm.Tm.LanguageDirection.TargetCulture;

		public StorageBasedAlignableCorpus(IStorage storage, AnnotatedTranslationMemory tm, CallContext context)
		{
			AlignableStorage = (storage as IAlignableStorage);
			if (AlignableStorage == null)
			{
				throw new Exception("Storage implementation does not implement IAlignableStorage: " + storage.GetType().Name);
			}
			_context = context;
			_tm = tm;
			_id = new StorageBasedAlignableCorpusId(_tm.Tm.ResourceId);
		}

		public AlignerDefinition GetAlignerDefinition()
		{
			return AlignableStorage.GetAlignerDefinition(_id.TmId.Id);
		}

		private AlignableTu AlignableTuFromPair(IAlignableContentPair pair)
		{
			return (pair as AlignableTu) ?? throw new ArgumentException("pair");
		}

		public int GetPostdatedContentPairCount(DateTime modelDate)
		{
			return AlignableStorage.GetPostdatedTranslationUnitCount(_id.TmId.Id, modelDate);
		}

		public int GetAlignedPredatedContentPairCount(DateTime modelDate)
		{
			return AlignableStorage.GetAlignedPredatedTranslationUnitCount(_id.TmId.Id, modelDate);
		}

		[Obsolete("replaced by UnalignedCount")]
		public int UnalignedUnscheduledContentPairCount(int scheduleDelta, DateTime modelDate)
		{
			throw new NotImplementedException();
		}

		public int UnalignedCount(DateTime? modelDate)
		{
			return AlignableStorage.GetUnalignedCount(_id.TmId.Id, modelDate);
		}

		public IEnumerable<IAlignableContentPair> Pairs()
		{
			RegularIterator iter = new RegularIterator();
			while (true)
			{
				IList<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit> tusForAlignment = _context.ResourceManager.GetTusForAlignment(_tm.Tm.ResourceId, iter, unalignedOnly: false, postdated: false);
				if (tusForAlignment != null && tusForAlignment.Count != 0)
				{
					foreach (Sdl.LanguagePlatform.TranslationMemory.TranslationUnit item in tusForAlignment)
					{
						yield return PairFromTu(item);
					}
					continue;
				}
				break;
			}
		}

		internal static void ExtractIndexData(CultureInfo sourceCulture, CultureInfo targetCulture, List<Token> sourceTokens, List<Token> targetTokens, LiftAlignedSpanPairSet alignmentData, HashSet<long> hashes, List<byte> lengths, List<byte> sigwordCounts)
		{
			if (alignmentData == null || alignmentData.IsEmpty)
			{
				return;
			}
			List<short> list = new List<short>();
			List<string> features = SubsegmentUtilities.GetFeatures(sourceTokens, sourceCulture, list);
			List<short> featureIndicesOnly = SubsegmentUtilities.GetFeatureIndicesOnly(targetTokens, targetCulture);
			SubsegmentUtilities.RawIndexVsSignificantIndexConverter srcToConverter = new SubsegmentUtilities.RawIndexVsSignificantIndexConverter(list);
			SubsegmentUtilities.RawIndexVsSignificantIndexConverter trgToConverter = new SubsegmentUtilities.RawIndexVsSignificantIndexConverter(featureIndicesOnly);
			SubsegmentUtilities.RawIndexVsSignificantIndexConverter srcFromConverter = SubsegmentUtilities.RawIndexVsSignificantIndexConverter.CreateNoopConverter((short)sourceTokens.Count);
			SubsegmentUtilities.RawIndexVsSignificantIndexConverter trgFromConverter = SubsegmentUtilities.RawIndexVsSignificantIndexConverter.CreateNoopConverter((short)targetTokens.Count);
			LiftAlignedSpanPairSet spanPairSet = SubsegmentUtilities.ConvertAlignmentSetIndices(alignmentData, srcFromConverter, trgFromConverter, srcToConverter, trgToConverter);
			short minLength = SubsegmentUtilities.MinDTAFragmentLengthToIndex(sourceCulture);
			short minSigWords = SubsegmentUtilities.MinDTAFragmentSignificantFeatures(sourceCulture);
			List<int> list2 = new List<int>();
			List<string> list3 = new List<string>();
			List<int> list4 = new List<int>();
			short maxLength = Math.Min((short)255, SubsegmentUtilities.MaxFragmentLengthToIndex((short)features.Count));
			List<LiftSpan> translatableSpans = SubsegmentUtilities.GetTranslatableSpans(spanPairSet, features, list, sourceTokens, minLength, minSigWords, maxLength, list2, list4, list3);
			HashSet<string> hashSet = new HashSet<string>();
			for (int i = 0; i < translatableSpans.Count; i++)
			{
				string s = list3[i];
				byte item = (byte)list2[i];
				byte item2 = (byte)list4[i];
				long hashCodeLong = Hash.GetHashCodeLong(s);
				string item3 = hashCodeLong.ToString() + "|" + item.ToString() + "|" + item2.ToString();
				if (!hashSet.Contains(item3))
				{
					hashSet.Add(item3);
					hashes.Add(hashCodeLong);
					lengths.Add(item);
					sigwordCounts.Add(item2);
				}
			}
		}

		private AlignableTu PairFromTu(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit tu)
		{
			if (tu.SourceSegment.Tokens != null)
			{
				_ = tu.TargetSegment.Tokens;
			}
			_tm.SourceTools.EnsureTokenizedSegment(tu.SourceSegment);
			_tm.TargetTools.EnsureTokenizedSegment(tu.TargetSegment);
			return new AlignableTu(tu);
		}
	}
}
