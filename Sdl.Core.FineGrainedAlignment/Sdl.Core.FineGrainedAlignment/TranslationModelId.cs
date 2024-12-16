using System;
using System.Collections.Generic;
using System.IO;

namespace Sdl.Core.FineGrainedAlignment
{
	public abstract class TranslationModelId
	{
		private const int _version = 1;

		private static readonly Dictionary<Guid, Func<BinaryReader, TranslationModelId>> _creatorFuncs;

		public abstract Guid SerializationTypeId
		{
			get;
		}

		protected abstract void Serialize(BinaryWriter writer);

		protected abstract void Deserialize(BinaryReader reader);

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

		public void ToSerialization(BinaryWriter writer)
		{
			writer.Write(1);
			writer.Write(SerializationTypeId.ToString());
			Serialize(writer);
		}

		public static TranslationModelId FromSerialization(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (num != 1)
			{
				throw new Exception("Bad version: " + num.ToString());
			}
			Guid guid = new Guid(reader.ReadString());
			if (!_creatorFuncs.ContainsKey(guid))
			{
				Guid guid2 = guid;
				throw new Exception("Unknown translation model type id: " + guid2.ToString());
			}
			return _creatorFuncs[guid](reader);
		}

		public static TranslationModelId FromSerialization(byte[] serialization)
		{
			using (MemoryStream input = new MemoryStream(serialization))
			{
				using (BinaryReader reader = new BinaryReader(input))
				{
					return FromSerialization(reader);
				}
			}
		}

		public static void RegisterTranslationModelIdType(Guid idType, Func<BinaryReader, TranslationModelId> creator)
		{
			lock (_creatorFuncs)
			{
				_creatorFuncs.Add(idType, creator);
			}
		}

		static TranslationModelId()
		{
			_creatorFuncs = new Dictionary<Guid, Func<BinaryReader, TranslationModelId>>();
			FGAInitializer.Initialize(null);
		}
	}
}
