using Sdl.Core.Globalization;
using Sdl.Core.Settings;
using System;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi.Implementation
{
	public class FileTypeCreator : AbstractFileTypeDefinitionComponent, IFileTypeCreator, IFileTypeDefinitionAware
	{
		private LocalizableString _description;

		public virtual LocalizableString Description
		{
			get
			{
				return _description;
			}
			set
			{
				_description = value;
			}
		}

		public virtual IFileTypeComponentBuilder TemplateComponentBuilder
		{
			get
			{
				throw new NotImplementedException("You must implement this property in the sub-class");
			}
		}

		public virtual IEnumerable<FileTypeCreatorWizardPage> CreateWizardPages(ISettingsBundle settingsBundle)
		{
			return new FileTypeCreatorWizardPage[0];
		}
	}
}
