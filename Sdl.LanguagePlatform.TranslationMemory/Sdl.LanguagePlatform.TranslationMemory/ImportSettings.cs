using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.TranslationMemory.EditScripts;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	public class ImportSettings
	{
		public enum NewFieldsOption
		{
			AddToSetup,
			Ignore,
			SkipTranslationUnit,
			Error
		}

		public enum FieldUpdateMode
		{
			Merge,
			Overwrite,
			LeaveUnchanged
		}

		public enum ImportTUProcessingMode
		{
			ProcessCleanedTUOnly,
			ProcessRawTUOnly,
			ProcessBothTUs
		}

		public enum TUUpdateMode
		{
			AddNew,
			Overwrite,
			LeaveUnchanged,
			KeepMostRecent,
			OverwriteCurrent
		}

		public static readonly int DefaultTagCountLimit = 250;

		private int _alignmentQuality;

		[DataMember]
		public bool OverrideTuUserIdWithCurrentContextUser
		{
			get;
			set;
		}

		[DataMember]
		public bool UseTmUserIdFromBilingualFile
		{
			get;
			set;
		}

		[DataMember]
		public NewFieldsOption NewFields
		{
			get;
			set;
		}

		[DataMember]
		public FieldUpdateMode ExistingFieldsUpdateMode
		{
			get;
			set;
		}

		[Obsolete("OverwriteExistingTUS property is obsolete, please use ExistingTUsUpdateMode instead.")]
		[DataMember]
		public bool OverwriteExistingTUs
		{
			get;
			set;
		}

		[DataMember(Order = 2, IsRequired = false)]
		public TUUpdateMode ExistingTUsUpdateMode
		{
			get;
			set;
		}

		[DataMember]
		public bool IsDocumentImport
		{
			get;
			set;
		}

		[DataMember]
		public bool PlainText
		{
			get;
			set;
		}

		[DataMember]
		public int TagCountLimit
		{
			get;
			set;
		}

		[DataMember]
		public bool IncrementUsageCount
		{
			get;
			set;
		}

		[DataMember]
		public FieldValues ProjectSettings
		{
			get;
			set;
		}

		[DataMember]
		public bool CheckMatchingSublanguages
		{
			get;
			set;
		}

		[DataMember]
		public FilterExpression Filter
		{
			get;
			set;
		}

		[DataMember]
		public EditScript EditScript
		{
			get;
			set;
		}

		[DataMember]
		public string InvalidTranslationUnitsExportPath
		{
			get;
			set;
		}

		[DataMember]
		public ConfirmationLevel[] ConfirmationLevels
		{
			get;
			set;
		}

		[DataMember]
		public IDictionary<FieldIdentifier, FieldIdentifier> FieldIdentifierMappings
		{
			get;
			set;
		}

		[DataMember]
		public ImportTUProcessingMode TUProcessingMode
		{
			get;
			set;
		}

		[DataMember]
		[Obsolete("This flag is now ignored. If Acronym recognizer is enabled for a TM, Acronym auto-substitution will always be enabled during import (just like other recognizers).")]
		public bool AcronymsAutoSubstitution
		{
			get;
			set;
		}

		[DataMember]
		public int AlignmentQuality
		{
			get
			{
				return _alignmentQuality;
			}
			set
			{
				if (_alignmentQuality < 0 || _alignmentQuality > 100)
				{
					throw new ArgumentOutOfRangeException("_alignmentQuality", "Alignment quality out of [0..100] range!");
				}
				_alignmentQuality = value;
			}
		}

		public ImportSettings()
		{
			IncrementUsageCount = false;
			NewFields = NewFieldsOption.Error;
			ExistingFieldsUpdateMode = FieldUpdateMode.Merge;
			CheckMatchingSublanguages = false;
			ProjectSettings = null;
			PlainText = false;
			Filter = null;
			InvalidTranslationUnitsExportPath = null;
			ConfirmationLevels = null;
			FieldIdentifierMappings = null;
			TagCountLimit = DefaultTagCountLimit;
			OverwriteExistingTUs = false;
			AcronymsAutoSubstitution = false;
			ExistingTUsUpdateMode = TUUpdateMode.AddNew;
			UseTmUserIdFromBilingualFile = true;
			OverrideTuUserIdWithCurrentContextUser = false;
		}
	}
}
