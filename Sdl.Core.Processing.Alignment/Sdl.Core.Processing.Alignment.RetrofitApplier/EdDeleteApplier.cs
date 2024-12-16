using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.EditDistance;
using Sdl.LanguagePlatform.Core.Tokenization;
using System;

namespace Sdl.Core.Processing.Alignment.RetrofitApplier
{
	public class EdDeleteApplier
	{
		public static void ProcessDeleteEditDistance(EdApplierContext context)
		{
			EditDistanceItem editDistanceItem = context.AllEditDistanceItems[context.EdIndex];
			IAbstractMarkupData abstractMarkupData = context.OriginalBilingualSegment[editDistanceItem.Source];
			int indexInParent = abstractMarkupData.IndexInParent;
			IAbstractMarkupDataContainer parent = abstractMarkupData.Parent;
			if (abstractMarkupData is BilingualTagPairToken)
			{
				context.EdRightIndex = int.MaxValue;
				HandleEdLeftTagPair(context);
				return;
			}
			abstractMarkupData.RemoveFromParent();
			IRevisionMarker revisionMarker = context.ItemFactory.CreateRevision(context.ItemFactory.PropertiesFactory.CreateRevisionProperties(RevisionType.Delete));
			revisionMarker.Add(abstractMarkupData);
			parent.Insert(indexInParent, revisionMarker);
			context.EdIndex--;
		}

		public static void HandleEdLeftTagPair(EdApplierContext context)
		{
			EditDistanceItem editDistanceItem = context.AllEditDistanceItems[context.EdIndex];
			int num = Math.Min(editDistanceItem.Target, context.EdRightIndex);
			IRevisionMarker revisionMarker = context.ItemFactory.CreateRevision(context.ItemFactory.PropertiesFactory.CreateRevisionProperties(RevisionType.Insert));
			IRevisionMarker revisionMarker2 = context.ItemFactory.CreateRevision(context.ItemFactory.PropertiesFactory.CreateRevisionProperties(RevisionType.Delete));
			IAbstractMarkupData abstractMarkupData = context.OriginalBilingualSegment[editDistanceItem.Source];
			IAbstractMarkupDataContainer parent = abstractMarkupData.Parent;
			parent.Insert(editDistanceItem.Source + 1, revisionMarker2);
			parent.Insert(editDistanceItem.Source + 1, revisionMarker);
			int num2 = -1;
			int num3 = -1;
			abstractMarkupData.RemoveFromParent();
			revisionMarker2.Insert(0, abstractMarkupData);
			BilingualTagPairToken bilingualTagPairToken = abstractMarkupData as BilingualTagPairToken;
			int anchor = bilingualTagPairToken.Anchor;
			int num4 = -1;
			while (num4 != anchor)
			{
				context.EdIndex--;
				editDistanceItem = context.AllEditDistanceItems[context.EdIndex];
				if (editDistanceItem.Operation != EditOperation.Insert)
				{
					abstractMarkupData = context.OriginalBilingualSegment[editDistanceItem.Source];
					abstractMarkupData.RemoveFromParent();
					revisionMarker2.Insert(0, abstractMarkupData);
					bilingualTagPairToken = (abstractMarkupData as BilingualTagPairToken);
					if (bilingualTagPairToken != null)
					{
						num4 = bilingualTagPairToken.Anchor;
						num2 = editDistanceItem.Target;
						num3 = editDistanceItem.Source;
					}
				}
			}
			int num5 = num - 1;
			if (num > 0 && num < context.UpdatedLinguaSegment.Tokens.Count && context.UpdatedLinguaSegment.Tokens[num] is TagToken)
			{
				num5 = num;
			}
			while (num5 >= num2)
			{
				Token token = context.UpdatedLinguaSegment.Tokens[num5];
				TagToken tagToken = token as TagToken;
				if (tagToken != null && tagToken.Tag.Type == TagType.End && context.LinguaTagPairEDMapper[tagToken.Tag.Anchor] < num3)
				{
					context.EdLeftIndex = editDistanceItem.Source;
					context.EdIndex = ConvertRightIndexToEdIndex(num5, context);
					EdInsertApplier.HandleEdRightTagPair(context);
					return;
				}
				revisionMarker.Insert(0, EdApplierUtilities.CreateBilingualToken(context, token));
				num5--;
			}
			if (revisionMarker.Count == 0)
			{
				revisionMarker.RemoveFromParent();
			}
			context.EdIndex--;
		}

		private static int ConvertRightIndexToEdIndex(int i, EdApplierContext context)
		{
			for (int j = 0; j < context.AllEditDistanceItems.Count; j++)
			{
				if (context.AllEditDistanceItems[j].Target == i)
				{
					return j;
				}
			}
			return 0;
		}
	}
}
