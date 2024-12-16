using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.FileTypeSupport.Framework.Native
{
	[Serializable]
	public class StructureTagProperties : AbstractTagProperties, IStructureTagProperties, IAbstractTagProperties, IAbstractBasicTagProperties, IMetaDataContainer, ICloneable
	{
		public StructureTagProperties()
		{
		}

		protected StructureTagProperties(StructureTagProperties other)
			: base(other)
		{
		}

		public override bool Equals(object obj)
		{
			if (!base.Equals(obj))
			{
				return false;
			}
			StructureTagProperties structureTagProperties = obj as StructureTagProperties;
			if (structureTagProperties == null)
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override object Clone()
		{
			return new StructureTagProperties(this);
		}
	}
}
