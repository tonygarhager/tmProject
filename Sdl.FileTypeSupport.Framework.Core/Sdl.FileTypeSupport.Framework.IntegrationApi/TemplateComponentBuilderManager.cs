using Sdl.Core.PluginFramework;
using System;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public class TemplateComponentBuilderManager
	{
		private List<IFileTypeComponentBuilder> _templateComponentBuilders = new List<IFileTypeComponentBuilder>();

		private IFileTypeManager _fileTypeManager;

		public IEnumerable<IFileTypeComponentBuilder> TemplateComponentBuilders
		{
			get
			{
				foreach (IFileTypeComponentBuilder templateComponentBuilder in _templateComponentBuilders)
				{
					yield return templateComponentBuilder;
				}
			}
		}

		public TemplateComponentBuilderManager(IFileTypeManager fileTypeManager)
		{
			if (fileTypeManager == null)
			{
				throw new ArgumentNullException("fileTypeManager cannot be null");
			}
			_fileTypeManager = fileTypeManager;
			LoadAllTemplateComponentBuilders();
		}

		protected void LoadAllTemplateComponentBuilders()
		{
			IExtensionPoint extensionPoint = PluginManager.DefaultPluginRegistry.GetExtensionPoint<FileTypeComponentBuilderAttribute>();
			for (int i = 0; i < extensionPoint.Extensions.Count; i++)
			{
				FileTypeComponentBuilderAttribute fileTypeComponentBuilderAttribute = extensionPoint.Extensions[i].ExtensionAttribute as FileTypeComponentBuilderAttribute;
				if (fileTypeComponentBuilderAttribute != null && fileTypeComponentBuilderAttribute.IsTemplate)
				{
					IFileTypeComponentBuilder fileTypeComponentBuilder = (IFileTypeComponentBuilder)extensionPoint.Extensions[i].CreateInstance();
					fileTypeComponentBuilder.FileTypeManager = _fileTypeManager;
					_templateComponentBuilders.Add(fileTypeComponentBuilder);
				}
			}
		}
	}
}
