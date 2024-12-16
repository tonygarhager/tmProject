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

		public ServerBasedTranslationMemory TranslationProvider
		{
			get
			{
				if (_lazyTranslationMemory == null)
				{
					TranslationMemoryEntity translationMemoryById = null;// TranslationProviderServer.Service.GetTranslationMemoryById(Entity.TranslationMemory.ForeignKey, TranslationProviderServer.GetDefaultTranslationMemoryRelationships(), includeLanguageResourceData: false, includeScheduledOperations: false);
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

		ITranslationMemory ITranslationMemoryLanguageDirection.TranslationProvider => TranslationProvider;

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

		ITranslationProvider ITranslationProviderLanguageDirection.TranslationProvider => TranslationProvider;

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

		public void UpdateCachedTranslationUnitCount()
		{
			//Entity.TuCount = TranslationProviderServer.Service.UpdateTranslationUnitCount(Entity.LanguageDirectionIdentity);
		}

		public int GetTranslationUnitCount()
		{
			return 0;// TranslationProviderServer.Service.GetTuCount(Entity.LanguageDirectionIdentity);
		}

		public bool ApplyFieldsToTranslationUnit(FieldValues values, bool overwrite, PersistentObjectToken translationUnitId)
		{
			return false;// TranslationProviderServer.Service.ApplyFieldsToTranslationUnit(Entity.LanguageDirectionIdentity, values, overwrite, translationUnitId);
		}

		public int ApplyFieldsToTranslationUnits(FieldValues values, bool overwrite, PersistentObjectToken[] translationUnitIds)
		{
			return 0;// TranslationProviderServer.Service.ApplyFieldsToTranslationUnits(Entity.LanguageDirectionIdentity, values, overwrite, translationUnitIds);
		}

		public bool DeleteTranslationUnit(PersistentObjectToken translationUnitId)
		{
			return false;// TranslationProviderServer.Service.DeleteTranslationUnit(Entity.LanguageDirectionIdentity, translationUnitId);
		}

		public int DeleteAllTranslationUnits()
		{
			return 0;// TranslationProviderServer.Service.DeleteAllTranslationUnits(Entity.LanguageDirectionIdentity);
		}

		public int DeleteTranslationUnits(PersistentObjectToken[] translationUnitIds)
		{
			return 0;// TranslationProviderServer.Service.DeleteTranslationUnits(Entity.LanguageDirectionIdentity, translationUnitIds);
		}

		public int DeleteTranslationUnitsWithIterator(ref RegularIterator iterator)
		{
			return 0;// TranslationProviderServer.Service.DeleteTranslationUnitsWithIterator(Entity.LanguageDirectionIdentity, ref iterator);
		}

		public int EditTranslationUnits(EditScript editScript, EditUpdateMode updateMode, PersistentObjectToken[] translationUnitIds)
		{
			return 0;// TranslationProviderServer.Service.EditTranslationUnits(Entity.LanguageDirectionIdentity, editScript, updateMode, translationUnitIds);
		}

		public int EditTranslationUnitsWithIterator(EditScript editScript, EditUpdateMode updateMode, ref RegularIterator iterator)
		{
			return 0;// TranslationProviderServer.Service.EditTranslationUnitsWithIterator(Entity.LanguageDirectionIdentity, editScript, updateMode, ref iterator);
		}

		public TranslationUnit[] PreviewEditTranslationUnitsWithIterator(EditScript editScript, ref RegularIterator iterator)
		{
			return null;// TranslationProviderServer.Service.PreviewEditTranslationUnitsWithIterator(Entity.LanguageDirectionIdentity, editScript, ref iterator);
		}

		public TranslationUnit[] GetDuplicateTranslationUnits(ref DuplicateIterator iterator)
		{
			return null;// TranslationProviderServer.Service.GetDuplicateTranslationUnits(Entity.LanguageDirectionIdentity, ref iterator);
        }

		public TranslationUnit GetTranslationUnit(PersistentObjectToken translationUnitId)
		{
			return null;// TranslationProviderServer.Service.GetTranslationUnit(Entity.LanguageDirectionIdentity, translationUnitId);
        }

		public TranslationUnit[] GetTranslationUnits(ref RegularIterator iterator)
		{
			return null;// TranslationProviderServer.Service.GetTranslationUnits(Entity.LanguageDirectionIdentity, ref iterator);
        }

		public TranslationUnit[] GetTranslationUnitsWithContextContent(ref RegularIterator iterator)
		{
			throw new NotImplementedException();
		}

		public ImportResult[] UpdateTranslationUnitsMasked(TranslationUnit[] translationUnits, bool[] mask)
		{
			return null;// TranslationProviderServer.Service.UpdateTranslationUnitsMasked(Entity.LanguageDirectionIdentity, translationUnits, mask);
        }

		public bool ReindexTranslationUnits(ref RegularIterator iterator)
		{
			return false;// TranslationProviderServer.Service.ReindexTranslationUnits(Entity.LanguageDirectionIdentity, ref iterator);
		}

		public SearchResults SearchSegment(SearchSettings settings, Segment segment)
		{
			return null;// TranslationProviderServer.Service.SearchSegment(Entity.LanguageDirectionIdentity, settings, segment);
        }

		public SearchResults[] SearchSegments(SearchSettings settings, Segment[] segments)
		{
			return null;// TranslationProviderServer.Service.SearchSegments(Entity.LanguageDirectionIdentity, settings, segments);
        }

		public SearchResults[] SearchSegmentsMasked(SearchSettings settings, Segment[] segments, bool[] mask)
		{
			return null;// TranslationProviderServer.Service.SearchSegmentsMasked(Entity.LanguageDirectionIdentity, settings, segments, mask);
        }

		public SearchResults SearchText(SearchSettings settings, string segment)
		{
			return null;// TranslationProviderServer.Service.SearchText(Entity.LanguageDirectionIdentity, settings, segment);
        }

		public SearchResults SearchTranslationUnit(SearchSettings settings, TranslationUnit translationUnit)
		{
			return null;// TranslationProviderServer.Service.SearchTranslationUnit(Entity.LanguageDirectionIdentity, settings, translationUnit);
        }

		public SearchResults[] SearchTranslationUnits(SearchSettings settings, TranslationUnit[] translationUnits)
		{
			return null;// TranslationProviderServer.Service.SearchTranslationUnits(Entity.LanguageDirectionIdentity, settings, translationUnits);
        }

		public SearchResults[] SearchTranslationUnitsMasked(SearchSettings settings, TranslationUnit[] translationUnits, bool[] mask)
		{
			return null;// TranslationProviderServer.Service.SearchTranslationUnitsMasked(Entity.LanguageDirectionIdentity, settings, translationUnits, mask);
        }

		public ImportResult AddTranslationUnit(TranslationUnit translationUnit, ImportSettings settings)
		{
			return null;// TranslationProviderServer.Service.AddTranslationUnit(Entity.LanguageDirectionIdentity, translationUnit, settings);
        }

		public ImportResult[] AddTranslationUnits(TranslationUnit[] translationUnits, ImportSettings settings)
		{
			return null;// TranslationProviderServer.Service.AddTranslationUnits(Entity.LanguageDirectionIdentity, translationUnits, settings);
        }

		public ImportResult[] AddOrUpdateTranslationUnits(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings)
		{
			return null;// TranslationProviderServer.Service.AddOrUpdateTranslationUnits(Entity.LanguageDirectionIdentity, translationUnits, previousTranslationHashes, settings);
        }

		public ImportResult[] AddTranslationUnitsMasked(TranslationUnit[] translationUnits, ImportSettings settings, bool[] mask)
		{
			return null;// TranslationProviderServer.Service.AddTranslationUnitsMasked(Entity.LanguageDirectionIdentity, translationUnits, settings, mask);
        }

		public ImportResult[] AddOrUpdateTranslationUnitsMasked(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings, bool[] mask)
		{
			return null;// TranslationProviderServer.Service.AddOrUpdateTranslationUnitsMasked(Entity.LanguageDirectionIdentity, translationUnits, previousTranslationHashes, settings, mask);
        }

		public ImportResult UpdateTranslationUnit(TranslationUnit translationUnit)
		{
			return null;// TranslationProviderServer.Service.UpdateTranslationUnit(Entity.LanguageDirectionIdentity, translationUnit);
        }

		public ImportResult[] UpdateTranslationUnits(TranslationUnit[] translationUnits)
		{
			return null;// TranslationProviderServer.Service.UpdateTranslationUnits(Entity.LanguageDirectionIdentity, translationUnits);
        }

		public SubsegmentSearchResultsCollection SubsegmentSearchSegment(SubsegmentSearchSettings settings, Segment segment)
		{
			return null;// TranslationProviderServer.Service.SubsegmentSearchSegment(Entity.LanguageDirectionIdentity, settings, segment);
        }

		public SubsegmentSearchResultsCollection[] SubsegmentSearchSegments(SubsegmentSearchSettings settings, Segment[] segments)
		{
			return null;// TranslationProviderServer.Service.SubsegmentSearchSegments(Entity.LanguageDirectionIdentity, settings, segments);
        }

		public List<SubsegmentMatchType> SupportedSubsegmentMatchTypes()
		{
			return null;// TranslationProviderServer.Service.SupportedSubsegmentMatchTypes(Entity.LanguageDirectionIdentity);
        }

		public SegmentAndSubsegmentSearchResults SearchSegment(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, Segment segment)
		{
			SegmentAndSubsegmentSearchResults segmentAndSubsegmentSearchResults = null;// TranslationProviderServer.Service.SearchSegment(Entity.LanguageDirectionIdentity, settings, subsegmentSettings, condition, segment);
            if (segmentAndSubsegmentSearchResults == null)
			{
				SearchResults results = SearchSegment(settings, segment);
				segmentAndSubsegmentSearchResults = new SegmentAndSubsegmentSearchResults(results, null);
			}
			return segmentAndSubsegmentSearchResults;
		}

		public SegmentAndSubsegmentSearchResults[] SearchSegments(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, Segment[] segments)
		{
			SegmentAndSubsegmentSearchResults[] array = null;// TranslationProviderServer.Service.SearchSegments(Entity.LanguageDirectionIdentity, settings, subsegmentSettings, condition, segments);
            if (array == null)
			{
				SearchResults[] results = SearchSegments(settings, segments);
				array = TranslationMemorySearchResultConverters.ToSegmentAndSubsegmentSearchResults(results);
			}
			return array;
		}

		public SegmentAndSubsegmentSearchResults[] SearchSegmentsMasked(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, Segment[] segments, bool[] mask)
		{
			SegmentAndSubsegmentSearchResults[] array = null;// TranslationProviderServer.Service.SearchSegmentsMasked(Entity.LanguageDirectionIdentity, settings, subsegmentSettings, condition, segments, mask);
            if (array == null)
			{
				SearchResults[] results = SearchSegmentsMasked(settings, segments, mask);
				array = TranslationMemorySearchResultConverters.ToSegmentAndSubsegmentSearchResults(results);
			}
			return array;
		}

		public SegmentAndSubsegmentSearchResults SearchTranslationUnit(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, TranslationUnit translationUnit)
		{
			SegmentAndSubsegmentSearchResults segmentAndSubsegmentSearchResults = null;// TranslationProviderServer.Service.SearchTranslationUnit(Entity.LanguageDirectionIdentity, settings, subsegmentSettings, condition, translationUnit);
            if (segmentAndSubsegmentSearchResults == null)
			{
				segmentAndSubsegmentSearchResults = new SegmentAndSubsegmentSearchResults(SearchTranslationUnit(settings, translationUnit), null);
			}
			return segmentAndSubsegmentSearchResults;
		}

		public SegmentAndSubsegmentSearchResults[] SearchTranslationUnits(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, TranslationUnit[] translationUnits)
		{
			SegmentAndSubsegmentSearchResults[] array = null;// TranslationProviderServer.Service.SearchTranslationUnits(Entity.LanguageDirectionIdentity, settings, subsegmentSettings, condition, translationUnits);
            if (array == null)
			{
				SearchResults[] results = SearchTranslationUnits(settings, translationUnits);
				array = TranslationMemorySearchResultConverters.ToSegmentAndSubsegmentSearchResults(results);
			}
			return array;
		}

		public SegmentAndSubsegmentSearchResults[] SearchTranslationUnitsMasked(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, TranslationUnit[] translationUnits, bool[] mask)
		{
			SegmentAndSubsegmentSearchResults[] array = null;// TranslationProviderServer.Service.SearchTranslationUnitsMasked(Entity.LanguageDirectionIdentity, settings, subsegmentSettings, condition, translationUnits, mask);
            if (array == null)
			{
				SearchResults[] results = SearchTranslationUnitsMasked(settings, translationUnits, mask);
				array = TranslationMemorySearchResultConverters.ToSegmentAndSubsegmentSearchResults(results);
			}
			return array;
		}

		public ImportEntity Import(ImportSettings settings, string fileName)
		{
			ImportEntity importInfo = new ImportEntity
			{
				LanguageDirection = new EntityReference<LanguageDirectionEntity>(Entity),
				ImportSettings = settings
			};
			return null;// TranslationProviderServer.Service.QueueTranslationMemoryImport(importInfo, fileName, recomputeFuzzyIndex: true);
        }
	}
}
