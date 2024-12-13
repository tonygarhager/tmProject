using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemory.EditScripts;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Exposes functionality for a specific language direction within a translation memory.
	/// </summary>
	public interface ITranslationMemoryLanguageDirection : ITranslationProviderLanguageDirection
	{
		/// <summary>
		/// Gets the translation memory to which this language direction belongs.
		/// </summary>
		new ITranslationMemory TranslationProvider
		{
			get;
		}

		/// <summary>
		/// Returns the number of translation units in this TM. Note that the computation of the TU
		/// count may be a time-consuming operation for some back-end storage systems. 
		/// </summary>
		int GetTranslationUnitCount();

		/// <summary>
		/// Applies the specified field values to the translation unit identified by the specified
		/// <paramref name="translationUnitId" />.
		/// </summary>
		/// <param name="values">The field values to apply</param>
		/// <param name="overwrite">If true, overwrite the old values, and merge the new values with the old ones otherwise.</param>
		/// <param name="translationUnitId">The identifier of the translation unit to apply the field values to</param>
		/// <returns>true if the translation unit has been modified. false otherwise.</returns>
		bool ApplyFieldsToTranslationUnit(FieldValues values, bool overwrite, PersistentObjectToken translationUnitId);

		/// <summary>
		/// Applies the specified field values to the translation units identified by the specified
		/// <paramref name="translationUnitIds" /> collection.
		/// </summary>
		/// <param name="values">The field values to apply</param>
		/// <param name="overwrite">If true, overwrite the old values, and merge the new values with the old ones otherwise.</param>
		/// <param name="translationUnitIds">A collection of translation unit IDs to apply the field values to</param>
		/// <returns>The number of changed translation units</returns>
		int ApplyFieldsToTranslationUnits(FieldValues values, bool overwrite, PersistentObjectToken[] translationUnitIds);

		/// <summary>
		/// Deletes the translation unit with the specified <paramref name="translationUnitId" /> from the TM.
		/// </summary>
		/// <param name="translationUnitId">The ID of the translation unit to delete.</param>
		/// <returns>true if the translation unit was deleted, false otherwise.</returns>
		bool DeleteTranslationUnit(PersistentObjectToken translationUnitId);

		/// <summary>
		/// Deletes all translation units from the TM.
		/// </summary>
		/// <returns>The number of deleted translation units</returns>
		int DeleteAllTranslationUnits();

		/// <summary>
		/// Deletes the translation units with the specified IDs from the translation memory.
		/// </summary>
		/// <param name="translationUnitIds">A collection of the translation unit IDs to delete</param>
		/// <returns>The number of deleted translation units</returns>
		int DeleteTranslationUnits(PersistentObjectToken[] translationUnitIds);

		/// <summary>
		/// Deletes all translation units in the TM, using an iterator. The iterator will
		/// process only a fixed maximum number of translation units (<see cref="P:Sdl.LanguagePlatform.TranslationMemory.TranslationMemoryIterator.MaxCount" />)
		/// and then the call will return, giving the client an opportunity to discontinue
		/// the process or update the UI before the next round-trip.
		/// <para>Optionally, a filter can be set on the iterator which then will only 
		/// delete those TUs which satisfy the filter condition.</para>
		/// </summary>
		/// <param name="iterator">The iterator to use.</param>
		/// <returns>The number of deleted translation units.</returns>
		int DeleteTranslationUnitsWithIterator(ref RegularIterator iterator);

		/// <summary>
		/// Applies an <see cref="T:Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditScript" /> to the translation units identified by the 
		/// identifiers in the <paramref name="translationUnitIds" /> collection.
		/// </summary>
		/// <param name="editScript">The edit script to apply</param>
		/// <param name="updateMode">The update mode, which can be to update changed
		/// translation units in-place or add new translation units to the TM (which is only relevant
		/// if segment data has been changed).</param>
		/// <param name="translationUnitIds">The collection of TU IDs to apply the edit script to.</param>
		/// <returns>The number of changed or edited translation units</returns>
		int EditTranslationUnits(EditScript editScript, EditUpdateMode updateMode, PersistentObjectToken[] translationUnitIds);

		/// <summary>
		/// Applies an <see cref="T:Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditScript" /> to the translation units, using an iterator.
		/// </summary>
		/// <param name="editScript">The edit script to apply</param>
		/// <param name="updateMode">The update mode, which can be to update changed
		/// translation units in-place or add new translation units to the TM (which is only relevant
		/// if segment data has been changed).</param>
		/// <param name="iterator">The iterator to use. Note that a filter condition can be set on the 
		/// iterator in which case the script will only be applied to those translation units
		/// which satisfy the filter condition.</param>
		/// <returns>The number of changed or edited translation units</returns>
		/// <remarks>Note that a filter condition can also be set on the <paramref name="editScript" /> 
		/// instead of the iterator. Depending on the back-end storage, it may be more efficient to 
		/// set it on the iterator than on the edit script.</remarks>
		int EditTranslationUnitsWithIterator(EditScript editScript, EditUpdateMode updateMode, ref RegularIterator iterator);

		/// <summary>
		/// Applies the specified edit script to the translation units in a TM, but unlike <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationMemoryLanguageDirection.EditTranslationUnitsWithIterator(Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditScript,Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditUpdateMode,Sdl.LanguagePlatform.TranslationMemory.RegularIterator@)" />, 
		/// the TUs will not be updated in the TM, but modified copies of the TUs will be returned to the caller. This
		/// can be used to "preview" the changes the edit script would apply.
		/// </summary>
		/// <param name="editScript">The edit script</param>
		/// <param name="iterator">An iterator. See also the iterator-related remarks for <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationMemoryLanguageDirection.EditTranslationUnitsWithIterator(Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditScript,Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditUpdateMode,Sdl.LanguagePlatform.TranslationMemory.RegularIterator@)" />.</param>
		/// <returns></returns>
		TranslationUnit[] PreviewEditTranslationUnitsWithIterator(EditScript editScript, ref RegularIterator iterator);

		/// <summary>
		/// Returns a set of potentially duplicated translation units in the TM, using a special 
		/// <see cref="T:Sdl.LanguagePlatform.TranslationMemory.DuplicateIterator" />. A group of TUs are considered potential duplicates
		/// of each other if the internal hash values for their source segments are identical.
		/// There may be several reasons for this being the case:
		/// <list type="bullet">
		/// <item>The TUs have the same source segment, but different translations</item>
		/// <item>The TUs have different source segments, but only differ in complex
		/// tokens (controlled through the TM's <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationMemory.Recognizers" />) which will result in
		/// the identical hash values,</item>
		/// <item>The source segments only differ by whitespace or certain punctuation, which do not
		/// modify the hash values,</item>
		/// <item>The hashing algorithm leads to collisions in which case the segments may be 
		/// entirely different, but still result in the same hash value.</item>
		/// </list>
		/// </summary>
		/// <param name="iterator">The iterator to use</param>
		/// <returns>The translation units which are potential duplicates, or null if no
		/// more potential duplicates can be found.</returns>
		TranslationUnit[] GetDuplicateTranslationUnits(ref DuplicateIterator iterator);

		/// <summary>
		/// Retrieves the translation unit with the specified <paramref name="translationUnitId" /> from the
		/// translation memory.
		/// </summary>
		/// <param name="translationUnitId">The ID of the translation unit to retrieve.</param>
		/// <returns>The translation unit with the specified ID, or null if that TU does not
		/// exist.</returns>
		TranslationUnit GetTranslationUnit(PersistentObjectToken translationUnitId);

		/// <summary>
		/// Retrieves a set of translation units, using an iterator. At most 
		/// <see cref="P:Sdl.LanguagePlatform.TranslationMemory.TranslationMemoryIterator.MaxCount" /> translation units will be returned in one
		/// round-trip. 
		/// <remarks>
		/// <list type="bullet">
		/// <item>A <see cref="T:Sdl.LanguagePlatform.TranslationMemory.FilterExpression" /> can be set on the iterator in which case 
		/// only those TUs which satisfy the filter condition will be returned.</item>
		/// <item>See also the remarks on <see cref="P:Sdl.LanguagePlatform.TranslationMemory.TranslationMemoryIterator.MaxScan" /> on how to avoid
		/// timeouts with filtered iteration.</item>
		/// </list>
		/// </remarks>
		/// </summary>
		/// <param name="iterator">The iterator</param>
		/// <returns>The translation units, or null if no more translation units can be 
		/// retrieved.</returns>
		TranslationUnit[] GetTranslationUnits(ref RegularIterator iterator);

		/// <summary>
		/// Updates (adds, overwrites, or merges) the specified translation units.
		/// </summary>
		/// <param name="translationUnits">The translation units to update.</param>
		/// <param name="mask">An optional mask with the same length as <paramref name="translationUnits" />. If 
		/// provided, only those TUs in <paramref name="translationUnits" /> will be updated for which the corresponding
		/// flag in <paramref name="mask" /> is <code>true</code>. Those TUs for which the corresponding
		/// flag is <code>false</code> will only be used to establish context information. If no masking
		/// is required, this parameter can be null.</param>
		/// <returns>An array of <see cref="T:Sdl.LanguagePlatform.TranslationMemory.ImportResult" />s with the same length as <paramref name="translationUnits" />, 
		/// which indicates, for each translation unit, the action taken by the translation memory.</returns>
		ImportResult[] UpdateTranslationUnitsMasked(TranslationUnit[] translationUnits, bool[] mask);

		/// <summary>
		/// Re-indexes the translation memory, using an iterator so that the client can update
		/// progress indicators or discontinue the process.
		/// <para>The same iterator instance should be passed in subsequent calls, in order to obtain 
		/// the next page, and so on.</para>
		/// </summary>
		/// <param name="iterator">The iterator to use for the re-indexing process.</param> 
		/// <returns>False if the iterator is at the end of the TM or the TM is empty, true otherwise. 
		/// The re-indexing process should be continued until the method returns false.</returns>
		/// <remarks>For larger TMs (&gt; 100.000 TUs) it is recommended to also recompute the index
		/// statistics after the re-indexing finished (see <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationMemory.RecomputeFuzzyIndexStatistics" />).</remarks>
		bool ReindexTranslationUnits(ref RegularIterator iterator);
	}
}
