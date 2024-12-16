using Sdl.Core.Api.DataAccess;
using Sdl.Enterprise2.Studio.Platform.Client.IdentityModel;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory;
//using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Client;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Security.Principal;
using System.Threading;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public class TranslationProviderServer
	{
		private static readonly Uri DefaultUri = new Uri("http://localhost");

		public Uri Uri
		{
			get;
			private set;
		}

		internal UserCredentials Credentials
		{
			get;
			private set;
		}

		internal TranslationMemoryAdministrationClient Service
		{
			get;
		}

		public bool IsTranslationMemoryLocationSupported
		{
			get
			{
				ServerBasedTranslationMemoryVersion serverVersion = GetServerVersion();
				if ((uint)(serverVersion - 5) <= 1u)
				{
					return false;// Service.SupportsTranslationMemoryLocation;
				}
				return false;
			}
		}

		public bool IsTranslationAndAnalysisServiceSupported
		{
			get
			{
				ServerBasedTranslationMemoryVersion serverVersion = GetServerVersion();
				if (serverVersion == ServerBasedTranslationMemoryVersion.OnPremiseRest)
				{
					return false;// Service.SupportsTranslationAndAnalysisService;
				}
				return false;
			}
		}

		public TranslationProviderServer()
		{
			GenericPrincipal genericPrincipal = Thread.CurrentPrincipal as GenericPrincipal;
			if (genericPrincipal == null)
			{
				throw new SecurityException(StringResources.ErrorInvalidSdlThreadPrincipal);
			}
			Uri = DefaultUri;
			Service = new TranslationMemoryAdministrationClient();
		}

		internal TranslationProviderServer(Uri serverUri, UserCredentials credentials)
		{
			Uri = (serverUri ?? throw new ArgumentNullException("serverUri"));
			Credentials = (credentials ?? throw new ArgumentException("credentials"));
			Service = null;// new TranslationMemoryAdministrationClient(serverUri.ToString(), credentials);
		}

		public TranslationProviderServer(Uri serverUri, bool useWindowsAuthentication, string userName, string password)
		{
			Uri = (serverUri ?? throw new ArgumentNullException("serverUri"));
			UserCredentials userCredentials = null;
			if (useWindowsAuthentication)
			{
				if (string.IsNullOrEmpty(password))
				{
					userCredentials = new UserCredentials(string.Empty, string.Empty, UserManagerTokenType.WindowsUser);
				}
				else
				{
					if (string.IsNullOrEmpty(userName))
					{
						throw new ArgumentException("You must specify a user name when not using basic Windows authentication.", "userName");
					}
					userCredentials = new UserCredentials(userName, password, UserManagerTokenType.CustomWindowsUser);
				}
			}
			else
			{
				if (string.IsNullOrEmpty(userName))
				{
					throw new ArgumentException("You must specify a user name when not using integrated Windows authentication.", "userName");
				}
				if (string.IsNullOrEmpty(password))
				{
					throw new ArgumentException("You must specify a password when not using integrated Windows authentication.", "password");
				}
				userCredentials = new UserCredentials(userName, password, UserManagerTokenType.CustomUser);
			}
			Credentials = userCredentials;
			Service = null;// new TranslationMemoryAdministrationClient(serverUri.ToString(), userCredentials);
		}

		public TranslationProviderServer(Uri serverUri, string userName, string authToken, DateTime expirationDate)
		{
			if (serverUri == null)
			{
				throw new ArgumentNullException("serverUri");
			}
			if (string.IsNullOrEmpty(authToken))
			{
				throw new ArgumentNullException("authToken");
			}
			Uri = serverUri;
			Credentials = new UserCredentials(userName, new UserCredentials.SsoData
			{
				AuthToken = authToken,
				ExpirationDate = expirationDate
			}, UserManagerTokenType.Saml2User);
			Service = null;// new TranslationMemoryAdministrationClient(serverUri.ToString(), Credentials);
		}

		public DatabaseServer GetDatabaseServer(Guid id, DatabaseServerProperties additionalProperties)
		{
			if (id == Guid.Empty)
			{
				throw new ArgumentNullException("id");
			}
			DatabaseServerEntity databaseServerByUniqueId = null;// Service.GetDatabaseServerByUniqueId(id, GetDatabaseServerRelationships(additionalProperties));
			if (databaseServerByUniqueId == null)
			{
				return null;
			}
			ClientObjectBuilder builder = new ClientObjectBuilder(this);
			return DatabaseServer.BuildDatabaseServer(builder, databaseServerByUniqueId);
		}

		public DatabaseServer GetDatabaseServer(string path, DatabaseServerProperties additionalProperties)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path");
			}
			DatabaseServerEntity databaseServerByPath = null;// Service.GetDatabaseServerByPath(path, GetDatabaseServerRelationships(additionalProperties));
			if (databaseServerByPath == null)
			{
				return null;
			}
			ClientObjectBuilder builder = new ClientObjectBuilder(this);
			return DatabaseServer.BuildDatabaseServer(builder, databaseServerByPath);
		}

		public TranslationMemoryContainer GetContainer(Guid id, ContainerProperties additionalProperties)
		{
			if (id == Guid.Empty)
			{
				throw new ArgumentNullException("id");
			}
			ContainerEntity containerByUniqueId = null;// Service.GetContainerByUniqueId(id, GetContainerRelationships(additionalProperties));
			if (containerByUniqueId == null)
			{
				return null;
			}
			ClientObjectBuilder builder = new ClientObjectBuilder(this);
			return TranslationMemoryContainer.BuildTranslationMemoryContainer(builder, containerByUniqueId);
		}

		public LicensingStatusInformation GetLicensingStatusInformation()
		{
			LicensingRestrictionsEntity licensingRestrictions = null;// Service.GetLicensingRestrictions();
			return new LicensingStatusInformation(licensingRestrictions);
		}

		public TranslationMemoryContainer GetContainer(string path, ContainerProperties additionalProperties)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path");
			}
			ContainerEntity containerByPath = null;// Service.GetContainerByPath(path, GetContainerRelationships(additionalProperties));
			if (containerByPath == null)
			{
				return null;
			}
			ClientObjectBuilder builder = new ClientObjectBuilder(this);
			return TranslationMemoryContainer.BuildTranslationMemoryContainer(builder, containerByPath);
		}

		public ServerBasedTranslationMemory GetTranslationMemory(string path, TranslationMemoryProperties additionalProperties)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path");
			}
			bool includeLanguageResourceData;
			bool includeScheduledOperations;
			string[] translationMemoryRelationships = GetTranslationMemoryRelationships(additionalProperties, out includeLanguageResourceData, out includeScheduledOperations);
			TranslationMemoryEntity translationMemoryByPath = null;// Service.GetTranslationMemoryByPath(path, translationMemoryRelationships, includeLanguageResourceData, includeScheduledOperations);
			if (!(translationMemoryByPath != null))
			{
				return null;
			}
			return ServerBasedTranslationMemory.BuildServerBasedTranslationMemory(new ClientObjectBuilder(this), translationMemoryByPath);
		}

		public ServerBasedTranslationMemory GetTranslationMemory(Guid id, TranslationMemoryProperties additionalProperties)
		{
			if (id == Guid.Empty)
			{
				throw new ArgumentNullException("id");
			}
			bool includeLanguageResourceData;
			bool includeScheduledOperations;
			string[] translationMemoryRelationships = GetTranslationMemoryRelationships(additionalProperties, out includeLanguageResourceData, out includeScheduledOperations);
			TranslationMemoryEntity translationMemoryEntity = null;
			try
			{
				translationMemoryEntity = null;// Service.GetTranslationMemoryByUniqueId(id, translationMemoryRelationships, includeLanguageResourceData, includeScheduledOperations);
			}
			catch (InvalidOperationException)
			{
			}
			if (!(translationMemoryEntity != null))
			{
				return null;
			}
			return ServerBasedTranslationMemory.BuildServerBasedTranslationMemory(new ClientObjectBuilder(this), translationMemoryEntity);
		}

		public ServerBasedTranslationMemory GetTranslationMemorySummary(string path, TranslationMemoryProperties additionalProperties)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path");
			}
			bool flag = true;// Service.ServerVersion == TMServerVersion.OnPremiseRest || Service.ServerVersion == TMServerVersion.Cloud;
			bool includeTemplates = !flag;
			bool includeLanguageResourceData;
			bool includeScheduledOperations;
			string[] translationMemoryRelationshipsByProperty = GetTranslationMemoryRelationshipsByProperty(additionalProperties, includeTemplates, out includeLanguageResourceData, out includeScheduledOperations);
			TranslationMemoryEntity translationMemoryByPath = null;// Service.GetTranslationMemoryByPath(path, translationMemoryRelationshipsByProperty, includeLanguageResourceData, includeScheduledOperations);
			if (!(translationMemoryByPath != null))
			{
				return null;
			}
			return ServerBasedTranslationMemory.BuildServerBasedTranslationMemory(new ClientObjectBuilder(this), translationMemoryByPath);
		}

		public ServerBasedFieldsTemplate GetFieldsTemplate(Guid id, FieldsTemplateProperties additionalProperties)
		{
			if (id == Guid.Empty)
			{
				throw new ArgumentNullException("id");
			}
			FieldGroupTemplateEntity fieldGroupTemplateByUniqueId = null;// Service.GetFieldGroupTemplateByUniqueId(id, GetFieldGroupTemplateRelationships(additionalProperties));
			if (!(fieldGroupTemplateByUniqueId != null))
			{
				return null;
			}
			return ServerBasedFieldsTemplate.BuildServerBasedFieldsTemplate(new ClientObjectBuilder(this), fieldGroupTemplateByUniqueId);
		}

		public ServerBasedFieldsTemplate GetFieldsTemplate(string path, FieldsTemplateProperties additionalProperties)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path");
			}
			FieldGroupTemplateEntity fieldGroupTemplateByPath = null;// Service.GetFieldGroupTemplateByPath(path, GetFieldGroupTemplateRelationships(additionalProperties));
			if (!(fieldGroupTemplateByPath != null))
			{
				return null;
			}
			return ServerBasedFieldsTemplate.BuildServerBasedFieldsTemplate(new ClientObjectBuilder(this), fieldGroupTemplateByPath);
		}

		public ServerBasedLanguageResourcesTemplate GetLanguageResourcesTemplate(Guid id, LanguageResourcesTemplateProperties additionalProperties)
		{
			if (id == Guid.Empty)
			{
				throw new ArgumentNullException("id");
			}
			LanguageResourceTemplateEntity languageResourceTemplateByUniqueId = null;// Service.GetLanguageResourceTemplateByUniqueId(id, GetLanguageResourceTemplateRelationships(additionalProperties), IncludesLanguageResourceData(additionalProperties));
			if (!(languageResourceTemplateByUniqueId != null))
			{
				return null;
			}
			return new ServerBasedLanguageResourcesTemplate(this, languageResourceTemplateByUniqueId);
		}

		public ServerBasedLanguageResourcesTemplate GetLanguageResourcesTemplate(string path, LanguageResourcesTemplateProperties additionalProperties)
		{
			LanguageResourceTemplateEntity languageResourceTemplateByPath = null;// Service.GetLanguageResourceTemplateByPath(path, GetLanguageResourceTemplateRelationships(additionalProperties), IncludesLanguageResourceData(additionalProperties));
			if (!(languageResourceTemplateByPath != null))
			{
				return null;
			}
			return ServerBasedLanguageResourcesTemplate.BuildServerBasedLanguageResourcesTemplate(new ClientObjectBuilder(this), languageResourceTemplateByPath);
		}

		public LanguageResourceBundle GetDefaultLanguageResources(CultureInfo language)
		{
			if (language == null)
			{
				throw new ArgumentNullException("language");
			}
			return GetDefaultLanguageResources(language.Name);
		}

		public LanguageResourceBundle GetDefaultLanguageResources(string languageCode)
		{
			if (string.IsNullOrEmpty(languageCode))
			{
				throw new ArgumentNullException("languageCode");
			}
			LanguageResourceEntity[] defaultLanguageResources = null;// Service.GetDefaultLanguageResources(languageCode);
			EntityCollection<LanguageResourceEntity> entities = new EntityCollection<LanguageResourceEntity>(defaultLanguageResources);
			return new LanguageResourceBundle(languageCode, entities);
		}

		public TranslationMemoryQueryFilters GetTranslationMemoriesQueryFilters()
		{
			TranslationMemoryEntityQueryFilters translationMemoriesQueryFilters = null;// Service.GetTranslationMemoriesQueryFilters();
			ClientObjectBuilder builder = new ClientObjectBuilder(this);
			return new TranslationMemoryQueryFilters
			{
				Containers = translationMemoriesQueryFilters.Containers.Select((ContainerEntity c) => TranslationMemoryContainer.BuildTranslationMemoryContainer(builder, c)).ToArray(),
				FieldTemplates = translationMemoriesQueryFilters.FieldTemplates.Select((FieldGroupTemplateEntity ft) => ServerBasedFieldsTemplate.BuildServerBasedFieldsTemplate(builder, ft)).ToArray(),
				IsMain = translationMemoriesQueryFilters.IsMain,
				IsProject = translationMemoriesQueryFilters.IsProject,
				LanguageResourceTemplates = translationMemoriesQueryFilters.LanguageResourceTemplates.Select((LanguageResourceTemplateEntity lrt) => ServerBasedLanguageResourcesTemplate.BuildServerBasedLanguageResourcesTemplate(builder, lrt)).ToArray(),
				SourceLanguageCodes = translationMemoriesQueryFilters.SourceLanguageCodes,
				TargetLanguageCodes = translationMemoriesQueryFilters.TargetLanguageCodes
			};
		}

		public PagedTranslationMemories GetTranslationMemoriesByQuery(TranslationMemoryQuery query)
		{
			bool includeLanguageResourceData;
			bool includeScheduledOperations;
			string[] translationMemoryRelationships = GetTranslationMemoryRelationships(query.Properties, out includeLanguageResourceData, out includeScheduledOperations);
			TranslationMemoryEntityQuery query2 = new TranslationMemoryEntityQuery
			{
				Relationships = translationMemoryRelationships,
				IncludeLanguageResourceData = includeLanguageResourceData,
				IncludeScheduledOperations = includeScheduledOperations,
				ContainerNames = query.ContainerNames,
				Index = query.Index,
				IsMain = query.IsMain,
				IsProject = query.IsProject,
				LanguageResourceIds = query.LanguageResourceIds,
				ResourceGroupPath = query.ResourceGroupPath,
				OwnerId = query.OwnerId,
				IncludeChildResourceGroups = query.IncludeChildResourceGroups,
				Size = query.Size,
				SourceLanguageCodes = query.SourceLanguageCodes,
				TargetLanguageCodes = query.TargetLanguageCodes,
				FieldTemplateIds = query.FieldTemplateIds,
				Text = query.Text
			};
			ClientObjectBuilder builder = new ClientObjectBuilder(this);
			PagedTranslationMemoryEntities translationMemoriesByQuery = null;// Service.GetTranslationMemoriesByQuery(query2);
			return new PagedTranslationMemories
			{
				Index = translationMemoriesByQuery.Index,
				Size = translationMemoriesByQuery.Size,
				TotalEntities = translationMemoriesByQuery.TotalEntities,
				TranslationMemories = translationMemoriesByQuery.TranslationMemoryEntities.Select((TranslationMemoryEntity tme) => ServerBasedTranslationMemory.BuildServerBasedTranslationMemory(builder, tme)).ToArray()
			};
		}

		public ReadOnlyCollection<ServerBasedTranslationMemory> GetTranslationMemories(TranslationMemoryProperties additionalProperties)
		{
			bool includeLanguageResourceData;
			bool includeScheduledOperations;
			string[] translationMemoryRelationships = GetTranslationMemoryRelationships(additionalProperties, out includeLanguageResourceData, out includeScheduledOperations);
			TranslationMemoryEntity[] translationMemories = null;// Service.GetTranslationMemories(translationMemoryRelationships, includeLanguageResourceData, includeScheduledOperations);
			List<ServerBasedTranslationMemory> list = new List<ServerBasedTranslationMemory>();
			ClientObjectBuilder builder = new ClientObjectBuilder(this);
			TranslationMemoryEntity[] array = translationMemories;
			foreach (TranslationMemoryEntity entity in array)
			{
				list.Add(ServerBasedTranslationMemory.BuildServerBasedTranslationMemory(builder, entity));
			}
			return list.AsReadOnly();
		}

		public ReadOnlyCollection<DatabaseServer> GetDatabaseServers(DatabaseServerProperties additionalProperties)
		{
			DatabaseServerEntity[] databaseServers = null;//Service.GetDatabaseServers(GetDatabaseServerRelationships(additionalProperties));
            List<DatabaseServer> list = new List<DatabaseServer>(databaseServers.Length);
			ClientObjectBuilder builder = new ClientObjectBuilder(this);
			DatabaseServerEntity[] array = databaseServers;
			foreach (DatabaseServerEntity entity in array)
			{
				list.Add(DatabaseServer.BuildDatabaseServer(builder, entity));
			}
			return list.AsReadOnly();
		}

		public ReadOnlyCollection<TranslationMemoryContainer> GetContainers(ContainerProperties additionalProperties)
		{
			string[] containerRelationships = GetContainerRelationships(additionalProperties);
			ContainerEntity[] containers = null;//Service.GetContainers(containerRelationships);
            ClientObjectBuilder builder = new ClientObjectBuilder(this);
			List<TranslationMemoryContainer> list = new List<TranslationMemoryContainer>();
			ContainerEntity[] array = containers;
			foreach (ContainerEntity entity in array)
			{
				list.Add(TranslationMemoryContainer.BuildTranslationMemoryContainer(builder, entity));
			}
			return list.AsReadOnly();
		}

		public ReadOnlyCollection<ServerBasedFieldsTemplate> GetFieldsTemplates(FieldsTemplateProperties additionalProperties, bool includeTmSpecific = true)
		{
			FieldGroupTemplateEntity[] array = null;/*(from template in Service.GetFieldGroupTemplates(GetFieldGroupTemplateRelationships(additionalProperties))
				where !template.IsTmSpecific.GetValueOrDefault() | includeTmSpecific
				select template).ToArray();*/
			List<ServerBasedFieldsTemplate> list = new List<ServerBasedFieldsTemplate>(array.Length);
			ClientObjectBuilder builder = new ClientObjectBuilder(this);
			FieldGroupTemplateEntity[] array2 = array;
			foreach (FieldGroupTemplateEntity entity in array2)
			{
				list.Add(ServerBasedFieldsTemplate.BuildServerBasedFieldsTemplate(builder, entity));
			}
			return list.AsReadOnly();
		}

		public ReadOnlyCollection<ServerBasedLanguageResourcesTemplate> GetLanguageResourcesTemplates(LanguageResourcesTemplateProperties additionalProperties, bool includeTmSpecific = true)
		{
			LanguageResourceTemplateEntity[] array = null;/*(from template in Service.GetLanguageResourceTemplates(GetLanguageResourceTemplateRelationships(additionalProperties), IncludesLanguageResourceData(additionalProperties))
            where !template.IsTmSpecific.GetValueOrDefault() | includeTmSpecific
				select template).ToArray();*/
			List<ServerBasedLanguageResourcesTemplate> list = new List<ServerBasedLanguageResourcesTemplate>(array.Length);
			ClientObjectBuilder builder = new ClientObjectBuilder(this);
			LanguageResourceTemplateEntity[] array2 = array;
			foreach (LanguageResourceTemplateEntity entity in array2)
			{
				list.Add(ServerBasedLanguageResourcesTemplate.BuildServerBasedLanguageResourcesTemplate(builder, entity));
			}
			return list.AsReadOnly();
		}

		private static string[] GetDatabaseServerRelationships(DatabaseServerProperties properties)
		{
			List<string> list = new List<string>(2);
			if ((properties & DatabaseServerProperties.Containers) == DatabaseServerProperties.Containers)
			{
				list.Add("Containers");
			}
			if ((properties & DatabaseServerProperties.ContainerTranslationMemories) == DatabaseServerProperties.ContainerTranslationMemories)
			{
				list.Add("Containers.TranslationMemories.FieldGroupTemplate");
				list.Add("Containers.TranslationMemories.LanguageResourceTemplate");
			}
			return list.ToArray();
		}

		private static string[] GetContainerRelationships(ContainerProperties properties)
		{
			List<string> list = new List<string>(3);
			if ((properties & ContainerProperties.DatabaseServer) == ContainerProperties.DatabaseServer)
			{
				list.Add("DatabaseServer");
			}
			if ((properties & ContainerProperties.TranslationMemories) == ContainerProperties.TranslationMemories)
			{
				list.Add("TranslationMemories.FieldGroupTemplate");
				list.Add("TranslationMemories.LanguageResourceTemplate");
			}
			return list.ToArray();
		}

		internal static string[] GetTranslationMemoryRelationships(TranslationMemoryProperties properties, out bool includeLanguageResourceData, out bool includeScheduledOperations)
		{
			return GetTranslationMemoryItems(properties, includeTemplates: true, out includeLanguageResourceData, out includeScheduledOperations);
		}

		internal static string[] GetTranslationMemoryRelationshipsByProperty(TranslationMemoryProperties properties, bool includeTemplates, out bool includeLanguageResourceData, out bool includeScheduledOperations)
		{
			return GetTranslationMemoryItems(properties, includeTemplates, out includeLanguageResourceData, out includeScheduledOperations);
		}

		internal static string[] GetDefaultTranslationMemoryRelationships()
		{
			return new string[2]
			{
				"FieldGroupTemplate",
				"LanguageResourceTemplate"
			};
		}

		private static string[] GetTranslationMemoryItems(TranslationMemoryProperties properties, bool includeTemplates, out bool includeLanguageResourceData, out bool includeScheduledOperations)
		{
			List<string> list = new List<string>();
			includeLanguageResourceData = false;
			includeScheduledOperations = false;
			if (includeTemplates)
			{
				list.Add("FieldGroupTemplate");
				list.Add("LanguageResourceTemplate");
			}
			if (includeTemplates && (properties & TranslationMemoryProperties.Fields) == TranslationMemoryProperties.Fields)
			{
				list.Add("FieldGroupTemplate.Fields.PickListItems");
			}
			if (includeTemplates && (properties & TranslationMemoryProperties.LanguageResources) == TranslationMemoryProperties.LanguageResources)
			{
				list.Add("LanguageResourceTemplate.LanguageResources");
			}
			if (includeTemplates && (properties & TranslationMemoryProperties.LanguageResourceData) == TranslationMemoryProperties.LanguageResourceData)
			{
				list.Add("LanguageResourceTemplate.LanguageResources");
				includeLanguageResourceData = true;
			}
			if ((properties & TranslationMemoryProperties.Container) == TranslationMemoryProperties.Container)
			{
				list.Add("Container");
			}
			if ((properties & TranslationMemoryProperties.LanguageDirections) == TranslationMemoryProperties.LanguageDirections)
			{
				includeScheduledOperations = true;
				list.Add("LanguageDirections");
				list.Add("LanguageDirections.Imports");
				list.Add("LanguageDirections.Exports");
			}
			if ((properties & TranslationMemoryProperties.ScheduledOperations) == TranslationMemoryProperties.ScheduledOperations)
			{
				includeScheduledOperations = true;
				if (!list.Contains("LanguageDirections.Imports"))
				{
					list.Add("LanguageDirections.Imports");
				}
				if (!list.Contains("LanguageDirections.Exports"))
				{
					list.Add("LanguageDirections.Exports");
				}
			}
			return list.ToArray();
		}

		private static string[] GetFieldGroupTemplateRelationships(FieldsTemplateProperties properties)
		{
			List<string> list = new List<string>(3);
			if ((properties & FieldsTemplateProperties.Fields) == FieldsTemplateProperties.Fields)
			{
				list.Add("Fields.PickListItems");
			}
			if ((properties & FieldsTemplateProperties.TranslationMemories) == FieldsTemplateProperties.TranslationMemories)
			{
				list.Add("TranslationMemories.FieldGroupTemplate");
				list.Add("TranslationMemories.LanguageResourceTemplate");
			}
			return list.ToArray();
		}

		private static string[] GetLanguageResourceTemplateRelationships(LanguageResourcesTemplateProperties properties)
		{
			List<string> list = new List<string>(3);
			if ((properties & LanguageResourcesTemplateProperties.LanguageResources) == LanguageResourcesTemplateProperties.LanguageResources || (properties & LanguageResourcesTemplateProperties.LanguageResourcesData) == LanguageResourcesTemplateProperties.LanguageResourcesData)
			{
				list.Add("LanguageResources");
			}
			if ((properties & LanguageResourcesTemplateProperties.TranslationMemories) == LanguageResourcesTemplateProperties.TranslationMemories)
			{
				list.Add("TranslationMemories.FieldGroupTemplate");
				list.Add("TranslationMemories.LanguageResourceTemplate");
			}
			return list.ToArray();
		}

		private static bool IncludesLanguageResourceData(LanguageResourcesTemplateProperties properties)
		{
			return (properties & LanguageResourcesTemplateProperties.LanguageResourcesData) == LanguageResourcesTemplateProperties.LanguageResourcesData;
		}

		public ScheduledServerTranslationMemoryImport GetScheduledTranslationMemoryImport(Guid importId)
		{
			ImportEntity translationMemoryImportByWorkItemUniqueId = null;//Service.GetTranslationMemoryImportByWorkItemUniqueId(importId);
            if (translationMemoryImportByWorkItemUniqueId == null)
			{
				throw new InvalidOperationException(StringResources.MessageImportNotFound);
			}
			return new ScheduledServerTranslationMemoryImport(this, translationMemoryImportByWorkItemUniqueId);
		}

		public ScheduledServerTranslationMemoryExport GetScheduledTranslationMemoryExport(Guid exportId)
		{
			ExportEntity translationMemoryExportByWorkItemUniqueId = null;//Service.GetTranslationMemoryExportByWorkItemUniqueId(exportId);
            if (translationMemoryExportByWorkItemUniqueId == null)
			{
				throw new InvalidOperationException(StringResources.MessageExportNotFound);
			}
			return new ScheduledServerTranslationMemoryExport(this, translationMemoryExportByWorkItemUniqueId);
		}

		public void DeleteBackgroundTask(Guid taskId)
		{
			DeleteBackgroundTasks(new Guid[1]
			{
				taskId
			});
		}

		public void DeleteBackgroundTasks(ICollection<Guid> tasksIdentities)
		{
			if (tasksIdentities == null)
			{
				throw new ArgumentNullException("tasks");
			}
			if (tasksIdentities.Count > 0)
			{
				//Service.DeleteTranslationWorkItemsByUniqueIds(tasksIdentities.ToArray());
			}
		}

		public ServerBasedTranslationMemoryVersion GetServerVersion()
		{
			return ServerBasedTranslationMemoryVersion.Server2014SP2;//(ServerBasedTranslationMemoryVersion)Service.ServerVersion;
        }
	}
}
