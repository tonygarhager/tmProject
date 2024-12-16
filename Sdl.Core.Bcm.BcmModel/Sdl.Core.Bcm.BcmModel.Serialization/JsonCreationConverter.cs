using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Sdl.Core.Bcm.BcmModel.Serialization
{
	internal abstract class JsonCreationConverter<T> : JsonConverter
	{
		public override bool CanWrite => false;

		protected abstract T Create(Type objectType, JObject jObject);

		public override bool CanConvert(Type objectType)
		{
			return typeof(T).IsAssignableFrom(objectType);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			JObject jObject = JObject.Load(reader);
			T val = Create(objectType, jObject);
			serializer.Populate(jObject.CreateReader(), val);
			return val;
		}
	}
}
