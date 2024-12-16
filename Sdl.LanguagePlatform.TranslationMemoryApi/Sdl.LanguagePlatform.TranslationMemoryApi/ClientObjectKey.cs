using Sdl.Core.Api.DataAccess;
using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	internal class ClientObjectKey
	{
		public Type EntityType;

		public Identity Id;

		public override bool Equals(object obj)
		{
			ClientObjectKey clientObjectKey = obj as ClientObjectKey;
			if (clientObjectKey == null)
			{
				return false;
			}
			if (clientObjectKey.Id.Equals(Id))
			{
				return clientObjectKey.EntityType == EntityType;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode() ^ EntityType.GetHashCode();
		}
	}
}
