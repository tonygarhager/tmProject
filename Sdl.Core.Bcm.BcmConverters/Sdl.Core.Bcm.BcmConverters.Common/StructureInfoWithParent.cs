using Sdl.FileTypeSupport.Framework.Native;

namespace Sdl.Core.Bcm.BcmConverters.Common
{
	internal sealed class StructureInfoWithParent
	{
		public int ParentId
		{
			get;
			set;
		}

		public StructureInfo StructureInfo
		{
			get;
			set;
		}
	}
}
