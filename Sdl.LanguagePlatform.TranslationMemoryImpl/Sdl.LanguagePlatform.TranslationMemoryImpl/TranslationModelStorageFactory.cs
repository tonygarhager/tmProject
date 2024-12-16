using Sdl.LanguagePlatform.Core;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	public class TranslationModelStorageFactory : StorageFactoryBase
	{
		private static readonly Dictionary<string, Func<FileContainer, ITranslationModelStorage>> _fileContainerCreators;

		private static readonly Dictionary<string, Func<DatabaseContainer, ITranslationModelStorage>> _databaseContainerCreators;

		private static readonly Dictionary<string, Func<InMemoryContainer, ITranslationModelStorage>> _memoryContainerCreators;

		public static readonly string ConnectionStringName;

		static TranslationModelStorageFactory()
		{
			_fileContainerCreators = new Dictionary<string, Func<FileContainer, ITranslationModelStorage>>();
			_databaseContainerCreators = new Dictionary<string, Func<DatabaseContainer, ITranslationModelStorage>>();
			_memoryContainerCreators = new Dictionary<string, Func<InMemoryContainer, ITranslationModelStorage>>();
			ConnectionStringName = "Sdl.Core.TranslationModel.ConnectionString";
			RegisterContainerCreators();
		}

		public static void RegisterContainerCreators()
		{
			RegisterContainerCreators(Assembly.GetExecutingAssembly());
		}

		public static void RegisterContainerCreators(Assembly a)
		{
			if (a == null)
			{
				a = Assembly.GetExecutingAssembly();
			}
			Type[] types = a.GetTypes();
			Type typeFromHandle = typeof(ITranslationModelStorage);
			Type[] array = types;
			foreach (Type type in array)
			{
				if (typeFromHandle.IsAssignableFrom(type))
				{
					CallRegister(type);
				}
			}
		}

		private static void CallRegister(Type t)
		{
			MethodInfo method = t.GetMethod("Register", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (method != null)
			{
				method.Invoke(null, null);
			}
		}

		public static void RegisterFileContainerCreator(string id, Func<FileContainer, ITranslationModelStorage> creatorFn)
		{
			if (_fileContainerCreators.ContainsKey(id))
			{
				throw new Exception("Duplicate TranslationModelStorageFactory registration for id: " + id);
			}
			if (creatorFn == null)
			{
				throw new ArgumentNullException("creatorFn");
			}
			_fileContainerCreators.Add(id.ToLowerInvariant(), creatorFn);
		}

		public static void RegisterDatabaseContainerCreator(string id, Func<DatabaseContainer, ITranslationModelStorage> creatorFn)
		{
			if (_databaseContainerCreators.ContainsKey(id))
			{
				throw new Exception("Duplicate TranslationModelStorageFactory registration for id: " + id);
			}
			if (creatorFn == null)
			{
				throw new ArgumentNullException("creatorFn");
			}
			_databaseContainerCreators.Add(id.ToLowerInvariant(), creatorFn);
		}

		public static void RegisterMemoryContainerCreator(string id, Func<InMemoryContainer, ITranslationModelStorage> creatorFn)
		{
			if (_memoryContainerCreators.ContainsKey(id))
			{
				throw new Exception("Duplicate TranslationModelStorageFactory registration for id: " + id);
			}
			if (creatorFn == null)
			{
				throw new ArgumentNullException("creatorFn");
			}
			_memoryContainerCreators.Add(id.ToLowerInvariant(), creatorFn);
		}

		internal static ITranslationModelStorage Create()
		{
			return Create((Dictionary<string, string>)null);
		}

		internal static ITranslationModelStorage Create(Container container)
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
			ITranslationModelStorage result;
			if (container is FileContainer)
			{
				if (_fileContainerCreators.Count == 0)
				{
					throw new Exception("TranslationModelStorageFactory - FileContainer provided but no creators registered");
				}
				FileContainer arg = (FileContainer)container;
				Dictionary<string, Func<FileContainer, ITranslationModelStorage>>.Enumerator enumerator = _fileContainerCreators.GetEnumerator();
				enumerator.MoveNext();
				result = enumerator.Current.Value(arg);
				enumerator.Dispose();
			}
			else
			{
				InMemoryContainer inMemoryContainer = container as InMemoryContainer;
				if (inMemoryContainer == null)
				{
					DatabaseContainer databaseContainer = container as DatabaseContainer;
					if (databaseContainer == null)
					{
						throw new ArgumentException("Unknown or unsupported container type");
					}
					if (databaseContainer.ProviderId == null)
					{
						throw new ArgumentException("Provider ID cannot be \"null\"");
					}
					string text = databaseContainer.ProviderId.ToLowerInvariant();
					if (!_databaseContainerCreators.ContainsKey(text))
					{
						throw new Exception("TranslationModelStorageFactory - unregistered provider type: " + text);
					}
					result = _databaseContainerCreators[text](databaseContainer);
				}
				else
				{
					InMemoryContainer arg2 = inMemoryContainer;
					if (_memoryContainerCreators.Count == 0)
					{
						throw new Exception("TranslationModelStorageFactory - InMemoryContainer provided but no creators registered");
					}
					Dictionary<string, Func<InMemoryContainer, ITranslationModelStorage>>.Enumerator enumerator2 = _memoryContainerCreators.GetEnumerator();
					enumerator2.MoveNext();
					result = enumerator2.Current.Value(arg2);
					enumerator2.Dispose();
				}
			}
			return result;
		}

		internal static ITranslationModelStorage Create(Dictionary<string, string> options)
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
