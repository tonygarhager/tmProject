using System;
using System.IO;

namespace Sdl.Core.FineGrainedAlignment
{
	public class ChiSquaredTranslationModelId : TranslationModelId
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

		static ChiSquaredTranslationModelId()
		{
			_locker = new object();
			_SerializationTypeId = new Guid("6b693188-2a92-4aa8-95b9-169872c61271");
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

		private static ChiSquaredTranslationModelId Create(BinaryReader reader)
		{
			ChiSquaredTranslationModelId chiSquaredTranslationModelId = new ChiSquaredTranslationModelId();
			chiSquaredTranslationModelId.Deserialize(reader);
			return chiSquaredTranslationModelId;
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
