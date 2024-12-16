using Sdl.LanguagePlatform.Core.Resources;
using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Sdl.Core.LanguageProcessing.Resources
{
	public class Configuration
	{
		public static readonly string ConfigurationSectionName = "sdl.languageplatform.resourceaccessors";

		public static CompositeResourceDataAccessor Load()
		{
			CompositeResourceDataAccessor compositeResourceDataAccessor = new CompositeResourceDataAccessor(addDefaultAccessor: false);
			ResourceDataAccessorConfigurationSection resourceDataAccessorConfigurationSection = ConfigurationManager.GetSection(ConfigurationSectionName) as ResourceDataAccessorConfigurationSection;
			if (resourceDataAccessorConfigurationSection?.ResourceDataAccessors == null || resourceDataAccessorConfigurationSection.ResourceDataAccessors.Count == 0)
			{
				compositeResourceDataAccessor.AddDefaultAccessor();
				return compositeResourceDataAccessor;
			}
			for (int i = 0; i < resourceDataAccessorConfigurationSection.ResourceDataAccessors.Count; i++)
			{
				ResourceDataAccessorConfigurationElement resourceDataAccessorConfigurationElement = resourceDataAccessorConfigurationSection.ResourceDataAccessors[i];
				Type type = Type.GetType(resourceDataAccessorConfigurationElement.Type);
				if (type == null)
				{
					throw new LanguageProcessingException(ErrorMessages.EMSG_ConfigurationCannotResolveType);
				}
				if (!type.GetInterfaces().Any((Type t) => t == typeof(IResourceDataAccessor)))
				{
					throw new LanguageProcessingException(ErrorMessages.EMSG_ConfigurationInvalidType);
				}
				if (type.IsAbstract)
				{
					throw new LanguageProcessingException(ErrorMessages.EMSG_ConfigurationAbstractType);
				}
				object obj = type.Assembly.CreateInstance(type.FullName ?? throw new InvalidOperationException(), ignoreCase: false, BindingFlags.CreateInstance, null, string.IsNullOrEmpty(resourceDataAccessorConfigurationElement.Parameter) ? null : new object[1]
				{
					resourceDataAccessorConfigurationElement.Parameter
				}, CultureInfo.CurrentCulture, null);
				if (obj == null)
				{
					throw new LanguageProcessingException(ErrorMessages.EMSG_ConfigurationCannotInstantiateOrCastType);
				}
				IResourceDataAccessor resourceDataAccessor = obj as IResourceDataAccessor;
				if (resourceDataAccessor == null)
				{
					throw new LanguageProcessingException(ErrorMessages.EMSG_ConfigurationCannotInstantiateOrCastType);
				}
				compositeResourceDataAccessor.Add(resourceDataAccessor);
			}
			if (compositeResourceDataAccessor.Count == 0)
			{
				compositeResourceDataAccessor.AddDefaultAccessor();
			}
			return compositeResourceDataAccessor;
		}
	}
}
