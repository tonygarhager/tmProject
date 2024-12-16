using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Sdl.Core.Bcm.BcmModel.Serialization
{
	internal class MarkupDataCreator : JsonCreationConverter<MarkupData>
	{
		protected override MarkupData Create(Type objectType, JObject jObject)
		{
			string type = (string)(JToken)jObject.Property("type");
			return MarkupDataMapping.GetType(type);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
}
