using Sdl.Core.LanguageProcessing.Resources;
using Sdl.Core.LanguageProcessing.Segmentation.T8;
using Sdl.LanguagePlatform.Core.Resources;
using Sdl.LanguagePlatform.Core.Segmentation;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Sdl.Core.LanguageProcessing.Segmentation
{
	public class SegmentationEngineFactory
	{
		public static SegmentationEngine CreateSegmentationEngine(CultureInfo culture)
		{
			if (culture == null)
			{
				throw new ArgumentNullException("culture");
			}
			return CreateSegmentationEngine(null, culture);
		}

		public static SegmentationEngine CreateSegmentationEngine(CultureInfo culture, IResourceDataAccessor accessor, string segmentationRulesFilePath)
		{
			if (accessor == null)
			{
				accessor = new ResourceFileResourceAccessor();
			}
			if (culture == null)
			{
				throw new ArgumentNullException("culture");
			}
			if (string.IsNullOrEmpty(segmentationRulesFilePath))
			{
				throw new ArgumentNullException("segmentationRulesFilePath");
			}
			SegmentationRules segmentationRules = SegmentationRules.Load(segmentationRulesFilePath, culture, accessor, keepListReferences: false);
			if (segmentationRules == null)
			{
				throw new LanguageProcessingException(ErrorMessages.EMSG_SegmentationRuleLoadError);
			}
			return new T8SegmentationEngine(segmentationRules);
		}

		public static SegmentationEngine CreateSegmentationEngine(IResourceDataAccessor accessor, CultureInfo culture)
		{
			if (culture == null)
			{
				throw new ArgumentNullException("culture");
			}
			if (accessor == null)
			{
				accessor = new ResourceFileResourceAccessor();
			}
			ResourceStatus resourceStatus = accessor.GetResourceStatus(culture, LanguageResourceType.SegmentationRules, fallback: true);
			if (resourceStatus == ResourceStatus.NotAvailable)
			{
				throw new LanguageProcessingException(ErrorMessages.EMSG_SegmentationNoRulesForLanguage);
			}
			SegmentationRules segmentationRules = null;
			try
			{
				byte[] array = accessor.GetResourceData(culture, LanguageResourceType.SegmentationRules, fallback: true);
				if (array != null && array.Length != 0)
				{
					int num = array.Length - 1;
					while (num > 0 && array[num] == 0)
					{
						num--;
					}
					if (num < array.Length - 1)
					{
						array = array.ToList().GetRange(0, num + 1).ToArray();
					}
					using (MemoryStream reader = new MemoryStream(array, writable: false))
					{
						segmentationRules = SegmentationRules.Load(reader, culture, accessor);
					}
				}
			}
			catch (Exception)
			{
				throw new LanguageProcessingException(ErrorMessages.EMSG_SegmentationRuleDeserializationError);
			}
			if (segmentationRules == null)
			{
				throw new LanguageProcessingException(ErrorMessages.EMSG_SegmentationRuleLoadError);
			}
			return new T8SegmentationEngine(segmentationRules);
		}
	}
}
