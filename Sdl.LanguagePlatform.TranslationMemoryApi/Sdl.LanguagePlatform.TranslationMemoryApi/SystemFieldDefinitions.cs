using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public static class SystemFieldDefinitions
	{
		public static FieldDefinition StructureContext = new FieldDefinition(Field.StructureContextFieldName, FieldValueType.MultipleString);

		public static FieldDefinition TextContext = new FieldDefinition(Field.TextContextFieldName, FieldValueType.MultipleString);

		public static FieldDefinition LastChangedAt = new FieldDefinition("chd", FieldValueType.DateTime);

		public static FieldDefinition LastChangedBy = new FieldDefinition("chu", FieldValueType.SingleString);

		public static FieldDefinition LastUsedAt = new FieldDefinition("usd", FieldValueType.DateTime);

		public static FieldDefinition LastUsedBy = new FieldDefinition("usu", FieldValueType.SingleString);

		public static FieldDefinition UsageCount = new FieldDefinition("usc", FieldValueType.Integer);

		public static FieldDefinition CreatedAt = new FieldDefinition("crd", FieldValueType.DateTime);

		public static FieldDefinition CreatedBy = new FieldDefinition("cru", FieldValueType.SingleString);

		public static FieldDefinition SourceText = new FieldDefinition("src", FieldValueType.SingleString);

		public static FieldDefinition TargetText = new FieldDefinition("trg", FieldValueType.SingleString);

		public static FieldDefinition PlainSourceTextLength = new FieldDefinition("sourceplainlength", FieldValueType.Integer);

		public static FieldDefinition PlainTargetTextLength = new FieldDefinition("targetplainlength", FieldValueType.Integer);

		public static FieldDefinition SourceTagCount = new FieldDefinition("sourcetagcount", FieldValueType.Integer);

		public static FieldDefinition TargetTagCount = new FieldDefinition("targettagcount", FieldValueType.Integer);
	}
}
