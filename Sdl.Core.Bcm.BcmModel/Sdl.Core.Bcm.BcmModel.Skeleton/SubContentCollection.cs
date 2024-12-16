using Sdl.Core.Bcm.BcmModel.Collections;
using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmModel.Skeleton
{
	[DataContract]
	public class SubContentCollection : KeyBasedCollection<string, LocalizableSubContent>
	{
		public SubContentCollection()
		{
			KeySelector = ((LocalizableSubContent subContent) => subContent.ParagraphUnitId);
		}
	}
}
