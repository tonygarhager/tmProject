using Sdl.Core.Globalization;
using Sdl.Core.LanguageProcessing.Resources;
using Sdl.Core.Processing.Alignment.Common;
using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.EditDistance;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.Lingua;
using Sdl.LanguagePlatform.TranslationMemoryTools;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.Core.Processing.Alignment.RetrofitApplier
{
	public class EdApplier
	{
		private bool _enableLogs;

		private const string DirLog = "C:\\\\temp\\\\RetrofitLogs";

		private const string SegmentFileLogName = "MarkupApplierLog.txt";

		private const string EdFileLogName = "ED.csv";

		private readonly CultureInfo _targetCulture;

		private readonly IDocumentItemFactory _itemFactory;

		private LanguageTools _targetTools;

		public int UpdatedSegmentErrorCount
		{
			get;
			set;
		}

		public int UpdatedSegmentsCount
		{
			get;
			set;
		}

		private LanguageTools TargetTools
		{
			get
			{
				if (_targetTools == null)
				{
					LanguageResources resources = new LanguageResources(_targetCulture, null);
					_targetTools = new LanguageTools(resources, BuiltinRecognizers.RecognizeNone);
				}
				return _targetTools;
			}
		}

		public ISegment UpdateBcm(ISegment originalBilingualSegment, ISegment updatedBilingualSegment, RetrofitUpdateSettings retrofitUpdateSettings)
		{
			if (retrofitUpdateSettings.SkipLockedSegment && originalBilingualSegment.Properties.IsLocked)
			{
				return originalBilingualSegment;
			}
			return ApplyMarkup(originalBilingualSegment, updatedBilingualSegment);
		}

		public EdApplier(CultureInfo targetCulture)
		{
			_targetCulture = targetCulture;
			_itemFactory = new DocumentItemFactory();
		}

		private Sdl.LanguagePlatform.Core.Segment GetLinguaSegmentFromBillingualSegment(ISegment victim)
		{
			LinguaTuBuilderSettings settings = new LinguaTuBuilderSettings
			{
				StripTags = false,
				ExcludeTagsInLockedContentText = true,
				IncludeTrackChanges = true,
				AlignTags = true,
				AcceptTrackChanges = true,
				IncludeComments = true
			};
			List<KeyValuePair<Sdl.LanguagePlatform.Core.Text, IAbstractMarkupData>> textAssociations;
			List<KeyValuePair<Tag, IAbstractMarkupData>> tagAssociations;
			Sdl.LanguagePlatform.Core.Segment segment = TUConverter.BuildLinguaSegment(_targetCulture, victim, settings, out textAssociations, out tagAssociations);
			TargetTools.EnsureTokenizedSegment(segment, forceRetokenization: false, allowTokenBundles: true);
			foreach (KeyValuePair<Tag, IAbstractMarkupData> item in tagAssociations)
			{
				item.Value.UniqueId = item.Key.Anchor;
			}
			return segment;
		}

		public ISegment ApplyMarkup(ISegment originalBilingualSegment, ISegment updatedBilingualSegment)
		{
			object obj = originalBilingualSegment.Clone();
			EdApplierContext edApplierContext = null;
			try
			{
				edApplierContext = InitializeEdContext(originalBilingualSegment, updatedBilingualSegment);
				if (edApplierContext.EditDistance.Distance.Equals(0.0))
				{
					EdApplierUtilities.LogSegment(_enableLogs, "=================", edApplierContext.UpdatedBilingualSegment, edApplierContext.OriginalBilingualSegment, (Sdl.FileTypeSupport.Framework.Bilingual.Segment)obj, "C:\\\\temp\\\\RetrofitLogs", "MarkupApplierLog.txt");
					return updatedBilingualSegment;
				}
				ApplyEditDistance(edApplierContext);
				EdApplierNormalizer.NormalizeFlatTokenSegment(edApplierContext);
				EdApplierUtilities.LogSegment(_enableLogs, "", edApplierContext.UpdatedBilingualSegment, edApplierContext.OriginalBilingualSegment, (Sdl.FileTypeSupport.Framework.Bilingual.Segment)obj, "C:\\\\temp\\\\RetrofitLogs", "MarkupApplierLog.txt");
			}
			catch (Exception ex)
			{
				EdApplierUtilities.LogSegment(_enableLogs, "error ... ", edApplierContext?.UpdatedBilingualSegment, edApplierContext?.OriginalBilingualSegment, (Sdl.FileTypeSupport.Framework.Bilingual.Segment)obj, "C:\\\\temp\\\\RetrofitLogs", "MarkupApplierLog.txt", ex.ToString());
				UpdatedSegmentErrorCount++;
				return (Sdl.FileTypeSupport.Framework.Bilingual.Segment)obj;
			}
			UpdateSegmentProperties(originalBilingualSegment);
			UpdatedSegmentsCount++;
			return edApplierContext.OriginalBilingualSegment;
		}

		private EdApplierContext InitializeEdContext(ISegment originalBilingualSegment, ISegment updatedBilingualSegment)
		{
			Sdl.LanguagePlatform.Core.Segment linguaSegmentFromBillingualSegment = GetLinguaSegmentFromBillingualSegment(originalBilingualSegment);
			Sdl.LanguagePlatform.Core.Segment linguaSegmentFromBillingualSegment2 = GetLinguaSegmentFromBillingualSegment(updatedBilingualSegment);
			SegmentEditDistanceComputer segmentEditDistanceComputer = new SegmentEditDistanceComputer();
			TagAssociations alignedTags;
			EditDistance editDistance = segmentEditDistanceComputer.ComputeEditDistance(linguaSegmentFromBillingualSegment.Tokens, linguaSegmentFromBillingualSegment2.Tokens, computeDiagonalOnly: false, BuiltinRecognizers.RecognizeNone, out alignedTags);
			if (editDistance.Distance.Equals(0.0))
			{
				return new EdApplierContext
				{
					EditDistance = editDistance
				};
			}
			EdApplierContext edApplierContext = new EdApplierContext
			{
				EditDistance = editDistance,
				AllEditDistanceItems = editDistance.Items,
				OriginalBilingualSegment = originalBilingualSegment,
				OriginalLinguaSegment = linguaSegmentFromBillingualSegment,
				UpdatedBilingualSegment = updatedBilingualSegment,
				UpdatedLinguaSegment = linguaSegmentFromBillingualSegment2,
				ItemFactory = _itemFactory,
				Map = new ModelMapper()
			};
			PrepareBcmTagPairMapper(edApplierContext);
			SetLinguaFriendlyOriginalBcm(originalBilingualSegment, linguaSegmentFromBillingualSegment);
			EdUtilities.PrepareEditDistance(edApplierContext, _enableLogs, "C:\\\\temp\\\\RetrofitLogs", "ED.csv");
			PrepareLinguaTagPairEdMapper(edApplierContext);
			return edApplierContext;
		}

		private static void ApplyEditDistance(EdApplierContext context)
		{
			context.EdIndex = context.EditDistance.Items.Count - 1;
			while (context.EdIndex >= 0)
			{
				switch (context.AllEditDistanceItems[context.EdIndex].Operation)
				{
				case EditOperation.Change:
					EdChangeApplier.ProcessChangeEditDistance(context);
					break;
				case EditOperation.Delete:
					EdDeleteApplier.ProcessDeleteEditDistance(context);
					break;
				case EditOperation.Insert:
					EdInsertApplier.ProcessInsertEditDistance(context);
					break;
				case EditOperation.Identity:
					context.EdIndex--;
					break;
				}
			}
		}

		private void UpdateSegmentProperties(ISegment segment)
		{
			segment.Properties.ConfirmationLevel = ConfirmationLevel.RejectedTranslation;
			if (segment.Properties.TranslationOrigin == null)
			{
				segment.Properties.TranslationOrigin = _itemFactory.CreateTranslationOrigin();
			}
			ITranslationOrigin translationOrigin = segment.Properties.TranslationOrigin;
			if (translationOrigin.OriginBeforeAdaptation == null)
			{
				translationOrigin.OriginBeforeAdaptation = _itemFactory.CreateTranslationOrigin();
			}
			translationOrigin.OriginBeforeAdaptation.OriginSystem = translationOrigin.OriginSystem;
			translationOrigin.OriginBeforeAdaptation.OriginType = translationOrigin.OriginType;
			translationOrigin.OriginBeforeAdaptation.OriginalTranslationHash = translationOrigin.OriginalTranslationHash;
			translationOrigin.OriginBeforeAdaptation.MatchPercent = translationOrigin.MatchPercent;
			translationOrigin.OriginBeforeAdaptation.IsStructureContextMatch = translationOrigin.IsStructureContextMatch;
			translationOrigin.OriginBeforeAdaptation.RepetitionTableId = translationOrigin.RepetitionTableId;
			translationOrigin.OriginBeforeAdaptation.TextContextMatchLevel = translationOrigin.TextContextMatchLevel;
			translationOrigin.OriginType = "Retrofit";
			translationOrigin.OriginSystem = "";
		}

		private static void SetLinguaFriendlyOriginalBcm(ISegment originalBcmSegment, Sdl.LanguagePlatform.Core.Segment linguaSegment)
		{
			ReplaceTagPairVisitor visitor = new ReplaceTagPairVisitor(linguaSegment);
			originalBcmSegment.AcceptVisitor(visitor);
		}

		private static void PrepareLinguaTagPairEdMapper(EdApplierContext context)
		{
			context.LinguaTagPairEDMapper = new Dictionary<int, int>();
			for (int i = 0; i < context.AllEditDistanceItems.Count; i++)
			{
				EditDistanceItem editDistanceItem = context.AllEditDistanceItems[i];
				TagToken tagToken;
				if (editDistanceItem.Target < context.UpdatedLinguaSegment.Tokens.Count && (tagToken = (context.UpdatedLinguaSegment.Tokens[editDistanceItem.Target] as TagToken)) != null && !context.LinguaTagPairEDMapper.ContainsKey(tagToken.Tag.Anchor))
				{
					context.LinguaTagPairEDMapper.Add(tagToken.Tag.Anchor, i);
				}
			}
		}

		private static void SetUpdateMappers(EdApplierContext context, IAbstractMarkupDataContainer container)
		{
			foreach (IAbstractMarkupData item in container)
			{
				ITagPair tagPair = item as ITagPair;
				if (tagPair == null)
				{
					ICommentMarker commentMarker = item as ICommentMarker;
					if (commentMarker == null)
					{
						IPlaceholderTag placeholderTag = item as IPlaceholderTag;
						if (placeholderTag == null)
						{
							IAbstractMarkupDataContainer abstractMarkupDataContainer = item as IAbstractMarkupDataContainer;
							if (abstractMarkupDataContainer != null)
							{
								SetUpdateMappers(context, abstractMarkupDataContainer);
							}
						}
						else
						{
							context.Map.UpdatePh.Add(item.UniqueId, placeholderTag);
						}
					}
					else
					{
						context.Map.UpdateComment.Add(item.UniqueId, commentMarker);
						SetUpdateMappers(context, commentMarker);
					}
				}
				else
				{
					context.Map.UpdateTag.Add(item.UniqueId, tagPair);
					SetUpdateMappers(context, tagPair);
				}
			}
		}

		private static void SetOriginalMappers(EdApplierContext context, IAbstractMarkupDataContainer container)
		{
			foreach (IAbstractMarkupData item in container)
			{
				ITagPair tagPair = item as ITagPair;
				if (tagPair == null)
				{
					ICommentMarker commentMarker = item as ICommentMarker;
					if (commentMarker == null)
					{
						IPlaceholderTag placeholderTag = item as IPlaceholderTag;
						if (placeholderTag == null)
						{
							IAbstractMarkupDataContainer abstractMarkupDataContainer = item as IAbstractMarkupDataContainer;
							if (abstractMarkupDataContainer != null)
							{
								SetOriginalMappers(context, abstractMarkupDataContainer);
							}
						}
						else
						{
							context.Map.OriginalPh.Add(item.UniqueId, placeholderTag);
						}
					}
					else
					{
						context.Map.OriginalComment.Add(item.UniqueId, commentMarker);
						SetOriginalMappers(context, commentMarker);
					}
				}
				else
				{
					context.Map.OriginalTag.Add(item.UniqueId, tagPair);
					SetOriginalMappers(context, tagPair);
				}
			}
		}

		private static void PrepareBcmTagPairMapper(EdApplierContext context)
		{
			context.Map.UpdateTag = new Dictionary<int, ITagPair>();
			context.Map.UpdatePh = new Dictionary<int, IPlaceholderTag>();
			context.Map.UpdateComment = new Dictionary<int, ICommentMarker>();
			context.Map.OriginalTag = new Dictionary<int, ITagPair>();
			context.Map.OriginalPh = new Dictionary<int, IPlaceholderTag>();
			context.Map.OriginalComment = new Dictionary<int, ICommentMarker>();
			SetUpdateMappers(context, context.UpdatedBilingualSegment);
			SetOriginalMappers(context, context.OriginalBilingualSegment);
		}
	}
}
