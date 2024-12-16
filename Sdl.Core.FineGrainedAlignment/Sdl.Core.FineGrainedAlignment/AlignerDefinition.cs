using System;
using System.Collections.Generic;
using System.IO;

namespace Sdl.Core.FineGrainedAlignment
{
	public abstract class AlignerDefinition
	{
		private const int VERSION = 1;

		private static readonly Dictionary<Guid, Func<BinaryReader, AlignerDefinition>> CreatorFuncs;

		public abstract bool IsModelFree
		{
			get;
		}

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

		public static AlignerDefinition FromSerialization(BinaryReader reader)
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
				throw new Exception("Unknown aligner definition type id: " + guid2.ToString());
			}
			return CreatorFuncs[guid](reader);
		}

		public static AlignerDefinition FromSerialization(byte[] serialization)
		{
			using (MemoryStream input = new MemoryStream(serialization))
			{
				using (BinaryReader reader = new BinaryReader(input))
				{
					return FromSerialization(reader);
				}
			}
		}

		public static void RegisterAlignerDefinitionType(Guid idType, Func<BinaryReader, AlignerDefinition> creator)
		{
			lock (CreatorFuncs)
			{
				CreatorFuncs.Add(idType, creator);
			}
		}

		static AlignerDefinition()
		{
			CreatorFuncs = new Dictionary<Guid, Func<BinaryReader, AlignerDefinition>>();
			FGAInitializer.Initialize(null);
		}
	}
}
