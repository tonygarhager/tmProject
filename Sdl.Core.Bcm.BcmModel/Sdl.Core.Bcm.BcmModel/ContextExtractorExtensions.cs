using Sdl.Core.Bcm.BcmModel.Skeleton;
using System.Linq;

namespace Sdl.Core.Bcm.BcmModel
{
	internal static class ContextExtractorExtensions
	{
		public static void ExtractContext(File file, ParagraphUnit paragraphUnit, FileSkeleton newSkeleton)
		{
			FileSkeleton skeleton = file.Skeleton;
			if (skeleton != null)
			{
				ExtractContextSkeleton(skeleton, paragraphUnit.StructureContextId, newSkeleton);
				if (paragraphUnit.ContextList != null)
				{
					foreach (int context in paragraphUnit.ContextList)
					{
						ExtractContextSkeleton(skeleton, context, newSkeleton);
					}
				}
			}
		}

		private static void ExtractContextSkeleton(FileSkeleton oldSkeleton, int contextId, FileSkeleton newSkeleton)
		{
			Context oldContext;
			for (oldContext = oldSkeleton.Contexts.SingleOrDefault((Context cd) => cd.Id == contextId); oldContext != null; oldContext = oldSkeleton.Contexts.SingleOrDefault((Context cd) => cd.Id == oldContext.ParentContextId))
			{
				if (newSkeleton.Contexts.Any((Context c) => c.Id == contextId))
				{
					break;
				}
				ContextDefinition oldContextDefinition = oldSkeleton.ContextDefinitions.FirstOrDefault((ContextDefinition cd) => cd.Id == oldContext.ContextDefinitionId);
				if (oldContextDefinition == null)
				{
					break;
				}
				newSkeleton.Contexts.GetOrAddWithExistingId(oldContext.Clone());
				if (newSkeleton.ContextDefinitions.All((ContextDefinition cd) => cd.Id != oldContextDefinition.Id))
				{
					newSkeleton.ContextDefinitions.GetOrAddWithExistingId(oldContextDefinition.Clone());
				}
				FormattingGroup oldFormattingGroup = oldSkeleton.FormattingGroups.SingleOrDefault((FormattingGroup fg) => fg.Id == oldContextDefinition.FormattingGroupId);
				if (oldFormattingGroup != null && newSkeleton.FormattingGroups.All((FormattingGroup fg) => fg.Id != oldFormattingGroup.Id))
				{
					newSkeleton.FormattingGroups.GetOrAddWithExistingId(oldFormattingGroup.Clone());
				}
			}
		}
	}
}
