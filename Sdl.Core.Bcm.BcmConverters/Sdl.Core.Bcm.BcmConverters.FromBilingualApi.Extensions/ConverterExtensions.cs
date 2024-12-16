using Sdl.Core.Bcm.BcmModel;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Core.Bcm.BcmConverters.FromBilingualApi.Extensions
{
	internal static class ConverterExtensions
	{
		public static void CopyMetadataFrom(this MetadataContainer metaDataContainer, IMetaDataContainer nativeMetaDataContainer)
		{
			if (nativeMetaDataContainer != null && nativeMetaDataContainer.MetaDataCount != 0)
			{
				metaDataContainer.AddMetadataFrom(nativeMetaDataContainer.MetaData);
			}
		}
	}
}
