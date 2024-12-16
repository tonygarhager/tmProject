using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Sdl.Core.Bcm.BcmConverters.Common;
using Sdl.Core.Bcm.BcmConverters.Extensions;
using Sdl.Core.Bcm.BcmConverters.Fields;
using Sdl.Core.Bcm.BcmConverters.TranslationUnitConverters.BcmToLinguaAlignmentSupport;
using Sdl.Core.Bcm.BcmConverters.TranslationUnitConverters.BcmToLinguaTokenSupport;
using Sdl.Core.Bcm.BcmConverters.TranslationUnitConverters.Lingua;
using Sdl.Core.Bcm.BcmModel;
using Sdl.Core.Bcm.BcmModel.Operations;
using Sdl.Core.Bcm.BcmModel.Serialization;
using Sdl.Core.Bcm.BcmModel.Skeleton;
using Sdl.Core.Bcm.BcmModel.Tokenization.Tokens;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Sdl.Core.Bcm.BcmConverters.TranslationUnitConverters
{
	public class BcmToLingua
	{
		private class AllPropertiesResolver : DefaultContractResolver
		{
			protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
			{
				JsonProperty jsonProperty = base.CreateProperty(member, memberSerialization);
				jsonProperty.Ignored = false;
				return jsonProperty;
			}
		}

		private class SkeletonContextInfo
		{
			public List<ContextDefinition> TmStrcutureContextDefinitions
			{
				get;
				set;
			}

			public List<ContextDefinition> SidContextDefinitions
			{
				get;
				set;
			}

			public List<Context> TmStructureContexts
			{
				get;
				set;
			}

			public List<Context> SidContexts
			{
				get;
				set;
			}

			public Dictionary<int, List<int>> ContextIdStructureContextDefinitionIds
			{
				get;
				set;
			}

			internal SkeletonContextInfo()
			{
				TmStrcutureContextDefinitions = new List<ContextDefinition>();
				SidContextDefinitions = new List<ContextDefinition>();
				TmStructureContexts = new List<Context>();
				SidContexts = new List<Context>();
				ContextIdStructureContextDefinitionIds = new Dictionary<int, List<int>>();
			}
		}

		private readonly Document _inputDocument;

		private readonly Fragment _fragment;

		private CultureInfo _sourceCulture;

		private CultureInfo _targetCulture;

		private readonly bool _createMappings;

		private readonly List<LinguaToBcmMap> _mapper = new List<LinguaToBcmMap>();

		private readonly Lazy<Dictionary<string, SkeletonContextInfo>> _skeletonContextInfo;

		public IEnumerable<LinguaToBcmMap> LinguaToBcmMapping => _mapper;

		public event EventHandler<BcmTuConversionEventArgs> TranslationUnitCreated;

		public BcmToLingua(Document inputDocument)
		{
			_inputDocument = inputDocument;
			_skeletonContextInfo = new Lazy<Dictionary<string, SkeletonContextInfo>>(() => PrepareSkeletonContextInfo(_inputDocument));
			SetCultures(inputDocument);
			_createMappings = false;
		}

		public BcmToLingua(Fragment fragment)
		{
			_fragment = fragment;
			SetCultures(fragment);
			_createMappings = false;
		}

		public BcmToLingua(Document inputDocument, bool createMappings)
			: this(inputDocument)
		{
			_createMappings = createMappings;
		}

		public BcmToLingua(Fragment fragment, bool createMappings)
			: this(fragment)
		{
			_createMappings = createMappings;
		}

		public TranslationUnit[] ConvertToTranslationUnits()
		{
			return ConvertToTranslationUnits(includeTokens: false, includeAlignmentData: false, includeUserNameSystemFields: false);
		}

		public TranslationUnit[] ConvertToTranslationUnits(bool includeTokens, bool includeAlignmentData, bool includeUserNameSystemFields)
		{
			if (_inputDocument != null)
			{
				return (from file in _inputDocument.Files
					from pu in file.ParagraphUnits
					select new
					{
						File = file,
						PU = pu
					} into puWrapper
					where !puWrapper.PU.IsStructure
					select new
					{
						File = puWrapper.File,
						PU = puWrapper.PU,
						SegmentPairs = puWrapper.PU.SegmentPairs,
						TmStructureContextDefinitionIds = GetPuTmStructureContextDefinitionIds(puWrapper.PU, puWrapper.File),
						SicContextDefinitionIds = GetPuSidContextDefinitionIds(puWrapper.PU, puWrapper.File)
					} into puWrapper
					from sp in puWrapper.SegmentPairs
					select new
					{
						PUWrapper = puWrapper,
						SegmentPair = sp
					} into spWrapper
					select ConvertSegmentPair(includeTokens, includeAlignmentData, includeUserNameSystemFields, spWrapper.SegmentPair, spWrapper.PUWrapper.File, spWrapper.PUWrapper.TmStructureContextDefinitionIds, spWrapper.PUWrapper.SicContextDefinitionIds)).ToArray();
			}
			return (from fsPair in GetFileSegmentPairs()
				select ConvertSegmentPair(includeTokens, includeAlignmentData, includeUserNameSystemFields, fsPair.SegmentPair, fsPair.File) into tu
				where tu != null
				select tu).ToArray();
		}

		public TranslationUnit[] ConvertToTranslationUnits(ParagraphUnit paragraphUnit, bool includeTokens, bool includeUserNameSystemFields, bool includeAlignmentData, File file)
		{
			List<TranslationUnit> list = new List<TranslationUnit>();
			if (paragraphUnit.IsStructure)
			{
				return list.ToArray();
			}
			foreach (SegmentPair segmentPair in paragraphUnit.SegmentPairs)
			{
				TranslationUnit translationUnit = ConvertSegmentPair(includeTokens, includeAlignmentData, includeUserNameSystemFields, segmentPair, file, GetPuTmStructureContextDefinitionIds(paragraphUnit, file), GetPuTmStructureContextDefinitionIds(paragraphUnit, file));
				if (translationUnit != null)
				{
					list.Add(translationUnit);
				}
			}
			return list.ToArray();
		}

		public TranslationUnit ConvertSegmentPair(bool includeTokens, bool includeAlignmentData, bool includeUserNameSystemFields, SegmentPair pair, File file)
		{
			if (_inputDocument != null)
			{
				return ConvertSegmentPair(includeTokens, includeAlignmentData, includeUserNameSystemFields, pair, file, GetPuTmStructureContextDefinitionIds(pair.Source.ParentParagraphUnit, file), GetPuSidContextDefinitionIds(pair.Source.ParentParagraphUnit, file));
			}
			return ConvertSegmentPair(includeTokens, includeAlignmentData, includeUserNameSystemFields, pair, file, null, null);
		}

		private TranslationUnit ConvertSegmentPair(bool includeTokens, bool includeAlignmentData, bool includeUserNameSystemFields, SegmentPair pair, File file, HashSet<int> puTmStructureContextDefinitionIds, HashSet<int> puSidContextDefinitionIds)
		{
			Sdl.Core.Globalization.ConfirmationLevel confirmationLevelOrDefault = GetConfirmationLevelOrDefault(pair);
			pair.Source.NormalizeTextItems();
			LinguaToBcmMap mapper;
			Sdl.LanguagePlatform.Core.Segment sourceSegment = ToLinguaSegment(pair.Source, _sourceCulture, file.Skeleton, out mapper);
			PersistentObjectToken resourceId = CreateSegmentPairResourceId(pair);
			TranslationUnit translationUnit = new TranslationUnit
			{
				ConfirmationLevel = confirmationLevelOrDefault,
				ResourceId = resourceId,
				SourceSegment = sourceSegment,
				TargetSegment = pair.Target?.ToLinguaSegment(_targetCulture, file.Skeleton)
			};
			UpdateTuContexts(pair, translationUnit);
			UpdateTuSystemFields(pair, translationUnit, includeUserNameSystemFields);
			UpdateTuCustomFields(pair, translationUnit);
			UpdateTuTranslationOrigin(pair, translationUnit);
			UpdateTuAlignmentTimestamp(pair, translationUnit);
			UpdateTuStructureContext(translationUnit, pair, file, puTmStructureContextDefinitionIds);
			UpdateTuSidContext(translationUnit, pair, file, puSidContextDefinitionIds);
			ConvertTokens(includeTokens, pair, translationUnit);
			ConvertAlignmentData(includeAlignmentData, pair, translationUnit);
			if (translationUnit.SourceSegment.Elements.Any() || (translationUnit.TargetSegment?.Elements != null && translationUnit.TargetSegment.Elements.Any()))
			{
				translationUnit.Trim();
				_mapper.Add(mapper);
				OnTranslationUnitCreated(new BcmTuConversionEventArgs(pair, translationUnit));
				return translationUnit;
			}
			return null;
		}

		private static PersistentObjectToken CreateSegmentPairResourceId(SegmentPair pair)
		{
			PersistentObjectToken result = new PersistentObjectToken();
			Sdl.Core.Bcm.BcmModel.Segment target = pair.Target;
			if (target != null && !target.Metadata.Any())
			{
				return result;
			}
			int id = Convert.ToInt32(pair.Target?.GetMetadata("tuid") ?? "0");
			Guid guid = new Guid(pair.Target?.GetMetadata("tuguid") ?? Guid.Empty.ToString());
			return new PersistentObjectToken
			{
				Id = id,
				Guid = guid
			};
		}

		private static void ConvertTokens(bool includeTokens, SegmentPair pair, TranslationUnit tu)
		{
			if (includeTokens)
			{
				List<Token> tokens = pair.Source.Tokens;
				if (tokens != null && tokens.Count > 0)
				{
					tu.SourceSegment.Tokens = BcmTokenHelper.BcmToLinguaTokens(pair.Source.Tokens);
				}
				Sdl.Core.Bcm.BcmModel.Segment target = pair.Target;
				int num;
				if (target == null)
				{
					num = 0;
				}
				else
				{
					List<Token> tokens2 = target.Tokens;
					num = ((((tokens2 != null) ? new int?(tokens2.Count) : null) > 0) ? 1 : 0);
				}
				if (num != 0)
				{
					tu.TargetSegment.Tokens = BcmTokenHelper.BcmToLinguaTokens(pair.Target.Tokens);
				}
			}
		}

		private static void ConvertAlignmentData(bool includeAlignment, SegmentPair pair, TranslationUnit tu)
		{
			if (includeAlignment && pair.Target?.AlignmentData != null)
			{
				tu.AlignmentData = BcmToLinguaAlignmentHelper.BcmToLinguaAlignment(pair.Target.AlignmentData);
				tu.AlignModelDate = pair.Target.AlignmentData.ModelDate;
				tu.InsertDate = pair.Target.AlignmentData.ContentInsertDate;
			}
		}

		private static void UpdateTuTranslationOrigin(SegmentPair pair, TranslationUnit tu)
		{
			if (pair.Target?.TranslationOrigin?.OriginType != null)
			{
				switch (pair.Target.TranslationOrigin.OriginType)
				{
				case "TM":
				case "tm":
					tu.Origin = TranslationUnitOrigin.TM;
					break;
				case "MachineTranslation":
				case "mt":
					tu.Origin = TranslationUnitOrigin.MachineTranslation;
					break;
				case "AdaptiveMachineTranslation":
				case "amt":
					tu.Origin = TranslationUnitOrigin.AdaptiveMachineTranslation;
					break;
				case "Alignment":
				case "auto-aligned":
					tu.Origin = TranslationUnitOrigin.Alignment;
					break;
				case "ContextTM":
				case "document-match":
					tu.Origin = TranslationUnitOrigin.ContextTM;
					break;
				case "nmt":
					tu.Origin = TranslationUnitOrigin.Nmt;
					break;
				case "automatic-translation":
					tu.Origin = TranslationUnitOrigin.AutomaticTranslation;
					break;
				default:
					tu.Origin = TranslationUnitOrigin.Unknown;
					break;
				}
			}
		}

		private void UpdateTuContexts(SegmentPair pair, TranslationUnit tu)
		{
			string text = (from x in pair.Target?.Metadata
				where x.Key.ToLower() == "Contexts".ToLower()
				select x.Value).FirstOrDefault();
			if (text != null)
			{
				JsonSerializerSettings settings = new JsonSerializerSettings
				{
					Converters = new List<JsonConverter>
					{
						new SegmentElementJsonConverter()
					},
					ContractResolver = new AllPropertiesResolver()
				};
				List<TuContext> values = JsonConvert.DeserializeObject<List<TuContext>>(text, settings);
				tu.Contexts.AddRange(values);
			}
		}

		private void UpdateTuSystemFields(SegmentPair pair, TranslationUnit tu, bool includeUserNameSystemFields)
		{
			string text = (from x in pair.Target?.Metadata
				where x.Key.ToLower() == "SystemFields".ToLower()
				select x.Value).FirstOrDefault();
			if (text != null)
			{
				tu.SystemFields = JsonConvert.DeserializeObject<SystemFields>(text, new JsonSerializerSettings
				{
					ContractResolver = new AllPropertiesResolver(),
					Converters = new List<JsonConverter>
					{
						new SystemFieldsJsonConverter(includeUserNameSystemFields)
					}
				});
			}
			if (TryGetSystemFieldsFromOrigin(pair.Target?.TranslationOrigin, out SystemFields systemFields))
			{
				if (tu.SystemFields == null)
				{
					tu.SystemFields = new SystemFields();
				}
				tu.SystemFields.CreationUser = systemFields.CreationUser;
				tu.SystemFields.ChangeUser = systemFields.ChangeUser;
				tu.SystemFields.CreationDate = systemFields.CreationDate;
				tu.SystemFields.ChangeDate = systemFields.ChangeDate;
			}
			if (includeUserNameSystemFields && TryGetOptionalSystemFieldsFromOrigin(pair.Target?.TranslationOrigin, out SystemFields systemFields2))
			{
				if (tu.SystemFields == null)
				{
					tu.SystemFields = new SystemFields();
				}
				tu.SystemFields.CreationDisplayUsername = systemFields2.CreationDisplayUsername;
				tu.SystemFields.ChangeDisplayUsername = systemFields2.ChangeDisplayUsername;
				tu.SystemFields.UseDisplayUsername = systemFields2.UseDisplayUsername;
			}
		}

		private void UpdateTuCustomFields(SegmentPair pair, TranslationUnit tu)
		{
			string text = (from x in pair.Target?.Metadata
				where x.Key.ToLower() == "CustomFields".ToLower()
				select x.Value).FirstOrDefault();
			if (text != null)
			{
				JArray jArray = JArray.Parse(text);
				foreach (JToken item in jArray)
				{
					FieldValue fieldValue = JsonConvert.DeserializeObject<FieldValue>(item.ToString(), new JsonConverter[1]
					{
						new FieldsConverter()
					});
					if (fieldValue != null)
					{
						tu.FieldValues.Add(fieldValue);
					}
				}
			}
		}

		private void UpdateTuAlignmentTimestamp(SegmentPair pair, TranslationUnit tu)
		{
			tu.InsertDate = (GetDateTimeMetadata(pair.Target, "tuInsertDate", DateTimeStyles.None) ?? DateTime.MinValue);
		}

		private bool TryGetSystemFieldsFromOrigin(TranslationOrigin origin, out SystemFields systemFields)
		{
			systemFields = null;
			if (origin == null)
			{
				return false;
			}
			if (!origin.MetaDataContainsKey("created_on") && !origin.MetaDataContainsKey("created_by") && !origin.MetaDataContainsKey("modified_on") && !origin.MetaDataContainsKey("last_modified_by"))
			{
				return false;
			}
			systemFields = new SystemFields
			{
				CreationUser = origin.GetMetadata("created_by"),
				CreationDate = (GetDateTimeMetadata(origin, "created_on") ?? DateTime.MinValue),
				ChangeUser = origin.GetMetadata("last_modified_by"),
				ChangeDate = (GetDateTimeMetadata(origin, "modified_on") ?? DateTime.MinValue)
			};
			if (origin.MetaDataContainsKey("used_on"))
			{
				systemFields.UseDate = (GetDateTimeMetadata(origin, "used_on") ?? DateTime.MinValue);
			}
			if (origin.MetaDataContainsKey("used_by"))
			{
				systemFields.UseUser = origin.GetMetadata("used_by");
			}
			return true;
		}

		private bool TryGetOptionalSystemFieldsFromOrigin(TranslationOrigin origin, out SystemFields systemFields)
		{
			systemFields = null;
			if (origin == null)
			{
				return false;
			}
			if (!origin.MetaDataContainsKey("created_by_username") && !origin.MetaDataContainsKey("modified_by_username") && !origin.MetaDataContainsKey("use_by_username"))
			{
				return false;
			}
			systemFields = new SystemFields
			{
				CreationDisplayUsername = origin.GetMetadata("created_by_username"),
				ChangeDisplayUsername = origin.GetMetadata("modified_by_username"),
				UseDisplayUsername = origin.GetMetadata("use_by_username")
			};
			return true;
		}

		private DateTime? GetDateTimeMetadata(MetadataContainer container, string key, DateTimeStyles style = DateTimeStyles.AssumeUniversal)
		{
			if (container == null || key == null)
			{
				return null;
			}
			string metadata = container.GetMetadata(key);
			if (DateTime.TryParse(metadata, DateTimeFormatInfo.InvariantInfo, style, out DateTime result))
			{
				return result;
			}
			return null;
		}

		private void UpdateTuStructureContext(TranslationUnit tu, SegmentPair sp, File file, HashSet<int> puContextDefinitionIds)
		{
			string[] array = (from cd in GetContextDefinitions(sp, file, GetTmStructureContextDefinitions, puContextDefinitionIds)
				select cd.TypeId).ToArray();
			if (array.Any())
			{
				tu.StructureContexts = array;
			}
		}

		private void UpdateTuSidContext(TranslationUnit tu, SegmentPair sp, File file, HashSet<int> puContextDefinitionIds)
		{
			string text = (from cd in GetContextDefinitions(sp, file, GetSidContextDefinitions, puContextDefinitionIds)
				select cd.Description).FirstOrDefault();
			if (text != null)
			{
				tu.IdContexts.Add(text);
			}
		}

		private IEnumerable<FileSegmentPair> GetFileSegmentPairs()
		{
			if (_fragment == null)
			{
				return GetDocumentSegmentPairs();
			}
			return GetFragmentSegmentPairs();
		}

		private IEnumerable<FileSegmentPair> GetFragmentSegmentPairs()
		{
			File file;
			IEnumerable<SegmentPair> segmentPairs = _fragment.GetSegmentPairs(out file);
			foreach (SegmentPair item in segmentPairs)
			{
				yield return new FileSegmentPair
				{
					SegmentPair = item,
					File = file
				};
			}
		}

		private IEnumerable<FileSegmentPair> GetDocumentSegmentPairs()
		{
			foreach (File file in _inputDocument.Files)
			{
				foreach (ParagraphUnit paragraphUnit in file.ParagraphUnits)
				{
					if (!paragraphUnit.IsStructure)
					{
						foreach (SegmentPair segmentPair in paragraphUnit.SegmentPairs)
						{
							yield return new FileSegmentPair
							{
								SegmentPair = segmentPair,
								File = file
							};
						}
					}
				}
			}
		}

		private IEnumerable<ContextDefinition> GetContextDefinitions(SegmentPair sp, File file, Func<FileSkeleton, IEnumerable<ContextDefinition>> contextDefinitionsProvider, HashSet<int> puContextDefinitionIds)
		{
			if (_fragment == null)
			{
				return GetDocumentContextDefinitions(sp, file, contextDefinitionsProvider, puContextDefinitionIds);
			}
			return GetFragmentContextDefinitions(contextDefinitionsProvider);
		}

		private IEnumerable<ContextDefinition> GetFragmentContextDefinitions(Func<FileSkeleton, IEnumerable<ContextDefinition>> contextDefinitionsProvider)
		{
			return contextDefinitionsProvider(_fragment.Skeleton);
		}

		private IEnumerable<ContextDefinition> GetDocumentContextDefinitions(SegmentPair sp, File file, Func<FileSkeleton, IEnumerable<ContextDefinition>> contextDefinitionsProvider, HashSet<int> puContextDefinitionIds)
		{
			ParagraphUnit paragraphUnit = sp?.Source?.ParentParagraphUnit;
			if (paragraphUnit == null)
			{
				return new List<ContextDefinition>();
			}
			IEnumerable<ContextDefinition> source = contextDefinitionsProvider(file.Skeleton);
			HashSet<int> puContextDefinitions = puContextDefinitionIds ?? GetPuContextDefinitionIds(paragraphUnit, file, null);
			return source.Where((ContextDefinition cd) => puContextDefinitions.Contains(cd.Id));
		}

		private IEnumerable<ContextDefinition> GetTmStructureContextDefinitions(FileSkeleton fileSkeleton)
		{
			if (fileSkeleton?.ContextDefinitions == null)
			{
				return new List<ContextDefinition>();
			}
			if (_skeletonContextInfo != null)
			{
				return _skeletonContextInfo.Value[fileSkeleton.FileId].TmStrcutureContextDefinitions;
			}
			return fileSkeleton.ContextDefinitions.Where((ContextDefinition cd) => cd.IsTmStructureContext);
		}

		private IEnumerable<ContextDefinition> GetSidContextDefinitions(FileSkeleton fileSkeleton)
		{
			if (fileSkeleton?.ContextDefinitions == null)
			{
				return new List<ContextDefinition>();
			}
			if (_skeletonContextInfo != null)
			{
				return _skeletonContextInfo.Value[fileSkeleton.FileId].SidContextDefinitions;
			}
			return fileSkeleton.ContextDefinitions.Where((ContextDefinition cd) => cd.IsSidContext);
		}

		private HashSet<int> GetPuTmStructureContextDefinitionIds(ParagraphUnit pu, File file)
		{
			Lazy<Dictionary<string, SkeletonContextInfo>> skeletonContextInfo = _skeletonContextInfo;
			return GetPuContextDefinitionIds(pu, file, (skeletonContextInfo != null) ? skeletonContextInfo.Value[file.Id].TmStructureContexts : null);
		}

		private HashSet<int> GetPuSidContextDefinitionIds(ParagraphUnit pu, File file)
		{
			Lazy<Dictionary<string, SkeletonContextInfo>> skeletonContextInfo = _skeletonContextInfo;
			return GetPuContextDefinitionIds(pu, file, (skeletonContextInfo != null) ? skeletonContextInfo.Value[file.Id].SidContexts : null);
		}

		private HashSet<int> GetPuContextDefinitionIds(ParagraphUnit pu, File file, IEnumerable<Context> skeletonContexts)
		{
			HashSet<int> hashSet;
			if (pu.ContextList == null)
			{
				hashSet = new HashSet<int>();
			}
			else
			{
				if (skeletonContexts == null)
				{
					skeletonContexts = file.Skeleton.Contexts;
				}
				HashSet<int> contextSet = new HashSet<int>(pu.ContextList);
				hashSet = new HashSet<int>(from c in skeletonContexts
					where contextSet.Contains(c.Id)
					select c.ContextDefinitionId);
			}
			List<int> value;
			if (_skeletonContextInfo == null)
			{
				hashSet.UnionWith(GetStructureContextDefinitionIds(file, pu.StructureContextId));
			}
			else if (_skeletonContextInfo.Value[file.Id].ContextIdStructureContextDefinitionIds.TryGetValue(pu.StructureContextId, out value))
			{
				hashSet.UnionWith(value);
			}
			return hashSet;
		}

		private Sdl.LanguagePlatform.Core.Segment ToLinguaSegment(Sdl.Core.Bcm.BcmModel.Segment segment, CultureInfo culture, FileSkeleton fileSkeleton, out LinguaToBcmMap mapper)
		{
			List<SegmentElement> list = new List<SegmentElement>();
			BcmToSegmentElementVisitor bcmToSegmentElementVisitor = new BcmToSegmentElementVisitor(list, fileSkeleton)
			{
				CreateLinguaBcmMappings = _createMappings
			};
			bcmToSegmentElementVisitor.VisitSegment(segment);
			Sdl.LanguagePlatform.Core.Segment result = new Sdl.LanguagePlatform.Core.Segment
			{
				Culture = culture,
				Elements = list
			};
			mapper = bcmToSegmentElementVisitor.LinguaToBcmMap;
			return result;
		}

		protected virtual void OnTranslationUnitCreated(BcmTuConversionEventArgs e)
		{
			this.TranslationUnitCreated?.Invoke(this, e);
		}

		private void SetCultures(Document inputDocument)
		{
			_sourceCulture = GetCulture(inputDocument.SourceLanguageCode);
			_targetCulture = GetCulture(inputDocument.TargetLanguageCode);
		}

		private void SetCultures(Fragment inputFragment)
		{
			_sourceCulture = GetCulture(inputFragment.SourceLanguageCode);
			_targetCulture = GetCulture(inputFragment.TargetLanguageCode);
		}

		private CultureInfo GetCulture(string languageCode)
		{
			if (string.IsNullOrEmpty(languageCode))
			{
				return null;
			}
			return new CultureInfo(languageCode);
		}

		private static Sdl.Core.Globalization.ConfirmationLevel GetConfirmationLevelOrDefault(SegmentPair pair)
		{
			return (Sdl.Core.Globalization.ConfirmationLevel)(pair.Target?.ConfirmationLevel ?? Sdl.Core.Bcm.BcmModel.ConfirmationLevel.NotTranslated);
		}

		private Dictionary<string, SkeletonContextInfo> PrepareSkeletonContextInfo(Document document)
		{
			Dictionary<string, SkeletonContextInfo> skeletonContextDefinitions = new Dictionary<string, SkeletonContextInfo>();
			document.Files.ForEach(delegate(File file)
			{
				SkeletonContextInfo skeletonContextInfo = new SkeletonContextInfo();
				skeletonContextDefinitions.Add(file.Id, skeletonContextInfo);
				Dictionary<int, List<int>> dictionary = new Dictionary<int, List<int>>();
				if (file.Skeleton?.ContextDefinitions != null)
				{
					foreach (ContextDefinition item in file.Skeleton?.ContextDefinitions)
					{
						if (item.IsTmStructureContext)
						{
							skeletonContextInfo.TmStrcutureContextDefinitions.Add(item);
						}
						else if (item.IsSidContext)
						{
							skeletonContextInfo.SidContextDefinitions.Add(item);
						}
						if (item.IsStructureContext)
						{
							List<int> structureContextDefinitionIds = GetStructureContextDefinitionIds(file, item.Id);
							if (structureContextDefinitionIds.Count > 0)
							{
								dictionary[item.Id] = structureContextDefinitionIds;
							}
						}
					}
				}
				if (file.Skeleton?.Contexts != null)
				{
					foreach (Context context in file.Skeleton?.Contexts)
					{
						if (skeletonContextInfo.TmStrcutureContextDefinitions.FirstOrDefault((ContextDefinition cd) => cd.Id == context.ContextDefinitionId) != null)
						{
							skeletonContextInfo.TmStructureContexts.Add(context);
						}
						if (skeletonContextInfo.SidContextDefinitions.FirstOrDefault((ContextDefinition cd) => cd.Id == context.ContextDefinitionId) != null)
						{
							skeletonContextInfo.SidContexts.Add(context);
						}
						if (dictionary.TryGetValue(context.ContextDefinitionId, out List<int> value))
						{
							skeletonContextInfo.ContextIdStructureContextDefinitionIds[context.Id] = value;
						}
					}
				}
			});
			return skeletonContextDefinitions;
		}

		private List<int> GetStructureContextDefinitionIds(File file, int contextDefinitionId)
		{
			List<int> list = new List<int>();
			Context structureContext;
			for (structureContext = file?.Skeleton?.Contexts.FirstOrDefault((Context c) => c.Id == contextDefinitionId); structureContext != null; structureContext = file.Skeleton?.Contexts.FirstOrDefault((Context c) => c.Id == structureContext.ParentContextId))
			{
				list.Add(structureContext.ContextDefinitionId);
			}
			return list;
		}
	}
}
