using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi.Buffer
{
	[Serializable]
	public class ParagraphUnitEndContentItem : AbstractContentItem
	{
		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ 0;
		}

		public override void Invoke(IAbstractNativeContentHandler output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			INativeGenerationContentHandler nativeGenerationContentHandler = output as INativeGenerationContentHandler;
			if (nativeGenerationContentHandler == null)
			{
				throw new ArgumentException("output parameter must be of type INativeGenerationContentHandler");
			}
			nativeGenerationContentHandler.ParagraphUnitEnd();
		}
	}
}
