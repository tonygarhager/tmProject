using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sdl.Core.Bcm.BcmModel.Tokenization.Tokens;
using System;

namespace Sdl.Core.Bcm.BcmModel.Serialization
{
	internal class TokenCreator : JsonCreationConverter<Token>
	{
		protected override Token Create(Type objectType, JObject jObject)
		{
			string type = (string)(JToken)jObject.Property("type");
			return TokenMapping.GetType(type);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
}
