using Newtonsoft.Json;
using Sdl.Core.Bcm.BcmModel;
using Sdl.Core.Bcm.BcmModel.Serialization;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Sdl.Core.Bcm.BcmConverters
{
	internal static class SegmentExtension
	{
		public static void CopyMetadataFromLinguaTu(this Segment segment, TranslationUnit tu, bool includeUserNameSystemFields)
		{
			segment.SetMetadata("tuid", Convert.ToString(tu.ResourceId.Id));
			segment.SetMetadata("tuguid", Convert.ToString(tu.ResourceId.Guid));
			List<TuContext> list = tu.Contexts?.Values?.ToList();
			if (list != null)
			{
				JsonSerializerSettings settings = new JsonSerializerSettings
				{
					Converters = new List<JsonConverter>
					{
						new SegmentElementJsonConverter()
					}
				};
				string value = JsonConvert.SerializeObject(list, settings);
				segment.SetMetadata("Contexts", value);
			}
			CreateTranslationOrigin(segment, tu);
			PopulateSegmentMetadataFields(segment, tu, includeUserNameSystemFields);
			PopulateTranslationOriginMetadata(segment, tu, includeUserNameSystemFields);
		}

		private static void PopulateSegmentMetadataFields(Segment segment, TranslationUnit tu, bool includeUserNameSystemFields)
		{
			JsonSerializerSettings settings = new JsonSerializerSettings
			{
				Converters = new List<JsonConverter>
				{
					new SystemFieldsJsonConverter(includeUserNameSystemFields)
				}
			};
			Dictionary<string, string> from = new Dictionary<string, string>
			{
				{
					"SystemFields",
					JsonConvert.SerializeObject(tu?.SystemFields, settings)
				},
				{
					"CustomFields",
					JsonConvert.SerializeObject(tu?.FieldValues)
				}
			};
			segment.AddMetadataFrom(from);
			segment.SetMetadata("tuInsertDate", tu?.InsertDate?.ToString(DateTimeFormatInfo.InvariantInfo));
		}

		private static void PopulateTranslationOriginMetadata(Segment segment, TranslationUnit tu, bool includeUserNameSystemFields)
		{
			segment.TranslationOrigin.SetMetadata("created_by", tu?.SystemFields.CreationUser);
			segment.TranslationOrigin.SetMetadata("created_on", tu?.SystemFields.CreationDate.ToString(DateTimeFormatInfo.InvariantInfo));
			segment.TranslationOrigin.SetMetadata("last_modified_by", tu?.SystemFields.ChangeUser);
			segment.TranslationOrigin.SetMetadata("modified_on", tu?.SystemFields.ChangeDate.ToString(DateTimeFormatInfo.InvariantInfo));
			segment.TranslationOrigin.SetMetadata("used_by", tu?.SystemFields.UseUser);
			segment.TranslationOrigin.SetMetadata("used_on", tu?.SystemFields.UseDate.ToString(DateTimeFormatInfo.InvariantInfo));
			if (includeUserNameSystemFields)
			{
				segment.TranslationOrigin.SetMetadata("created_by_username", tu?.SystemFields.CreationDisplayUsername);
				segment.TranslationOrigin.SetMetadata("modified_by_username", tu?.SystemFields.ChangeDisplayUsername);
				segment.TranslationOrigin.SetMetadata("use_by_username", tu?.SystemFields.UseDisplayUsername);
			}
		}

		private static void CreateTranslationOrigin(Segment segment, TranslationUnit tu)
		{
			string originalTranslationHash = tu?.TargetSegment?.GetWeakHashCode().ToString(CultureInfo.InvariantCulture);
			string segmentOriginFromTu = GetSegmentOriginFromTu(tu);
			segment.TranslationOrigin = new TranslationOrigin(segmentOriginFromTu, "", 0, isStructureContextMatch: false, isSidContextMatch: false, TextContextMatchLevel.None, originalTranslationHash, null);
		}

		private static string GetSegmentOriginFromTu(TranslationUnit tu)
		{
			if (tu == null)
			{
				return string.Empty;
			}
			switch (tu.Origin)
			{
			case TranslationUnitOrigin.TM:
				return "tm";
			case TranslationUnitOrigin.MachineTranslation:
				return "mt";
			case TranslationUnitOrigin.Alignment:
				return "auto-aligned";
			case TranslationUnitOrigin.ContextTM:
				return "document-match";
			case TranslationUnitOrigin.AdaptiveMachineTranslation:
				return "amt";
			case TranslationUnitOrigin.Nmt:
				return "nmt";
			case TranslationUnitOrigin.AutomaticTranslation:
				return "automatic-translation";
			case TranslationUnitOrigin.Unknown:
				return "unknown";
			default:
				return "unknown";
			}
		}
	}
}
