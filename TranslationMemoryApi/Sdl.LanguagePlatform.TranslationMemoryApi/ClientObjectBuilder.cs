using Sdl.Core.Api.DataAccess;
using System;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	internal class ClientObjectBuilder : ClientObjectBuilder<TranslationProviderServer>
	{
		public ClientObjectBuilder(TranslationProviderServer server)
			: base(server)
		{
		}
	}
	internal class ClientObjectBuilder<TProvider>
	{
		private readonly TProvider _server;

		private readonly Dictionary<ClientObjectKey, object> _clientObjectCache = new Dictionary<ClientObjectKey, object>();

		public TProvider Server => _server;

		public object this[ClientObjectKey key]
		{
			get
			{
				_clientObjectCache.TryGetValue(key, out object value);
				return value;
			}
			set
			{
				_clientObjectCache[key] = value;
			}
		}

		public ClientObjectBuilder(TProvider server)
		{
			_server = server;
		}

		public ClientObjectKey CreateKey(Entity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException("entity");
			}
			if (entity.Id == null)
			{
				throw new ArgumentNullException("entity.Id");
			}
			return new ClientObjectKey
			{
				EntityType = entity.GetType(),
				Id = entity.Id
			};
		}

		public ClientObjectKey CreateKey<TEntity>(Identity id) where TEntity : Entity
		{
			ClientObjectKey clientObjectKey = new ClientObjectKey();
			clientObjectKey.EntityType = typeof(TEntity);
			clientObjectKey.Id = id;
			return clientObjectKey;
		}
	}
}
