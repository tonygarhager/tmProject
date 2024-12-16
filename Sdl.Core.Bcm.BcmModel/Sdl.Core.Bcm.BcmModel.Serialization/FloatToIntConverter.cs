using Newtonsoft.Json;
using System;

namespace Sdl.Core.Bcm.BcmModel.Serialization
{
	internal class FloatToIntConverter : JsonConverter
	{
		public override bool CanWrite => false;

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			int num = Convert.ToInt32(reader.Value);
			return num;
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(float);
		}
	}
}
