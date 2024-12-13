using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemory.EditScripts;
using Sdl.LanguagePlatform.TranslationMemoryApi.Helpers;
using Sdl.LanguagePlatform.TranslationMemoryImpl;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.ServiceModel;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Represents the language direction of a file-based translation memory (see <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.FileBasedTranslationMemory" />).
	/// </summary>
	public class FileBasedTranslationMemoryLanguageDirection : ISubsegmentTranslationMemoryLanguageDirection, ITranslationMemoryLanguageDirection, ITranslationProviderLanguageDirection, IAdvancedContextTranslationMemoryLanguageDirection
	{
		private AbstractLocalTranslationMemory _tm;

		/// <summary>
		/// Gets the translation memory to which this language direction belongs.
		/// </summary>
		/// <value></value>
		public ITranslationMemory TranslationProvider => _tm;

		/// <summary>
		/// Gets the translation memory to which this language direction belongs.
		/// </summary>
		/// <value></value>
		ITranslationProvider ITranslationProviderLanguageDirection.TranslationProvider => _tm;

		/// <summary>
		/// Gets the language direction of the translation memory.
		/// </summary>
		public LanguagePair LanguageDirection => _tm.Setup.LanguageDirection;

		/// <summary>
		/// Gets a flag which indicates whether the translation provider supports
		/// searches in the reversed language direction.
		/// </summary>
		/// <value></value>
		public bool CanReverseLanguageDirection => _tm.Setup.CanReverseLanguageDirection;

		/// <summary>
		/// Gets the source language.
		/// </summary>
		/// <value></value>
		public CultureInfo SourceLanguage => LanguageDirection.SourceCulture;

		/// <summary>
		/// Gets the target language.
		/// </summary>
		/// <value></value>
		public CultureInfo TargetLanguage => LanguageDirection.TargetCulture;

		internal FileBasedTranslationMemoryLanguageDirection(AbstractLocalTranslationMemory tm)
		{
			_tm = tm;
		}

		/// <summary>
		/// Deletes all translation units from the TM.
		/// </summary>
		/// <returns>The number of deleted translation units</returns>
		public int DeleteAllTranslationUnits()
		{
			try
			{
				return _tm.Service.DeleteAllTranslationUnits(_tm.Container, _tm.Setup.ResourceId);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		/// <summary>
		/// Deletes the translation unit with the specified <paramref name="translationUnitId" /> from the TM.
		/// </summary>
		/// <param name="translationUnitId">The ID of the translation unit to delete.</param>
		/// <returns>
		/// <c>true</c> if the translation unit was deleted, <c>false</c> otherwise.
		/// </returns>
		public bool DeleteTranslationUnit(PersistentObjectToken translationUnitId)
		{
			try
			{
				return _tm.Service.DeleteTranslationUnit(_tm.Container, _tm.Setup.ResourceId, translationUnitId);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		/// <summary>
		/// Deletes the translation units with the specified IDs from the translation memory.
		/// </summary>
		/// <param name="translationUnitIds">A collection of the translation unit IDs to delete</param>
		/// <returns>The number of deleted translation units</returns>
		public int DeleteTranslationUnits(PersistentObjectToken[] translationUnitIds)
		{
			try
			{
				return _tm.Service.DeleteTranslationUnits(_tm.Container, _tm.Setup.ResourceId, translationUnitIds);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

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
		public int DeleteTranslationUnitsWithIterator(ref RegularIterator iterator)
		{
			if (iterator == null)
			{
				throw new ArgumentNullException("iterator");
			}
			try
			{
				return _tm.Service.DeleteTranslationUnitsWithIterator(_tm.Container, _tm.Setup.ResourceId, ref iterator);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		/// <summary>
		/// Iteratively applies an <see cref="T:Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditScript" /> to all translation 
		/// units in the current translation memory.
		/// </summary>
		/// <param name="editScript">The edit script to apply.</param>
		/// <param name="updateMode">The update mode to apply.</param>
		/// <returns>The number of edited translation units per round-trip.</returns>
		public int EditTranslationUnits(EditScript editScript, EditUpdateMode updateMode)
		{
			return EditTranslationUnits(editScript, updateMode, (FilterExpression)null);
		}

		/// <summary>
		/// Iteratively applies an <see cref="T:Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditScript" /> to all translation 
		/// units in the current translation memory. A filter condition can be sued in which case only
		/// those translation units which match the filter will be edited.
		/// </summary>
		/// <param name="editScript">The edit script to apply.</param>
		/// <param name="updateMode">The update mode to apply.</param>
		/// <param name="filter">An optional filter to use for the TM iteration.</param>
		/// <returns>The number of edited translation units per round-trip.</returns>
		public int EditTranslationUnits(EditScript editScript, EditUpdateMode updateMode, FilterExpression filter)
		{
			if (editScript == null)
			{
				throw new ArgumentNullException("editScript");
			}
			try
			{
				int num = 0;
				RegularIterator iterator = new RegularIterator();
				iterator.Filter = filter;
				iterator.Forward = true;
				int num2 = 0;
				do
				{
					num2 = _tm.Service.EditTranslationUnitsWithIterator(_tm.Container, _tm.Setup.ResourceId, editScript, updateMode, ref iterator);
					num += num2;
				}
				while (num2 > 0);
				return num;
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

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
		/// <returns>
		/// The number of changed or edited translation units
		/// </returns>
		/// <remarks>Note that a filter condition can also be set on the <paramref name="editScript" />
		/// instead of the iterator. Depending on the back-end storage, it may be more efficient to
		/// set it on the iterator than on the edit script.</remarks>
		public int EditTranslationUnitsWithIterator(EditScript editScript, EditUpdateMode updateMode, ref RegularIterator iterator)
		{
			if (editScript == null)
			{
				throw new ArgumentNullException("editScript");
			}
			if (iterator == null)
			{
				throw new ArgumentNullException("iterator");
			}
			try
			{
				return _tm.Service.EditTranslationUnitsWithIterator(_tm.Container, _tm.Setup.ResourceId, editScript, updateMode, ref iterator);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		/// <summary>
		/// Re-indexes the translation memory, using an iterator so that the client can update
		/// progress indicators or discontinue the process.
		/// <para>The same iterator instance should be passed in subsequent calls, in order to obtain
		/// the next page, and so on.</para>
		/// </summary>
		/// <param name="iterator">The iterator to use for the re-indexing process.</param>
		/// <returns>
		/// False if the iterator is at the end of the TM or the TM is empty, <c>true</c> otherwise.
		/// The re-indexing process should be continued until the method returns <c>false</c>.
		/// </returns>
		/// <remarks>For larger TMs (&gt; 100.000 TUs) it is recommended to also recompute the index
		/// statistics after the re-indexing finished (see <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationMemory.RecomputeFuzzyIndexStatistics" />).</remarks>
		public bool ReindexTranslationUnits(ref RegularIterator iterator)
		{
			try
			{
				return _tm.Service.ReindexTranslationUnits(_tm.Container, _tm.Setup.ResourceId, ref iterator);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		/// <summary>
		/// Iteratively applies an <see cref="T:Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditScript" /> to all translation 
		/// units in the current translation memory. A filter condition can be sued in which case only
		/// those translation units which match the filter will be edited.
		/// </summary>
		/// <param name="editScript">The edit script to apply.</param>
		/// <param name="filter">An optional filter to use for the TM iteration.</param>
		/// <returns>The number of edited translation units per round-trip.</returns>
		public TranslationUnit[] PreviewEditTranslationUnits(EditScript editScript, FilterExpression filter)
		{
			if (editScript == null)
			{
				throw new ArgumentNullException("editScript");
			}
			try
			{
				TranslationUnit[] array = null;
				RegularIterator iterator = new RegularIterator();
				iterator.Filter = filter;
				iterator.Forward = true;
				int num = 0;
				do
				{
					array = _tm.Service.PreviewEditTranslationUnitsWithIterator(_tm.Container, _tm.Setup.ResourceId, editScript, ref iterator);
				}
				while (array == null || array.Length == 0);
				return array;
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		/// <summary>
		/// Applies an <see cref="T:Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditScript" /> to the translation units identified by the
		/// identifiers in the <paramref name="translationUnitIds" /> collection.
		/// </summary>
		/// <param name="editScript">The edit script to apply</param>
		/// <param name="updateMode">The update mode, which can be to update changed
		/// translation units in-place or add new translation units to the TM (which is only relevant
		/// if segment data has been changed).</param>
		/// <param name="translationUnitIds">The collection of TU IDs to apply the edit script to.</param>
		/// <returns>
		/// The number of changed or edited translation units
		/// </returns>
		public int EditTranslationUnits(EditScript editScript, EditUpdateMode updateMode, PersistentObjectToken[] translationUnitIds)
		{
			if (editScript == null)
			{
				throw new ArgumentNullException("editScript");
			}
			try
			{
				return _tm.Service.EditTranslationUnits(_tm.Container, _tm.Setup.ResourceId, editScript, updateMode, translationUnitIds);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		/// <summary>
		/// Adds a translation unit to the database. If the provider doesn't support adding/updating, the
		/// implementation should return a reasonable <see cref="T:Sdl.LanguagePlatform.TranslationMemory.ImportResult" /> but should not throw an exception.
		/// </summary>
		/// <param name="translationUnit">The translation unit.</param>
		/// <param name="settings">The settings used for this operation.</param>
		/// <returns>
		/// An <see cref="T:Sdl.LanguagePlatform.TranslationMemory.ImportResult" /> which represents the status of the operation (succeeded, ignored, etc).
		/// </returns>
		public ImportResult AddTranslationUnit(TranslationUnit translationUnit, ImportSettings settings)
		{
			try
			{
				return _tm.Service.AddTranslationUnit(_tm.Container, _tm.Setup.ResourceId, translationUnit, settings);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		/// <summary>
		/// Adds an array of translation units to the database. If the provider doesn't support adding/updating, the
		/// implementation should return a reasonable <see cref="T:Sdl.LanguagePlatform.TranslationMemory.ImportResult" /> but should not throw an exception.
		/// </summary>
		/// <param name="translationUnits">An arrays of translation units to be added.</param>
		/// <param name="settings">The settings used for this operation.</param>
		/// <returns>
		/// An array of <see cref="T:Sdl.LanguagePlatform.TranslationMemory.ImportResult" /> objects, which mirrors the translation unit array. It has the exact same size and contains the
		/// status of each add operation for each particular translation unit with the same index within the array.
		/// </returns>
		public ImportResult[] AddTranslationUnits(TranslationUnit[] translationUnits, ImportSettings settings)
		{
			try
			{
				return _tm.Service.AddTranslationUnits(_tm.Container, _tm.Setup.ResourceId, translationUnits, settings);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		/// <summary>
		/// Adds an array of translation units to the database. If hash codes of the previous translations are provided,
		/// a found translation will be overwritten. If none is found, or the hash is 0 or the collection is <c>null</c>,
		/// the operation behaves identical to <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.FileBasedTranslationMemoryLanguageDirection.AddTranslationUnits(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[],Sdl.LanguagePlatform.TranslationMemory.ImportSettings)" />.
		/// <para>
		/// If the provider doesn't support adding/updating, the
		/// implementation should return a reasonable <see cref="T:Sdl.LanguagePlatform.TranslationMemory.ImportResult" /> but should not throw an exception.
		/// </para>
		/// </summary>
		/// <param name="translationUnits">An arrays of translation units to be added.</param>
		/// <param name="previousTranslationHashes">If provided, a corresponding array of a the hash code of a previous translation.</param>
		/// <param name="settings">The settings used for this operation.</param>
		/// <returns>
		/// An array of <see cref="T:Sdl.LanguagePlatform.TranslationMemory.ImportResult" /> objects, which mirrors the translation unit array. It has the exact same size and contains the
		/// status of each add operation for each particular translation unit with the same index within the array.
		/// </returns>
		public ImportResult[] AddOrUpdateTranslationUnits(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings)
		{
			try
			{
				return _tm.Service.AddOrUpdateTranslationUnits(_tm.Container, _tm.Setup.ResourceId, translationUnits, previousTranslationHashes, settings);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		/// <summary>
		/// Adds an array of translation units to the database, but will only add those
		/// for which the corresponding mask field is <c>true</c>. If the provider doesn't support adding/updating, the
		/// implementation should return a reasonable <see cref="T:Sdl.LanguagePlatform.TranslationMemory.ImportResult" /> but should not throw an exception.
		/// </summary>
		/// <param name="translationUnits">An arrays of translation units to be added.</param>
		/// <param name="settings">The settings used for this operation.</param>
		/// <param name="mask">A <c>bool</c> array with the same cardinality as the TU array, specifying which TUs to add.</param>
		/// <returns>
		/// An array of <see cref="T:Sdl.LanguagePlatform.TranslationMemory.ImportResult" /> objects, which mirrors the translation unit array. It has the exact same size and contains the
		/// status of each add operation for each particular translation unit with the same index within the array.
		/// </returns>
		public ImportResult[] AddTranslationUnitsMasked(TranslationUnit[] translationUnits, ImportSettings settings, bool[] mask)
		{
			try
			{
				return _tm.Service.AddTranslationUnitsMasked(_tm.Container, _tm.Setup.ResourceId, translationUnits, settings, mask);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		/// <summary>
		/// Adds an array of translation units to the database, but will only add those
		/// for which the corresponding mask field is <c>true</c>. If the previous translation hashes are provided,
		/// existing translations will be updated if the target segment hash changed.
		/// <para>
		/// If the provider doesn't support adding/updating, the
		/// implementation should return a reasonable <see cref="T:Sdl.LanguagePlatform.TranslationMemory.ImportResult" /> but should not throw an exception.
		/// </para>
		/// </summary>
		/// <param name="translationUnits">An arrays of translation units to be added.</param>
		/// <param name="previousTranslationHashes">Corresponding hash codes of a previous translation (0 if unknown). The parameter may be <c>null</c>.</param>
		/// <param name="settings">The settings used for this operation.</param>
		/// <param name="mask">A <c>bool</c> array with the same cardinality as the TU array, specifying which TUs to add.</param>
		/// <returns>
		/// An array of <see cref="T:Sdl.LanguagePlatform.TranslationMemory.ImportResult" /> objects, which mirrors the translation unit array. It has the exact same size and contains the
		/// status of each add operation for each particular translation unit with the same index within the array.
		/// </returns>
		public ImportResult[] AddOrUpdateTranslationUnitsMasked(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings, bool[] mask)
		{
			try
			{
				return _tm.Service.AddOrUpdateTranslationUnitsMasked(_tm.Container, _tm.Setup.ResourceId, translationUnits, previousTranslationHashes, settings, mask);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		/// <summary>
		/// Retrieves the translation unit with the specified <paramref name="translationUnitId" /> from the
		/// translation memory.
		/// </summary>
		/// <param name="translationUnitId">The ID of the translation unit to retrieve.</param>
		/// <returns>
		/// The translation unit with the specified ID, or <c>null</c> if that TU does not
		/// exist.
		/// </returns>
		public TranslationUnit GetTranslationUnit(PersistentObjectToken translationUnitId)
		{
			try
			{
				return _tm.Service.GetTranslationUnit(_tm.Container, _tm.Setup.ResourceId, translationUnitId);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		/// <summary>
		/// Retrieves a set of translation units, using an iterator. At most
		/// <see cref="P:Sdl.LanguagePlatform.TranslationMemory.TranslationMemoryIterator.MaxCount" /> translation units will be returned in one
		/// round-trip.
		/// <remarks>
		/// 		<list type="bullet">
		/// 			<item>A <see cref="T:Sdl.LanguagePlatform.TranslationMemory.FilterExpression" /> can be set on the iterator in which case
		/// only those TUs which satisfy the filter condition will be returned.</item>
		/// 			<item>See also the remarks on <see cref="P:Sdl.LanguagePlatform.TranslationMemory.TranslationMemoryIterator.MaxScan" /> on how to avoid
		/// timeouts with filtered iteration.</item>
		/// 		</list>
		/// 	</remarks>
		/// </summary>
		/// <param name="iterator">The iterator</param>
		/// <returns>
		/// The translation units, or <c>null</c> if no more translation units can be
		/// retrieved.
		/// </returns>
		public TranslationUnit[] GetTranslationUnits(ref RegularIterator iterator)
		{
			try
			{
				return _tm.Service.GetTranslationUnits(_tm.Container, _tm.Setup.ResourceId, ref iterator);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		/// <summary>
		/// Identical to <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.FileBasedTranslationMemoryLanguageDirection.GetTranslationUnits(Sdl.LanguagePlatform.TranslationMemory.RegularIterator@)" /> except that the implementation will
		/// attempt to populate the <see cref="P:Sdl.LanguagePlatform.TranslationMemory.TuContext.Segment1" /> and <see cref="P:Sdl.LanguagePlatform.TranslationMemory.TuContext.Segment2" /> properties for
		/// any items in the <see cref="P:Sdl.LanguagePlatform.TranslationMemory.TranslationUnit.Contexts" /> collection of a tu
		/// </summary>
		/// <param name="iterator"></param>
		/// <returns></returns>
		/// <remarks>Implementations must ensure <see cref="P:Sdl.LanguagePlatform.TranslationMemory.TuContext.Segment1" /> and <see cref="P:Sdl.LanguagePlatform.TranslationMemory.TuContext.Segment2" /> are tokenized and that their <see cref="P:Sdl.LanguagePlatform.Core.Segment.Tokens" /> collections are returned to the client.</remarks>
		public TranslationUnit[] GetTranslationUnitsWithContextContent(ref RegularIterator iterator)
		{
			try
			{
				return _tm.Service.GetTranslationUnitsWithContextContent(_tm.Container, _tm.Setup.ResourceId, ref iterator);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		/// <summary>
		/// Returns a set of potentially duplicated translation units in the TM, using a special
		/// <see cref="T:Sdl.LanguagePlatform.TranslationMemory.DuplicateIterator" />. A group of TUs are considered potential duplicates
		/// of each other if the internal hash values for their source segments are identical.
		/// There may be several reasons for this being the case:
		/// <list type="bullet">
		/// 		<item>The TUs have the same source segment, but different translations</item>
		/// 		<item>The TUs have different source segments, but only differ in complex
		/// tokens (controlled through the translation memory's <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationMemory.Recognizers" />) which will result in
		/// the identical hash values,</item>
		/// 		<item>The source segments only differ by whitespace or certain punctuation, which do not
		/// modify the hash values,</item>
		/// 		<item>The hashing algorithm leads to collisions in which case the segments may be
		/// entirely different, but still result in the same hash value.</item>
		/// 	</list>
		/// </summary>
		/// <param name="iterator">The iterator to use</param>
		/// <returns>
		/// The translation units which are potential duplicates, or <c>null</c> if no
		/// more potential duplicates can be found.
		/// </returns>
		public TranslationUnit[] GetDuplicateTranslationUnits(ref DuplicateIterator iterator)
		{
			try
			{
				return _tm.Service.GetDuplicateTranslationUnits(_tm.Container, _tm.Setup.ResourceId, ref iterator);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		/// <summary>
		/// Returns the number of translation units in this TM. Note that the computation of the TU
		/// count may be a time-consuming operation for some back-end storage systems.
		/// </summary>
		/// <returns>The number of translation units.</returns>
		public int GetTranslationUnitCount()
		{
			try
			{
				return _tm.Service.GetTuCount(_tm.Container, _tm.Setup.ResourceId);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		/// <summary>
		/// Performs a segment search.
		/// </summary>
		/// <param name="settings">The settings that define the search parameters.</param>
		/// <param name="segment">The segment to search for.</param>
		/// <returns>
		/// A <see cref="T:Sdl.LanguagePlatform.TranslationMemory.SearchResults" /> object containing the results or an empty object if no results were found.
		/// </returns>
		public SearchResults SearchSegment(SearchSettings settings, Segment segment)
		{
			try
			{
				return _tm.Service.SearchSegment(_tm.Container, _tm.Setup.ResourceId, settings, segment);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		/// <summary>
		///
		/// </summary>
		public SegmentAndSubsegmentSearchResults SearchSegment(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, Segment segment)
		{
			try
			{
				return (_tm.Service as ISubsegmentTranslationMemoryService)?.SearchSegment(_tm.Container, _tm.Setup.ResourceId, settings, subsegmentSettings, condition, segment);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		/// <summary>
		/// Performs a subsegment search on the given segment
		/// </summary>
		/// <param name="settings">Subsegment search settings</param>
		/// <param name="segment">Segment to use in search query</param>
		/// <returns>
		/// A <see cref="T:Sdl.LanguagePlatform.TranslationMemory.SubsegmentSearchResultsCollection" /> object containing the results or an empty object if no results were found.
		/// </returns>
		public SubsegmentSearchResultsCollection SubsegmentSearchSegment(SubsegmentSearchSettings settings, Segment segment)
		{
			try
			{
				return (_tm.Service as ISubsegmentTranslationMemoryService)?.SubsegmentSearchSegment(_tm.Container, _tm.Setup.ResourceId, settings, segment);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		/// <summary>
		/// Performs a subsegment search on the given array of segments
		/// </summary>
		/// <param name="settings">Subsegment search settings</param>
		/// <param name="segments">Segment to use in search query</param>
		/// <returns>
		/// A <see cref="T:Sdl.LanguagePlatform.TranslationMemory.SubsegmentSearchResultsCollection" /> array containing the results or an empty array if no results were found.
		/// </returns>
		public SubsegmentSearchResultsCollection[] SubsegmentSearchSegments(SubsegmentSearchSettings settings, Segment[] segments)
		{
			try
			{
				return (_tm.Service as ISubsegmentTranslationMemoryService)?.SubsegmentSearchSegments(_tm.Container, _tm.Setup.ResourceId, settings, segments);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		/// <summary>
		/// Performs a search for an array of segments.
		/// </summary>
		/// <param name="settings">The settings that define the search parameters.</param>
		/// <param name="segments">The array containing the segments to search for.</param>
		/// <returns>
		/// An array of <see cref="T:Sdl.LanguagePlatform.TranslationMemory.SearchResults" /> objects, which mirrors the segments array. It has the exact same size and contains the
		/// search results for each segment with the same index within the segments array.
		/// </returns>
		public SearchResults[] SearchSegments(SearchSettings settings, Segment[] segments)
		{
			return SearchSegmentsMasked(settings, segments, null);
		}

		/// <summary>
		///
		/// </summary>
		public SegmentAndSubsegmentSearchResults[] SearchSegments(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, Segment[] segments)
		{
			return SearchSegmentsMasked(settings, subsegmentSettings, condition, segments, null);
		}

		/// <summary>
		/// Performs a search for an array of segments, specifying a mask which specifies which segments should actually be
		/// searched (only those for which the corresponding mask bit is <c>true</c> are searched). If the mask is <c>null</c>, the method
		/// behaves identically to <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.FileBasedTranslationMemoryLanguageDirection.SearchSegments(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.Core.Segment[])" />. Passing a mask only makes sense in document search contexts (<see cref="P:Sdl.LanguagePlatform.TranslationMemory.SearchSettings.IsDocumentSearch" />
		/// set to <c>true</c>).
		/// </summary>
		/// <param name="settings">The settings that define the search parameters.</param>
		/// <param name="segments">The array containing the segments to search for.</param>
		/// <param name="mask">The array containing the segments to search for.</param>
		/// <returns>
		/// An array of <see cref="T:Sdl.LanguagePlatform.TranslationMemory.SearchResults" /> objects, which mirrors the segments array. It has the exact same size and contains the
		/// search results for each segment with the same index within the segments array.
		/// </returns>
		public SearchResults[] SearchSegmentsMasked(SearchSettings settings, Segment[] segments, bool[] mask)
		{
			try
			{
				return _tm.Service.SearchSegmentsMasked(_tm.Container, _tm.Setup.ResourceId, settings, segments, mask);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		/// <summary>
		///
		/// </summary>
		public SegmentAndSubsegmentSearchResults[] SearchSegmentsMasked(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, Segment[] segments, bool[] mask)
		{
			try
			{
				return (_tm.Service as ISubsegmentTranslationMemoryService)?.SearchSegmentsMasked(_tm.Container, _tm.Setup.ResourceId, settings, subsegmentSettings, condition, segments, mask);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		/// <summary>
		/// Performs a text search.
		/// </summary>
		/// <param name="settings">The settings that define the search parameters.</param>
		/// <param name="segment">The text to search for.</param>
		/// <returns>
		/// A <see cref="T:Sdl.LanguagePlatform.TranslationMemory.SearchResults" /> object containing the results or an empty object if no results were found.
		/// </returns>
		public SearchResults SearchText(SearchSettings settings, string segment)
		{
			try
			{
				return _tm.Service.SearchText(_tm.Container, _tm.Setup.ResourceId, settings, segment);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		/// <summary>
		/// Performs a translation unit search.
		/// </summary>
		/// <param name="settings">The settings that define the search parameters.</param>
		/// <param name="translationUnit">The translation unit to search for.</param>
		/// <returns>
		/// A <see cref="T:Sdl.LanguagePlatform.TranslationMemory.SearchResults" /> object containing the results or an empty object if no results were found.
		/// </returns>
		public SearchResults SearchTranslationUnit(SearchSettings settings, TranslationUnit translationUnit)
		{
			try
			{
				return _tm.Service.SearchTranslationUnit(_tm.Container, _tm.Setup.ResourceId, settings, translationUnit);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		/// <summary>
		///
		/// </summary>
		public SegmentAndSubsegmentSearchResults SearchTranslationUnit(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, TranslationUnit translationUnit)
		{
			try
			{
				return (_tm.Service as ISubsegmentTranslationMemoryService)?.SearchTranslationUnit(_tm.Container, _tm.Setup.ResourceId, settings, subsegmentSettings, condition, translationUnit);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		/// <summary>
		/// Performs a translation unit search for an array of translation units.
		/// </summary>
		/// <param name="settings">The settings that define the search parameters.</param>
		/// <param name="translationUnits">The array containing the translation units to search for.</param>
		/// <returns>
		/// An array of <see cref="T:Sdl.LanguagePlatform.TranslationMemory.SearchResults" /> objects, which mirrors the translation unit array. It has the exact same size and contains
		/// the search results for each translation unit with the same index within the translation unit array.
		/// </returns>
		public SearchResults[] SearchTranslationUnits(SearchSettings settings, TranslationUnit[] translationUnits)
		{
			return SearchTranslationUnitsMasked(settings, translationUnits, null);
		}

		/// <summary>
		///
		/// </summary>
		public SegmentAndSubsegmentSearchResults[] SearchTranslationUnits(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, TranslationUnit[] translationUnits)
		{
			return SearchTranslationUnitsMasked(settings, subsegmentSettings, condition, translationUnits, null);
		}

		/// <summary>
		/// Similar to <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.FileBasedTranslationMemoryLanguageDirection.SearchTranslationUnits(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[])" />, but allows passing a mask which specifies which TUs are actually searched. This is useful
		/// in document search contexts where some TUs are passed which should be used to establish a (text) context, but which should not be
		/// processed.
		/// </summary>
		/// <param name="settings">The settings that define the search parameters.</param>
		/// <param name="translationUnits">The array containing the translation units to search for.</param>
		/// <param name="mask">A <c>bool</c> array which specifies which TUs are actually searched (mask[i] = <c>true</c>). If <c>null</c>, the method
		/// behaves identically to <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.FileBasedTranslationMemoryLanguageDirection.SearchTranslationUnits(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[])" />.</param>
		/// <returns>
		/// An array of <see cref="T:Sdl.LanguagePlatform.TranslationMemory.SearchResults" /> objects, which mirrors the translation unit array. It has the exact same size and contains
		/// the search results for each translation unit with the same index within the translation unit array.
		/// </returns>
		public virtual SearchResults[] SearchTranslationUnitsMasked(SearchSettings settings, TranslationUnit[] translationUnits, bool[] mask)
		{
			try
			{
				return _tm.Service.SearchTranslationUnitsMasked(_tm.Container, _tm.Setup.ResourceId, settings, translationUnits, mask);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		/// <summary>
		///
		/// </summary>
		public virtual SegmentAndSubsegmentSearchResults[] SearchTranslationUnitsMasked(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, TranslationUnit[] translationUnits, bool[] mask)
		{
			try
			{
				ISubsegmentTranslationMemoryService subsegmentTranslationMemoryService = _tm.Service as ISubsegmentTranslationMemoryService;
				if (subsegmentTranslationMemoryService == null)
				{
					return null;
				}
				SearchSettings settings2 = FileBasedTMSettingsCloner.CloneSettings(settings);
				return subsegmentTranslationMemoryService.SearchTranslationUnitsMasked(_tm.Container, _tm.Setup.ResourceId, settings2, subsegmentSettings, condition, translationUnits, mask);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		/// <summary>
		/// Updates the properties and fields of an existing translation unit if the source and target segments are unchanged or
		/// adds a new translation unit otherwise. If the provider doesn't support adding/updating, the
		/// implementation should return a reasonable ImportResult but should not throw an exception.
		/// <para>The translation unit should be initialized in a previous call to the translation memory, so that the ID property is set to a
		/// valid value.</para>
		/// </summary>
		/// <param name="translationUnit">The translation unit to be updated.</param>
		/// <returns>The result of the operation.</returns>
		public ImportResult UpdateTranslationUnit(TranslationUnit translationUnit)
		{
			try
			{
				return _tm.Service.UpdateTranslationUnit(_tm.Container, _tm.Setup.ResourceId, translationUnit);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		/// <summary>
		/// Updates the properties and fields of an array of existing translation units if the source and target segments are unchanged or
		/// adds new translation units otherwise. If the provider doesn't support adding/updating, the
		/// implementation should return a reasonable ImportResult but should not throw an exception.
		/// <para>The translation units should be initialized in previous calls to the translation memory, so that their ID properties
		/// are set to valid values.</para>
		/// </summary>
		/// <param name="translationUnits">The translation unit array to be updated.</param>
		/// <returns>
		/// An array of results which mirrors the translation unit array. It has the exact same size and contains the
		/// results for each translation unit with the same index within the translation unit array.
		/// </returns>
		public ImportResult[] UpdateTranslationUnits(TranslationUnit[] translationUnits)
		{
			try
			{
				return _tm.Service.UpdateTranslationUnits(_tm.Container, _tm.Setup.ResourceId, translationUnits);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		/// <summary>
		/// Updates (adds, overwrites, or merges) the specified translation units.
		/// </summary>
		/// <param name="translationUnits">The translation units to update.</param>
		/// <param name="mask">An optional mask with the same length as <paramref name="translationUnits" />. If
		/// provided, only those TUs in <paramref name="translationUnits" /> will be updated for which the corresponding
		/// flag in <paramref name="mask" /> is <code>true</code>. Those TUs for which the corresponding
		/// flag is <code>false</code> will only be used to establish context information. If no masking
		/// is required, this parameter can be null.</param>
		/// <returns>
		/// An array of <see cref="T:Sdl.LanguagePlatform.TranslationMemory.ImportResult" />s with the same length as <paramref name="translationUnits" />,
		/// which indicates, for each translation unit, the action taken by the translation memory.
		/// </returns>
		public ImportResult[] UpdateTranslationUnitsMasked(TranslationUnit[] translationUnits, bool[] mask)
		{
			try
			{
				return _tm.Service.UpdateTranslationUnitsMasked(_tm.Container, _tm.Setup.ResourceId, translationUnits, mask);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		/// <summary>
		/// Applies the specified field values to the translation unit identified by the specified
		/// <paramref name="translationUnitId" />.
		/// </summary>
		/// <param name="values">The field values to apply</param>
		/// <param name="overwrite">If true, overwrite the old values, and merge the new values with the old ones otherwise.</param>
		/// <param name="translationUnitId">The identifier of the translation unit to apply the field values to</param>
		/// <returns>
		/// true if the translation unit has been modified. false otherwise.
		/// </returns>
		public bool ApplyFieldsToTranslationUnit(FieldValues values, bool overwrite, PersistentObjectToken translationUnitId)
		{
			try
			{
				return _tm.Service.ApplyFieldsToTranslationUnit(_tm.Container, _tm.Setup.ResourceId, values, overwrite, translationUnitId);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		/// <summary>
		/// Applies the specified field values to the translation units identified by the specified
		/// <paramref name="translationUnitIds" /> collection.
		/// </summary>
		/// <param name="values">The field values to apply</param>
		/// <param name="overwrite">If true, overwrite the old values, and merge the new values with the old ones otherwise.</param>
		/// <param name="translationUnitIds">A collection of translation unit IDs to apply the field values to</param>
		/// <returns>The number of changed translation units</returns>
		public int ApplyFieldsToTranslationUnits(FieldValues values, bool overwrite, PersistentObjectToken[] translationUnitIds)
		{
			try
			{
				return _tm.Service.ApplyFieldsToTranslationUnits(_tm.Container, _tm.Setup.ResourceId, values, overwrite, translationUnitIds);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		/// <summary>
		/// Applies the specified edit script to the translation units in a TM, but unlike <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.FileBasedTranslationMemoryLanguageDirection.EditTranslationUnitsWithIterator(Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditScript,Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditUpdateMode,Sdl.LanguagePlatform.TranslationMemory.RegularIterator@)" />,
		/// the TUs will not be updated in the TM, but modified copies of the TUs will be returned to the caller. This
		/// can be used to "preview" the changes the edit script would apply.
		/// </summary>
		/// <param name="editScript">The edit script</param>
		/// <param name="iterator">An iterator. See also the iterator-related remarks for <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.FileBasedTranslationMemoryLanguageDirection.EditTranslationUnitsWithIterator(Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditScript,Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditUpdateMode,Sdl.LanguagePlatform.TranslationMemory.RegularIterator@)" />.</param>
		/// <returns></returns>
		public TranslationUnit[] PreviewEditTranslationUnitsWithIterator(EditScript editScript, ref RegularIterator iterator)
		{
			try
			{
				return _tm.Service.PreviewEditTranslationUnitsWithIterator(_tm.Container, _tm.Setup.ResourceId, editScript, ref iterator);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		/// <summary>
		/// Determines whether the specified <see cref="T:System.Object" /> is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="T:System.Object" /> to compare with this instance.</param>
		/// <returns>
		/// 	<c>true</c> if the specified <see cref="T:System.Object" /> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		/// <exception cref="T:System.NullReferenceException">The <paramref name="obj" /> parameter is <c>null</c>.</exception>
		public override bool Equals(object obj)
		{
			FileBasedTranslationMemoryLanguageDirection fileBasedTranslationMemoryLanguageDirection = obj as FileBasedTranslationMemoryLanguageDirection;
			if (fileBasedTranslationMemoryLanguageDirection == null)
			{
				return false;
			}
			if (fileBasedTranslationMemoryLanguageDirection.TranslationProvider.Uri.Equals(TranslationProvider.Uri))
			{
				return fileBasedTranslationMemoryLanguageDirection.LanguageDirection.Equals(LanguageDirection);
			}
			return false;
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode()
		{
			return TranslationProvider.Uri.GetHashCode();
		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public List<SubsegmentMatchType> SupportedSubsegmentMatchTypes()
		{
			try
			{
				if (_tm.Service is ISubsegmentTranslationMemoryService)
				{
					ISubsegmentTranslationMemoryService subsegmentTranslationMemoryService = _tm.Service as ISubsegmentTranslationMemoryService;
					return subsegmentTranslationMemoryService.SupportedSubsegmentMatchTypes(_tm.Container, _tm.Setup.ResourceId);
				}
				return new List<SubsegmentMatchType>();
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}
	}
}
