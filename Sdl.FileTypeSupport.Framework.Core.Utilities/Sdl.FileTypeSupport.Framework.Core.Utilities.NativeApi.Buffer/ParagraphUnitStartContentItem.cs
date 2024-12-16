using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi.Buffer
{
	[Serializable]
	public class ParagraphUnitStartContentItem : AbstractContentItem
	{
		private IParagraphUnitProperties _properties;

		public ParagraphUnitStartContentItem(IParagraphUnitProperties properties)
		{
			_properties = properties;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			ParagraphUnitStartContentItem paragraphUnitStartContentItem = (ParagraphUnitStartContentItem)obj;
			if (paragraphUnitStartContentItem._properties == null != (_properties == null))
			{
				return false;
			}
			if (_properties != null && !_properties.Equals(paragraphUnitStartContentItem._properties))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			if (_properties == null)
			{
				return 0;
			}
			return _properties.GetHashCode();
		}

		public override string ToString()
		{
			if (_properties == null)
			{
				return base.ToString();
			}
			return _properties.ToString();
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
			nativeGenerationContentHandler.ParagraphUnitStart(_properties);
		}
	}
}
