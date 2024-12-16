using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.LanguagePlatform.Core.EditDistance;
using Sdl.LanguagePlatform.Core.Tokenization;
using System.Collections.Generic;

namespace Sdl.Core.Processing.Alignment.RetrofitApplier
{
	internal class EdChangeApplier
	{
		public static void ProcessChangeEditDistance(EdApplierContext context)
		{
			EditDistanceItem editDistanceItem = context.AllEditDistanceItems[context.EdIndex];
			context.BilingualDeleteTokens = new List<IAbstractMarkupData>();
			context.LinguaInsertTokens = new List<Token>();
			while (context.EdIndex >= 0 && editDistanceItem.Operation == EditOperation.Change)
			{
				IAbstractMarkupData abstractMarkupData = context.OriginalBilingualSegment[editDistanceItem.Source];
				Token token = context.UpdatedLinguaSegment.Tokens[editDistanceItem.Target];
				BilingualTagPairToken bilingualTagPairToken = abstractMarkupData as BilingualTagPairToken;
				if (bilingualTagPairToken != null && token is TagToken)
				{
					if (LeftLongerEqualThanRightTagStart(context, bilingualTagPairToken, editDistanceItem.Target))
					{
						context.BilingualDeleteTokens.Clear();
						context.LinguaInsertTokens.Clear();
						context.EdRightIndex = int.MaxValue;
						EdDeleteApplier.HandleEdLeftTagPair(context);
					}
					else
					{
						context.BilingualDeleteTokens.Clear();
						context.LinguaInsertTokens.Clear();
						context.EdLeftIndex = int.MaxValue;
						EdInsertApplier.HandleEdRightTagPair(context);
					}
					break;
				}
				PlaceholderTag placeholderTag = abstractMarkupData as PlaceholderTag;
				if (placeholderTag != null)
				{
					TagToken tagToken = token as TagToken;
					if (tagToken != null && ArePhsEqual(context, placeholderTag, tagToken))
					{
						editDistanceItem = AdvanceEd(context);
						continue;
					}
				}
				context.BilingualDeleteTokens.Insert(0, context.OriginalBilingualSegment[editDistanceItem.Source]);
				context.LinguaInsertTokens.Insert(0, token);
				editDistanceItem = AdvanceEd(context);
			}
			DoTextReplacement(context);
		}

		private static bool LeftLongerEqualThanRightTagStart(EdApplierContext context, BilingualTagPairToken bilingualDeleteToken, int linguaIndex)
		{
			int indexInParent = bilingualDeleteToken.IndexInParent;
			int num = -1;
			int num2 = indexInParent - 1;
			while (num2 > 0 && num == -1)
			{
				BilingualTagPairToken bilingualTagPairToken = context.OriginalBilingualSegment[num2] as BilingualTagPairToken;
				if (bilingualTagPairToken != null && bilingualTagPairToken.Anchor == bilingualDeleteToken.Anchor)
				{
					num = num2;
				}
				num2--;
			}
			int num3 = num;
			num = -1;
			num2 = linguaIndex - 1;
			TagToken tagToken = context.UpdatedLinguaSegment.Tokens[linguaIndex] as TagToken;
			while (num2 > 0 && num == -1)
			{
				TagToken tagToken2 = context.UpdatedLinguaSegment.Tokens[num2] as TagToken;
				if (tagToken2 != null && tagToken != null && tagToken2.Tag.Anchor == tagToken.Tag.Anchor)
				{
					num = num2;
				}
				num2--;
			}
			int num4 = num;
			return num3 >= num4;
		}

		private static bool AreTagsEqual(EdApplierContext context, BilingualTagPairToken originalTagPairToken, TagToken updateLinguaToken)
		{
			ITagPair source = context.Map.OriginalTag[originalTagPairToken.UniqueId];
			ITagPair target = context.Map.UpdateTag[updateLinguaToken.Tag.Anchor];
			return EdUtilities.AreTagsEqual(source, target);
		}

		private static EditDistanceItem AdvanceEd(EdApplierContext context)
		{
			context.EdIndex--;
			if (context.EdIndex < 0)
			{
				return default(EditDistanceItem);
			}
			return context.AllEditDistanceItems[context.EdIndex];
		}

		private static bool ArePhsEqual(EdApplierContext context, IPlaceholderTag originalPh, TagToken updateLinguaToken)
		{
			return EdUtilities.ArePhsEqual(originalPh, context.Map.UpdatePh[updateLinguaToken.Tag.Anchor]);
		}

		private static void DoTextReplacement(EdApplierContext context)
		{
			if (context.BilingualDeleteTokens.Count != 0)
			{
				IAbstractMarkupData abstractMarkupData = context.BilingualDeleteTokens[0];
				int indexInParent = abstractMarkupData.IndexInParent;
				IAbstractMarkupDataContainer parent = abstractMarkupData.Parent;
				IRevisionMarker revisionMarker = context.ItemFactory.CreateRevision(context.ItemFactory.PropertiesFactory.CreateRevisionProperties(RevisionType.Delete));
				while (context.BilingualDeleteTokens.Count > 0)
				{
					context.BilingualDeleteTokens[0].RemoveFromParent();
					revisionMarker.Add(context.BilingualDeleteTokens[0]);
					context.BilingualDeleteTokens.RemoveAt(0);
				}
				parent.Insert(indexInParent, revisionMarker);
				revisionMarker = context.ItemFactory.CreateRevision(context.ItemFactory.PropertiesFactory.CreateRevisionProperties(RevisionType.Insert));
				while (context.LinguaInsertTokens.Count > 0)
				{
					revisionMarker.Add(EdApplierUtilities.CreateBilingualToken(context, context.LinguaInsertTokens[0]));
					context.LinguaInsertTokens.RemoveAt(0);
				}
				parent.Insert(indexInParent + 1, revisionMarker);
			}
		}
	}
}
