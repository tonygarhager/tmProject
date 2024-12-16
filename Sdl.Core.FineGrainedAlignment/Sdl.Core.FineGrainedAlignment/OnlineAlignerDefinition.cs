using System;
using System.IO;

namespace Sdl.Core.FineGrainedAlignment
{
	[Serializable]
	public class OnlineAlignerDefinition : AlignerDefinition
	{
		private static bool _registered;

		private static readonly object _locker;

		private const int _version = 1;

		private readonly string _url;

		private static readonly Guid _SerializationTypeId;

		private string Url => _url;

		public override bool IsModelFree => true;

		public override Guid SerializationTypeId => _SerializationTypeId;

		public OnlineAlignerDefinition(string url)
		{
			_url = url;
		}

		static OnlineAlignerDefinition()
		{
			_locker = new object();
			_SerializationTypeId = new Guid("714dab32-2e7d-4195-87da-980e49041b54");
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

		private static OnlineAlignerDefinition Create(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (num != 1)
			{
				throw new Exception("Bad version: " + num.ToString());
			}
			string url = reader.ReadString();
			OnlineAlignerDefinition onlineAlignerDefinition = new OnlineAlignerDefinition(url);
			onlineAlignerDefinition.Deserialize(reader);
			return onlineAlignerDefinition;
		}

		protected override void Serialize(BinaryWriter writer)
		{
			writer.Write(1);
			writer.Write(_url);
		}

		protected override void Deserialize(BinaryReader reader)
		{
		}
	}
}
