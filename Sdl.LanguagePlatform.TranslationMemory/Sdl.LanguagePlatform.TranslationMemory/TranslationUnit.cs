using Sdl.Core.FineGrainedAlignment.Core;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.LanguagePlatform.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	public class TranslationUnit : PersistentObject, ITypedKeyValueContainer
	{
		private const string ChangeDateFieldName = "chd";

		private const string ChangeUserFieldName = "chu";

		private const string UseDateFieldName = "usd";

		private const string UseUserFieldName = "usu";

		private const string UseCountFieldName = "usc";

		private const string CreationDateFieldName = "crd";

		private const string CreationUserFieldName = "cru";

		private const string OriginFieldName = "x-origin";

		private const string FormatFieldName = "x-format";

		private const string ConfirmationLevelFieldName = "confirmationlevel";

		[IgnoreDataMember]
		[XmlIgnore]
		public IDocumentProperties DocumentProperties
		{
			get;
			set;
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public IFileProperties FileProperties
		{
			get;
			set;
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public ISegmentPair DocumentSegmentPair
		{
			get;
			set;
		}

		[DataMember]
		public Segment SourceSegment
		{
			get;
			set;
		}

		[DataMember]
		public Segment TargetSegment
		{
			get;
			set;
		}

		[DataMember]
		public TuContexts Contexts
		{
			get;
			set;
		}

		[DataMember]
		public TuIdContexts IdContexts
		{
			get;
			set;
		}

		[DataMember]
		public LiftAlignedSpanPairSet AlignmentData
		{
			get;
			set;
		}

		[DataMember]
		public DateTime? AlignModelDate
		{
			get;
			set;
		}

		[DataMember]
		public DateTime? InsertDate
		{
			get;
			set;
		}

		[DataMember]
		public TranslationUnitOrigin Origin
		{
			get;
			set;
		}

		[DataMember]
		public ConfirmationLevel ConfirmationLevel
		{
			get;
			set;
		}

		[DataMember]
		public TranslationUnitFormat Format
		{
			get;
			set;
		}

		[DataMember]
		public SystemFields SystemFields
		{
			get;
			set;
		}

		[DataMember]
		public FieldValues FieldValues
		{
			get;
			set;
		}

		[IgnoreDataMember]
		public string[] StructureContexts
		{
			get
			{
				MultipleStringFieldValue multipleStringFieldValue = FieldValues[Field.StructureContextFieldName] as MultipleStringFieldValue;
				if (multipleStringFieldValue != null)
				{
					return multipleStringFieldValue.Values.ToArray();
				}
				return new string[0];
			}
			set
			{
				MultipleStringFieldValue multipleStringFieldValue = FieldValues[Field.StructureContextFieldName] as MultipleStringFieldValue;
				if (multipleStringFieldValue == null)
				{
					multipleStringFieldValue = new MultipleStringFieldValue(Field.StructureContextFieldName);
					FieldValues.Add(multipleStringFieldValue);
				}
				multipleStringFieldValue.Values = value;
			}
		}

		public TranslationUnit()
			: this(null, null)
		{
		}

		public TranslationUnit(Segment sourceSegment, Segment targetSegment)
		{
			SystemFields = new SystemFields();
			FieldValues = new FieldValues();
			Contexts = new TuContexts();
			IdContexts = new TuIdContexts();
			SourceSegment = sourceSegment;
			TargetSegment = targetSegment;
		}

		public TranslationUnit(TranslationUnit other)
		{
			SystemFields = new SystemFields(other.SystemFields);
			FieldValues = new FieldValues(other.FieldValues);
			if (other.Contexts != null)
			{
				Contexts = new TuContexts(other.Contexts);
			}
			if (other.IdContexts != null)
			{
				IdContexts = new TuIdContexts(other.IdContexts);
			}
			Origin = other.Origin;
			Format = other.Format;
			ConfirmationLevel = other.ConfirmationLevel;
			SourceSegment = other.SourceSegment.Duplicate();
			TargetSegment = other.TargetSegment.Duplicate();
			AlignModelDate = other.AlignModelDate;
			if (other.AlignmentData != null)
			{
				AlignmentData = new LiftAlignedSpanPairSet(other.AlignmentData.Save());
			}
			InsertDate = other.InsertDate;
		}

		public TranslationUnit Duplicate()
		{
			return new TranslationUnit(this);
		}

		public void DeleteTags()
		{
			DeleteTags(Segment.DeleteTagsAction.KeepTextPlaceholders);
		}

		public void DeleteTags(Segment.DeleteTagsAction action)
		{
			SourceSegment?.DeleteTags(action);
			TargetSegment?.DeleteTags(action);
		}

		public ErrorCode Validate()
		{
			return Validate(Segment.ValidationMode.ReportAllErrors);
		}

		public ErrorCode Validate(Segment.ValidationMode mode)
		{
			if (SystemFields == null)
			{
				return ErrorCode.Other;
			}
			if (SourceSegment == null)
			{
				return ErrorCode.EmptySourceSegment;
			}
			if (TargetSegment == null)
			{
				return ErrorCode.EmptyTargetSegment;
			}
			ErrorCode errorCode = SourceSegment.Validate(mode);
			if (errorCode != 0)
			{
				return errorCode;
			}
			errorCode = TargetSegment.Validate(mode);
			if (errorCode == ErrorCode.OK)
			{
				return ErrorCode.OK;
			}
			return errorCode;
		}

		public bool IsValid()
		{
			return Validate() == ErrorCode.OK;
		}

		public int GetMaxTagAnchor()
		{
			int num = 0;
			if (SourceSegment != null)
			{
				num = SourceSegment.GetMaxTagAnchor();
			}
			if (TargetSegment != null)
			{
				num = Math.Max(num, TargetSegment.GetMaxTagAnchor());
			}
			return num;
		}

		private static void CollectIntegerTagIDs(List<Tag> alignableTags, ISet<int> usedIDs)
		{
			if (alignableTags != null)
			{
				foreach (Tag alignableTag in alignableTags)
				{
					if (!string.IsNullOrEmpty(alignableTag.TagID) && int.TryParse(alignableTag.TagID, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result) && !usedIDs.Contains(result))
					{
						usedIDs.Add(result);
					}
				}
			}
		}

		private static void AssignTagIDs(List<Tag> alignableTags, ISet<int> usedIDs, ref int nextId)
		{
			if (alignableTags != null)
			{
				foreach (Tag alignableTag in alignableTags)
				{
					if (string.IsNullOrEmpty(alignableTag.TagID))
					{
						do
						{
							nextId++;
						}
						while (usedIDs.Contains(nextId));
						usedIDs.Add(nextId);
						alignableTag.TagID = nextId.ToString(CultureInfo.InvariantCulture);
					}
				}
			}
		}

		private static void AssignTagIDs(List<Tag> srcAlignableTags, List<Tag> trgAlignableTags, out HashSet<int> usedIDs)
		{
			usedIDs = new HashSet<int>();
			CollectIntegerTagIDs(srcAlignableTags, usedIDs);
			CollectIntegerTagIDs(trgAlignableTags, usedIDs);
			int nextId = 0;
			AssignTagIDs(srcAlignableTags, usedIDs, ref nextId);
			AssignTagIDs(trgAlignableTags, usedIDs, ref nextId);
		}

		public void CheckAndComputeTagAssociations()
		{
			List<Tag> alignableTags = GetAlignableTags(SourceSegment);
			List<Tag> alignableTags2 = GetAlignableTags(TargetSegment);
			int maxAlignmentAnchor = 0;
			if (alignableTags != null || alignableTags2 != null)
			{
				RenumberTagAnchors(out maxAlignmentAnchor);
			}
			AssignTagIDs(alignableTags, alignableTags2, out HashSet<int> usedIDs);
			if (alignableTags == null || alignableTags2 == null)
			{
				if (alignableTags2 != null)
				{
					ClearAlignmentAnchors(alignableTags2);
				}
				if (alignableTags != null)
				{
					ClearAlignmentAnchors(alignableTags);
				}
				return;
			}
			bool flag = false;
			foreach (Tag item in alignableTags)
			{
				if (item.AlignmentAnchor > 0)
				{
					if (FindAlignedTag(alignableTags2, item.AlignmentAnchor) == null)
					{
						item.AlignmentAnchor = 0;
					}
				}
				else
				{
					flag = true;
				}
			}
			foreach (Tag item2 in alignableTags2)
			{
				if (item2.AlignmentAnchor > 0)
				{
					if (FindAlignedTag(alignableTags, item2.AlignmentAnchor) == null)
					{
						item2.AlignmentAnchor = 0;
					}
				}
				else
				{
					flag = true;
				}
			}
			if (flag)
			{
				AlignUnalignedTags(alignableTags, alignableTags2, maxAlignmentAnchor);
			}
			EnsureConsistentAlignment(alignableTags, alignableTags2, usedIDs);
		}

		public bool GenerateAlignmentData()
		{
			if (AlignmentData != null)
			{
				return false;
			}
			if (SourceSegment?.Tokens == null)
			{
				return false;
			}
			if (TargetSegment?.Tokens == null)
			{
				return false;
			}
			AlignmentData = new LiftAlignedSpanPairSet((short)SourceSegment.Tokens.Count, (short)TargetSegment.Tokens.Count);
			PlaceableComputer.ConvertPlaceablesToAlignments(PlaceableComputer.ComputePlaceables(SourceSegment, TargetSegment), AlignmentData, SourceSegment.Tokens, TargetSegment.Tokens);
			return true;
		}

		private static void EnsureConsistentAlignment(IEnumerable<Tag> srcAlignableTags, ICollection<Tag> trgAlignableTags, IEnumerable<int> usedTagIDs)
		{
			int num = usedTagIDs.Concat(new int[1]).Max();
			foreach (Tag srcAlignableTag in srcAlignableTags)
			{
				Tag tag = FindAlignedTag(trgAlignableTags, srcAlignableTag.AlignmentAnchor);
				if (tag != null && (srcAlignableTag.TagID == null || tag.TagID == null || !string.Equals(srcAlignableTag.TagID, tag.TagID, StringComparison.Ordinal)))
				{
					if (!string.IsNullOrEmpty(srcAlignableTag.TagID))
					{
						tag.TagID = srcAlignableTag.TagID;
					}
					else if (!string.IsNullOrEmpty(tag.TagID))
					{
						srcAlignableTag.TagID = tag.TagID;
					}
					else
					{
						srcAlignableTag.TagID = num.ToString(CultureInfo.InvariantCulture);
						tag.TagID = srcAlignableTag.TagID;
						num++;
					}
				}
			}
		}

		private static void AlignUnalignedLockedContentTags(IEnumerable<Tag> srcAlignableTags, IList<Tag> trgAlignableTags, ref int maxAlignmentAnchor)
		{
			foreach (Tag srcAlignableTag in srcAlignableTags)
			{
				if (srcAlignableTag.AlignmentAnchor == 0 && srcAlignableTag.Type == TagType.LockedContent)
				{
					foreach (Tag trgAlignableTag in trgAlignableTags)
					{
						if (trgAlignableTag.AlignmentAnchor == 0 && trgAlignableTag.Type == TagType.LockedContent && !(srcAlignableTag.TextEquivalent != trgAlignableTag.TextEquivalent))
						{
							maxAlignmentAnchor++;
							srcAlignableTag.AlignmentAnchor = maxAlignmentAnchor;
							trgAlignableTag.AlignmentAnchor = srcAlignableTag.AlignmentAnchor;
							break;
						}
					}
				}
			}
		}

		private static void AlignUnalignedTags(IList<Tag> srcAlignableTags, IList<Tag> trgAlignableTags, int maxAlignmentAnchor)
		{
			AlignUnalignedLockedContentTags(srcAlignableTags, trgAlignableTags, ref maxAlignmentAnchor);
			foreach (Tag srcAlignableTag in srcAlignableTags)
			{
				if (srcAlignableTag.AlignmentAnchor == 0)
				{
					Tag tag = null;
					SegmentElement.Similarity similarity = SegmentElement.Similarity.None;
					foreach (Tag trgAlignableTag in trgAlignableTags)
					{
						if (trgAlignableTag.AlignmentAnchor == 0 && trgAlignableTag.Type == srcAlignableTag.Type && (tag == null || srcAlignableTag.GetSimilarity(trgAlignableTag) > similarity))
						{
							tag = trgAlignableTag;
							similarity = srcAlignableTag.GetSimilarity(trgAlignableTag);
						}
					}
					if (tag != null && (similarity > SegmentElement.Similarity.IdenticalType || TagIDsAreCompatible(srcAlignableTag, tag)))
					{
						maxAlignmentAnchor++;
						srcAlignableTag.AlignmentAnchor = maxAlignmentAnchor;
						tag.AlignmentAnchor = srcAlignableTag.AlignmentAnchor;
					}
				}
			}
		}

		private static bool TagIDsAreCompatible(Tag t1, Tag t2)
		{
			if (!string.IsNullOrEmpty(t1.TagID) && !string.IsNullOrEmpty(t2.TagID))
			{
				return string.Equals(t1.TagID, t2.TagID, StringComparison.OrdinalIgnoreCase);
			}
			return true;
		}

		private static Tag FindAlignedTag(ICollection<Tag> tags, int anchor)
		{
			if (tags == null || tags.Count == 0 || anchor <= 0)
			{
				return null;
			}
			return tags.FirstOrDefault((Tag t) => t.AlignmentAnchor == anchor);
		}

		private static void ClearAlignmentAnchors(IEnumerable<Tag> list)
		{
			foreach (Tag item in list)
			{
				item.AlignmentAnchor = 0;
			}
		}

		private void RenumberTagAnchors(out int maxAlignmentAnchor)
		{
			maxAlignmentAnchor = 0;
			SourceSegment?.RenumberTagAnchors(ref maxAlignmentAnchor);
			TargetSegment?.RenumberTagAnchors(ref maxAlignmentAnchor);
		}

		private static List<Tag> GetAlignableTags(Segment s)
		{
			if (s?.Elements == null || s.Elements.Count == 0)
			{
				return null;
			}
			List<Tag> list = null;
			foreach (SegmentElement element in s.Elements)
			{
				Tag tag = element as Tag;
				if (tag != null)
				{
					if (tag.Type == TagType.Start || tag.Type == TagType.Standalone || tag.Type == TagType.TextPlaceholder || tag.Type == TagType.LockedContent)
					{
						if (list == null)
						{
							list = new List<Tag>();
						}
						list.Add(tag);
					}
					else
					{
						tag.AlignmentAnchor = 0;
					}
				}
			}
			return list;
		}

		public void Trim()
		{
			SourceSegment?.Trim();
			TargetSegment?.Trim();
		}

		public FieldValue GetValue(string fieldName)
		{
			switch (fieldName.ToLower(CultureInfo.InvariantCulture))
			{
			case "chd":
				return new DateTimeFieldValue(fieldName, SystemFields.ChangeDate);
			case "chu":
				return new SingleStringFieldValue(fieldName, SystemFields.ChangeUser);
			case "usd":
				return new DateTimeFieldValue(fieldName, SystemFields.UseDate);
			case "usu":
				return new SingleStringFieldValue(fieldName, SystemFields.UseUser);
			case "usc":
				return new IntFieldValue(fieldName, SystemFields.UseCount);
			case "crd":
				return new DateTimeFieldValue(fieldName, SystemFields.CreationDate);
			case "cru":
				return new SingleStringFieldValue(fieldName, SystemFields.CreationUser);
			case "src":
				return new SingleStringFieldValue(fieldName, SourceSegment.ToString());
			case "trg":
				return new SingleStringFieldValue(fieldName, TargetSegment.ToString());
			case "sourceplainlength":
				return new IntFieldValue(fieldName, SourceSegment.ToPlain().Length);
			case "targetplainlength":
				return new IntFieldValue(fieldName, TargetSegment.ToPlain().Length);
			case "sourcetagcount":
				return new IntFieldValue(fieldName, SourceSegment.GetTagCount());
			case "targettagcount":
				return new IntFieldValue(fieldName, TargetSegment.GetTagCount());
			case "x-origin":
				return new SingleStringFieldValue(fieldName, Origin.ToString());
			case "x-format":
				return new SingleStringFieldValue(fieldName, Format.ToString());
			case "confirmationlevel":
				return new SinglePicklistFieldValue("confirmationlevel", new PicklistItem(ConfirmationLevel.ToString()));
			default:
				return FieldValues?.Lookup(fieldName);
			}
		}

		public void SetValue(FieldValue fv, bool addIfMissing)
		{
			if (fv == null)
			{
				throw new ArgumentNullException("fv");
			}
			if (string.IsNullOrEmpty(fv.Name))
			{
				throw new ArgumentException("fv.Name");
			}
			FieldType fieldType = Field.GetFieldType(fv.Name);
			if (fieldType != FieldType.System && fieldType != 0)
			{
				throw new LanguagePlatformException(ErrorCode.EditScriptIncompatibleField);
			}
			switch (fv.Name.ToLower(CultureInfo.InvariantCulture))
			{
			case "chd":
			{
				if (fv.ValueType != FieldValueType.DateTime)
				{
					throw new LanguagePlatformException(ErrorCode.EditScriptIncompatibleFieldValueTypes);
				}
				DateTimeFieldValue dateTimeFieldValue = fv as DateTimeFieldValue;
				if (dateTimeFieldValue == null)
				{
					throw new Exception("Internal error");
				}
				SystemFields.ChangeDate = dateTimeFieldValue.Value;
				return;
			}
			case "chu":
			{
				if (fv.ValueType != FieldValueType.SingleString)
				{
					throw new LanguagePlatformException(ErrorCode.EditScriptIncompatibleFieldValueTypes);
				}
				SingleStringFieldValue singleStringFieldValue = fv as SingleStringFieldValue;
				if (singleStringFieldValue == null)
				{
					throw new Exception("Internal error");
				}
				if (string.IsNullOrEmpty(singleStringFieldValue.Value))
				{
					throw new LanguagePlatformException(ErrorCode.EditScriptInvalidValue);
				}
				SystemFields.ChangeUser = singleStringFieldValue.Value;
				return;
			}
			case "usd":
			{
				if (fv.ValueType != FieldValueType.DateTime)
				{
					throw new LanguagePlatformException(ErrorCode.EditScriptIncompatibleFieldValueTypes);
				}
				DateTimeFieldValue dateTimeFieldValue = fv as DateTimeFieldValue;
				if (dateTimeFieldValue == null)
				{
					throw new Exception("Internal error");
				}
				SystemFields.UseDate = dateTimeFieldValue.Value;
				return;
			}
			case "usu":
			{
				if (fv.ValueType != FieldValueType.SingleString)
				{
					throw new LanguagePlatformException(ErrorCode.EditScriptIncompatibleFieldValueTypes);
				}
				SingleStringFieldValue singleStringFieldValue = fv as SingleStringFieldValue;
				if (singleStringFieldValue == null)
				{
					throw new Exception("Internal error");
				}
				if (string.IsNullOrEmpty(singleStringFieldValue.Value))
				{
					throw new LanguagePlatformException(ErrorCode.EditScriptInvalidValue);
				}
				SystemFields.UseUser = singleStringFieldValue.Value;
				return;
			}
			case "usc":
			{
				if (fv.ValueType != FieldValueType.Integer)
				{
					throw new LanguagePlatformException(ErrorCode.EditScriptIncompatibleFieldValueTypes);
				}
				IntFieldValue intFieldValue = fv as IntFieldValue;
				if (intFieldValue == null)
				{
					throw new Exception("Internal error");
				}
				if (intFieldValue.Value < 0)
				{
					throw new LanguagePlatformException(ErrorCode.EditScriptInvalidValue);
				}
				SystemFields.UseCount = intFieldValue.Value;
				return;
			}
			case "crd":
			{
				if (fv.ValueType != FieldValueType.DateTime)
				{
					throw new LanguagePlatformException(ErrorCode.EditScriptIncompatibleFieldValueTypes);
				}
				DateTimeFieldValue dateTimeFieldValue = fv as DateTimeFieldValue;
				if (dateTimeFieldValue == null)
				{
					throw new Exception("Internal error");
				}
				SystemFields.CreationDate = dateTimeFieldValue.Value;
				return;
			}
			case "cru":
			{
				if (fv.ValueType != FieldValueType.SingleString)
				{
					throw new LanguagePlatformException(ErrorCode.EditScriptIncompatibleFieldValueTypes);
				}
				SingleStringFieldValue singleStringFieldValue = fv as SingleStringFieldValue;
				if (singleStringFieldValue == null)
				{
					throw new Exception("Internal error");
				}
				if (string.IsNullOrEmpty(singleStringFieldValue.Value))
				{
					throw new LanguagePlatformException(ErrorCode.EditScriptInvalidValue);
				}
				SystemFields.CreationUser = singleStringFieldValue.Value;
				return;
			}
			case "x-format":
			{
				if (fv.ValueType != FieldValueType.SingleString)
				{
					throw new LanguagePlatformException(ErrorCode.EditScriptIncompatibleFieldValueTypes);
				}
				SingleStringFieldValue singleStringFieldValue = fv as SingleStringFieldValue;
				if (singleStringFieldValue == null)
				{
					throw new Exception("Internal error");
				}
				if (string.IsNullOrEmpty(singleStringFieldValue.Value))
				{
					throw new LanguagePlatformException(ErrorCode.EditScriptInvalidValue);
				}
				try
				{
					object obj = Enum.Parse(typeof(TranslationUnitFormat), singleStringFieldValue.Value, ignoreCase: true);
					Format = (TranslationUnitFormat)obj;
				}
				catch
				{
					throw new LanguagePlatformException(ErrorCode.EditScriptInvalidValue);
				}
				return;
			}
			case "x-origin":
			{
				if (fv.ValueType != FieldValueType.SingleString)
				{
					throw new LanguagePlatformException(ErrorCode.EditScriptIncompatibleFieldValueTypes);
				}
				SingleStringFieldValue singleStringFieldValue = fv as SingleStringFieldValue;
				if (singleStringFieldValue == null)
				{
					throw new Exception("Internal error");
				}
				if (string.IsNullOrEmpty(singleStringFieldValue.Value))
				{
					throw new LanguagePlatformException(ErrorCode.EditScriptInvalidValue);
				}
				try
				{
					object obj3 = Enum.Parse(typeof(TranslationUnitOrigin), singleStringFieldValue.Value, ignoreCase: true);
					Origin = (TranslationUnitOrigin)obj3;
				}
				catch
				{
					throw new LanguagePlatformException(ErrorCode.EditScriptInvalidValue);
				}
				return;
			}
			case "confirmationlevel":
			{
				if (fv.ValueType != FieldValueType.SingleString)
				{
					throw new LanguagePlatformException(ErrorCode.EditScriptIncompatibleFieldValueTypes);
				}
				SingleStringFieldValue singleStringFieldValue = fv as SingleStringFieldValue;
				if (singleStringFieldValue == null)
				{
					throw new Exception("Internal error");
				}
				if (string.IsNullOrEmpty(singleStringFieldValue.Value))
				{
					throw new LanguagePlatformException(ErrorCode.EditScriptInvalidValue);
				}
				try
				{
					object obj5 = Enum.Parse(typeof(ConfirmationLevel), singleStringFieldValue.Value, ignoreCase: true);
					ConfirmationLevel = (ConfirmationLevel)obj5;
				}
				catch
				{
					throw new LanguagePlatformException(ErrorCode.EditScriptInvalidValue);
				}
				return;
			}
			}
			FieldValue fieldValue = FieldValues.Lookup(fv.Name);
			if (fieldValue == null)
			{
				if (addIfMissing)
				{
					FieldValues.Add(fv.Duplicate());
				}
				return;
			}
			if (fieldValue.ValueType != fv.ValueType)
			{
				throw new LanguagePlatformException(ErrorCode.EditScriptIncompatibleFieldValueTypes);
			}
			FieldValues.Remove(fv.Name);
			FieldValues.Add(fv.Duplicate());
		}

		public FieldValue GetValue(string fieldName, FieldValueType t)
		{
			FieldValue value = GetValue(fieldName);
			if (value == null)
			{
				return null;
			}
			if (value.ValueType != t)
			{
				throw new LanguagePlatformException(ErrorCode.TMIncompatibleFieldTypes);
			}
			return value;
		}

		public static FieldValueType GetSystemFieldType(string fieldName)
		{
			return Field.LookupSpecialField(fieldName)?.ValueType ?? FieldValueType.Unknown;
		}

		public FieldValueType GetType(string fieldName)
		{
			FieldValueType systemFieldType = GetSystemFieldType(fieldName);
			if (systemFieldType != 0)
			{
				return systemFieldType;
			}
			return GetValue(fieldName)?.ValueType ?? FieldValueType.Unknown;
		}

		public List<Placeable> ComputePlaceables()
		{
			return PlaceableComputer.ComputePlaceables(this);
		}

		public TranslationUnit ExtractFragment(SegmentRange sourceRange, SegmentRange targetRange)
		{
			Segment segment = new Segment(SourceSegment.Culture);
			segment.AddRange(GetSegmentElements(SourceSegment, sourceRange));
			Segment segment2 = new Segment(TargetSegment.Culture);
			segment2.AddRange(GetSegmentElements(TargetSegment, targetRange));
			return new TranslationUnit(segment, segment2)
			{
				Origin = TranslationUnitOrigin.TM
			};
		}

		private static IEnumerable<SegmentElement> GetSegmentElements(Segment segment, SegmentRange range)
		{
			Dictionary<string, Tag> dictionary = new Dictionary<string, Tag>();
			for (int i = 0; i < range.From.Index; i++)
			{
				Tag tag = segment.Elements[i] as Tag;
				if (tag != null)
				{
					ProcessTag(tag, dictionary);
				}
			}
			List<SegmentElement> list = new List<SegmentElement>(dictionary.Values);
			for (int j = range.From.Index; j <= range.Into.Index; j++)
			{
				SegmentElement segmentElement = segment.Elements[j];
				Tag tag2 = segmentElement as Tag;
				if (tag2 == null)
				{
					Text text = segmentElement as Text;
					if (text != null)
					{
						Text item = ExtractText(text, j, range);
						list.Add(item);
					}
				}
				else
				{
					ProcessTag(tag2, dictionary);
					list.Add(tag2);
				}
			}
			for (int k = range.Into.Index + 1; k < segment.Elements.Count; k++)
			{
				Tag tag3 = segment.Elements[k] as Tag;
				if (tag3 != null && tag3.Type == TagType.End && tag3.TagID != null && dictionary.ContainsKey(tag3.TagID))
				{
					list.Add(tag3);
					dictionary.Remove(tag3.TagID);
				}
			}
			return list;
		}

		private static void ProcessTag(Tag tag, IDictionary<string, Tag> tags)
		{
			switch (tag.Type)
			{
			case TagType.Start:
				tags.Add(tag.TagID, tag);
				break;
			case TagType.End:
				if (tag.TagID != null && tags.ContainsKey(tag.TagID))
				{
					tags.Remove(tag.TagID);
				}
				break;
			}
		}

		private static Text ExtractText(Text text, int textElementIndex, SegmentRange range)
		{
			if (range.Length > 0)
			{
				return new Text(text.Value.Substring(range.From.Position, range.Length));
			}
			if (range.From.Index == textElementIndex)
			{
				return new Text(text.Value.Substring(range.From.Position));
			}
			if (range.Into.Index != textElementIndex)
			{
				return text;
			}
			return new Text(text.Value.Substring(0, range.Into.Position));
		}
	}
}
