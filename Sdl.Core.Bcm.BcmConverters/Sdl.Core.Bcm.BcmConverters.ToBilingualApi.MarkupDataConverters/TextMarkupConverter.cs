using Sdl.Core.Bcm.BcmModel;
using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Native;

namespace Sdl.Core.Bcm.BcmConverters.ToBilingualApi.MarkupDataConverters
{
	internal class TextMarkupConverter : MarkupDataConverter
	{
		private readonly PropertiesFactory _propertiesFactory;

		public TextMarkupConverter(PropertiesFactory propertiesFactory)
		{
			_propertiesFactory = propertiesFactory;
		}

		internal override IAbstractMarkupData Convert(MarkupData source)
		{
			TextMarkup textMarkup = source as TextMarkup;
			if (textMarkup == null)
			{
				return null;
			}
			return new Text
			{
				Properties = _propertiesFactory.CreateTextProperties(textMarkup.Text)
			};
		}
	}
}
