using Sdl.FileTypeSupport.Framework;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Sdl.FileTypeSupport.Bilingual.SdlXliff
{
	public class DocSkeleton
	{
		private class RevisionComparer : IEqualityComparer<IRevisionProperties>
		{
			public bool Equals(IRevisionProperties x, IRevisionProperties y)
			{
				return x == y;
			}

			public int GetHashCode(IRevisionProperties obj)
			{
				return RuntimeHelpers.GetHashCode(obj);
			}
		}

		private Dictionary<ICommentProperties, string> _commentsItems = new Dictionary<ICommentProperties, string>();

		private Dictionary<string, ICommentProperties> _commentsById = new Dictionary<string, ICommentProperties>();

		private Dictionary<IRevisionProperties, string> _revisions;

		private Dictionary<string, IRevisionProperties> _revisionsById = new Dictionary<string, IRevisionProperties>();

		public bool HasCommentsItems => _commentsItems.Count > 0;

		public bool HasRevisions => _revisions.Count > 0;

		public IEnumerable<KeyValuePair<string, ICommentProperties>> CommentsItemsById => _commentsById;

		public IEnumerable<KeyValuePair<string, IRevisionProperties>> RevisionsById => _revisionsById;

		public DocSkeleton()
		{
			_revisions = new Dictionary<IRevisionProperties, string>(new RevisionComparer());
		}

		public bool HasCommentsItemWithId(string id)
		{
			if (_commentsById.ContainsKey(id))
			{
				return true;
			}
			return false;
		}

		public void AddCommentsItem(string id, ICommentProperties commentsItem)
		{
			if (!_commentsItems.ContainsKey(commentsItem))
			{
				_commentsItems.Add(commentsItem, id);
			}
			if (!_commentsById.ContainsKey(id))
			{
				_commentsById.Add(id, commentsItem);
			}
		}

		public string StoreComments(ICommentProperties comments)
		{
			if (comments == null)
			{
				throw new FileTypeSupportException("Comments parameter null!");
			}
			if (_commentsItems.ContainsKey(comments))
			{
				return _commentsItems[comments];
			}
			string text = Guid.NewGuid().ToString();
			_commentsItems.Add(comments, text);
			_commentsById.Add(text, comments);
			return text;
		}

		public ICommentProperties GetComments(string id)
		{
			if (_commentsById.ContainsKey(id))
			{
				return _commentsById[id];
			}
			return null;
		}

		public bool HasRevisionWithId(string id)
		{
			if (_commentsById.ContainsKey(id))
			{
				return true;
			}
			return false;
		}

		public void AddRevision(string id, IRevisionProperties revision)
		{
			_revisions.Add(revision, id);
			_revisionsById.Add(id, revision);
		}

		public string StoreRevision(IRevisionProperties revision)
		{
			if (revision == null)
			{
				throw new ArgumentNullException("revision");
			}
			if (_revisions.ContainsKey(revision))
			{
				return _revisions[revision];
			}
			string text = Guid.NewGuid().ToString();
			_revisions.Add(revision, text);
			_revisionsById.Add(text, revision);
			return text;
		}

		public IRevisionProperties GetRevision(string id)
		{
			if (_revisionsById.TryGetValue(id, out IRevisionProperties value))
			{
				return value;
			}
			return null;
		}
	}
}
