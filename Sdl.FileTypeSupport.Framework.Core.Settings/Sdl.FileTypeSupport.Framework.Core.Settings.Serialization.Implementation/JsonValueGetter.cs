using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.FileTypeSupport.Framework.Core.Settings.Serialization.Implementation
{
	internal class JsonValueGetter : IValueGetter
	{
		private readonly JObject _currentObject;

		public JsonValueGetter(JObject jObject)
		{
			_currentObject = jObject;
		}

		public int GetValue(string key, int defaultValue)
		{
			if (_currentObject.TryGetValue(key, out JToken value))
			{
				return value.Value<int>();
			}
			return defaultValue;
		}

		public string GetValue(string key, string defaultValue)
		{
			if (_currentObject.TryGetValue(key, out JToken value))
			{
				return value.Value<string>();
			}
			return defaultValue;
		}

		public bool GetValue(string key, bool defaultValue)
		{
			if (_currentObject.TryGetValue(key, out JToken value))
			{
				return value.Value<bool>();
			}
			return defaultValue;
		}

		public T GetValue<T>(string key, T defaultValue, bool discardKey) where T : ISettingsClass, new()
		{
			if (_currentObject.TryGetValue(key, out JToken value))
			{
				JObject jObject = value as JObject;
				if (jObject != null)
				{
					T result = new T();
					result.ResetToDefaults();
					result.Read(new JsonValueGetter(jObject));
					return result;
				}
			}
			return (T)defaultValue.Clone();
		}

		public List<string> GetStringList(string key, List<string> defaultValue)
		{
			if (_currentObject.TryGetValue(key, out JToken value))
			{
				JArray jArray = value as JArray;
				if (jArray != null)
				{
					List<string> list = new List<string>();
					{
						foreach (JToken item in jArray)
						{
							list.Add(item.Value<string>());
						}
						return list;
					}
				}
			}
			return new List<string>(defaultValue);
		}

		public Dictionary<string, string> GetStringDictionary(string key, Dictionary<string, string> defaultValue)
		{
			if (_currentObject.TryGetValue(key, out JToken value))
			{
				JObject jObject = value as JObject;
				if (jObject != null)
				{
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					{
						foreach (KeyValuePair<string, JToken> item in jObject)
						{
							dictionary.Add(item.Key, item.Value.Value<string>());
						}
						return dictionary;
					}
				}
			}
			return defaultValue.ToDictionary((KeyValuePair<string, string> item) => item.Key, (KeyValuePair<string, string> item) => item.Value);
		}

		public List<T> GetCompositeList<T>(string key, List<T> defaultValue) where T : ISettingsClass, new()
		{
			if (_currentObject.TryGetValue(key, out JToken value))
			{
				JArray jArray = value as JArray;
				if (jArray != null)
				{
					List<T> list = new List<T>();
					{
						foreach (JToken item2 in jArray)
						{
							JObject jObject = item2 as JObject;
							if (jObject != null)
							{
								T item = new T();
								item.ResetToDefaults();
								item.Read(new JsonValueGetter(jObject));
								list.Add(item);
							}
						}
						return list;
					}
				}
			}
			return new List<T>(defaultValue.Select((T x) => (T)x.Clone()));
		}
	}
}
