using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.FileTypeSupport.Framework.Bilingual
{
	[Serializable]
	public class StructureTag : AbstractTag, IStructureTag, IAbstractTag, IAbstractDataContent, IAbstractMarkupData, ICloneable, ISupportsUniqueId
	{
		public virtual IStructureTagProperties Properties
		{
			get
			{
				return (IStructureTagProperties)TagProperties;
			}
			set
			{
				TagProperties = value;
			}
		}

		public StructureTag()
		{
		}

		protected StructureTag(StructureTag other)
			: base(other)
		{
		}

		public override object Clone()
		{
			return new StructureTag(this);
		}

		public override void AcceptVisitor(IMarkupDataVisitor visitor)
		{
			throw new NotImplementedException(StringResources.StructureTag_MarkupVisitorError);
		}
	}
}
