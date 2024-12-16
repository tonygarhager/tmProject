using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.Processing.Alignment.RetrofitApplier
{
	internal class EdApplierNormalizer
	{
		public static void NormalizeFlatTokenSegment(EdApplierContext context)
		{
			NormalizeRevisionContainerOrder(context.OriginalBilingualSegment);
			NormalizeRevisionContainersMerge(context.OriginalBilingualSegment);
			NormalizeTagContainersBcm(context);
		}

		private static void NormalizeTagContainersBcm(EdApplierContext context)
		{
			List<BilingualTagPairToken> firstLevelFakeTagEnders = GetFirstLevelFakeTagEnders(context.OriginalBilingualSegment);
			for (int num = firstLevelFakeTagEnders.Count - 1; num >= 0; num--)
			{
				BilingualTagPairToken bilingualTagPairToken = firstLevelFakeTagEnders[num];
				IAbstractMarkupDataContainer abstractMarkupDataContainer = EdApplierUtilities.CreateTagPairFromFakeToken(context, bilingualTagPairToken);
				IAbstractMarkupDataContainer parent = bilingualTagPairToken.Parent;
				parent.Insert(bilingualTagPairToken.IndexInParent + 1, abstractMarkupDataContainer as IAbstractMarkupData);
				HandleFakeTag(bilingualTagPairToken, abstractMarkupDataContainer);
			}
		}

		private static List<BilingualTagPairToken> GetFirstLevelFakeTagEnders(ISegment container)
		{
			List<BilingualTagPairToken> list = new List<BilingualTagPairToken>();
			foreach (IAbstractMarkupData allSubItem in container.AllSubItems)
			{
				BilingualTagPairToken bilingualTagPairToken = allSubItem as BilingualTagPairToken;
				if (bilingualTagPairToken != null && !bilingualTagPairToken.IsStart)
				{
					list.Add(bilingualTagPairToken);
				}
			}
			return list;
		}

		private static void HandleFakeTag(BilingualTagPairToken tagEndToken, IAbstractMarkupDataContainer tp)
		{
			int num = -1;
			IAbstractMarkupDataContainer parent = tagEndToken.Parent;
			int indexInParent = tagEndToken.IndexInParent;
			int anchor = tagEndToken.Anchor;
			parent[indexInParent].RemoveFromParent();
			indexInParent--;
			BilingualTagPairToken bilingualTagPairToken = parent[indexInParent] as BilingualTagPairToken;
			if (bilingualTagPairToken != null)
			{
				num = bilingualTagPairToken.Anchor;
			}
			while (num != anchor)
			{
				IAbstractMarkupData abstractMarkupData = parent[indexInParent];
				abstractMarkupData.RemoveFromParent();
				indexInParent--;
				tp.Insert(0, abstractMarkupData);
				num = ((parent[indexInParent] as BilingualTagPairToken)?.Anchor ?? (-1));
			}
			parent[indexInParent].RemoveFromParent();
		}

		private static void NormalizeRevisionContainersMerge(ISegment segment)
		{
			for (int i = 0; i < segment.Count(); i++)
			{
				if (segment[i] is RevisionMarker)
				{
					NormalizeSingleRevision(segment[i] as RevisionMarker);
				}
			}
		}

		private static void NormalizeRevisionContainerOrder(ISegment segment)
		{
			for (int i = 0; i < segment.Count(); i++)
			{
				RevisionMarker revisionMarker = segment[i] as RevisionMarker;
				if (revisionMarker != null && revisionMarker.Properties.RevisionType == RevisionType.Delete)
				{
					NormalizeDeleteRevisionOrder(segment, revisionMarker);
				}
			}
		}

		private static void NormalizeSingleRevision(RevisionMarker revision)
		{
			if (revision.Count != 0)
			{
				IAbstractMarkupDataContainer parent = revision.Parent;
				int indexInParent = revision.IndexInParent;
				bool flag = true;
				RevisionMarker nextRevFirst = default(RevisionMarker);
				while ((indexInParent + 1 < parent.Count && (nextRevFirst = (parent[indexInParent + 1] as RevisionMarker)) != null) & flag)
				{
					flag = NormalizeNextFirst(nextRevFirst, revision);
				}
			}
		}

		private static bool NormalizeNextFirst(RevisionMarker nextRevFirst, RevisionMarker revision)
		{
			if (nextRevFirst.Properties.RevisionType != revision.Properties.RevisionType)
			{
				return false;
			}
			EdApplierUtilities.MoveAllcontainerContent(nextRevFirst, revision);
			nextRevFirst.RemoveFromParent();
			return true;
		}

		private static void NormalizeDeleteRevisionOrder(ISegment segment, RevisionMarker revision)
		{
			if (revision.Properties.RevisionType == RevisionType.Delete)
			{
				int indexInParent = revision.IndexInParent;
				RevisionMarker revisionMarker;
				while (indexInParent - 1 >= 0 && (revisionMarker = (segment[indexInParent - 1] as RevisionMarker)) != null && revisionMarker.Properties.RevisionType == RevisionType.Insert)
				{
					revision.RemoveFromParent();
					segment.Insert(revisionMarker.IndexInParent, revision);
					indexInParent = revision.IndexInParent;
				}
			}
		}
	}
}
