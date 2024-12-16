using Sdl.Core.Bcm.BcmConverters.ToBilingualApi.Extensions;
using Sdl.Core.Bcm.BcmModel;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Native;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.Bcm.BcmConverters.ToBilingualApi.MarkupDataConverters
{
	internal class SegmentConverter : MarkupDataConverter
	{
		private readonly IDocumentItemFactory _documentItemFactory;

		private readonly Dictionary<string, ISegmentPairProperties> _segPropsMap;

		public SegmentConverter(IDocumentItemFactory documentItemFactory)
		{
			_documentItemFactory = documentItemFactory;
			_segPropsMap = new Dictionary<string, ISegmentPairProperties>();
		}

		internal override IAbstractMarkupData Convert(MarkupData source)
		{
			Segment segment = source as Segment;
			if (segment == null)
			{
				return null;
			}
			ISegmentPairProperties segmentPairProperties = CreateSegmentPairProperties(segment);
			segmentPairProperties.ConfirmationLevel = (Sdl.Core.Globalization.ConfirmationLevel)segment.ConfirmationLevel;
			segmentPairProperties.Id = new SegmentId(segment.SegmentNumber);
			segmentPairProperties.IsLocked = segment.IsLocked;
			if (segment.TranslationOrigin == null)
			{
				segment.TranslationOrigin = new Sdl.Core.Bcm.BcmModel.TranslationOrigin();
			}
			segment.TranslationOrigin.AddMetadataFrom(segment.Metadata);
			segmentPairProperties.TranslationOrigin = ConvertTranslationOrigin(segment.TranslationOrigin);
			ISegment segment2 = _documentItemFactory.CreateSegment(segmentPairProperties);
			segment2.ConvertAndAddChildren(segment.Children, base.ConverterFactory);
			return segment2;
		}

		private ISegmentPairProperties CreateSegmentPairProperties(Segment segment)
		{
			ISegmentPairProperties segmentPairProperties = (from x in _segPropsMap
				where x.Key == segment.SegmentNumber
				select x.Value).FirstOrDefault();
			if (segmentPairProperties != null)
			{
				return segmentPairProperties;
			}
			segmentPairProperties = _documentItemFactory.CreateSegmentPairProperties();
			_segPropsMap.Add(segment.SegmentNumber, segmentPairProperties);
			return segmentPairProperties;
		}

		private static ITranslationOrigin ConvertTranslationOrigin(Sdl.Core.Bcm.BcmModel.TranslationOrigin to)
		{
			if (to == null)
			{
				return null;
			}
			Sdl.FileTypeSupport.Framework.Native.TranslationOrigin translationOrigin = new Sdl.FileTypeSupport.Framework.Native.TranslationOrigin
			{
				TextContextMatchLevel = (Sdl.FileTypeSupport.Framework.NativeApi.TextContextMatchLevel)to.TextContextMatchLevel,
				IsStructureContextMatch = to.IsStructureContextMatch,
				IsSIDContextMatch = to.IsSidContextMatch,
				MatchPercent = (byte)to.MatchPercent,
				OriginSystem = to.OriginSystem,
				OriginBeforeAdaptation = ConvertTranslationOrigin(to.OriginBeforeAdaptation),
				OriginType = to.OriginType
			};
			if (to.OriginalTranslationHash != null)
			{
				translationOrigin.OriginalTranslationHash = to.OriginalTranslationHash;
			}
			translationOrigin.CopyMetadataFrom(to.Metadata);
			return translationOrigin;
		}
	}
}
