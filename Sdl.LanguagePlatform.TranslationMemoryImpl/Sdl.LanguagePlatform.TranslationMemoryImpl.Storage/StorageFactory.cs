using Sdl.LanguagePlatform.Core;
using System;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl.Storage
{
	public class StorageFactory : StorageFactoryBase
	{
		public static readonly string ConnectionStringName = "Sdl.LanguagePlatform.TranslationMemory.ConnectionString";

		internal static IStorage Create()
		{
			return Create((Dictionary<string, string>)null);
		}

		internal static IStorage Create(Container container)
		{
			if (container == null)
			{
				throw new ArgumentNullException();
			}
			NamedConnectionStringContainer namedConnectionStringContainer = container as NamedConnectionStringContainer;
			if (namedConnectionStringContainer != null)
			{
				container = ContainerFactory.GetContainerFromNamedConnectionString(namedConnectionStringContainer.ConnectionStringName);
			}
			if (!(container is InMemoryContainer))
			{
				FileContainer fileContainer = container as FileContainer;
				if (fileContainer == null)
				{
					DatabaseContainer databaseContainer = container as DatabaseContainer;
					if (databaseContainer != null)
					{
						if (databaseContainer.ProviderId == null)
						{
							throw new ArgumentException("Provider ID cannot be \"null\"");
						}
						string text = databaseContainer.ProviderId.ToLowerInvariant();
						switch (text)
						{
						case "system.data.sqlclient":
							return SqlStorage.Create(databaseContainer);
						case "oracle.dataaccess.client":
							throw new NotSupportedException();
						case "system.data.sqlite":
							return new SqliteStorage(databaseContainer);
						default:
							throw new LanguagePlatformException(ErrorCode.ConfigurationUnknownProviderType, text);
						}
					}
					throw new ArgumentException("Unknown or unsupported container type");
				}
				return new SqliteStorage(fileContainer);
			}
			return new InMemoryStorage();
		}

		internal static IStorage Create(Dictionary<string, string> options)
		{
			Container containerFromOptions;
			if (options != null)
			{
				containerFromOptions = ContainerFactory.GetContainerFromOptions(options);
				if (containerFromOptions != null)
				{
					return Create(containerFromOptions);
				}
			}
			containerFromOptions = ContainerFactory.GetContainerFromNamedConnectionString(ConnectionStringName);
			if (containerFromOptions != null)
			{
				return Create(containerFromOptions);
			}
			throw new LanguagePlatformException(ErrorCode.ConfigurationConnectionStringNotFound);
		}
	}
}
