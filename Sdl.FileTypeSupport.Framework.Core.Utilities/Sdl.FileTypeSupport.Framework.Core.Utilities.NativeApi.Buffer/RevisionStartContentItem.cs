using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi.Buffer
{
	[Serializable]
	public class RevisionStartContentItem : AbstractContentItem
	{
		public IRevisionProperties RevisionProperties
		{
			get;
			set;
		}

		public RevisionStartContentItem(IRevisionProperties revisionInfo)
		{
			if (revisionInfo == null)
			{
				throw new ArgumentNullException("revisionInfo");
			}
			RevisionProperties = revisionInfo;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			RevisionStartContentItem revisionStartContentItem = (RevisionStartContentItem)obj;
			if (RevisionProperties != null)
			{
				return RevisionProperties.Equals(revisionStartContentItem.RevisionProperties);
			}
			return revisionStartContentItem.RevisionProperties == null;
		}

		public override int GetHashCode()
		{
			int num = (RevisionProperties != null) ? RevisionProperties.GetHashCode() : 0;
			return base.GetHashCode() ^ num;
		}

		public override string ToString()
		{
			if (RevisionProperties != null)
			{
				return RevisionProperties.ToString();
			}
			return base.ToString();
		}

		public override void Invoke(IAbstractNativeContentHandler output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			output.RevisionStart(RevisionProperties);
		}
	}
}
