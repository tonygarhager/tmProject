using Sdl.Core.Bcm.BcmConverters.ToBilingualApi.Extensions;
using Sdl.Core.Bcm.BcmModel;
using Sdl.FileTypeSupport.Framework.Native;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Core.Bcm.BcmConverters.ToBilingualApi.MarkupDataConverters
{
	internal class DependencyFileConverter
	{
		public IDependencyFileProperties Convert(DependencyFile depFile)
		{
			return new DependencyFileProperties
			{
				Id = depFile.Id,
				ExpectedUsage = depFile.Usage.Convert(),
				CurrentFilePath = depFile.FileName,
				OriginalFilePath = depFile.Location
			};
		}
	}
}
