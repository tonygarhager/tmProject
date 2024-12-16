using Sdl.LanguagePlatform.Core;
using System;
using System.Data.SQLite;
using System.IO;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	public class SqliteStorageUtils
	{
		private const bool _UsePooling = false;

		public static bool UsePooling => false;

		public static string BuildConnectionString(FileContainer container)
		{
			return BuildConnectionString(container.Path, container.CreateIfNotExists);
		}

		private static string BuildConnectionString(string file, bool create)
		{
			if (string.IsNullOrEmpty(file))
			{
				throw new ArgumentException("No data file specified in connection string");
			}
			SQLiteConnectionStringBuilder sQLiteConnectionStringBuilder = new SQLiteConnectionStringBuilder
			{
				SyncMode = SynchronizationModes.Off,
				DataSource = file.Replace('\\', '/'),
				Enlist = false,
				Pooling = false,
				ForeignKeys = true
			};
			FileInfo fileInfo = new FileInfo(file);
			if (!fileInfo.Exists || fileInfo.Length == 0L)
			{
				if (!create)
				{
					throw new StorageException(ErrorCode.StorageDataFileNotFound, file);
				}
				sQLiteConnectionStringBuilder.PageSize = 8192;
				SQLiteConnection.CreateFile(file);
			}
			else if (fileInfo.IsReadOnly)
			{
				sQLiteConnectionStringBuilder.ReadOnly = true;
			}
			return sQLiteConnectionStringBuilder.ToString();
		}

		public static string BuildConnectionString(DatabaseContainer container)
		{
			if (!container.ProviderId.Trim().ToLowerInvariant().Equals("system.data.sqlite", StringComparison.InvariantCultureIgnoreCase))
			{
				throw new ArgumentException("container.ProviderId is invalid for this storage implementation");
			}
			return container.ConnectionString ?? BuildConnectionString(container.Database, create: false);
		}
	}
}
