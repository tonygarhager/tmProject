using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi.Buffer
{
	[Serializable]
	public class StructureTagContentItem : AbstractContentItem
	{
		private IStructureTagProperties _Properties;

		public IStructureTagProperties Properties
		{
			get
			{
				return _Properties;
			}
			set
			{
				_Properties = value;
			}
		}

		public StructureTagContentItem(IStructureTagProperties properties)
		{
			_Properties = properties;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			StructureTagContentItem structureTagContentItem = (StructureTagContentItem)obj;
			if (_Properties != null)
			{
				return _Properties.Equals(structureTagContentItem._Properties);
			}
			return _Properties == structureTagContentItem._Properties;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ ((_Properties != null) ? _Properties.GetHashCode() : 0);
		}

		public override string ToString()
		{
			if (_Properties == null)
			{
				return base.ToString();
			}
			return _Properties.ToString();
		}

		public override void Invoke(IAbstractNativeContentHandler output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			output.StructureTag(_Properties);
		}
	}
}
