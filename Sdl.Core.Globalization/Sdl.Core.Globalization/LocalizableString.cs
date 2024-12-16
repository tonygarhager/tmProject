using System;
using System.Reflection;
using System.Resources;

namespace Sdl.Core.Globalization
{
	public class LocalizableString
	{
		private readonly string _internalContent;

		public string Content
		{
			get
			{
				if (!IsResource)
				{
					return _internalContent;
				}
				return LoadResourceString(_internalContent);
			}
		}

		public string RawContent => _internalContent;

		public bool IsResource => _internalContent.StartsWith("assembly://", StringComparison.OrdinalIgnoreCase);

		public LocalizableString(string content)
		{
			_internalContent = content;
		}

		public override string ToString()
		{
			return Content;
		}

		private static string LoadResourceString(string embeddedResourceName)
		{
			if (!embeddedResourceName.StartsWith("assembly://", StringComparison.OrdinalIgnoreCase))
			{
				throw new ArgumentException(Resources.ResourceHasNoAssembly);
			}
			string text = embeddedResourceName.Substring(11);
			if (text.IndexOf("/", StringComparison.Ordinal) < 0)
			{
				throw new ArgumentException(Resources.ResourceHasNoSlash);
			}
			string assemblyString = text.Substring(0, text.IndexOf("/", StringComparison.Ordinal));
			if (text.IndexOf("/", StringComparison.Ordinal) < 0)
			{
				throw new ArgumentException(Resources.ResourceIdHasNoNamespace);
			}
			text = text.Substring(text.IndexOf("/", StringComparison.Ordinal) + 1);
			if (text.IndexOf("/", StringComparison.Ordinal) == 0)
			{
				throw new ArgumentException(Resources.ResourceIdStartsWithSlash);
			}
			string baseName = text.Substring(0, text.IndexOf("/", StringComparison.Ordinal));
			text = text.Substring(text.IndexOf("/", StringComparison.Ordinal) + 1);
			string name = text;
			Assembly assembly = Assembly.Load(assemblyString);
			ResourceManager resourceManager = new ResourceManager(baseName, assembly);
			return resourceManager.GetString(name);
		}
	}
}
