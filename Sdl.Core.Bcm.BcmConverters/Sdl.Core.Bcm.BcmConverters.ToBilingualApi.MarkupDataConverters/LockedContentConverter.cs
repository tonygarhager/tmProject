using Sdl.Core.Bcm.BcmConverters.ToBilingualApi.Extensions;
using Sdl.Core.Bcm.BcmModel;
using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Core.Bcm.BcmConverters.ToBilingualApi.MarkupDataConverters
{
	internal class LockedContentConverter : MarkupDataConverter
	{
		private readonly IPropertiesFactory _propertiesFactory;

		public LockedContentConverter(IPropertiesFactory propertiesFactory)
		{
			_propertiesFactory = propertiesFactory;
		}

		internal override IAbstractMarkupData Convert(MarkupData source)
		{
			LockedContentContainer lockedContentContainer = source as LockedContentContainer;
			if (lockedContentContainer == null)
			{
				return null;
			}
			LockedContent lockedContent = new LockedContent
			{
				Content = new LockedContainer(),
				Properties = _propertiesFactory.CreateLockedContentProperties(LockTypeFlags.Manual)
			};
			lockedContent.Content.ConvertAndAddChildren(lockedContentContainer.Children, base.ConverterFactory);
			return lockedContent;
		}
	}
}
