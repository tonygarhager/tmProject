using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace Sdl.Core.FineGrainedAlignment
{
	public abstract class AlignableCorpusId
	{
		private const int VERSION = 1;

		private static readonly Dictionary<Guid, Func<BinaryReader, AlignableCorpusId>> CreatorFuncs;

		private static readonly Dictionary<Guid, Func<JsonReader, AlignableCorpusId>> JsonCreatorFuncs;

		public abstract Guid SerializationTypeId
		{
			get;
		}

		protected abstract void Serialize(BinaryWriter writer);

		protected abstract void Serialize(JsonWriter writer);

		protected abstract void Deserialize(BinaryReader reader);

		protected abstract void Deserialize(JsonReader reader);

		public string ToJsonSerialization()
		{
			using (StringWriter stringWriter = new StringWriter())
			{
				using (JsonTextWriter jsonTextWriter = new JsonTextWriter(stringWriter))
				{
					ToSerialization(jsonTextWriter);
					jsonTextWriter.Close();
					stringWriter.Close();
					return stringWriter.ToString();
				}
			}
		}

		public byte[] ToSerialization()
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					ToSerialization(binaryWriter);
					binaryWriter.Close();
					memoryStream.Close();
					return memoryStream.ToArray();
				}
			}
		}

		public void ToSerialization(JsonWriter writer)
		{
			writer.WriteStartObject();
			writer.WritePropertyName("Version");
			writer.WriteValue(1);
			writer.WritePropertyName("SerializationTypeId");
			writer.WriteValue(SerializationTypeId.ToString());
			Serialize(writer);
			writer.WriteEndObject();
		}

		public void ToSerialization(BinaryWriter writer)
		{
			writer.Write(1);
			writer.Write(SerializationTypeId.ToString());
			Serialize(writer);
		}

		public static AlignableCorpusId FromJsonSerialization(string serialization)
		{
			JsonTextReader reader = new JsonTextReader(new StringReader(serialization));
			return FromSerialization(reader);
		}

		public static AlignableCorpusId FromSerialization(JsonReader reader)
		{
			JObject jObject = JObject.Load(reader);
			int num = jObject["Version"].Value<int>();
			if (num != 1)
			{
				throw new Exception("Bad version: " + num.ToString());
			}
			Guid guid = jObject["SerializationTypeId"].ToObject<Guid>();
			if (!JsonCreatorFuncs.ContainsKey(guid))
			{
				Guid guid2 = guid;
				throw new Exception("Unknown aligner definition type id: " + guid2.ToString());
			}
			return JsonCreatorFuncs[guid](jObject.CreateReader());
		}

		public static AlignableCorpusId FromSerialization(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (num != 1)
			{
				throw new Exception("Bad version: " + num.ToString());
			}
			Guid guid = new Guid(reader.ReadString());
			if (!CreatorFuncs.ContainsKey(guid))
			{
				Guid guid2 = guid;
				throw new Exception("Unknown corpus type id: " + guid2.ToString());
			}
			return CreatorFuncs[guid](reader);
		}

		public static AlignableCorpusId FromSerialization(byte[] serialization)
		{
			using (MemoryStream input = new MemoryStream(serialization))
			{
				using (BinaryReader reader = new BinaryReader(input))
				{
					return FromSerialization(reader);
				}
			}
		}

		public static void RegisterCorpusIdType(Guid idType, Func<BinaryReader, AlignableCorpusId> creator, Func<JsonReader, AlignableCorpusId> jsonCreator)
		{
			lock (CreatorFuncs)
			{
				CreatorFuncs.Add(idType, creator);
			}
			lock (JsonCreatorFuncs)
			{
				JsonCreatorFuncs.Add(idType, jsonCreator);
			}
		}

		static AlignableCorpusId()
		{
			CreatorFuncs = new Dictionary<Guid, Func<BinaryReader, AlignableCorpusId>>();
			JsonCreatorFuncs = new Dictionary<Guid, Func<JsonReader, AlignableCorpusId>>();
			FGAInitializer.Initialize(null);
		}
	}
}
