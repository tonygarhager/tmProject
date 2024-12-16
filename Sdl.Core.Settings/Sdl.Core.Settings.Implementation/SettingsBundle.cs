using Sdl.Core.Settings.Implementation.Xml;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Xml;

namespace Sdl.Core.Settings.Implementation
{
	internal class SettingsBundle : ISettingsBundle
	{
		private Sdl.Core.Settings.Implementation.Xml.SettingsBundle _xmlBundle;

		private IDictionary<string, SettingsGroup> _lazySettingsGroups = new Dictionary<string, SettingsGroup>();

		private ISettingsBundle _parent;

		private bool _isDefault;

		public ISettingsBundle Parent
		{
			get
			{
				return _parent;
			}
			set
			{
				SettingsGroup[] array = new SettingsGroup[_lazySettingsGroups.Values.Count];
				_lazySettingsGroups.Values.CopyTo(array, 0);
				SettingsGroup[] array2 = array;
				foreach (SettingsGroup settingsGroup in array2)
				{
					settingsGroup.OnParentChanging();
				}
				_parent = value;
				SettingsGroup[] array3 = array;
				foreach (SettingsGroup settingsGroup2 in array3)
				{
					settingsGroup2.OnParentChanged();
				}
			}
		}

		public SettingsBundle ParentImpl => (SettingsBundle)Parent;

		public bool IsEmpty
		{
			get
			{
				foreach (Sdl.Core.Settings.Implementation.Xml.SettingsGroup settingsGroup in _xmlBundle.SettingsGroups)
				{
					if (settingsGroup.Settings.Count > 0)
					{
						return false;
					}
				}
				return true;
			}
		}

		public bool IsDefault => _isDefault;

		internal Sdl.Core.Settings.Implementation.Xml.SettingsBundle XmlBundle => _xmlBundle;

		internal SettingsBundle(Sdl.Core.Settings.Implementation.Xml.SettingsBundle xmlBundle, ISettingsBundle parent, bool isDefault)
		{
			_xmlBundle = xmlBundle;
			_parent = parent;
			_isDefault = isDefault;
		}

		internal void WriteXml(XmlWriter writer, bool includeInheritedSettings)
		{
			if (includeInheritedSettings)
			{
				Dictionary<string, Dictionary<string, Setting>> dictionary = new Dictionary<string, Dictionary<string, Setting>>();
				CollectSettings(this, dictionary);
				writer.WriteStartElement("SettingsBundle");
				foreach (KeyValuePair<string, Dictionary<string, Setting>> item in dictionary)
				{
					if (item.Value.Count > 0)
					{
						writer.WriteStartElement("SettingsGroup");
						writer.WriteAttributeString("Id", item.Key);
						foreach (Setting value in item.Value.Values)
						{
							value.WriteXml(writer);
						}
						writer.WriteEndElement();
					}
				}
				writer.WriteEndElement();
			}
			else
			{
				XmlBundle.WriteXml(writer);
			}
		}

		private void CollectSettings(SettingsBundle bundle, Dictionary<string, Dictionary<string, Setting>> settings)
		{
			foreach (Sdl.Core.Settings.Implementation.Xml.SettingsGroup settingsGroup in bundle.XmlBundle.SettingsGroups)
			{
				if (!settings.TryGetValue(settingsGroup.Id, out Dictionary<string, Setting> value))
				{
					value = new Dictionary<string, Setting>();
					settings[settingsGroup.Id] = value;
				}
				foreach (Setting value2 in settingsGroup.Settings.Values)
				{
					if (!value.ContainsKey(value2.Id))
					{
						value[value2.Id] = value2;
					}
				}
			}
			SettingsBundle parentImpl = bundle.ParentImpl;
			if (parentImpl != null)
			{
				CollectSettings(parentImpl, settings);
			}
		}

		public T GetSettingsGroup<T>(string id) where T : ISettingsGroup, new()
		{
			return (T)GetSettingsGroup(id, typeof(T));
		}

		public T GetSettingsGroup<T>() where T : ISettingsGroup, new()
		{
			return GetSettingsGroup<T>(typeof(T).Name);
		}

		public ISettingsGroup GetSettingsGroup(string id)
		{
			return GetSettingsGroup(id, typeof(GenericSettingsGroup));
		}

		public void RemoveSettingsGroup(string id)
		{
			ISettingsGroup settingsGroup = GetSettingsGroup(id);
			settingsGroup.Reset();
			if (_lazySettingsGroups.ContainsKey(id))
			{
				_lazySettingsGroups.Remove(id);
			}
			_xmlBundle.RemoveSettingsGroup(id);
		}

		public void Reset()
		{
			SettingsGroup[] array = new SettingsGroup[_lazySettingsGroups.Values.Count];
			_lazySettingsGroups.Values.CopyTo(array, 0);
			SettingsGroup[] array2 = array;
			foreach (ISettingsGroup settingsGroup in array2)
			{
				settingsGroup.Reset();
			}
		}

		internal ISettingsGroup GetSettingsGroup(string id, Type type)
		{
			try
			{
				Monitor.Enter(_lazySettingsGroups);
				if (_lazySettingsGroups.TryGetValue(id, out SettingsGroup value))
				{
					if (type.IsAssignableFrom(value.GetType()))
					{
						return value;
					}
					_lazySettingsGroups.Remove(id);
				}
				Sdl.Core.Settings.Implementation.Xml.SettingsGroup settingsGroup = null;
				IEnumerator<Sdl.Core.Settings.Implementation.Xml.SettingsGroup> enumerator = _xmlBundle.SettingsGroups.GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Id == id)
					{
						settingsGroup = enumerator.Current;
					}
				}
				if (settingsGroup == null)
				{
					settingsGroup = new Sdl.Core.Settings.Implementation.Xml.SettingsGroup();
					settingsGroup.Id = id;
					_xmlBundle.SettingsGroups.Add(settingsGroup);
				}
				SettingsGroup settingsGroup2 = (SettingsGroup)type.InvokeMember(null, BindingFlags.CreateInstance, null, null, null);
				settingsGroup2.XmlSettingsGroup = settingsGroup;
				settingsGroup2.SettingsBundle = this;
				_lazySettingsGroups.Add(id, settingsGroup2);
				if (!type.IsAssignableFrom(type))
				{
					throw new InvalidCastException($"The specified settings group type ('{type.AssemblyQualifiedName}') is not compatible with the actual settings group type ('{type.AssemblyQualifiedName}').");
				}
				return settingsGroup2;
			}
			finally
			{
				Monitor.Exit(_lazySettingsGroups);
			}
		}

		public IEnumerable<string> GetSettingsGroupIds()
		{
			foreach (Sdl.Core.Settings.Implementation.Xml.SettingsGroup settingsGroup in _xmlBundle.SettingsGroups)
			{
				yield return settingsGroup.Id;
			}
			if (Parent != null)
			{
				foreach (string settingsGroupId in Parent.GetSettingsGroupIds())
				{
					yield return settingsGroupId;
				}
			}
		}

		public bool ContainsSettingsGroup(string id)
		{
			if (_lazySettingsGroups.ContainsKey(id))
			{
				return true;
			}
			IEnumerator<Sdl.Core.Settings.Implementation.Xml.SettingsGroup> enumerator = _xmlBundle.SettingsGroups.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Id == id)
				{
					return true;
				}
			}
			if (Parent != null)
			{
				return Parent.ContainsSettingsGroup(id);
			}
			return false;
		}

		public void Assign(ISettingsBundle setting)
		{
			SettingsBundle settingsBundle = (SettingsBundle)setting;
			if (_lazySettingsGroups.Count != 0)
			{
				throw new InvalidOperationException("This method is currently only supported for empty settings bundles.");
			}
			_xmlBundle = (Sdl.Core.Settings.Implementation.Xml.SettingsBundle)settingsBundle.XmlBundle.Clone();
		}

		public bool AddSettingsGroup(ISettingsGroup settingsGroup)
		{
			if (settingsGroup == null || string.IsNullOrEmpty(settingsGroup.Id))
			{
				return false;
			}
			string id = settingsGroup.Id;
			Type type = settingsGroup.GetType();
			try
			{
				Monitor.Enter(_lazySettingsGroups);
				if (_lazySettingsGroups.TryGetValue(id, out SettingsGroup value))
				{
					if (type.IsAssignableFrom(value.GetType()))
					{
						return false;
					}
					_lazySettingsGroups.Remove(id);
				}
				Sdl.Core.Settings.Implementation.Xml.SettingsGroup settingsGroup2 = null;
				IEnumerator<Sdl.Core.Settings.Implementation.Xml.SettingsGroup> enumerator = _xmlBundle.SettingsGroups.GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Id == id)
					{
						settingsGroup2 = enumerator.Current;
					}
				}
				if (settingsGroup2 != null)
				{
					return false;
				}
				if (settingsGroup2 == null)
				{
					settingsGroup2 = new Sdl.Core.Settings.Implementation.Xml.SettingsGroup();
					settingsGroup2.Id = id;
					_xmlBundle.SettingsGroups.Add(settingsGroup2);
				}
				Type type2 = type;
				SettingsGroup settingsGroup3 = (SettingsGroup)type2.InvokeMember(null, BindingFlags.CreateInstance, null, null, null);
				SettingsGroup settingsGroup4 = settingsGroup as SettingsGroup;
				settingsGroup3.XmlSettingsGroup = (settingsGroup4.XmlSettingsGroup.Clone() as Sdl.Core.Settings.Implementation.Xml.SettingsGroup);
				settingsGroup3.SettingsBundle = this;
				_lazySettingsGroups.Add(id, settingsGroup3);
				if (!type.IsAssignableFrom(type2))
				{
					throw new InvalidCastException($"The specified settings group type ('{type.AssemblyQualifiedName}') is not compatible with the actual settings group type ('{type2.AssemblyQualifiedName}').");
				}
			}
			finally
			{
				Monitor.Exit(_lazySettingsGroups);
			}
			return true;
		}
	}
}
