using Sdl.Core.Bcm.BcmModel;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Core.Bcm.BcmConverters.ToBilingualApi.MarkupDataConverters
{
	internal abstract class MarkupDataConverter
	{
		protected internal MarkupDataConverterFactory ConverterFactory
		{
			protected get;
			set;
		}

		internal abstract IAbstractMarkupData Convert(MarkupData source);

		internal MarkupDataConverter SetFactory(MarkupDataConverterFactory factory)
		{
			ConverterFactory = factory;
			return this;
		}
	}
}
