using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Sdl.Core.Globalization.LanguageRegistry
{
	internal class ProductDictionaryConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return true;
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			Dictionary<Product, List<AlternativeLanguageCode>> dictionary = new Dictionary<Product, List<AlternativeLanguageCode>>();
			JObject jObject = JObject.Load(reader);
			if (jObject.HasValues)
			{
				foreach (JProperty item in jObject.Properties())
				{
					dictionary.Add(new Product
					{
						ProductId = item.Name
					}, item.Value.ToObject<List<AlternativeLanguageCode>>());
				}
				return dictionary;
			}
			return dictionary;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteStartObject();
			Dictionary<Product, List<AlternativeLanguageCode>> dictionary = value as Dictionary<Product, List<AlternativeLanguageCode>>;
			if (dictionary != null)
			{
				foreach (KeyValuePair<Product, List<AlternativeLanguageCode>> item in dictionary)
				{
					writer.WritePropertyName(item.Key.ProductId);
					serializer.Serialize(writer, item.Value);
				}
			}
			writer.WriteEndObject();
		}
	}
}
