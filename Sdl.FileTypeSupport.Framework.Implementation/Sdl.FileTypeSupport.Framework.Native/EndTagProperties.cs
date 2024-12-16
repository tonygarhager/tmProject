using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.FileTypeSupport.Framework.Native
{
	[Serializable]
	public class EndTagProperties : AbstractBasicTagProperties, IEndTagProperties, IAbstractBasicTagProperties, IMetaDataContainer, ICloneable, IAbstractInlineTagProperties
	{
		public EndTagProperties()
		{
			base.IsWordStop = false;
		}

		protected EndTagProperties(EndTagProperties other)
			: base(other)
		{
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			EndTagProperties obj2 = (EndTagProperties)obj;
			if (!base.Equals((object)obj2))
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
			return new EndTagProperties(this);
		}
	}
}
