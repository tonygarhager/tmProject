using Sdl.Core.Bcm.BcmModel.Collections;

namespace Sdl.Core.Bcm.BcmModel.Skeleton
{
	public interface ITermAttributeContainer
	{
		DictionaryEx<string, object> TermAttributes
		{
			get;
			set;
		}
	}
}
