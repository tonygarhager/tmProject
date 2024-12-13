using Sdl.Enterprise2.Studio.Platform.Client.IdentityModel;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Client;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Security;
using System.Security.Principal;
using System.Threading;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Represents server-provided access to translation memories and provides related administrative
	/// and maintenance services. 
	/// </summary>
	public class CloudTranslationProvider
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
		/// Initializes a new instance of the <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.CloudTranslationProvider" /> class.
		/// </summary>
		/// <remarks>This is for in-line server side usage and assumes thet the SDL identity has been propagated to the calling Thread.</remarks>
		/// <exception cref="T:System.Security.SecurityException">Thrown if the current thread's principal is invalid.</exception>
		public CloudTranslationProvider()
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
		/// <param name="serverUri"></param>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <exception cref="T:System.Security.SecurityException"></exception>
		public CloudTranslationProvider(Uri serverUri, string username, string password)
		{
			Uri = serverUri;
			Service = new TranslationMemoryAdministrationClient(serverUri.ToString(), username, password, RestClientVersion.Cloud);
		}

		/// <summary>
		/// Creates a new translation provider server.
		/// </summary>
		/// <param name="serverUri">The URI of the server. This is of the form http://servername:port.</param>
		/// <param name="credentials">The user credentials.</param>
		/// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="serverUri" /> or <paramref name="credentials" /> are <c>null</c>.</exception>
		internal CloudTranslationProvider(Uri serverUri, UserCredentials credentials)
		{
			if (serverUri == null)
			{
				throw new ArgumentNullException("serverUri");
			}
			if (credentials == null)
			{
				throw new ArgumentException("credentials");
			}
			Uri = serverUri;
			Credentials = credentials;
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
		public CloudTranslationProvider(Uri serverUri, bool useWindowsAuthentication, string userName, string password)
		{
			if (serverUri == null)
			{
				throw new ArgumentNullException("serverUri");
			}
			Uri = serverUri;
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
			return null;
		}

		/// <summary>
		/// Gets the translation memory with the corresponding unique id.
		/// </summary>
		/// <param name="id">The translation memory's unique id.</param>
		/// <param name="additionalProperties">The additional related objects to retrieve.</param>
		/// <returns>The translation memory, or null if no translation memory with given id exists.</returns>
		/// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="id" /> is empty.</exception>
		public CloudBasedTranslationMemory GetTranslationMemory(Guid id, TranslationMemoryProperties additionalProperties)
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
			return CloudBasedTranslationMemory.BuildCloudBasedTranslationMemory(new ClientObjectBuilder<CloudTranslationProvider>(this), translationMemoryEntity);
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
			return null;
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
			return null;
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
			return null;
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
			return null;
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
			RecognizerFactory recognizerFactory = new RecognizerFactory();
			LanguageResourceBundle languageResourceBundle = new LanguageResourceBundle(languageCode);
			languageResourceBundle.Abbreviations = recognizerFactory.GetAbbreviations(defaultLanguageResources);
			languageResourceBundle.Variables = recognizerFactory.GetVariables(defaultLanguageResources);
			languageResourceBundle.OrdinalFollowers = recognizerFactory.GetOrdinalFollowers(defaultLanguageResources);
			languageResourceBundle.SegmentationRules = recognizerFactory.GetSegmentationRules(languageCode, defaultLanguageResources);
			return languageResourceBundle;
		}

		/// <summary>
		/// Returns a collection of all the server-based translation memories on this server.
		/// </summary>
		/// <param name="additionalProperties">The additional related objects to retrieve.</param>
		/// <returns>A read-only collection of server-based translation memories.</returns>
		public ReadOnlyCollection<CloudBasedTranslationMemory> GetTranslationMemories(TranslationMemoryProperties additionalProperties)
		{
			bool includeLanguageResourceData;
			bool includeScheduledOperations;
			string[] translationMemoryRelationships = GetTranslationMemoryRelationships(additionalProperties, out includeLanguageResourceData, out includeScheduledOperations);
			TranslationMemoryEntity[] translationMemories = Service.GetTranslationMemories(translationMemoryRelationships, includeLanguageResourceData, includeScheduledOperations);
			List<CloudBasedTranslationMemory> list = new List<CloudBasedTranslationMemory>();
			ClientObjectBuilder<CloudTranslationProvider> builder = new ClientObjectBuilder<CloudTranslationProvider>(this);
			TranslationMemoryEntity[] array = translationMemories;
			foreach (TranslationMemoryEntity entity in array)
			{
				list.Add(CloudBasedTranslationMemory.BuildCloudBasedTranslationMemory(builder, entity));
			}
			return list.AsReadOnly();
		}

		internal static string[] GetTranslationMemoryRelationships(TranslationMemoryProperties properties, out bool includeLanguageResourceData, out bool includeScheduledOperations)
		{
			List<string> list = new List<string>(3);
			includeLanguageResourceData = false;
			includeScheduledOperations = false;
			list.Add("FieldGroupTemplate");
			list.Add("LanguageResourceTemplate");
			if ((properties & TranslationMemoryProperties.Fields) == TranslationMemoryProperties.Fields)
			{
				list.Add("FieldGroupTemplate.Fields.PickListItems");
			}
			if ((properties & TranslationMemoryProperties.LanguageResources) == TranslationMemoryProperties.LanguageResources)
			{
				list.Add("LanguageResourceTemplate.LanguageResources");
			}
			if ((properties & TranslationMemoryProperties.LanguageResourceData) == TranslationMemoryProperties.LanguageResourceData)
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
				if (!includeScheduledOperations)
				{
					includeScheduledOperations = true;
				}
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

		internal static string[] GetDefaultTranslationMemoryRelationships()
		{
			return new string[2]
			{
				"FieldGroupTemplate",
				"LanguageResourceTemplate"
			};
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
	}
}
