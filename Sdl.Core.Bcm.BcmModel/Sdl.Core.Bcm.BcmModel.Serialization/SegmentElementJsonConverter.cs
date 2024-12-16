using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sdl.LanguagePlatform.Core;
using System;

namespace Sdl.Core.Bcm.BcmModel.Serialization
{
	public class SegmentElementJsonConverter : JsonConverter<SegmentElement>
	{
		private const string TextType = "text";

		private const string TagType = "tag";

		public override void WriteJson(JsonWriter writer, SegmentElement value, JsonSerializer serializer)
		{
			JObject jObject = null;
			Text text = value as Text;
			if (text == null)
			{
				Tag tag = value as Tag;
				if (tag != null)
				{
					jObject = JObject.FromObject(tag);
					jObject.Add("$elementType", JToken.FromObject("tag"));
				}
			}
			else
			{
				jObject = JObject.FromObject(text);
				jObject.Add("$elementType", JToken.FromObject("text"));
			}
			jObject?.WriteTo(writer);
		}

		public override SegmentElement ReadJson(JsonReader reader, Type objectType, SegmentElement existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			JObject jObject = JObject.Load(reader);
			switch (jObject["$elementType"]?.Value<string>())
			{
			case "text":
				return jObject.ToObject<Text>();
			case "tag":
				return jObject.ToObject<Tag>();
			default:
				return null;
			}
		}
	}
}
