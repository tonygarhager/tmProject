using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.Bilingual
{
	[Serializable]
	public class CommentMarker : AbstractMarkerWithContent, ICommentMarker, IAbstractMarker, IAbstractMarkupData, ICloneable, ISupportsUniqueId, IAbstractMarkupDataContainer, IList<IAbstractMarkupData>, ICollection<IAbstractMarkupData>, IEnumerable<IAbstractMarkupData>, IEnumerable
	{
		private ICommentProperties _Comments;

		public ICommentProperties Comments
		{
			get
			{
				return _Comments;
			}
			set
			{
				_Comments = value;
			}
		}

		public CommentMarker()
		{
		}

		protected CommentMarker(CommentMarker commentMarker)
			: base(commentMarker)
		{
			_Comments = commentMarker._Comments;
		}

		protected CommentMarker(CommentMarker commentMarker, int splitBeforeItemIndex)
			: base(commentMarker, splitBeforeItemIndex)
		{
			_Comments = commentMarker._Comments;
		}

		public CommentMarker(params IAbstractMarkupData[] markupData)
		{
			ReadMarkupData(markupData);
		}

		private void ReadMarkupData(IList<IAbstractMarkupData> markupDataList)
		{
			foreach (IAbstractMarkupData markupData in markupDataList)
			{
				Add(markupData);
			}
		}

		public override void AcceptVisitor(IMarkupDataVisitor visitor)
		{
			if (visitor == null)
			{
				throw new ArgumentNullException("visitor");
			}
			visitor.VisitCommentMarker(this);
		}

		public override object Clone()
		{
			return new CommentMarker(this);
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			CommentMarker commentMarker = (CommentMarker)obj;
			if (!base.Equals((object)commentMarker))
			{
				return false;
			}
			if (_Comments == null != (commentMarker._Comments == null))
			{
				return false;
			}
			if (_Comments != null && !_Comments.Equals(commentMarker._Comments))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ ((_Comments != null) ? _Comments.GetHashCode() : 0);
		}

		public override IAbstractMarkupDataContainer Split(int splitBeforeItemIndex)
		{
			return new CommentMarker(this, splitBeforeItemIndex);
		}
	}
}
