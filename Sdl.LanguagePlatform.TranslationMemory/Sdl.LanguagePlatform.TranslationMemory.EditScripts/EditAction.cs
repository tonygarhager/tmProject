using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory.EditScripts
{
	[DataContract]
	[KnownType(typeof(EditActionChangeFieldValue))]
	[KnownType(typeof(EditActionDeleteFieldValue))]
	[KnownType(typeof(EditActionDeleteAllFieldValues))]
	[KnownType(typeof(EditActionDeleteTags))]
	[KnownType(typeof(EditActionRenameField))]
	[KnownType(typeof(EditActionSearchReplace))]
	public abstract class EditAction
	{
		public abstract bool Apply(TranslationUnit tu);

		public abstract bool Validate(IFieldDefinitions fields, bool throwIfInvalid);
	}
}
