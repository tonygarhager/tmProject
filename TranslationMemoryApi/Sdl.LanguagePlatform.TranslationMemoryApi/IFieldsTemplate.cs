namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Represents a fields template, which is a named collection of fields that can be applied to a translation memory.
	/// </summary>
	public interface IFieldsTemplate
	{
		/// <summary>
		/// Gets or sets a general description of the entity.
		/// </summary>
		/// <value>The general description.</value>
		string Description
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets an entity name.
		/// </summary>
		/// <value>The entity name.</value>
		string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the collections of fields in this fields template. 
		/// </summary>
		/// <remarks>In case of a server-based fields template (<see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedFieldsTemplate" />), any changes to this collection are only persisted when calling <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedFieldsTemplate.Save" />.</remarks>
		FieldDefinitionCollection FieldDefinitions
		{
			get;
		}
	}
}
