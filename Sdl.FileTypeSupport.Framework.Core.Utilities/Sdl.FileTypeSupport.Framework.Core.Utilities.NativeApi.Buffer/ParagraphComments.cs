using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi.Buffer
{
	[Serializable]
	public class ParagraphComments : AbstractContentItem
	{
		public ICommentProperties CommentProperties
		{
			get;
			set;
		}

		public ParagraphComments(ICommentProperties commentInfo)
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
			ParagraphComments paragraphComments = (ParagraphComments)obj;
			if (CommentProperties != null)
			{
				return CommentProperties.Equals(paragraphComments.CommentProperties);
			}
			return paragraphComments.CommentProperties == null;
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
			output.ParagraphComments(CommentProperties);
		}
	}
}
