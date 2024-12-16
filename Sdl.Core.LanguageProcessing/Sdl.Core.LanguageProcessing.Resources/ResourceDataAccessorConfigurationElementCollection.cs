using System.Configuration;

namespace Sdl.Core.LanguageProcessing.Resources
{
	internal class ResourceDataAccessorConfigurationElementCollection : ConfigurationElementCollection
	{
		public override ConfigurationElementCollectionType CollectionType => ConfigurationElementCollectionType.AddRemoveClearMap;

		public new string AddElementName
		{
			get
			{
				return base.AddElementName;
			}
			set
			{
				base.AddElementName = value;
			}
		}

		public new string ClearElementName
		{
			get
			{
				return base.ClearElementName;
			}
			set
			{
				base.ClearElementName = value;
			}
		}

		public new string RemoveElementName => base.RemoveElementName;

		public new int Count => base.Count;

		public ResourceDataAccessorConfigurationElement this[int index]
		{
			get
			{
				return (ResourceDataAccessorConfigurationElement)BaseGet(index);
			}
			set
			{
				if (BaseGet(index) != null)
				{
					BaseRemoveAt(index);
				}
				BaseAdd(index, value);
			}
		}

		public new ResourceDataAccessorConfigurationElement this[string name] => (ResourceDataAccessorConfigurationElement)BaseGet(name);

		protected override ConfigurationElement CreateNewElement()
		{
			return new ResourceDataAccessorConfigurationElement();
		}

		protected override ConfigurationElement CreateNewElement(string name)
		{
			return new ResourceDataAccessorConfigurationElement(name);
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((ResourceDataAccessorConfigurationElement)element).Name;
		}

		public int IndexOf(ResourceDataAccessorConfigurationElement element)
		{
			return BaseIndexOf(element);
		}

		public void Add(ResourceDataAccessorConfigurationElement element)
		{
			BaseAdd(element);
		}

		protected override void BaseAdd(ConfigurationElement element)
		{
			BaseAdd(element, throwIfExists: false);
		}

		public void Remove(ResourceDataAccessorConfigurationElement element)
		{
			if (BaseIndexOf(element) >= 0)
			{
				BaseRemove(element.Name);
			}
		}

		public void RemoveAt(int index)
		{
			BaseRemoveAt(index);
		}

		public void Remove(string name)
		{
			BaseRemove(name);
		}

		public void Clear()
		{
			BaseClear();
		}
	}
}
