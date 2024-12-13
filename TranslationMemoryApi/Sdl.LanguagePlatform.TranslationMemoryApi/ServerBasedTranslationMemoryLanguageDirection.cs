using Sdl.Core.Api.DataAccess;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemory.EditScripts;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Represents a language direction of a multilingual server-based translation memory (<see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedTranslationMemory" />).
	/// </summary>
	public class ServerBasedTranslationMemoryLanguageDirection : ISubsegmentTranslationMemoryLanguageDirection, ITranslationMemoryLanguageDirection, ITranslationProviderLanguageDirection
	{
		private ServerBasedTranslationMemory _lazyTranslationMemory;

		internal LanguageDirectionEntity Entity
		{
			get;
			set;
		}

		internal TranslationProviderServer TranslationProviderServer
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the server-based translation memory to which this language direction belongs.
		/// </summary>
		/// <value></value>
		public ServerBasedTranslationMemory TranslationProvider
		{
			get
			{
				if (_lazyTranslationMemory == null)
				{
					TranslationMemoryEntity translationMemoryById = TranslationProviderServer.Service.GetTranslationMemoryById(Entity.TranslationMemory.ForeignKey, TranslationProviderServer.GetDefaultTranslationMemoryRelationships(), includeLanguageResourceData: false, includeScheduledOperations: false);
					_lazyTranslationMemory = new ServerBasedTranslationMemory(TranslationProviderServer, translationMemoryById);
				}
				return _lazyTranslationMemory;
			}
			internal set
			{
				_lazyTranslationMemory = value;
				if (_lazyTranslationMemory != null)
				{
					TranslationProviderServer = _lazyTranslationMemory.TranslationProviderServer;
					Entity.TranslationMemory = new EntityReference<TranslationMemoryEntity>(_lazyTranslationMemory.Entity);
				}
				else
				{
					TranslationProviderServer = null;
				}
			}
		}

		/// <summary>
		/// Gets the server-based translation memory to which this language direction belongs.
		/// </summary>
		/// <value></value>
		ITranslationMemory ITranslationMemoryLanguageDirection.TranslationProvider => TranslationProvider;

		/// <summary>
		/// Gets the cached total translation unit count for this language direction.
		/// This count is computed at regular intervals and when performing imports.
		/// To calculate the actual translation unit count, use <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedTranslationMemoryLanguageDirection.GetTranslationUnitCount" />
		/// but bear in mind that this is a relatively expensive operation.
		/// </summary>
		public int CachedTranslationUnitCount
		{
			get
			{
				if (Entity.TuCount.HasValue && Entity.TuCount.HasValue)
				{
					return Entity.TuCount.Value;
				}
				return 0;
			}
		}

		/// <summary>
		/// Gets the translation memory to which this language direction belongs.
		/// </summary>
		ITranslationProvider ITranslationProviderLanguageDirection.TranslationProvider => TranslationProvider;

		/// <summary>
		/// Gets or sets the source language.
		/// </summary>
		/// <exception cref="T:System.InvalidOperationException">Thrown when trying to set the source language after the language direction has been created.</exception>
		public CultureInfo SourceLanguage
		{
			get
			{
				if (!string.IsNullOrEmpty(Entity.SourceCultureName))
				{
					return CultureInfoExtensions.GetCultureInfo(Entity.SourceCultureName);
				}
				return null;
			}
			set
			{
				if (!IsNewObject)
				{
					throw new InvalidOperationException("You cannot change the source language after a language direction has been created.");
				}
				Entity.SourceCultureName = value?.Name;
			}
		}

		/// <summary>
		/// Gets or sets the target language.
		/// </summary>
		/// <exception cref="T:System.InvalidOperationException">Thrown when trying to set the target language after the language direction has been created.</exception>
		public CultureInfo TargetLanguage
		{
			get
			{
				if (!string.IsNullOrEmpty(Entity.TargetCultureName))
				{
					return CultureInfoExtensions.GetCultureInfo(Entity.TargetCultureName);
				}
				return null;
			}
			set
			{
				if (!IsNewObject)
				{
					throw new InvalidOperationException("You cannot change the target language after a language direction has been created.");
				}
				Entity.TargetCultureName = value?.Name;
			}
		}

		/// <summary>
		/// Gets or sets the source language code.
		/// </summary>
		/// <exception cref="T:System.InvalidOperationException">Thrown when trying to set the source language after the language direction has been created.</exception>
		public string SourceLanguageCode
		{
			get
			{
				return Entity.SourceCultureName;
			}
			set
			{
				if (!IsNewObject)
				{
					throw new InvalidOperationException("You cannot change the source language after a language direction has been created.");
				}
				Entity.SourceCultureName = value;
			}
		}

		/// <summary>
		/// Gets or sets the target language code.
		/// </summary>
		/// <exception cref="T:System.InvalidOperationException">Thrown when trying to set the target language after the language direction has been created.</exception>
		public string TargetLanguageCode
		{
			get
			{
				return Entity.TargetCultureName;
			}
			set
			{
				if (!IsNewObject)
				{
					throw new InvalidOperationException("You cannot change the target language after a language direction has been created.");
				}
				Entity.TargetCultureName = value;
			}
		}

		/// <summary>
		/// Gets a flag which indicates whether the translation provider supports
		/// searches in the reversed language direction.
		/// </summary>
		/// <value></value>
		public bool CanReverseLanguageDirection => false;

		private bool IsNewObject
		{
			get
			{
				if (Entity.Id != null)
				{
					return Entity.Id.Value == null;
				}
				return true;
			}
		}

		internal static ServerBasedTranslationMemoryLanguageDirection BuildServerBasedTranslationMemoryLanguageDirection(ClientObjectBuilder builder, LanguageDirectionEntity entity)
		{
			ClientObjectKey key = builder.CreateKey(entity);
			ServerBasedTranslationMemoryLanguageDirection serverBasedTranslationMemoryLanguageDirection = builder[key] as ServerBasedTranslationMemoryLanguageDirection;
			if (serverBasedTranslationMemoryLanguageDirection != null)
			{
				return serverBasedTranslationMemoryLanguageDirection;
			}
			serverBasedTranslationMemoryLanguageDirection = (ServerBasedTranslationMemoryLanguageDirection)(builder[key] = new ServerBasedTranslationMemoryLanguageDirection(builder.Server, entity));
			serverBasedTranslationMemoryLanguageDirection.TranslationProvider = ServerBasedTranslationMemory.BuildServerBasedTranslationMemory(builder, entity.TranslationMemory.Entity);
			return serverBasedTranslationMemoryLanguageDirection;
		}

		internal static ServerBasedTranslationMemoryLanguageDirection BuildServerBasedTranslationMemoryLanguageDirection(ClientObjectBuilder builder, LanguageDirectionEntity langDirEntity, TranslationMemoryEntity tmEntity)
		{
			ClientObjectKey key = builder.CreateKey(langDirEntity);
			ServerBasedTranslationMemoryLanguageDirection serverBasedTranslationMemoryLanguageDirection = builder[key] as ServerBasedTranslationMemoryLanguageDirection;
			if (serverBasedTranslationMemoryLanguageDirection != null)
			{
				return serverBasedTranslationMemoryLanguageDirection;
			}
			serverBasedTranslationMemoryLanguageDirection = new ServerBasedTranslationMemoryLanguageDirection(builder.Server, langDirEntity);
			serverBasedTranslationMemoryLanguageDirection.TranslationProvider = ServerBasedTranslationMemory.BuildServerBasedTranslationMemory(builder, tmEntity);
			builder[key] = serverBasedTranslationMemoryLanguageDirection;
			return serverBasedTranslationMemoryLanguageDirection;
		}

		/// <summary>
		/// Creates a new, empty language direction.
		/// </summary>
		/// <remarks>In order to add a new language direction to a server-based translation memory, create a new <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedTranslationMemoryLanguageDirection" />,
		/// set its <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedTranslationMemoryLanguageDirection.SourceLanguage" /> and <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedTranslationMemoryLanguageDirection.TargetLanguage" /> properties, then add it to the language directions of the translation memory (<see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedTranslationMemory.LanguageDirections" />)
		/// and save the translation memory (<see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedTranslationMemory.Save" />).</remarks>
		public ServerBasedTranslationMemoryLanguageDirection()
		{
			Entity = new LanguageDirectionEntity();
			Entity.UniqueId = Guid.NewGuid();
		}

		internal ServerBasedTranslationMemoryLanguageDirection(TranslationProviderServer server, LanguageDirectionEntity entity)
		{
			if (server == null)
			{
				throw new ArgumentNullException("server");
			}
			if (entity == null)
			{
				throw new ArgumentNullException("entity");
			}
			if (entity.Id == null || entity.Id.Value == null)
			{
				throw new ArgumentException("The language direction entity should have a valid id.", "entity");
			}
			TranslationProviderServer = server;
			Entity = entity;
		}

		internal ServerBasedTranslationMemoryLanguageDirection(ServerBasedTranslationMemory translationMemory, LanguageDirectionEntity entity)
		{
			if (translationMemory == null)
			{
				throw new ArgumentNullException("translationMemory");
			}
			if (entity == null)
			{
				throw new ArgumentNullException("entity");
			}
			if (entity.Id == null || entity.Id.Value == null)
			{
				throw new ArgumentException("The language direction entity should have a valid id.", "entity");
			}
			TranslationProviderServer = translationMemory.TranslationProviderServer;
			_lazyTranslationMemory = translationMemory;
			Entity = entity;
		}

		/// <summary>
		/// Updates the cached translation unit count. Normally, the translation unit count is updated automatically
		/// every night. Only use this method to ensure the translation unit count reported by <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedTranslationMemoryLanguageDirection.CachedTranslationUnitCount" /> after adding
		/// a large amount of translation units. There is no need to call this method after an import using a <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.TranslationMemoryImporter" />
		/// or a <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ScheduledServerTranslationMemoryImport" />; these classes automatically update the cached translation unit count. 
		/// </summary>
		public void UpdateCachedTranslationUnitCount()
		{
			Entity.TuCount = TranslationProviderServer.Service.UpdateTranslationUnitCount(Entity.LanguageDirectionIdentity);
		}

		/// <summary>
		/// Returns the number of translation units in this TM.
		/// </summary>
		/// <returns></returns>
		public int GetTranslationUnitCount()
		{
			return TranslationProviderServer.Service.GetTuCount(Entity.LanguageDirectionIdentity);
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
			return TranslationProviderServer.Service.ApplyFieldsToTranslationUnit(Entity.LanguageDirectionIdentity, values, overwrite, translationUnitId);
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
			return TranslationProviderServer.Service.ApplyFieldsToTranslationUnits(Entity.LanguageDirectionIdentity, values, overwrite, translationUnitIds);
		}

		/// <summary>
		/// Deletes the translation unit with the specified <paramref name="translationUnitId" /> from the TM.
		/// </summary>
		/// <param name="translationUnitId">The ID of the translation unit to delete.</param>
		/// <returns>
		/// true if the translation unit was deleted, false otherwise.
		/// </returns>
		public bool DeleteTranslationUnit(PersistentObjectToken translationUnitId)
		{
			return TranslationProviderServer.Service.DeleteTranslationUnit(Entity.LanguageDirectionIdentity, translationUnitId);
		}

		/// <summary>
		/// Deletes all translation units from the TM.
		/// </summary>
		/// <returns>The number of deleted translation units</returns>
		public int DeleteAllTranslationUnits()
		{
			return TranslationProviderServer.Service.DeleteAllTranslationUnits(Entity.LanguageDirectionIdentity);
		}

		/// <summary>
		/// Deletes the translation units with the specified IDs from the translation memory.
		/// </summary>
		/// <param name="translationUnitIds">A collection of the translation unit IDs to delete</param>
		/// <returns>The number of deleted translation units</returns>
		public int DeleteTranslationUnits(PersistentObjectToken[] translationUnitIds)
		{
			return TranslationProviderServer.Service.DeleteTranslationUnits(Entity.LanguageDirectionIdentity, translationUnitIds);
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
			return TranslationProviderServer.Service.DeleteTranslationUnitsWithIterator(Entity.LanguageDirectionIdentity, ref iterator);
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
			return TranslationProviderServer.Service.EditTranslationUnits(Entity.LanguageDirectionIdentity, editScript, updateMode, translationUnitIds);
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
			return TranslationProviderServer.Service.EditTranslationUnitsWithIterator(Entity.LanguageDirectionIdentity, editScript, updateMode, ref iterator);
		}

		/// <summary>
		/// Applies the specified edit script to the translation units in a TM, but unlike <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedTranslationMemoryLanguageDirection.EditTranslationUnitsWithIterator(Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditScript,Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditUpdateMode,Sdl.LanguagePlatform.TranslationMemory.RegularIterator@)" />,
		/// the TUs will not be updated in the TM, but modified copies of the TUs will be returned to the caller. This
		/// can be used to "preview" the changes the edit script would apply.
		/// </summary>
		/// <param name="editScript">The edit script</param>
		/// <param name="iterator">An iterator. See also the iterator-related remarks for <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedTranslationMemoryLanguageDirection.EditTranslationUnitsWithIterator(Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditScript,Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditUpdateMode,Sdl.LanguagePlatform.TranslationMemory.RegularIterator@)" />.</param>
		/// <returns></returns>
		public TranslationUnit[] PreviewEditTranslationUnitsWithIterator(EditScript editScript, ref RegularIterator iterator)
		{
			return TranslationProviderServer.Service.PreviewEditTranslationUnitsWithIterator(Entity.LanguageDirectionIdentity, editScript, ref iterator);
		}

		/// <summary>
		/// Returns a set of potentially duplicated translation units in the TM, using a special
		/// <see cref="T:Sdl.LanguagePlatform.TranslationMemory.DuplicateIterator" />. A group of TUs are considered potential duplicates
		/// of each other if the internal hash values for their source segments are identical.
		/// There may be several reasons for this being the case:
		/// <list type="bullet">
		/// 		<item>The TUs have the same source segment, but different translations</item>
		/// 		<item>The TUs have different source segments, but only differ in complex
		/// tokens (controlled through the TM's <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationMemory.Recognizers" />) which will result in
		/// the identical hash values,</item>
		/// 		<item>The source segments only differ by whitespace or certain punctuation, which do not
		/// modify the hash values,</item>
		/// 		<item>The hashing algorithm leads to collisions in which case the segments may be
		/// entirely different, but still result in the same hash value.</item>
		/// 	</list>
		/// </summary>
		/// <param name="iterator">The iterator to use</param>
		/// <returns>
		/// The translation units which are potential duplicates, or null if no
		/// more potential duplicates can be found.
		/// </returns>
		public TranslationUnit[] GetDuplicateTranslationUnits(ref DuplicateIterator iterator)
		{
			return TranslationProviderServer.Service.GetDuplicateTranslationUnits(Entity.LanguageDirectionIdentity, ref iterator);
		}

		/// <summary>
		/// Retrieves the translation unit with the specified <paramref name="translationUnitId" /> from the
		/// translation memory.
		/// </summary>
		/// <param name="translationUnitId">The ID of the translation unit to retrieve.</param>
		/// <returns>
		/// The translation unit with the specified ID, or null if that TU does not
		/// exist.
		/// </returns>
		public TranslationUnit GetTranslationUnit(PersistentObjectToken translationUnitId)
		{
			return TranslationProviderServer.Service.GetTranslationUnit(Entity.LanguageDirectionIdentity, translationUnitId);
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
		/// The translation units, or null if no more translation units can be
		/// retrieved.
		/// </returns>
		public TranslationUnit[] GetTranslationUnits(ref RegularIterator iterator)
		{
			return TranslationProviderServer.Service.GetTranslationUnits(Entity.LanguageDirectionIdentity, ref iterator);
		}

		/// <summary>
		/// Identical to <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedTranslationMemoryLanguageDirection.GetTranslationUnits(Sdl.LanguagePlatform.TranslationMemory.RegularIterator@)" /> except that the implementation will
		/// attempt to populate the <see cref="P:Sdl.LanguagePlatform.TranslationMemory.TuContext.Segment1" /> and <see cref="P:Sdl.LanguagePlatform.TranslationMemory.TuContext.Segment2" /> properties for
		/// any items in the <see cref="P:Sdl.LanguagePlatform.TranslationMemory.TranslationUnit.Contexts" /> collection of a tu
		/// </summary>
		/// <param name="iterator"></param>
		/// <returns></returns>
		/// <remarks>Implementations must ensure <see cref="P:Sdl.LanguagePlatform.TranslationMemory.TuContext.Segment1" /> and <see cref="P:Sdl.LanguagePlatform.TranslationMemory.TuContext.Segment2" /> are tokenized and that their <see cref="P:Sdl.LanguagePlatform.Core.Segment.Tokens" /> collections are returned to the client.</remarks>
		public TranslationUnit[] GetTranslationUnitsWithContextContent(ref RegularIterator iterator)
		{
			throw new NotImplementedException();
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
			return TranslationProviderServer.Service.UpdateTranslationUnitsMasked(Entity.LanguageDirectionIdentity, translationUnits, mask);
		}

		/// <summary>
		/// Re-indexes the translation memory, using an iterator so that the client can update
		/// progress indicators or discontinue the process.
		/// <para>The same iterator instance should be passed in subsequent calls, in order to obtain
		/// the next page, and so on.</para>
		/// </summary>
		/// <param name="iterator">The iterator to use for the re-indexing process.</param>
		/// <returns>
		/// False if the iterator is at the end of the TM or the TM is empty, true otherwise.
		/// The re-indexing process should be continued until the method returns false.
		/// </returns>
		/// <remarks>For larger TMs (&gt; 100.000 TUs) it is recommended to also recompute the index
		/// statistics after the re-indexing finished (see <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationMemory.RecomputeFuzzyIndexStatistics" />).</remarks>
		public bool ReindexTranslationUnits(ref RegularIterator iterator)
		{
			return TranslationProviderServer.Service.ReindexTranslationUnits(Entity.LanguageDirectionIdentity, ref iterator);
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
			return TranslationProviderServer.Service.SearchSegment(Entity.LanguageDirectionIdentity, settings, segment);
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
			return TranslationProviderServer.Service.SearchSegments(Entity.LanguageDirectionIdentity, settings, segments);
		}

		/// <summary>
		/// Performs a search for an array of segments, specifying a mask which specifies which segments should actually be
		/// searched (only those for which the corresponding mask bit is <c>true</c> are searched). If the mask is <c>null</c>, the method
		/// behaves identically to <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedTranslationMemoryLanguageDirection.SearchSegments(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.Core.Segment[])" />. Passing a mask only makes sense in document search contexts (<see cref="P:Sdl.LanguagePlatform.TranslationMemory.SearchSettings.IsDocumentSearch" />
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
			return TranslationProviderServer.Service.SearchSegmentsMasked(Entity.LanguageDirectionIdentity, settings, segments, mask);
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
			return TranslationProviderServer.Service.SearchText(Entity.LanguageDirectionIdentity, settings, segment);
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
			return TranslationProviderServer.Service.SearchTranslationUnit(Entity.LanguageDirectionIdentity, settings, translationUnit);
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
			return TranslationProviderServer.Service.SearchTranslationUnits(Entity.LanguageDirectionIdentity, settings, translationUnits);
		}

		/// <summary>
		/// Similar to <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedTranslationMemoryLanguageDirection.SearchTranslationUnits(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[])" />, but allows passing a mask which specifies which TUs are actually searched. This is useful
		/// in document search contexts where some TUs are passed which should be used to establish a (text) context, but which should not be
		/// processed.
		/// </summary>
		/// <param name="settings">The settings that define the search parameters.</param>
		/// <param name="translationUnits">The array containing the translation units to search for.</param>
		/// <param name="mask">A <c>bool</c> array which specifies which TUs are actually searched (mask[i] = <c>true</c>). If <c>null</c>, the method
		/// behaves identically to <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedTranslationMemoryLanguageDirection.SearchTranslationUnits(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[])" />.</param>
		/// <returns>
		/// An array of <see cref="T:Sdl.LanguagePlatform.TranslationMemory.SearchResults" /> objects, which mirrors the translation unit array. It has the exact same size and contains
		/// the search results for each translation unit with the same index within the translation unit array.
		/// </returns>
		public SearchResults[] SearchTranslationUnitsMasked(SearchSettings settings, TranslationUnit[] translationUnits, bool[] mask)
		{
			return TranslationProviderServer.Service.SearchTranslationUnitsMasked(Entity.LanguageDirectionIdentity, settings, translationUnits, mask);
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
			return TranslationProviderServer.Service.AddTranslationUnit(Entity.LanguageDirectionIdentity, translationUnit, settings);
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
			return TranslationProviderServer.Service.AddTranslationUnits(Entity.LanguageDirectionIdentity, translationUnits, settings);
		}

		/// <summary>
		/// Adds an array of translation units to the database. If hash codes of the previous translations are provided,
		/// a found translation will be overwritten. If none is found, or the hash is 0 or the collection is <c>null</c>,
		/// the operation behaves identical to <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedTranslationMemoryLanguageDirection.AddTranslationUnits(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[],Sdl.LanguagePlatform.TranslationMemory.ImportSettings)" />.
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
			return TranslationProviderServer.Service.AddOrUpdateTranslationUnits(Entity.LanguageDirectionIdentity, translationUnits, previousTranslationHashes, settings);
		}

		/// <summary>
		/// Adds an array of translation units to the database, but will only add those
		/// for which the corresponding mask field is <c>true</c>. If the provider doesn't support adding/updating, the
		/// implementation should return a reasonable ImportResult but should not throw an exception.
		/// </summary>
		/// <param name="translationUnits">An arrays of translation units to be added.</param>
		/// <param name="settings">The settings used for this operation.</param>
		/// <param name="mask">A boolean array with the same cardinality as the TU array, specifying which TUs to add.</param>
		/// <returns>
		/// An array of ImportResult objects, which mirrors the translation unit array. It has the exact same size and contains the
		/// status of each add operation for each particular translation unit with the same index within the array.
		/// </returns>
		public ImportResult[] AddTranslationUnitsMasked(TranslationUnit[] translationUnits, ImportSettings settings, bool[] mask)
		{
			return TranslationProviderServer.Service.AddTranslationUnitsMasked(Entity.LanguageDirectionIdentity, translationUnits, settings, mask);
		}

		/// <summary>
		/// Adds an array of translation units to the database, but will only add those
		/// for which the corresponding mask field is true. If the previous translation hashes are provided,
		/// existing translations will be updated if the target segment hash changed.
		/// <para>
		/// If the provider doesn't support adding/updating, the
		/// implementation should return a reasonable ImportResult but should not throw an exception.
		/// </para>
		/// </summary>
		/// <param name="translationUnits">An arrays of translation units to be added.</param>
		/// <param name="previousTranslationHashes">Corresponding hash codes of a previous translation (0 if unknown). The parameter may be null.</param>
		/// <param name="settings">The settings used for this operation.</param>
		/// <param name="mask">A boolean array with the same cardinality as the TU array, specifying which TUs to add.</param>
		/// <returns>
		/// An array of ImportResult objects, which mirrors the translation unit array. It has the exact same size and contains the
		/// status of each add operation for each particular translation unit with the same index within the array.
		/// </returns>
		public ImportResult[] AddOrUpdateTranslationUnitsMasked(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings, bool[] mask)
		{
			return TranslationProviderServer.Service.AddOrUpdateTranslationUnitsMasked(Entity.LanguageDirectionIdentity, translationUnits, previousTranslationHashes, settings, mask);
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
			return TranslationProviderServer.Service.UpdateTranslationUnit(Entity.LanguageDirectionIdentity, translationUnit);
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
			return TranslationProviderServer.Service.UpdateTranslationUnits(Entity.LanguageDirectionIdentity, translationUnits);
		}

		/// <summary>
		/// Searches the TM for subsegment matches for a given segment
		/// </summary>
		/// <param name="settings">The settings that define the search parameters.</param>
		/// <param name="segment">The segment for which subsegment matches should be sought, aka 'query segment'</param>
		/// <returns>A <see cref="T:Sdl.LanguagePlatform.TranslationMemory.SubsegmentSearchResultsCollection" /> object containing any subsegment matches found.</returns>
		/// <remarks>See <see cref="T:Sdl.LanguagePlatform.TranslationMemory.SubsegmentSearchSettings" /> remarks for further information.</remarks>
		public SubsegmentSearchResultsCollection SubsegmentSearchSegment(SubsegmentSearchSettings settings, Segment segment)
		{
			return TranslationProviderServer.Service.SubsegmentSearchSegment(Entity.LanguageDirectionIdentity, settings, segment);
		}

		/// <summary>
		/// Searches the TM for subsegment matches for an array of segments.
		/// </summary>
		/// <param name="settings">The settings that define the search parameters.</param>
		/// <param name="segments">The array containing the segments for which subsegment matches should be sought</param>
		/// <returns>A <see cref="T:Sdl.LanguagePlatform.TranslationMemory.SubsegmentSearchResultsCollection" /> object containing any subsegment matches found.</returns>
		/// <remarks>See <see cref="T:Sdl.LanguagePlatform.TranslationMemory.SubsegmentSearchSettings" /> remarks for further information.</remarks>
		public SubsegmentSearchResultsCollection[] SubsegmentSearchSegments(SubsegmentSearchSettings settings, Segment[] segments)
		{
			return TranslationProviderServer.Service.SubsegmentSearchSegments(Entity.LanguageDirectionIdentity, settings, segments);
		}

		/// <summary>
		/// Reports the subsegment match types supported by this TM.
		/// </summary>
		public List<SubsegmentMatchType> SupportedSubsegmentMatchTypes()
		{
			return TranslationProviderServer.Service.SupportedSubsegmentMatchTypes(Entity.LanguageDirectionIdentity);
		}

		/// <summary>
		/// Performs a segment-level search, and optionally a subsegment search
		/// </summary>
		/// <param name="settings">The settings that define the segment-level search parameters.</param>
		/// <param name="subsegmentSettings">The settings that define the subsegment search parameters, or null if a subsegment search should not be performed.</param>
		/// <param name="segment">The segment to search for.</param>
		/// <param name="condition">If <paramref name="subsegmentSettings" /> is not null, specifies the conditions under which a subsegment search will be performed.</param>
		/// <returns>A <see cref="T:Sdl.LanguagePlatform.TranslationMemory.SearchResults" /> object containing the results or an empty object if no results were found.</returns>
		public SegmentAndSubsegmentSearchResults SearchSegment(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, Segment segment)
		{
			SegmentAndSubsegmentSearchResults segmentAndSubsegmentSearchResults = TranslationProviderServer.Service.SearchSegment(Entity.LanguageDirectionIdentity, settings, subsegmentSettings, condition, segment);
			if (segmentAndSubsegmentSearchResults == null)
			{
				SearchResults results = SearchSegment(settings, segment);
				segmentAndSubsegmentSearchResults = new SegmentAndSubsegmentSearchResults(results, null);
			}
			return segmentAndSubsegmentSearchResults;
		}

		/// <summary>
		/// Performs a segment-level search, and optionally a subsegment search, for an array of segments.
		/// </summary>
		/// <param name="settings">The settings that define the segment-level search parameters.</param>
		/// <param name="subsegmentSettings">The settings that define the subsegment search parameters, or null if a subsegment search should not be performed.</param>
		/// <param name="condition">If <paramref name="subsegmentSettings" /> is not null, specifies the conditions under which a subsegment search will be performed.</param>
		/// <param name="segments">The array containing the segments to search for.</param>
		/// <returns>An array of <see cref="T:Sdl.LanguagePlatform.TranslationMemory.SearchResults" /> objects, which mirrors the segments array. It has the exact same size and contains the
		/// search results for each segment with the same index within the segments array.</returns>
		public SegmentAndSubsegmentSearchResults[] SearchSegments(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, Segment[] segments)
		{
			SegmentAndSubsegmentSearchResults[] array = TranslationProviderServer.Service.SearchSegments(Entity.LanguageDirectionIdentity, settings, subsegmentSettings, condition, segments);
			if (array == null)
			{
				SearchResults[] results = SearchSegments(settings, segments);
				array = TranslationMemorySearchResultConverters.ToSegmentAndSubsegmentSearchResults(results);
			}
			return array;
		}

		/// <summary>
		/// Performs a segment-level search, and optionally a subsegment search, for an array of segments, specifying a mask which specifies which segments should actually be
		/// searched (only those for which the corresponding mask bit is <c>true</c> are searched). If the mask is <c>null</c>, the method
		/// behaves identically to <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedTranslationMemoryLanguageDirection.SearchSegments(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.Core.Segment[])" />. Passing a mask only makes sense in document search contexts (<see cref="P:Sdl.LanguagePlatform.TranslationMemory.SearchSettings.IsDocumentSearch" />
		/// set to <c>true</c>).
		/// </summary>
		/// <param name="settings">The settings that define the segment-level search parameters.</param>
		/// <param name="subsegmentSettings">The settings that define the subsegment search parameters, or null if a subsegment search should not be performed.</param>
		/// <param name="condition">If <paramref name="subsegmentSettings" /> is not null, specifies the conditions under which a subsegment search will be performed.</param>
		/// <param name="segments">The array containing the segments to search for.</param>
		/// <param name="mask">The array containing the segments to search for.</param>
		/// <returns>An array of <see cref="T:Sdl.LanguagePlatform.TranslationMemory.SearchResults" /> objects, which mirrors the segments array. It has the exact same size and contains the
		/// search results for each segment with the same index within the segments array.</returns>
		public SegmentAndSubsegmentSearchResults[] SearchSegmentsMasked(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, Segment[] segments, bool[] mask)
		{
			SegmentAndSubsegmentSearchResults[] array = TranslationProviderServer.Service.SearchSegmentsMasked(Entity.LanguageDirectionIdentity, settings, subsegmentSettings, condition, segments, mask);
			if (array == null)
			{
				SearchResults[] results = SearchSegmentsMasked(settings, segments, mask);
				array = TranslationMemorySearchResultConverters.ToSegmentAndSubsegmentSearchResults(results);
			}
			return array;
		}

		/// <summary>
		/// Performs a translation unit search, and optionally a subsegment search on the source segment
		/// </summary>
		/// <param name="settings">The settings that define the segment-level search parameters.</param>
		/// <param name="subsegmentSettings">The settings that define the subsegment search parameters, or null if a subsegment search should not be performed.</param>
		/// <param name="condition">If <paramref name="subsegmentSettings" /> is not null, specifies the conditions under which a subsegment search will be performed.</param>
		/// <param name="translationUnit">The translation unit to search for.</param>
		/// <returns>An object containing the results or an empty object if no results were found.</returns>
		public SegmentAndSubsegmentSearchResults SearchTranslationUnit(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, TranslationUnit translationUnit)
		{
			SegmentAndSubsegmentSearchResults segmentAndSubsegmentSearchResults = TranslationProviderServer.Service.SearchTranslationUnit(Entity.LanguageDirectionIdentity, settings, subsegmentSettings, condition, translationUnit);
			if (segmentAndSubsegmentSearchResults == null)
			{
				segmentAndSubsegmentSearchResults = new SegmentAndSubsegmentSearchResults(SearchTranslationUnit(settings, translationUnit), null);
			}
			return segmentAndSubsegmentSearchResults;
		}

		/// <summary>
		/// Performs a translation unit search, and optionally a subsegment search on the source segment, for an array of translation units.
		/// </summary>
		/// <param name="settings">The settings that define the segment-level search parameters.</param>
		/// <param name="subsegmentSettings">The settings that define the subsegment search parameters, or null if a subsegment search should not be performed.</param>
		/// <param name="condition">If <paramref name="subsegmentSettings" /> is not null, specifies the conditions under which a subsegment search will be performed.</param>
		/// <param name="translationUnits">The array containing the translation units to search for.</param>
		/// <returns>An array of objects, which mirrors the translation unit array. It has the exact same size and contains
		/// the search results for each translation unit with the same index within the translation unit array.</returns>
		public SegmentAndSubsegmentSearchResults[] SearchTranslationUnits(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, TranslationUnit[] translationUnits)
		{
			SegmentAndSubsegmentSearchResults[] array = TranslationProviderServer.Service.SearchTranslationUnits(Entity.LanguageDirectionIdentity, settings, subsegmentSettings, condition, translationUnits);
			if (array == null)
			{
				SearchResults[] results = SearchTranslationUnits(settings, translationUnits);
				array = TranslationMemorySearchResultConverters.ToSegmentAndSubsegmentSearchResults(results);
			}
			return array;
		}

		/// <summary>
		/// Similar to <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedTranslationMemoryLanguageDirection.SearchTranslationUnits(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[])" />, but allows passing a mask which specifies which TUs are actually searched. This is useful
		/// in document search contexts where some TUs are passed which should be used to establish a (text) context, but which should not be
		/// processed.
		/// </summary>
		/// <param name="settings">The settings that define the segment-level search parameters.</param>
		/// <param name="subsegmentSettings">The settings that define the subsegment search parameters, or null if a subsegment search should not be performed.</param>
		/// <param name="condition">If <paramref name="subsegmentSettings" /> is not null, specifies the conditions under which a subsegment search will be performed.</param>
		/// <param name="translationUnits">The array containing the translation units to search for.</param>
		/// <param name="mask">A <c>bool</c> array which specifies which TUs are actually searched (mask[i] = <c>true</c>). If <c>null</c>, the method
		/// behaves identically to <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedTranslationMemoryLanguageDirection.SearchTranslationUnits(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[])" />.
		/// </param>
		/// <returns>An array of objects, which mirrors the translation unit array. It has the exact same size and contains
		/// the search results for each translation unit with the same index within the translation unit array.</returns>
		public SegmentAndSubsegmentSearchResults[] SearchTranslationUnitsMasked(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, TranslationUnit[] translationUnits, bool[] mask)
		{
			SegmentAndSubsegmentSearchResults[] array = TranslationProviderServer.Service.SearchTranslationUnitsMasked(Entity.LanguageDirectionIdentity, settings, subsegmentSettings, condition, translationUnits, mask);
			if (array == null)
			{
				SearchResults[] results = SearchTranslationUnitsMasked(settings, translationUnits, mask);
				array = TranslationMemorySearchResultConverters.ToSegmentAndSubsegmentSearchResults(results);
			}
			return array;
		}

		/// <summary>
		///  Use server tm import feature to import TM
		/// </summary>
		public ImportEntity Import(ImportSettings settings, string fileName)
		{
			ImportEntity importInfo = new ImportEntity
			{
				LanguageDirection = new EntityReference<LanguageDirectionEntity>(Entity),
				ImportSettings = settings
			};
			return TranslationProviderServer.Service.QueueTranslationMemoryImport(importInfo, fileName, recomputeFuzzyIndex: true);
		}
	}
}
