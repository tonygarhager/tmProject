using Sdl.Core.FineGrainedAlignment;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Resources;
using Sdl.LanguagePlatform.Lingua.Index;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryImpl.FGA;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl.Storage
{
	internal class SqliteStorage : DbStorage, IAlignableStorage, IStorage, IDisposable
	{
		private class IndexCache
		{
			public readonly bool IsLocalFile;

			public readonly bool IsReadOnly;

			public readonly InMemoryFuzzyIndex[] FuzzyIndexCaches;

			public readonly int[] LoadTicks;

			public Dictionary<long, int> HashCache;

			public IndexCache(bool isLocalFile, bool isReadOnly)
			{
				FuzzyIndexCaches = new InMemoryFuzzyIndex[10];
				LoadTicks = new int[10];
				IsLocalFile = isLocalFile;
				IsReadOnly = isReadOnly;
			}
		}

		private const string ResourceWriteCountKey = "RESWRCNT";

		private const string LastAnalyzeCountKey = "LAST_ANALYZE";

		private const string VersionCreatedKey = "VERSION_CREATED";

		private const string ProductTelemetricsTargetKey = "ProductTelemetricsTarget";

		private const string ProductTelemetricsTargetExceptionValue = "Studio4";

		private const string CreateStrictHashingFileBasedTMsKey = "CreateStrictHashingFileBasedTMs";

		private const int ReloadDelta = 1800000;

		private const string CurrentVersion = "8.06";

		private const string CanReportReindexRequiredVersion = "8.09";

		private const string CanChooseTextContextMatchTypeVersion = "8.10";

		private const string CreatedVersion = "8.10";

		private static readonly Dictionary<string, IndexCache> IndexCaches;

		private const int AllocatedIndexCaches = 10;

		private IndexCache _currentIndexCache;

		private bool _versionChecked;

		private bool _isReadOnly;

		private bool _serializesTokens;

		private bool _canReportReindexRequired;

		private bool _canChooseTextContextMatchType;

		private SHA1 _hasher;

		private static bool? _applyFga;

		private static readonly object Locker;

		private static readonly Dictionary<string, Dictionary<DateTime, int>> AlignedPredatedTuCountCache;

		public bool DeleteTmRequiresEmptyTm => true;

		public bool IsReadOnly => _currentIndexCache?.IsReadOnly ?? _isReadOnly;

		internal bool CanChooseTextContextMatchType
		{
			get
			{
				CheckVersion();
				return _canChooseTextContextMatchType;
			}
		}

		protected override bool CanReportReindexRequired
		{
			get
			{
				CheckVersion();
				return _canReportReindexRequired;
			}
		}

		public static bool UseFileBasedFga()
		{
			if (_applyFga.HasValue)
			{
				return _applyFga.Value;
			}
			lock (Locker)
			{
				if (_applyFga.HasValue)
				{
					return _applyFga.Value;
				}
				_applyFga = true;
				string text = ConfigurationManager.AppSettings["ProductTelemetricsTarget"];
				if (text != null)
				{
					_applyFga = (string.CompareOrdinal(text, "Studio4") != 0);
				}
				return _applyFga.Value;
			}
		}

		static SqliteStorage()
		{
			Locker = new object();
			AlignedPredatedTuCountCache = new Dictionary<string, Dictionary<DateTime, int>>();
			IndexCaches = new Dictionary<string, IndexCache>(StringComparer.OrdinalIgnoreCase);
			TranslationModelStorageFactory.RegisterContainerCreators(Assembly.GetExecutingAssembly());
		}

		private bool IsLocalFile(string dataFile)
		{
			if (string.IsNullOrEmpty(dataFile))
			{
				return false;
			}
			try
			{
				FileInfo fileInfo = new FileInfo(dataFile);
				_isReadOnly = fileInfo.IsReadOnly;
				DriveInfo driveInfo = new DriveInfo(Path.GetPathRoot(fileInfo.FullName));
				return driveInfo.DriveType != DriveType.Network && driveInfo.DriveType != DriveType.Unknown;
			}
			catch
			{
				return false;
			}
		}

		public SqliteStorage(FileContainer container)
		{
			string connectionString = SqliteStorageUtils.BuildConnectionString(container);
			CommonConstruct(connectionString);
		}

		public SqliteStorage(DatabaseContainer container)
		{
			if (container == null)
			{
				throw new ArgumentNullException();
			}
			if (container.ProviderId == null)
			{
				throw new ArgumentException("container.ProviderId is null");
			}
			string connectionString = SqliteStorageUtils.BuildConnectionString(container);
			CommonConstruct(connectionString);
		}

		public SqliteStorage(string connectionString)
		{
			CommonConstruct(connectionString);
		}

		private void CommonConstruct(string connectionString)
		{
			_hasher = null;
			SQLiteConnection sQLiteConnection = (SQLiteConnection)(_conn = new SQLiteConnection(connectionString));
			_KeepConnection = SqliteStorageUtils.UsePooling;
			lock (IndexCaches)
			{
				if (!IndexCaches.TryGetValue(connectionString, out _currentIndexCache))
				{
					_conn.Open();
					bool isLocalFile = IsLocalFile(sQLiteConnection.FileName);
					_currentIndexCache = new IndexCache(isLocalFile, _isReadOnly);
					IndexCaches.Add(connectionString, _currentIndexCache);
				}
			}
		}

		private static string GetPasswordName(Permission permission)
		{
			switch (permission)
			{
			case Permission.ReadOnly:
				return "pwdro";
			case Permission.ReadWrite:
				return "pwdrw";
			case Permission.Maintenance:
				return "pwdmnt";
			case Permission.Administrator:
				return "pwdadm";
			default:
				throw new NotSupportedException("Invalid permission level");
			}
		}

		public void SetPassword(PersistentObjectToken tmId, Permission permission, string pwd)
		{
			string passwordName = GetPasswordName(permission);
			pwd = GetStringHash(pwd);
			if (tmId == null || tmId.Id == 0)
			{
				SetParameter(passwordName, pwd ?? string.Empty);
			}
			else
			{
				SetParameter(tmId.Id, passwordName, pwd ?? string.Empty);
			}
		}

		private string GetStringHash(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return s;
			}
			if (_hasher == null)
			{
				_hasher = SHA1.Create();
			}
			return BitConverter.ToString(_hasher.ComputeHash(Encoding.UTF8.GetBytes(s)));
		}

		public bool HasPassword(PersistentObjectToken tmId, Permission permission)
		{
			return !string.IsNullOrEmpty(GetPassword(tmId, permission));
		}

		private string GetPassword(PersistentObjectToken tmId, Permission permission)
		{
			string passwordName = GetPasswordName(permission);
			if (tmId == null || tmId.Id == 0)
			{
				return GetParameter(passwordName);
			}
			return GetParameter(tmId.Id, passwordName);
		}

		public Permission GetPasswordPermissionLevel(PersistentObjectToken resourceId, string suppliedPassword)
		{
			if (suppliedPassword == null)
			{
				suppliedPassword = string.Empty;
			}
			string stringHash = GetStringHash(suppliedPassword);
			Permission[] obj = new Permission[4]
			{
				Permission.Administrator,
				Permission.Maintenance,
				Permission.ReadWrite,
				Permission.ReadOnly
			};
			bool flag = false;
			Permission[] array = obj;
			foreach (Permission permission in array)
			{
				string text = GetPassword(resourceId, permission) ?? string.Empty;
				if (!flag)
				{
					flag = !string.IsNullOrEmpty(text);
				}
				if (string.Equals(text, stringHash))
				{
					return permission;
				}
			}
			if (!flag)
			{
				return Permission.Administrator;
			}
			return Permission.None;
		}

		private SQLiteCommand CreateCommand(string cmdText, bool beginTransaction)
		{
			SQLiteCommand sQLiteCommand = string.IsNullOrEmpty(cmdText) ? new SQLiteCommand() : new SQLiteCommand(cmdText);
			InitializeCommand(sQLiteCommand, beginTransaction);
			return sQLiteCommand;
		}

		private void ExecuteSchemaCommand(string schemaName, bool ignoreErrors)
		{
			string[] array = ReadSchemaCommands(schemaName, ";", this);
			using (SQLiteCommand sQLiteCommand = CreateCommand(null, beginTransaction: true))
			{
				string[] array2 = array;
				foreach (string text in array2)
				{
					sQLiteCommand.CommandText = text.Trim('\r', '\n', '\t');
					try
					{
						sQLiteCommand.ExecuteNonQuery();
					}
					catch (SQLiteException)
					{
						if (!ignoreErrors)
						{
							throw;
						}
					}
				}
			}
		}

		private void CheckVersion()
		{
			if (_versionChecked)
			{
				return;
			}
			string text = GetVersion();
			string createdVersion = GetCreatedVersion();
			_versionChecked = true;
			string parameter = GetParameter("TokenDataVersion");
			_serializesTokens = false;
			if (!string.IsNullOrEmpty(parameter))
			{
				if (!int.TryParse(parameter, out int result))
				{
					throw new Exception("Invalid parameter value '" + parameter + "' for parameter TokenDataVersion");
				}
				if (result == 0 || result > 1)
				{
					throw new Exception("TokenDataVersion value is unsupported: " + result.ToString());
				}
				_serializesTokens = true;
			}
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			if (!string.IsNullOrEmpty(createdVersion))
			{
				int num = StringComparer.OrdinalIgnoreCase.Compare("8.09", createdVersion);
				_canReportReindexRequired = (num <= 0);
				num = StringComparer.OrdinalIgnoreCase.Compare("8.10", createdVersion);
				_canChooseTextContextMatchType = (num <= 0);
			}
			int num2 = StringComparer.OrdinalIgnoreCase.Compare("8.06", text);
			if (num2 < 0)
			{
				throw new LanguagePlatformException(ErrorCode.StorageVersionDataNewer);
			}
			if (num2 == 0)
			{
				return;
			}
			bool flag = false;
			if (text.Equals("8.03"))
			{
				using (SQLiteCommand sQLiteCommand = CreateCommand("CREATE TABLE fuzzy_data(\r\n\ttranslation_memory_id INT NOT NULL CONSTRAINT FK_fi1_tm REFERENCES translation_memories(id) ON DELETE CASCADE,\r\n\ttranslation_unit_id INT NOT NULL,\r\n\tfi1 TEXT,\r\n\tfi2 TEXT,\r\n\tfi4 TEXT,\r\nCONSTRAINT PK_fi1 PRIMARY KEY (\r\n\ttranslation_memory_id, translation_unit_id\r\n)\r\n);", beginTransaction: true))
				{
					sQLiteCommand.ExecuteNonQuery();
				}
				using (SQLiteCommand sQLiteCommand2 = CreateCommand("INSERT INTO fuzzy_data(translation_memory_id, translation_unit_id, fi1, fi2, fi4)\r\nSELECT x1.translation_memory_id, x1.translation_unit_id, x1.feature fi1, x2.feature fi2, x4.feature fi4\r\nFROM fuzzy_index1 x1\r\n\tINNER JOIN fuzzy_index2 x2 ON (x1.translation_memory_id = x2.translation_memory_id AND \r\n\t\t\t\t   x1.translation_unit_id = x2.translation_unit_id) \r\n\tINNER JOIN fuzzy_index4 x4 ON (x2.translation_memory_id = x4.translation_memory_id AND \r\n\t\t\t\t   x2.translation_unit_id = x4.translation_unit_id)\r\n", beginTransaction: true))
				{
					sQLiteCommand2.ExecuteNonQuery();
				}
				string[] array = new string[3]
				{
					"DROP TABLE fuzzy_index1",
					"DROP TABLE fuzzy_index2",
					"DROP TABLE fuzzy_index4"
				};
				foreach (string cmdText in array)
				{
					using (SQLiteCommand sQLiteCommand3 = CreateCommand(cmdText, beginTransaction: true))
					{
						sQLiteCommand3.ExecuteNonQuery();
					}
				}
				SetParameter("VERSION", "8.04");
				text = "8.04";
				flag = true;
				CommitTransaction();
				array = new string[1]
				{
					"VACUUM"
				};
				foreach (string cmdText2 in array)
				{
					using (SQLiteCommand sQLiteCommand4 = CreateCommand(cmdText2, beginTransaction: false))
					{
						sQLiteCommand4.ExecuteNonQuery();
					}
				}
			}
			if (text.Equals("8.04"))
			{
				using (SQLiteCommand sQLiteCommand5 = CreateCommand("ALTER TABLE fuzzy_data ADD COLUMN fi8 TEXT", beginTransaction: true))
				{
					sQLiteCommand5.ExecuteNonQuery();
				}
				SetParameter("VERSION", "8.05");
				text = "8.05";
				flag = true;
			}
			if (text.Equals("8.05"))
			{
				using (SQLiteCommand sQLiteCommand6 = CreateCommand("ALTER TABLE translation_memories ADD COLUMN flags INT NOT NULL DEFAULT 0", beginTransaction: true))
				{
					sQLiteCommand6.ExecuteNonQuery();
				}
				using (SQLiteCommand sQLiteCommand7 = CreateCommand("ALTER TABLE translation_memories ADD COLUMN tucount INT NOT NULL DEFAULT 0", beginTransaction: true))
				{
					sQLiteCommand7.ExecuteNonQuery();
				}
				UpdateTuCounts();
				SetParameter("VERSION", "8.06");
				flag = true;
			}
			if (!flag)
			{
				throw new LanguagePlatformException(ErrorCode.StorageVersionDataOutdated);
			}
			_versionChecked = true;
		}

		private void UpdateTuCounts()
		{
			using (SQLiteCommand sQLiteCommand = CreateCommand("SELECT translation_memory_id, count(*) FROM translation_units GROUP BY translation_memory_id", beginTransaction: false))
			{
				using (SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader())
				{
					while (sQLiteDataReader.Read())
					{
						int num = Convert.ToInt32(sQLiteDataReader[0]);
						int num2 = Convert.ToInt32(sQLiteDataReader[1]);
						using (SQLiteCommand sQLiteCommand2 = CreateCommand("UPDATE translation_memories SET tucount = @tucount WHERE id = @tmid", beginTransaction: true))
						{
							sQLiteCommand2.Parameters.Add("@tmid", DbType.Int32).Value = num;
							sQLiteCommand2.Parameters.Add("@tucount", DbType.Int32).Value = num2;
							sQLiteCommand2.ExecuteNonQuery();
						}
					}
				}
			}
		}

		public bool HasFuzzyCacheNonEmpty(Container container)
		{
			string key = SqliteStorageUtils.BuildConnectionString(container as FileContainer);
			if (!IndexCaches.TryGetValue(key, out IndexCache value))
			{
				return false;
			}
			if (value.FuzzyIndexCaches.Length > 1 && value.FuzzyIndexCaches[1] != null)
			{
				return value.FuzzyIndexCaches[1].Count > 0;
			}
			return false;
		}

		public bool? GetReindexRequired(int tmId, long currentSignatureHash)
		{
			CheckVersion();
			if (!_canReportReindexRequired)
			{
				return null;
			}
			using (SQLiteCommand sQLiteCommand = CreateCommand("SELECT 1 FROM translation_units WHERE (tokenization_sig_hash is null or tokenization_sig_hash <> @sig_hash)", beginTransaction: false))
			{
				sQLiteCommand.Parameters.Add("@tm_id", DbType.Int32).Value = tmId;
				sQLiteCommand.Parameters.Add("@sig_hash", DbType.Int64).Value = currentSignatureHash;
				return GetReindexRequired(sQLiteCommand);
			}
		}

		public List<TranslationUnit> GetTusForReindex(int tmId, int startAfter, int count, long currentSigHash)
		{
			CheckVersion();
			if (!_canReportReindexRequired)
			{
				return new List<TranslationUnit>();
			}
			string cmdText = "SELECT id, guid, source_hash, source_segment, target_hash, target_segment " + (SerializesTokens() ? ", source_token_data, target_token_data " : ", null, null ") + ", null, null, insert_date,\r\n                                                0, null, null \r\n                                              FROM translation_units \r\n                                              WHERE id > @start_after and (tokenization_sig_hash is null or tokenization_sig_hash <> @sighash)\r\n                                              ORDER BY id ASC LIMIT @limit";
			using (SQLiteCommand sQLiteCommand = CreateCommand(cmdText, beginTransaction: false))
			{
				sQLiteCommand.Parameters.Add("@tm_id", DbType.Int32).Value = tmId;
				sQLiteCommand.Parameters.Add("@start_after", DbType.Int32).Value = startAfter;
				sQLiteCommand.Parameters.Add("@limit", DbType.Int32).Value = count;
				sQLiteCommand.Parameters.Add("@sighash", DbType.Int64).Value = currentSigHash;
				return GetTuRange(sQLiteCommand);
			}
		}

		public int GetTuCountForReindex(int tmId, long currentSignatureHash)
		{
			CheckVersion();
			if (!_canReportReindexRequired)
			{
				return -1;
			}
			using (SQLiteCommand sQLiteCommand = CreateCommand("SELECT count(*) FROM translation_units WHERE (tokenization_sig_hash is null or tokenization_sig_hash <> @hash)", beginTransaction: false))
			{
				sQLiteCommand.Parameters.Add("@tm_id", DbType.Int32).Value = tmId;
				sQLiteCommand.Parameters.Add("@hash", DbType.Int64).Value = currentSignatureHash;
				return GetTuCountForReindex(sQLiteCommand);
			}
		}

		public void ClearFuzzyCache(Container container)
		{
			if (container != null)
			{
				string key = SqliteStorageUtils.BuildConnectionString(container as FileContainer);
				if (IndexCaches.TryGetValue(key, out IndexCache value) && value != null)
				{
					ClearAllFuzzyIndexCaches(value);
				}
			}
		}

		public List<PersistentObjectToken> DeleteTusFiltered(int tmId, FilterExpression filter, int startAfter, int count, bool forward, TextContextMatchType textContextMatchType, bool deleteOrphanContexts)
		{
			throw new NotImplementedException();
		}

		public void ClearCache()
		{
			if (_currentIndexCache != null)
			{
				ClearAllFuzzyIndexCaches(_currentIndexCache);
			}
		}

		public void Flush()
		{
		}

		public void CreateSchema()
		{
			ClearCache();
			DropSchema();
			ExecuteSchemaCommand("Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.SQL.CreateSchemaSQLite.sql", ignoreErrors: false);
			SetParameter("TokenDataVersion", 1.ToString());
			SetParameter("AlignmentDataVersion", 1.ToString());
			ClearCache();
		}

		public void DropSchema()
		{
			ExecuteSchemaCommand("Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.SQL.DropSchemaSQLite.sql", ignoreErrors: true);
			ClearCache();
		}

		public bool SchemaExists()
		{
			if (_conn.State != ConnectionState.Open)
			{
				_conn.Open();
			}
			return _conn.GetSchema("Tables").Select("Table_Name = 'translation_memories'").Length == 1;
		}

		public string GetParameter(string name)
		{
			using (SQLiteCommand sQLiteCommand = CreateCommand("SELECT value FROM parameters WHERE translation_memory_id IS NULL AND name = @name", beginTransaction: false))
			{
				sQLiteCommand.Parameters.Add("@name", DbType.String).Value = name;
				string result = string.Empty;
				try
				{
					result = (string)sQLiteCommand.ExecuteScalar();
				}
				catch
				{
				}
				return result;
			}
		}

		public string GetParameter(int tmId, string name)
		{
			using (SQLiteCommand sQLiteCommand = CreateCommand("SELECT value FROM parameters WHERE translation_memory_id = @tmid AND name = @name", beginTransaction: false))
			{
				sQLiteCommand.Parameters.Add("@name", DbType.String).Value = name;
				sQLiteCommand.Parameters.Add("@tmid", DbType.Int32).Value = tmId;
				string result = string.Empty;
				try
				{
					result = (string)sQLiteCommand.ExecuteScalar();
				}
				catch
				{
				}
				return result;
			}
		}

		public void SetParameter(string name, string value)
		{
			using (SQLiteCommand sQLiteCommand = CreateCommand("DELETE FROM parameters WHERE translation_memory_id IS NULL AND name = @name", beginTransaction: true))
			{
				sQLiteCommand.Parameters.Add("@name", DbType.String).Value = name;
				sQLiteCommand.ExecuteNonQuery();
			}
			if (value != null)
			{
				using (SQLiteCommand sQLiteCommand2 = CreateCommand("INSERT INTO parameters (name, value) VALUES (@name, @value)", beginTransaction: true))
				{
					sQLiteCommand2.Parameters.Add("@name", DbType.String).Value = name;
					sQLiteCommand2.Parameters.Add("@value", DbType.String).Value = value;
					sQLiteCommand2.ExecuteNonQuery();
				}
			}
		}

		public void SetParameter(int tmId, string name, string value)
		{
			using (SQLiteCommand sQLiteCommand = CreateCommand("DELETE FROM parameters WHERE translation_memory_id = @tmid AND name = @name", beginTransaction: true))
			{
				sQLiteCommand.Parameters.Add("@name", DbType.String).Value = name;
				sQLiteCommand.Parameters.Add("@tmid", DbType.Int32).Value = tmId;
				sQLiteCommand.ExecuteNonQuery();
			}
			if (value != null)
			{
				using (SQLiteCommand sQLiteCommand2 = CreateCommand("INSERT INTO parameters (translation_memory_id, name, value) VALUES (@tmid, @name, @value)", beginTransaction: true))
				{
					sQLiteCommand2.Parameters.Add("@name", DbType.String).Value = name;
					sQLiteCommand2.Parameters.Add("@value", DbType.String).Value = value;
					sQLiteCommand2.Parameters.Add("@tmid", DbType.Int32).Value = tmId;
					sQLiteCommand2.ExecuteNonQuery();
				}
			}
		}

		public override string GetVersion()
		{
			return GetParameter("VERSION");
		}

		public string GetCreatedVersion()
		{
			return GetParameter("VERSION_CREATED");
		}

		public override bool HasGuids()
		{
			return true;
		}

		public override bool HasFlags()
		{
			return true;
		}

		protected override DateTime GetDateTime(DbDataReader reader, int column)
		{
			return Utils.StringToDate(reader.GetString(column));
		}

		private bool SerializesTokens()
		{
			CheckVersion();
			return _serializesTokens;
		}

		public void Optimize()
		{
			Optimize(alwaysAnalyze: false);
		}

		private void Optimize(bool alwaysAnalyze)
		{
			if (IsReadOnly)
			{
				return;
			}
			bool flag = alwaysAnalyze;
			int num = 0;
			if (!flag)
			{
				string parameter = GetParameter("LAST_ANALYZE");
				num = GetMaxRowId();
				int result;
				if (string.IsNullOrEmpty(parameter))
				{
					flag = true;
				}
				else if (!int.TryParse(parameter, NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
				{
					flag = true;
				}
				else if ((double)num > (double)result * 1.5)
				{
					flag = true;
				}
			}
			if (flag)
			{
				using (SQLiteCommand sQLiteCommand = CreateCommand("ANALYZE", beginTransaction: true))
				{
					sQLiteCommand.ExecuteNonQuery();
				}
				SetParameter("LAST_ANALYZE", num.ToString(CultureInfo.InvariantCulture));
			}
		}

		public int ResolveResourceGuid(Guid guid)
		{
			throw new NotImplementedException();
		}

		public int ResolveTranslationMemoryGuid(Guid guid)
		{
			throw new NotImplementedException();
		}

		public int ResolveTranslationUnitGuid(int tmId, Guid guid)
		{
			throw new NotImplementedException();
		}

		public int ResolveAttributeGuid(int tmId, Guid guid)
		{
			throw new NotImplementedException();
		}

		public int ResolvePicklistValueGuid(int tmId, Guid guid)
		{
			throw new NotImplementedException();
		}

		public List<Resource> GetResources(bool includeData)
		{
			CheckVersion();
			string cmdText = includeData ? "SELECT id, guid, type, language, data FROM resources" : "SELECT id, guid, type, language, NULL FROM resources";
			using (SQLiteCommand cmd = CreateCommand(cmdText, beginTransaction: false))
			{
				return GetResources(cmd, includeData);
			}
		}

		public int GetResourcesWriteCount()
		{
			string parameter = GetParameter("RESWRCNT");
			if (parameter == null || !int.TryParse(parameter, out int result))
			{
				return 0;
			}
			return result;
		}

		private void IncResourcesWriteCount()
		{
			SetParameter("RESWRCNT", (GetResourcesWriteCount() + 1).ToString(CultureInfo.InvariantCulture));
		}

		private Resource GetResource(LanguageResourceType type, string language, bool includeData)
		{
			CheckVersion();
			string cmdText = (language == null) ? "SELECT id, guid, type, language, data FROM resources WHERE type = @type AND language IS NULL" : "SELECT id, guid, type, language, data FROM resources WHERE type = @type AND language = @language";
			using (SQLiteCommand sQLiteCommand = CreateCommand(cmdText, beginTransaction: false))
			{
				sQLiteCommand.Parameters.Add("@type", DbType.Int32).Value = type;
				sQLiteCommand.Parameters.Add("@language", DbType.String).Value = language;
				return GetResource(sQLiteCommand, includeData);
			}
		}

		public List<Resource> GetResources(int tmId, bool includeData)
		{
			CheckVersion();
			using (SQLiteCommand sQLiteCommand = CreateCommand("SELECT r.id, r.guid, r.type, r.language, r.data \r\n\t\t\t\tFROM resources r INNER JOIN tm_resources tr ON r.id = tr.resource_id\r\n\t\t\t\tWHERE tr.tm_id = @tm_id", beginTransaction: false))
			{
				sQLiteCommand.Parameters.Add("@tm_id", DbType.Int32).Value = tmId;
				return GetResources(sQLiteCommand, includeData);
			}
		}

		public Resource GetResource(int key, bool includeData)
		{
			CheckVersion();
			string cmdText = includeData ? "SELECT id, guid, type, language, data FROM resources WHERE id = @id" : "SELECT id, guid, type, language, NULL FROM resources WHERE id = @id";
			using (SQLiteCommand sQLiteCommand = CreateCommand(cmdText, beginTransaction: false))
			{
				sQLiteCommand.Parameters.Add("@id", DbType.Int32).Value = key;
				return GetResource(sQLiteCommand, includeData);
			}
		}

		public bool AddResource(Resource resource)
		{
			CheckVersion();
			if (resource.Language == null && GetResource(resource.Type, null, includeData: false) != null)
			{
				return false;
			}
			using (SQLiteCommand sQLiteCommand = CreateCommand("INSERT INTO resources(type, language, data, guid) VALUES(@type, @language, @data, @guid)", beginTransaction: true))
			{
				sQLiteCommand.Parameters.Add("@type", DbType.Int32).Value = (int)resource.Type;
				sQLiteCommand.Parameters.Add("@data", DbType.Binary).Value = resource.Data;
				sQLiteCommand.Parameters.Add("@guid", DbType.Guid).Value = resource.Guid;
				sQLiteCommand.Parameters.Add("@language", DbType.String).Value = (string.IsNullOrEmpty(resource.Language) ? ((IConvertible)DBNull.Value) : ((IConvertible)resource.Language));
				try
				{
					sQLiteCommand.ExecuteNonQuery();
				}
				catch (SQLiteException ex)
				{
					if (ex.ErrorCode != 19)
					{
						throw;
					}
					return false;
				}
				sQLiteCommand.Parameters.Clear();
				sQLiteCommand.CommandText = "SELECT last_insert_rowid()";
				resource.Id = Convert.ToInt32(sQLiteCommand.ExecuteScalar());
			}
			IncResourcesWriteCount();
			return true;
		}

		public bool DeleteResource(int key)
		{
			using (SQLiteCommand sQLiteCommand = CreateCommand("DELETE FROM tm_resources WHERE resource_id = @id", beginTransaction: true))
			{
				sQLiteCommand.Parameters.Add("@id", DbType.Int32).Value = key;
				sQLiteCommand.ExecuteNonQuery();
			}
			bool flag;
			using (SQLiteCommand sQLiteCommand2 = CreateCommand("DELETE FROM resources WHERE id = @id", beginTransaction: true))
			{
				sQLiteCommand2.Parameters.Add("@id", DbType.Int32).Value = key;
				flag = (sQLiteCommand2.ExecuteNonQuery() > 0);
			}
			if (flag)
			{
				IncResourcesWriteCount();
			}
			return flag;
		}

		public bool UpdateResource(Resource resource)
		{
			bool flag;
			using (SQLiteCommand sQLiteCommand = CreateCommand("UPDATE resources SET data = @data WHERE id = @id", beginTransaction: true))
			{
				sQLiteCommand.Parameters.Add("@id", DbType.Int32).Value = resource.Id;
				sQLiteCommand.Parameters.Add("@data", DbType.Binary).Value = resource.Data;
				flag = (sQLiteCommand.ExecuteNonQuery() > 0);
			}
			if (flag)
			{
				IncResourcesWriteCount();
			}
			return flag;
		}

		private string GetFgaColspec()
		{
			string result = ", " + 0.ToString();
			if (SupportsAlignmentData())
			{
				result = ", fga_support";
			}
			return result;
		}

		private string GetCmColspec()
		{
			string result = ", 0, " + 1.ToString() + ", 0";
			if (_canChooseTextContextMatchType)
			{
				result = ", data_version, text_context_match_type, id_context_match";
			}
			return result;
		}

		public List<TranslationMemory> GetTMsByResourceId(int resourceId)
		{
			CheckVersion();
			using (SQLiteCommand sQLiteCommand = CreateCommand("SELECT t.id, t.guid, t.name, t.source_language, t.target_language, t.copyright, t.description, t.settings, \r\n\t\t\t\tt.creation_user, t.creation_date, t.expiration_date, t.fuzzy_indexes, t.last_recompute_date, t.last_recompute_size" + GetFgaColspec() + GetCmColspec() + " FROM translation_memories t\r\n\t\t\t\tINNER JOIN tm_resources tr ON t.id = tr.tm_id\r\n\t\t\t\tWHERE tr.resource_id = @resource_id", beginTransaction: false))
			{
				sQLiteCommand.Parameters.Add("@resource_id", DbType.Int32).Value = resourceId;
				return GetTms(sQLiteCommand);
			}
		}

		public bool AttachTmResource(int tmId, int resourceId)
		{
			bool flag;
			using (SQLiteCommand sQLiteCommand = CreateCommand("INSERT INTO tm_resources(tm_id, resource_id) VALUES(@tm_id, @resource_id)", beginTransaction: true))
			{
				sQLiteCommand.Parameters.Add("@tm_id", DbType.Int32).Value = tmId;
				sQLiteCommand.Parameters.Add("@resource_id", DbType.Int32).Value = resourceId;
				try
				{
					flag = (sQLiteCommand.ExecuteNonQuery() > 0);
				}
				catch (SQLiteException ex)
				{
					if (ex.ErrorCode != 19)
					{
						throw;
					}
					return false;
				}
			}
			if (flag)
			{
				IncResourcesWriteCount();
			}
			return flag;
		}

		public bool DetachTmResource(int tmId, int resourceId)
		{
			bool flag;
			using (SQLiteCommand sQLiteCommand = CreateCommand("DELETE FROM tm_resources WHERE tm_id = @tm_id AND resource_id = @resource_id", beginTransaction: true))
			{
				sQLiteCommand.Parameters.Add("@tm_id", DbType.Int32).Value = tmId;
				sQLiteCommand.Parameters.Add("@resource_id", DbType.Int32).Value = resourceId;
				flag = (sQLiteCommand.ExecuteNonQuery() > 0);
			}
			if (flag)
			{
				IncResourcesWriteCount();
			}
			return flag;
		}

		public List<TranslationMemory> GetTms()
		{
			CheckVersion();
			using (SQLiteCommand cmd = CreateCommand("SELECT id, guid, name, source_language, target_language, copyright, description, settings, \r\n\t\t\t\tcreation_user, creation_date, expiration_date, fuzzy_indexes, last_recompute_date, last_recompute_size" + GetFgaColspec() + GetCmColspec() + " FROM translation_memories", beginTransaction: false))
			{
				return GetTms(cmd);
			}
		}

		public TranslationMemory GetTm(int id)
		{
			CheckVersion();
			using (SQLiteCommand sQLiteCommand = CreateCommand("SELECT id, guid, name, source_language, target_language, copyright, description, settings, \r\n\t\t\t\tcreation_user, creation_date, expiration_date, fuzzy_indexes, last_recompute_date, last_recompute_size " + GetFgaColspec() + GetCmColspec() + " FROM translation_memories WHERE id = @id", beginTransaction: false))
			{
				sQLiteCommand.Parameters.Add("@id", DbType.Int32).Value = id;
				return GetTm(sQLiteCommand);
			}
		}

		public TranslationMemory GetTm(string name)
		{
			CheckVersion();
			using (SQLiteCommand sQLiteCommand = CreateCommand("SELECT id, guid, name, source_language, target_language, copyright, description, settings, \r\n\t\t\t\tcreation_user, creation_date, expiration_date, fuzzy_indexes, last_recompute_date, last_recompute_size" + GetFgaColspec() + GetCmColspec() + " FROM translation_memories WHERE name = @name", beginTransaction: false))
			{
				sQLiteCommand.Parameters.Add("@name", DbType.String).Value = name;
				return GetTm(sQLiteCommand);
			}
		}

		public bool AddTm(TranslationMemory tm)
		{
			CheckVersion();
			if (tm.TextContextMatchType == (TextContextMatchType)0)
			{
				throw new Exception("TextContextMatchType must be specified");
			}
			BooleanSettingsWrapper booleanSettingsWrapper = new BooleanSettingsWrapper(tm.Recognizers, tm.TokenizerFlags, tm.WordCountFlags);
			using (SQLiteCommand sQLiteCommand = CreateCommand("INSERT INTO translation_memories(guid, name, source_language, target_language, copyright, description, settings,\r\n\t\t\t\tcreation_user, creation_date, expiration_date, fuzzy_indexes, fga_support, data_version, text_context_match_type, id_context_match) \r\n\t\t\t\tVALUES(@guid, @name, @src_lang, @trg_lang, @copyright, @description, @settings, @cru, @crd, @expd, @fuzzy_indexes, @fga_support, @data_version, @text_context_match_type, @id_context_match)", beginTransaction: true))
			{
				sQLiteCommand.Parameters.Add("@guid", DbType.Guid).Value = tm.Guid;
				sQLiteCommand.Parameters.Add("@name", DbType.String).Value = tm.Name;
				sQLiteCommand.Parameters.Add("@src_lang", DbType.String).Value = tm.LanguageDirection.SourceCultureName;
				sQLiteCommand.Parameters.Add("@trg_lang", DbType.String).Value = tm.LanguageDirection.TargetCultureName;
				sQLiteCommand.Parameters.Add("@copyright", DbType.String).Value = (string.IsNullOrEmpty(tm.Copyright) ? ((IConvertible)DBNull.Value) : ((IConvertible)tm.Copyright));
				sQLiteCommand.Parameters.Add("@description", DbType.String).Value = (string.IsNullOrEmpty(tm.Description) ? ((IConvertible)DBNull.Value) : ((IConvertible)tm.Description));
				sQLiteCommand.Parameters.Add("@settings", DbType.Int32).Value = booleanSettingsWrapper.DbSettingsValue;
				sQLiteCommand.Parameters.Add("@cru", DbType.String).Value = tm.CreationUser;
				sQLiteCommand.Parameters.Add("@crd", DbType.String).Value = Utils.NormalizeToString(tm.CreationDate);
				DateTime? d = tm.ExpirationDate;
				if (d.HasValue && d == DateTimeUtilities.Normalize(DateTime.MaxValue))
				{
					d = null;
				}
				sQLiteCommand.Parameters.Add("@expd", DbType.String).Value = (d.HasValue ? ((IConvertible)Utils.NormalizeToString(d.Value)) : ((IConvertible)DBNull.Value));
				sQLiteCommand.Parameters.Add("@fuzzy_indexes", DbType.Int32).Value = (int)tm.FuzzyIndexes;
				sQLiteCommand.Parameters.Add("@fga_support", DbType.Int32).Value = (int)tm.FGASupport;
				sQLiteCommand.Parameters.Add("@data_version", DbType.Int32).Value = tm.DataVersion;
				sQLiteCommand.Parameters.Add("@text_context_match_type", DbType.Int32).Value = (int)tm.TextContextMatchType;
				sQLiteCommand.Parameters.Add("@id_context_match", DbType.Boolean).Value = tm.IdContextMatch;
				try
				{
					sQLiteCommand.ExecuteNonQuery();
				}
				catch (SQLiteException ex)
				{
					if (ex.ErrorCode != 19)
					{
						throw;
					}
					return false;
				}
				sQLiteCommand.Parameters.Clear();
				sQLiteCommand.CommandText = "SELECT last_insert_rowid()";
				tm.Id = Convert.ToInt32(sQLiteCommand.ExecuteScalar());
				return true;
			}
		}

		public bool DeleteTm(int key)
		{
			CheckVersion();
			using (SQLiteCommand sQLiteCommand = CreateCommand("DELETE FROM tm_resources WHERE tm_id = @id", beginTransaction: true))
			{
				sQLiteCommand.Parameters.Add("@id", DbType.Int32).Value = key;
				sQLiteCommand.ExecuteNonQuery();
			}
			using (SQLiteCommand sQLiteCommand2 = CreateCommand("DELETE FROM parameters WHERE translation_memory_id = @id", beginTransaction: true))
			{
				sQLiteCommand2.Parameters.Add("@id", DbType.Int32).Value = key;
				sQLiteCommand2.ExecuteNonQuery();
			}
			DeleteAllTus(key);
			using (SQLiteCommand sQLiteCommand3 = CreateCommand("DELETE FROM picklist_values WHERE attribute_id in (SELECT id FROM attributes WHERE tm_id = @id)", beginTransaction: true))
			{
				sQLiteCommand3.Parameters.Add("@id", DbType.Int32).Value = key;
				sQLiteCommand3.ExecuteNonQuery();
			}
			using (SQLiteCommand sQLiteCommand4 = CreateCommand("DELETE FROM attributes WHERE tm_id = @id", beginTransaction: true))
			{
				sQLiteCommand4.Parameters.Add("@id", DbType.Int32).Value = key;
				sQLiteCommand4.ExecuteNonQuery();
			}
			using (SQLiteCommand sQLiteCommand5 = CreateCommand("DELETE FROM translation_memories WHERE id = @id", beginTransaction: true))
			{
				sQLiteCommand5.Parameters.Add("@id", DbType.Int32).Value = key;
				return sQLiteCommand5.ExecuteNonQuery() > 0;
			}
		}

		public bool DeleteTmSchema(int key)
		{
			return true;
		}

		private HashSet<string> GetColumnNames(string tableName)
		{
			HashSet<string> hashSet = new HashSet<string>();
			using (SQLiteCommand sQLiteCommand = CreateCommand("pragma table_info('" + tableName + "')", beginTransaction: false))
			{
				using (SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader())
				{
					while (sQLiteDataReader.Read())
					{
						hashSet.Add(sQLiteDataReader.GetString(1).ToLower());
					}
					return hashSet;
				}
			}
		}

		private HashSet<string> GetTableNames()
		{
			HashSet<string> hashSet = new HashSet<string>();
			using (SQLiteCommand sQLiteCommand = CreateCommand("SELECT name FROM sqlite_master WHERE type='table' ", beginTransaction: false))
			{
				using (SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader())
				{
					while (sQLiteDataReader.Read())
					{
						hashSet.Add(sQLiteDataReader.GetString(0).ToLower());
					}
					return hashSet;
				}
			}
		}

		private HashSet<string> GetIndexNames()
		{
			HashSet<string> hashSet = new HashSet<string>();
			using (SQLiteCommand sQLiteCommand = CreateCommand("SELECT name FROM sqlite_master WHERE type='index' ", beginTransaction: false))
			{
				using (SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader())
				{
					while (sQLiteDataReader.Read())
					{
						hashSet.Add(sQLiteDataReader.GetString(0).ToLower());
					}
					return hashSet;
				}
			}
		}

		public void InPlaceUpgrade()
		{
			CheckVersion();
			string text = GetCreatedVersion();
			if (string.IsNullOrEmpty(text))
			{
				AddFgaToLegacyTm();
				text = GetCreatedVersion();
			}
			bool result = true;
			string text2 = ConfigurationManager.AppSettings["CreateStrictHashingFileBasedTMs"];
			if (text2 != null)
			{
				bool.TryParse(text2, out result);
			}
			bool flag = false;
			if (text == "8.10")
			{
				text = "8.09";
			}
			while (true)
			{
				if (flag)
				{
					return;
				}
				if (text == null)
				{
					break;
				}
				if (!(text == "8.08"))
				{
					if (!(text == "8.09"))
					{
						if (!(text == "8.10"))
						{
							break;
						}
						flag = true;
					}
					else if (!result)
					{
						flag = true;
					}
					else
					{
						Update809To810();
					}
				}
				else
				{
					Update808To809();
				}
				text = GetCreatedVersion();
			}
			throw new Exception("Unknown CreatedVersion found while upgrading TM: " + text);
		}

		private void Update808To809()
		{
			using (SQLiteCommand sQLiteCommand = CreateCommand("", beginTransaction: true))
			{
				if (!GetColumnNames("translation_units").Contains("tokenization_sig_hash"))
				{
					sQLiteCommand.CommandText = "alter table translation_units add column tokenization_sig_hash INTEGER";
					sQLiteCommand.ExecuteNonQuery();
					sQLiteCommand.CommandText = "CREATE INDEX idx_tus_toksighash ON translation_units(tokenization_sig_hash);";
					sQLiteCommand.ExecuteNonQuery();
				}
			}
			SetParameter("VERSION_CREATED", "8.09");
		}

		private void Update809To810()
		{
			HashSet<string> tableNames = GetTableNames();
			HashSet<string> indexNames = GetIndexNames();
			bool result = true;
			string text = ConfigurationManager.AppSettings["CreateStrictHashingFileBasedTMs"];
			if (text != null)
			{
				bool.TryParse(text, out result);
			}
			int num = 1;
			if (!result)
			{
				num = 0;
			}
			using (SQLiteCommand sQLiteCommand = CreateCommand("", beginTransaction: true))
			{
				HashSet<string> columnNames = GetColumnNames("translation_memories");
				if (!columnNames.Contains("data_version"))
				{
					sQLiteCommand.CommandText = "alter table translation_memories add column data_version INTEGER NOT NULL DEFAULT " + num.ToString();
					sQLiteCommand.ExecuteNonQuery();
				}
				else
				{
					sQLiteCommand.CommandText = "update translation_memories set data_version = " + num.ToString();
					sQLiteCommand.ExecuteNonQuery();
				}
				if (!columnNames.Contains("text_context_match_type"))
				{
					sQLiteCommand.CommandText = "alter table translation_memories add column text_context_match_type INTEGER NOT NULL DEFAULT " + 1.ToString();
					sQLiteCommand.ExecuteNonQuery();
				}
				if (!columnNames.Contains("id_context_match"))
				{
					sQLiteCommand.CommandText = "alter table translation_memories add column id_context_match BIT DEFAULT 0";
					sQLiteCommand.ExecuteNonQuery();
				}
				if (!tableNames.Contains("translation_unit_idcontexts"))
				{
					sQLiteCommand.CommandText = "CREATE TABLE translation_unit_idcontexts(\r\n\t                    translation_unit_id INT NOT NULL \r\n\t\t                CONSTRAINT FK_translation_unit_idcontexts REFERENCES translation_units(id) ON DELETE CASCADE,\r\n\t                    idcontext TEXT NOT NULL,\t\r\n                        CONSTRAINT PK_tuidc PRIMARY KEY (\r\n\t                        translation_unit_id,\r\n\t                        idcontext\t\r\n                            )\r\n                    );";
					sQLiteCommand.ExecuteNonQuery();
				}
				if (!indexNames.Contains("idx_tus_idcontexts"))
				{
					sQLiteCommand.CommandText = "CREATE INDEX idx_tus_idcontexts ON translation_unit_idcontexts(translation_unit_id, idcontext)";
					sQLiteCommand.ExecuteNonQuery();
				}
				if (!indexNames.Contains("idx_ctx1"))
				{
					sQLiteCommand.CommandText = "CREATE INDEX idx_ctx1 ON translation_unit_contexts(left_source_context)";
					sQLiteCommand.ExecuteNonQuery();
				}
				if (!indexNames.Contains("idx_ctx2"))
				{
					sQLiteCommand.CommandText = "CREATE INDEX idx_ctx2 ON translation_unit_contexts(left_target_context)";
					sQLiteCommand.ExecuteNonQuery();
				}
			}
			SetParameter("VERSION_CREATED", "8.10");
		}

		public void AddFgaToLegacyTm()
		{
			CheckVersion();
			string createdVersion = GetCreatedVersion();
			if (!string.IsNullOrEmpty(createdVersion))
			{
				throw new Exception("AddFGAToLegacyTM called for a TM that already has a CreatedVersion: " + createdVersion);
			}
			using (SQLiteCommand sQLiteCommand = CreateCommand("", beginTransaction: true))
			{
				HashSet<string> columnNames = GetColumnNames("translation_units");
				HashSet<string> tableNames = GetTableNames();
				HashSet<string> indexNames = GetIndexNames();
				if (!SerializesTokens())
				{
					SetParameter("TokenDataVersion", 1.ToString());
					_serializesTokens = true;
				}
				if (!columnNames.Contains("source_token_data"))
				{
					sQLiteCommand.CommandText = "alter table translation_units add column source_token_data BLOB";
					sQLiteCommand.ExecuteNonQuery();
					sQLiteCommand.CommandText = "alter table translation_units add column target_token_data BLOB";
					sQLiteCommand.ExecuteNonQuery();
				}
				if (!SupportsAlignmentData())
				{
					SetParameter("AlignmentDataVersion", 1.ToString());
				}
				if (!columnNames.Contains("alignment_data"))
				{
					sQLiteCommand.CommandText = "alter table translation_units add column alignment_data BLOB";
					sQLiteCommand.ExecuteNonQuery();
				}
				if (!columnNames.Contains("align_model_date"))
				{
					sQLiteCommand.CommandText = "alter table translation_units add column align_model_date DATETIME";
					sQLiteCommand.ExecuteNonQuery();
				}
				if (!indexNames.Contains("idx_tus_align_model_date"))
				{
					sQLiteCommand.CommandText = "create index idx_tus_align_model_date on translation_units(align_model_date)";
					sQLiteCommand.ExecuteNonQuery();
				}
				if (!columnNames.Contains("insert_date"))
				{
					sQLiteCommand.CommandText = "alter table translation_units add column insert_date DATETIME";
					sQLiteCommand.ExecuteNonQuery();
					sQLiteCommand.CommandText = "CREATE INDEX idx_tus_insert_dates ON translation_units(insert_date)";
					sQLiteCommand.ExecuteNonQuery();
				}
				if (!GetColumnNames("translation_memories").Contains("fga_support"))
				{
					sQLiteCommand.CommandText = "alter table translation_memories add column fga_support int not null default 1";
					sQLiteCommand.ExecuteNonQuery();
				}
				if (!tableNames.Contains("vocab_src"))
				{
					sQLiteCommand.CommandText = "CREATE TABLE vocab_src(id INT NOT NULL, vocab TEXT NOT NULL, freq INT NOT NULL)";
					sQLiteCommand.ExecuteNonQuery();
					sQLiteCommand.CommandText = "CREATE INDEX idx_vocab_src ON vocab_src (vocab)";
					sQLiteCommand.ExecuteNonQuery();
				}
				else if (!GetColumnNames("vocab_src").Contains("freq"))
				{
					sQLiteCommand.CommandText = "alter table vocab_src add column freq int not null default 1";
					sQLiteCommand.ExecuteNonQuery();
				}
				if (!tableNames.Contains("vocab_trg"))
				{
					sQLiteCommand.CommandText = "CREATE TABLE vocab_trg(id INT NOT NULL, vocab TEXT NOT NULL, freq INT NOT NULL)";
					sQLiteCommand.ExecuteNonQuery();
					sQLiteCommand.CommandText = "CREATE INDEX idx_vocab_trg ON vocab_trg (vocab)";
					sQLiteCommand.ExecuteNonQuery();
				}
				else if (!GetColumnNames("vocab_trg").Contains("freq"))
				{
					sQLiteCommand.CommandText = "alter table vocab_trg add column freq int not null default 1";
					sQLiteCommand.ExecuteNonQuery();
				}
				if (!tableNames.Contains("vocabfilter"))
				{
					sQLiteCommand.CommandText = "CREATE TABLE vocabfilter(token TEXT NOT NULL)";
					sQLiteCommand.ExecuteNonQuery();
					sQLiteCommand.CommandText = "CREATE INDEX idx_vocabfilter ON vocabfilter (token)";
					sQLiteCommand.ExecuteNonQuery();
				}
				if (!tableNames.Contains("trans_model"))
				{
					sQLiteCommand.CommandText = "CREATE TABLE trans_model(id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, sourcekey INTEGER NOT NULL, targetkey INTEGER NOT NULL, floatval REAL NOT NULL)";
					sQLiteCommand.ExecuteNonQuery();
					sQLiteCommand.CommandText = "CREATE INDEX idx_trans_model_sourcekey ON trans_model (sourcekey)";
					sQLiteCommand.ExecuteNonQuery();
					sQLiteCommand.CommandText = "CREATE INDEX idx_trans_model_targetkey ON trans_model (targetkey)";
					sQLiteCommand.ExecuteNonQuery();
				}
				if (!tableNames.Contains("trans_model_rev"))
				{
					sQLiteCommand.CommandText = "CREATE TABLE trans_model_rev(id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, sourcekey INTEGER NOT NULL, targetkey INTEGER NOT NULL, floatval REAL NOT NULL)";
					sQLiteCommand.ExecuteNonQuery();
					sQLiteCommand.CommandText = "CREATE INDEX idx_trans_model_rev_sourcekey ON trans_model_rev (sourcekey)";
					sQLiteCommand.ExecuteNonQuery();
					sQLiteCommand.CommandText = "CREATE INDEX idx_trans_model_rev_targetkey ON trans_model_rev (targetkey)";
					sQLiteCommand.ExecuteNonQuery();
				}
				if (!tableNames.Contains("translation_unit_fragments"))
				{
					sQLiteCommand.CommandText = "CREATE TABLE translation_unit_fragments(translation_unit_id INT NOT NULL \r\n\t\t                                    CONSTRAINT FK_tuf_tu REFERENCES translation_units(id) ON DELETE CASCADE,\r\n\t                                        fragment_hash INTEGER NOT NULL)";
					sQLiteCommand.ExecuteNonQuery();
					sQLiteCommand.CommandText = "CREATE INDEX idx_tufragments_ids ON translation_unit_fragments(translation_unit_id)";
					sQLiteCommand.ExecuteNonQuery();
					sQLiteCommand.CommandText = "CREATE INDEX idx_tufragments_hashes ON translation_unit_fragments(fragment_hash)";
					sQLiteCommand.ExecuteNonQuery();
				}
				SetParameter("VERSION_CREATED", "8.08");
			}
		}

		public bool UpdateTm(TranslationMemory tm)
		{
			BooleanSettingsWrapper booleanSettingsWrapper = new BooleanSettingsWrapper(tm.Recognizers, tm.TokenizerFlags, tm.WordCountFlags);
			string cmdText = "UPDATE translation_memories SET guid = @guid, name = @name, source_language = @src_lang, target_language = @trg_lang, \r\n\t\t\t\tcopyright = @copyright, description = @description, settings = @settings, creation_user = @cru, creation_date = @crd, \r\n\t\t\t\texpiration_date = @expd, fuzzy_indexes = @fuzzy_indexes" + ((tm.FGASupport != 0) ? ",fga_support = @fga_support" : "") + "  WHERE id = @id";
			using (SQLiteCommand sQLiteCommand = CreateCommand(cmdText, beginTransaction: true))
			{
				sQLiteCommand.Parameters.Add("@id", DbType.Int32).Value = tm.Id;
				sQLiteCommand.Parameters.Add("@guid", DbType.Guid).Value = tm.Guid;
				sQLiteCommand.Parameters.Add("@name", DbType.String).Value = tm.Name;
				sQLiteCommand.Parameters.Add("@src_lang", DbType.String).Value = tm.LanguageDirection.SourceCultureName;
				sQLiteCommand.Parameters.Add("@trg_lang", DbType.String).Value = tm.LanguageDirection.TargetCultureName;
				sQLiteCommand.Parameters.Add("@copyright", DbType.String).Value = (string.IsNullOrEmpty(tm.Copyright) ? ((IConvertible)DBNull.Value) : ((IConvertible)tm.Copyright));
				sQLiteCommand.Parameters.Add("@description", DbType.String).Value = (string.IsNullOrEmpty(tm.Description) ? ((IConvertible)DBNull.Value) : ((IConvertible)tm.Description));
				sQLiteCommand.Parameters.Add("@settings", DbType.Int32).Value = booleanSettingsWrapper.DbSettingsValue;
				sQLiteCommand.Parameters.Add("@cru", DbType.String).Value = tm.CreationUser;
				sQLiteCommand.Parameters.Add("@crd", DbType.String).Value = Utils.NormalizeToString(tm.CreationDate);
				DateTime? d = tm.ExpirationDate;
				if (d.HasValue && d == DateTimeUtilities.Normalize(DateTime.MaxValue))
				{
					d = null;
				}
				sQLiteCommand.Parameters.Add("@expd", DbType.String).Value = (d.HasValue ? ((IConvertible)Utils.NormalizeToString(d.Value)) : ((IConvertible)DBNull.Value));
				sQLiteCommand.Parameters.Add("@fuzzy_indexes", DbType.Int32).Value = (int)tm.FuzzyIndexes;
				if (tm.FGASupport != 0)
				{
					sQLiteCommand.Parameters.Add("@fga_support", DbType.Int32).Value = (int)tm.FGASupport;
				}
				return sQLiteCommand.ExecuteNonQuery() > 0;
			}
		}

		public int GetTuCount(int tmId)
		{
			using (SQLiteCommand sQLiteCommand = CreateCommand("SELECT tucount FROM translation_memories WHERE id = @tm_id", beginTransaction: false))
			{
				sQLiteCommand.Parameters.Add("@tm_id", DbType.Int32).Value = tmId;
				return Convert.ToInt32(sQLiteCommand.ExecuteScalar() ?? throw new StorageException(ErrorCode.TMOrContainerMissing));
			}
		}

		private int GetMaxRowId()
		{
			using (SQLiteCommand sQLiteCommand = CreateCommand("SELECT max(rowid) FROM translation_units", beginTransaction: false))
			{
				object obj = sQLiteCommand.ExecuteScalar();
				if (obj == null)
				{
					return 0;
				}
				return (!Convert.IsDBNull(obj)) ? Convert.ToInt32(obj) : 0;
			}
		}

		public List<AttributeDeclaration> GetAttributes(int tmId)
		{
			using (SQLiteCommand sQLiteCommand = CreateCommand("SELECT id, guid, name, type, tm_id FROM attributes WHERE tm_id = @tm_id", beginTransaction: false))
			{
				using (SQLiteCommand sQLiteCommand2 = CreateCommand("SELECT id, guid, value FROM picklist_values WHERE attribute_id = @attribute_id", beginTransaction: false))
				{
					sQLiteCommand.Parameters.Add("@tm_id", DbType.Int32).Value = tmId;
					sQLiteCommand2.Parameters.Add("@attribute_id", DbType.Int32);
					return GetAttributes(sQLiteCommand, sQLiteCommand2);
				}
			}
		}

		public AttributeDeclaration GetAttribute(int tmId, string name)
		{
			using (SQLiteCommand sQLiteCommand = CreateCommand("SELECT id, guid, name, type, tm_id FROM attributes WHERE name = @name AND tm_id = @tm_id", beginTransaction: false))
			{
				using (SQLiteCommand sQLiteCommand2 = CreateCommand("SELECT id, guid, value FROM picklist_values WHERE attribute_id = @attribute_id", beginTransaction: false))
				{
					sQLiteCommand.Parameters.Add("@name", DbType.String).Value = name;
					sQLiteCommand.Parameters.Add("@tm_id", DbType.Int32).Value = tmId;
					sQLiteCommand2.Parameters.Add("@attribute_id", DbType.Int32);
					return GetAttribute(sQLiteCommand, sQLiteCommand2);
				}
			}
		}

		public AttributeDeclaration GetAttribute(int tmId, int id)
		{
			using (SQLiteCommand sQLiteCommand = CreateCommand("SELECT id, guid, name, type, tm_id FROM attributes WHERE id = @id", beginTransaction: false))
			{
				using (SQLiteCommand sQLiteCommand2 = CreateCommand("SELECT id, guid, value FROM picklist_values WHERE attribute_id = @attribute_id", beginTransaction: false))
				{
					sQLiteCommand.Parameters.Add("@id", DbType.Int32).Value = id;
					sQLiteCommand2.Parameters.Add("@attribute_id", DbType.Int32);
					return GetAttribute(sQLiteCommand, sQLiteCommand2);
				}
			}
		}

		public bool AddAttribute(AttributeDeclaration attribute)
		{
			using (SQLiteCommand sQLiteCommand = CreateCommand(null, beginTransaction: true))
			{
				using (SQLiteCommand sQLiteCommand2 = CreateCommand(null, beginTransaction: true))
				{
					sQLiteCommand.CommandText = "INSERT INTO attributes(name, guid, type, tm_id) VALUES(@name, @guid, @type, @tm_id)";
					sQLiteCommand.Parameters.Add("@tm_id", DbType.Int32).Value = attribute.TMId;
					sQLiteCommand.Parameters.Add("@name", DbType.String).Value = attribute.Name;
					sQLiteCommand.Parameters.Add("@type", DbType.Int32).Value = (int)attribute.Type;
					sQLiteCommand.Parameters.Add("@guid", DbType.Guid).Value = attribute.Guid;
					sQLiteCommand2.CommandText = "SELECT last_insert_rowid()";
					try
					{
						sQLiteCommand.ExecuteNonQuery();
					}
					catch (SQLiteException ex)
					{
						if (ex.ErrorCode != 19)
						{
							throw;
						}
						return false;
					}
					attribute.Id = Convert.ToInt32(sQLiteCommand2.ExecuteScalar());
					if (attribute.Type != FieldValueType.MultiplePicklist && attribute.Type != FieldValueType.SinglePicklist)
					{
						return true;
					}
					sQLiteCommand.Parameters.Clear();
					sQLiteCommand.CommandText = "INSERT INTO picklist_values(attribute_id, value, guid) VALUES(@attribute_id, @value, @guid)";
					sQLiteCommand.Parameters.Add("@attribute_id", DbType.Int32).Value = attribute.Id;
					sQLiteCommand.Parameters.Add("@value", DbType.String, 50);
					sQLiteCommand.Parameters.Add("@guid", DbType.Guid);
					sQLiteCommand.Prepare();
					foreach (PickValue item in attribute.Picklist)
					{
						sQLiteCommand.Parameters[1].Value = item.Value;
						sQLiteCommand.Parameters[2].Value = item.Guid;
						try
						{
							sQLiteCommand.ExecuteNonQuery();
						}
						catch (SQLiteException ex2)
						{
							if (ex2.ErrorCode != 19)
							{
								throw;
							}
							return false;
						}
						item.Id = Convert.ToInt32(sQLiteCommand2.ExecuteScalar());
					}
					return true;
				}
			}
		}

		public bool DeleteAttribute(int tmId, int key)
		{
			using (SQLiteCommand sQLiteCommand = CreateCommand("DELETE FROM picklist_values WHERE attribute_id = @id", beginTransaction: true))
			{
				sQLiteCommand.Parameters.Add("@id", DbType.Int32).Value = key;
				sQLiteCommand.ExecuteNonQuery();
			}
			using (SQLiteCommand sQLiteCommand2 = CreateCommand("DELETE FROM attributes WHERE id = @id", beginTransaction: true))
			{
				sQLiteCommand2.Parameters.Add("@id", DbType.Int32).Value = key;
				return sQLiteCommand2.ExecuteNonQuery() > 0;
			}
		}

		public bool DeleteAttribute(int tmId, string name)
		{
			AttributeDeclaration attribute = GetAttribute(tmId, name);
			if (attribute != null)
			{
				return DeleteAttribute(tmId, attribute.Id);
			}
			return false;
		}

		public PickValue GetPicklistValue(int tmId, int key)
		{
			using (SQLiteCommand sQLiteCommand = CreateCommand("SELECT id, guid, value FROM picklist_values WHERE id = @id", beginTransaction: false))
			{
				sQLiteCommand.Parameters.Add("@id", DbType.Int32).Value = key;
				return GetPicklistValue(sQLiteCommand);
			}
		}

		public int AddPicklistValue(int tmId, int attributeId, PickValue value)
		{
			using (SQLiteCommand sQLiteCommand = CreateCommand("INSERT INTO picklist_values(attribute_id, guid, value) VALUES(@attribute_id, @guid, @value)", beginTransaction: true))
			{
				sQLiteCommand.Parameters.Add("@attribute_id", DbType.Int32).Value = attributeId;
				sQLiteCommand.Parameters.Add("@value", DbType.String).Value = value.Value;
				sQLiteCommand.Parameters.Add("@guid", DbType.Guid).Value = value.Guid;
				sQLiteCommand.ExecuteNonQuery();
				sQLiteCommand.Parameters.Clear();
				sQLiteCommand.CommandText = "SELECT last_insert_rowid()";
				value.Id = Convert.ToInt32(sQLiteCommand.ExecuteScalar());
				return value.Id;
			}
		}

		public int DeleteOrphanContexts(int tmId, TextContextMatchType textContextMatchType)
		{
			string str = "DELETE FROM translation_unit_contexts WHERE ";
			string str2 = "translation_unit_contexts.left_source_context <> 0 AND NOT EXISTS (SELECT id from translation_units WHERE translation_memory_id = @tm_id AND (source_hash = translation_unit_contexts.left_source_context) AND (target_hash = translation_unit_contexts.left_target_context))";
			if (textContextMatchType == TextContextMatchType.PrecedingAndFollowingSource)
			{
				str2 = " (translation_unit_contexts.left_source_context <> 0 AND NOT EXISTS (SELECT id FROM translation_units WHERE translation_memory_id = @tm_id AND source_hash = translation_unit_contexts.left_source_context))  OR (translation_unit_contexts.left_target_context <> 0 AND NOT EXISTS (SELECT id FROM translation_units WHERE translation_memory_id = @tm_id AND source_hash = translation_unit_contexts.left_target_context))";
			}
			str += str2;
			using (SQLiteCommand sQLiteCommand = CreateCommand(str, beginTransaction: true))
			{
				sQLiteCommand.Parameters.Add("@tm_id", DbType.Int32).Value = tmId;
				return sQLiteCommand.ExecuteNonQuery();
			}
		}

		public bool DeletePicklistValue(int tmId, int attributeId, string value)
		{
			using (SQLiteCommand sQLiteCommand = CreateCommand("DELETE FROM picklist_values WHERE attribute_id = @attribute_id AND value = @value", beginTransaction: true))
			{
				sQLiteCommand.Parameters.Add("@attribute_id", DbType.Int32).Value = attributeId;
				sQLiteCommand.Parameters.Add("@value", DbType.String).Value = value;
				return sQLiteCommand.ExecuteNonQuery() > 0;
			}
		}

		public bool RenameAttribute(int tmId, int attributeKey, string newName)
		{
			if (GetAttribute(tmId, newName) != null)
			{
				throw new StorageException(ErrorCode.StorageFieldAlreadyExists, newName);
			}
			using (SQLiteCommand sQLiteCommand = CreateCommand("UPDATE attributes SET name = @newname WHERE id = @attribute_id", beginTransaction: true))
			{
				sQLiteCommand.Parameters.Add("@attribute_id", DbType.Int32).Value = attributeKey;
				sQLiteCommand.Parameters.Add("@newname", DbType.String).Value = newName;
				return sQLiteCommand.ExecuteNonQuery() > 0;
			}
		}

		public bool RenamePicklistValue(int tmId, int attributeId, string oldName, string newName)
		{
			using (SQLiteCommand sQLiteCommand = CreateCommand("UPDATE picklist_values SET value = @newname WHERE attribute_id = @attribute_id AND value = @oldname", beginTransaction: true))
			{
				sQLiteCommand.Parameters.Add("@attribute_id", DbType.Int32).Value = attributeId;
				sQLiteCommand.Parameters.Add("@newname", DbType.String).Value = newName;
				sQLiteCommand.Parameters.Add("@oldname", DbType.String).Value = oldName;
				return sQLiteCommand.ExecuteNonQuery() > 0;
			}
		}

		public void AddTu(TranslationUnit tu, FuzzyIndexes indexes, bool keepId, long tokenizationSignatureHash)
		{
			if (keepId && tu.Id <= 0)
			{
				keepId = false;
			}
			string text = string.Empty;
			string text2 = string.Empty;
			DateTime value = DateTimeUtilities.Normalize(DateTime.Now);
			if (!tu.InsertDate.HasValue)
			{
				tu.InsertDate = value;
			}
			bool flag = SupportsAlignmentData();
			if (flag)
			{
				text = ", alignment_data, align_model_date, insert_date ";
				text2 = ", @alignment_data, @align_model_date, @insert_date ";
			}
			string cmdText = "INSERT INTO translation_units(" + (keepId ? "id, " : string.Empty) + "guid, translation_memory_id, \r\n\t\t\t\tsource_hash, source_segment, \r\n\t\t\t\ttarget_hash, target_segment, \r\n\t\t\t\tcreation_date, creation_user, change_date, change_user, last_used_date, last_used_user, usage_counter, flags " + (_canReportReindexRequired ? ", tokenization_sig_hash" : string.Empty) + (SerializesTokens() ? ", source_token_data, target_token_data " : string.Empty) + text + ") VALUES(" + (keepId ? "@id, " : string.Empty) + "@guid, @tm_id, @source_hash, @source_text, @target_hash, @target_text, \r\n\t\t\t\t@crd, @cru, @chd, @chu, @lud, @luu, @usc, @flags " + (_canReportReindexRequired ? ", @sighash" : string.Empty) + (SerializesTokens() ? ", @source_token_data, @target_token_data " : string.Empty) + text2 + ")";
			using (SQLiteCommand sQLiteCommand = CreateCommand(cmdText, beginTransaction: true))
			{
				sQLiteCommand.Parameters.Add("@guid", DbType.Guid).Value = tu.Guid;
				sQLiteCommand.Parameters.Add("@tm_id", DbType.Int32).Value = tu.TranslationMemoryId;
				sQLiteCommand.Parameters.Add("@source_hash", DbType.Int64).Value = tu.Source.Hash;
				sQLiteCommand.Parameters.Add("@target_hash", DbType.Int64).Value = tu.Target.Hash;
				sQLiteCommand.Parameters.Add("@crd", DbType.String).Value = Utils.NormalizeToString(tu.CreationDate);
				sQLiteCommand.Parameters.Add("@cru", DbType.String).Value = tu.CreationUser;
				sQLiteCommand.Parameters.Add("@chd", DbType.String).Value = Utils.NormalizeToString(tu.ChangeDate);
				sQLiteCommand.Parameters.Add("@chu", DbType.String).Value = tu.ChangeUser;
				sQLiteCommand.Parameters.Add("@lud", DbType.String).Value = Utils.NormalizeToString(tu.LastUsedDate);
				sQLiteCommand.Parameters.Add("@luu", DbType.String).Value = tu.LastUsedUser;
				sQLiteCommand.Parameters.Add("@usc", DbType.Int32).Value = tu.UsageCounter;
				sQLiteCommand.Parameters.Add("@flags", DbType.Int32).Value = tu.Flags;
				if (keepId)
				{
					sQLiteCommand.Parameters.Add("@id", DbType.Int32).Value = tu.Id;
				}
				if (_canReportReindexRequired)
				{
					sQLiteCommand.Parameters.Add("@sighash", DbType.Int64).Value = tokenizationSignatureHash;
				}
				sQLiteCommand.Parameters.Add("@source_text", DbType.String).Value = tu.Source.Text;
				sQLiteCommand.Parameters.Add("@target_text", DbType.String).Value = tu.Target.Text;
				if (flag)
				{
					sQLiteCommand.Parameters.Add("@alignment_data", DbType.Binary).Value = tu.AlignmentData;
					sQLiteCommand.Parameters.Add("@align_model_date", DbType.String).Value = (tu.AlignModelDate.HasValue ? ((IConvertible)Utils.NormalizeToString(tu.AlignModelDate.Value)) : ((IConvertible)DBNull.Value));
					sQLiteCommand.Parameters.Add("@insert_date", DbType.String).Value = Utils.NormalizeToString(tu.InsertDate.GetValueOrDefault());
				}
				if (SerializesTokens())
				{
					sQLiteCommand.Parameters.Add("@source_token_data", DbType.Binary).Value = tu.SourceTokenData;
					sQLiteCommand.Parameters.Add("@target_token_data", DbType.Binary).Value = tu.TargetTokenData;
				}
				sQLiteCommand.ExecuteNonQuery();
				if (!keepId)
				{
					sQLiteCommand.Parameters.Clear();
					sQLiteCommand.CommandText = "SELECT last_insert_rowid()";
					tu.Id = Convert.ToInt32(sQLiteCommand.ExecuteScalar());
				}
				AddAttributeValues(tu);
				bool canChooseTextContextMatchType = _canChooseTextContextMatchType;
				IndexTu(tu, indexes, canChooseTextContextMatchType);
				IncTuCount(tu.TranslationMemoryId, 1);
			}
		}

		public List<Tuple<Guid, int>> AddTus(Tuple<TranslationUnit, ImportType>[] batchTUs, FuzzyIndexes indexes, long tokenizationSignatureHash, int tmid)
		{
			throw new NotImplementedException();
		}

		private void IncTuCount(int tmId, int cnt)
		{
			using (SQLiteCommand sQLiteCommand = CreateCommand("UPDATE translation_memories SET tucount = tucount + @cnt WHERE id = @tmid", beginTransaction: true))
			{
				sQLiteCommand.Parameters.AddWithValue("@cnt", cnt);
				sQLiteCommand.Parameters.AddWithValue("@tmid", tmId);
				sQLiteCommand.ExecuteNonQuery();
			}
		}

		private void DecTuCount(int tmId, int cnt)
		{
			using (SQLiteCommand sQLiteCommand = CreateCommand("UPDATE translation_memories SET tucount = tucount - @cnt WHERE id = @tmid", beginTransaction: true))
			{
				sQLiteCommand.Parameters.AddWithValue("@cnt", cnt);
				sQLiteCommand.Parameters.AddWithValue("@tmid", tmId);
				sQLiteCommand.ExecuteNonQuery();
			}
		}

		private void DeleteAttributeValues(TranslationUnit tu)
		{
			DeleteAttributeValues(tu.Id);
		}

		private void DeleteAttributeValues(int id)
		{
			using (SQLiteCommand sQLiteCommand = CreateCommand("DELETE FROM date_attributes WHERE translation_unit_id = @tu_id", beginTransaction: true))
			{
				sQLiteCommand.Parameters.Add("@tu_id", DbType.Int32).Value = id;
				sQLiteCommand.ExecuteNonQuery();
				sQLiteCommand.CommandText = "DELETE FROM string_attributes WHERE translation_unit_id = @tu_id";
				sQLiteCommand.ExecuteNonQuery();
				sQLiteCommand.CommandText = "DELETE FROM numeric_attributes WHERE translation_unit_id = @tu_id";
				sQLiteCommand.ExecuteNonQuery();
				sQLiteCommand.CommandText = "DELETE FROM picklist_attributes WHERE translation_unit_id = @tu_id";
				sQLiteCommand.ExecuteNonQuery();
			}
		}

		private void AddAttributeValues(TranslationUnit tu)
		{
			if (tu.Attributes != null && tu.Attributes.Count > 0)
			{
				using (SQLiteCommand sQLiteCommand = CreateCommand("INSERT INTO date_attributes(attribute_id, value, translation_unit_id) \r\n\t\t\t\t\t\tVALUES(@attribute_id, @date_value, @tu_id)", beginTransaction: true))
				{
					using (SQLiteCommand sQLiteCommand2 = CreateCommand("INSERT INTO string_attributes(attribute_id, value, translation_unit_id) \r\n\t\t\t\t\t\tVALUES(@attribute_id, @string_value, @tu_id)", beginTransaction: true))
					{
						using (SQLiteCommand sQLiteCommand3 = CreateCommand("INSERT INTO numeric_attributes(attribute_id, value, translation_unit_id) \r\n\t\t\t\t\t\tVALUES(@attribute_id, @numeric_value, @tu_id)", beginTransaction: true))
						{
							using (SQLiteCommand sQLiteCommand4 = CreateCommand("INSERT INTO picklist_attributes(translation_unit_id, picklist_value_id)\r\n\t\t\t\t\t\tVALUES(@tu_id, @picklist_value_id)", beginTransaction: true))
							{
								sQLiteCommand.Parameters.Add("@attribute_id", DbType.Int32);
								sQLiteCommand.Parameters.Add("@date_value", DbType.String);
								sQLiteCommand.Parameters.Add("@tu_id", DbType.Int32).Value = tu.Id;
								sQLiteCommand.Prepare();
								sQLiteCommand2.Parameters.Add("@attribute_id", DbType.Int32);
								sQLiteCommand2.Parameters.Add("@string_value", DbType.String);
								sQLiteCommand2.Parameters.Add("@tu_id", DbType.Int32).Value = tu.Id;
								sQLiteCommand2.Prepare();
								sQLiteCommand3.Parameters.Add("@attribute_id", DbType.Int32);
								sQLiteCommand3.Parameters.Add("@numeric_value", DbType.Int32);
								sQLiteCommand3.Parameters.Add("@tu_id", DbType.Int32).Value = tu.Id;
								sQLiteCommand3.Prepare();
								sQLiteCommand4.Parameters.Add("@picklist_value_id", DbType.Int32);
								sQLiteCommand4.Parameters.Add("@tu_id", DbType.Int32).Value = tu.Id;
								sQLiteCommand4.Prepare();
								foreach (AttributeValue attribute in tu.Attributes)
								{
									SQLiteParameter sQLiteParameter = sQLiteCommand.Parameters[0];
									SQLiteParameter sQLiteParameter2 = sQLiteCommand2.Parameters[0];
									object obj2 = sQLiteCommand3.Parameters[0].Value = attribute.DeclarationId;
									object obj5 = sQLiteParameter.Value = (sQLiteParameter2.Value = obj2);
									switch (attribute.Type)
									{
									case FieldValueType.SingleString:
										sQLiteCommand2.Parameters[1].Value = attribute.Value;
										sQLiteCommand2.ExecuteNonQuery();
										break;
									case FieldValueType.MultipleString:
									{
										string[] array2 = (string[])attribute.Value;
										foreach (string value2 in array2)
										{
											sQLiteCommand2.Parameters[1].Value = value2;
											sQLiteCommand2.ExecuteNonQuery();
										}
										break;
									}
									case FieldValueType.DateTime:
									{
										string value = Utils.DateToSqlite1dot0dot60String((DateTime)attribute.Value);
										sQLiteCommand.Parameters[1].Value = value;
										sQLiteCommand.ExecuteNonQuery();
										break;
									}
									case FieldValueType.SinglePicklist:
										sQLiteCommand4.Parameters[0].Value = attribute.Value;
										sQLiteCommand4.ExecuteNonQuery();
										break;
									case FieldValueType.MultiplePicklist:
									{
										int[] array = (int[])attribute.Value;
										foreach (int num in array)
										{
											sQLiteCommand4.Parameters[0].Value = num;
											sQLiteCommand4.ExecuteNonQuery();
										}
										break;
									}
									case FieldValueType.Integer:
										sQLiteCommand3.Parameters[1].Value = attribute.Value;
										sQLiteCommand3.ExecuteNonQuery();
										break;
									default:
										throw new ArgumentException("Unknown attribute type.", "Type");
									}
								}
							}
						}
					}
				}
			}
		}

		private bool IsFuzzyIndexLoaded(FuzzyIndexes type)
		{
			return _currentIndexCache.FuzzyIndexCaches[(int)type] != null;
		}

		private bool FuzzyIndexRequiresReload(FuzzyIndexes type)
		{
			if (_currentIndexCache.FuzzyIndexCaches[(int)type] == null)
			{
				return true;
			}
			if (_currentIndexCache.IsLocalFile)
			{
				return false;
			}
			int num = _currentIndexCache.LoadTicks[(int)type];
			int tickCount = Environment.TickCount;
			if (tickCount >= num)
			{
				return tickCount - num > 1800000;
			}
			return true;
		}

		private static void ClearAllFuzzyIndexCaches(IndexCache indexCache)
		{
			if (indexCache == null)
			{
				return;
			}
			for (int i = 0; i < 10; i++)
			{
				if (indexCache.FuzzyIndexCaches[i] != null)
				{
					indexCache.FuzzyIndexCaches[i].Clear();
					indexCache.FuzzyIndexCaches[i] = null;
				}
			}
		}

		private void EnsureFuzzyIndexLoaded(FuzzyIndexes type)
		{
			lock (_currentIndexCache)
			{
				if (FuzzyIndexRequiresReload(type))
				{
					_currentIndexCache.FuzzyIndexCaches[(int)type] = new InMemoryFuzzyIndex();
					using (SQLiteCommand sQLiteCommand = CreateCommand($"SELECT translation_memory_id, translation_unit_id, fi{(int)type} FROM fuzzy_data ORDER BY translation_unit_id ASC", beginTransaction: false))
					{
						using (SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader())
						{
							char[] separator = new char[1]
							{
								'|'
							};
							while (sQLiteDataReader.Read())
							{
								int @int = sQLiteDataReader.GetInt32(1);
								string text = sQLiteDataReader.IsDBNull(2) ? null : sQLiteDataReader.GetString(2);
								List<int> list = new List<int>();
								if (text != null)
								{
									string[] array = text.Split(separator, StringSplitOptions.RemoveEmptyEntries);
									for (int i = 0; i < array.Length; i++)
									{
										int item = int.Parse(array[i], CultureInfo.InvariantCulture);
										list.Add(item);
									}
								}
								_currentIndexCache.FuzzyIndexCaches[(int)type].Add(@int, list);
							}
						}
					}
					_currentIndexCache.LoadTicks[(int)type] = Environment.TickCount;
				}
			}
		}

		private static string GetFeatureString(List<int> features)
		{
			if (features == null || features.Count == 0)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			foreach (int feature in features)
			{
				if (flag)
				{
					flag = false;
				}
				else
				{
					stringBuilder.Append("|");
				}
				stringBuilder.Append(feature.ToString(CultureInfo.InvariantCulture));
			}
			return stringBuilder.ToString();
		}

		private void AddFeaturesToCache(int tuId, List<int> features, FuzzyIndexes type)
		{
			if (IsFuzzyIndexLoaded(type) && features != null && features.Count > 0)
			{
				if (_currentIndexCache.FuzzyIndexCaches[(int)type].ContainsKey(tuId))
				{
					ClearAllFuzzyIndexCaches(_currentIndexCache);
					EnsureFuzzyIndexLoaded(type);
				}
				_currentIndexCache.FuzzyIndexCaches[(int)type].Add(tuId, features);
			}
		}

		private List<TranslationUnit> GetTuSet(SQLiteCommand searchCmd, int count, TuContextData tuContextData)
		{
			List<TranslationUnit> list = new List<TranslationUnit>();
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			StringBuilder stringBuilder = new StringBuilder();
			using (SQLiteDataReader sQLiteDataReader = searchCmd.ExecuteReader())
			{
				while (sQLiteDataReader.Read())
				{
					TranslationUnit translationUnit = ReadTu(sQLiteDataReader);
					dictionary.Add(translationUnit.Id, list.Count);
					if (list.Count > 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append(translationUnit.Id.ToString(CultureInfo.InvariantCulture));
					list.Add(translationUnit);
					if (count > 0 && list.Count >= count)
					{
						break;
					}
				}
			}
			string arg = stringBuilder.ToString();
			if (list.Count <= 0)
			{
				return list;
			}
			using (SQLiteCommand sQLiteCommand = CreateCommand(string.Format("\r\nSELECT da.translation_unit_id, da.attribute_id, a.name, a.type, da.value \r\nFROM date_attributes da INNER JOIN attributes a ON da.attribute_id = a.id \r\nWHERE da.translation_unit_id IN ({0}) \r\nORDER BY da.translation_unit_id, da.attribute_id;\r\n\r\nSELECT sa.translation_unit_id, sa.attribute_id, a.name, a.type, sa.value \r\nFROM string_attributes sa INNER JOIN attributes a ON sa.attribute_id = a.id\r\nWHERE sa.translation_unit_id IN ({0}) \r\nORDER BY sa.translation_unit_id, sa.attribute_id;\r\n\r\nSELECT na.translation_unit_id, na.attribute_id, a.name, a.type, na.value \r\nFROM numeric_attributes na INNER JOIN attributes a ON na.attribute_id = a.id\r\nWHERE na.translation_unit_id IN ({0}) \r\nORDER BY na.translation_unit_id, na.attribute_id;\r\n\r\nSELECT pa.translation_unit_id, pv.attribute_id, a.name, a.type, pa.picklist_value_id FROM picklist_attributes pa \r\nINNER JOIN picklist_values pv ON pv.id = pa.picklist_value_id\r\nINNER JOIN attributes a ON pv.attribute_id = a.id\r\nWHERE pa.translation_unit_id IN ({0})\r\nORDER BY pa.translation_unit_id, pv.attribute_id\r\n", arg), beginTransaction: false))
			{
				using (SQLiteDataReader reader = sQLiteCommand.ExecuteReader())
				{
					ReadAttributeValues(reader, list, dictionary, autoAdvanceResultSet: true);
				}
			}
			if (tuContextData.TextContext != null && tuContextData.TextContext.Context1 != -1)
			{
				using (SQLiteCommand sQLiteCommand2 = CreateCommand($"\r\nSELECT translation_unit_id, left_source_context, left_target_context \r\nFROM translation_unit_contexts \r\nWHERE translation_unit_id IN ({arg})\r\nand left_source_context={tuContextData.TextContext.Context1}", beginTransaction: false))
				{
					using (SQLiteDataReader contextReader = sQLiteCommand2.ExecuteReader())
					{
						ReadTextContexts(contextReader, list, dictionary);
					}
				}
			}
			if (string.IsNullOrEmpty(tuContextData.IdContext))
			{
				return list;
			}
			using (SQLiteCommand sQLiteCommand3 = CreateCommand($"\r\nSELECT translation_unit_id, idcontext\r\nFROM translation_unit_idcontexts\r\nWHERE translation_unit_id IN ({arg})\r\nand idcontext='{tuContextData.IdContext}'", beginTransaction: false))
			{
				using (SQLiteDataReader contextReader2 = sQLiteCommand3.ExecuteReader())
				{
					ReadIdContexts(contextReader2, list, dictionary);
					return list;
				}
			}
		}

		private List<TranslationUnit> GetTus(SQLiteCommand searchCmd, bool idContextMatch, int count)
		{
			using (SQLiteCommand sQLiteCommand = CreateCommand("\r\nSELECT da.attribute_id, a.name, a.type, da.value \r\nFROM date_attributes da INNER JOIN attributes a ON da.attribute_id = a.id \r\nWHERE da.translation_unit_id = @tu_id\r\nORDER BY da.attribute_id;\r\n\r\nSELECT sa.attribute_id, a.name, a.type, sa.value \r\nFROM string_attributes sa INNER JOIN attributes a ON sa.attribute_id = a.id\r\nWHERE sa.translation_unit_id = @tu_id\r\nORDER BY sa.attribute_id;\r\n\r\nSELECT na.attribute_id, a.name, a.type, na.value \r\nFROM numeric_attributes na INNER JOIN attributes a ON na.attribute_id = a.id\r\nWHERE na.translation_unit_id = @tu_id\r\nORDER BY na.attribute_id;\r\n\r\nSELECT pv.attribute_id, a.name, a.type, pa.picklist_value_id FROM picklist_attributes pa \r\nINNER JOIN picklist_values pv ON pv.id = pa.picklist_value_id\r\nINNER JOIN attributes a ON pv.attribute_id = a.id\r\nWHERE pa.translation_unit_id = @tu_id\r\nORDER BY pv.attribute_id\r\n", beginTransaction: false))
			{
				using (SQLiteCommand sQLiteCommand2 = CreateCommand("\r\nSELECT left_source_context, left_target_context \r\nFROM translation_unit_contexts \r\nWHERE translation_unit_id = @tu_id", beginTransaction: false))
				{
					using (SQLiteCommand sQLiteCommand3 = CreateCommand("\r\nSELECT left_source_context, left_target_context \r\nFROM translation_unit_contexts \r\nWHERE translation_unit_id = @tu_id", beginTransaction: false))
					{
						sQLiteCommand.Parameters.Add("@tu_id", DbType.Int32);
						sQLiteCommand2.Parameters.Add("@tu_id", DbType.Int32);
						sQLiteCommand3.Parameters.Add("@tu_id", DbType.Int32);
						return GetTus(searchCmd, sQLiteCommand, sQLiteCommand2, idContextMatch ? sQLiteCommand3 : null, count);
					}
				}
			}
		}

		public List<TranslationUnit> GetTus(int tmId, int startAfter, int count, bool forward, bool idContextMatch, bool includeContextContent, TextContextMatchType textContextMatchType, CultureInfo sourceCulture, CultureInfo targetCulture)
		{
			string alignmentDataColspec = GetAlignmentDataColspec();
			string str = "SELECT id, guid, translation_memory_id, \r\n\t\t\t\tsource_hash, source_segment, 0, 0, \r\n\t\t\t\ttarget_hash, target_segment, 0, 0, \r\n\t\t\t\tcreation_date, creation_user, change_date, change_user, last_used_date, last_used_user, usage_counter, flags " + (SerializesTokens() ? ", source_token_data, target_token_data " : ", null, null ") + alignmentDataColspec + ", 0, null, null ";
			string str2 = forward ? "FROM translation_units WHERE id > @start_after\r\n\t\t\t\tORDER BY id ASC\r\n\t\t\t\tLIMIT @limit" : "FROM translation_units WHERE id <= @start_after\r\n\t\t\t\tORDER BY id DESC\r\n\t\t\t\tLIMIT @limit";
			str += str2;
			string cmdText = "\r\nSELECT translation_unit_id, left_source_context, left_target_context \r\nFROM translation_unit_contexts \r\nWHERE translation_unit_id >= @from_tu_id AND translation_unit_id <= @into_tu_id";
			if (includeContextContent)
			{
				cmdText = "\r\nSELECT translation_unit_id, left_source_context, left_target_context, source_segment, null " + (SerializesTokens() ? ", source_token_data " : ", null ") + ", target_segment, null" + (SerializesTokens() ? ", target_token_data " : ", null ") + ", 0 FROM translation_unit_contexts \r\n    LEFT OUTER JOIN translation_units\r\n\tON id = (select max(id) from translation_units where source_hash = left_source_context and target_hash = left_target_context and translation_memory_id = @tm_id)\r\n    WHERE translation_unit_id >= @from_tu_id AND translation_unit_id <= @into_tu_id";
				if (textContextMatchType == TextContextMatchType.PrecedingAndFollowingSource)
				{
					cmdText = "\r\nSELECT translation_unit_id, left_source_context, left_target_context, t1.source_segment, null" + (SerializesTokens() ? ", t1.source_token_data" : ", null ") + ", t2.source_segment, null" + (SerializesTokens() ? ", t2.source_token_data" : ", null ") + ", 0, 0\r\nFROM translation_unit_contexts \r\n    LEFT OUTER JOIN translation_units As t1\r\n\tON t1.id = (select max(id) from translation_units where source_hash = left_source_context and translation_memory_id = @tm_id)\r\n\tLEFT OUTER JOIN translation_units As t2\r\n\tON t2.id = (select max(id) from translation_units where source_hash = left_target_context and translation_memory_id = @tm_id)\r\n    WHERE translation_unit_id >= @from_tu_id AND translation_unit_id <= @into_tu_id";
				}
			}
			using (SQLiteCommand sQLiteCommand = CreateCommand(str, beginTransaction: false))
			{
				using (SQLiteCommand sQLiteCommand2 = CreateCommand(cmdText, beginTransaction: false))
				{
					using (SQLiteCommand sQLiteCommand4 = CreateCommand("\r\nSELECT translation_unit_id, idcontext\r\nFROM translation_unit_idcontexts\r\nWHERE translation_unit_id >= @from_tu_id AND translation_unit_id <= @into_tu_id", beginTransaction: false))
					{
						using (SQLiteCommand sQLiteCommand3 = CreateCommand("\r\nSELECT da.translation_unit_id, da.attribute_id, a.name, a.type, da.value \r\nFROM date_attributes da INNER JOIN attributes a ON da.attribute_id = a.id \r\nWHERE da.translation_unit_id >= @from_tu_id AND da.translation_unit_id <= @into_tu_id \r\nORDER BY da.translation_unit_id, da.attribute_id;\r\n\r\nSELECT sa.translation_unit_id, sa.attribute_id, a.name, a.type, sa.value \r\nFROM string_attributes sa INNER JOIN attributes a ON sa.attribute_id = a.id\r\nWHERE sa.translation_unit_id >= @from_tu_id AND sa.translation_unit_id <= @into_tu_id \r\nORDER BY sa.translation_unit_id, sa.attribute_id;\r\n\r\nSELECT na.translation_unit_id, na.attribute_id, a.name, a.type, na.value \r\nFROM numeric_attributes na INNER JOIN attributes a ON na.attribute_id = a.id\r\nWHERE na.translation_unit_id >= @from_tu_id AND na.translation_unit_id <= @into_tu_id \r\nORDER BY na.translation_unit_id, na.attribute_id;\r\n\r\nSELECT pa.translation_unit_id, pv.attribute_id, a.name, a.type, pa.picklist_value_id FROM picklist_attributes pa \r\nINNER JOIN picklist_values pv ON pv.id = pa.picklist_value_id\r\nINNER JOIN attributes a ON pv.attribute_id = a.id\r\nWHERE pa.translation_unit_id >= @from_tu_id AND pa.translation_unit_id <= @into_tu_id \r\nORDER BY pa.translation_unit_id, pv.attribute_id\r\n", beginTransaction: false))
						{
							sQLiteCommand.Parameters.Add("@tm_id", DbType.Int32).Value = tmId;
							sQLiteCommand.Parameters.Add("@start_after", DbType.Int32).Value = startAfter;
							sQLiteCommand.Parameters.Add("@limit", DbType.Int32).Value = count;
							sQLiteCommand2.Parameters.Add("@from_tu_id", DbType.Int32);
							sQLiteCommand2.Parameters.Add("@into_tu_id", DbType.Int32);
							sQLiteCommand2.Parameters.Add("@tm_id", DbType.Int32).Value = tmId;
							sQLiteCommand3.Parameters.Add("@from_tu_id", DbType.Int32);
							sQLiteCommand3.Parameters.Add("@into_tu_id", DbType.Int32);
							sQLiteCommand4.Parameters.Add("@from_tu_id", DbType.Int32);
							sQLiteCommand4.Parameters.Add("@into_tu_id", DbType.Int32);
							return GetTuRange(sQLiteCommand, sQLiteCommand2, idContextMatch ? sQLiteCommand4 : null, sQLiteCommand3, textContextMatchType, sourceCulture, targetCulture);
						}
					}
				}
			}
		}

		public List<TranslationUnit> GetTus(int tmId, List<Tuple<int, long, long>> tuAndHash, bool unused)
		{
			throw new NotImplementedException();
		}

		public List<int> GetTmIds()
		{
			List<int> list = new List<int>();
			using (SQLiteCommand sQLiteCommand = CreateCommand("SELECT id FROM translation_memories ORDER BY id ASC", beginTransaction: false))
			{
				using (SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader())
				{
					while (sQLiteDataReader.Read())
					{
						int item = Convert.ToInt32(sQLiteDataReader[0]);
						list.Add(item);
					}
					return list;
				}
			}
		}

		public List<PersistentObjectToken> GetTuIds(int tmId, int startAfter, int count, bool forward)
		{
			string cmdText = forward ? "SELECT id, guid\r\n\t\t\t\tFROM translation_units WHERE id > @start_after \r\n\t\t\t\tORDER BY id ASC\r\n\t\t\t\tLIMIT @limit" : "SELECT id, guid\r\n\t\t\t\tFROM translation_units WHERE id <= @start_after \r\n\t\t\t\tORDER BY id DESC\r\n\t\t\t\tLIMIT @limit";
			using (SQLiteCommand sQLiteCommand = CreateCommand(cmdText, beginTransaction: false))
			{
				sQLiteCommand.Parameters.Add("@tm_id", DbType.Int32).Value = tmId;
				sQLiteCommand.Parameters.Add("@start_after", DbType.Int32).Value = startAfter;
				sQLiteCommand.Parameters.Add("@limit", DbType.Int32).Value = count;
				return GetTuIds(sQLiteCommand);
			}
		}

		private TranslationUnit GetTu(SQLiteCommand cmd, bool idContextMatch)
		{
			List<TranslationUnit> tus = GetTus(cmd, idContextMatch, 1);
			if (tus.Count <= 0)
			{
				return null;
			}
			return tus[0];
		}

		public TranslationUnit GetTu(int tmId, int key, bool idContextMatch)
		{
			string alignmentDataColspec = GetAlignmentDataColspec();
			using (SQLiteCommand sQLiteCommand = CreateCommand("SELECT id, guid, translation_memory_id, \r\n\t\t\t\tsource_hash, source_segment, 0, 0, \r\n\t\t\t\ttarget_hash, target_segment, 0, 0, \r\n\t\t\t\tcreation_date, creation_user, change_date, change_user, last_used_date, last_used_user, usage_counter, flags " + (SerializesTokens() ? ", source_token_data, target_token_data " : ", null, null ") + alignmentDataColspec + ", 0, null, null FROM translation_units WHERE id = @id", beginTransaction: false))
			{
				sQLiteCommand.Parameters.Add("@id", DbType.Int32).Value = key;
				return GetTu(sQLiteCommand, idContextMatch);
			}
		}

		public bool DeleteTu(int tmId, PersistentObjectToken key, TextContextMatchType textContextMatchType, bool deleteOrphanContexts)
		{
			int id = key.Id;
			UnindexTu(tmId, id);
			DeleteAttributeValues(id);
			DeleteContexts(tmId, id);
			using (SQLiteCommand sQLiteCommand = CreateCommand("", beginTransaction: true))
			{
				sQLiteCommand.Parameters.Add("@id", DbType.Int32).Value = id;
				sQLiteCommand.Parameters.Add("@tm_id", DbType.Int32).Value = tmId;
				sQLiteCommand.CommandText = "select source_hash, target_hash from translation_units where id = @id";
				long @int;
				long int2;
				using (SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader())
				{
					if (!sQLiteDataReader.Read())
					{
						return false;
					}
					@int = sQLiteDataReader.GetInt64(0);
					int2 = sQLiteDataReader.GetInt64(1);
				}
				sQLiteCommand.CommandText = "DELETE FROM translation_units WHERE id = @id";
				bool flag = sQLiteCommand.ExecuteNonQuery() > 0;
				if (!flag)
				{
					return false;
				}
				DecTuCount(tmId, 1);
				if (!deleteOrphanContexts)
				{
					return flag;
				}
				sQLiteCommand.CommandText = "select count(*) from translation_units where translation_memory_id = @tm_id AND source_hash = @oldSourceHash and target_hash = @oldTargetHash";
				if (textContextMatchType == TextContextMatchType.PrecedingAndFollowingSource)
				{
					sQLiteCommand.CommandText = "select count(*) from translation_units where translation_memory_id = @tm_id AND source_hash = @oldSourceHash";
				}
				sQLiteCommand.Parameters.Add("@oldSourceHash", DbType.Int64).Value = @int;
				sQLiteCommand.Parameters.Add("@oldTargetHash", DbType.Int64).Value = int2;
				if (Convert.ToInt64(sQLiteCommand.ExecuteScalar()) == 0L)
				{
					sQLiteCommand.CommandText = "delete from translation_unit_contexts where left_source_context = @oldSourceHash and left_target_context = @oldTargetHash";
					if (textContextMatchType == TextContextMatchType.PrecedingAndFollowingSource)
					{
						sQLiteCommand.CommandText = "delete from translation_unit_contexts where left_source_context = @oldSourceHash or left_target_context = @oldSourceHash";
					}
					sQLiteCommand.ExecuteNonQuery();
				}
				return flag;
			}
		}

		public List<PersistentObjectToken> DeleteTus(int tmId, List<PersistentObjectToken> keys, TextContextMatchType textContextMatchType, bool deleteOrphanContexts)
		{
			List<PersistentObjectToken> list = new List<PersistentObjectToken>();
			foreach (PersistentObjectToken key in keys)
			{
				if (DeleteTu(tmId, key, textContextMatchType, deleteOrphanContexts))
				{
					list.Add(key);
				}
			}
			return list;
		}

		private void UpdateLastRecomputeInformation(int tmId)
		{
			int tuCount = GetTuCount(tmId);
			DateTime val = DateTimeUtilities.Normalize(DateTime.Now);
			using (SQLiteCommand sQLiteCommand = CreateCommand("UPDATE translation_memories SET last_recompute_size = @recsize, last_recompute_date = @recdate WHERE id = @tmid", beginTransaction: true))
			{
				sQLiteCommand.Parameters.Add("@tmid", DbType.Int32).Value = tmId;
				sQLiteCommand.Parameters.Add("@recsize", DbType.Int32).Value = tuCount;
				sQLiteCommand.Parameters.Add("@recdate", DbType.String).Value = Utils.NormalizeToString(val);
				sQLiteCommand.ExecuteNonQuery();
			}
		}

		public int DeleteAllTus(int tmId)
		{
			int result;
			using (SQLiteCommand sQLiteCommand = CreateCommand("DELETE FROM translation_units", beginTransaction: true))
			{
				sQLiteCommand.Parameters.Add("@tm_id", DbType.Int32).Value = tmId;
				result = sQLiteCommand.ExecuteNonQuery();
				sQLiteCommand.CommandText = "UPDATE translation_memories SET last_recompute_date = NULL, last_recompute_size = NULL\r\n\t\t\t\t\tWHERE id = @tm_id";
				sQLiteCommand.ExecuteNonQuery();
			}
			using (SQLiteCommand sQLiteCommand2 = CreateCommand("DELETE FROM translation_unit_contexts", beginTransaction: true))
			{
				sQLiteCommand2.Parameters.Add("@id", DbType.Int32).Value = tmId;
				sQLiteCommand2.ExecuteNonQuery();
			}
			using (SQLiteCommand sQLiteCommand3 = CreateCommand("DELETE FROM string_attributes", beginTransaction: true))
			{
				sQLiteCommand3.Parameters.Add("@id", DbType.Int32).Value = tmId;
				sQLiteCommand3.ExecuteNonQuery();
			}
			using (SQLiteCommand sQLiteCommand4 = CreateCommand("DELETE FROM picklist_attributes", beginTransaction: true))
			{
				sQLiteCommand4.Parameters.Add("@id", DbType.Int32).Value = tmId;
				sQLiteCommand4.ExecuteNonQuery();
			}
			using (SQLiteCommand sQLiteCommand5 = CreateCommand("DELETE FROM numeric_attributes", beginTransaction: true))
			{
				sQLiteCommand5.Parameters.Add("@id", DbType.Int32).Value = tmId;
				sQLiteCommand5.ExecuteNonQuery();
			}
			using (SQLiteCommand sQLiteCommand6 = CreateCommand("DELETE FROM date_attributes", beginTransaction: true))
			{
				sQLiteCommand6.Parameters.Add("@id", DbType.Int32).Value = tmId;
				sQLiteCommand6.ExecuteNonQuery();
			}
			using (SQLiteCommand sQLiteCommand7 = CreateCommand("DELETE FROM translation_units", beginTransaction: true))
			{
				sQLiteCommand7.Parameters.Add("@id", DbType.Int32).Value = tmId;
				sQLiteCommand7.ExecuteNonQuery();
			}
			using (SQLiteCommand sQLiteCommand8 = CreateCommand("DELETE FROM fuzzy_data", beginTransaction: true))
			{
				sQLiteCommand8.Parameters.Add("@id", DbType.Int32).Value = tmId;
				sQLiteCommand8.ExecuteNonQuery();
			}
			ClearCache();
			UpdateTuCounts();
			return result;
		}

		public bool UpdateTuHeader(TranslationUnit tu, bool rewriteAttributes)
		{
			using (SQLiteCommand sQLiteCommand = CreateCommand("UPDATE translation_units SET change_date = @chd, change_user = @chu, last_used_date = @lud, \r\n\t\t\t\tlast_used_user = @luu, usage_counter = @usc, flags = @flags WHERE id = @id", beginTransaction: true))
			{
				sQLiteCommand.Parameters.Add("@id", DbType.Int32).Value = tu.Id;
				sQLiteCommand.Parameters.Add("@chd", DbType.String).Value = Utils.NormalizeToString(tu.ChangeDate);
				sQLiteCommand.Parameters.Add("@chu", DbType.String).Value = tu.ChangeUser;
				sQLiteCommand.Parameters.Add("@lud", DbType.String).Value = Utils.NormalizeToString(tu.LastUsedDate);
				sQLiteCommand.Parameters.Add("@luu", DbType.String).Value = tu.LastUsedUser;
				sQLiteCommand.Parameters.Add("@usc", DbType.Int32).Value = tu.UsageCounter;
				sQLiteCommand.Parameters.Add("@flags", DbType.Int32).Value = tu.Flags;
				if (sQLiteCommand.ExecuteNonQuery() <= 0)
				{
					return false;
				}
			}
			if (!rewriteAttributes)
			{
				return true;
			}
			DeleteAttributeValues(tu);
			AddAttributeValues(tu);
			return true;
		}

		private static string TuIdsToCommaSeparatedList(IEnumerable<int> keys)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (int key in keys)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(",");
				}
				stringBuilder.Append(key.ToString(CultureInfo.InvariantCulture));
			}
			return stringBuilder.ToString();
		}

		public bool[] UpdateTuAlignmentData(IEnumerable<TuAlignmentDataInternal> tuAlignmentDatas, int tmId)
		{
			Dictionary<int, List<long>> dictionary = new Dictionary<int, List<long>>();
			List<bool> list = new List<bool>();
			using (SQLiteCommand sQLiteCommand = CreateCommand(string.Empty, beginTransaction: true))
			{
				sQLiteCommand.CommandText = "UPDATE translation_units SET alignment_data = @alignment_data, align_model_date = @align_model_date WHERE id = @id AND insert_date = @insert_date";
				SQLiteParameter sQLiteParameter = sQLiteCommand.Parameters.Add("@id", DbType.Int32);
				SQLiteParameter sQLiteParameter2 = sQLiteCommand.Parameters.Add("@tmId", DbType.Int32);
				SQLiteParameter sQLiteParameter3 = sQLiteCommand.Parameters.Add("@alignment_data", DbType.Binary);
				SQLiteParameter sQLiteParameter4 = sQLiteCommand.Parameters.Add("@align_model_date", DbType.String);
				SQLiteParameter sQLiteParameter5 = sQLiteCommand.Parameters.Add("@insert_date", DbType.String);
				sQLiteParameter2.Value = tmId;
				foreach (TuAlignmentDataInternal tuAlignmentData in tuAlignmentDatas)
				{
					sQLiteParameter.Value = tuAlignmentData.tuId.Id;
					sQLiteParameter3.Value = ((tuAlignmentData.alignmentData == null) ? ((object)DBNull.Value) : ((object)tuAlignmentData.alignmentData));
					sQLiteParameter4.Value = (tuAlignmentData.alignModelDate.HasValue ? ((IConvertible)Utils.NormalizeToString(tuAlignmentData.alignModelDate.Value)) : ((IConvertible)DBNull.Value));
					sQLiteParameter5.Value = Utils.NormalizeToString(tuAlignmentData.insertDate);
					if (sQLiteCommand.ExecuteNonQuery() != 1)
					{
						list.Add(item: false);
					}
					else
					{
						List<long> list2 = new List<long>();
						dictionary.Add(tuAlignmentData.tuId.Id, list2);
						list2.AddRange(tuAlignmentData.hashes);
						list.Add(item: true);
					}
				}
			}
			Dictionary<int, List<long>>.KeyCollection keys = dictionary.Keys;
			using (SQLiteCommand sQLiteCommand2 = CreateCommand(string.Empty, beginTransaction: true))
			{
				string str = TuIdsToCommaSeparatedList(keys);
				sQLiteCommand2.CommandText = "DELETE FROM translation_unit_fragments WHERE translation_unit_id IN (" + str + ")";
				sQLiteCommand2.ExecuteNonQuery();
			}
			using (SQLiteCommand sQLiteCommand3 = CreateCommand("INSERT INTO translation_unit_fragments (translation_unit_id, fragment_hash) VALUES (@id, @fragment_hash)", beginTransaction: true))
			{
				SQLiteParameter sQLiteParameter6 = sQLiteCommand3.Parameters.Add("@id", DbType.Int32);
				SQLiteParameter sQLiteParameter7 = sQLiteCommand3.Parameters.Add("@fragment_hash", DbType.Int64);
				foreach (int item in keys)
				{
					sQLiteParameter6.Value = item;
					foreach (long item2 in dictionary[item])
					{
						sQLiteParameter7.Value = item2;
						sQLiteCommand3.ExecuteNonQuery();
					}
				}
			}
			InvalidateAlignedPredatedTuCountCache();
			return list.ToArray();
		}

		private static void Swap<T>(ref T lhs, ref T rhs)
		{
			T val = lhs;
			lhs = rhs;
			rhs = val;
		}

		private void ProcessAdditionalHashesForTu(string contextColNameToSelect, string contextColNameToFilter, string newHash, SQLiteCommand addCmd, SQLiteParameter idParm, SQLiteParameter contextParmToAdd, SQLiteParameter contextParmToExpand, long[] otherNewHashes, Dictionary<long, long[]> hashesToExpand)
		{
			if (otherNewHashes.Length != 0)
			{
				using (SQLiteCommand sQLiteCommand = CreateCommand("select id, " + contextColNameToSelect + " from translation_units inner join translation_unit_contexts on translation_unit_id = id where " + contextColNameToFilter + " = " + newHash, beginTransaction: true))
				{
					using (SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader())
					{
						while (sQLiteDataReader.Read())
						{
							int @int = sQLiteDataReader.GetInt32(0);
							long int2 = sQLiteDataReader.GetInt64(1);
							idParm.Value = @int;
							foreach (long num in otherNewHashes)
							{
								contextParmToExpand.Value = int2;
								contextParmToAdd.Value = num;
								addCmd.ExecuteNonQuery();
								if (hashesToExpand.ContainsKey(int2))
								{
									long[] array = hashesToExpand[int2];
									foreach (long num2 in array)
									{
										contextParmToExpand.Value = num2;
										addCmd.ExecuteNonQuery();
									}
								}
							}
						}
					}
				}
			}
		}

		private Dictionary<long, long[]> GetHashExpansionMap(List<int[]> tuIdsWithDupHashes, bool target)
		{
			Dictionary<long, long[]> dictionary = new Dictionary<long, long[]>();
			foreach (int[] tuIdsWithDupHash in tuIdsWithDupHashes)
			{
				string text = TuIdsToCommaSeparatedList(tuIdsWithDupHash);
				int num = tuIdsWithDupHash.Min();
				long num2 = -1L;
				HashSet<long> hashSet = new HashSet<long>();
				string text2 = "source_hash";
				if (target)
				{
					text2 = "target_hash";
				}
				using (SQLiteCommand sQLiteCommand = CreateCommand("select id, " + text2 + " from translation_units where id in (" + text + ")", beginTransaction: false))
				{
					using (SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader())
					{
						while (sQLiteDataReader.Read())
						{
							int @int = sQLiteDataReader.GetInt32(0);
							long int2 = sQLiteDataReader.GetInt64(1);
							if (@int == num)
							{
								num2 = int2;
							}
							else if (hashSet.Count < 10)
							{
								hashSet.Add(int2);
							}
						}
					}
				}
				hashSet.Remove(num2);
				if (hashSet.Count != 0)
				{
					if (dictionary.TryGetValue(num2, out long[] value))
					{
						List<long> list = new List<long>(value);
						list.AddRange(hashSet);
						dictionary[num2] = list.ToArray();
					}
					else
					{
						dictionary.Add(num2, hashSet.ToArray());
					}
				}
			}
			return dictionary;
		}

		public void AddDeduplicatedContextHashes(int tmId, ref List<int[]> tuIdsWithDupSourceHashes, ref List<int[]> tuIdsWithDupTargetHashes)
		{
			TextContextMatchType textContextMatchType = TextContextMatchType.PrecedingSourceAndTarget;
			if (tuIdsWithDupTargetHashes == null)
			{
				textContextMatchType = TextContextMatchType.PrecedingAndFollowingSource;
			}
			string text = "insert or ignore into translation_unit_contexts(translation_unit_id, left_source_context, left_target_context) select @id, @context1, @context2";
			if (textContextMatchType == TextContextMatchType.PrecedingSourceAndTarget)
			{
				text += " where exists (select id from translation_units where translation_memory_id = @tm_id AND source_hash = @context1 and target_hash = @context2)";
			}
			using (SQLiteCommand sQLiteCommand = CreateCommand(text, beginTransaction: true))
			{
				sQLiteCommand.Parameters.Add("@tm_id", DbType.Int32).Value = tmId;
				SQLiteParameter idParm = sQLiteCommand.Parameters.Add("@id", DbType.Int32);
				SQLiteParameter sQLiteParameter = sQLiteCommand.Parameters.Add("@context1", DbType.Int64);
				SQLiteParameter sQLiteParameter2 = sQLiteCommand.Parameters.Add("@context2", DbType.Int64);
				Dictionary<long, long[]> hashExpansionMap = GetHashExpansionMap(tuIdsWithDupSourceHashes, target: false);
				tuIdsWithDupSourceHashes = null;
				Dictionary<long, long[]> dictionary = hashExpansionMap;
				if (textContextMatchType == TextContextMatchType.PrecedingSourceAndTarget)
				{
					dictionary = GetHashExpansionMap(tuIdsWithDupTargetHashes, target: true);
					tuIdsWithDupTargetHashes = null;
				}
				string rhs = "left_target_context";
				string lhs = "left_source_context";
				SQLiteParameter lhs2 = sQLiteParameter2;
				SQLiteParameter rhs2 = sQLiteParameter;
				foreach (long key in hashExpansionMap.Keys)
				{
					ProcessAdditionalHashesForTu(rhs, lhs, key.ToString(CultureInfo.InvariantCulture), sQLiteCommand, idParm, rhs2, lhs2, hashExpansionMap[key], dictionary);
				}
				Swap(ref lhs, ref rhs);
				Swap(ref lhs2, ref rhs2);
				foreach (long key2 in dictionary.Keys)
				{
					ProcessAdditionalHashesForTu(rhs, lhs, key2.ToString(CultureInfo.InvariantCulture), sQLiteCommand, idParm, rhs2, lhs2, dictionary[key2], hashExpansionMap);
				}
			}
		}

		public List<int[]> GetDuplicateSegmentHashes(int tmId, bool target, long? currentSigHash)
		{
			CheckVersion();
			string str = "null";
			if (_canReportReindexRequired)
			{
				str = "tokenization_sig_hash";
			}
			string cmdText = string.Format("select id, {0}, " + str + " from translation_units where {0} in \r\n                    (select {0} from translation_units \r\n                    group by {0}\r\n                    having count(*) >1) order by {0}\r\n                    ", target ? "target_hash" : "source_hash");
			List<int[]> list = new List<int[]>();
			Dictionary<int, long?> dictionary = null;
			long num = -1L;
			using (SQLiteCommand sQLiteCommand = CreateCommand(cmdText, beginTransaction: false))
			{
				using (SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader())
				{
					while (sQLiteDataReader.Read())
					{
						int @int = sQLiteDataReader.GetInt32(0);
						long int2 = sQLiteDataReader.GetInt64(1);
						long? value = null;
						if (!sQLiteDataReader.IsDBNull(2))
						{
							value = sQLiteDataReader.GetInt64(2);
						}
						if (dictionary == null || int2 != num)
						{
							if (dictionary != null && (!currentSigHash.HasValue || dictionary.Values.Any((long? x) => !x.HasValue || x.Value != currentSigHash)))
							{
								list.Add(dictionary.Keys.ToArray());
							}
							dictionary = new Dictionary<int, long?>();
							num = int2;
						}
						dictionary.Add(@int, value);
					}
				}
			}
			if (dictionary != null && (!currentSigHash.HasValue || dictionary.Values.Any((long? x) => !x.HasValue || x.Value != currentSigHash)))
			{
				list.Add(dictionary.Keys.ToArray());
			}
			return list;
		}

		public void UpdateTuIndices(List<TranslationUnit> tus, FuzzyIndexes indexes, long tokenizationSignatureHash, TextContextMatchType textContextMatchType)
		{
			CheckVersion();
			if (tus.Count != 0)
			{
				int tmId = tus[0].TranslationMemoryId;
				if (tus.Any((TranslationUnit x) => x.TranslationMemoryId != tmId))
				{
					throw new Exception("Attempt to update tu indices in multiple TMs");
				}
				List<int> list = tus.Select((TranslationUnit x) => x.Id).ToList();
				string str = TuIdsToCommaSeparatedList(list);
				UnindexTus(list, tmId);
				string commandText = "\r\n\t\t\t\tUPDATE translation_units \r\n\t\t\t\tSET source_hash = @source_hash, \r\n\t\t\t\t\tsource_segment = @source_text, \r\n\t\t\t\t\ttarget_hash = @target_hash, \r\n\t\t\t\t\ttarget_segment = @target_text " + (SerializesTokens() ? ", source_token_data = @source_token_data, target_token_data = @target_token_data " : string.Empty) + (_canReportReindexRequired ? ", tokenization_sig_hash = @sighash " : string.Empty) + "WHERE id = @id";
				string cmdText = "select id, source_hash, target_hash from translation_units where id in (" + str + ")";
				Dictionary<int, Pair<long>> dictionary = new Dictionary<int, Pair<long>>();
				DateTime val = DateTimeUtilities.Normalize(DateTime.Now);
				bool canChooseTextContextMatchType = _canChooseTextContextMatchType;
				List<TranslationUnit> list2 = new List<TranslationUnit>();
				Dictionary<long, long> dictionary2 = new Dictionary<long, long>();
				Dictionary<long, long> dictionary3 = new Dictionary<long, long>();
				using (SQLiteCommand sQLiteCommand = CreateCommand(cmdText, beginTransaction: true))
				{
					sQLiteCommand.Parameters.Add("@tmId", DbType.Int32).Value = tmId;
					using (SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader())
					{
						while (sQLiteDataReader.Read())
						{
							int @int = sQLiteDataReader.GetInt32(0);
							long int2 = sQLiteDataReader.GetInt64(1);
							long int3 = sQLiteDataReader.GetInt64(2);
							dictionary.Add(@int, new Pair<long>(int2, int3));
						}
					}
					sQLiteCommand.CommandText = commandText;
					SQLiteParameter sQLiteParameter = sQLiteCommand.Parameters.Add("@id", DbType.Int32);
					SQLiteParameter sQLiteParameter2 = sQLiteCommand.Parameters.Add("@source_hash", DbType.Int64);
					SQLiteParameter sQLiteParameter3 = sQLiteCommand.Parameters.Add("@target_hash", DbType.Int64);
					SQLiteParameter sQLiteParameter4 = sQLiteCommand.Parameters.Add("@target_text", DbType.String);
					SQLiteParameter sQLiteParameter5 = sQLiteCommand.Parameters.Add("@source_text", DbType.String);
					if (_canReportReindexRequired)
					{
						sQLiteCommand.Parameters.Add("@sighash", DbType.Int64).Value = tokenizationSignatureHash;
					}
					SQLiteParameter sQLiteParameter6 = null;
					SQLiteParameter sQLiteParameter7 = null;
					if (SerializesTokens())
					{
						sQLiteParameter6 = sQLiteCommand.Parameters.Add("@source_token_data", DbType.Binary);
						sQLiteParameter7 = sQLiteCommand.Parameters.Add("@target_token_data", DbType.Binary);
					}
					foreach (TranslationUnit tu in tus)
					{
						sQLiteParameter.Value = tu.Id;
						sQLiteParameter2.Value = tu.Source.Hash;
						sQLiteParameter3.Value = tu.Target.Hash;
						sQLiteParameter5.Value = tu.Source.Text;
						sQLiteParameter4.Value = tu.Target.Text;
						if (SerializesTokens())
						{
							if (sQLiteParameter6 != null)
							{
								sQLiteParameter6.Value = tu.SourceTokenData;
							}
							if (sQLiteParameter7 != null)
							{
								sQLiteParameter7.Value = tu.TargetTokenData;
							}
						}
						sQLiteCommand.ExecuteNonQuery();
						IndexTu(tu, indexes, canChooseTextContextMatchType);
					}
					foreach (TranslationUnit tu2 in tus)
					{
						if (dictionary.ContainsKey(tu2.Id))
						{
							Pair<long> pair = dictionary[tu2.Id];
							long left = pair.Left;
							long right = pair.Right;
							if (left != tu2.Source.Hash && !dictionary2.ContainsKey(left) && left != 0L)
							{
								dictionary2.Add(left, tu2.Source.Hash);
							}
							if (right != tu2.Target.Hash && !dictionary3.ContainsKey(right) && right != 0L)
							{
								dictionary3.Add(right, tu2.Target.Hash);
							}
							if ((left != tu2.Source.Hash || right != tu2.Target.Hash) && SupportsAlignmentData())
							{
								list2.Add(tu2);
							}
						}
					}
					if (list2.Count > 0)
					{
						str = TuIdsToCommaSeparatedList(list2.Select((TranslationUnit x) => x.Id).ToList());
						sQLiteCommand.CommandText = "DELETE from translation_unit_fragments where translation_unit_id in (" + str + ")";
						sQLiteCommand.ExecuteNonQuery();
						sQLiteCommand.CommandText = "\r\n\t\t\t\t        UPDATE translation_units \r\n\t\t\t\t        SET alignment_data = null, align_model_date = null, insert_date = @insert_date\r\n                            WHERE id IN (" + str + ")";
						sQLiteCommand.Parameters.Add("@insert_date", DbType.String).Value = Utils.NormalizeToString(val);
						sQLiteCommand.ExecuteNonQuery();
						if (canChooseTextContextMatchType)
						{
							sQLiteCommand.CommandText = "INSERT INTO translation_unit_fragments (translation_unit_id, fragment_hash) values (@id, @relaxedhash)";
							SQLiteParameter sQLiteParameter8 = sQLiteCommand.Parameters.Add("@relaxedhash", DbType.Int64);
							foreach (TranslationUnit item in list2)
							{
								sQLiteParameter8.Value = item.Source.RelaxedHash;
								sQLiteParameter.Value = item.Id;
								sQLiteCommand.ExecuteNonQuery();
							}
						}
						InvalidateAlignedPredatedTuCountCache();
					}
					SQLiteParameter sQLiteParameter9 = sQLiteCommand.Parameters.Add("@context1", DbType.Int64);
					SQLiteParameter sQLiteParameter10 = sQLiteCommand.Parameters.Add("@oldcontext1", DbType.Int64);
					SQLiteParameter sQLiteParameter11 = sQLiteCommand.Parameters.Add("@context2", DbType.Int64);
					SQLiteParameter sQLiteParameter12 = sQLiteCommand.Parameters.Add("@oldcontext2", DbType.Int64);
					sQLiteCommand.CommandText = "update or ignore translation_unit_contexts set left_source_context = @context1 where left_source_context = @oldcontext1";
					foreach (KeyValuePair<long, long> item2 in dictionary2)
					{
						sQLiteParameter10.Value = item2.Key;
						sQLiteParameter9.Value = item2.Value;
						sQLiteCommand.ExecuteNonQuery();
					}
					sQLiteCommand.CommandText = "update or ignore translation_unit_contexts set left_target_context = @context2 where left_target_context = @oldcontext2";
					if (textContextMatchType == TextContextMatchType.PrecedingAndFollowingSource)
					{
						foreach (KeyValuePair<long, long> item3 in dictionary2)
						{
							sQLiteParameter12.Value = item3.Key;
							sQLiteParameter11.Value = item3.Value;
							sQLiteCommand.ExecuteNonQuery();
						}
					}
					else
					{
						foreach (KeyValuePair<long, long> item4 in dictionary3)
						{
							sQLiteParameter12.Value = item4.Key;
							sQLiteParameter11.Value = item4.Value;
							sQLiteCommand.ExecuteNonQuery();
						}
					}
				}
			}
		}

		public void ClearFuzzyIndex(FuzzyIndexes index)
		{
			throw new NotImplementedException();
		}

		private void IndexTu(TranslationUnit tu, FuzzyIndexes indexes, bool includeFragmentHash)
		{
			string value = null;
			string value2 = null;
			string value3 = null;
			string value4 = null;
			if ((indexes & FuzzyIndexes.SourceWordBased) == FuzzyIndexes.SourceWordBased && tu.Source.Features != null && tu.Source.Features.Count > 0)
			{
				value = GetFeatureString(tu.Source.Features);
				AddFeaturesToCache(tu.Id, tu.Source.Features, FuzzyIndexes.SourceWordBased);
			}
			if ((indexes & FuzzyIndexes.SourceCharacterBased) == FuzzyIndexes.SourceCharacterBased)
			{
				value2 = GetFeatureString(tu.Source.ConcordanceFeatures);
				AddFeaturesToCache(tu.Id, tu.Source.ConcordanceFeatures, FuzzyIndexes.SourceCharacterBased);
			}
			if ((indexes & FuzzyIndexes.TargetCharacterBased) == FuzzyIndexes.TargetCharacterBased)
			{
				value3 = GetFeatureString(tu.Target.ConcordanceFeatures);
				AddFeaturesToCache(tu.Id, tu.Target.ConcordanceFeatures, FuzzyIndexes.TargetCharacterBased);
			}
			if ((indexes & FuzzyIndexes.TargetWordBased) == FuzzyIndexes.TargetWordBased)
			{
				value4 = GetFeatureString(tu.Target.Features);
				AddFeaturesToCache(tu.Id, tu.Target.Features, FuzzyIndexes.TargetWordBased);
			}
			using (SQLiteCommand sQLiteCommand = CreateCommand("INSERT INTO fuzzy_data(translation_memory_id, translation_unit_id, fi1, fi2, fi4, fi8) \r\n\t\t\t\t\tVALUES(@tm_id, @tu_id, @fi1, @fi2, @fi4, @fi8)", beginTransaction: true))
			{
				sQLiteCommand.Parameters.Add("@tm_id", DbType.Int32).Value = tu.TranslationMemoryId;
				sQLiteCommand.Parameters.Add("@tu_id", DbType.Int32).Value = tu.Id;
				sQLiteCommand.Parameters.Add("@fi1", DbType.String).Value = value;
				sQLiteCommand.Parameters.Add("@fi2", DbType.String).Value = value2;
				sQLiteCommand.Parameters.Add("@fi4", DbType.String).Value = value3;
				sQLiteCommand.Parameters.Add("@fi8", DbType.String).Value = value4;
				sQLiteCommand.ExecuteNonQuery();
			}
			if (includeFragmentHash)
			{
				using (SQLiteCommand sQLiteCommand2 = CreateCommand("", beginTransaction: true))
				{
					sQLiteCommand2.Parameters.Clear();
					sQLiteCommand2.Parameters.Add("@tu_id", DbType.Int32).Value = tu.Id;
					sQLiteCommand2.Parameters.Add("@relaxedhash", DbType.Int64).Value = tu.Source.RelaxedHash;
					sQLiteCommand2.CommandText = "DELETE FROM translation_unit_fragments WHERE translation_unit_id = @tu_id AND fragment_hash = @relaxedhash";
					sQLiteCommand2.ExecuteNonQuery();
					sQLiteCommand2.CommandText = "INSERT INTO translation_unit_fragments (translation_unit_id, fragment_hash) values (@tu_id, @relaxedhash)";
					sQLiteCommand2.ExecuteNonQuery();
				}
			}
		}

		private void UnindexTus(IEnumerable<int> tuIds, int tmId)
		{
			List<int> list = tuIds.ToList();
			string str = TuIdsToCommaSeparatedList(list);
			foreach (int item in list)
			{
				InMemoryFuzzyIndex[] fuzzyIndexCaches = _currentIndexCache.FuzzyIndexCaches;
				for (int i = 0; i < fuzzyIndexCaches.Length; i++)
				{
					fuzzyIndexCaches[i]?.Delete(item);
				}
			}
			using (SQLiteCommand sQLiteCommand = CreateCommand("DELETE FROM fuzzy_data WHERE translation_unit_id in (" + str + ")", beginTransaction: true))
			{
				sQLiteCommand.Parameters.Add("@tmId", DbType.Int32).Value = tmId;
				sQLiteCommand.ExecuteNonQuery();
			}
		}

		private void UnindexTu(int tmId, int tuId)
		{
			using (SQLiteCommand sQLiteCommand = CreateCommand("DELETE FROM fuzzy_data WHERE translation_unit_id = @id", beginTransaction: true))
			{
				sQLiteCommand.Parameters.Add("@tmId", DbType.Int32).Value = tmId;
				sQLiteCommand.Parameters.Add("@id", DbType.Int32).Value = tuId;
				sQLiteCommand.ExecuteNonQuery();
			}
			InMemoryFuzzyIndex[] fuzzyIndexCaches = _currentIndexCache.FuzzyIndexCaches;
			for (int i = 0; i < fuzzyIndexCaches.Length; i++)
			{
				fuzzyIndexCaches[i]?.Delete(tuId);
			}
		}

		public TuContexts GetTextContexts(int tmId, int tuId)
		{
			using (SQLiteCommand sQLiteCommand = CreateCommand("SELECT left_source_context, left_target_context FROM translation_unit_contexts WHERE translation_unit_id = @tu_id", beginTransaction: false))
			{
				sQLiteCommand.Parameters.Add("@tu_id", DbType.Int32).Value = tuId;
				return GetTextContexts(sQLiteCommand);
			}
		}

		public bool DeleteContexts(int tmId, int tuId)
		{
			using (SQLiteCommand sQLiteCommand = CreateCommand("DELETE FROM translation_unit_contexts WHERE translation_unit_id = @tu_id", beginTransaction: true))
			{
				sQLiteCommand.Parameters.Add("@tu_id", DbType.Int32).Value = tuId;
				return sQLiteCommand.ExecuteNonQuery() > 0;
			}
		}

		public void AddContexts(int tmId, int tuId, TuContexts contexts)
		{
			if (contexts != null && contexts.Length != 0)
			{
				using (SQLiteCommand sQLiteCommand = CreateCommand("INSERT OR IGNORE INTO translation_unit_contexts(translation_unit_id, left_source_context, left_target_context) \r\n\t\t\t\tVALUES(@tu_id, @left_sh, @left_th)", beginTransaction: true))
				{
					sQLiteCommand.Parameters.Add("@tu_id", DbType.Int32).Value = tuId;
					sQLiteCommand.Parameters.Add("@left_sh", DbType.Int64);
					sQLiteCommand.Parameters.Add("@left_th", DbType.Int64);
					sQLiteCommand.Prepare();
					foreach (TuContext value in contexts.Values)
					{
						sQLiteCommand.Parameters[1].Value = value.Context1;
						sQLiteCommand.Parameters[2].Value = value.Context2;
						sQLiteCommand.ExecuteNonQuery();
					}
				}
			}
		}

		public void AddIdContexts(int tmId, int tuId, TuIdContexts contexts)
		{
			if (contexts != null && contexts.Length != 0)
			{
				using (SQLiteCommand sQLiteCommand = CreateCommand("INSERT OR IGNORE INTO translation_unit_idcontexts(translation_unit_id, idcontext) \r\n\t\t\t\tVALUES(@tu_id, @idcontext)", beginTransaction: true))
				{
					sQLiteCommand.Parameters.Add("@tu_id", DbType.Int32).Value = tuId;
					sQLiteCommand.Parameters.Add("@idcontext", DbType.String);
					sQLiteCommand.Prepare();
					foreach (string value in contexts.Values)
					{
						sQLiteCommand.Parameters[1].Value = value;
						sQLiteCommand.ExecuteNonQuery();
					}
				}
			}
		}

		public List<TranslationUnit> FuzzySearch(int tmId, List<int> feature, FuzzyIndexes index, int minScore, int maxHits, bool concordance, int lastTuId, TuContextData tuContextData, bool descendingOrder)
		{
			EnsureFuzzyIndexLoaded(index);
			string alignmentDataColspec = GetAlignmentDataColspec();
			ScoringMethod scoringMethod = (!concordance) ? ScoringMethod.Dice : ScoringMethod.Query;
			List<Hit> list = _currentIndexCache.FuzzyIndexCaches[(int)index].Search(feature, maxHits, minScore, lastTuId, scoringMethod, null, descendingOrder);
			if (list == null || list.Count == 0)
			{
				return new List<TranslationUnit>();
			}
			StringBuilder stringBuilder = new StringBuilder("SELECT id, guid, translation_memory_id, \r\n\t\t\t\tsource_hash, source_segment, 0, 0, \r\n\t\t\t\ttarget_hash, target_segment, 0, 0, \r\n\t\t\t\tcreation_date, creation_user, change_date, change_user, last_used_date, last_used_user, usage_counter, flags " + (SerializesTokens() ? ", source_token_data, target_token_data " : ", null, null ") + alignmentDataColspec + ", 0, null, null FROM translation_units WHERE id IN (");
			bool flag = true;
			foreach (Hit item in list)
			{
				if (flag)
				{
					flag = false;
				}
				else
				{
					stringBuilder.Append(",");
				}
				stringBuilder.Append(item.Key.ToString(CultureInfo.InvariantCulture));
			}
			stringBuilder.Append(")");
			using (SQLiteCommand sQLiteCommand = CreateCommand(stringBuilder.ToString(), beginTransaction: false))
			{
				sQLiteCommand.Parameters.Add("@tm_id", DbType.Int32).Value = tmId;
				return GetTuSet(sQLiteCommand, maxHits, tuContextData);
			}
		}

		public List<TranslationUnit> SubsegmentSearch(int tmId, List<long> fragmentHashes, byte minFragmentLength, byte minSigWords, int maxHits, Dictionary<int, HashSet<long>> hashesPerTu)
		{
			if (!SupportsAlignmentData())
			{
				throw new Exception("SubsegmentSearchAdvanced called but SupportsAlignmentData == false");
			}
			if (!SerializesTokens())
			{
				throw new Exception("SubsegmentSearchAdvanced called but SerializesTokens == false");
			}
			int count = fragmentHashes.Count;
			StringBuilder stringBuilder = new StringBuilder();
			using (SQLiteCommand sQLiteCommand = CreateCommand("", beginTransaction: false))
			{
				HashSet<long> hashSet = new HashSet<long>();
				for (int i = 0; i < fragmentHashes.Count; i += count)
				{
					stringBuilder.Clear();
					int num = Math.Min(count, fragmentHashes.Count - i);
					for (int j = 0; j < num; j++)
					{
						if (stringBuilder.Length > 0)
						{
							stringBuilder.Append(",");
						}
						stringBuilder.Append(fragmentHashes[i + j]);
					}
					string text2 = sQLiteCommand.CommandText = "select distinct fragment_hash from translation_unit_fragments \r\n                                        where fragment_hash in (" + stringBuilder?.ToString() + ")";
					using (SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader())
					{
						while (sQLiteDataReader.Read())
						{
							hashSet.Add(sQLiteDataReader.GetInt64(0));
						}
					}
				}
				if (hashSet.Count == 0)
				{
					return new List<TranslationUnit>();
				}
				List<long> list = hashSet.ToList();
				StringBuilder stringBuilder2 = new StringBuilder();
				for (int k = 0; k < list.Count; k += 400)
				{
					stringBuilder.Clear();
					int num2 = Math.Min(400, list.Count - k);
					for (int l = 0; l < num2; l++)
					{
						long num3 = list[k + l];
						string value = $"select * from (SELECT translation_unit_id, fragment_hash\r\n\t\t\t\t                            FROM translation_unit_fragments \r\n                                            WHERE fragment_hash = {num3} order by translation_unit_id desc limit {maxHits})";
						if (stringBuilder.Length > 0)
						{
							stringBuilder.Append("\r\n                                union\r\n                                ");
						}
						stringBuilder.Append(value);
					}
					sQLiteCommand.CommandText = stringBuilder.ToString();
					using (SQLiteDataReader sQLiteDataReader2 = sQLiteCommand.ExecuteReader())
					{
						while (sQLiteDataReader2.Read())
						{
							int @int = sQLiteDataReader2.GetInt32(0);
							long int2 = sQLiteDataReader2.GetInt64(1);
							if (hashesPerTu.TryGetValue(@int, out HashSet<long> value2))
							{
								value2.Add(int2);
							}
							else
							{
								hashesPerTu.Add(@int, new HashSet<long>
								{
									int2
								});
								if (stringBuilder2.Length > 0)
								{
									stringBuilder2.Append(",");
								}
								stringBuilder2.Append(@int);
							}
						}
					}
				}
				if (hashesPerTu.Keys.Count == 0)
				{
					return new List<TranslationUnit>();
				}
				sQLiteCommand.Parameters.Add("@tm_id", DbType.Int32).Value = tmId;
				string text4 = sQLiteCommand.CommandText = "SELECT DISTINCT id, guid, translation_memory_id, \r\n\t\t\t\t    source_hash, source_segment, 0, 0, \r\n\t\t\t\t    target_hash, target_segment, 0, 0, \r\n\t\t\t\t    creation_date,  creation_user, change_date, change_user, last_used_date, last_used_user, usage_counter, flags \r\n                    , source_token_data, target_token_data \r\n                    , alignment_data, align_model_date, insert_date \r\n                    , 0, null, null\r\n                    FROM translation_units \r\n                    WHERE id  IN (" + stringBuilder2?.ToString() + ") ";
				return GetTuSet(sQLiteCommand, hashesPerTu.Count, new TuContextData());
			}
		}

		public List<TranslationUnit> ExactSearch(int tmId, List<long> sourceHashes, int maxHits)
		{
			CheckVersion();
			if (_canChooseTextContextMatchType)
			{
				return null;
			}
			int count = sourceHashes.Count;
			string alignmentDataColspec = GetAlignmentDataColspec();
			HashSet<long> hashSet = new HashSet<long>();
			Dictionary<int, List<long>> dictionary = new Dictionary<int, List<long>>();
			using (SQLiteCommand sQLiteCommand = CreateCommand("", beginTransaction: false))
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < sourceHashes.Count; i += count)
				{
					stringBuilder.Clear();
					int num = Math.Min(count, sourceHashes.Count - i);
					for (int j = 0; j < num; j++)
					{
						if (stringBuilder.Length > 0)
						{
							stringBuilder.Append(",");
						}
						stringBuilder.Append(sourceHashes[i + j]);
					}
					string text2 = sQLiteCommand.CommandText = "select distinct source_hash from translation_units \r\n                                        where source_hash in (" + stringBuilder?.ToString() + ")";
					using (SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader())
					{
						while (sQLiteDataReader.Read())
						{
							hashSet.Add(sQLiteDataReader.GetInt64(0));
						}
					}
				}
				if (hashSet.Count == 0)
				{
					return new List<TranslationUnit>();
				}
				List<long> list = hashSet.ToList();
				StringBuilder stringBuilder2 = new StringBuilder();
				for (int k = 0; k < list.Count; k += 400)
				{
					stringBuilder.Clear();
					int num2 = Math.Min(400, list.Count - k);
					for (int l = 0; l < num2; l++)
					{
						long num3 = list[k + l];
						string value = $"select * from (SELECT id, source_hash\r\n\t\t\t\t                                FROM translation_units \r\n                                                WHERE source_hash = {num3} \r\n                                                order by id desc \r\n                                                limit {maxHits})";
						if (stringBuilder.Length > 0)
						{
							stringBuilder.Append("\r\n                                union\r\n                                ");
						}
						stringBuilder.Append(value);
					}
					sQLiteCommand.CommandText = stringBuilder.ToString();
					stringBuilder.Clear();
					using (SQLiteDataReader sQLiteDataReader2 = sQLiteCommand.ExecuteReader())
					{
						while (sQLiteDataReader2.Read())
						{
							int @int = sQLiteDataReader2.GetInt32(0);
							long int2 = sQLiteDataReader2.GetInt64(1);
							if (dictionary.TryGetValue(@int, out List<long> value2))
							{
								value2.Add(int2);
							}
							else
							{
								dictionary.Add(@int, new List<long>
								{
									int2
								});
								if (stringBuilder2.Length > 0)
								{
									stringBuilder2.Append(",");
								}
								stringBuilder2.Append(@int);
							}
						}
					}
				}
				if (dictionary.Keys.Count == 0)
				{
					return new List<TranslationUnit>();
				}
				sQLiteCommand.Parameters.Add("@tm_id", DbType.Int32).Value = tmId;
				string text4 = sQLiteCommand.CommandText = "SELECT DISTINCT id, guid, translation_memory_id, \r\n\t\t\t\t    source_hash, source_segment, 0, 0, \r\n\t\t\t\t    target_hash, target_segment, 0, 0, \r\n\t\t\t\t    creation_date,  creation_user, change_date, change_user, last_used_date, last_used_user, usage_counter, flags " + (SerializesTokens() ? ", source_token_data, target_token_data " : ", null, null ") + alignmentDataColspec + ", 0, null, null FROM translation_units \r\n                    WHERE id  IN (" + stringBuilder2?.ToString() + ")";
				return GetTuSet(sQLiteCommand, dictionary.Count, new TuContextData());
			}
		}

		private string GetAlignmentDataColspec()
		{
			if (!SupportsAlignmentData())
			{
				return ", null, null, null ";
			}
			return ", alignment_data, align_model_date, insert_date ";
		}

		public void AddorUpdateLastSearch(int tmId, List<int> tuIds, DateTime lastSearch)
		{
			throw new NotImplementedException();
		}

		public List<TranslationUnit> ExactSearch(int tmId, long sourceHash, long targetHash, int maxHits, DateTime lastChangeDate, int skipRows, TuContextData tuContextData, bool descendingOrder, List<int> tuIdsToSkip)
		{
			string alignmentDataColspec = GetAlignmentDataColspec();
			string text = string.Format("SELECT id, guid, translation_memory_id, \r\n\t\t\t\tsource_hash, source_segment, 0, 0, \r\n\t\t\t\ttarget_hash, target_segment, 0, 0, \r\n\t\t\t\tcreation_date,  creation_user, change_date, change_user, last_used_date, last_used_user, usage_counter, flags " + (SerializesTokens() ? ", source_token_data, target_token_data " : ", null, null ") + alignmentDataColspec + ", 0, null, null FROM translation_units WHERE translation_memory_id = @tm_id AND source_hash = @source_hash AND \r\n                change_date {0} @last_change_date", descendingOrder ? "<=" : ">=");
			if (tuIdsToSkip != null)
			{
				string str = TuIdsToCommaSeparatedList(tuIdsToSkip);
				text = text + " AND id NOT IN (" + str + ")";
			}
			if (!string.IsNullOrEmpty(tuContextData.IdContext))
			{
				if (tuIdsToSkip != null)
				{
					throw new Exception("tuContextData and tuIdsToSkip should not both be non-null");
				}
				text = string.Format("SELECT id, guid, translation_memory_id, \r\n\t\t\t\t        source_hash, source_segment, 0, 0, \r\n\t\t\t\t        target_hash, target_segment, 0, 0, \r\n\t\t\t\t        creation_date,  creation_user, change_date, change_user, last_used_date, last_used_user, usage_counter, flags " + (SerializesTokens() ? ", source_token_data, target_token_data " : ", null, null ") + alignmentDataColspec + ", 0, null, null FROM translation_units left outer join translation_unit_idcontexts as i on i.translation_unit_id = id AND idcontext = @idcontext\r\n                                        WHERE source_hash = @source_hash  AND\r\n                        change_date {0} @last_change_date", descendingOrder ? "<=" : ">=");
			}
			using (SQLiteCommand sQLiteCommand = CreateCommand(text, beginTransaction: false))
			{
				sQLiteCommand.Parameters.Add("@tm_id", DbType.Int32).Value = tmId;
				sQLiteCommand.Parameters.Add("@source_hash", DbType.Int64).Value = sourceHash;
				sQLiteCommand.Parameters.Add("@last_change_date", DbType.DateTime).Value = lastChangeDate;
				if (targetHash != 0L)
				{
					sQLiteCommand.CommandText += " AND target_hash = @target_hash";
					sQLiteCommand.Parameters.Add("@target_hash", DbType.Int64).Value = targetHash;
				}
				string text2 = " order by change_date " + (descendingOrder ? "desc" : "asc") + ", id " + (descendingOrder ? "desc" : "asc");
				if (!string.IsNullOrEmpty(tuContextData.IdContext))
				{
					text2 = " order by idcontext desc, change_date " + (descendingOrder ? "desc" : "asc") + ", id " + (descendingOrder ? "desc" : "asc");
				}
				sQLiteCommand.CommandText += text2;
				sQLiteCommand.CommandText += " LIMIT @limit OFFSET @offset";
				sQLiteCommand.Parameters.Add("@limit", DbType.Int32).Value = maxHits;
				sQLiteCommand.Parameters.Add("@offset", DbType.Int32).Value = skipRows;
				if (!string.IsNullOrEmpty(tuContextData.IdContext))
				{
					sQLiteCommand.Parameters.Add("@idcontext", DbType.String).Value = tuContextData.IdContext;
				}
				return GetTuSet(sQLiteCommand, maxHits, tuContextData);
			}
		}

		private bool DuplicateSearchReverse(SQLiteCommand cmd, long lastHash, int lastTuId, string hashCol, string alignmentDataColspec)
		{
			if (lastTuId > 0)
			{
				cmd.CommandText = "SELECT id, guid, translation_memory_id, \r\n\t\t\t\t\t\t\tsource_hash, source_segment, 0, 0, \r\n\t\t\t\t\t\t\ttarget_hash, target_segment, 0, 0, \r\n\t\t\t\t\t\t\tcreation_date, creation_user, change_date, change_user, last_used_date, last_used_user, usage_counter, flags " + (SerializesTokens() ? ", source_token_data, target_token_data " : ", null, null ") + alignmentDataColspec + ", 0, null, null " + $"FROM translation_units WHERE {hashCol} = {lastHash}\r\n\t\t\t\t\t\t\tAND id <= @last_tu_id ORDER BY id DESC\r\n\t\t\t\t\t\t\tLIMIT @limit";
			}
			else
			{
				cmd.CommandText = $"SELECT {hashCol} FROM translation_units \r\n\t\t\t\t\t\t\tWHERE {hashCol} <= {lastHash}\r\n\t\t\t\t\t\t\tGROUP BY {hashCol} HAVING COUNT(*) > 1 \r\n\t\t\t\t\t\t\tORDER BY {hashCol} DESC LIMIT 1";
				using (SQLiteDataReader sQLiteDataReader = cmd.ExecuteReader())
				{
					if (!sQLiteDataReader.Read())
					{
						sQLiteDataReader.Close();
						return false;
					}
					lastHash = sQLiteDataReader.GetInt64(0);
					sQLiteDataReader.Close();
				}
				cmd.CommandText = "SELECT id, guid, translation_memory_id, \r\n\t\t\t\t\t\tsource_hash, source_segment, 0, 0, \r\n\t\t\t\t\t\ttarget_hash, target_segment, 0, 0, \r\n\t\t\t\t\t\tcreation_date, creation_user, change_date, change_user, last_used_date, last_used_user, usage_counter, flags " + (SerializesTokens() ? ", source_token_data, target_token_data " : ", null, null ") + alignmentDataColspec + ", 0, null, null " + $"FROM translation_units WHERE {hashCol} = {lastHash}\r\n\t\t\t\t\t\tORDER BY id DESC\r\n\t\t\t\t\t\tLIMIT @limit";
			}
			return true;
		}

		private bool DuplicateSearchForward(SQLiteCommand cmd, long lastHash, int lastTuId, string hashCol, string alignmentDataColspec)
		{
			if (lastTuId > 0)
			{
				cmd.CommandText = "SELECT id, guid, translation_memory_id, \r\n\t\t\t\t\t\t\tsource_hash, source_segment, 0, 0, \r\n\t\t\t\t\t\t\ttarget_hash, target_segment, 0, 0, \r\n\t\t\t\t\t\t\tcreation_date, creation_user, change_date, change_user, last_used_date, last_used_user, usage_counter, flags " + (SerializesTokens() ? ", source_token_data, target_token_data " : ", null, null ") + alignmentDataColspec + ", 0, null, null " + $"FROM translation_units WHERE {hashCol} = {lastHash}\r\n\t\t\t\t\t\t\tAND id > @last_tu_id \r\n\t\t\t\t\t\t\tORDER BY id ASC\r\n\t\t\t\t\t\t\tLIMIT @limit";
			}
			else
			{
				cmd.CommandText = $"SELECT {hashCol} FROM translation_units \r\n\t\t\t\t\t\t\tWHERE {hashCol} > {lastHash}\r\n\t\t\t\t\t\t\tGROUP BY {hashCol} HAVING COUNT(*) > 1 \r\n\t\t\t\t\t\t\tORDER BY {hashCol} ASC LIMIT 1";
				using (SQLiteDataReader sQLiteDataReader = cmd.ExecuteReader())
				{
					if (!sQLiteDataReader.Read())
					{
						sQLiteDataReader.Close();
						return false;
					}
					lastHash = sQLiteDataReader.GetInt64(0);
					sQLiteDataReader.Close();
				}
				cmd.CommandText = "SELECT id, guid, translation_memory_id, \r\n\t\t\t\t\t\tsource_hash, source_segment, 0, 0, \r\n\t\t\t\t\t\ttarget_hash, target_segment, 0, 0, \r\n\t\t\t\t\t\tcreation_date, creation_user, change_date, change_user, last_used_date, last_used_user, usage_counter, flags " + (SerializesTokens() ? ", source_token_data, target_token_data " : ", null, null  ") + alignmentDataColspec + ", 0, null, null " + $"FROM translation_units WHERE {hashCol} = {lastHash}\r\n\t\t\t\t\t\tORDER BY id ASC\r\n\t\t\t\t\t\tLIMIT @limit";
			}
			return true;
		}

		public List<TranslationUnit> DuplicateSearch(int tmId, long lastHash, int lastTuId, int count, bool forward, bool targetSegments)
		{
			string alignmentDataColspec = GetAlignmentDataColspec();
			string hashCol = targetSegments ? "target_hash" : "source_hash";
			using (SQLiteCommand sQLiteCommand = CreateCommand(null, beginTransaction: false))
			{
				if (!((!forward) ? DuplicateSearchReverse(sQLiteCommand, lastHash, lastTuId, hashCol, alignmentDataColspec) : DuplicateSearchForward(sQLiteCommand, lastHash, lastTuId, hashCol, alignmentDataColspec)))
				{
					return new List<TranslationUnit>();
				}
				sQLiteCommand.Parameters.Add("@tm_id", DbType.Int32).Value = tmId;
				sQLiteCommand.Parameters.Add("@limit", DbType.Int32).Value = count;
				if (lastTuId > 0)
				{
					sQLiteCommand.Parameters.Add("@last_tu_id", DbType.Int32).Value = lastTuId;
				}
				return GetTuSet(sQLiteCommand, count, new TuContextData());
			}
		}

		public void RecomputeFrequencies(int tmId)
		{
			Optimize(alwaysAnalyze: true);
			UpdateTuCounts();
			UpdateLastRecomputeInformation(tmId);
		}

		public bool SupportsAlignmentData(bool ignoreFbfgaSetting = false)
		{
			CheckVersion();
			if (!UseFileBasedFga() && !ignoreFbfgaSetting)
			{
				return false;
			}
			string parameter = GetParameter("AlignmentDataVersion");
			if (string.IsNullOrEmpty(parameter))
			{
				return false;
			}
			if (!int.TryParse(parameter, out int result))
			{
				throw new Exception("Invalid parameter value '" + parameter + "' for parameter AlignmentDataVersion");
			}
			if (result == 0 || result > 1)
			{
				throw new Exception("AlignmentDataVersion value is unsupported: " + result.ToString());
			}
			if (!_serializesTokens)
			{
				throw new Exception("TM with alignment data support must also serialize tokens");
			}
			return true;
		}

		private bool HasFgaSupport(int tmId)
		{
			TranslationMemory tm = GetTm(tmId);
			if (tm.FGASupport != FGASupport.Automatic)
			{
				return tm.FGASupport == FGASupport.NonAutomatic;
			}
			return true;
		}

		public void ClearAlignmentData(int tmId)
		{
			if (HasFgaSupport(tmId))
			{
				using (SQLiteCommand sQLiteCommand = CreateCommand("UPDATE\ttranslation_units SET alignment_data = null , align_model_date = NULL", beginTransaction: false))
				{
					sQLiteCommand.ExecuteNonQuery();
					sQLiteCommand.CommandText = "DELETE FROM translation_unit_fragments";
					sQLiteCommand.ExecuteNonQuery();
				}
				InvalidateAlignedPredatedTuCountCache();
			}
		}

		public int GetPostdatedTranslationUnitCount(int tmid, DateTime modelDate)
		{
			if (!HasFgaSupport(tmid))
			{
				return 0;
			}
			using (SQLiteCommand sQLiteCommand = CreateCommand("select count (*) from translation_units where @dateVal < insert_date or insert_date is null", beginTransaction: false))
			{
				sQLiteCommand.Parameters.Add("@dateval", DbType.String).Value = Utils.NormalizeToString(modelDate);
				return Convert.ToInt32(sQLiteCommand.ExecuteScalar());
			}
		}

		private int? TryGetAlignedPredatedTuCount(DateTime modelDate)
		{
			SQLiteConnection sQLiteConnection = _conn as SQLiteConnection;
			if (_conn == null)
			{
				return null;
			}
			lock (AlignedPredatedTuCountCache)
			{
				if (sQLiteConnection != null && sQLiteConnection.FileName != null && AlignedPredatedTuCountCache.TryGetValue(sQLiteConnection.FileName, out Dictionary<DateTime, int> value) && value != null && value.TryGetValue(modelDate, out int value2))
				{
					return value2;
				}
				return null;
			}
		}

		private void SetAlignedPredatedTuCount(DateTime modelDate, int count)
		{
			SQLiteConnection sQLiteConnection = _conn as SQLiteConnection;
			if (_conn != null)
			{
				lock (AlignedPredatedTuCountCache)
				{
					if (!AlignedPredatedTuCountCache.TryGetValue(sQLiteConnection.FileName, out Dictionary<DateTime, int> value))
					{
						value = new Dictionary<DateTime, int>();
						AlignedPredatedTuCountCache.Add(sQLiteConnection.FileName, value);
					}
					value.Remove(modelDate);
					value.Add(modelDate, count);
				}
			}
		}

		private void InvalidateAlignedPredatedTuCountCache()
		{
			SQLiteConnection sQLiteConnection = _conn as SQLiteConnection;
			if (_conn != null)
			{
				lock (AlignedPredatedTuCountCache)
				{
					if (sQLiteConnection != null && sQLiteConnection.FileName != null)
					{
						AlignedPredatedTuCountCache.Remove(sQLiteConnection.FileName);
					}
				}
			}
		}

		public int GetAlignedPredatedTranslationUnitCount(int tmid, DateTime modelDate)
		{
			if (!HasFgaSupport(tmid))
			{
				return 0;
			}
			int? num = TryGetAlignedPredatedTuCount(modelDate);
			if (num.HasValue)
			{
				return num.Value;
			}
			using (SQLiteCommand sQLiteCommand = CreateCommand("select count (*) from translation_units where insert_date < @model_date AND align_model_date < @model_date AND align_model_date < insert_date", beginTransaction: false))
			{
				sQLiteCommand.Parameters.AddWithValue("model_date", Utils.NormalizeToString(modelDate));
				object value = sQLiteCommand.ExecuteScalar();
				num = Convert.ToInt32(value);
				SetAlignedPredatedTuCount(modelDate, num.Value);
				return num.Value;
			}
		}

		public int GetUnalignedCount(int tmid, DateTime? modelDate)
		{
			if (!HasFgaSupport(tmid))
			{
				return 0;
			}
			using (SQLiteCommand sQLiteCommand = CreateCommand("select count (*) from translation_units where align_model_date is NULL", beginTransaction: false))
			{
				return Convert.ToInt32(sQLiteCommand.ExecuteScalar());
			}
		}

		public List<int> UnalignedTusUpdateSchedule(int tmId, int startAfter, int count, int scheduleDelta, DateTime modelDate)
		{
			throw new NotImplementedException();
		}

		public List<(int, DateTime)> GetAlignmentTimestamps(int tmId, List<int> tuIds)
		{
			throw new NotImplementedException();
		}

		public List<(int, DateTime)> GetAlignmentTimestamps(int tmId, int startAfter, int count, DateTime modelDate)
		{
			throw new NotImplementedException();
		}

		public List<TranslationUnit> GetAlignableTus(int tmId, int startAfter, int count, bool unalignedOnly, bool unalignedOrPostdated)
		{
			if (!HasFgaSupport(tmId))
			{
				return null;
			}
			if (unalignedOnly && unalignedOrPostdated)
			{
				throw new Exception("GetAlignableTus - unalignedOnly && unalignedOrPostdated");
			}
			string text = string.Empty;
			if (unalignedOnly)
			{
				text = "alignment_data is NULL";
			}
			else if (unalignedOrPostdated)
			{
				text = "((alignment_data is NULL) OR (align_model_date < insert_date))";
			}
			string cmdText = "SELECT id, guid, source_hash, source_segment, target_hash, target_segment, " + (SerializesTokens() ? "source_token_data, target_token_data" : "null, null") + ", " + (SupportsAlignmentData() ? "alignment_data, align_model_date, insert_date" : "null, null, null") + " \r\n                                                , 0, null, null\r\n                                              FROM translation_units \r\n                                              WHERE id > @start_after " + ((text.Length > 0) ? (" and " + text) : string.Empty) + " \r\n                                              ORDER BY id ASC LIMIT @limit";
			using (SQLiteCommand sQLiteCommand = CreateCommand(cmdText, beginTransaction: false))
			{
				sQLiteCommand.Parameters.Add("@tm_id", DbType.Int32).Value = tmId;
				sQLiteCommand.Parameters.Add("@start_after", DbType.Int32).Value = startAfter;
				sQLiteCommand.Parameters.Add("@limit", DbType.Int32).Value = count;
				return GetTuRange(sQLiteCommand);
			}
		}

		public List<TranslationUnit> GetAlignableTus(int tmId, List<int> tuIds)
		{
			string str = TuIdsToCommaSeparatedList(tuIds);
			string cmdText = string.Format("SELECT id, guid, source_hash, source_segment, target_hash, target_segment, {0}, {1} \r\n                                                , 0, null, null\r\n                                              FROM translation_units \r\n                                              WHERE id in (" + str + ")", SerializesTokens() ? "source_token_data, target_token_data" : "null, null", SupportsAlignmentData() ? "alignment_data, align_model_date, insert_date" : "null, null, null");
			using (SQLiteCommand sQLiteCommand = CreateCommand(cmdText, beginTransaction: false))
			{
				sQLiteCommand.Parameters.Add("@tm_id", DbType.Int32).Value = tmId;
				return GetTuRange(sQLiteCommand);
			}
		}

		public int GetPairCount(int tmid)
		{
			if (HasFgaSupport(tmid))
			{
				return GetTuCount(tmid);
			}
			return 0;
		}

		public AlignerDefinition GetAlignerDefinition(int tmId)
		{
			if (!HasFgaSupport(tmId))
			{
				return null;
			}
			string parameter = GetParameter(tmId, "AlignerDefinition");
			if (string.IsNullOrEmpty(parameter))
			{
				return null;
			}
			try
			{
				return AlignerDefinition.FromSerialization((byte[])DbStorage.BinarySerializeStringToObject(parameter));
			}
			catch (Exception)
			{
				SetParameter(tmId, "AlignerDefinition", null);
				SetParameter("TranslationModelDate", null);
				SetParameter("TranslationModelName", null);
				SetParameter("TranslationModelSampleCount", null);
				SetParameter("TranslationModelVersion", null);
				SetIsAlignmentEnabled(tmId, enabled: false);
				CommitTransaction();
				return null;
			}
		}

		public void SetAlignerDefinition(int tmId, AlignerDefinition definition)
		{
			if (!HasFgaSupport(tmId))
			{
				throw new Exception("The TM does not support FGA");
			}
			if (definition == null)
			{
				SetParameter(tmId, "AlignerDefinition", null);
				return;
			}
			ModelBasedAlignerDefinition modelBasedAlignerDefinition = definition as ModelBasedAlignerDefinition;
			if (modelBasedAlignerDefinition == null)
			{
				throw new Exception("Unsupported AlignerDefinition type in SqliteStorage: " + definition.GetType().Name);
			}
			ChiSquaredTranslationModelId chiSquaredTranslationModelId = modelBasedAlignerDefinition.ModelId as ChiSquaredTranslationModelId;
			if (chiSquaredTranslationModelId == null)
			{
				throw new Exception("Invalid TranslationModelID type: " + modelBasedAlignerDefinition.ModelId.GetType().Name);
			}
			if (chiSquaredTranslationModelId.InternalId != 1)
			{
				throw new Exception("Invalid ChiSquaredTranslationModelId: " + chiSquaredTranslationModelId.InternalId.ToString());
			}
			byte[] o = definition.ToSerialization();
			SetParameter(tmId, "AlignerDefinition", DbStorage.BinarySerializeObjectToString(o));
		}

		public void SetIsAlignmentEnabled(int tmId, bool enabled)
		{
			if (enabled && !HasFgaSupport(tmId))
			{
				throw new Exception("The TM does not support FGA");
			}
			using (SQLiteCommand sQLiteCommand = CreateCommand(null, beginTransaction: false))
			{
				FGASupport fGASupport = enabled ? FGASupport.Automatic : FGASupport.NonAutomatic;
				int num = (int)fGASupport;
				sQLiteCommand.CommandText = "update translation_memories set fga_support = " + num.ToString() + " where id = " + tmId.ToString();
				sQLiteCommand.ExecuteNonQuery();
			}
		}

		public void PrepareForModelBuild(int tmId)
		{
			if (HasFgaSupport(tmId))
			{
				DateTime val = DateTimeUtilities.Normalize(DateTime.Now);
				using (SQLiteCommand sQLiteCommand = CreateCommand("update translation_units set insert_date = @insert_date where insert_date is null", beginTransaction: false))
				{
					sQLiteCommand.Parameters.Add("@insert_date", DbType.String).Value = Utils.NormalizeToString(val);
					sQLiteCommand.ExecuteNonQuery();
				}
			}
		}

		public List<TranslationUnit> ExactSearch(int tmId, List<long> sourceHashes, FilterExpression hardFilter)
		{
			throw new NotImplementedException();
		}

		public List<TranslationUnit> GetTusFiltered(int tmId, FilterExpression filter, int startAfter, int count, bool forward, bool idContextMatch, bool includeContextContent, TextContextMatchType textContextMatchType, CultureInfo sourceCulture, CultureInfo targetCulture)
		{
			throw new NotImplementedException();
		}

		public bool CleanupSchema()
		{
			return true;
		}

		public int GetUnalignedUnscheduledTUCount(int tmId, int scheduleDelta, DateTime modelDate)
		{
			throw new NotSupportedException();
		}

		public void ScheduleTusForAlignment(int id, List<int> ids)
		{
			throw new NotSupportedException();
		}

		public Dictionary<int, List<TranslationUnit>> FuzzySearch(int tmId, Dictionary<int, List<int>> features, int minScore, int maxHits, FilterExpression hardFilter)
		{
			throw new NotImplementedException();
		}

		public List<TranslationUnit> DuplicateSearch(int tmId, List<long> sourceHashes, List<long> targetHashes)
		{
			throw new NotImplementedException();
		}

		public List<TranslationUnit> GetFullTusByIds(int tmId, List<int> tuIds)
		{
			throw new NotImplementedException();
		}
	}
}
