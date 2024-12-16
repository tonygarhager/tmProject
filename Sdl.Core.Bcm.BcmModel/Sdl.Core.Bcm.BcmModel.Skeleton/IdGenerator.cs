using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.Bcm.BcmModel.Skeleton
{
	public class IdGenerator
	{
		private readonly ConcurrentDictionary<Type, int> _id = new ConcurrentDictionary<Type, int>();

		public int GetNext<T>() where T : SkeletonItem
		{
			return _id.AddOrUpdate(typeof(T), 1, (Type type, int i) => i + 1);
		}

		public void Update<T>(T item) where T : SkeletonItem
		{
			if (item != null)
			{
				_id.AddOrUpdate(typeof(T), item.Id, (Type type, int i) => Math.Max(i, item.Id));
			}
		}

		public void UpdateFrom(FileSkeleton sk)
		{
			Update(sk.CommentDefinitions.LastOrDefault());
			Update(sk.ContextDefinitions.LastOrDefault());
			Update(sk.Contexts.LastOrDefault());
			Update(sk.TerminologyData.LastOrDefault());
			Update(sk.FormattingGroups.LastOrDefault());
			Update(sk.StructureTagDefinitions.LastOrDefault());
			Update(sk.PlaceholderTagDefinitions.LastOrDefault());
			Update(sk.TagPairDefinitions.LastOrDefault());
		}

		public void UpdateFrom(Document document)
		{
			Update(document, (FileSkeleton skeleton) => skeleton.CommentDefinitions);
			Update(document, (FileSkeleton skeleton) => skeleton.ContextDefinitions);
			Update(document, (FileSkeleton skeleton) => skeleton.Contexts);
			Update(document, (FileSkeleton skeleton) => skeleton.TerminologyData);
			Update(document, (FileSkeleton skeleton) => skeleton.FormattingGroups);
			Update(document, (FileSkeleton skeleton) => skeleton.StructureTagDefinitions);
			Update(document, (FileSkeleton skeleton) => skeleton.PlaceholderTagDefinitions);
			Update(document, (FileSkeleton skeleton) => skeleton.TagPairDefinitions);
		}

		private void Update<T>(Document document, Func<FileSkeleton, SkeletonCollection<T>> collectionProvider) where T : SkeletonItem
		{
			List<File> source = document.Files.Where((File f) => collectionProvider(f.Skeleton)?.Any() ?? false).ToList();
			if (source.Any())
			{
				int max = (from x in source.SelectMany((File f) => collectionProvider(f.Skeleton))
					select x.Id).Max();
				_id.AddOrUpdate(typeof(T), max, (Type type, int i) => Math.Max(i, max));
			}
		}
	}
}
