using Sdl.Core.Api.DataAccess;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemory.EditScripts;
using System;
using System.Globalization;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public class CloudBasedTranslationMemoryLanguageDirection : ITranslationMemoryLanguageDirection, ITranslationProviderLanguageDirection
	{
		private CloudTranslationProvider _server;

		private LanguageDirectionEntity _entity;

		private CloudBasedTranslationMemory _lazyTranslationMemory;

		internal LanguageDirectionEntity Entity
		{
			get
			{
				return _entity;
			}
			set
			{
				_entity = value;
			}
		}

		internal CloudTranslationProvider CloudTranslationProvider => _server;

		public CloudBasedTranslationMemory TranslationProvider
		{
			get
			{
				if (_lazyTranslationMemory == null)
				{
					TranslationMemoryEntity translationMemoryById = null;// _server.Service.GetTranslationMemoryById(_entity.TranslationMemory.ForeignKey, TranslationProviderServer.GetDefaultTranslationMemoryRelationships(), includeLanguageResourceData: false, includeScheduledOperations: false);
                    _lazyTranslationMemory = new CloudBasedTranslationMemory(_server, translationMemoryById);
				}
				return _lazyTranslationMemory;
			}
			internal set
			{
				_lazyTranslationMemory = value;
				if (_lazyTranslationMemory != null)
				{
					_server = _lazyTranslationMemory.Provider;
					_entity.TranslationMemory = new EntityReference<TranslationMemoryEntity>(_lazyTranslationMemory.Entity);
				}
				else
				{
					_server = null;
				}
			}
		}

		ITranslationMemory ITranslationMemoryLanguageDirection.TranslationProvider => TranslationProvider;

		public int CachedTranslationUnitCount
		{
			get
			{
				if (_entity.TuCount.HasValue && _entity.TuCount.HasValue)
				{
					return _entity.TuCount.Value;
				}
				return 0;
			}
		}

		ITranslationProvider ITranslationProviderLanguageDirection.TranslationProvider => TranslationProvider;

		public CultureInfo SourceLanguage
		{
			get
			{
				if (!string.IsNullOrEmpty(_entity.SourceCultureName))
				{
					return CultureInfoExtensions.GetCultureInfo(_entity.SourceCultureName);
				}
				return null;
			}
			set
			{
				if (!IsNewObject)
				{
					throw new InvalidOperationException("You cannot change the source language after a language direction has been created.");
				}
				_entity.SourceCultureName = value?.Name;
			}
		}

		public CultureInfo TargetLanguage
		{
			get
			{
				if (!string.IsNullOrEmpty(_entity.TargetCultureName))
				{
					return CultureInfoExtensions.GetCultureInfo(_entity.TargetCultureName);
				}
				return null;
			}
			set
			{
				if (!IsNewObject)
				{
					throw new InvalidOperationException("You cannot change the target language after a language direction has been created.");
				}
				_entity.TargetCultureName = value?.Name;
			}
		}

		public string SourceLanguageCode
		{
			get
			{
				return _entity.SourceCultureName;
			}
			set
			{
				if (!IsNewObject)
				{
					throw new InvalidOperationException("You cannot change the source language after a language direction has been created.");
				}
				_entity.SourceCultureName = value;
			}
		}

		public string TargetLanguageCode
		{
			get
			{
				return _entity.TargetCultureName;
			}
			set
			{
				if (!IsNewObject)
				{
					throw new InvalidOperationException("You cannot change the target language after a language direction has been created.");
				}
				_entity.TargetCultureName = value;
			}
		}

		public bool CanReverseLanguageDirection => false;

		private bool IsNewObject
		{
			get
			{
				if (_entity.Id != null)
				{
					return _entity.Id.Value == null;
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

		public CloudBasedTranslationMemoryLanguageDirection()
		{
			_entity = new LanguageDirectionEntity();
			_entity.UniqueId = Guid.NewGuid();
		}

		internal CloudBasedTranslationMemoryLanguageDirection(CloudTranslationProvider server, LanguageDirectionEntity entity)
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
			_server = server;
			_entity = entity;
		}

		internal CloudBasedTranslationMemoryLanguageDirection(CloudBasedTranslationMemory translationMemory, LanguageDirectionEntity entity)
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
			_lazyTranslationMemory = translationMemory;
			_entity = entity;
		}

		public void UpdateCachedTranslationUnitCount()
		{
			_entity.TuCount = null;// _server.Service.UpdateTranslationUnitCount(_entity.LanguageDirectionIdentity);
        }

		public int GetTranslationUnitCount()
		{
			return 0;// _server.Service.GetTuCount(_entity.LanguageDirectionIdentity);
		}

		public bool ApplyFieldsToTranslationUnit(FieldValues values, bool overwrite, PersistentObjectToken translationUnitId)
		{
			return false;// CloudTranslationProvider.Service.ApplyFieldsToTranslationUnit(Entity.LanguageDirectionIdentity, values, overwrite, translationUnitId);
		}

		public int ApplyFieldsToTranslationUnits(FieldValues values, bool overwrite, PersistentObjectToken[] translationUnitIds)
		{
			return 0;// CloudTranslationProvider.Service.ApplyFieldsToTranslationUnits(Entity.LanguageDirectionIdentity, values, overwrite, translationUnitIds);
		}

		public bool DeleteTranslationUnit(PersistentObjectToken translationUnitId)
		{
			return false;// CloudTranslationProvider.Service.DeleteTranslationUnit(Entity.LanguageDirectionIdentity, translationUnitId);
		}

		public int DeleteAllTranslationUnits()
		{
			return 0;// CloudTranslationProvider.Service.DeleteAllTranslationUnits(Entity.LanguageDirectionIdentity);
		}

		public int DeleteTranslationUnits(PersistentObjectToken[] translationUnitIds)
		{
			return 0;// CloudTranslationProvider.Service.DeleteTranslationUnits(Entity.LanguageDirectionIdentity, translationUnitIds);
		}

		public int DeleteTranslationUnitsWithIterator(ref RegularIterator iterator)
		{
			return 0;// CloudTranslationProvider.Service.DeleteTranslationUnitsWithIterator(Entity.LanguageDirectionIdentity, ref iterator);
		}

		public int EditTranslationUnits(EditScript editScript, EditUpdateMode updateMode, PersistentObjectToken[] translationUnitIds)
		{
			return 0;// CloudTranslationProvider.Service.EditTranslationUnits(Entity.LanguageDirectionIdentity, editScript, updateMode, translationUnitIds);
		}

		public int EditTranslationUnitsWithIterator(EditScript editScript, EditUpdateMode updateMode, ref RegularIterator iterator)
		{
			return 0;// CloudTranslationProvider.Service.EditTranslationUnitsWithIterator(Entity.LanguageDirectionIdentity, editScript, updateMode, ref iterator);
		}

		public TranslationUnit[] PreviewEditTranslationUnitsWithIterator(EditScript editScript, ref RegularIterator iterator)
		{
			return null;// CloudTranslationProvider.Service.PreviewEditTranslationUnitsWithIterator(Entity.LanguageDirectionIdentity, editScript, ref iterator);
        }

		public TranslationUnit[] GetDuplicateTranslationUnits(ref DuplicateIterator iterator)
		{
			return null;// CloudTranslationProvider.Service.GetDuplicateTranslationUnits(Entity.LanguageDirectionIdentity, ref iterator);
        }

		public TranslationUnit GetTranslationUnit(PersistentObjectToken translationUnitId)
		{
			return null;// CloudTranslationProvider.Service.GetTranslationUnit(Entity.LanguageDirectionIdentity, translationUnitId);
        }

		public TranslationUnit[] GetTranslationUnits(ref RegularIterator iterator)
		{
			return null;// CloudTranslationProvider.Service.GetTranslationUnits(Entity.LanguageDirectionIdentity, ref iterator);
        }

		public ImportResult[] UpdateTranslationUnitsMasked(TranslationUnit[] translationUnits, bool[] mask)
		{
			return null;// CloudTranslationProvider.Service.UpdateTranslationUnitsMasked(Entity.LanguageDirectionIdentity, translationUnits, mask);
        }

		public bool ReindexTranslationUnits(ref RegularIterator iterator)
		{
			return false;// CloudTranslationProvider.Service.ReindexTranslationUnits(Entity.LanguageDirectionIdentity, ref iterator);
		}

		public SearchResults SearchSegment(SearchSettings settings, Segment segment)
		{
			return null;// CloudTranslationProvider.Service.SearchSegment(Entity.LanguageDirectionIdentity, settings, segment);
        }

		public SearchResults[] SearchSegments(SearchSettings settings, Segment[] segments)
		{
			return null;// CloudTranslationProvider.Service.SearchSegments(Entity.LanguageDirectionIdentity, settings, segments);
        }

		public SearchResults[] SearchSegmentsMasked(SearchSettings settings, Segment[] segments, bool[] mask)
		{
			return null;// CloudTranslationProvider.Service.SearchSegmentsMasked(Entity.LanguageDirectionIdentity, settings, segments, mask);
        }

		public SearchResults SearchText(SearchSettings settings, string segment)
		{
			return null;// CloudTranslationProvider.Service.SearchText(Entity.LanguageDirectionIdentity, settings, segment);
        }

		public SearchResults SearchTranslationUnit(SearchSettings settings, TranslationUnit translationUnit)
		{
			return null;// CloudTranslationProvider.Service.SearchTranslationUnit(Entity.LanguageDirectionIdentity, settings, translationUnit);
        }

		public SearchResults[] SearchTranslationUnits(SearchSettings settings, TranslationUnit[] translationUnits)
		{
			return null;// CloudTranslationProvider.Service.SearchTranslationUnits(Entity.LanguageDirectionIdentity, settings, translationUnits);
        }

		public SearchResults[] SearchTranslationUnitsMasked(SearchSettings settings, TranslationUnit[] translationUnits, bool[] mask)
		{
			return null;// CloudTranslationProvider.Service.SearchTranslationUnitsMasked(Entity.LanguageDirectionIdentity, settings, translationUnits, mask);
        }

		public ImportResult AddTranslationUnit(TranslationUnit translationUnit, ImportSettings settings)
		{
			return null;// CloudTranslationProvider.Service.AddTranslationUnit(Entity.LanguageDirectionIdentity, translationUnit, settings);
        }

		public ImportResult[] AddTranslationUnits(TranslationUnit[] translationUnits, ImportSettings settings)
		{
			return null;// CloudTranslationProvider.Service.AddTranslationUnits(Entity.LanguageDirectionIdentity, translationUnits, settings);
        }

		public ImportResult[] AddOrUpdateTranslationUnits(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings)
		{
			return null;// CloudTranslationProvider.Service.AddOrUpdateTranslationUnits(Entity.LanguageDirectionIdentity, translationUnits, previousTranslationHashes, settings);
        }

		public ImportResult[] AddTranslationUnitsMasked(TranslationUnit[] translationUnits, ImportSettings settings, bool[] mask)
		{
			return null;// CloudTranslationProvider.Service.AddTranslationUnitsMasked(Entity.LanguageDirectionIdentity, translationUnits, settings, mask);
        }

		public ImportResult[] AddOrUpdateTranslationUnitsMasked(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings, bool[] mask)
		{
			return null;// CloudTranslationProvider.Service.AddOrUpdateTranslationUnitsMasked(Entity.LanguageDirectionIdentity, translationUnits, previousTranslationHashes, settings, mask);
        }

		public ImportResult UpdateTranslationUnit(TranslationUnit translationUnit)
		{
			return null;// CloudTranslationProvider.Service.UpdateTranslationUnit(Entity.LanguageDirectionIdentity, translationUnit);
        }

		public ImportResult[] UpdateTranslationUnits(TranslationUnit[] translationUnits)
		{
			return null;// CloudTranslationProvider.Service.UpdateTranslationUnits(Entity.LanguageDirectionIdentity, translationUnits);
        }
	}
}
