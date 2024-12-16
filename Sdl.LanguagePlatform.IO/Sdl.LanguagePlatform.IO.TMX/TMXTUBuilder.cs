using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Sdl.LanguagePlatform.IO.TMX
{
	internal class TMXTUBuilder
	{
		private static HashSet<string> _SdlxIgnoreFields;

		private static Dictionary<string, TranslationUnitOrigin> _WorkbenchSpecialUserIDs;

		private Segment _CurrentSegment;

		private TranslationUnitFormat _Flavor = TranslationUnitFormat.Unknown;

		private header _TMXHeader;

		private TMXReaderSettings _Settings;

		private TranslationUnit _TUTemplate;

		private List<TranslationUnit> _TUs = new List<TranslationUnit>();

		private List<TuContext> _DeferredContexts = new List<TuContext>();

		private StringBuilder _TagContent;

		private int _NextUnanchoredPlaceholderId = -1;

		private static char[] _CommaSeparator;

		private static char[] _SemicolonSeparator;

		private CultureInfo _SourceCulture;

		private CultureInfo _TargetCulture;

		public Segment CurrentSegment => _CurrentSegment;

		public List<TranslationUnit> TranslationUnits => _TUs;

		static TMXTUBuilder()
		{
			_SdlxIgnoreFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			_SdlxIgnoreFields.Add("Context");
			_SdlxIgnoreFields.Add("Source File");
			_WorkbenchSpecialUserIDs = new Dictionary<string, TranslationUnitOrigin>(StringComparer.OrdinalIgnoreCase);
			_WorkbenchSpecialUserIDs.Add("ALIGN!", TranslationUnitOrigin.Alignment);
			_WorkbenchSpecialUserIDs.Add("TALIGN!", TranslationUnitOrigin.Alignment);
			_WorkbenchSpecialUserIDs.Add("MT!", TranslationUnitOrigin.MachineTranslation);
			_CommaSeparator = new char[1]
			{
				','
			};
			_SemicolonSeparator = new char[1]
			{
				';'
			};
		}

		public TMXTUBuilder(TranslationUnitFormat flavor, header tmxHeader, TMXReaderSettings settings)
		{
			_Settings = (settings ?? throw new ArgumentNullException("settings"));
			_Flavor = flavor;
			_TMXHeader = tmxHeader;
			_TUTemplate = new TranslationUnit();
			_TUs = new List<TranslationUnit>();
			_TUTemplate.Format = _Flavor;
		}

		public void AddNote(string n)
		{
			if (_Flavor != TranslationUnitFormat.TradosTranslatorsWorkbench)
			{
				_ = _Flavor;
				_ = 1;
			}
		}

		private bool AddFieldValue(string fieldName, FieldValueType type, string value)
		{
			return AddFieldValue(fieldName, type, value, null);
		}

		private bool AddFieldValue(string actualFieldName, FieldValueType actualFieldValueType, string actualFieldValue, char[] fieldValueSeparator)
		{
			actualFieldName = actualFieldName?.Trim();
			if (string.IsNullOrEmpty(actualFieldName))
			{
				return false;
			}
			if (string.IsNullOrEmpty(actualFieldValue))
			{
				return false;
			}
			string[] array = null;
			FieldIdentifier fieldIdentifier = new FieldIdentifier(actualFieldValueType, actualFieldName);
			FieldIdentifier fieldIdentifier2 = MapFieldIdentifier(fieldIdentifier);
			if (fieldIdentifier2 == null || string.IsNullOrEmpty(fieldIdentifier2.FieldName))
			{
				return false;
			}
			actualFieldName = Field.RemoveIllegalChars(fieldIdentifier2.FieldName);
			actualFieldValueType = fieldIdentifier2.FieldValueType;
			Field field = _Settings.Context.FieldDefinitions.Lookup(actualFieldName);
			if (field == null)
			{
				if (!_Settings.Context.MayAddNewFields)
				{
					return false;
				}
				field = _Settings.Context.FieldDefinitions.Add(actualFieldName, actualFieldValueType);
			}
			else if (field.ValueType != actualFieldValueType)
			{
				if ((field.ValueType != FieldValueType.MultipleString || actualFieldValueType != FieldValueType.SingleString) && (field.ValueType != FieldValueType.MultiplePicklist || actualFieldValueType != FieldValueType.SinglePicklist))
				{
					return false;
				}
				actualFieldValueType = field.ValueType;
			}
			if (actualFieldValueType == FieldValueType.MultiplePicklist || actualFieldValueType == FieldValueType.MultipleString)
			{
				if (fieldValueSeparator == null)
				{
					array = new string[1]
					{
						actualFieldValue
					};
				}
				else
				{
					array = actualFieldValue.Split(fieldValueSeparator, StringSplitOptions.RemoveEmptyEntries);
					if (array.Length == 0)
					{
						return false;
					}
				}
			}
			FieldValue fieldValue = _TUTemplate.FieldValues.LookupOrCreate(field.Name, field.ValueType);
			switch (actualFieldValueType)
			{
			case FieldValueType.SingleString:
			{
				string text4 = actualFieldValue.Trim();
				if (text4.Length > 0)
				{
					(fieldValue as SingleStringFieldValue).Value = text4;
				}
				break;
			}
			case FieldValueType.MultipleString:
			{
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					string text2 = array2[i].Trim();
					if (text2.Length > 0)
					{
						(fieldValue as MultipleStringFieldValue).Add(text2);
					}
				}
				break;
			}
			case FieldValueType.DateTime:
			{
				DateTime result2 = default(DateTime);
				TMXConversions.TryTMXToDateTime(actualFieldValue, out result2);
				(fieldValue as DateTimeFieldValue).Value = result2;
				break;
			}
			case FieldValueType.SinglePicklist:
			{
				string text3 = actualFieldValue.Trim();
				PicklistField picklistField2 = field as PicklistField;
				if (text3.Length != 0)
				{
					PicklistItem picklistItem2 = picklistField2.Picklist.Lookup(text3);
					if (picklistItem2 == null && _Settings.Context.MayAddNewFields)
					{
						picklistItem2 = picklistField2.Picklist.Add(text3);
					}
					if (picklistItem2 != null)
					{
						(fieldValue as SinglePicklistFieldValue).Value = picklistItem2;
					}
				}
				break;
			}
			case FieldValueType.MultiplePicklist:
			{
				PicklistField picklistField = field as PicklistField;
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					string text = array2[i].Trim();
					if (text.Length != 0)
					{
						PicklistItem picklistItem = picklistField.Picklist.Lookup(text);
						if (picklistItem == null && _Settings.Context.MayAddNewFields)
						{
							picklistItem = picklistField.Picklist.Add(text);
						}
						if (picklistItem != null)
						{
							(fieldValue as MultiplePicklistFieldValue).Add(picklistItem);
						}
					}
				}
				break;
			}
			case FieldValueType.Integer:
			{
				if (int.TryParse(actualFieldValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result))
				{
					(fieldValue as IntFieldValue).Value = result;
				}
				break;
			}
			}
			return true;
		}

		public void AddProperty(prop p)
		{
			AddProperty(p, null);
		}

		public void AddProperty(prop p, TMXStartOfInputEvent soi)
		{
			if (string.IsNullOrEmpty(p.Value))
			{
				return;
			}
			bool flag = false;
			if (soi != null)
			{
				flag = soi.IncludesContextContent;
			}
			if (_Flavor == TranslationUnitFormat.TradosTranslatorsWorkbench)
			{
				if (p.type.Length > 5)
				{
					string actualFieldName = p.type.Substring(5);
					if (p.type.StartsWith("Att::"))
					{
						AddFieldValue(actualFieldName, FieldValueType.MultiplePicklist, p.Value, _CommaSeparator);
					}
					else if (p.type.StartsWith("Txt::"))
					{
						AddFieldValue(actualFieldName, FieldValueType.MultipleString, p.Value, _CommaSeparator);
					}
				}
			}
			else if (_Flavor == TranslationUnitFormat.SDLTradosStudio2009)
			{
				if (p.type.Length <= 0)
				{
					return;
				}
				if (p.type.Equals(TM8Emitter.ContextFieldName, StringComparison.OrdinalIgnoreCase))
				{
					string[] array = p.Value.Split(_CommaSeparator, StringSplitOptions.RemoveEmptyEntries);
					if (array.Length == 2 && long.TryParse(array[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out long result) && long.TryParse(array[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out long result2) && (!flag || (result == 0L && result2 == 0L)))
					{
						_TUTemplate.Contexts.Add(new TuContext(result, result2));
					}
					return;
				}
				if (p.type.Equals(TM8Emitter.IdContextFieldName, StringComparison.OrdinalIgnoreCase))
				{
					if (!string.IsNullOrEmpty(p.Value))
					{
						_TUTemplate.IdContexts.Add(p.Value);
					}
					return;
				}
				if (p.type.Equals(TM8Emitter.ContextContentFieldName, StringComparison.OrdinalIgnoreCase))
				{
					string[] array2 = p.Value.Split(new string[1]
					{
						TM8Emitter.ContextSeparator
					}, StringSplitOptions.None);
					if (array2.Length == 4)
					{
						try
						{
							Segment segment = DeserializeSegment(array2[0], array2[1], null);
							Segment segment2 = DeserializeSegment(array2[2], array2[3], null);
							TuContext tuContext = new TuContext((segment != null) ? 1 : 0, (segment2 != null) ? 1 : 0);
							tuContext.Segment1 = segment;
							tuContext.Segment2 = segment2;
							_DeferredContexts.Add(tuContext);
						}
						catch (Exception)
						{
						}
					}
					return;
				}
				if (p.type.Equals(TM8Emitter.OriginFieldName, StringComparison.OrdinalIgnoreCase))
				{
					try
					{
						object obj = Enum.Parse(typeof(TranslationUnitOrigin), p.Value, ignoreCase: true);
						_TUTemplate.Origin = (TranslationUnitOrigin)obj;
					}
					catch
					{
					}
					return;
				}
				if (p.type.Equals(TM8Emitter.ConfirmationLevelFieldName, StringComparison.OrdinalIgnoreCase))
				{
					try
					{
						if (p.Value == "Unknown")
						{
							_TUTemplate.ConfirmationLevel = ConfirmationLevel.Unspecified;
						}
						else
						{
							object obj3 = Enum.Parse(typeof(ConfirmationLevel), p.Value, ignoreCase: true);
							_TUTemplate.ConfirmationLevel = (ConfirmationLevel)obj3;
						}
					}
					catch
					{
					}
					return;
				}
				if (p.type.Equals(TM8Emitter.FormatFieldName, StringComparison.OrdinalIgnoreCase))
				{
					try
					{
						object obj5 = Enum.Parse(typeof(TranslationUnitFormat), p.Value, ignoreCase: true);
						_TUTemplate.Format = (TranslationUnitFormat)obj5;
					}
					catch
					{
					}
					return;
				}
				if (p.type.Equals(TM8Emitter.LastUsedByFieldName, StringComparison.OrdinalIgnoreCase))
				{
					try
					{
						_TUTemplate.SystemFields.UseUser = p.Value;
					}
					catch
					{
					}
					return;
				}
				int num = p.type.LastIndexOf(':');
				if (num > 2)
				{
					string text = p.type.Substring(2, num - 2);
					string type = p.type.Substring(num + 1);
					_ = p.Value;
					FieldValueType typeFromString = GetTypeFromString(type);
					if (!string.IsNullOrEmpty(text) && typeFromString != 0)
					{
						AddFieldValue(text, typeFromString, p.Value);
					}
				}
			}
			else if (_Flavor == TranslationUnitFormat.SDLX)
			{
				if (!p.type.StartsWith("x-") || p.type.Length <= 6 || p.type[5] != ':')
				{
					return;
				}
				string text2 = p.type.Substring(6);
				if (_SdlxIgnoreFields.Contains(text2))
				{
					return;
				}
				string text3 = p.type.Substring(2, 3);
				if (text3 == null)
				{
					return;
				}
				if (!(text3 == "ALS"))
				{
					if (!(text3 == "ALM"))
					{
						if (!(text3 == "TXT"))
						{
							if (!(text3 == "DAT"))
							{
								if (text3 == "NUM")
								{
									AddFieldValue(text2, FieldValueType.Integer, p.Value);
								}
							}
							else
							{
								AddFieldValue(text2, FieldValueType.DateTime, p.Value);
							}
						}
						else
						{
							AddFieldValue(text2, FieldValueType.SingleString, p.Value, _SemicolonSeparator);
						}
					}
					else
					{
						AddFieldValue(text2, FieldValueType.MultiplePicklist, p.Value);
					}
				}
				else
				{
					AddFieldValue(text2, FieldValueType.SinglePicklist, p.Value);
				}
			}
			else if (_Flavor == TranslationUnitFormat.IdiomWorldServer)
			{
				if (p.type != null && p.type.StartsWith("x-idiom-tm-uda-", StringComparison.OrdinalIgnoreCase))
				{
					string text4 = p.type.Substring("x-idiom-tm-uda-".Length);
					if (!string.IsNullOrEmpty(text4))
					{
						AddFieldValue(text4, FieldValueType.MultipleString, p.Value);
					}
				}
			}
			else if (_TMXHeader != null && string.Equals(_TMXHeader.otmf, "TAUS TM v1.0.0", StringComparison.OrdinalIgnoreCase) && p.type != null && p.type.StartsWith("tda-", StringComparison.OrdinalIgnoreCase))
			{
				string fieldName = p.type;
				if (p.type.Equals("tda-org", StringComparison.OrdinalIgnoreCase))
				{
					fieldName = "Organization";
				}
				else if (p.type.Equals("tda-industry", StringComparison.OrdinalIgnoreCase))
				{
					fieldName = "Industry";
				}
				else if (p.type.Equals("tda-prod"))
				{
					fieldName = "Product";
				}
				else if (p.type.Equals("tda-type"))
				{
					fieldName = "DocType";
				}
				AddFieldValue(fieldName, FieldValueType.MultipleString, p.Value);
			}
		}

		private Segment DeserializeSegment(string segmentText, string base64Binary, CultureInfo culture)
		{
			string oldValue = TM8Emitter.ContextSeparatorChar + TM8Emitter.ContextSeparatorChar;
			segmentText = segmentText.Replace(oldValue, TM8Emitter.ContextSeparatorChar);
			if (segmentText.Length == 0 && base64Binary.Length == 0)
			{
				return null;
			}
			byte[] array = Convert.FromBase64String(base64Binary);
			if (array.Length == 0)
			{
				array = null;
			}
			return SegmentSerializer.Load(segmentText, array, culture);
		}

		public void AddSegmentNote(string n)
		{
		}

		public void AddSegmentProperty(prop p)
		{
			if (_Flavor == TranslationUnitFormat.SDLX)
			{
				AddProperty(p);
			}
		}

		public string NormalizeSpaceAfterTagRemove(string text)
		{
			string text2 = CurrentSegment.ToPlain();
			if (text2.Equals(string.Empty))
			{
				return text;
			}
			bool flag = char.IsWhiteSpace(text2[text2.Length - 1]);
			bool flag2 = char.IsWhiteSpace(text[0]);
			if (flag && flag2)
			{
				text = text.TrimStart(" ".ToCharArray());
				flag2 = false;
			}
			if (!"ja-JP zh-CN zh-HK zh-MO zh-SG zh-TW".Contains(_CurrentSegment.CultureName))
			{
				bool flag3 = "fr-BE fr-CA fr-CH fr-FR fr-LU fr-MC".Contains(_CurrentSegment.CultureName) && ":!?;".Contains(Convert.ToString(text[0]));
				if (!flag && !flag2 && (!char.IsPunctuation(text[0]) | flag3))
				{
					text = " " + text;
				}
				if (flag && text.Length > 0 && char.IsPunctuation(text[0]) && !flag3)
				{
					CurrentSegment.TrimEnd();
				}
				flag3 = (text.Length > 1 && "fr-BE fr-CA fr-CH fr-FR fr-LU fr-MC".Contains(_CurrentSegment.CultureName) && ":!?;".Contains(Convert.ToString(text[1])));
				if (flag2 && text.Length > 1 && char.IsPunctuation(text[1]) && !flag3)
				{
					text = text.TrimStart(" ".ToCharArray());
				}
			}
			return text;
		}

		public void AddSegmentText(string text, bool isWhitespace, bool istagskipped)
		{
			if (_CurrentSegment == null)
			{
				throw new LanguagePlatformException(ErrorCode.TMXNoSegmentOpen);
			}
			if (!isWhitespace || _CurrentSegment.Elements.Count > 0)
			{
				if (istagskipped)
				{
					text = NormalizeSpaceAfterTagRemove(text);
				}
				_CurrentSegment.Add(text);
			}
		}

		public void AddSegmentTag(Tag t)
		{
			if (_CurrentSegment == null)
			{
				throw new LanguagePlatformException(ErrorCode.TMXNoSegmentOpen);
			}
			if (!_Settings.PlainText)
			{
				if (t.Anchor <= 0 && (t.Type == TagType.Standalone || t.Type == TagType.LockedContent || t.Type == TagType.TextPlaceholder || t.Type == TagType.UnmatchedStart || t.Type == TagType.UnmatchedEnd))
				{
					t.Anchor = _NextUnanchoredPlaceholderId;
					_NextUnanchoredPlaceholderId--;
				}
				if (_Flavor == TranslationUnitFormat.SDLItd || _Flavor == TranslationUnitFormat.SDLX)
				{
					SDLXCleaner.MapSDLXTagTypes(t);
				}
				_CurrentSegment.Add(t);
			}
		}

		public void AddTagContent(string tagContent)
		{
			if (_Flavor == TranslationUnitFormat.TradosTranslatorsWorkbench || _Flavor == TranslationUnitFormat.Unknown)
			{
				if (_TagContent == null)
				{
					_TagContent = new StringBuilder();
				}
				_TagContent.Append(tagContent);
			}
		}

		public void CloseTag()
		{
			if (_TagContent != null && (_Flavor == TranslationUnitFormat.TradosTranslatorsWorkbench || _Flavor == TranslationUnitFormat.Unknown))
			{
				Tag tag = _CurrentSegment.LastElement as Tag;
				if (tag != null && _TagContent.Length > 0)
				{
					_CurrentSegment.LastElement = new ContentBearingTag(tag, _TagContent.ToString());
				}
				_TagContent = null;
			}
		}

		private void ChangeLastElementToText(string replacement)
		{
			int num = _CurrentSegment.Elements.Count - 1;
			if (num > 0 && _CurrentSegment.Elements[num - 1] is Text)
			{
				(_CurrentSegment.Elements[num - 1] as Text).Value += replacement;
				_CurrentSegment.Elements.RemoveAt(num);
			}
			else
			{
				_CurrentSegment.Elements[num] = new Text(replacement);
			}
		}

		public void OpenSegment(string lang)
		{
			if ((_Flavor == TranslationUnitFormat.SDLX || _Flavor == TranslationUnitFormat.SDLItd) && string.Equals(lang, "pt", StringComparison.OrdinalIgnoreCase))
			{
				lang = "pt-PT";
			}
			CultureInfo cultureInfo = CultureInfoExtensions.GetCultureInfo(lang, _Settings.SkipUnknownCultures);
			if (cultureInfo != null && _Settings.ResolveNeutralCultures && cultureInfo.IsNeutralCulture)
			{
				cultureInfo = CultureInfoExtensions.GetRegionQualifiedCulture(cultureInfo);
			}
			if (cultureInfo == null)
			{
				if (!_Settings.SkipUnknownCultures)
				{
					throw new LanguagePlatformException(ErrorCode.UndefinedOrInvalidLanguage);
				}
				cultureInfo = CultureInfo.InvariantCulture;
			}
			_CurrentSegment = new Segment(cultureInfo);
		}

		private void CleanCurrentSegment()
		{
			_CurrentSegment.Trim();
			switch (_Flavor)
			{
			case TranslationUnitFormat.SDLTradosStudio2009:
				TransformTextPlaceholderTags(_CurrentSegment);
				break;
			case TranslationUnitFormat.SDLX:
				SDLXCleaner.ApplyStandardCleanupHeuristics(_CurrentSegment);
				break;
			case TranslationUnitFormat.TradosTranslatorsWorkbench:
				WorkbenchCleaner.RebracketMIFPlaceholders(_CurrentSegment);
				WorkbenchCleaner.FixMistaggings(_CurrentSegment);
				break;
			}
			_CurrentSegment.AnchorDanglingTags();
		}

		public bool CloseSegment()
		{
			return CloseSegment(null);
		}

		public bool CloseSegment(TMXStartOfInputEvent soi)
		{
			bool result = false;
			if (_CurrentSegment.Culture != CultureInfo.InvariantCulture)
			{
				CleanCurrentSegment();
				bool flag = IsCompatible(_CurrentSegment.Culture, _Settings.Context.SourceCulture);
				bool flag2 = IsCompatible(_CurrentSegment.Culture, _Settings.Context.TargetCulture);
				if (flag && flag2)
				{
					if (_TUTemplate.SourceSegment == null)
					{
						_TUTemplate.SourceSegment = _CurrentSegment;
					}
					else if (_TUTemplate.TargetSegment == null)
					{
						_TUTemplate.TargetSegment = _CurrentSegment;
					}
					else
					{
						_TUTemplate.TargetSegment = _CurrentSegment;
					}
				}
				else if (flag)
				{
					_TUTemplate.SourceSegment = _CurrentSegment;
				}
				else if (flag2)
				{
					_TUTemplate.TargetSegment = _CurrentSegment;
				}
				if ((flag | flag2) && _TUTemplate.SourceSegment != null && _TUTemplate.TargetSegment != null)
				{
					TranslationUnit translationUnit = new TranslationUnit(_TUTemplate);
					EnhanceAlignmentUsingTagContents(translationUnit);
					MapContentBearingTags(translationUnit);
					translationUnit.Trim();
					if (soi != null)
					{
						if (_SourceCulture == null && translationUnit.SourceSegment != null)
						{
							_SourceCulture = translationUnit.SourceSegment.Culture;
						}
						if (_TargetCulture == null && translationUnit.TargetSegment != null)
						{
							_TargetCulture = translationUnit.TargetSegment.Culture;
						}
						if (_DeferredContexts.Count > 0)
						{
							List<TuContext> list = new List<TuContext>();
							if (soi.TextContextMatchType == TextContextMatchType.PrecedingAndFollowingSource)
							{
								if (_SourceCulture != null)
								{
									foreach (TuContext deferredContext in _DeferredContexts)
									{
										if (deferredContext.Segment1 != null)
										{
											deferredContext.Segment1.Culture = _SourceCulture;
										}
										if (deferredContext.Segment2 != null)
										{
											deferredContext.Segment2.Culture = _SourceCulture;
										}
										list.Add(deferredContext);
									}
								}
							}
							else
							{
								foreach (TuContext deferredContext2 in _DeferredContexts)
								{
									if (deferredContext2.Segment1 != null)
									{
										deferredContext2.Segment1.Culture = _SourceCulture;
									}
									if (deferredContext2.Segment2 != null)
									{
										deferredContext2.Segment2.Culture = _TargetCulture;
									}
									list.Add(deferredContext2);
								}
								list = list.FindAll((TuContext x) => (x.Segment1 == null || x.Segment1.Culture != null) && (x.Segment2 == null || x.Segment2.Culture != null));
							}
							translationUnit.Contexts.AddRange(list);
						}
					}
					TranslationUnit translationUnit2 = null;
					TranslationUnit translationUnit3 = null;
					bool flag3 = false;
					bool flag4 = false;
					if (_Settings.CleanupMode != ImportSettings.ImportTUProcessingMode.ProcessRawTUOnly)
					{
						translationUnit2 = new TranslationUnit(_TUTemplate);
						switch (_Flavor)
						{
						case TranslationUnitFormat.TradosTranslatorsWorkbench:
							flag3 = WorkbenchCleaner.ApplyCleanupHeuristics(translationUnit2);
							translationUnit3 = new TranslationUnit(translationUnit2);
							flag4 = WorkbenchCleaner.ConvertEmptyTagPairsToLockedContentPlaceholders(translationUnit3);
							if (flag4)
							{
								EnhanceAlignmentUsingTagContents(translationUnit3);
								MapContentBearingTags(translationUnit3);
								translationUnit3.Trim();
							}
							break;
						case TranslationUnitFormat.SDLX:
							flag3 = SDLXCleaner.ApplyExtendedCleanupHeuristics(translationUnit2);
							break;
						case TranslationUnitFormat.IdiomWorldServer:
							flag3 = WSCleaner.ApplyCleanupHeuristics(translationUnit2);
							break;
						default:
							flag3 = GenericCleaner.ApplyCleanupHeuristics(translationUnit2);
							break;
						case TranslationUnitFormat.SDLTradosStudio2009:
							break;
						}
						if (flag3)
						{
							EnhanceAlignmentUsingTagContents(translationUnit2);
							MapContentBearingTags(translationUnit2);
							translationUnit2.Trim();
						}
					}
					if (!flag3 && !flag4)
					{
						_TUs.Add(translationUnit);
					}
					else
					{
						if (flag3)
						{
							_TUs.Add(translationUnit2);
						}
						if (flag4)
						{
							_TUs.Add(translationUnit3);
						}
						if (_Settings.CleanupMode == ImportSettings.ImportTUProcessingMode.ProcessBothTUs)
						{
							_TUs.Add(translationUnit);
						}
					}
					result = true;
					_TUTemplate.FieldValues.Clear();
					_DeferredContexts.Clear();
				}
			}
			Reset();
			return result;
		}

		private void Reset()
		{
			_CurrentSegment = null;
			_NextUnanchoredPlaceholderId = -1;
		}

		private static void MapContentBearingTags(Segment segment)
		{
			for (int i = 0; i < segment.Elements.Count; i++)
			{
				ContentBearingTag contentBearingTag = segment.Elements[i] as ContentBearingTag;
				if (contentBearingTag != null)
				{
					segment.Elements[i] = new Tag(contentBearingTag);
				}
			}
		}

		private static void MapContentBearingTags(TranslationUnit tu)
		{
			MapContentBearingTags(tu.SourceSegment);
			MapContentBearingTags(tu.TargetSegment);
		}

		private static void EnhanceAlignmentUsingTagContents(TranslationUnit tu)
		{
			int maxAlignmentAnchor;
			List<ContentBearingTag> list = CollectContentBearingTags(tu.SourceSegment, out maxAlignmentAnchor);
			int maxAlignmentAnchor2;
			List<ContentBearingTag> list2 = CollectContentBearingTags(tu.TargetSegment, out maxAlignmentAnchor2);
			if (list == null || list.Count == 0 || list2 == null || list2.Count == 0)
			{
				return;
			}
			int num = Math.Max(maxAlignmentAnchor, maxAlignmentAnchor2) + 1;
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i] == null || list[i].AlignmentAnchor != 0 || string.IsNullOrEmpty(list[i].Content))
				{
					continue;
				}
				for (int j = 0; j < list2.Count; j++)
				{
					if (list2[j] != null && list2[j].AlignmentAnchor == 0 && !string.IsNullOrEmpty(list2[j].Content) && list[i].Type == list2[j].Type && string.Equals(list[i].Content, list2[j].Content))
					{
						list[i].AlignmentAnchor = num;
						list2[j].AlignmentAnchor = num;
						num++;
						list[i] = null;
						list2[j] = null;
						break;
					}
				}
			}
		}

		private static List<ContentBearingTag> CollectContentBearingTags(Segment s, out int maxAlignmentAnchor)
		{
			List<ContentBearingTag> list = null;
			maxAlignmentAnchor = 0;
			for (int i = 0; i < s.Elements.Count; i++)
			{
				Tag tag = s.Elements[i] as Tag;
				if (tag == null)
				{
					continue;
				}
				if (tag.AlignmentAnchor > maxAlignmentAnchor)
				{
					maxAlignmentAnchor = tag.AlignmentAnchor;
				}
				if (tag is ContentBearingTag && (tag.Type == TagType.Standalone || tag.Type == TagType.LockedContent || tag.Type == TagType.Start || tag.Type == TagType.TextPlaceholder))
				{
					if (list == null)
					{
						list = new List<ContentBearingTag>();
					}
					list.Add(tag as ContentBearingTag);
				}
			}
			return list;
		}

		private bool IsCompatible(CultureInfo segmentCulture, CultureInfo tmCulture)
		{
			if (segmentCulture == null || tmCulture == null)
			{
				return false;
			}
			if (object.Equals(tmCulture, CultureInfo.InvariantCulture))
			{
				return true;
			}
			if (segmentCulture == tmCulture)
			{
				return true;
			}
			if (_Settings.Context.CheckMatchingSublanguages)
			{
				return false;
			}
			if (segmentCulture.TwoLetterISOLanguageName == tmCulture.TwoLetterISOLanguageName)
			{
				return true;
			}
			return false;
		}

		public void AddAttribute(string key, string value)
		{
			DateTime result = default(DateTime);
			switch (key)
			{
			case "tuid":
				break;
			case "o-encoding":
				break;
			case "datatype":
				break;
			case "creationtool":
				break;
			case "creationtoolversion":
				break;
			case "segtype":
				break;
			case "o-tmf":
				break;
			case "srclang":
				break;
			case "usagecount":
			{
				if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result2))
				{
					_TUTemplate.SystemFields.UseCount = result2;
				}
				break;
			}
			case "lastusagedate":
				if (TMXConversions.TryTMXToDateTime(value, out result))
				{
					_TUTemplate.SystemFields.UseDate = result;
				}
				break;
			case "creationdate":
				if (TMXConversions.TryTMXToDateTime(value, out result))
				{
					_TUTemplate.SystemFields.CreationDate = result;
				}
				break;
			case "creationid":
				_TUTemplate.SystemFields.CreationUser = value;
				if (_Flavor == TranslationUnitFormat.TradosTranslatorsWorkbench)
				{
					SetOriginFromUserID(value);
				}
				break;
			case "changedate":
				if (TMXConversions.TryTMXToDateTime(value, out result))
				{
					_TUTemplate.SystemFields.ChangeDate = result;
				}
				break;
			case "changeid":
				_TUTemplate.SystemFields.ChangeUser = value;
				break;
			default:
				throw new LanguagePlatformException(ErrorCode.TMXUnknownTMXAttribute);
			}
		}

		private void SetOriginFromUserID(string uid)
		{
			if (!string.IsNullOrEmpty(uid) && _WorkbenchSpecialUserIDs.TryGetValue(uid, out TranslationUnitOrigin value))
			{
				_TUTemplate.Origin = value;
			}
		}

		private void TransformTextPlaceholderTags(Segment segment)
		{
			if (segment?.Elements == null)
			{
				return;
			}
			for (int i = 0; i < segment.Elements.Count; i++)
			{
				Tag tag = segment.Elements[i] as Tag;
				if (tag == null || tag.Type != TagType.Start)
				{
					continue;
				}
				bool flag = string.CompareOrdinal(tag.TagID, 0, TMXReader.LockedContentTagID, 0, TMXReader.LockedContentTagID.Length) == 0;
				if (!((string.CompareOrdinal(tag.TagID, 0, TMXReader.TextPlaceholderTagID, 0, TMXReader.TextPlaceholderTagID.Length) == 0) | flag))
				{
					continue;
				}
				string text = (!flag) ? tag.TagID.Substring(TMXReader.TextPlaceholderTagID.Length) : tag.TagID.Substring(TMXReader.LockedContentTagID.Length);
				tag.TagID = ((text.Length > 0) ? text : null);
				string text2 = null;
				bool flag2 = false;
				int num = 0;
				for (num = i + 1; num < segment.Elements.Count; num++)
				{
					if (flag2)
					{
						break;
					}
					if (segment.Elements[num] is Text)
					{
						text2 = ((text2 != null) ? (text2 + ((Text)segment.Elements[num]).Value) : ((Text)segment.Elements[num]).Value);
					}
					else
					{
						if (!(segment.Elements[num] is Tag))
						{
							continue;
						}
						Tag tag2 = segment.Elements[num] as Tag;
						if (tag2 != null && tag2.Anchor == tag.Anchor && tag2.Type == TagType.End)
						{
							if (flag)
							{
								tag.Type = TagType.LockedContent;
							}
							else
							{
								tag.Type = TagType.TextPlaceholder;
							}
							tag.TextEquivalent = text2;
							segment.Elements.RemoveRange(i + 1, num - i);
							flag2 = true;
						}
					}
				}
			}
		}

		public void AddContext(TuContext tuc)
		{
			if (_Flavor == TranslationUnitFormat.SDLTradosStudio2009)
			{
				_TUTemplate.Contexts.Add(tuc);
			}
		}

		public static string GetTypeName(FieldValueType type)
		{
			switch (type)
			{
			case FieldValueType.SingleString:
				return "SingleString";
			case FieldValueType.MultipleString:
				return "MultipleString";
			case FieldValueType.DateTime:
				return "DateTime";
			case FieldValueType.SinglePicklist:
				return "SinglePicklist";
			case FieldValueType.MultiplePicklist:
				return "MultiplePicklist";
			case FieldValueType.Integer:
				return "Integer";
			default:
				return null;
			}
		}

		public static FieldValueType GetType(string value)
		{
			switch (value.ToLower())
			{
			case "singlestring":
				return FieldValueType.SingleString;
			case "multiplestring":
				return FieldValueType.MultipleString;
			case "datetime":
				return FieldValueType.DateTime;
			case "singlepicklist":
				return FieldValueType.SinglePicklist;
			case "multiplepicklist":
				return FieldValueType.MultiplePicklist;
			case "integer":
				return FieldValueType.Integer;
			default:
				return FieldValueType.Unknown;
			}
		}

		public static FieldValueType GetTypeFromString(string type)
		{
			if (type.Equals("SingleString", StringComparison.OrdinalIgnoreCase))
			{
				return FieldValueType.SingleString;
			}
			if (type.Equals("MultipleString", StringComparison.OrdinalIgnoreCase))
			{
				return FieldValueType.MultipleString;
			}
			if (type.Equals("DateTime", StringComparison.OrdinalIgnoreCase))
			{
				return FieldValueType.DateTime;
			}
			if (type.Equals("SinglePicklist", StringComparison.OrdinalIgnoreCase))
			{
				return FieldValueType.SinglePicklist;
			}
			if (type.Equals("MultiplePicklist", StringComparison.OrdinalIgnoreCase))
			{
				return FieldValueType.MultiplePicklist;
			}
			if (type.Equals("Integer", StringComparison.OrdinalIgnoreCase))
			{
				return FieldValueType.Integer;
			}
			return FieldValueType.Unknown;
		}

		private FieldIdentifier MapFieldIdentifier(FieldIdentifier fieldIdentifier)
		{
			return MapFieldIdentifier(_Settings.FieldIdentifierMappings, fieldIdentifier);
		}

		public static FieldIdentifier MapFieldIdentifier(IDictionary<FieldIdentifier, FieldIdentifier> fieldIdentifierMappings, FieldIdentifier fieldIdentifier)
		{
			if (fieldIdentifierMappings == null || !fieldIdentifierMappings.TryGetValue(fieldIdentifier, out FieldIdentifier value))
			{
				return fieldIdentifier;
			}
			return value;
		}
	}
}
