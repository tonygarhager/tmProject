using Sdl.Core.Bcm.BcmModel.Skeleton;

namespace Sdl.Core.Bcm.BcmModel
{
	public interface ISkeletonItemReference<out T> where T : SkeletonItem
	{
		T Definition
		{
			get;
		}
	}
}
