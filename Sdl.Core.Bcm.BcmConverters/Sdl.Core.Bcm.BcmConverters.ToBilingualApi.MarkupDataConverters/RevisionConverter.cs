using Sdl.Core.Bcm.BcmConverters.ToBilingualApi.Extensions;
using Sdl.Core.Bcm.BcmModel;
using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.Core.Bcm.BcmConverters.ToBilingualApi.MarkupDataConverters
{
	internal class RevisionConverter : MarkupDataConverter
	{
		private readonly IPropertiesFactory _propertiesFactory;

		public RevisionConverter(IPropertiesFactory propertiesFactory)
		{
			_propertiesFactory = propertiesFactory;
		}

		internal override IAbstractMarkupData Convert(MarkupData source)
		{
			RevisionContainer revisionContainer = source as RevisionContainer;
			if (revisionContainer == null)
			{
				return null;
			}
			RevisionProperties revisionProperties = _propertiesFactory.CreateRevisionProperties(ConvertRevisionType(revisionContainer.RevisionType)) as RevisionProperties;
			revisionProperties.Author = revisionContainer.Author;
			revisionProperties.Date = revisionContainer.Timestamp;
			revisionProperties.CopyMetadataFrom(revisionProperties.MetaData);
			RevisionMarker revisionMarker = new RevisionMarker
			{
				Properties = revisionProperties
			};
			revisionMarker.ConvertAndAddChildren(revisionContainer.Children, base.ConverterFactory);
			return revisionMarker;
		}

		private static Sdl.FileTypeSupport.Framework.BilingualApi.RevisionType ConvertRevisionType(Sdl.Core.Bcm.BcmModel.RevisionType revisionType)
		{
			switch (revisionType)
			{
			case Sdl.Core.Bcm.BcmModel.RevisionType.Inserted:
				return Sdl.FileTypeSupport.Framework.BilingualApi.RevisionType.Insert;
			case Sdl.Core.Bcm.BcmModel.RevisionType.Deleted:
				return Sdl.FileTypeSupport.Framework.BilingualApi.RevisionType.Delete;
			case Sdl.Core.Bcm.BcmModel.RevisionType.Unchanged:
				return Sdl.FileTypeSupport.Framework.BilingualApi.RevisionType.Unchanged;
			default:
				throw new ArgumentOutOfRangeException("revisionType");
			}
		}
	}
}
