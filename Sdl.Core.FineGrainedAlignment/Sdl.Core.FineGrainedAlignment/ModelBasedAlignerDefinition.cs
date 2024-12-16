using System;
using System.IO;

namespace Sdl.Core.FineGrainedAlignment
{
	[Serializable]
	public class ModelBasedAlignerDefinition : AlignerDefinition
	{
		private static bool _registered;

		private static readonly object _locker;

		private const int _version = 1;

		private readonly TranslationModelId _modelId;

		private static readonly Guid _SerializationTypeId;

		public TranslationModelId ModelId => _modelId;

		public override bool IsModelFree => false;

		public override Guid SerializationTypeId => _SerializationTypeId;

		public ModelBasedAlignerDefinition(TranslationModelId modelId)
		{
			_modelId = modelId;
		}

		static ModelBasedAlignerDefinition()
		{
			_locker = new object();
			_SerializationTypeId = new Guid("2c4d69c3-16e3-408f-9b2c-309c99724116");
			FGAInitializer.Initialize(null);
			Register();
		}

		private static void Register()
		{
			lock (_locker)
			{
				if (!_registered)
				{
					AlignerDefinition.RegisterAlignerDefinitionType(_SerializationTypeId, Create);
				}
				_registered = true;
			}
		}

		private static ModelBasedAlignerDefinition Create(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (num != 1)
			{
				throw new Exception("Bad version: " + num.ToString());
			}
			TranslationModelId modelId = TranslationModelId.FromSerialization(reader);
			ModelBasedAlignerDefinition modelBasedAlignerDefinition = new ModelBasedAlignerDefinition(modelId);
			modelBasedAlignerDefinition.Deserialize(reader);
			return modelBasedAlignerDefinition;
		}

		protected override void Serialize(BinaryWriter writer)
		{
			writer.Write(1);
			_modelId.ToSerialization(writer);
		}

		protected override void Deserialize(BinaryReader reader)
		{
		}
	}
}
