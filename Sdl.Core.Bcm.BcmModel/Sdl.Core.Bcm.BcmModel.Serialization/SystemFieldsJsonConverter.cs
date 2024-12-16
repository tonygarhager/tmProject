using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sdl.LanguagePlatform.TranslationMemory;
using System;

namespace Sdl.Core.Bcm.BcmModel.Serialization
{
	public class SystemFieldsJsonConverter : JsonConverter<SystemFields>
	{
		private readonly bool _includeUserNameSystemFields;

		public SystemFieldsJsonConverter(bool includeUserNameSystemFields)
		{
			_includeUserNameSystemFields = includeUserNameSystemFields;
		}

		public override void WriteJson(JsonWriter writer, SystemFields value, JsonSerializer serializer)
		{
			JObject jObject = JObject.FromObject(value);
			if (_includeUserNameSystemFields)
			{
				jObject.Add("created_by_username", value.CreationDisplayUsername);
				jObject.Add("modified_by_username", value.ChangeDisplayUsername);
				jObject.Add("use_by_username", value.UseDisplayUsername);
			}
			jObject?.WriteTo(writer);
		}

		public override SystemFields ReadJson(JsonReader reader, Type objectType, SystemFields existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			JObject jObject = JObject.Load(reader);
			SystemFields systemFields = jObject.ToObject<SystemFields>();
			if (!_includeUserNameSystemFields)
			{
				return systemFields;
			}
			systemFields.CreationDisplayUsername = jObject["created_by_username"]?.Value<string>();
			systemFields.ChangeDisplayUsername = jObject["modified_by_username"]?.Value<string>();
			systemFields.UseDisplayUsername = jObject["use_by_username"]?.Value<string>();
			return systemFields;
		}
	}
}
