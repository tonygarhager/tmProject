using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi.Buffer
{
	[Serializable]
	public class CommentStartContentItem : AbstractContentItem
	{
		public ICommentProperties CommentProperties
		{
			get;
			set;
		}

		public CommentStartContentItem(ICommentProperties commentInfo)
		{
			if (commentInfo == null)
			{
				throw new ArgumentNullException("commentInfo");
			}
			CommentProperties = commentInfo;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			CommentStartContentItem commentStartContentItem = (CommentStartContentItem)obj;
			if (CommentProperties != null)
			{
				return CommentProperties.Equals(commentStartContentItem.CommentProperties);
			}
			return commentStartContentItem.CommentProperties == null;
		}

		public override int GetHashCode()
		{
			int num = (CommentProperties != null) ? CommentProperties.GetHashCode() : 0;
			return base.GetHashCode() ^ num;
		}

		public override string ToString()
		{
			if (CommentProperties != null)
			{
				return CommentProperties.ToString();
			}
			return base.ToString();
		}

		public override void Invoke(IAbstractNativeContentHandler output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			output.CommentStart(CommentProperties);
		}
	}
}
