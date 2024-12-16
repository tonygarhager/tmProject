using Sdl.Core.Api.DataAccess;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Segmentation;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory;
//using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Client;
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

		public TranslationProviderServer TranslationProviderServer
		{
			get;
			private set;
		}

		public string CreationUserName
		{
			get
			{
				VerifyNotDeleted();
				return base.Entity.CreationUser;
			}
		}

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

		public ServerBasedTranslationMemoryLanguageDirectionCollection LanguageDirections
		{
			get
			{
				VerifyNotDeleted();
				if (_lazyLanguageDirections == null)
				{
					LanguageDirectionEntity[] languageDirectionsByTranslationMemoryId = null;// TranslationProviderServer.Service.GetLanguageDirectionsByTranslationMemoryId(base.Entity.Id, new string[0]);
                    base.Entity.LanguageDirections = new EntityCollection<LanguageDirectionEntity>(languageDirectionsByTranslationMemoryId);
					_lazyLanguageDirections = new ServerBasedTranslationMemoryLanguageDirectionCollection(this, base.Entity.LanguageDirections);
				}
				return _lazyLanguageDirections;
			}
		}

		public ReadOnlyCollection<LanguagePair> SupportedLanguageDirections
		{
			get
			{
				VerifyNotDeleted();
				List<LanguagePair> list = new List<LanguagePair>(LanguageDirections.Select((ServerBasedTranslationMemoryLanguageDirection ld) => new LanguagePair(ld.SourceLanguageCode, ld.TargetLanguageCode)));
				return new ReadOnlyCollection<LanguagePair>(list);
			}
		}

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

		public FieldDefinitionCollection FieldDefinitions
		{
			get
			{
				VerifyNotDeleted();
				VerifyFieldTemplateLoaded();
				return _lazyFieldsTemplate.FieldDefinitions;
			}
		}

		public LanguageResourceBundleCollection LanguageResourceBundles
		{
			get
			{
				VerifyNotDeleted();
				VerifyLanguageResourceTemplateLoaded();
				return _lazyLanguageResourcesTemplate.LanguageResourceBundles;
			}
		}

		public int CachedTranslationUnitCount => LanguageDirections.Sum((ServerBasedTranslationMemoryLanguageDirection ld) => ld.CachedTranslationUnitCount);

		public ProviderStatusInfo StatusInfo => new ProviderStatusInfo(available: true, "OK");

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

		public bool SupportsTaggedInput => true;

		public bool SupportsScoring => true;

		public bool SupportsSearchForTranslationUnits => true;

		public bool SupportsSearchForTranslationUnitUsingContainsOperator => false;// TranslationProviderServer.Service.SupportsSearchForTranslationUnitUsingContainsOperator;

		public bool SupportsMultipleResults => true;

		public bool SupportsFilters => true;

		public bool SupportsPenalties => true;

		public bool SupportsStructureContext => true;

		public bool SupportsDocumentSearches => true;

		public bool SupportsUpdate => true;

		public bool SupportsPlaceables => true;

		public bool SupportsTranslation => true;

		public bool SupportsFuzzySearch => true;

		public bool SupportsConcordanceSearch => true;

		public bool SupportsSourceConcordanceSearch => true;

		public bool SupportsTargetConcordanceSearch => true;

		public bool SupportsWordCounts => true;

		public TranslationMethod TranslationMethod => TranslationMethod.TranslationMemory;

		public bool IsReadOnly => false;

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

		public TranslationMemoryContainer Container
		{
			get
			{
				VerifyNotDeleted();
				if (_lazyContainer == null && base.Entity.Container.ForeignKey != null && base.Entity.Container.ForeignKey.Value != null)
				{
					ContainerEntity containerById = null;// TranslationProviderServer.Service.GetContainerById(base.Entity.Container.ForeignKey, new string[0]);
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

		public ScheduledRecomputeStatisticsOperation CurrentRecomputeStatisticsOperation
		{
			get
			{
				if (base.Entity.CurrentRecomputeStatisticsWorkItemUniqueId.HasValue && _lazyCurrentRecomputeStatisticsOperation == null)
				{
					ScheduledOperationEntity scheduledOperation = null;// TranslationProviderServer.Service.GetScheduledOperation(base.Entity.CurrentRecomputeStatisticsWorkItemUniqueId.Value);
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

		public ScheduledReindexOperation CurrentReindexOperation
		{
			get
			{
				if (base.Entity.CurrentReindexWorkItemUniqueId.HasValue && _lazyCurrentReindexOperation == null)
				{
					ScheduledOperationEntity scheduledOperation = null;// TranslationProviderServer.Service.GetScheduledOperation(base.Entity.CurrentReindexWorkItemUniqueId.Value);
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

		public int? FuzzyIndexStatisticsSize => base.Entity.LastRecomputeSize;

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

		private static string[] GetServerBasedTranslationMemorySchemes()
		{
			return new string[2]
			{
				"sdltm.http",
				"sdltm.https"
			};
		}

		public static bool IsServerBasedTranslationMemory(Uri uri)
		{
			if (uri != null)
			{
				return GetServerBasedTranslationMemorySchemes().FirstOrDefault((string scheme) => uri.Scheme.Equals(scheme, StringComparison.OrdinalIgnoreCase)) != null;
			}
			return false;
		}

		public static ServerBasedTranslationMemoryVersion GetServerVersion(Uri uri)
		{
			TranslationMemoryAdministrationClient translationMemoryAdministrationClient = CreateTranslationMemoryAdministrationClient(uri);
			return ServerBasedTranslationMemoryVersion.Server2014SP2;// (ServerBasedTranslationMemoryVersion)translationMemoryAdministrationClient.ServerVersion;
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
			return null;// new TranslationMemoryAdministrationClient(text);
        }

		public static bool ServerSupportsTranslationAndAnalysisService(Uri uri)
		{
			try
			{
				TranslationMemoryAdministrationClient translationMemoryAdministrationClient = CreateTranslationMemoryAdministrationClient(uri);
				//TMServerVersion serverVersion = translationMemoryAdministrationClient.ServerVersion;
				return false;// serverVersion == TMServerVersion.OnPremiseRest && translationMemoryAdministrationClient.SupportsTranslationAndAnalysisService;
			}
			catch (AggregateException)
			{
				return false;
			}
		}

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
				translationMemoryEntity = null;// TranslationProviderServer.Service.CreateTranslationMemory(base.Entity, ParentResourceGroupPath);
				base.Entity.Id = translationMemoryEntity.Id;
				base.Entity.UniqueId = translationMemoryEntity.UniqueId;
				base.Entity.CreationDate = translationMemoryEntity.CreationDate;
				base.Entity.CreationUser = translationMemoryEntity.CreationUser;
			}
			else
			{
				VerifyPermission("tm.edit");
				translationMemoryEntity = null;// TranslationProviderServer.Service.UpdateTranslationMemory(base.Entity);
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

		public void Delete()
		{
			VerifyNotDeleted();
			VerifyExistingObject();
			VerifyPermission("tm.delete");
			//TranslationProviderServer.Service.DeleteTranslationMemory(base.Entity.Id);
			base.Entity = null;
		}

		public int GetTranslationUnitCount()
		{
			int num = 0;
			foreach (ServerBasedTranslationMemoryLanguageDirection languageDirection in LanguageDirections)
			{
				num += languageDirection.GetTranslationUnitCount();
			}
			return num;
		}

		public bool SupportsLanguageDirection(LanguagePair languageDirection)
		{
			VerifyNotDeleted();
			return LanguageDirections.GetLanguageDirection(languageDirection.SourceCultureName, languageDirection.TargetCultureName) != null;
		}

		ITranslationProviderLanguageDirection ITranslationProvider.GetLanguageDirection(LanguagePair languageDirection)
		{
			VerifyNotDeleted();
			return GetLanguageDirection(languageDirection);
		}

		public void RefreshStatusInfo()
		{
		}

		public string SerializeState()
		{
			return null;
		}

		public void LoadState(string translationProviderState)
		{
		}

		public bool HasPermission(string permission)
		{
			VerifyNotDeleted();
			if (base.Entity.Permissions == null)
			{
				return true;
			}
			return base.Entity.Permissions.HasPermission(permission);
		}

		public bool ShouldRecomputeFuzzyIndexStatistics()
		{
			return RecomputeStatisticsHelper.ShouldRecomputeFuzzyIndexStatistics(FuzzyIndexStatisticsSize, GetTranslationUnitCount());
		}

		public void RecomputeFuzzyIndexStatistics()
		{
			RecomputeStatisticsResult recomputeStatisticsResult = null;// TranslationProviderServer.Service.RecomputeFuzzyIndexStatistics(base.Entity.Id);
            base.Entity.LastRecomputeDate = recomputeStatisticsResult.LastRecomputeDate;
			base.Entity.LastRecomputeSize = recomputeStatisticsResult.LastRecomputeSize;
		}

		public void Reindex()
		{
			//TranslationProviderServer.Service.ReindexTranslationMemory(base.Entity.Id);
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

		public bool Equals(ServerBasedTranslationMemory other)
		{
			return base.Id == other.Id;
		}

		public override bool Equals(object obj)
		{
			ServerBasedTranslationMemory serverBasedTranslationMemory = obj as ServerBasedTranslationMemory;
			if (serverBasedTranslationMemory != null)
			{
				return Equals(serverBasedTranslationMemory);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.Id.GetHashCode();
		}
	}
}
