using System;
using System.IO;

namespace Sdl.Core.FineGrainedAlignment
{
	[Serializable]
	public class TrainedTranslationModelId : TranslationModelId
	{
		private static bool _registered;

		private static readonly object _locker;

		private static readonly Guid _SerializationTypeId;

		private const int _version = 1;

		public override Guid SerializationTypeId => _SerializationTypeId;

		public int InternalId
		{
			get;
			set;
		}

		static TrainedTranslationModelId()
		{
			_locker = new object();
			_SerializationTypeId = new Guid("c92cc62f-8c37-4637-bf4f-1d877c59a8fa");
			FGAInitializer.Initialize(null);
			Register();
		}

		private static void Register()
		{
			lock (_locker)
			{
				if (!_registered)
				{
					TranslationModelId.RegisterTranslationModelIdType(_SerializationTypeId, Create);
				}
				_registered = true;
			}
		}

		private static TrainedTranslationModelId Create(BinaryReader reader)
		{
			TrainedTranslationModelId trainedTranslationModelId = new TrainedTranslationModelId();
			trainedTranslationModelId.Deserialize(reader);
			return trainedTranslationModelId;
		}

		protected override void Serialize(BinaryWriter writer)
		{
			writer.Write(1);
			writer.Write(InternalId);
		}

		protected override void Deserialize(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (num != 1)
			{
				throw new Exception("Bad version: " + num.ToString());
			}
			InternalId = reader.ReadInt32();
		}
	}
}
