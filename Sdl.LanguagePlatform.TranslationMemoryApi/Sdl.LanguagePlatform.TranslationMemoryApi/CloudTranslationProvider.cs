using Sdl.Enterprise2.Studio.Platform.Client.IdentityModel;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory;
//using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Client;
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
	public class CloudTranslationProvider
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

		public CloudTranslationProvider(Uri serverUri, string username, string password)
		{
			Uri = serverUri;
			Service = null;// new TranslationMemoryAdministrationClient(serverUri.ToString(), username, password, RestClientVersion.Cloud);
		}

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
			Service = null;// new TranslationMemoryAdministrationClient(serverUri.ToString(), credentials);
		}

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
			Service = null;// new TranslationMemoryAdministrationClient(serverUri.ToString(), userCredentials);
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
			return null;
		}

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
				translationMemoryEntity = null;// Service.GetTranslationMemoryByUniqueId(id, translationMemoryRelationships, includeLanguageResourceData, includeScheduledOperations);
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

		public ServerBasedFieldsTemplate GetFieldsTemplate(Guid id, FieldsTemplateProperties additionalProperties)
		{
			if (id == Guid.Empty)
			{
				throw new ArgumentNullException("id");
			}
			FieldGroupTemplateEntity fieldGroupTemplateByUniqueId = null;// Service.GetFieldGroupTemplateByUniqueId(id, GetFieldGroupTemplateRelationships(additionalProperties));
			return null;
		}

		public ServerBasedFieldsTemplate GetFieldsTemplate(string path, FieldsTemplateProperties additionalProperties)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path");
			}
			FieldGroupTemplateEntity fieldGroupTemplateByPath = null;// Service.GetFieldGroupTemplateByPath(path, GetFieldGroupTemplateRelationships(additionalProperties));
			return null;
		}

		public ServerBasedLanguageResourcesTemplate GetLanguageResourcesTemplate(Guid id, LanguageResourcesTemplateProperties additionalProperties)
		{
			if (id == Guid.Empty)
			{
				throw new ArgumentNullException("id");
			}
			LanguageResourceTemplateEntity languageResourceTemplateByUniqueId = null;// Service.GetLanguageResourceTemplateByUniqueId(id, GetLanguageResourceTemplateRelationships(additionalProperties), IncludesLanguageResourceData(additionalProperties));
			return null;
		}

		public ServerBasedLanguageResourcesTemplate GetLanguageResourcesTemplate(string path, LanguageResourcesTemplateProperties additionalProperties)
		{
			LanguageResourceTemplateEntity languageResourceTemplateByPath = null;// Service.GetLanguageResourceTemplateByPath(path, GetLanguageResourceTemplateRelationships(additionalProperties), IncludesLanguageResourceData(additionalProperties));
            return null;
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
            RecognizerFactory recognizerFactory = new RecognizerFactory();
			LanguageResourceBundle languageResourceBundle = new LanguageResourceBundle(languageCode);
			languageResourceBundle.Abbreviations = recognizerFactory.GetAbbreviations(defaultLanguageResources);
			languageResourceBundle.Variables = recognizerFactory.GetVariables(defaultLanguageResources);
			languageResourceBundle.OrdinalFollowers = recognizerFactory.GetOrdinalFollowers(defaultLanguageResources);
			languageResourceBundle.SegmentationRules = recognizerFactory.GetSegmentationRules(languageCode, defaultLanguageResources);
			return languageResourceBundle;
		}

		public ReadOnlyCollection<CloudBasedTranslationMemory> GetTranslationMemories(TranslationMemoryProperties additionalProperties)
		{
			bool includeLanguageResourceData;
			bool includeScheduledOperations;
			string[] translationMemoryRelationships = GetTranslationMemoryRelationships(additionalProperties, out includeLanguageResourceData, out includeScheduledOperations);
			TranslationMemoryEntity[] translationMemories = null;// Service.GetTranslationMemories(translationMemoryRelationships, includeLanguageResourceData, includeScheduledOperations);
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
