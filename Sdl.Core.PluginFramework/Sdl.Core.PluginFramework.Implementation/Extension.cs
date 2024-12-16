using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;

namespace Sdl.Core.PluginFramework.Implementation
{
	internal class Extension : IExtension
	{
		private readonly XElement _extensionElement;

		private ExtensionPoint _extensionPoint;

		private ExtensionAttribute _lazyExtensionAttribute;

		private Type _lazyExtensionType;

		private AuxiliaryExtensionAttributeCollection _lazyAuxiliaryExtensionAttributes;

		private readonly IObjectResolver _objectResolver;

		public IPlugin Plugin => PluginImpl;

		public IExtensionPoint ExtensionPoint => _extensionPoint;

		public bool Enabled
		{
			get;
			private set;
		}

		public ExtensionAttribute ExtensionAttribute
		{
			get
			{
				if (_lazyExtensionAttribute == null)
				{
					XElement attributeElement = _extensionElement.Element(XName.Get("extensionAttribute", string.Empty));
					_lazyExtensionAttribute = (ExtensionAttribute)PluginDeserializer.DeserializeAttribute(this, attributeElement, _objectResolver);
				}
				return _lazyExtensionAttribute;
			}
		}

		public Type ExtensionType
		{
			get
			{
				if (_lazyExtensionType == null)
				{
					_lazyExtensionType = PluginImpl.PluginRegistry.PluginTypeLoader.LoadType(PluginImpl, ExtensionTypeName);
				}
				return _lazyExtensionType;
			}
		}

		public AuxiliaryExtensionAttributeCollection AuxiliaryExtensionAttributes
		{
			get
			{
				if (_lazyAuxiliaryExtensionAttributes == null)
				{
					List<AuxiliaryExtensionAttribute> list = new List<AuxiliaryExtensionAttribute>();
					foreach (XElement item in _extensionElement.Element(XName.Get("auxiliaryExtensionAttributes", string.Empty)).Elements(XName.Get("auxiliaryExtensionAttribute")))
					{
						list.Add((AuxiliaryExtensionAttribute)PluginDeserializer.DeserializeAttribute(this, item, _objectResolver));
					}
					_lazyAuxiliaryExtensionAttributes = new AuxiliaryExtensionAttributeCollection(list);
				}
				return _lazyAuxiliaryExtensionAttributes;
			}
		}

		internal Plugin PluginImpl
		{
			get;
		}

		internal string ExtensionTypeName
		{
			get;
		}

		internal string ExtensionAttributeTypeName => _extensionElement.Element(XName.Get("extensionAttribute", string.Empty)).Attribute(XName.Get("type", string.Empty))?.Value;

		public event EventHandler<ExtensionEventArgs> EnabledChanged;

		internal Extension(Plugin plugin, XElement extensionElement, bool enabled, IObjectResolver objectResolver)
		{
			PluginImpl = plugin;
			_extensionElement = extensionElement;
			_objectResolver = objectResolver;
			ExtensionTypeName = PluginXmlUtils.GetRequiredAttribute(PluginImpl, _extensionElement, "type");
			Enabled = enabled;
			ResolveExtensionTypeName();
		}

		private void ResolveExtensionTypeName()
		{
			XElement xElement = _extensionElement.Element(XName.Get("extensionAttribute", string.Empty));
			string value = xElement.Attribute("type").Value;
			string[] array = value.Split(',');
			if (array.Length > 2)
			{
				string text = array[0] + ", " + array[1];
				Type type = Type.GetType(text);
				if (type != null && string.Compare(type.AssemblyQualifiedName, value, StringComparison.InvariantCulture) != 0)
				{
					xElement.Attribute("type").Value = text;
				}
			}
		}

		public bool SetEnabled(bool enabled)
		{
			if (Enabled != enabled)
			{
				Enabled = enabled;
				OnEnabledChanged();
				return ExtensionPoint.IsDynamic;
			}
			return true;
		}

		public object CreateInstance()
		{
			object obj = _objectResolver.CreateObject(ExtensionType);
			IExtensionAware extensionAware = obj as IExtensionAware;
			if (extensionAware != null)
			{
				extensionAware.Extension = this;
			}
			return obj;
		}

		public T[] GetAuxiliaryExtensionAttributes<T>() where T : AuxiliaryExtensionAttribute
		{
			List<T> list = new List<T>();
			foreach (AuxiliaryExtensionAttribute auxiliaryExtensionAttribute in AuxiliaryExtensionAttributes)
			{
				T val = auxiliaryExtensionAttribute as T;
				if (val != null)
				{
					list.Add(val);
				}
			}
			return list.ToArray();
		}

		public T GetPluginResource<T>(string resourceName) where T : class
		{
			return PluginImpl.GetPluginResource<T>(resourceName);
		}

		public void Validate()
		{
			try
			{
				ExtensionAttribute.GetType();
				AuxiliaryExtensionAttributes.GetType();
				ExtensionType.GetType();
			}
			catch (Exception innerException)
			{
				throw new PluginFrameworkException(string.Format(CultureInfo.InvariantCulture, "Failed to validate extension '{0}'", new object[1]
				{
					ExtensionTypeName
				}), innerException);
			}
		}

		internal void RegisterExtensionPoint(ExtensionPoint extensionPoint)
		{
			_extensionPoint = extensionPoint;
		}

		private void OnEnabledChanged()
		{
			this.EnabledChanged?.Invoke(this, new ExtensionEventArgs(this));
		}
	}
}
