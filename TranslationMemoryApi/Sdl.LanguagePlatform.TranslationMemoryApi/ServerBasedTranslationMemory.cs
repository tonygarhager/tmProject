using Sdl.Core.Api.DataAccess;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Segmentation;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Client;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Represents a <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationMemory" /> which is hosted on a server, as opposed 
	/// to a <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.FileBasedTranslationMemory" />.
	/// </summary>
	public class ServerBasedTranslationMemory : RemoteTranslationMemory, ITranslationMemory2015, ITranslationMemory, ITranslationProvider, INotifyPropertyChanged, IEditableObject, IEquatable<ServerBasedTranslationMemory>, IPermissionCheck, IAdvancedContextTranslationMemory
	{
		private const int DefaultAccuracy = 20;

		private TranslationMemoryEntity _backupEntity;

		internal ServerBasedTranslationMemoryLanguageDirectionCollection _lazyLanguageDirections;

		private ServerBasedFieldsTemplate _lazyFieldsTemplate;

		private ServerBasedLanguageResourcesTemplate _lazyLanguageResourcesTemplate;

		internal TranslationMemoryContainer _lazyContainer;

		private FuzzyIndexTuningSettings _lazyFuzzyIndexTuningSettings;

		private ScheduledRecomputeStatisticsOperation _lazyCurrentRecomputeStatisticsOperation;

		private ScheduledReindexOperation _lazyCurrentReindexOperation;

		/// <summary>
		/// Gets the server.
		/// </summary>
		public TranslationProviderServer TranslationProviderServer
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the creation user of this translation memory.
		/// </summary>
		/// <value></value>
		/// <exception cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ObjectDeletedException">Thrown when this object has been deleted.</exception>
		public string CreationUserName
		{
			get
			{
				VerifyNotDeleted();
				return base.Entity.CreationUser;
			}
		}

		/// <summary>
		/// Gets or sets the expiration date for this translation memory.
		/// </summary>
		/// <value></value>
		/// <remarks>You have to call <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedTranslationMemory.Save" /> in order to perist changes to this property.</remarks>
		/// <remarks>TODO what if this is set to a time in the past?</remarks>
		/// <exception cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ObjectDeletedException">Thrown when this object has been deleted.</exception>
		public DateTime? ExpirationDate
		{
			get
			{
				VerifyNotDeleted();
				if (!base.Entity.ExpirationDate.HasValue)
				{
					return null;
				}
				return base.Entity.ExpirationDate.Value;
			}
			set
			{
				VerifyNotDeleted();
				if (!value.HasValue)
				{
					base.Entity.ExpirationDate = null;
				}
				else
				{
					base.Entity.ExpirationDate = value.Value;
				}
				OnPropertyChanged("ExpirationDate");
			}
		}

		/// <summary>
		/// Gets this list of language directions for this translation memory.
		/// </summary>
		/// <exception cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ObjectDeletedException">Thrown when this object has been deleted.</exception>
		public ServerBasedTranslationMemoryLanguageDirectionCollection LanguageDirections
		{
			get
			{
				VerifyNotDeleted();
				if (_lazyLanguageDirections == null)
				{
					LanguageDirectionEntity[] languageDirectionsByTranslationMemoryId = TranslationProviderServer.Service.GetLanguageDirectionsByTranslationMemoryId(base.Entity.Id, new string[0]);
					base.Entity.LanguageDirections = new EntityCollection<LanguageDirectionEntity>(languageDirectionsByTranslationMemoryId);
					_lazyLanguageDirections = new ServerBasedTranslationMemoryLanguageDirectionCollection(this, base.Entity.LanguageDirections);
				}
				return _lazyLanguageDirections;
			}
		}

		/// <summary>
		/// Gets the list of language directions which are supported by this translation memory.
		/// </summary>
		/// <value></value>
		/// <exception cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ObjectDeletedException">Thrown when this object has been deleted.</exception>
		public ReadOnlyCollection<LanguagePair> SupportedLanguageDirections
		{
			get
			{
				VerifyNotDeleted();
				List<LanguagePair> list = new List<LanguagePair>(LanguageDirections.Select((ServerBasedTranslationMemoryLanguageDirection ld) => new LanguagePair(ld.SourceLanguageCode, ld.TargetLanguageCode)));
				return new ReadOnlyCollection<LanguagePair>(list);
			}
		}

		/// <summary>
		/// Gets or sets the field group template. Can be null.
		/// </summary>
		/// <remarks>
		/// <para>
		/// You need to call <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationMemory.Save" /> to persist the change after setting this property.
		/// </para>
		/// <para>The template is potentially shared by a number of translation memories. 
		/// Changes made to the template will affect all translation memories which refer to this
		/// template.
		/// </para>
		/// <para>
		/// If you want to make field changes for this translation memory only, make those modifications through the <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationMemory.FieldDefinitions" /> property.
		/// Before you can do that, set the <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedTranslationMemory.FieldsTemplate" /> property to null.
		/// </para>
		/// </remarks>
		/// <exception cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ObjectDeletedException">Thrown when this object has been deleted.</exception>
		public ServerBasedFieldsTemplate FieldsTemplate
		{
			get
			{
				VerifyNotDeleted();
				VerifyFieldTemplateLoaded();
				if (_lazyFieldsTemplate.Entity == null)
				{
					return null;
				}
				if (!_lazyFieldsTemplate.Entity.IsTmSpecific.Value)
				{
					return _lazyFieldsTemplate;
				}
				return null;
			}
			set
			{
				VerifyNotDeleted();
				VerifyFieldTemplateLoaded();
				if (value == null && !_lazyFieldsTemplate.Entity.IsTmSpecific.Value)
				{
					InitializeTmSpecificFields();
				}
				else if (value != null)
				{
					_lazyFieldsTemplate = value;
					base.Entity.FieldGroupTemplate = new EntityReference<FieldGroupTemplateEntity>(_lazyFieldsTemplate.Entity);
				}
				OnPropertyChanged("FieldsTemplate");
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedLanguageResourcesTemplate" /> object associated with this TM.
		/// </summary>
		/// <remarks>The template is a shared entity that defines the language resources that are 
		/// associated with one or more TMs. Modifying the template will therefore cause modifications to 
		/// the shared data and alter the language resources for all associated TMs.
		/// <para>
		/// If you want to make language resource changes for this translation memory only, make those modifications through the <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationMemory.LanguageResourceBundles" /> property.
		/// Before you can do that, set the <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedTranslationMemory.LanguageResourcesTemplate" /> property to null.
		/// </para>/// </remarks>
		/// <exception cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ObjectDeletedException">Thrown when this object has been deleted.</exception>
		public ServerBasedLanguageResourcesTemplate LanguageResourcesTemplate
		{
			get
			{
				VerifyNotDeleted();
				VerifyLanguageResourceTemplateLoaded();
				if (_lazyLanguageResourcesTemplate.Entity == null)
				{
					return null;
				}
				if (!_lazyLanguageResourcesTemplate.Entity.IsTmSpecific.Value)
				{
					return _lazyLanguageResourcesTemplate;
				}
				return null;
			}
			set
			{
				VerifyNotDeleted();
				VerifyLanguageResourceTemplateLoaded();
				if (value == null && !_lazyLanguageResourcesTemplate.Entity.IsTmSpecific.Value)
				{
					InitializeTmSpecificLanguageResources();
				}
				else if (value != null)
				{
					_lazyLanguageResourcesTemplate = value;
					base.Entity.LanguageResourceTemplate = new EntityReference<LanguageResourceTemplateEntity>(_lazyLanguageResourcesTemplate.Entity);
				}
				OnPropertyChanged("LanguageResourcesTemplate");
			}
		}

		/// <summary>
		/// Gets or sets an indication whether this translation memory is a project translation memory.
		/// </summary>
		/// <exception cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ObjectDeletedException">Thrown when this object has been deleted.</exception>
		public bool IsProjectTranslationMemory
		{
			get
			{
				return base.Entity.IsProjectTranslationMemory.Value;
			}
			set
			{
				base.Entity.IsProjectTranslationMemory = value;
			}
		}

		/// <summary>
		/// Gets the custom fields defined for this TM.
		/// </summary>
		/// <value></value>
		/// <remarks>In case this is a server-based translation memory, which is associated with a fields template (see <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.IFieldsTemplate" />),
		/// this returns a read-only fields collection identical to the template's fields collection. In all other cases,
		/// the field collection returned can be modified. Note that you have to call <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedTranslationMemory.Save" /> to persist any changes made to
		/// the fields.</remarks>
		/// <exception cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ObjectDeletedException">Thrown when this object has been deleted.</exception>
		public FieldDefinitionCollection FieldDefinitions
		{
			get
			{
				VerifyNotDeleted();
				VerifyFieldTemplateLoaded();
				return _lazyFieldsTemplate.FieldDefinitions;
			}
		}

		/// <summary>
		/// Gets the language resources which are associated with this TM.
		/// </summary>
		/// <value></value>
		/// <remarks>In case this is a server-based translation memory, which is associated with a language resources template (see <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ILanguageResourcesTemplate" />),
		/// this returns a read-only language resources collection identical to the template's language resources collection. In all other cases,
		/// the language resources collection returned can be modified. Note that you have to call <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedTranslationMemory.Save" /> to persist any changes made to
		/// language resources.</remarks>
		/// <exception cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ObjectDeletedException">Thrown when this object has been deleted.</exception>
		public LanguageResourceBundleCollection LanguageResourceBundles
		{
			get
			{
				VerifyNotDeleted();
				VerifyLanguageResourceTemplateLoaded();
				return _lazyLanguageResourcesTemplate.LanguageResourceBundles;
			}
		}

		/// <summary>
		/// Gets the cached total translation unit count for all language directions.
		/// This count is computed at regular intervals and when performing imports.
		/// To calculate the actual translation unit count, use <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedTranslationMemory.GetTranslationUnitCount" />
		/// but bear in mind that this is a relatively expensive operation.
		/// </summary>
		public int CachedTranslationUnitCount => LanguageDirections.Sum((ServerBasedTranslationMemoryLanguageDirection ld) => ld.CachedTranslationUnitCount);

		/// <summary>
		/// Gets the status info for the provider.
		/// </summary>
		/// <value></value>
		public ProviderStatusInfo StatusInfo => new ProviderStatusInfo(available: true, "OK");

		/// <summary>
		/// Gets a URI which uniquely identifies this translation provider.
		/// </summary>
		/// <value></value>
		/// <exception cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ObjectDeletedException">Thrown when this object has been deleted.</exception>
		public Uri Uri
		{
			get
			{
				VerifyNotDeleted();
				string text = TranslationProviderServer.Uri.ToString();
				if (text.EndsWith("/", StringComparison.OrdinalIgnoreCase))
				{
					text = text.Substring(0, text.Length - 1);
				}
				return new Uri($"sdltm.{text}?orgPath={Uri.EscapeDataString(ParentResourceGroupPath)}&tmName={Uri.EscapeDataString(base.Name)}");
			}
		}

		/// <summary>
		/// <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsTaggedInput" />. Always true for TMs.
		/// </summary>
		public bool SupportsTaggedInput => true;

		/// <summary>
		/// <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsScoring" />. Always true for TMs.
		/// </summary>
		public bool SupportsScoring => true;

		/// <summary>
		/// <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsSearchForTranslationUnits" />. Always true for TMs.
		/// </summary>
		public bool SupportsSearchForTranslationUnits => true;

		/// <summary>
		/// States whether the TM supports 'contains' TU searches (better performance than 'matches' searches) 
		/// </summary>
		public bool SupportsSearchForTranslationUnitUsingContainsOperator => TranslationProviderServer.Service.SupportsSearchForTranslationUnitUsingContainsOperator;

		/// <summary>
		/// <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsMultipleResults" />. Always true for TMs.
		/// </summary>
		public bool SupportsMultipleResults => true;

		/// <summary>
		/// <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsFilters" />. Always true for TMs.
		/// </summary>
		public bool SupportsFilters => true;

		/// <summary>
		/// <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsPenalties" />. Always true for TMs.
		/// </summary>
		public bool SupportsPenalties => true;

		/// <summary>
		/// <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsStructureContext" />. Always true for TMs.
		/// </summary>
		public bool SupportsStructureContext => true;

		/// <summary>
		/// <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsDocumentSearches" />. Always true for TMs.
		/// </summary>
		public bool SupportsDocumentSearches => true;

		/// <summary>
		/// <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsUpdate" />. Always true for TMs.
		/// </summary>
		public bool SupportsUpdate => true;

		/// <summary>
		/// <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsPlaceables" />. Always true for TMs.
		/// </summary>
		public bool SupportsPlaceables => true;

		/// <summary>
		/// <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsTranslation" />. Always true for TMs.
		/// </summary>
		public bool SupportsTranslation => true;

		/// <summary>
		/// <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsFuzzySearch" />. Always true for TMs.
		/// </summary>
		public bool SupportsFuzzySearch => true;

		/// <summary>
		/// <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsConcordanceSearch" />. Always true for TMs.
		/// </summary>
		public bool SupportsConcordanceSearch => true;

		/// <summary>
		/// <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsSourceConcordanceSearch" />. Always true for TMs.
		/// </summary>
		public bool SupportsSourceConcordanceSearch => true;

		/// <summary>
		/// <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsTargetConcordanceSearch" />. Returns <code>true</code>
		/// if this translation memory has a word-based fuzzy index for the target language (see <see cref="T:Sdl.LanguagePlatform.TranslationMemory.FuzzyIndexes" />).
		/// </summary>
		public bool SupportsTargetConcordanceSearch => true;

		/// <summary>
		/// <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsWordCounts" />. Always true for TMs.
		/// </summary>
		public bool SupportsWordCounts => true;

		/// <summary>
		/// <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.TranslationMethod" />. 
		/// Always returns <see cref="F:Sdl.LanguagePlatform.TranslationMemoryApi.TranslationMethod.TranslationMemory" />
		/// </summary>
		public TranslationMethod TranslationMethod => TranslationMethod.TranslationMemory;

		/// <summary>
		/// <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.IsReadOnly" />. 
		/// </summary>
		public bool IsReadOnly => false;

		/// <summary>
		/// Returns <code>true</code> if this translation memory has unsaved changes.
		/// </summary>
		public bool IsDirty
		{
			get
			{
				if (base.Entity.IsDirty)
				{
					return true;
				}
				if (FieldsTemplate.Entity.IsTmSpecific.Value && FieldsTemplate.Entity.IsDirty)
				{
					return true;
				}
				if (LanguageResourcesTemplate.Entity.IsTmSpecific.Value && LanguageResourcesTemplate.Entity.IsDirty)
				{
					return true;
				}
				return false;
			}
		}

		/// <summary>
		/// Gets or sets the translation memory container that contains this translation memory.
		/// </summary>
		/// <remarks>You cannot change this property after the translation memory has been created.</remarks>
		/// <exception cref="T:System.InvalidOperationException">Thrown when trying to set this property after the translation memory has been created.</exception>
		/// <exception cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ObjectDeletedException">Thrown when this object has been deleted.</exception>
		public TranslationMemoryContainer Container
		{
			get
			{
				VerifyNotDeleted();
				if (_lazyContainer == null && base.Entity.Container.ForeignKey != null && base.Entity.Container.ForeignKey.Value != null)
				{
					ContainerEntity containerById = TranslationProviderServer.Service.GetContainerById(base.Entity.Container.ForeignKey, new string[0]);
					_lazyContainer = new TranslationMemoryContainer(TranslationProviderServer, containerById);
				}
				return _lazyContainer;
			}
			set
			{
				VerifyNotDeleted();
				VerifyNewObject("You cannot change the container once the translation memory has been created.");
				_lazyContainer = value;
				base.Entity.Container = new EntityReference<ContainerEntity>(value.Entity.Id);
				OnPropertyChanged("Container");
			}
		}

		/// <summary>
		/// Gets or sets the parent resource group path.
		/// </summary>
		/// <value>The parent resource group path.</value>
		[Required(ErrorMessage = "Required Field")]
		public string ParentResourceGroupPath
		{
			get
			{
				return base.Entity.ParentResourceGroupPath;
			}
			set
			{
				base.Entity.ParentResourceGroupPath = value;
			}
		}

		/// <summary>
		/// Gets the parent resource group name.
		/// </summary>
		public string ParentResourceGroupName
		{
			get
			{
				return base.Entity.ParentResourceGroupName;
			}
			set
			{
				base.Entity.ParentResourceGroupName = value;
			}
		}

		/// <summary>
		/// Gets the parent resource group description.
		/// </summary>
		public string ParentResourceGroupDescription
		{
			get
			{
				return base.Entity.ParentResourceGroupDescription;
			}
			set
			{
				base.Entity.ParentResourceGroupDescription = value;
			}
		}

		/// <summary>
		/// Gets the collection of paths for the linked resource groups.
		/// </summary>
		public string[] LinkedResourceGroupPaths
		{
			get
			{
				return base.Entity.LinkedResourceGroupPaths;
			}
			set
			{
				base.Entity.LinkedResourceGroupPaths = value;
			}
		}

		/// <summary>
		/// Gets the most recent scheduled recomputation of fuzzy index statistics; if any.
		/// </summary>
		public ScheduledRecomputeStatisticsOperation CurrentRecomputeStatisticsOperation
		{
			get
			{
				if (base.Entity.CurrentRecomputeStatisticsWorkItemUniqueId.HasValue && _lazyCurrentRecomputeStatisticsOperation == null)
				{
					ScheduledOperationEntity scheduledOperation = TranslationProviderServer.Service.GetScheduledOperation(base.Entity.CurrentRecomputeStatisticsWorkItemUniqueId.Value);
					if (scheduledOperation != null)
					{
						_lazyCurrentRecomputeStatisticsOperation = new ScheduledRecomputeStatisticsOperation(scheduledOperation);
						_lazyCurrentRecomputeStatisticsOperation.TranslationMemory = this;
					}
					else
					{
						base.Entity.CurrentRecomputeStatisticsWorkItemUniqueId = null;
					}
				}
				return _lazyCurrentRecomputeStatisticsOperation;
			}
		}

		/// <summary>
		/// Gets the most recent scheduled operation.
		/// </summary>
		public ScheduledReindexOperation CurrentReindexOperation
		{
			get
			{
				if (base.Entity.CurrentReindexWorkItemUniqueId.HasValue && _lazyCurrentReindexOperation == null)
				{
					ScheduledOperationEntity scheduledOperation = TranslationProviderServer.Service.GetScheduledOperation(base.Entity.CurrentReindexWorkItemUniqueId.Value);
					if (scheduledOperation != null)
					{
						_lazyCurrentReindexOperation = new ScheduledReindexOperation(scheduledOperation);
						_lazyCurrentReindexOperation.TranslationMemory = this;
					}
					else
					{
						base.Entity.CurrentReindexWorkItemUniqueId = null;
					}
				}
				return _lazyCurrentReindexOperation;
			}
		}

		/// <summary>
		/// Gets or sets the fuzzy index tuning settings for the TM. Note that you have to call <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedTranslationMemory.Save" />
		/// to persists changes to this property.
		/// </summary>
		public FuzzyIndexTuningSettings FuzzyIndexTuningSettings
		{
			get
			{
				if (_lazyFuzzyIndexTuningSettings == null)
				{
					_lazyFuzzyIndexTuningSettings = new FuzzyIndexTuningSettings
					{
						MinScoreIncrease = base.Entity.MinScoreIncrease.Value,
						MinSearchVectorLengthSourceCharIndex = base.Entity.MinSearchVectorLengthSourceCharIndex.Value,
						MinSearchVectorLengthTargetCharIndex = base.Entity.MinSearchVectorLengthTargetCharIndex.Value,
						MinSearchVectorLengthSourceWordIndex = base.Entity.MinSearchVectorLengthSourceWordIndex.Value,
						MinSearchVectorLengthTargetWordIndex = base.Entity.MinSearchVectorLengthTargetWordIndex.Value
					};
				}
				return _lazyFuzzyIndexTuningSettings;
			}
			set
			{
				_lazyFuzzyIndexTuningSettings = value;
			}
		}

		/// <summary>
		/// Gets the time of the last fuzzy index statistics recomputation of this TM, if available.
		/// </summary>
		/// <value></value>
		public DateTime? FuzzyIndexStatisticsRecomputedAt
		{
			get
			{
				if (base.Entity.LastRecomputeDate.HasValue)
				{
					return base.Entity.LastRecomputeDate.Value;
				}
				return null;
			}
		}

		/// <summary>
		/// Gets the size of the TM at the point of the last fuzzy index statistics recomputation,
		/// if available.
		/// </summary>
		/// <value></value>
		public int? FuzzyIndexStatisticsSize => base.Entity.LastRecomputeSize;

		/// <summary>
		/// Gets a list of export tasks currently associated with the language directions encapsulated by this translation memory.
		/// </summary>
		public ReadOnlyCollection<ScheduledServerTranslationMemoryExport> CurrentExportTasks
		{
			get
			{
				List<ScheduledServerTranslationMemoryExport> list = new List<ScheduledServerTranslationMemoryExport>();
				if (base.Entity.LanguageDirections != null)
				{
					foreach (LanguageDirectionEntity languageDirection in base.Entity.LanguageDirections)
					{
						if (languageDirection.Exports != null)
						{
							foreach (ExportEntity export in languageDirection.Exports)
							{
								ScheduledServerTranslationMemoryExport scheduledServerTranslationMemoryExport = new ScheduledServerTranslationMemoryExport(TranslationProviderServer, export);
								scheduledServerTranslationMemoryExport.LanguageDirection = ServerBasedTranslationMemoryLanguageDirection.BuildServerBasedTranslationMemoryLanguageDirection(new ClientObjectBuilder(TranslationProviderServer), languageDirection);
								list.Add(scheduledServerTranslationMemoryExport);
							}
						}
					}
				}
				return new ReadOnlyCollection<ScheduledServerTranslationMemoryExport>(list);
			}
		}

		/// <summary>
		/// Gets a list of import tasks currently associated with the language directions encapsulated by this translation memory.
		/// </summary>
		public ReadOnlyCollection<ScheduledServerTranslationMemoryImport> CurrentImportTasks
		{
			get
			{
				List<ScheduledServerTranslationMemoryImport> list = new List<ScheduledServerTranslationMemoryImport>();
				if (base.Entity.LanguageDirections != null)
				{
					foreach (LanguageDirectionEntity languageDirection in base.Entity.LanguageDirections)
					{
						if (languageDirection.Imports != null)
						{
							foreach (ImportEntity import in languageDirection.Imports)
							{
								if (import.ScheduledOperation != null)
								{
									ScheduledServerTranslationMemoryImport scheduledServerTranslationMemoryImport = new ScheduledServerTranslationMemoryImport(TranslationProviderServer, import);
									scheduledServerTranslationMemoryImport.LanguageDirection = ServerBasedTranslationMemoryLanguageDirection.BuildServerBasedTranslationMemoryLanguageDirection(new ClientObjectBuilder(TranslationProviderServer), languageDirection);
									list.Add(scheduledServerTranslationMemoryImport);
								}
							}
						}
					}
				}
				return new ReadOnlyCollection<ScheduledServerTranslationMemoryImport>(list);
			}
		}

		/// <summary>
		/// Gets or sets the flags affecting tokenizer behaviour for this TM.
		/// </summary>
		public TokenizerFlags TokenizerFlags
		{
			get
			{
				VerifyNotDeleted();
				return base.Entity.TokenizerFlags.GetValueOrDefault();
			}
			set
			{
				VerifyNotDeleted();
				base.Entity.TokenizerFlags = value;
				OnPropertyChanged("TokenizerFlags");
			}
		}

		/// <summary>
		/// Gets or sets the flags affecting word count behaviour for this TM.
		/// </summary>
		public WordCountFlags WordCountFlags
		{
			get
			{
				VerifyNotDeleted();
				VerifyNotDeleted();
				ServerBasedTranslationMemoryVersion serverVersion = TranslationProviderServer.GetServerVersion();
				if ((uint)serverVersion <= 3u && base.Entity.WordCountFlags.HasValue)
				{
					return base.Entity.WordCountFlags ?? WordCountFlags.AllFlags;
				}
				return base.Entity.WordCountFlags ?? (WordCountFlags.BreakOnHyphen | WordCountFlags.BreakOnDash | WordCountFlags.BreakOnTag | WordCountFlags.BreakOnApostrophe);
			}
			set
			{
				VerifyNotDeleted();
				ServerBasedTranslationMemoryVersion serverVersion = TranslationProviderServer.GetServerVersion();
				if ((uint)serverVersion <= 3u && base.Entity.WordCountFlags.HasValue)
				{
					value &= ~WordCountFlags.BreakOnApostrophe;
				}
				base.Entity.WordCountFlags = value;
				OnPropertyChanged("WordCountFlags");
			}
		}

		/// <summary>
		/// Gets or sets the context matching type flag
		/// </summary>
		public TextContextMatchType TextContextMatchType
		{
			get
			{
				return base.Entity.TextContextMatchType ?? TextContextMatchType.PrecedingAndFollowingSource;
			}
			set
			{
				if (base.Entity != null)
				{
					base.Entity.TextContextMatchType = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets flag determining whether legacy hashes are used
		/// </summary>
		public bool UsesLegacyHashes
		{
			get
			{
				return base.Entity.UsesLegacyHashes ?? true;
			}
			set
			{
				base.Entity.UsesLegacyHashes = value;
			}
		}

		/// <summary>
		/// Gets or sets the flag determining whether id context matching is used
		/// </summary>
		public bool UsesIdContextMatching
		{
			get
			{
				return base.Entity.UsesIdContextMatching ?? true;
			}
			set
			{
				base.Entity.UsesIdContextMatching = value;
			}
		}

		/// <summary>
		/// Gets the valid server-based translation memory schemes.
		/// </summary>
		/// <returns>server-based translation memory schemes</returns>
		private static string[] GetServerBasedTranslationMemorySchemes()
		{
			return new string[2]
			{
				"sdltm.http",
				"sdltm.https"
			};
		}

		/// <summary>
		/// Determines whether the given URI represents a server-based translation memory.
		/// </summary>
		/// <param name="uri">The URI to check.</param>
		/// <returns>Whether <paramref name="uri" /> represents server-based translation memory</returns>
		public static bool IsServerBasedTranslationMemory(Uri uri)
		{
			if (uri != null)
			{
				return GetServerBasedTranslationMemorySchemes().FirstOrDefault((string scheme) => uri.Scheme.Equals(scheme, StringComparison.OrdinalIgnoreCase)) != null;
			}
			return false;
		}

		/// <summary>
		/// Determines the TM Server version
		/// </summary>
		/// <param name="uri"></param>
		/// <returns></returns>
		public static ServerBasedTranslationMemoryVersion GetServerVersion(Uri uri)
		{
			TranslationMemoryAdministrationClient translationMemoryAdministrationClient = CreateTranslationMemoryAdministrationClient(uri);
			return (ServerBasedTranslationMemoryVersion)translationMemoryAdministrationClient.ServerVersion;
		}

		private static TranslationMemoryAdministrationClient CreateTranslationMemoryAdministrationClient(Uri uri)
		{
			string text = uri.AbsoluteUri;
			if (text.StartsWith("sdltm.", StringComparison.InvariantCultureIgnoreCase))
			{
				if (!string.IsNullOrWhiteSpace(uri.Query))
				{
					text = text.Replace(uri.Query, "");
				}
				text = text.Substring("sdltm.".Length);
			}
			return new TranslationMemoryAdministrationClient(text);
		}

		/// <summary>
		/// Check if the server supports Translation and Analysis Service
		/// </summary>
		/// <param name="uri"></param>
		/// <returns></returns>
		public static bool ServerSupportsTranslationAndAnalysisService(Uri uri)
		{
			try
			{
				TranslationMemoryAdministrationClient translationMemoryAdministrationClient = CreateTranslationMemoryAdministrationClient(uri);
				TMServerVersion serverVersion = translationMemoryAdministrationClient.ServerVersion;
				return serverVersion == TMServerVersion.OnPremiseRest && translationMemoryAdministrationClient.SupportsTranslationAndAnalysisService;
			}
			catch (AggregateException)
			{
				return false;
			}
		}

		/// <summary>
		/// Gets the server-based translation memory name from the given URI.
		/// </summary>
		/// <param name="uri">URI</param>
		/// <returns>server-based translation memory name</returns>
		public static string GetServerBasedTranslationMemoryPath(Uri uri)
		{
			if (!IsServerBasedTranslationMemory(uri))
			{
				throw new ArgumentException(StringResources.UnvalidServerBasedTMUri, "uri");
			}
			if (!string.IsNullOrWhiteSpace(uri.Query))
			{
				int num = uri.OriginalString.IndexOf('?');
				string text = uri.OriginalString.Substring(num + 1);
				Dictionary<string, string> dictionary = text.Split('&').ToDictionary((string x) => x.Split('=')[0], (string x) => Uri.UnescapeDataString(x.Split('=')[1]));
				return (dictionary["orgPath"] + "/" + dictionary["tmName"]).Replace("//", "/");
			}
			string components = uri.GetComponents(UriComponents.AbsoluteUri, UriFormat.Unescaped);
			string[] array = components.Split(new char[1]
			{
				'/'
			}, StringSplitOptions.RemoveEmptyEntries);
			string text2 = "";
			for (int i = 2; i < array.Count(); i++)
			{
				text2 = text2 + "/" + array[i];
			}
			return text2;
		}

		internal static ServerBasedTranslationMemory BuildServerBasedTranslationMemory(ClientObjectBuilder builder, TranslationMemoryEntity entity)
		{
			ClientObjectKey key = builder.CreateKey(entity);
			ServerBasedTranslationMemory serverBasedTranslationMemory = builder[key] as ServerBasedTranslationMemory;
			if (serverBasedTranslationMemory != null)
			{
				return serverBasedTranslationMemory;
			}
			serverBasedTranslationMemory = (ServerBasedTranslationMemory)(builder[key] = new ServerBasedTranslationMemory(builder.Server, entity));
			ClientObjectKey key2 = builder.CreateKey<FieldGroupTemplateEntity>(entity.FieldGroupTemplate.ForeignKey);
			ServerBasedFieldsTemplate serverBasedFieldsTemplate = builder[key2] as ServerBasedFieldsTemplate;
			if (serverBasedFieldsTemplate == null && entity.FieldGroupTemplate.IsLoaded)
			{
				serverBasedFieldsTemplate = ServerBasedFieldsTemplate.BuildServerBasedFieldsTemplate(builder, entity.FieldGroupTemplate.Entity);
			}
			serverBasedTranslationMemory._lazyFieldsTemplate = serverBasedFieldsTemplate;
			ClientObjectKey key3 = builder.CreateKey<LanguageResourceTemplateEntity>(entity.LanguageResourceTemplate.ForeignKey);
			ServerBasedLanguageResourcesTemplate serverBasedLanguageResourcesTemplate = builder[key3] as ServerBasedLanguageResourcesTemplate;
			if (serverBasedLanguageResourcesTemplate == null && entity.LanguageResourceTemplate.IsLoaded)
			{
				serverBasedLanguageResourcesTemplate = ServerBasedLanguageResourcesTemplate.BuildServerBasedLanguageResourcesTemplate(builder, entity.LanguageResourceTemplate.Entity);
			}
			serverBasedTranslationMemory._lazyLanguageResourcesTemplate = serverBasedLanguageResourcesTemplate;
			ClientObjectKey key4 = builder.CreateKey<ContainerEntity>(entity.Container.ForeignKey);
			TranslationMemoryContainer translationMemoryContainer = builder[key4] as TranslationMemoryContainer;
			if (translationMemoryContainer == null && entity.Container.IsLoaded)
			{
				translationMemoryContainer = TranslationMemoryContainer.BuildTranslationMemoryContainer(builder, entity.Container.Entity);
			}
			entity.Container = new EntityReference<ContainerEntity>(entity.Container.ForeignKey);
			serverBasedTranslationMemory._lazyContainer = translationMemoryContainer;
			if (entity.LanguageDirections.IsLoaded)
			{
				serverBasedTranslationMemory._lazyLanguageDirections = new ServerBasedTranslationMemoryLanguageDirectionCollection(serverBasedTranslationMemory, entity.LanguageDirections);
			}
			if (entity.CurrentRecomputeStatisticsOperation != null)
			{
				serverBasedTranslationMemory._lazyCurrentRecomputeStatisticsOperation = new ScheduledRecomputeStatisticsOperation(entity.CurrentRecomputeStatisticsOperation)
				{
					TranslationMemory = serverBasedTranslationMemory
				};
			}
			if (entity.CurrentReindexOperation != null)
			{
				serverBasedTranslationMemory._lazyCurrentReindexOperation = new ScheduledReindexOperation(entity.CurrentReindexOperation)
				{
					TranslationMemory = serverBasedTranslationMemory
				};
			}
			return serverBasedTranslationMemory;
		}

		/// <summary>
		/// Creates a new translation memory. Note that you have to call <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedTranslationMemory.Save" /> to 
		/// persist the translation memory.
		/// </summary>
		/// <param name="server">The server with which the database server should be registered.</param>
		/// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="server" /> is null.</exception>
		public ServerBasedTranslationMemory(TranslationProviderServer server)
			: base(new TranslationMemoryEntity())
		{
			if (server == null)
			{
				throw new ArgumentNullException("server");
			}
			TranslationProviderServer = server;
			base.Entity.UniqueId = Guid.NewGuid();
			base.Entity.Recognizers = BuiltinRecognizers.RecognizeAll;
			base.Entity.FuzzyIndexes = (FuzzyIndexes.SourceWordBased | FuzzyIndexes.TargetWordBased);
			base.Entity.MinScoreIncrease = 20;
			base.Entity.MinSearchVectorLengthSourceCharIndex = 5;
			base.Entity.MinSearchVectorLengthSourceWordIndex = 3;
			base.Entity.MinSearchVectorLengthTargetCharIndex = 5;
			base.Entity.MinSearchVectorLengthTargetWordIndex = 3;
			base.Entity.IsProjectTranslationMemory = false;
			base.Entity.TextContextMatchType = TextContextMatchType.PrecedingSourceAndTarget;
			_lazyLanguageDirections = new ServerBasedTranslationMemoryLanguageDirectionCollection(this, base.Entity.LanguageDirections);
			InitializeTmSpecificFields();
			InitializeTmSpecificLanguageResources();
		}

		private void InitializeTmSpecificFields()
		{
			FieldGroupTemplateEntity fieldGroupTemplateEntity = new FieldGroupTemplateEntity();
			fieldGroupTemplateEntity.UniqueId = Guid.NewGuid();
			fieldGroupTemplateEntity.Name = base.Entity.UniqueId.ToString();
			fieldGroupTemplateEntity.IsTmSpecific = true;
			base.Entity.FieldGroupTemplate = new EntityReference<FieldGroupTemplateEntity>(fieldGroupTemplateEntity);
			ServerBasedFieldsTemplate newFieldsTemplate = new ServerBasedFieldsTemplate(this, isReadOnly: false, isNewTMSpecific: true);
			if (_lazyFieldsTemplate != null)
			{
				_lazyFieldsTemplate.FieldDefinitions.ToList().ForEach(delegate(FieldDefinition fd)
				{
					FieldDefinition newFd = new FieldDefinition(fd.Name, fd.ValueType);
					if (fd.IsPicklist)
					{
						fd.PicklistItems.ToList().ForEach(delegate(PicklistItemDefinition pid)
						{
							newFd.PicklistItems.Add(new PicklistItemDefinition(pid.Name));
						});
					}
					newFieldsTemplate.FieldDefinitions.Add(newFd);
				});
			}
			_lazyFieldsTemplate = newFieldsTemplate;
		}

		private void InitializeTmSpecificLanguageResources()
		{
			LanguageResourceTemplateEntity languageResourceTemplateEntity = new LanguageResourceTemplateEntity();
			languageResourceTemplateEntity.UniqueId = Guid.NewGuid();
			languageResourceTemplateEntity.Name = base.Entity.UniqueId.ToString();
			languageResourceTemplateEntity.IsTmSpecific = true;
			base.Entity.LanguageResourceTemplate = new EntityReference<LanguageResourceTemplateEntity>(languageResourceTemplateEntity);
			ServerBasedLanguageResourcesTemplate newLanguageResourceTemplate = new ServerBasedLanguageResourcesTemplate(this, isReadOnly: false, isNewTMSpecific: true);
			if (_lazyLanguageResourcesTemplate != null)
			{
				_lazyLanguageResourcesTemplate.LanguageResourceBundles.ToList().ForEach(delegate(LanguageResourceBundle bundle)
				{
					LanguageResourceBundle languageResourceBundle = new LanguageResourceBundle(bundle.LanguageCode);
					newLanguageResourceTemplate.LanguageResourceBundles.Add(languageResourceBundle);
					if (bundle.Abbreviations != null)
					{
						languageResourceBundle.Abbreviations = new Wordlist();
						CopyWordList(bundle.Abbreviations, languageResourceBundle.Abbreviations);
					}
					else
					{
						languageResourceBundle.Abbreviations = null;
					}
					if (bundle.OrdinalFollowers != null)
					{
						languageResourceBundle.OrdinalFollowers = new Wordlist();
						CopyWordList(bundle.OrdinalFollowers, languageResourceBundle.OrdinalFollowers);
					}
					else
					{
						languageResourceBundle.OrdinalFollowers = null;
					}
					if (bundle.Variables != null)
					{
						languageResourceBundle.Variables = new Wordlist();
						CopyWordList(bundle.Variables, languageResourceBundle.Variables);
					}
					else
					{
						languageResourceBundle.Variables = null;
					}
					if (bundle.SegmentationRules != null)
					{
						languageResourceBundle.SegmentationRules = (bundle.SegmentationRules.Clone() as SegmentationRules);
					}
				});
			}
			_lazyLanguageResourcesTemplate = newLanguageResourceTemplate;
		}

		private static void CopyWordList(Wordlist oldList, Wordlist newList)
		{
			oldList?.Items.ToList().ForEach(delegate(string word)
			{
				newList.Add(word);
			});
		}

		/// <summary>
		/// Constructor for existing database servers.
		/// </summary>
		/// <param name="server">The server to which the translation memory belongs.</param>
		/// <param name="entity">The translation memory entity.</param>
		/// <exception cref="T:System.ArgumentNullException">Thrown when trying to set TranslationProviderServer to null.</exception>
		/// <exception cref="T:System.ArgumentNullException">Thrown when trying to set TranslationMemoryEntity to null.</exception>
		/// <exception cref="T:System.ArgumentException">Thrown when translation memory id is not set.</exception>
		internal ServerBasedTranslationMemory(TranslationProviderServer server, TranslationMemoryEntity entity)
			: base(entity)
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
				throw new ArgumentException(StringResources.DatabaseServer_EntityHasNoId, "entity");
			}
			TranslationProviderServer = server;
		}

		/// <summary>
		/// Gets the language direction with specified source and target language.
		/// </summary>
		/// <param name="languageDirection">The language direction.</param>
		/// <returns>The language direction; or null if no such language direction exists.</returns>
		/// <exception cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ObjectDeletedException">Thrown when this object has been deleted.</exception>
		public ServerBasedTranslationMemoryLanguageDirection GetLanguageDirection(LanguagePair languageDirection)
		{
			VerifyNotDeleted();
			return LanguageDirections.FirstOrDefault((ServerBasedTranslationMemoryLanguageDirection ld) => ld.SourceLanguageCode.Equals(languageDirection.SourceCultureName, StringComparison.OrdinalIgnoreCase) && ld.TargetLanguageCode.Equals(languageDirection.TargetCultureName, StringComparison.OrdinalIgnoreCase));
		}

		private void VerifyFieldTemplateLoaded()
		{
			if (_lazyFieldsTemplate == null && !base.Entity.FieldGroupTemplate.IsLoaded && base.Entity.FieldGroupTemplate.ForeignKey != null)
			{
				_lazyFieldsTemplate = TranslationProviderServer.GetFieldsTemplate((Guid)base.Entity.FieldGroupTemplate.ForeignKey.Value, FieldsTemplateProperties.All);
				base.Entity.FieldGroupTemplate = new EntityReference<FieldGroupTemplateEntity>(_lazyFieldsTemplate.Entity);
			}
		}

		private void VerifyLanguageResourceTemplateLoaded()
		{
			if (_lazyLanguageResourcesTemplate == null && !base.Entity.LanguageResourceTemplate.IsLoaded && base.Entity.LanguageResourceTemplate.ForeignKey != null)
			{
				_lazyLanguageResourcesTemplate = TranslationProviderServer.GetLanguageResourcesTemplate((Guid)base.Entity.LanguageResourceTemplate.ForeignKey.Value, LanguageResourcesTemplateProperties.All);
				base.Entity.LanguageResourceTemplate = new EntityReference<LanguageResourceTemplateEntity>(_lazyLanguageResourcesTemplate.Entity);
			}
		}

		ITranslationMemoryLanguageDirection ITranslationMemory.GetLanguageDirection(LanguagePair languageDirection)
		{
			VerifyNotDeleted();
			return GetLanguageDirection(languageDirection);
		}

		/// <summary>
		/// Saves the changes to this translation memory. This includes
		/// changes to language directions, fields and language resources.
		/// </summary>
		/// <exception cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ObjectDeletedException">Thrown when this object has been deleted.</exception>
		public void Save()
		{
			VerifyNotDeleted();
			VerifyFieldTemplateLoaded();
			VerifyLanguageResourceTemplateLoaded();
			ServerBasedTranslationMemoryVersion serverVersion = TranslationProviderServer.GetServerVersion();
			if ((uint)serverVersion <= 3u && base.Entity.WordCountFlags.HasValue)
			{
				base.Entity.WordCountFlags = (base.Entity.WordCountFlags.Value & ~WordCountFlags.BreakOnApostrophe);
			}
			if (base.Entity.Container.ForeignKey == null)
			{
				throw new NullPropertyException("Container does not exist for this TM.");
			}
			if (_lazyLanguageResourcesTemplate.LanguageResourcesLoaded)
			{
				_lazyLanguageResourcesTemplate.LanguageResourceBundles.SaveToEntities();
			}
			if (_lazyFuzzyIndexTuningSettings != null)
			{
				if (base.Entity.MinScoreIncrease.Value != _lazyFuzzyIndexTuningSettings.MinScoreIncrease)
				{
					base.Entity.MinScoreIncrease = _lazyFuzzyIndexTuningSettings.MinScoreIncrease;
				}
				if (base.Entity.MinSearchVectorLengthSourceCharIndex.Value != _lazyFuzzyIndexTuningSettings.MinSearchVectorLengthSourceCharIndex)
				{
					base.Entity.MinSearchVectorLengthSourceCharIndex = _lazyFuzzyIndexTuningSettings.MinSearchVectorLengthSourceCharIndex;
				}
				if (base.Entity.MinSearchVectorLengthTargetCharIndex.Value != _lazyFuzzyIndexTuningSettings.MinSearchVectorLengthTargetCharIndex)
				{
					base.Entity.MinSearchVectorLengthTargetCharIndex = _lazyFuzzyIndexTuningSettings.MinSearchVectorLengthTargetCharIndex;
				}
				if (base.Entity.MinSearchVectorLengthSourceWordIndex.Value != _lazyFuzzyIndexTuningSettings.MinSearchVectorLengthSourceWordIndex)
				{
					base.Entity.MinSearchVectorLengthSourceWordIndex = _lazyFuzzyIndexTuningSettings.MinSearchVectorLengthSourceWordIndex;
				}
				if (base.Entity.MinSearchVectorLengthTargetWordIndex.Value != _lazyFuzzyIndexTuningSettings.MinSearchVectorLengthTargetWordIndex)
				{
					base.Entity.MinSearchVectorLengthTargetWordIndex = _lazyFuzzyIndexTuningSettings.MinSearchVectorLengthTargetWordIndex;
				}
			}
			TranslationMemoryEntity translationMemoryEntity;
			if (base.IsNewObject)
			{
				translationMemoryEntity = TranslationProviderServer.Service.CreateTranslationMemory(base.Entity, ParentResourceGroupPath);
				base.Entity.Id = translationMemoryEntity.Id;
				base.Entity.UniqueId = translationMemoryEntity.UniqueId;
				base.Entity.CreationDate = translationMemoryEntity.CreationDate;
				base.Entity.CreationUser = translationMemoryEntity.CreationUser;
			}
			else
			{
				VerifyPermission("tm.edit");
				translationMemoryEntity = TranslationProviderServer.Service.UpdateTranslationMemory(base.Entity);
			}
			base.Entity.ParentResourceGroupDescription = translationMemoryEntity.ParentResourceGroupDescription;
			base.Entity.ParentResourceGroupName = translationMemoryEntity.ParentResourceGroupName;
			base.Entity.ParentResourceGroupPath = translationMemoryEntity.ParentResourceGroupPath;
			base.Entity.Permissions = translationMemoryEntity.Permissions;
			base.Entity.LanguageDirections = translationMemoryEntity.LanguageDirections;
			if (_lazyLanguageDirections != null)
			{
				_lazyLanguageDirections.UpdateEntities(translationMemoryEntity.LanguageDirections);
			}
			if (_lazyFieldsTemplate.Entity.IsTmSpecific.HasValue && _lazyFieldsTemplate.Entity.IsTmSpecific.Value && translationMemoryEntity.FieldGroupTemplate.IsLoaded)
			{
				_lazyFieldsTemplate.Entity = translationMemoryEntity.FieldGroupTemplate.Entity;
			}
			if (translationMemoryEntity.FieldGroupTemplate.IsLoaded)
			{
				base.Entity.FieldGroupTemplate = translationMemoryEntity.FieldGroupTemplate;
			}
			else if (base.Entity.FieldGroupTemplate.ForeignKey == null)
			{
				base.Entity.FieldGroupTemplate.ForeignKey = translationMemoryEntity.FieldGroupTemplate.ForeignKey;
			}
			if (_lazyLanguageResourcesTemplate.Entity.IsTmSpecific.HasValue && _lazyLanguageResourcesTemplate.Entity.IsTmSpecific.Value && translationMemoryEntity.LanguageResourceTemplate.IsLoaded)
			{
				_lazyLanguageResourcesTemplate.Entity = translationMemoryEntity.LanguageResourceTemplate.Entity;
			}
			if (translationMemoryEntity.LanguageResourceTemplate.IsLoaded)
			{
				base.Entity.LanguageResourceTemplate = translationMemoryEntity.LanguageResourceTemplate;
			}
			else if (base.Entity.LanguageResourceTemplate.ForeignKey == null)
			{
				base.Entity.LanguageResourceTemplate.ForeignKey = translationMemoryEntity.LanguageResourceTemplate.ForeignKey;
			}
		}

		/// <summary>
		/// Deletes this translation memory.
		/// </summary>
		/// <remarks>The translation memory will be deleted from the server, including all its content.</remarks>
		/// <exception cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ObjectDeletedException">Thrown when this object has been deleted.</exception>
		/// <exception cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ObjectNotSavedException">Thrown when this object has not been initially saved yet.</exception>
		public void Delete()
		{
			VerifyNotDeleted();
			VerifyExistingObject();
			VerifyPermission("tm.delete");
			TranslationProviderServer.Service.DeleteTranslationMemory(base.Entity.Id);
			base.Entity = null;
		}

		/// <summary>
		/// Gets the total translation unit count for all language directions in this translation memory.
		/// </summary>
		/// <returns>
		/// The total translation unit count for this TM.
		/// </returns>
		/// <exception cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ObjectDeletedException">Thrown when this object has been deleted.</exception>
		public int GetTranslationUnitCount()
		{
			int num = 0;
			foreach (ServerBasedTranslationMemoryLanguageDirection languageDirection in LanguageDirections)
			{
				num += languageDirection.GetTranslationUnitCount();
			}
			return num;
		}

		/// <summary>
		/// Checks whether this translation provider supports the specified language direction.
		/// </summary>
		/// <param name="languageDirection">The language direction.</param>
		/// <returns>
		/// True if the specified language direction is supported.
		/// </returns>
		/// <exception cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ObjectDeletedException">Thrown when this object has been deleted.</exception>
		public bool SupportsLanguageDirection(LanguagePair languageDirection)
		{
			VerifyNotDeleted();
			return LanguageDirections.GetLanguageDirection(languageDirection.SourceCultureName, languageDirection.TargetCultureName) != null;
		}

		/// <summary>
		/// Gets a translation provider for the specified language direction.
		/// </summary>
		/// <param name="languageDirection">The language direction.</param>
		/// <returns>
		/// The language direction matching the given source and target language.
		/// </returns>
		/// <exception cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ObjectDeletedException">Thrown when this object has been deleted.</exception>
		ITranslationProviderLanguageDirection ITranslationProvider.GetLanguageDirection(LanguagePair languageDirection)
		{
			VerifyNotDeleted();
			return GetLanguageDirection(languageDirection);
		}

		/// <summary>
		/// Ensures that the provider's status information (<see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedTranslationMemory.StatusInfo" />) is refreshed,
		/// in case it is cached.
		/// </summary>
		public void RefreshStatusInfo()
		{
		}

		/// <summary>
		/// Serializes any meaningful state information for this translation provider that can be stored in projects
		/// and sent around the supply chain.
		/// </summary>
		/// <returns>
		/// A string representing the state of this translation provider that can later be passed into
		/// the <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedTranslationMemory.LoadState(System.String)" /> method to restore the state after creating a new translation provider.
		/// </returns>
		/// <remarks>The format of this string can be decided upon by the translation provider implementation.</remarks>
		public string SerializeState()
		{
			return null;
		}

		/// <summary>
		/// Loads previously serialized state information into this translation provider instance.
		/// </summary>
		/// <param name="translationProviderState">A string representing the state of translation provider that was previously saved
		/// using <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedTranslationMemory.SerializeState" />.</param>
		/// <remarks>The format of this string can be decided upon by the translation provider implementation.</remarks>
		public void LoadState(string translationProviderState)
		{
		}

		/// <summary>
		/// Checks whether the current user has the specified permission on this translation memory.
		/// </summary>
		/// <param name="permission">A permission ID. See <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.TranslationMemoryPermissions" />.</param>
		/// <returns>
		/// True if the user has the specified permission for this TM.
		/// </returns>
		public bool HasPermission(string permission)
		{
			VerifyNotDeleted();
			if (base.Entity.Permissions == null)
			{
				return true;
			}
			return base.Entity.Permissions.HasPermission(permission);
		}

		/// <summary>
		/// Gets a flag which indicates whether it is recommended to recompute the fuzzy
		/// index statistics (see <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedTranslationMemory.RecomputeFuzzyIndexStatistics" />).
		/// </summary>
		/// <returns><c>true</c> if it is recommended to recompute the fuzzy index statistics.</returns>
		public bool ShouldRecomputeFuzzyIndexStatistics()
		{
			return RecomputeStatisticsHelper.ShouldRecomputeFuzzyIndexStatistics(FuzzyIndexStatisticsSize, GetTranslationUnitCount());
		}

		/// <summary>
		/// Synchronously recomputes the fuzzy index statistics of the TM.
		/// </summary>
		/// <remarks>This can be a long running operation, especially for larger TMs. It is recommended to use
		/// <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ScheduledRecomputeStatisticsOperation" /> to perform this operation on the Execution Server.</remarks>
		public void RecomputeFuzzyIndexStatistics()
		{
			RecomputeStatisticsResult recomputeStatisticsResult = TranslationProviderServer.Service.RecomputeFuzzyIndexStatistics(base.Entity.Id);
			base.Entity.LastRecomputeDate = recomputeStatisticsResult.LastRecomputeDate;
			base.Entity.LastRecomputeSize = recomputeStatisticsResult.LastRecomputeSize;
		}

		/// <summary>
		/// Reindexes this translation memory.
		/// </summary>
		public void Reindex()
		{
			TranslationProviderServer.Service.ReindexTranslationMemory(base.Entity.Id);
		}

		private void VerifyExistingObject()
		{
			VerifyExistingObject("You need to save this translation memory before being able to perform this operation.");
		}

		private void VerifyExistingObject(string message)
		{
			if (base.IsNewObject)
			{
				throw new ObjectDeletedException(message);
			}
		}

		private void VerifyNewObject(string message)
		{
			if (!base.IsNewObject)
			{
				throw new ObjectDeletedException(message);
			}
		}

		private void VerifyNotDeleted(string message)
		{
			if (base.IsDeleted)
			{
				throw new ObjectDeletedException(message);
			}
		}

		private void VerifyPermission(string permission)
		{
			if (!HasPermission(permission))
			{
				throw new SecurityException($"Current user does not have {permission} permission on this translation memory.");
			}
		}

		void IEditableObject.BeginEdit()
		{
			if (base.Entity != null && _backupEntity == null)
			{
				_backupEntity = base.Entity.Clone();
				_backupEntity.MarkAsClean();
			}
		}

		void IEditableObject.CancelEdit()
		{
			if (base.Entity != null && _backupEntity != null)
			{
				base.Entity = _backupEntity;
				_backupEntity = null;
			}
		}

		void IEditableObject.EndEdit()
		{
			_backupEntity = null;
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
		/// </returns>
		public bool Equals(ServerBasedTranslationMemory other)
		{
			return base.Id == other.Id;
		}

		/// <summary>
		/// Determines whether the specified <see cref="T:System.Object" /> is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="T:System.Object" /> to compare with this instance.</param>
		/// <returns>
		/// 	<c>true</c> if the specified <see cref="T:System.Object" /> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		/// <exception cref="T:System.NullReferenceException">
		/// The <paramref name="obj" /> parameter is null.
		/// </exception>
		public override bool Equals(object obj)
		{
			ServerBasedTranslationMemory serverBasedTranslationMemory = obj as ServerBasedTranslationMemory;
			if (serverBasedTranslationMemory != null)
			{
				return Equals(serverBasedTranslationMemory);
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
			return base.Id.GetHashCode();
		}
	}
}
