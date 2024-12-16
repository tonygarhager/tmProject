using System.Collections.Generic;

namespace Sdl.Core.Bcm.BcmConverters.Common
{
	internal sealed class ContextPropertiesItem
	{
		public int? StructureInfoId
		{
			get;
			set;
		}

		public List<int> ContextInfoIds
		{
			get;
			set;
		}
	}
}
