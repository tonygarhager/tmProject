using System;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.Core
{
	[DataContract]
	public class FaultDescription
	{
		[DataMember]
		public FaultStatus Status
		{
			get;
			set;
		}

		[DataMember]
		public string Message
		{
			get;
			set;
		}

		[DataMember]
		public string Data
		{
			get;
			set;
		}

		[DataMember]
		public ErrorCode ErrorCode
		{
			get;
			set;
		}

		public FaultDescription(Exception e)
		{
			Message = e.Message;
			Status = FaultStatus.Error;
			Data = e.ToString();
			ErrorCode = ErrorCode.System;
		}

		public FaultDescription(ErrorCode code, FaultStatus status)
			: this(code, status, null)
		{
		}

		public FaultDescription(ErrorCode code)
			: this(code, FaultStatus.Error, null)
		{
		}

		public FaultDescription(ErrorCode code, string data)
			: this(code, FaultStatus.Error, data)
		{
		}

		public FaultDescription(ErrorCode code, FaultStatus status, string data)
		{
			Data = data;
			ErrorCode = code;
			Status = status;
			Message = (((data == null) ? GetDescriptionFromErrorCode(code) : string.Format(GetDescriptionFromErrorCode(code), data)) ?? "Unknown error");
		}

		public static string GetDescriptionFromErrorCode(ErrorCode code)
		{
			switch (code)
			{
			case ErrorCode.OK:
				return ErrorMessages.EMSG_OK;
			case ErrorCode.System:
				return ErrorMessages.EMSG_System;
			case ErrorCode.StorageError:
				return ErrorMessages.EMSG_StorageError;
			case ErrorCode.EmptySourceSegment:
				return ErrorMessages.EMSG_EmptySourceSegment;
			case ErrorCode.InvalidSourceSegment:
				return ErrorMessages.EMSG_InvalidSourceSegment;
			case ErrorCode.EmptyTargetSegment:
				return ErrorMessages.EMSG_EmptyTargetSegment;
			case ErrorCode.InvalidTargetSegment:
				return ErrorMessages.EMSG_InvalidTargetSegment;
			case ErrorCode.SourceLanguageIncompatibleWithTM:
				return ErrorMessages.EMSG_SourceLanguageIncompatibleWithTM;
			case ErrorCode.TargetLanguageIncompatibleWithTM:
				return ErrorMessages.EMSG_TargetLanguageIncompatibleWithTM;
			case ErrorCode.Other:
				return ErrorMessages.EMSG_Other;
			case ErrorCode.UndefinedOrInvalidLanguage:
				return ErrorMessages.EMSG_UndefinedOrInvalidLanguage;
			case ErrorCode.NeutralLanguage:
				return ErrorMessages.EMSG_NeutralLanguage;
			case ErrorCode.EmptySegment:
				return ErrorMessages.EMSG_EmptySegment;
			case ErrorCode.InvalidSegment:
				return ErrorMessages.EMSG_InvalidSegment;
			case ErrorCode.InvalidSegmentPeripheralWhitespace:
				return ErrorMessages.EMSG_InvalidSegmentPeripheralWhitespace;
			case ErrorCode.NoTextInSegment:
				return ErrorMessages.EMSG_NoTextInSegment;
			case ErrorCode.TagInvalidTagAnchor:
				return ErrorMessages.EMSG_InvalidTagAnchor;
			case ErrorCode.TagEmptyOrInvalidTagID:
				return ErrorMessages.EMSG_EmptyOrInvalidTagID;
			case ErrorCode.TagAnchorAlreadyUsed:
				return ErrorMessages.EMSG_TagAnchorAlreadyUsed;
			case ErrorCode.TagAnchorNotOpen:
				return ErrorMessages.EMSG_TagAnchorNotOpen;
			case ErrorCode.TagAnchorAlreadyClosed:
				return ErrorMessages.EMSG_TagAnchorAlreadyClosed;
			case ErrorCode.TagAnchorNotClosed:
				return ErrorMessages.EMSG_TagAnchorNotClosed;
			case ErrorCode.TagWarningTagsDropped:
				return ErrorMessages.EMSG_TagWarningTagsDropped;
			case ErrorCode.NotImplemented:
				return ErrorMessages.EMSG_NotImplemented;
			case ErrorCode.TMNotFound:
				return ErrorMessages.EMSG_TMNotFound;
			case ErrorCode.TMOrContainerMissing:
				return ErrorMessages.EMSG_TMOrContainerMissing;
			case ErrorCode.SegmentationUnknownVariable:
				return ErrorMessages.EMSG_SegmentationUnknownVariable;
			case ErrorCode.SegmentationNoRulesForLanguage:
				return ErrorMessages.EMSG_SegmentationNoRulesForLanguage;
			case ErrorCode.SegmentationRuleDeserializationError:
				return ErrorMessages.EMSG_SegmentationRuleDeserializationError;
			case ErrorCode.SegmentationRuleLoadError:
				return ErrorMessages.EMSG_SegmentationRuleLoadError;
			case ErrorCode.SegmentationEmptyRuleSet:
				return ErrorMessages.EMSG_SegmentationEmptyRuleSet;
			case ErrorCode.SegmentationIllegalKeywordInRule:
				return ErrorMessages.EMSG_SegmentationIllegalKeywordInRule;
			case ErrorCode.SegmentationIllegalContinuation:
				return ErrorMessages.EMSG_SegmentationIllegalContinuation;
			case ErrorCode.SegmentationTrailingJunk:
				return ErrorMessages.EMSG_SegmentationTrailingJunk;
			case ErrorCode.SegmentationUnknownRuleType:
				return ErrorMessages.EMSG_SegmentationUnknownRuleType;
			case ErrorCode.SegmentationInvalidVariableName:
				return ErrorMessages.EMSG_SegmentationInvalidVariableName;
			case ErrorCode.SegmentationInvalidRule:
				return ErrorMessages.EMSG_SegmentationInvalidRule;
			case ErrorCode.SegmentationCannotObtainSegmentor:
				return ErrorMessages.EMSG_SegmentationCannotObtainSegmentor;
			case ErrorCode.SegmentationSRXParseError:
				return ErrorMessages.EMSG_SegmentationSRXParseError;
			case ErrorCode.SegmentationSRXNoVersion:
				return ErrorMessages.EMSG_SegmentationSRXNoVersion;
			case ErrorCode.SegmentationSRXUnsupportedVersion:
				return ErrorMessages.EMSG_SegmentationSRXUnsupportedVersion;
			case ErrorCode.SegmentationSRXInternalError:
				return ErrorMessages.EMSG_SegmentationSRXInternalError;
			case ErrorCode.SegmentationSRXInvalidDocument:
				return ErrorMessages.EMSG_SegmentationSRXInvalidDocument;
			case ErrorCode.SegmentationTWBMalformedRule:
				return ErrorMessages.EMSG_SegmentationTWBMalformedRule;
			case ErrorCode.SegmentationTWBUnsupportedNestedExceptions:
				return ErrorMessages.EMSG_SegmentationTWBUnsupportedNestedExceptions;
			case ErrorCode.SegmentationTWBUnsupportedExceptionConstraints:
				return ErrorMessages.EMSG_SegmentationTWBUnsupportedExceptionConstraints;
			case ErrorCode.SegmentationTWBUnsupportedMultipleMatchContexts:
				return ErrorMessages.EMSG_SegmentationTWBUnsupportedMultipleMatchContexts;
			case ErrorCode.ConfigurationCannotLoad:
				return ErrorMessages.EMSG_ConfigurationCannotLoad;
			case ErrorCode.ConfigurationCannotResolveType:
				return ErrorMessages.EMSG_ConfigurationCannotResolveType;
			case ErrorCode.ConfigurationInvalidType:
				return ErrorMessages.EMSG_ConfigurationInvalidType;
			case ErrorCode.ConfigurationAbstractType:
				return ErrorMessages.EMSG_ConfigurationAbstractType;
			case ErrorCode.ConfigurationCannotInstantiateOrCastType:
				return ErrorMessages.EMSG_ConfigurationCannotInstantiateOrCastType;
			case ErrorCode.ConfigurationConnectionStringNotFound:
				return ErrorMessages.EMSG_ConfigurationConnectionStringNotFound;
			case ErrorCode.ConfigurationUnknownProviderType:
				return ErrorMessages.EMSG_ConfigurationUnknownProviderType;
			case ErrorCode.ConfigurationOtherError:
				return ErrorMessages.EMSG_ConfigurationOtherError;
			case ErrorCode.AuthInsufficientPermissions:
				return ErrorMessages.EMSG_AuthInsufficientPermissions;
			case ErrorCode.AuthUnknownOrNonauthenticatedUser:
				return ErrorMessages.EMSG_AuthUnknownOrNonauthenticatedUser;
			case ErrorCode.AuthInvalidUser:
				return ErrorMessages.EMSG_AuthInvalidUser;
			case ErrorCode.TMInvalidFieldName:
				return ErrorMessages.EMSG_InvalidFieldName;
			case ErrorCode.TMInvalidPicklistValueName:
				return ErrorMessages.EMSG_InvalidPicklistValueName;
			case ErrorCode.TMInvalidTMName:
				return ErrorMessages.EMSG_TMInvalidTMName;
			case ErrorCode.TMAlreadyExists:
				return ErrorMessages.EMSG_TMAlreadyExists;
			case ErrorCode.DAFieldTypesInconsistent:
				return ErrorMessages.EMSG_DAFieldTypesInconsistent;
			case ErrorCode.InvalidTMSourceLanguage:
				return ErrorMessages.EMSG_InvalidTMSourceLanguage;
			case ErrorCode.InvalidTMTargetLanguage:
				return ErrorMessages.EMSG_InvalidTMTargetLanguage;
			case ErrorCode.DATUNotInTM:
				return ErrorMessages.EMSG_DATUNotInTM;
			case ErrorCode.DAInvalidSegmentAfterDeserialization:
				return ErrorMessages.EMSG_DAInvalidSegmentAfterDeserialization;
			case ErrorCode.DAIndexDuplicateKey:
				return ErrorMessages.EMSG_DAIndexDuplicateKey;
			case ErrorCode.TMInvalidIteratorSize:
				return ErrorMessages.EMSG_TMInvalidIteratorSize;
			case ErrorCode.TMUndefinedCascadeElement:
				return ErrorMessages.EMSG_TMUndefinedCascadeElement;
			case ErrorCode.TMUninitializedCascadeElement:
				return ErrorMessages.EMSG_TMUninitializedCascadeElement;
			case ErrorCode.TMUndefinedTMInCascade:
				return ErrorMessages.EMSG_TMUndefinedTMInCascade;
			case ErrorCode.TMUninitializedTMInCascade:
				return ErrorMessages.EMSG_TMUninitializedTMInCascade;
			case ErrorCode.TMCannotInitializeTMInCascade:
				return ErrorMessages.EMSG_TMCannotInitializeTMInCascade;
			case ErrorCode.TMIncompatibleTMLanguageDirectionInCascade:
				return ErrorMessages.EMSG_TMIncompatibleTMLanguageDirectionInCascade;
			case ErrorCode.TMIncompatibleFieldTypes:
				return ErrorMessages.EMSG_TMIncompatibleFieldTypes;
			case ErrorCode.TMInvalidSortSpecification:
				return ErrorMessages.EMSG_TMInvalidSortSpecification;
			case ErrorCode.TMNoTMOpen:
				return ErrorMessages.EMSG_TMNoTMOpen;
			case ErrorCode.TMFieldAlreadyExists:
				return ErrorMessages.EMSG_TMFieldAlreadyExists;
			case ErrorCode.TMPicklistValueAlreadyExists:
				return ErrorMessages.EMSG_TMPicklistValueAlreadyExists;
			case ErrorCode.TMResourceAlreadyExists:
				return ErrorMessages.EMSG_TMResourceAlreadyExists;
			case ErrorCode.TMResourceDoesNotExist:
				return ErrorMessages.EMSG_TMResourceDoesNotExist;
			case ErrorCode.TMSourceLanguageMismatch:
				return ErrorMessages.EMSG_TMSourceLanguageMismatch;
			case ErrorCode.TMTargetLanguageMismatch:
				return ErrorMessages.EMSG_TMTargetLanguageMismatch;
			case ErrorCode.TMCannotModifyExactSearchOnlyFlag:
				return ErrorMessages.EMSG_TMCannotModifyExactSearchOnlyFlag;
			case ErrorCode.TMSearchModeNotSupported:
				return ErrorMessages.EMSG_TMSearchModeNotSupported;
			case ErrorCode.TMCannotModifyFullTextSearchFlag:
				return ErrorMessages.EMSG_TMCannotModifyFullTextSearchFlag;
			case ErrorCode.TMCannotModifyFuzzyIndices:
				return ErrorMessages.EMSG_TMCannotModifyFuzzyIndices;
			case ErrorCode.TMImportFieldNotExists:
				return ErrorMessages.EMSG_TMImportFieldNotExists;
			case ErrorCode.TMImportIncompatibleFieldTypes:
				return ErrorMessages.EMSG_TMImportIncompatibleFieldTypes;
			case ErrorCode.TMTULimitExceeded:
				return ErrorMessages.EMSG_TMTULimitExceeded;
			case ErrorCode.TMExceededTMNameLimit:
				return ErrorMessages.EMSG_TMExceededTMNameLimit;
			case ErrorCode.TMExceededFieldNameLimit:
				return ErrorMessages.EMSG_TMExceededFieldNameLimit;
			case ErrorCode.TMExceededPicklistValueNameLimit:
				return ErrorMessages.EMSG_TMExceededPicklistValueNameLimit;
			case ErrorCode.TMExceededCopyrightFieldLimit:
				return ErrorMessages.EMSG_TMExceededCopyrightFieldLimit;
			case ErrorCode.TMExceededDescriptionFieldLimit:
				return ErrorMessages.EMSG_TMExceededDescriptionFieldLimit;
			case ErrorCode.TMExceededTextFieldValueLimit:
				return ErrorMessages.EMSG_TMExceededTextFieldValueLimit;
			case ErrorCode.StorageTMAlreadyExists:
				return ErrorMessages.EMSG_StorageTMAlreadyExists;
			case ErrorCode.StorageSchemaDoesntExist:
				return ErrorMessages.EMSG_StorageSchemaDoesntExist;
			case ErrorCode.StorageSchemaAlreadyExists:
				return ErrorMessages.EMSG_StorageSchemaAlreadyExists;
			case ErrorCode.StoragePicklistValueAlreadyExists:
				return ErrorMessages.EMSG_StoragePicklistValueAlreadyExists;
			case ErrorCode.StorageIncompatibleAttributeType:
				return ErrorMessages.EMSG_StorageIncompatibleAttributeType;
			case ErrorCode.StorageFieldNotFound:
				return ErrorMessages.EMSG_StorageFieldNotFound;
			case ErrorCode.StorageFieldAlreadyExists:
				return ErrorMessages.EMSG_StorageFieldAlreadyExists;
			case ErrorCode.StorageParameterNotFound:
				return ErrorMessages.EMSG_StorageParameterNotFound;
			case ErrorCode.StorageDataFileNotFound:
				return ErrorMessages.EMSG_StorageDataFileNotFound;
			case ErrorCode.StorageVersionDataOutdated:
				return ErrorMessages.EMSG_StorageDataOutdated;
			case ErrorCode.StorageVersionDataNewer:
				return ErrorMessages.EMSG_StorageVersionDataNewer;
			case ErrorCode.TMXUnknownVersion:
				return ErrorMessages.EMSG_TMXUnknownVersion;
			case ErrorCode.TMXNoSegmentOpen:
				return ErrorMessages.EMSG_TMXNoSegmentOpen;
			case ErrorCode.TMXUnknownTMXAttribute:
				return ErrorMessages.EMSG_TMXUnknownTMXAttribute;
			case ErrorCode.TMXMissingVersion:
				return ErrorMessages.EMSG_TMXMissingVersion;
			case ErrorCode.TMXCannotAddTagData:
				return ErrorMessages.EMSG_TMXCannotAddTagData;
			case ErrorCode.TMXInternalParserError:
				return ErrorMessages.EMSG_TMXInternalParserError;
			case ErrorCode.TMXUnexpectedInputData:
				return ErrorMessages.EMSG_TMXUnexpectedInputData;
			case ErrorCode.TMXCannotDetermineLanguageDirection:
				return ErrorMessages.EMSG_TMXCannotDetermineLanguageDirection;
			case ErrorCode.StemmerErrorInStemmingRule:
				return ErrorMessages.EMSG_StemmerErrorInStemmingRule;
			case ErrorCode.TokenizerInvalidNumericFormat:
				return ErrorMessages.EMSG_TokenizerInvalidNumericFormat;
			case ErrorCode.TokenizerInvalidCharacterSet:
				return ErrorMessages.EMSG_TokenizerInvalidCharacterSet;
			case ErrorCode.NoRegionSpecificLanguageFound:
				return ErrorMessages.EMSG_NoRegionSpecificLanguageFound;
			case ErrorCode.ErrorInFilterExpression:
				return ErrorMessages.EMSG_FILTER_ErrorInFilterExpression;
			case ErrorCode.ErrorFilterUnbalancedParenthesis:
				return ErrorMessages.EMSG_Filter_UnbalancedParenthesis;
			case ErrorCode.ErrorFilterExpectFieldName:
				return ErrorMessages.EMSG_Filter_ExpectFieldName;
			case ErrorCode.ErrorFilterInvalidFieldName:
				return ErrorMessages.EMSG_Filter_InvalidFieldName;
			case ErrorCode.ErrorFilterEmptyFieldName:
				return ErrorMessages.EMSG_Filter_EmptyFieldName;
			case ErrorCode.ErrorFilterUnknownField:
				return ErrorMessages.EMSG_Filter_UnknownField;
			case ErrorCode.ErrorFilterPrematureEndOfInput:
				return ErrorMessages.EMSG_Filter_PrematureEndOfInput;
			case ErrorCode.ErrorFilterOperatorExpected:
				return ErrorMessages.EMSG_Filter_OperatorExpected;
			case ErrorCode.ErrorFilterNotAPicklistField:
				return ErrorMessages.EMSG_Filter_NotAPicklistField;
			case ErrorCode.ErrorFilterPicklistItemNotFound:
				return ErrorMessages.EMSG_Filter_PicklistItemNotFound;
			case ErrorCode.ErrorFilterFieldNameNotSurroundedByDoubleQuotes:
				return ErrorMessages.EMSG_Filter_FieldNameNotSurroundedByDoubleQuotes;
			case ErrorCode.ErrorFilterMultipleValuesNotBracketed:
				return ErrorMessages.EMSG_Filter_MultipleValuesNotBracketed;
			case ErrorCode.ErrorFilterMultivalueFieldNotBracketed:
				return ErrorMessages.EMSG_Filter_MultivalueFieldNotBracketed;
			case ErrorCode.ErrorFilterFieldRequiresIntegerValue:
				return ErrorMessages.EMSG_Filter_FieldRequiresIntegerValue;
			case ErrorCode.ErrorFilterValueNotSurroundedByDoubleQuotes:
				return ErrorMessages.EMSG_Filter_ValueNotSurroundedByDoubleQuotes;
			case ErrorCode.ErrorFilterCannotParseDate:
				return ErrorMessages.EMSG_Filter_CannotParseDate;
			case ErrorCode.ErrorFilterTrailingBackslash:
				return ErrorMessages.EMSG_Filter_TrailingBackslash;
			case ErrorCode.ErrorFilterIllegalInputSymbol:
				return ErrorMessages.EMSG_Filter_IllegalInputSymbol;
			case ErrorCode.ErrorFilterTrailingJunk:
				return ErrorMessages.EMSG_Filter_TrailingJunk;
			case ErrorCode.TMToolsNoTM:
				return ErrorMessages.EMSG_TMToolsNoTM;
			case ErrorCode.TMToolsNoFileHeader:
				return ErrorMessages.EMSG_TMToolsNoFileHeader;
			case ErrorCode.TMToolsIncompatibleLanguagesBetweenDocumentAndTM:
				return ErrorMessages.EMSG_TMToolsIncompatibleLanguagesBetweenDocumentAndTM;
			case ErrorCode.InternalError:
				return ErrorMessages.EMSG_InternalError;
			case ErrorCode.CorruptData:
				return ErrorMessages.EMSG_CorruptData;
			case ErrorCode.InconsistentData:
				return ErrorMessages.EMSG_InconsistentData;
			case ErrorCode.CannotInitializeSpring:
				return ErrorMessages.EMSG_CannotInitializeSpring;
			case ErrorCode.CannotInitializeFilterManager:
				return ErrorMessages.EMSG_CannotInitializeFilterManager;
			case ErrorCode.InvalidOperation:
				return ErrorMessages.EMSG_InvalidOperation;
			case ErrorCode.ReadonlyResource:
				return ErrorMessages.EMSG_ReadonlyResource;
			case ErrorCode.EmbeddedResourceNotFound:
				return ErrorMessages.EMSG_EmbeddedResourceNotFound;
			case ErrorCode.ResourceNotAvailable:
				return ErrorMessages.EMSG_ResourceNotAvailable;
			case ErrorCode.DataComponentMissing:
				return ErrorMessages.EMSG_DataComponentMissing;
			case ErrorCode.DataComponentIncompatible:
				return ErrorMessages.EMSG_DataComponentIncompatible;
			case ErrorCode.DataComponentNotOpen:
				return ErrorMessages.EMSG_DataComponentNotOpen;
			case ErrorCode.DataComponentAlreadyInUse:
				return ErrorMessages.EMSG_DataComponentAlreadyInUse;
			case ErrorCode.SegmentNotTokenized:
				return ErrorMessages.EMSG_SegmentNotTokenized;
			case ErrorCode.TagCountLimitExceeded:
				return ErrorMessages.EMSG_TagCountLimitExceeded;
			case ErrorCode.NoTranslatableContentInFile:
				return ErrorMessages.EMSG_NoTranslatableContentInFile;
			case ErrorCode.NoContentInFile:
				return ErrorMessages.EMSG_NoContentInFile;
			case ErrorCode.UnexpectedDocumentContent:
				return ErrorMessages.EMSG_UnexpectedDocumentContent;
			case ErrorCode.LanguageResourceFileNotFound:
				return ErrorMessages.EMSG_LanguageResourceFileNotFound;
			case ErrorCode.InvalidLanguageResourceFile:
				return ErrorMessages.EMSG_InvalidLanguageResourceFile;
			case ErrorCode.CorruptDataInResourceFile:
				return ErrorMessages.EMSG_CorruptDataInResourceFile;
			case ErrorCode.XmlError:
				return ErrorMessages.EMSG_XmlError;
			case ErrorCode.EditScriptEmptyFieldName:
				return ErrorMessages.EMSG_EDIT_FieldNameEmpty;
			case ErrorCode.EditScriptEmptySearchPattern:
				return ErrorMessages.EMSG_EDIT_SearchPatternNullOrEmpty;
			case ErrorCode.EditScriptSystemField:
				return ErrorMessages.EMSG_EDIT_SystemField;
			case ErrorCode.EditScriptIncompatibleFieldValueTypes:
				return ErrorMessages.EMSG_EDIT_IncompatibleFieldValueTypes;
			case ErrorCode.EditScriptInvalidValue:
				return ErrorMessages.EMSG_EDIT_InvalidValue;
			case ErrorCode.EditScriptIncompatibleField:
				return ErrorMessages.EMSG_EDIT_IncompatibleField;
			case ErrorCode.EditScriptInvalidOperationForFieldValueType:
				return ErrorMessages.EMSG_EDIT_InvalidOperationForFieldValueType;
			case ErrorCode.SQLiteNotADatabase:
				return ErrorMessages.EMSG_SQLITE_SQLiteNotADatabase;
			case ErrorCode.SQLiteCorrupt:
				return ErrorMessages.EMSG_SQLITE_SQLiteCorrupt;
			case ErrorCode.SQLiteOtherError:
				return ErrorMessages.EMSG_SQLITE_SQLiteOtherError;
			case ErrorCode.EmptyData:
				return ErrorMessages.EMSG_EmptyData;
			default:
				return null;
			}
		}
	}
}
