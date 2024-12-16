using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.LanguagePlatform.Core.EditDistance;
using Sdl.LanguagePlatform.Core.Tokenization;
using System;

namespace Sdl.Core.Processing.Alignment.RetrofitApplier
{
	public class EdInsertApplier
	{
		public static void ProcessInsertEditDistance(EdApplierContext context)
		{
			EditDistanceItem editDistanceItem = context.AllEditDistanceItems[context.EdIndex];
			Token token = context.UpdatedLinguaSegment.Tokens[editDistanceItem.Target];
			int source = editDistanceItem.Source;
			ISegment originalBilingualSegment = context.OriginalBilingualSegment;
			if (EdApplierUtilities.IsTokenTagPair(token))
			{
				context.EdLeftIndex = int.MaxValue;
				HandleEdRightTagPair(context);
				return;
			}
			GetBilingualTokenForLinguaToken(context, token, out IAbstractMarkupData bilingualInsertToken);
			IRevisionMarker revisionMarker = context.ItemFactory.CreateRevision(context.ItemFactory.PropertiesFactory.CreateRevisionProperties(RevisionType.Insert));
			revisionMarker.Add(bilingualInsertToken);
			originalBilingualSegment.Insert(source, revisionMarker);
			context.EdIndex--;
		}

		private static void GetBilingualTokenForLinguaToken(EdApplierContext context, Token linguaInsertToken, out IAbstractMarkupData bilingualInsertToken)
		{
			if (EdApplierUtilities.IsTokenPlaceholder(linguaInsertToken, out TagToken tagToken))
			{
				if (!context.Map.UpdatePh.ContainsKey(tagToken.Tag.Anchor))
				{
					throw new Exception("lingua Inserted ph should have a biligual equivalent ph, check the flow");
				}
				bilingualInsertToken = (IPlaceholderTag)context.Map.UpdatePh[tagToken.Tag.Anchor].Clone();
			}
			else if (EdApplierUtilities.IsTokenTagPair(linguaInsertToken))
			{
				if (!context.Map.UpdateTag.ContainsKey(tagToken.Tag.Anchor))
				{
					throw new Exception("lingua Inserted tag should have a biligual equivalent tag, check the flow");
				}
				bilingualInsertToken = (ITagPair)context.Map.UpdateTag[tagToken.Tag.Anchor].Clone();
			}
			else
			{
				bilingualInsertToken = context.ItemFactory.CreateText(context.ItemFactory.PropertiesFactory.CreateTextProperties(linguaInsertToken.Text));
			}
		}

		public static void HandleEdRightTagPair(EdApplierContext context)
		{
			EditDistanceItem editDistanceItem = context.AllEditDistanceItems[context.EdIndex];
			int num = Math.Min(editDistanceItem.Source, context.EdLeftIndex);
			ISegment originalBilingualSegment = context.OriginalBilingualSegment;
			IRevisionMarker revisionMarker = context.ItemFactory.CreateRevision(context.ItemFactory.PropertiesFactory.CreateRevisionProperties(RevisionType.Insert));
			IRevisionMarker revisionMarker2 = context.ItemFactory.CreateRevision(context.ItemFactory.PropertiesFactory.CreateRevisionProperties(RevisionType.Delete));
			originalBilingualSegment.Insert(num, revisionMarker);
			originalBilingualSegment.Insert(num, revisionMarker2);
			int num2 = -1;
			int num3 = -1;
			Token token = context.UpdatedLinguaSegment.Tokens[editDistanceItem.Target];
			GetBilingualTokenForLinguaToken(context, token, out IAbstractMarkupData bilingualInsertToken);
			revisionMarker.Insert(0, bilingualInsertToken);
			TagToken tagToken = token as TagToken;
			int anchor = tagToken.Tag.Anchor;
			while (num3 != anchor)
			{
				context.EdIndex--;
				editDistanceItem = context.AllEditDistanceItems[context.EdIndex];
				token = context.UpdatedLinguaSegment.Tokens[editDistanceItem.Target];
				tagToken = (token as TagToken);
				if (tagToken != null)
				{
					num3 = tagToken.Tag.Anchor;
					num2 = editDistanceItem.Source;
				}
			}
			for (int num4 = num - 1; num4 >= num2; num4--)
			{
				IAbstractMarkupData abstractMarkupData = context.OriginalBilingualSegment[num4];
				BilingualTagPairToken bilingualTagPairToken = abstractMarkupData as BilingualTagPairToken;
				if (bilingualTagPairToken != null && !bilingualTagPairToken.IsStart && bilingualTagPairToken.StartToken.IndexInParent < num2)
				{
					context.EdRightIndex = editDistanceItem.Target;
					context.EdIndex = ConvertLeftIndexToEdIndex(num4, context);
					EdDeleteApplier.HandleEdLeftTagPair(context);
					return;
				}
				abstractMarkupData.RemoveFromParent();
				revisionMarker2.Insert(0, abstractMarkupData);
			}
			if (revisionMarker2.Count == 0)
			{
				revisionMarker2.RemoveFromParent();
			}
			context.EdIndex--;
		}

		private static int ConvertLeftIndexToEdIndex(int i, EdApplierContext context)
		{
			for (int j = 0; j < context.AllEditDistanceItems.Count; j++)
			{
				if (context.AllEditDistanceItems[j].Source == i)
				{
					return j;
				}
			}
			return 0;
		}
	}
}
