using Sdl.Core.Globalization;
using Sdl.Core.Settings;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public interface IFileTypeCreator : IFileTypeDefinitionAware
	{
		LocalizableString Description
		{
			get;
			set;
		}

		IFileTypeComponentBuilder TemplateComponentBuilder
		{
			get;
		}

		IEnumerable<FileTypeCreatorWizardPage> CreateWizardPages(ISettingsBundle settingsBundle);
	}
}
