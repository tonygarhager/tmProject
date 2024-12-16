using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.Core.Settings.Serialization.Implementation
{
	internal class JsonValueProcessor : IValueProcessor
	{
		public JObject CurrentObject
		{
			get;
			set;
		}

		public JsonValueProcessor()
		{
			CurrentObject = new JObject();
		}

		public void Process(string key, string value, string defaultValue)
		{
			CurrentObject.Add(key, value);
		}

		public void Process(string key, bool value, bool defaultValue)
		{
			CurrentObject.Add(key, value);
		}

		public void Process(string key, int value, int defaultValue)
		{
			CurrentObject.Add(key, value);
		}

		public void Process(string key, ISettingsClass value, ISettingsClass defaultValue, bool discardKey)
		{
			JsonValueProcessor jsonValueProcessor = new JsonValueProcessor();
			value.Save(jsonValueProcessor);
			CurrentObject.Add(key, jsonValueProcessor.CurrentObject);
		}

		public void Process(string key, List<string> value, List<string> defaultValue)
		{
			JArray jArray = new JArray();
			foreach (string item in value)
			{
				jArray.Add(item);
			}
			CurrentObject.Add(key, jArray);
		}

		public void Process(string key, IReadOnlyDictionary<string, string> value, IReadOnlyDictionary<string, string> defaultValues)
		{
			JObject jObject = new JObject();
			foreach (KeyValuePair<string, string> item in value)
			{
				jObject.Add(item.Key, item.Value);
			}
			CurrentObject.Add(key, jObject);
		}

		public void Process(string key, IReadOnlyList<ISettingsClass> value, IReadOnlyList<ISettingsClass> defaultValue)
		{
			JArray jArray = new JArray();
			foreach (ISettingsClass item in value)
			{
				JsonValueProcessor jsonValueProcessor = new JsonValueProcessor();
				item.Save(jsonValueProcessor);
				jArray.Add(jsonValueProcessor.CurrentObject);
			}
			CurrentObject.Add(key, jArray);
		}
	}
}
