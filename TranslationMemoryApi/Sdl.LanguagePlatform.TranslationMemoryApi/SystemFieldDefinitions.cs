using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// This class defines the available translation unit system fields that can be used in filter expressions in addition to user-defined fields.
	/// </summary>
	public static class SystemFieldDefinitions
	{
		/// <summary>
		/// The document structure information associated with the translation unit.
		/// </summary>
		public static FieldDefinition StructureContext = new FieldDefinition(Field.StructureContextFieldName, FieldValueType.MultipleString);

		/// <summary>
		/// The text contexts this translation unit occurs in.
		/// Since text context values are directly stored at the TU, and not represented as field values,
		/// this pseudo field name is only relevant for edit scripts.
		/// </summary>
		public static FieldDefinition TextContext = new FieldDefinition(Field.TextContextFieldName, FieldValueType.MultipleString);

		/// <summary>
		/// The date/time the translation unit was last changed.
		/// </summary>
		public static FieldDefinition LastChangedAt = new FieldDefinition("chd", FieldValueType.DateTime);

		/// <summary>
		/// The if of the user that last changed the translation unit.
		/// </summary>
		public static FieldDefinition LastChangedBy = new FieldDefinition("chu", FieldValueType.SingleString);

		/// <summary>
		/// The date/time the translation unit was last used.
		/// </summary>
		public static FieldDefinition LastUsedAt = new FieldDefinition("usd", FieldValueType.DateTime);

		/// <summary>
		/// The name of the user that last used the translation unit.
		/// </summary>
		public static FieldDefinition LastUsedBy = new FieldDefinition("usu", FieldValueType.SingleString);

		/// <summary>
		/// The number of times the translation unit has been used so far.
		/// </summary>
		public static FieldDefinition UsageCount = new FieldDefinition("usc", FieldValueType.Integer);

		/// <summary>
		/// The date/time the translation unit was created.
		/// </summary>
		public static FieldDefinition CreatedAt = new FieldDefinition("crd", FieldValueType.DateTime);

		/// <summary>
		/// The name of the user that created the translation unit.
		/// </summary>
		public static FieldDefinition CreatedBy = new FieldDefinition("cru", FieldValueType.SingleString);

		/// <summary>
		/// The source text of the translation unit.
		/// </summary>
		public static FieldDefinition SourceText = new FieldDefinition("src", FieldValueType.SingleString);

		/// <summary>
		/// The target text of the the translation unit.
		/// </summary>
		public static FieldDefinition TargetText = new FieldDefinition("trg", FieldValueType.SingleString);

		/// <summary>
		/// The length of the plain source text of the the translation unit, in characters.
		/// </summary>
		public static FieldDefinition PlainSourceTextLength = new FieldDefinition("sourceplainlength", FieldValueType.Integer);

		/// <summary>
		/// The length of the plain source text of the the translation unit, in characters.
		/// </summary>
		public static FieldDefinition PlainTargetTextLength = new FieldDefinition("targetplainlength", FieldValueType.Integer);

		/// <summary>
		/// The number of tags occurring in the source segment.
		/// </summary>
		public static FieldDefinition SourceTagCount = new FieldDefinition("sourcetagcount", FieldValueType.Integer);

		/// <summary>
		/// The number of tags occurring in the target segment.
		/// </summary>
		public static FieldDefinition TargetTagCount = new FieldDefinition("targettagcount", FieldValueType.Integer);
	}
}
