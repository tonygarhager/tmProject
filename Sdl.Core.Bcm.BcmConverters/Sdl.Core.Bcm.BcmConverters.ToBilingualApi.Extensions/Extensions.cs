using Sdl.Core.Bcm.BcmConverters.Common;
using Sdl.Core.Bcm.BcmConverters.ToBilingualApi.MarkupDataConverters;
using Sdl.Core.Bcm.BcmModel;
using Sdl.Core.Bcm.BcmModel.Annotations;
using Sdl.Core.Bcm.BcmModel.Skeleton;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Sdl.Core.Bcm.BcmConverters.ToBilingualApi.Extensions
{
	internal static class Extensions
	{
		internal static void CopyMetadataFrom(this IMetaDataContainer container, IEnumerable<KeyValuePair<string, string>> metaData, bool isEndTagMetadata = false)
		{
			if (metaData != null)
			{
				foreach (KeyValuePair<string, string> metaDatum in metaData)
				{
					string text = metaDatum.Key;
					if (isEndTagMetadata)
					{
						text = text.Substring("__end_".Length);
					}
					container.SetMetaData(text, metaDatum.Value);
				}
			}
		}

		internal static void ConvertAndAddChildren(this ICollection<IAbstractMarkupData> container, IEnumerable<MarkupData> children, MarkupDataConverterFactory factory)
		{
			if (children != null)
			{
				foreach (MarkupData child in children)
				{
					if (child is TerminologyAnnotationContainer)
					{
						ProcessTerminologyContainer(ref container, child, factory);
					}
					else
					{
						MarkupDataConverter converter = factory.GetConverter(child.GetType());
						IAbstractMarkupData item = converter.Convert(child);
						container.Add(item);
					}
				}
			}
		}

		internal static Sdl.FileTypeSupport.Framework.NativeApi.SegmentationHint Convert(this Sdl.Core.Bcm.BcmModel.SegmentationHint segmentationHint)
		{
			switch (segmentationHint)
			{
			case Sdl.Core.Bcm.BcmModel.SegmentationHint.Include:
				return Sdl.FileTypeSupport.Framework.NativeApi.SegmentationHint.Include;
			case Sdl.Core.Bcm.BcmModel.SegmentationHint.MayExclude:
				return Sdl.FileTypeSupport.Framework.NativeApi.SegmentationHint.MayExclude;
			case Sdl.Core.Bcm.BcmModel.SegmentationHint.IncludeWithText:
				return Sdl.FileTypeSupport.Framework.NativeApi.SegmentationHint.IncludeWithText;
			case Sdl.Core.Bcm.BcmModel.SegmentationHint.Exclude:
				return Sdl.FileTypeSupport.Framework.NativeApi.SegmentationHint.Exclude;
			default:
				throw new ArgumentOutOfRangeException("segmentationHint", segmentationHint, null);
			}
		}

		internal static Color Convert(this string displayColor)
		{
			string[] array = displayColor.Split(',');
			int red = int.Parse(array[0]);
			int green = int.Parse(array[1]);
			int blue = int.Parse(array[2]);
			return Color.FromArgb(red, green, blue);
		}

		internal static string ToRgbColorString(this Color displayColor)
		{
			return $"{displayColor.R},{displayColor.G},{displayColor.B}";
		}

		internal static Sdl.FileTypeSupport.Framework.NativeApi.DependencyFileUsage Convert(this Sdl.Core.Bcm.BcmModel.DependencyFileUsage usage)
		{
			switch (usage)
			{
			case Sdl.Core.Bcm.BcmModel.DependencyFileUsage.None:
				return Sdl.FileTypeSupport.Framework.NativeApi.DependencyFileUsage.None;
			case Sdl.Core.Bcm.BcmModel.DependencyFileUsage.Extraction:
				return Sdl.FileTypeSupport.Framework.NativeApi.DependencyFileUsage.Extraction;
			case Sdl.Core.Bcm.BcmModel.DependencyFileUsage.Generation:
				return Sdl.FileTypeSupport.Framework.NativeApi.DependencyFileUsage.Generation;
			case Sdl.Core.Bcm.BcmModel.DependencyFileUsage.Final:
				return Sdl.FileTypeSupport.Framework.NativeApi.DependencyFileUsage.Final;
			default:
				return Sdl.FileTypeSupport.Framework.NativeApi.DependencyFileUsage.None;
			}
		}

		internal static Severity Convert(this CommentSeverity commentSeverity)
		{
			switch (commentSeverity)
			{
			case CommentSeverity.Low:
				return Severity.Low;
			case CommentSeverity.Medium:
				return Severity.Medium;
			case CommentSeverity.High:
				return Severity.High;
			default:
				return Severity.Undefined;
			}
		}

		internal static Sdl.FileTypeSupport.Framework.NativeApi.SniffInfo Convert(this Sdl.Core.Bcm.BcmConverters.Common.SniffInfo sniffInfo)
		{
			Sdl.FileTypeSupport.Framework.NativeApi.SniffInfo sniffInfo2 = new Sdl.FileTypeSupport.Framework.NativeApi.SniffInfo
			{
				IsSupported = sniffInfo.IsSupported,
				SuggestedTargetEncoding = sniffInfo.SuggestedTargetEncoding
			};
			if (sniffInfo.DetectedEncoding != null)
			{
				sniffInfo2.DetectedEncoding = new Pair<Codepage, DetectionLevel>(new Codepage(sniffInfo.DetectedEncoding.Item1), sniffInfo.DetectedEncoding.Item2);
			}
			if (sniffInfo.DetectedSourceLanguage != null)
			{
				sniffInfo2.DetectedSourceLanguage = new Pair<Language, DetectionLevel>(new Language(sniffInfo.DetectedSourceLanguage.Item1), sniffInfo.DetectedSourceLanguage.Item2);
			}
			if (sniffInfo.DetectedTargetLanguage != null)
			{
				sniffInfo2.DetectedTargetLanguage = new Pair<Language, DetectionLevel>(new Language(sniffInfo.DetectedTargetLanguage.Item1), sniffInfo.DetectedTargetLanguage.Item2);
			}
			sniffInfo2.CopyMetadataFrom(sniffInfo.Metadata);
			return sniffInfo2;
		}

		private static void ProcessTerminologyContainer(ref ICollection<IAbstractMarkupData> container, MarkupData markupData, MarkupDataConverterFactory factory)
		{
			TerminologyAnnotationContainer terminologyAnnotationContainer = markupData as TerminologyAnnotationContainer;
			if (terminologyAnnotationContainer != null)
			{
				List<IAbstractMarkupData> list = new List<IAbstractMarkupData>();
				list.ConvertAndAddChildren(terminologyAnnotationContainer.Children, factory);
				foreach (IAbstractMarkupData item in list)
				{
					container.Add(item);
				}
			}
		}
	}
}
