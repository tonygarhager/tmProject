using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sdl.Core.FineGrainedAlignment;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.IO;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl.FGA
{
	internal class StorageBasedAlignableCorpusId : AlignableCorpusId
	{
		private static bool _registered;

		private static readonly object Locker;

		private const int Version = 1;

		private static readonly Guid _SerializationTypeId;

		public PersistentObjectToken TmId
		{
			get;
			set;
		}

		public override Guid SerializationTypeId => _SerializationTypeId;

		public StorageBasedAlignableCorpusId(PersistentObjectToken id)
		{
			TmId = id;
		}

		protected override void Serialize(JsonWriter writer)
		{
			writer.WritePropertyName("StorageBasedAlignableCorpusId");
			writer.WriteStartObject();
			writer.WritePropertyName("Version");
			writer.WriteValue(1);
			writer.WritePropertyName("TmId");
			writer.WriteValue(TmId.Id);
			writer.WritePropertyName("TmGuid");
			writer.WriteValue(TmId.Guid);
		}

		protected override void Deserialize(BinaryReader reader)
		{
		}

		protected override void Deserialize(JsonReader reader)
		{
		}

		protected override void Serialize(BinaryWriter writer)
		{
			writer.Write(1);
			writer.Write(TmId.Id);
			writer.Write(TmId.Guid.ToString());
		}

		static StorageBasedAlignableCorpusId()
		{
			Locker = new object();
			_SerializationTypeId = new Guid("26b574d3-3646-4fcc-8ddb-d1d66a37a0d2");
			Register();
		}

		private static void Register()
		{
			lock (Locker)
			{
				if (!_registered)
				{
					AlignableCorpusId.RegisterCorpusIdType(_SerializationTypeId, Create, JsonCreate);
				}
				_registered = true;
			}
		}

		private static StorageBasedAlignableCorpusId Create(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (num != 1)
			{
				throw new Exception("Bad version: " + num.ToString());
			}
			int id = reader.ReadInt32();
			Guid guid = new Guid(reader.ReadString());
			StorageBasedAlignableCorpusId storageBasedAlignableCorpusId = new StorageBasedAlignableCorpusId(new PersistentObjectToken(id, guid));
			storageBasedAlignableCorpusId.Deserialize(reader);
			return storageBasedAlignableCorpusId;
		}

		private static StorageBasedAlignableCorpusId JsonCreate(JsonReader reader)
		{
			JToken jToken = JObject.Load(reader)["StorageBasedAlignableCorpusId"];
			int num = jToken["Version"].Value<int>();
			if (num != 1)
			{
				throw new Exception("Bad version: " + num.ToString());
			}
			int id = jToken["TmId"].Value<int>();
			Guid guid = jToken["TmGuid"].ToObject<Guid>();
			StorageBasedAlignableCorpusId storageBasedAlignableCorpusId = new StorageBasedAlignableCorpusId(new PersistentObjectToken(id, guid));
			storageBasedAlignableCorpusId.Deserialize(reader);
			return storageBasedAlignableCorpusId;
		}
	}
}
