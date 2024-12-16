using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sdl.LanguagePlatform.TranslationMemory;
using System;

namespace Sdl.Core.Bcm.BcmConverters.Fields
{
	public class FieldsConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return typeof(FieldValue).IsAssignableFrom(objectType);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			JObject jObject = JObject.Load(reader);
			FieldValue fieldValue = Create(jObject);
			serializer.Populate(jObject.CreateReader(), fieldValue);
			return fieldValue;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		private static FieldValue Create(JObject jObject)
		{
			FieldValueType valueType = (FieldValueType)((dynamic)jObject).ValueType;
			Field field = new Field((string)((dynamic)jObject).Name, valueType);
			return field.CreateValue();
		}
	}
}
