using Sdl.Core.Api.DataAccess;
using Sdl.Enterprise2.Studio.Platform.Client.IdentityModel;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Client;
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
	/// <summary>
	/// Represents server-provided access to translation memories and provides related administrative
	/// and maintenance services. 
	/// </summary>
	public class TranslationProviderServer
	{
		private static readonly Uri DefaultUri = new Uri("http://localhost");

		/// <summary>
		/// Gets the URI that was used to connect to this server.
		/// </summary>
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

		/// <summary>
		/// If the  server version is OnPremiseRest/Cloud, returns a flag to indicate if the translation memory location is supported
		/// </summary>
		public bool IsTranslationMemoryLocationSupported
		{
			get
			{
				ServerBasedTranslationMemoryVersion serverVersion = GetServerVersion();
				if ((uint)(serverVersion - 5) <= 1u)
				{
					return Service.SupportsTranslationMemoryLocation;
				}
				return false;
			}
		}

		/// <summary>
		/// Check if the server has Translation and Analysis Service support.
		/// </summary>
		public bool IsTranslationAndAnalysisServiceSupported
		{
			get
			{
				ServerBasedTranslationMemoryVersion serverVersion = GetServerVersion();
				if (serverVersion == ServerBasedTranslationMemoryVersion.OnPremiseRest)
				{
					return Service.SupportsTranslationAndAnalysisService;
				}
				return false;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.TranslationProviderServer" /> class.
		/// </summary>
		/// <remarks>This is for in-line server side usage and assumes thet the SDL identity has been propagated to the calling Thread.</remarks>
		/// <exception cref="T:System.Security.SecurityException">Thrown if the current thread's principal is invalid.</exception>
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

		/// <summary>
		/// Creates a new translation provider server.
		/// </summary>
		/// <param name="serverUri">The URI of the server. This is of the form http://servername:port.</param>
		/// <param name="credentials">The user credentials.</param>
		/// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="serverUri" /> or <paramref name="credentials" /> are <c>null</c>.</exception>
		internal TranslationProviderServer(Uri serverUri, UserCredentials credentials)
		{
			Uri = (serverUri ?? throw new ArgumentNullException("serverUri"));
			Credentials = (credentials ?? throw new ArgumentException("credentials"));
			Service = new TranslationMemoryAdministrationClient(serverUri.ToString(), credentials);
		}

		/// <summary>
		/// Creates a new translation provider server.
		/// </summary>
		/// <param name="serverUri">The URI of the server. This is of the form http://servername:port.</param>
		/// <param name="useWindowsAuthentication">Whether to use Windows authentication. When set to <code>false</code>, <paramref name="userName" />
		/// and <paramref name="password" /> have to be specified. When set to <code>true</code>, either pass <code>null</code> for <paramref name="userName" />
		/// and <paramref name="password" /> in order to log on as the currently logged on Windows user, or set <paramref name="userName" />
		/// to a domain-qualified Windows user name and <paramref name="password" /> to the matching password to log on using basic Windows authentication.</param>
		/// <param name="userName">When using custom authentication or basic Windows authentication, the user name of the user. Pass <code>null</code>
		/// to use integrated Windows authentication(see <paramref name="useWindowsAuthentication" />).</param>
		/// <param name="password">When using custom authentication or basic Windows authentication the password of the user. Pass <code>null</code>
		/// to use integrated Windows authentication (see <paramref name="useWindowsAuthentication" />).</param>
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
			Service = new TranslationMemoryAdministrationClient(serverUri.ToString(), userCredentials);
		}

		/// <summary>
		/// Creates a new translation provider server.
		/// </summary>
		/// <param name="serverUri">The URI of the server. This is of the form http://servername:port.</param>
		/// <param name="userName"></param>
		/// <param name="authToken"></param>
		/// <param name="expirationDate"></param>
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
			Service = new TranslationMemoryAdministrationClient(serverUri.ToString(), Credentials);
		}

		/// <summary>
		/// Obtains a representation of an existing database server.
		/// </summary>
		/// <param name="id">The database server id.</param>
		/// <param name="additionalProperties">The additional related objects to retrieve.</param>
		/// <returns>
		/// The database server, or null if no database server with the given id exists.
		/// </returns>
		/// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="id" /> is empty.</exception>
		public DatabaseServer GetDatabaseServer(Guid id, DatabaseServerProperties additionalProperties)
		{
			if (id == Guid.Empty)
			{
				throw new ArgumentNullException("id");
			}
			DatabaseServerEntity databaseServerByUniqueId = Service.GetDatabaseServerByUniqueId(id, GetDatabaseServerRelationships(additionalProperties));
			if (databaseServerByUniqueId == null)
			{
				return null;
			}
			ClientObjectBuilder builder = new ClientObjectBuilder(this);
			return DatabaseServer.BuildDatabaseServer(builder, databaseServerByUniqueId);
		}

		/// <summary>
		/// Obtains a representation of an existing database server.
		/// </summary>
		/// <param name="path">The path of the database server (note that this is not the physical server name).</param>
		/// <param name="additionalProperties">The additional related objects to retrieve.</param>
		/// <returns> The database server, or null if no database server with the given id exists. </returns>
		/// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="path" /> is null or empty.</exception>
		public DatabaseServer GetDatabaseServer(string path, DatabaseServerProperties additionalProperties)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path");
			}
			DatabaseServerEntity databaseServerByPath = Service.GetDatabaseServerByPath(path, GetDatabaseServerRelationships(additionalProperties));
			if (databaseServerByPath == null)
			{
				return null;
			}
			ClientObjectBuilder builder = new ClientObjectBuilder(this);
			return DatabaseServer.BuildDatabaseServer(builder, databaseServerByPath);
		}

		/// <summary>
		/// Gets the <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.TranslationMemoryContainer" /> with the specified ID, or null
		/// if the container does not exist.
		/// </summary>
		/// <param name="id">The container id.</param>
		/// <param name="additionalProperties">The additional related objects to retrieve.</param>
		/// <returns>The container, or null if no container with given id exists.</returns>
		/// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="id" /> is empty.</exception>
		public TranslationMemoryContainer GetContainer(Guid id, ContainerProperties additionalProperties)
		{
			if (id == Guid.Empty)
			{
				throw new ArgumentNullException("id");
			}
			ContainerEntity containerByUniqueId = Service.GetContainerByUniqueId(id, GetContainerRelationships(additionalProperties));
			if (containerByUniqueId == null)
			{
				return null;
			}
			ClientObjectBuilder builder = new ClientObjectBuilder(this);
			return TranslationMemoryContainer.BuildTranslationMemoryContainer(builder, containerByUniqueId);
		}

		/// <summary>
		/// Gets the license status information for this translation provider server.
		/// </summary>
		/// <returns>A license status information object.</returns>
		public LicensingStatusInformation GetLicensingStatusInformation()
		{
			LicensingRestrictionsEntity licensingRestrictions = Service.GetLicensingRestrictions();
			return new LicensingStatusInformation(licensingRestrictions);
		}

		/// <summary>
		/// Gets the <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.TranslationMemoryContainer" /> with the specified path, or null
		/// if the container does not exist.
		/// </summary>
		/// <param name="path">The path of the container</param>
		/// <param name="additionalProperties">The additional related objects to retrieve.</param>
		/// <returns> The container, or null if no container with given path exists. </returns>
		/// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="path" /> is null or empty.</exception>
		public TranslationMemoryContainer GetContainer(string path, ContainerProperties additionalProperties)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path");
			}
			ContainerEntity containerByPath = Service.GetContainerByPath(path, GetContainerRelationships(additionalProperties));
			if (containerByPath == null)
			{
				return null;
			}
			ClientObjectBuilder builder = new ClientObjectBuilder(this);
			return TranslationMemoryContainer.BuildTranslationMemoryContainer(builder, containerByPath);
		}

		/// <summary>
		/// Gets the translation memory with the corresponding unique path.
		/// Gets a translation memory through its path.
		/// </summary>
		/// <param name="path">The translation memory's name.</param>
		/// <param name="additionalProperties">The additional related objects to retrieve.</param>
		/// <returns> The translation memory, or null if no translation memory with given path exists. </returns>
		/// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="path" /> is null or empty.</exception>
		public ServerBasedTranslationMemory GetTranslationMemory(string path, TranslationMemoryProperties additionalProperties)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path");
			}
			bool includeLanguageResourceData;
			bool includeScheduledOperations;
			string[] translationMemoryRelationships = GetTranslationMemoryRelationships(additionalProperties, out includeLanguageResourceData, out includeScheduledOperations);
			TranslationMemoryEntity translationMemoryByPath = Service.GetTranslationMemoryByPath(path, translationMemoryRelationships, includeLanguageResourceData, includeScheduledOperations);
			if (!(translationMemoryByPath != null))
			{
				return null;
			}
			return ServerBasedTranslationMemory.BuildServerBasedTranslationMemory(new ClientObjectBuilder(this), translationMemoryByPath);
		}

		/// <summary>
		/// Gets the translation memory with the corresponding unique id.
		/// </summary>
		/// <param name="id">The translation memory's unique id.</param>
		/// <param name="additionalProperties">The additional related objects to retrieve.</param>
		/// <returns>The translation memory, or null if no translation memory with given id exists.</returns>
		/// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="id" /> is empty.</exception>
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
				translationMemoryEntity = Service.GetTranslationMemoryByUniqueId(id, translationMemoryRelationships, includeLanguageResourceData, includeScheduledOperations);
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

		/// <summary>
		/// Gets the translation memory summary with the corresponding unique path.
		/// Gets a translation memory summary through its path.
		/// No template information is provided.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="additionalProperties"></param>
		/// <returns></returns>
		/// <exception cref="T:System.ArgumentNullException"></exception>
		public ServerBasedTranslationMemory GetTranslationMemorySummary(string path, TranslationMemoryProperties additionalProperties)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path");
			}
			bool flag = Service.ServerVersion == TMServerVersion.OnPremiseRest || Service.ServerVersion == TMServerVersion.Cloud;
			bool includeTemplates = !flag;
			bool includeLanguageResourceData;
			bool includeScheduledOperations;
			string[] translationMemoryRelationshipsByProperty = GetTranslationMemoryRelationshipsByProperty(additionalProperties, includeTemplates, out includeLanguageResourceData, out includeScheduledOperations);
			TranslationMemoryEntity translationMemoryByPath = Service.GetTranslationMemoryByPath(path, translationMemoryRelationshipsByProperty, includeLanguageResourceData, includeScheduledOperations);
			if (!(translationMemoryByPath != null))
			{
				return null;
			}
			return ServerBasedTranslationMemory.BuildServerBasedTranslationMemory(new ClientObjectBuilder(this), translationMemoryByPath);
		}

		/// <summary>
		/// Gets the fields template with the specified <paramref name="id" />, 
		/// or null if no such fields template exists.
		/// </summary>
		/// <param name="id">The fields template ID.</param>
		/// <param name="additionalProperties">The additional related objects to retrieve.</param>
		/// <returns>The fields template, or null if no fields template with given id exists.</returns>
		/// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="id" /> is empty.</exception>
		public ServerBasedFieldsTemplate GetFieldsTemplate(Guid id, FieldsTemplateProperties additionalProperties)
		{
			if (id == Guid.Empty)
			{
				throw new ArgumentNullException("id");
			}
			FieldGroupTemplateEntity fieldGroupTemplateByUniqueId = Service.GetFieldGroupTemplateByUniqueId(id, GetFieldGroupTemplateRelationships(additionalProperties));
			if (!(fieldGroupTemplateByUniqueId != null))
			{
				return null;
			}
			return ServerBasedFieldsTemplate.BuildServerBasedFieldsTemplate(new ClientObjectBuilder(this), fieldGroupTemplateByUniqueId);
		}

		/// <summary>
		/// Gets the fields template with the specified <paramref name="path" />,
		/// or null if no such fields template exists.
		/// </summary>
		/// <param name="path">The fields template path.</param>
		/// <param name="additionalProperties">The additional related objects to retrieve.</param>
		/// <returns> The fields template, or null if no fields template with given id exists. </returns>
		/// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="path" /> is null or empty.</exception>
		public ServerBasedFieldsTemplate GetFieldsTemplate(string path, FieldsTemplateProperties additionalProperties)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path");
			}
			FieldGroupTemplateEntity fieldGroupTemplateByPath = Service.GetFieldGroupTemplateByPath(path, GetFieldGroupTemplateRelationships(additionalProperties));
			if (!(fieldGroupTemplateByPath != null))
			{
				return null;
			}
			return ServerBasedFieldsTemplate.BuildServerBasedFieldsTemplate(new ClientObjectBuilder(this), fieldGroupTemplateByPath);
		}

		/// <summary>
		/// Gets the LanguageResourcesTemplate with the specified ID.
		/// </summary>
		/// <param name="id">The language resources template ID.</param>
		/// <param name="additionalProperties">The additional related objects to retrieve.</param>
		/// <returns>The language resource group template, or <c>null</c> if it does not exist.</returns>
		/// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="id" /> is empty.</exception>
		public ServerBasedLanguageResourcesTemplate GetLanguageResourcesTemplate(Guid id, LanguageResourcesTemplateProperties additionalProperties)
		{
			if (id == Guid.Empty)
			{
				throw new ArgumentNullException("id");
			}
			LanguageResourceTemplateEntity languageResourceTemplateByUniqueId = Service.GetLanguageResourceTemplateByUniqueId(id, GetLanguageResourceTemplateRelationships(additionalProperties), IncludesLanguageResourceData(additionalProperties));
			if (!(languageResourceTemplateByUniqueId != null))
			{
				return null;
			}
			return new ServerBasedLanguageResourcesTemplate(this, languageResourceTemplateByUniqueId);
		}

		/// <summary>
		/// Gets the language resources template with the specified path.
		/// </summary>
		/// <param name="path">The language resources template path.</param>
		/// <param name="additionalProperties">The additional related objects to retrieve.</param>
		/// <returns> The language resource group template, or <c>null</c> if it does not exist. </returns>
		/// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="path" /> is null or empty.</exception>
		public ServerBasedLanguageResourcesTemplate GetLanguageResourcesTemplate(string path, LanguageResourcesTemplateProperties additionalProperties)
		{
			LanguageResourceTemplateEntity languageResourceTemplateByPath = Service.GetLanguageResourceTemplateByPath(path, GetLanguageResourceTemplateRelationships(additionalProperties), IncludesLanguageResourceData(additionalProperties));
			if (!(languageResourceTemplateByPath != null))
			{
				return null;
			}
			return ServerBasedLanguageResourcesTemplate.BuildServerBasedLanguageResourcesTemplate(new ClientObjectBuilder(this), languageResourceTemplateByPath);
		}

		/// <summary>
		/// Gets default language resources for the specified language from the server.
		/// </summary>
		/// <param name="language">The language for which to get the default language resources.</param>
		/// <returns>A <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.LanguageResourceBundle" /> instance, populated with the default language resources. Note that
		/// some default language resources might be null.</returns>
		/// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="language" /> is null.</exception>
		public LanguageResourceBundle GetDefaultLanguageResources(CultureInfo language)
		{
			if (language == null)
			{
				throw new ArgumentNullException("language");
			}
			return GetDefaultLanguageResources(language.Name);
		}

		/// <summary>
		/// Gets default language resources for the specified language code from the server.
		/// </summary>
		/// <param name="languageCode">The language code for which to get the default language resources.</param>
		/// <returns>A <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.LanguageResourceBundle" /> instance, populated with the default language resources. Note that
		/// some default language resources might be null.</returns>
		/// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="languageCode" /> is null or empty.</exception>
		public LanguageResourceBundle GetDefaultLanguageResources(string languageCode)
		{
			if (string.IsNullOrEmpty(languageCode))
			{
				throw new ArgumentNullException("languageCode");
			}
			LanguageResourceEntity[] defaultLanguageResources = Service.GetDefaultLanguageResources(languageCode);
			EntityCollection<LanguageResourceEntity> entities = new EntityCollection<LanguageResourceEntity>(defaultLanguageResources);
			return new LanguageResourceBundle(languageCode, entities);
		}

		/// <summary>
		/// Gets all the available filters to be queried for translation memories.
		/// </summary>
		/// <returns></returns>
		public TranslationMemoryQueryFilters GetTranslationMemoriesQueryFilters()
		{
			TranslationMemoryEntityQueryFilters translationMemoriesQueryFilters = Service.GetTranslationMemoriesQueryFilters();
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

		/// <summary>
		///  Returns the paged collection of the server-based translation memories on this server.
		/// </summary>
		/// <returns>A read-only paged collection of server-based translation memories.</returns>
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
			PagedTranslationMemoryEntities translationMemoriesByQuery = Service.GetTranslationMemoriesByQuery(query2);
			return new PagedTranslationMemories
			{
				Index = translationMemoriesByQuery.Index,
				Size = translationMemoriesByQuery.Size,
				TotalEntities = translationMemoriesByQuery.TotalEntities,
				TranslationMemories = translationMemoriesByQuery.TranslationMemoryEntities.Select((TranslationMemoryEntity tme) => ServerBasedTranslationMemory.BuildServerBasedTranslationMemory(builder, tme)).ToArray()
			};
		}

		/// <summary>
		/// Returns a collection of all the server-based translation memories on this server.
		/// </summary>
		/// <param name="additionalProperties">The additional related objects to retrieve.</param>
		/// <returns>A read-only collection of server-based translation memories.</returns>
		public ReadOnlyCollection<ServerBasedTranslationMemory> GetTranslationMemories(TranslationMemoryProperties additionalProperties)
		{
			bool includeLanguageResourceData;
			bool includeScheduledOperations;
			string[] translationMemoryRelationships = GetTranslationMemoryRelationships(additionalProperties, out includeLanguageResourceData, out includeScheduledOperations);
			TranslationMemoryEntity[] translationMemories = Service.GetTranslationMemories(translationMemoryRelationships, includeLanguageResourceData, includeScheduledOperations);
			List<ServerBasedTranslationMemory> list = new List<ServerBasedTranslationMemory>();
			ClientObjectBuilder builder = new ClientObjectBuilder(this);
			TranslationMemoryEntity[] array = translationMemories;
			foreach (TranslationMemoryEntity entity in array)
			{
				list.Add(ServerBasedTranslationMemory.BuildServerBasedTranslationMemory(builder, entity));
			}
			return list.AsReadOnly();
		}

		/// <summary>
		/// Gets the database servers registered with this server for use as translation memory container hosts.
		/// </summary>
		/// <param name="additionalProperties">The additional related objects to retrieve.</param>
		/// <returns>A read-only collection of database servers registered with this server.</returns>
		public ReadOnlyCollection<DatabaseServer> GetDatabaseServers(DatabaseServerProperties additionalProperties)
		{
			DatabaseServerEntity[] databaseServers = Service.GetDatabaseServers(GetDatabaseServerRelationships(additionalProperties));
			List<DatabaseServer> list = new List<DatabaseServer>(databaseServers.Length);
			ClientObjectBuilder builder = new ClientObjectBuilder(this);
			DatabaseServerEntity[] array = databaseServers;
			foreach (DatabaseServerEntity entity in array)
			{
				list.Add(DatabaseServer.BuildDatabaseServer(builder, entity));
			}
			return list.AsReadOnly();
		}

		/// <summary>
		/// Gets all translation memory containers managed by this translation provider server.
		/// </summary>
		/// <param name="additionalProperties">The additional related objects to retrieve.</param>
		/// <returns>A read-only collection of translation memory containers.</returns>
		public ReadOnlyCollection<TranslationMemoryContainer> GetContainers(ContainerProperties additionalProperties)
		{
			string[] containerRelationships = GetContainerRelationships(additionalProperties);
			ContainerEntity[] containers = Service.GetContainers(containerRelationships);
			ClientObjectBuilder builder = new ClientObjectBuilder(this);
			List<TranslationMemoryContainer> list = new List<TranslationMemoryContainer>();
			ContainerEntity[] array = containers;
			foreach (ContainerEntity entity in array)
			{
				list.Add(TranslationMemoryContainer.BuildTranslationMemoryContainer(builder, entity));
			}
			return list.AsReadOnly();
		}

		/// <summary>
		/// Gets all the fields templates available on this server.
		/// </summary>
		/// <returns>A read-only collection of fields templates.</returns>
		public ReadOnlyCollection<ServerBasedFieldsTemplate> GetFieldsTemplates(FieldsTemplateProperties additionalProperties, bool includeTmSpecific = true)
		{
			FieldGroupTemplateEntity[] array = (from template in Service.GetFieldGroupTemplates(GetFieldGroupTemplateRelationships(additionalProperties))
				where !template.IsTmSpecific.GetValueOrDefault() | includeTmSpecific
				select template).ToArray();
			List<ServerBasedFieldsTemplate> list = new List<ServerBasedFieldsTemplate>(array.Length);
			ClientObjectBuilder builder = new ClientObjectBuilder(this);
			FieldGroupTemplateEntity[] array2 = array;
			foreach (FieldGroupTemplateEntity entity in array2)
			{
				list.Add(ServerBasedFieldsTemplate.BuildServerBasedFieldsTemplate(builder, entity));
			}
			return list.AsReadOnly();
		}

		/// <summary>
		/// Gets all the language resources templates available on this server.
		/// </summary>
		/// <returns>A read-only collection of language resources templates.</returns>
		public ReadOnlyCollection<ServerBasedLanguageResourcesTemplate> GetLanguageResourcesTemplates(LanguageResourcesTemplateProperties additionalProperties, bool includeTmSpecific = true)
		{
			LanguageResourceTemplateEntity[] array = (from template in Service.GetLanguageResourceTemplates(GetLanguageResourceTemplateRelationships(additionalProperties), IncludesLanguageResourceData(additionalProperties))
				where !template.IsTmSpecific.GetValueOrDefault() | includeTmSpecific
				select template).ToArray();
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

		/// <summary>
		/// Gets a scheduled translation memory import by import Id.
		/// </summary>
		public ScheduledServerTranslationMemoryImport GetScheduledTranslationMemoryImport(Guid importId)
		{
			ImportEntity translationMemoryImportByWorkItemUniqueId = Service.GetTranslationMemoryImportByWorkItemUniqueId(importId);
			if (translationMemoryImportByWorkItemUniqueId == null)
			{
				throw new InvalidOperationException(StringResources.MessageImportNotFound);
			}
			return new ScheduledServerTranslationMemoryImport(this, translationMemoryImportByWorkItemUniqueId);
		}

		/// <summary>
		/// Gets a scheduled translation memory export by export Id.
		/// </summary>
		public ScheduledServerTranslationMemoryExport GetScheduledTranslationMemoryExport(Guid exportId)
		{
			ExportEntity translationMemoryExportByWorkItemUniqueId = Service.GetTranslationMemoryExportByWorkItemUniqueId(exportId);
			if (translationMemoryExportByWorkItemUniqueId == null)
			{
				throw new InvalidOperationException(StringResources.MessageExportNotFound);
			}
			return new ScheduledServerTranslationMemoryExport(this, translationMemoryExportByWorkItemUniqueId);
		}

		/// <summary>
		/// Deletes a background task.
		/// </summary>
		/// <param name="taskId">The unique identifier for the task to be deleted.</param>
		public void DeleteBackgroundTask(Guid taskId)
		{
			DeleteBackgroundTasks(new Guid[1]
			{
				taskId
			});
		}

		/// <summary>
		/// Deletes a collection background tasks.
		/// </summary>
		/// <param name="tasksIdentities">The unique identifiers for the tasks to be deleted.</param>
		public void DeleteBackgroundTasks(ICollection<Guid> tasksIdentities)
		{
			if (tasksIdentities == null)
			{
				throw new ArgumentNullException("tasks");
			}
			if (tasksIdentities.Count > 0)
			{
				Service.DeleteTranslationWorkItemsByUniqueIds(tasksIdentities.ToArray());
			}
		}

		/// <summary>
		/// Returns the version of the server.
		/// </summary>
		/// <returns></returns>
		public ServerBasedTranslationMemoryVersion GetServerVersion()
		{
			return (ServerBasedTranslationMemoryVersion)Service.ServerVersion;
		}
	}
}
