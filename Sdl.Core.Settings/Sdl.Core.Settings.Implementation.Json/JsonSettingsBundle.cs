using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Sdl.Core.Settings.Implementation.Json
{
	public class JsonSettingsBundle : ISettingsBundle
	{
		[JsonProperty]
		internal Dictionary<string, JsonSettingsGroup> _groups = new Dictionary<string, JsonSettingsGroup>();

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		private JsonSettingsBundle _parent;

		public bool IsDefault
		{
			get;
		}

		[JsonIgnore]
		public ISettingsBundle Parent
		{
			get
			{
				return _parent;
			}
			set
			{
				JsonSettingsGroup[] array = new JsonSettingsGroup[_groups.Count];
				_groups.Values.CopyTo(array, 0);
				JsonSettingsGroup[] array2 = array;
				foreach (JsonSettingsGroup jsonSettingsGroup in array2)
				{
					jsonSettingsGroup.OnParentChanging();
				}
				_parent = (value as JsonSettingsBundle);
				JsonSettingsGroup[] array3 = array;
				foreach (JsonSettingsGroup jsonSettingsGroup2 in array3)
				{
					jsonSettingsGroup2.OnParentChanged();
				}
			}
		}

		public bool IsEmpty => _groups.Count == 0;

		public JsonSettingsBundle()
		{
		}

		internal void FixUpBundleRefs()
		{
			foreach (JsonSettingsGroup value in _groups.Values)
			{
				value.SettingsBundle = this;
			}
		}

		internal void RemoveNullGroups()
		{
			List<string> list = new List<string>();
			foreach (string key in _groups.Keys)
			{
				if (_groups[key] == null)
				{
					list.Add(key);
				}
			}
			foreach (string item in list)
			{
				_groups.Remove(item);
			}
		}

		internal JsonSettingsBundle(ISettingsBundle parent, bool isDefault)
		{
			_parent = (parent as JsonSettingsBundle);
			IsDefault = isDefault;
		}

		public bool AddSettingsGroup(ISettingsGroup settingsGroup)
		{
			if (settingsGroup == null || _groups.ContainsKey(settingsGroup.Id))
			{
				return false;
			}
			JsonSettingsGroup jsonSettingsGroup = settingsGroup as JsonSettingsGroup;
			JsonSettingsGroup jsonSettingsGroup2 = jsonSettingsGroup.Clone() as JsonSettingsGroup;
			jsonSettingsGroup2.SettingsBundle = this;
			_groups.Add(settingsGroup.Id, jsonSettingsGroup2);
			return true;
		}

		public void Assign(ISettingsBundle settings)
		{
			JsonSettingsBundle jsonSettingsBundle = (JsonSettingsBundle)settings;
			if (_groups.Count != 0)
			{
				throw new InvalidOperationException("This method is currently only supported for empty settings bundles.");
			}
			_groups = jsonSettingsBundle._groups;
		}

		public bool ContainsSettingsGroup(string id)
		{
			if (_groups.ContainsKey(id))
			{
				return true;
			}
			if (_parent != null)
			{
				return _parent.ContainsSettingsGroup(id);
			}
			return false;
		}

		public T GetSettingsGroup<T>(string id) where T : ISettingsGroup, new()
		{
			throw new NotImplementedException();
		}

		public T GetSettingsGroup<T>() where T : ISettingsGroup, new()
		{
			throw new NotImplementedException();
		}

		public ISettingsGroup GetSettingsGroup(string id)
		{
			if (!_groups.ContainsKey(id))
			{
				JsonSettingsGroup jsonSettingsGroup = new JsonSettingsGroup(id);
				jsonSettingsGroup.SettingsBundle = this;
				_groups.Add(id, jsonSettingsGroup);
				return jsonSettingsGroup;
			}
			return _groups[id];
		}

		public IEnumerable<string> GetSettingsGroupIds()
		{
			return _groups.Keys;
		}

		public void RemoveSettingsGroup(string id)
		{
			if (_groups.ContainsKey(id))
			{
				_groups.Remove(id);
			}
		}

		public void Reset()
		{
			if (_groups.Count != 0)
			{
				foreach (KeyValuePair<string, JsonSettingsGroup> group in _groups)
				{
					group.Value.Reset();
				}
			}
		}
	}
}
