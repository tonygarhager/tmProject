using System;

namespace Sdl.Core.PluginFramework.Implementation
{
	internal class ExtensionPoint : IExtensionPoint
	{
		private class ActiveExtensionListFilter : IListFilter<IExtension>
		{
			public bool ShouldInclude(IExtension item)
			{
				return item.Enabled;
			}
		}

		private readonly FilterableList<IExtension> _allExtensions = new FilterableList<IExtension>();

		private ExtensionCollection _lazyReadOnlyAllExtensions;

		private IFilteredList<IExtension> _lazyEnabledExtensions;

		private ExtensionCollection _lazyReadOnlyEnabledExtensions;

		private ExtensionPointInfoAttribute _lazyExtensionPointInfoAttribute;

		private readonly object _syncObject = new object();

		public string Name => ExtensionPointInfoAttribute.Name;

		public Type ExtensionAttributeType => TypeLoaderUtil.GetType(ExtensionAttributeTypeName);

		public bool IsDynamic => ExtensionPointInfoAttribute.Behavior == ExtensionPointBehavior.Dynamic;

		public ExtensionCollection AllExtensions
		{
			get
			{
				lock (_syncObject)
				{
					if (_lazyReadOnlyAllExtensions == null)
					{
						_lazyReadOnlyAllExtensions = new ExtensionCollection(_allExtensions);
					}
				}
				return _lazyReadOnlyAllExtensions;
			}
		}

		public ExtensionCollection Extensions
		{
			get
			{
				lock (_syncObject)
				{
					if (_lazyReadOnlyEnabledExtensions == null)
					{
						if (_lazyEnabledExtensions == null)
						{
							_lazyEnabledExtensions = _allExtensions.GetFilteredList(new ActiveExtensionListFilter());
						}
						_lazyReadOnlyEnabledExtensions = new ExtensionCollection(_lazyEnabledExtensions);
					}
				}
				return _lazyReadOnlyEnabledExtensions;
			}
		}

		internal string ExtensionAttributeTypeName
		{
			get;
		}

		private ExtensionPointInfoAttribute ExtensionPointInfoAttribute
		{
			get
			{
				if (_lazyExtensionPointInfoAttribute == null)
				{
					object[] customAttributes = ExtensionAttributeType.GetCustomAttributes(typeof(ExtensionPointInfoAttribute), inherit: false);
					if (customAttributes.Length == 0)
					{
						throw new PluginFrameworkException("Missing extension point info attribute.");
					}
					_lazyExtensionPointInfoAttribute = (ExtensionPointInfoAttribute)customAttributes[0];
				}
				return _lazyExtensionPointInfoAttribute;
			}
		}

		public event EventHandler<ExtensionEventArgs> ExtensionEnabledChanged;

		internal ExtensionPoint(string extensionAttributeTypeName)
		{
			ExtensionAttributeTypeName = extensionAttributeTypeName;
		}

		internal void RegisterExtension(Extension extension)
		{
			extension.EnabledChanged += Extension_EnabledChanged;
			_allExtensions.Add(extension);
		}

		private void Extension_EnabledChanged(object sender, ExtensionEventArgs e)
		{
			_lazyEnabledExtensions?.Refresh();
			OnExtensionEnabledChanged(e);
		}

		private void OnExtensionEnabledChanged(ExtensionEventArgs e)
		{
			this.ExtensionEnabledChanged?.Invoke(this, e);
		}
	}
}
