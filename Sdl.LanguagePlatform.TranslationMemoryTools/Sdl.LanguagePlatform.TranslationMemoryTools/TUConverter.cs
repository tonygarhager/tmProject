using Sdl.Core.Globalization;
using Sdl.Core.LanguageProcessing.Tokenization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Sdl.LanguagePlatform.TranslationMemoryTools
{
	public class TUConverter
	{
		private const string SidKey = "sdl:sid";

		public static Segment BuildLinguaSegment(CultureInfo culture, ISegment segment, bool includeTrackChanges = false)
		{
			LinguaTuBuilderSettings settings = new LinguaTuBuilderSettings
			{
				IncludeTrackChanges = false,
				StripTags = false,
				ExcludeTagsInLockedContentText = false,
				AcceptTrackChanges = true
			};
			return BuildLinguaSegment(culture, segment, settings);
		}

		public static Segment BuildLinguaSegment(CultureInfo culture, ISegment segment, bool stripTags, bool excludeTagsInLockedContentText, bool acceptTrackChanges, out bool hasTrackChanges, bool includeTrackChanges = false)
		{
			LinguaTuBuilderSettings settings = new LinguaTuBuilderSettings
			{
				IncludeTrackChanges = includeTrackChanges,
				StripTags = stripTags,
				ExcludeTagsInLockedContentText = excludeTagsInLockedContentText,
				AcceptTrackChanges = acceptTrackChanges
			};
			List<KeyValuePair<Tag, IAbstractMarkupData>> tagAssociations;
			List<KeyValuePair<Text, IAbstractMarkupData>> textAssociations;
			return BuildLinguaSegmentInternal(culture, segment, settings, out hasTrackChanges, out tagAssociations, out textAssociations);
		}

		public static Segment BuildLinguaSegment(CultureInfo culture, ISegment segment, LinguaTuBuilderSettings settings)
		{
			bool hasTrackChanges;
			List<KeyValuePair<Tag, IAbstractMarkupData>> tagAssociations;
			List<KeyValuePair<Text, IAbstractMarkupData>> textAssociations;
			return BuildLinguaSegmentInternal(culture, segment, settings, out hasTrackChanges, out tagAssociations, out textAssociations);
		}

		public static Segment BuildLinguaSegment(CultureInfo culture, ISegment segment, LinguaTuBuilderSettings settings, out List<KeyValuePair<Text, IAbstractMarkupData>> textAssociations, out List<KeyValuePair<Tag, IAbstractMarkupData>> tagAssociations)
		{
			bool hasTrackChanges;
			return BuildLinguaSegmentInternal(culture, segment, settings, out hasTrackChanges, out tagAssociations, out textAssociations);
		}

		private static Segment BuildLinguaSegmentInternal(CultureInfo culture, IAbstractMarkupDataContainer segment, LinguaTuBuilderSettings settings, out bool hasTrackChanges, out List<KeyValuePair<Tag, IAbstractMarkupData>> tagAssociations, out List<KeyValuePair<Text, IAbstractMarkupData>> textAssociations)
		{
			Segment segment2 = new Segment(culture);
			hasTrackChanges = AppendToLinguaSegment(segment, segment2, settings, out tagAssociations, out textAssociations);
			if (!settings.DoNotTrim)
			{
				segment2.Trim();
			}
			segment2.MergeAdjacentTextRuns();
			return segment2;
		}

		public static Segment BuildLinguaSegment(CultureInfo culture, ISegment segment, bool stripTags, bool excludeTagsInLockedContentText, bool includeTrackChanges = false)
		{
			bool hasTrackChanges;
			return BuildLinguaSegment(culture, segment, stripTags, excludeTagsInLockedContentText, acceptTrackChanges: true, out hasTrackChanges, includeTrackChanges);
		}

		public static string GetMostSignificantStructureContext(IParagraphUnitProperties paragraphProperties)
		{
			if (paragraphProperties?.Contexts?.Contexts == null || paragraphProperties.Contexts.Contexts.Count <= 0)
			{
				return null;
			}
			return (from context in paragraphProperties.Contexts.Contexts
				where context.Purpose == ContextPurpose.Match && context.ContextType != "sdl:sid"
				select context.ContextType).FirstOrDefault();
		}

		public static string GeSIDContext(IParagraphUnitProperties paragraphProperties)
		{
			if (paragraphProperties?.Contexts?.Contexts == null || paragraphProperties.Contexts.Contexts.Count <= 0)
			{
				return null;
			}
			return (from context in paragraphProperties.Contexts.Contexts
				where context.Purpose == ContextPurpose.Match && context.ContextType == "sdl:sid"
				select context.Description).FirstOrDefault();
		}

		public static TranslationUnit BuildLinguaTranslationUnit(LanguagePair lp, IDocumentProperties documentProperties, IFileProperties fileProperties, ISegmentPair sp, IParagraphUnitProperties paragraphProperties, bool stripTags, bool excludeTagsInLockedContentText, bool acceptTrackChanges, out bool hasSourceTrackChanges, bool includeTrackChanges = false)
		{
			TranslationUnit translationUnit = BuildLinguaTranslationUnitInternal(lp, sp, paragraphProperties, stripTags, excludeTagsInLockedContentText, acceptTrackChanges, alignTags: false, out hasSourceTrackChanges, includeTrackChanges);
			SetTranslationUnitDocInfo(translationUnit, documentProperties, fileProperties, sp);
			return translationUnit;
		}

		public static TranslationUnit BuildLinguaTranslationUnit(LanguagePair lp, ISegmentPair sp, IParagraphUnitProperties paragraphProperties, bool stripTags, bool excludeTagsInLockedContentText, bool acceptTrackChanges, out bool hasSourceTrackChanges, bool includeTrackChanges = false)
		{
			return BuildLinguaTranslationUnitInternal(lp, sp, paragraphProperties, stripTags, excludeTagsInLockedContentText, acceptTrackChanges, alignTags: false, out hasSourceTrackChanges, includeTrackChanges);
		}

		public static TranslationUnit BuildLinguaTranslationUnit(LanguagePair lp, IDocumentProperties documentProperties, IFileProperties fileProperties, ISegmentPair sp, IParagraphUnitProperties paragraphProperties, bool stripTags, bool excludeTagsInLockedContentText, bool acceptTrackChanges, bool alignTags, out bool hasSourceTrackChanges, bool includeTrackChanges = false)
		{
			TranslationUnit translationUnit = BuildLinguaTranslationUnitInternal(lp, sp, paragraphProperties, stripTags, excludeTagsInLockedContentText, acceptTrackChanges, alignTags, out hasSourceTrackChanges, includeTrackChanges);
			SetTranslationUnitDocInfo(translationUnit, documentProperties, fileProperties, sp);
			return translationUnit;
		}

		public static TranslationUnit BuildLinguaTranslationUnit(LanguagePair lp, ISegmentPair sp, IParagraphUnitProperties paragraphProperties, bool stripTags, bool excludeTagsInLockedContentText, bool acceptTrackChanges, bool alignTags, out bool hasSourceTrackChanges, bool includeTrackChanges = false)
		{
			return BuildLinguaTranslationUnitInternal(lp, sp, paragraphProperties, stripTags, excludeTagsInLockedContentText, acceptTrackChanges, alignTags, out hasSourceTrackChanges, includeTrackChanges);
		}

		public static List<Token> TokenizeSegment(Segment segment)
		{
			return new Tokenizer(new TokenizerSetup
			{
				Culture = segment.Culture,
				BuiltinRecognizers = BuiltinRecognizers.RecognizeNone
			}).Tokenize(segment);
		}

		internal static void OverwriteTargetTagIdsFromSource(List<KeyValuePair<Tag, IAbstractMarkupData>> sourceTags, List<KeyValuePair<Tag, IAbstractMarkupData>> targetTags)
		{
			Tag[] source = (from x in sourceTags
				where x.Key.Type == TagType.LockedContent
				select x.Key).ToArray();
			Tag[] array = (from x in targetTags
				where x.Key.Type == TagType.LockedContent
				select x.Key).ToArray();
			List<string> usedIds = new List<string>();
			Tag[] array2 = array;
			foreach (Tag targetLc in array2)
			{
				Tag tag = source.FirstOrDefault((Tag x) => x.TextEquivalent == targetLc.TextEquivalent && !usedIds.Contains(x.TagID));
				if (tag != null)
				{
					targetLc.TagID = tag.TagID;
					usedIds.Add(tag.TagID);
				}
			}
		}

		private static TranslationUnit BuildLinguaTranslationUnitInternal(LanguagePair lp, ISegmentPair sp, IParagraphUnitProperties paragraphProperties, bool stripTags, bool excludeTagsInLockedContentText, bool acceptTrackChanges, bool alignTags, out bool hasSourceTrackChanges, bool includeTrackChanges = false)
		{
			hasSourceTrackChanges = false;
			if (lp == null)
			{
				throw new ArgumentNullException("lp");
			}
			if (sp == null)
			{
				throw new ArgumentNullException("sp");
			}
			List<KeyValuePair<Tag, IAbstractMarkupData>> tagAssociations = null;
			LinguaTuBuilderSettings settings = new LinguaTuBuilderSettings
			{
				IncludeTrackChanges = includeTrackChanges,
				StripTags = stripTags,
				ExcludeTagsInLockedContentText = excludeTagsInLockedContentText,
				AcceptTrackChanges = acceptTrackChanges
			};
			List<KeyValuePair<Tag, IAbstractMarkupData>> tagAssociations2;
			List<KeyValuePair<Text, IAbstractMarkupData>> textAssociations;
			Segment segment = BuildLinguaSegmentInternal(lp.SourceCulture, sp.Source, settings, out hasSourceTrackChanges, out tagAssociations2, out textAssociations);
			if (segment.IsEmpty)
			{
				return null;
			}
			Segment segment2 = null;
			if (sp.Target != null && lp.TargetCulture != null)
			{
				segment2 = BuildLinguaSegmentInternal(lp.TargetCulture, sp.Target, settings, out bool _, out tagAssociations, out textAssociations);
			}
			if (tagAssociations != null)
			{
				OverwriteTargetTagIdsFromSource(tagAssociations2, tagAssociations);
			}
			if (alignTags && tagAssociations2 != null && tagAssociations != null && tagAssociations2.Count > 0 && tagAssociations.Count > 0)
			{
				AlignTags(tagAssociations2, tagAssociations);
			}
			TranslationUnit translationUnit = new TranslationUnit
			{
				SourceSegment = segment,
				TargetSegment = segment2,
				Origin = TranslationUnitOrigin.Unknown
			};
			if (sp.Properties != null)
			{
				translationUnit.ConfirmationLevel = sp.Properties.ConfirmationLevel;
				if (sp.Properties.TranslationOrigin?.MetaData == null || !sp.Properties.TranslationOrigin.MetaData.Any())
				{
					DateTime dateTime2 = translationUnit.SystemFields.CreationDate = (translationUnit.SystemFields.ChangeDate = DateTime.Now);
				}
				else
				{
					foreach (KeyValuePair<string, string> metaDatum in sp.Properties.TranslationOrigin.MetaData)
					{
						switch (metaDatum.Key)
						{
						case "created_on":
							translationUnit.SystemFields.CreationDate = TryParseDateTimeWithFallback(metaDatum.Value);
							break;
						case "created_by":
							translationUnit.SystemFields.CreationUser = metaDatum.Value;
							break;
						case "modified_on":
							translationUnit.SystemFields.ChangeDate = TryParseDateTimeWithFallback(metaDatum.Value);
							break;
						case "last_modified_by":
							translationUnit.SystemFields.ChangeUser = metaDatum.Value;
							break;
						}
					}
				}
			}
			if (segment2 != null && sp.Properties.TranslationOrigin != null && !string.IsNullOrEmpty(sp.Properties.TranslationOrigin.OriginType))
			{
				switch (sp.Properties.TranslationOrigin.OriginType)
				{
				case "document-match":
					translationUnit.Origin = TranslationUnitOrigin.ContextTM;
					break;
				case "auto-aligned":
					translationUnit.Origin = TranslationUnitOrigin.Alignment;
					break;
				case "mt":
					if (sp.Properties.ConfirmationLevel != ConfirmationLevel.Translated)
					{
						translationUnit.Origin = TranslationUnitOrigin.MachineTranslation;
						break;
					}
					goto case "tm";
				case "tm":
					translationUnit.Origin = TranslationUnitOrigin.TM;
					break;
				default:
					translationUnit.Origin = TranslationUnitOrigin.Unknown;
					break;
				}
			}
			if (paragraphProperties == null)
			{
				paragraphProperties = sp.Source.ParentParagraphUnit?.Properties;
			}
			if (paragraphProperties == null)
			{
				return translationUnit;
			}
			string mostSignificantStructureContext = GetMostSignificantStructureContext(paragraphProperties);
			if (!string.IsNullOrEmpty(mostSignificantStructureContext))
			{
				translationUnit.StructureContexts = new string[1]
				{
					mostSignificantStructureContext
				};
			}
			string text = GeSIDContext(paragraphProperties);
			if (!string.IsNullOrEmpty(text))
			{
				translationUnit.IdContexts.Add(text);
			}
			return translationUnit;
		}

		private static DateTime TryParseDateTimeWithFallback(string metaValue)
		{
			if (DateTime.TryParse(metaValue, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime result))
			{
				return result;
			}
			if (!DateTime.TryParse(metaValue, CultureInfo.CurrentCulture, DateTimeStyles.AssumeUniversal, out result))
			{
				return DateTime.MinValue;
			}
			return result;
		}

		private static void AlignTags(List<KeyValuePair<Tag, IAbstractMarkupData>> sourceTags, List<KeyValuePair<Tag, IAbstractMarkupData>> targetTags)
		{
			if (sourceTags != null && targetTags != null)
			{
				int num = sourceTags.Max((KeyValuePair<Tag, IAbstractMarkupData> t) => t.Key.AlignmentAnchor);
				foreach (KeyValuePair<Tag, IAbstractMarkupData> targetTag in targetTags)
				{
					num = Math.Max(num, targetTag.Key.AlignmentAnchor);
				}
				foreach (KeyValuePair<Tag, IAbstractMarkupData> sourceTag in sourceTags)
				{
					if (sourceTag.Key.AlignmentAnchor == 0 && IsAlignableTag(sourceTag))
					{
						foreach (KeyValuePair<Tag, IAbstractMarkupData> targetTag2 in targetTags)
						{
							if (targetTag2.Key.AlignmentAnchor == 0 && IsAlignableTag(targetTag2) && sourceTag.Key.Type == targetTag2.Key.Type)
							{
								bool flag = false;
								switch (sourceTag.Key.Type)
								{
								case TagType.TextPlaceholder:
								case TagType.LockedContent:
									flag = string.Equals(sourceTag.Key.TextEquivalent, targetTag2.Key.TextEquivalent);
									break;
								case TagType.Start:
								case TagType.Standalone:
									flag = AreAlignable(sourceTag, targetTag2);
									break;
								}
								if (flag)
								{
									num++;
									sourceTag.Key.AlignmentAnchor = num;
									targetTag2.Key.AlignmentAnchor = num;
									break;
								}
							}
						}
					}
				}
			}
		}

		private static bool IsAlignableTag(KeyValuePair<Tag, IAbstractMarkupData> tag)
		{
			Tag key = tag.Key;
			if (key != null)
			{
				return key.Type != TagType.End;
			}
			return false;
		}

		private static bool AreAlignable(KeyValuePair<Tag, IAbstractMarkupData> leftTag, KeyValuePair<Tag, IAbstractMarkupData> rightTag)
		{
			if (string.Equals(leftTag.Key.TagID, rightTag.Key.TagID))
			{
				return true;
			}
			if (leftTag.Value != null || rightTag.Value != null)
			{
				return false;
			}
			IAbstractMarkupData value = leftTag.Value;
			IAbstractTag abstractTag = value as IAbstractTag;
			IRevisionMarker revisionMarker;
			if (abstractTag != null)
			{
				IAbstractTag abstractTag2 = rightTag.Value as IAbstractTag;
				if (abstractTag2 != null)
				{
					if (leftTag.Value == null || rightTag.Value == null || abstractTag.TagProperties == null || abstractTag2.TagProperties == null)
					{
						return false;
					}
					IStartTagProperties startTagProperties = abstractTag.TagProperties as IStartTagProperties;
					if (startTagProperties != null)
					{
						IStartTagProperties startTagProperties2 = abstractTag2.TagProperties as IStartTagProperties;
						if (startTagProperties2 != null)
						{
							if (startTagProperties.SegmentationHint != startTagProperties2.SegmentationHint || startTagProperties.CanHide != startTagProperties2.CanHide || startTagProperties.IsSoftBreak != startTagProperties2.IsSoftBreak || startTagProperties.IsWordStop != startTagProperties2.IsWordStop)
							{
								return false;
							}
							if (startTagProperties.Formatting == null != (startTagProperties2.Formatting == null))
							{
								return false;
							}
							if (startTagProperties.Formatting == null)
							{
								return string.Equals(startTagProperties.TagContent, startTagProperties2.TagContent, StringComparison.InvariantCultureIgnoreCase);
							}
							return startTagProperties.Formatting.Equals(startTagProperties2.Formatting);
						}
					}
					return false;
				}
				revisionMarker = (value as IRevisionMarker);
				if (revisionMarker != null)
				{
					goto IL_0155;
				}
			}
			else
			{
				revisionMarker = (value as IRevisionMarker);
				if (revisionMarker != null)
				{
					goto IL_0155;
				}
			}
			goto IL_016d;
			IL_016d:
			return false;
			IL_0155:
			IRevisionMarker revisionMarker2 = rightTag.Value as IRevisionMarker;
			if (revisionMarker2 != null)
			{
				return revisionMarker.Equals(revisionMarker2);
			}
			goto IL_016d;
		}

		public static TranslationUnit BuildLinguaTranslationUnit(LanguagePair lp, IDocumentProperties documentProperties, IFileProperties fileProperties, ISegmentPair sp, IParagraphUnitProperties paragraphProperties, bool stripTags, bool excludeTagsInLockedContentText, bool includeTrackChanges = false)
		{
			TranslationUnit translationUnit = BuildLinguaTranslationUnit(lp, sp, paragraphProperties, stripTags, excludeTagsInLockedContentText, includeTrackChanges);
			SetTranslationUnitDocInfo(translationUnit, documentProperties, fileProperties, sp);
			return translationUnit;
		}

		public static TranslationUnit BuildLinguaTranslationUnit(LanguagePair lp, ISegmentPair sp, IParagraphUnitProperties paragraphProperties, LinguaTuBuilderSettings flags)
		{
			bool hasSourceTrackChanges;
			return BuildLinguaTranslationUnitInternal(lp, sp, paragraphProperties, flags.StripTags, flags.ExcludeTagsInLockedContentText, flags.AcceptTrackChanges, flags.AlignTags, out hasSourceTrackChanges, flags.IncludeTrackChanges);
		}

		public static TranslationUnit BuildLinguaTranslationUnit(LanguagePair lp, ISegmentPair sp, IParagraphUnitProperties paragraphProperties, bool stripTags, bool excludeTagsInLockedContentText, bool includeTrackChanges = false)
		{
			bool hasSourceTrackChanges;
			return BuildLinguaTranslationUnitInternal(lp, sp, paragraphProperties, stripTags, excludeTagsInLockedContentText, acceptTrackChanges: true, alignTags: false, out hasSourceTrackChanges, includeTrackChanges);
		}

		private static bool AppendToLinguaSegment(IAbstractMarkupDataContainer data, Segment result, LinguaTuBuilderSettings flags, out List<KeyValuePair<Tag, IAbstractMarkupData>> tagAssociations, out List<KeyValuePair<Text, IAbstractMarkupData>> textAssociations)
		{
			LinguaSegmentBuilder linguaSegmentBuilder = new LinguaSegmentBuilder(result, flags);
			linguaSegmentBuilder.VisitChildNodes(data);
			tagAssociations = linguaSegmentBuilder.TagAssociations;
			textAssociations = linguaSegmentBuilder.TextAssociations;
			return linguaSegmentBuilder.HasTrackChanges;
		}

		private static void SetTranslationUnitDocInfo(TranslationUnit tu, IDocumentProperties documentProperties, IFileProperties fileProperties, ISegmentPair sp)
		{
			if (tu != null)
			{
				tu.DocumentProperties = (documentProperties?.Clone() as IDocumentProperties);
				tu.FileProperties = (fileProperties?.Clone() as IFileProperties);
				tu.DocumentSegmentPair = sp;
			}
		}
	}
}
