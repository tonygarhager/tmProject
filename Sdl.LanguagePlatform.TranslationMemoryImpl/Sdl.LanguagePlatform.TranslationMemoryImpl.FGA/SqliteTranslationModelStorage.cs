using Sdl.Core.FineGrainedAlignment;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.Text;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl.FGA
{
	internal class SqliteTranslationModelStorage : DbStorageBase, ITranslationModelStorage, IDisposable
	{
		public const string _TranslationModelDateKey = "TranslationModelDate";

		public const string _TranslationModelNameKey = "TranslationModelName";

		public const string _TranslationModelSampleCountKey = "TranslationModelSampleCount";

		public const string _TranslationModelVersionKey = "TranslationModelVersion";

		private static bool _registered;

		private static readonly object Locker = new object();

		public static void Register()
		{
			lock (Locker)
			{
				if (!_registered)
				{
					TranslationModelStorageFactory.RegisterDatabaseContainerCreator("system.data.sqlite", (DatabaseContainer container) => new SqliteTranslationModelStorage(container));
					TranslationModelStorageFactory.RegisterFileContainerCreator("system.data.sqlite", (FileContainer container) => new SqliteTranslationModelStorage(container));
					_registered = true;
				}
			}
		}

		public bool IsFileBased()
		{
			return true;
		}

		public SqliteTranslationModelStorage(FileContainer container)
		{
			string connectionString = SqliteStorageUtils.BuildConnectionString(container);
			CommonConstruct(connectionString);
		}

		public SqliteTranslationModelStorage(DatabaseContainer container)
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

		public SqliteTranslationModelStorage(string connectionString)
		{
			CommonConstruct(connectionString);
		}

		private void CommonConstruct(string connectionString)
		{
			_KeepConnection = SqliteStorageUtils.UsePooling;
			SQLiteConnection sQLiteConnection = (SQLiteConnection)(_conn = new SQLiteConnection(connectionString));
		}

		public void WriteTranslationModelData(int modelId, IEnumerable<TranslationModelMatrixEntry> entries, bool isReversedMatrix)
		{
			CheckModelExists(modelId);
			using (SQLiteCommand sQLiteCommand = CreateCommand(null, beginTransaction: true))
			{
				sQLiteCommand.CommandText = "INSERT INTO " + TranslationModelMatrixTableName(isReversedMatrix) + " (sourcekey, targetkey, floatval) VALUES (@sourcekey, @targetkey, @floatval)";
				SQLiteParameter sQLiteParameter = sQLiteCommand.Parameters.Add("floatval", DbType.Double);
				SQLiteParameter sQLiteParameter2 = sQLiteCommand.Parameters.Add("sourcekey", DbType.Int32);
				SQLiteParameter sQLiteParameter3 = sQLiteCommand.Parameters.Add("targetkey", DbType.Int32);
				foreach (TranslationModelMatrixEntry entry in entries)
				{
					sQLiteParameter.Value = entry.Value;
					sQLiteParameter2.Value = entry.SourceKey;
					sQLiteParameter3.Value = entry.TargetKey;
					sQLiteCommand.ExecuteNonQuery();
				}
			}
		}

		public List<TranslationModelMatrixEntry> ReadTranslationModelData(int modelId, ref int startAfter, int count, bool isReversedMatrix)
		{
			CheckModelExists(modelId);
			if (count < 0)
			{
				throw new ArgumentException("count");
			}
			int num = -1;
			int num2 = 0;
			List<TranslationModelMatrixEntry> list = new List<TranslationModelMatrixEntry>();
			if (count == 0)
			{
				return list;
			}
			using (SQLiteCommand sQLiteCommand = CreateCommand(null, beginTransaction: false))
			{
				sQLiteCommand.CommandText = "SELECT id, sourcekey, targetkey, floatval FROM " + TranslationModelMatrixTableName(isReversedMatrix) + " WHERE id > " + startAfter.ToString();
				using (SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader())
				{
					while (sQLiteDataReader.Read())
					{
						num = sQLiteDataReader.GetInt32(0);
						TranslationModelMatrixEntry translationModelMatrixEntry = new TranslationModelMatrixEntry();
						translationModelMatrixEntry.SourceKey = sQLiteDataReader.GetInt32(1);
						translationModelMatrixEntry.TargetKey = sQLiteDataReader.GetInt32(2);
						translationModelMatrixEntry.Value = sQLiteDataReader.GetDouble(3);
						list.Add(translationModelMatrixEntry);
						num2++;
						if (num2 == count)
						{
							break;
						}
					}
					if (num2 < count)
					{
						num = -1;
					}
				}
			}
			startAfter = num;
			return list;
		}

		public List<TranslationModelMatrixEntry> ReadTranslationModelData(int modelId, HashSet<int> sourceKeys, HashSet<int> targetKeys, bool isReversedMatrix)
		{
			CheckModelExists(modelId);
			List<TranslationModelMatrixEntry> list = new List<TranslationModelMatrixEntry>();
			if (sourceKeys.Count == 0 || targetKeys.Count == 0)
			{
				return list;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (int sourceKey in sourceKeys)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(",");
				}
				stringBuilder.Append(sourceKey.ToString());
			}
			StringBuilder stringBuilder2 = new StringBuilder();
			foreach (int targetKey in targetKeys)
			{
				if (stringBuilder2.Length > 0)
				{
					stringBuilder2.Append(",");
				}
				stringBuilder2.Append(targetKey.ToString());
			}
			using (SQLiteCommand sQLiteCommand = CreateCommand(null, beginTransaction: false))
			{
				sQLiteCommand.CommandText = "SELECT id, sourcekey, targetkey, floatval FROM " + TranslationModelMatrixTableName(isReversedMatrix) + " WHERE sourcekey in (" + stringBuilder?.ToString() + ") and targetkey in (" + stringBuilder2?.ToString() + ")";
				using (SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader())
				{
					while (sQLiteDataReader.Read())
					{
						TranslationModelMatrixEntry translationModelMatrixEntry = new TranslationModelMatrixEntry();
						translationModelMatrixEntry.SourceKey = sQLiteDataReader.GetInt32(1);
						translationModelMatrixEntry.TargetKey = sQLiteDataReader.GetInt32(2);
						translationModelMatrixEntry.Value = sQLiteDataReader.GetDouble(3);
						list.Add(translationModelMatrixEntry);
					}
					return list;
				}
			}
		}

		public void DeleteTranslationModel(int modelId)
		{
			ClearTranslationModel(modelId);
			SetParameter("TranslationModelName", null);
			SetParameter("TranslationModelDate", null);
		}

		public int TotalVocabSize(int modelId, bool target)
		{
			CheckModelExists(modelId);
			string str = VocabTableName(target);
			using (SQLiteCommand sQLiteCommand = CreateCommand("select count(*) from " + str, beginTransaction: false))
			{
				return Convert.ToInt32(sQLiteCommand.ExecuteScalar());
			}
		}

		public int AddTranslationModel(string name, List<AlignableCorpusId> corpusIds, CultureInfo sourceCulture, CultureInfo targetCulture)
		{
			if (GetParameter("TranslationModelName") != null)
			{
				throw new Exception("File-based TM only supports 1 translation model");
			}
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException("name");
			}
			SetParameter("TranslationModelName", name);
			SetParameter("TranslationModelVersion", 2.ToString());
			return 1;
		}

		public void UpdateTranslationModel(int modelId, string name, List<AlignableCorpusId> corpusIds)
		{
			CheckModelExists(modelId);
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException("name");
			}
			if (modelId != 1)
			{
				throw new ArgumentException("modelId");
			}
			SetParameter("TranslationModelName", name);
		}

		public void GetTranslationModelDetails(int modelId, out string name, List<AlignableCorpusId> corpusIds, out CultureInfo sourceCulture, out CultureInfo targetCulture, out DateTime? modelDate, out int sampleCount, out int version)
		{
			CheckModelExists(modelId);
			corpusIds.Clear();
			int @int;
			Guid guid;
			using (SQLiteCommand sQLiteCommand = CreateCommand(null, beginTransaction: false))
			{
				sQLiteCommand.CommandText = "select id, guid, source_language, target_language from translation_memories";
				using (SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader())
				{
					if (!sQLiteDataReader.Read())
					{
						throw new Exception("The translation memory is missing");
					}
					@int = sQLiteDataReader.GetInt32(0);
					guid = sQLiteDataReader.GetGuid(1);
					sourceCulture = new CultureInfo(sQLiteDataReader.GetString(2));
					targetCulture = new CultureInfo(sQLiteDataReader.GetString(3));
				}
			}
			PersistentObjectToken id = new PersistentObjectToken(@int, guid);
			name = GetParameter("TranslationModelName");
			string parameter = GetParameter("TranslationModelDate");
			modelDate = DbStorageBase.ParseDateParam(parameter, "TranslationModelDate");
			sampleCount = 0;
			parameter = GetParameter("TranslationModelSampleCount");
			if (!string.IsNullOrEmpty(parameter))
			{
				sampleCount = int.Parse(parameter);
			}
			version = 1;
			parameter = GetParameter("TranslationModelVersion");
			if (!string.IsNullOrEmpty(parameter))
			{
				version = int.Parse(parameter);
			}
			corpusIds.Add(new StorageBasedAlignableCorpusId(id));
		}

		public void GetAllTranslationModelDetails(List<string> names, List<int> modelIds, List<List<AlignableCorpusId>> corpusIdLists, List<CultureInfo> sourceCultures, List<CultureInfo> targetCultures, List<DateTime?> modelDates, List<int> sampleCounts, List<int> versions)
		{
			names.Clear();
			modelIds.Clear();
			sourceCultures.Clear();
			targetCultures.Clear();
			modelDates.Clear();
			List<AlignableCorpusId> list = new List<AlignableCorpusId>();
			GetTranslationModelDetails(1, out string name, list, out CultureInfo sourceCulture, out CultureInfo targetCulture, out DateTime? modelDate, out int sampleCount, out int version);
			names.Add(name);
			sourceCultures.Add(sourceCulture);
			targetCultures.Add(targetCulture);
			modelDates.Add(modelDate);
			sampleCounts.Add(sampleCount);
			modelIds.Add(1);
			versions.Add(version);
			corpusIdLists.Clear();
			corpusIdLists.Add(list);
		}

		public void ClearTranslationModel(int modelId)
		{
			if (modelId != 1)
			{
				throw new Exception("Model not found with id: " + modelId.ToString());
			}
			using (SQLiteCommand sQLiteCommand = CreateCommand(null, beginTransaction: false))
			{
				sQLiteCommand.CommandText = "delete from " + VocabTableName(target: false);
				sQLiteCommand.ExecuteNonQuery();
				sQLiteCommand.CommandText = "delete from " + VocabTableName(target: true);
				sQLiteCommand.ExecuteNonQuery();
				sQLiteCommand.CommandText = "delete from trans_model";
				sQLiteCommand.ExecuteNonQuery();
				sQLiteCommand.CommandText = "delete from trans_model_rev";
				sQLiteCommand.ExecuteNonQuery();
			}
			SetParameter("TranslationModelDate", null);
		}

		public List<TranslationModelVocabEntry> LoadTranslationModelVocab(int modelId, bool target, IEnumerable<string> tokensToLoad)
		{
			CheckModelExists(modelId);
			string str = VocabTableName(target);
			List<TranslationModelVocabEntry> list = new List<TranslationModelVocabEntry>();
			using (SQLiteCommand sQLiteCommand = CreateCommand(null, beginTransaction: false))
			{
				sQLiteCommand.CommandText = "select vocab, id,freq from " + str;
				if (tokensToLoad != null)
				{
					IEnumerator<string> enumerator = tokensToLoad.GetEnumerator();
					using (SQLiteCommand sQLiteCommand2 = CreateCommand(null, beginTransaction: true))
					{
						sQLiteCommand2.CommandText = "delete from vocabfilter";
						sQLiteCommand2.ExecuteNonQuery();
						sQLiteCommand2.CommandText = "insert into vocabfilter (token) values (@tokenval)";
						SQLiteParameter sQLiteParameter = sQLiteCommand2.Parameters.Add("tokenval", DbType.String);
						while (enumerator.MoveNext())
						{
							sQLiteParameter.Value = enumerator.Current;
							sQLiteCommand2.ExecuteNonQuery();
						}
						enumerator.Dispose();
						CommitTransaction();
					}
					sQLiteCommand.CommandText += " inner join vocabfilter on vocab = vocabfilter.token";
				}
				using (SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader())
				{
					while (sQLiteDataReader.Read())
					{
						TranslationModelVocabEntry translationModelVocabEntry = new TranslationModelVocabEntry();
						translationModelVocabEntry.Key = sQLiteDataReader.GetInt32(1);
						translationModelVocabEntry.Token = sQLiteDataReader.GetString(0);
						translationModelVocabEntry.Occurrences = sQLiteDataReader.GetInt32(2);
						list.Add(translationModelVocabEntry);
					}
					sQLiteDataReader.Close();
					return list;
				}
			}
		}

		public void SaveTranslationModelVocab(int modelId, bool target, IEnumerable<TranslationModelVocabEntry> vocab)
		{
			CheckModelExists(modelId);
			string str = VocabTableName(target);
			using (SQLiteCommand sQLiteCommand = CreateCommand(null, beginTransaction: true))
			{
				sQLiteCommand.CommandText = "insert into " + str + " (vocab, id, freq) values (@vocabval, @codeval, @freq)";
				SQLiteParameter sQLiteParameter = sQLiteCommand.Parameters.Add("vocabval", DbType.String);
				SQLiteParameter sQLiteParameter2 = sQLiteCommand.Parameters.Add("codeval", DbType.Int32);
				SQLiteParameter sQLiteParameter3 = sQLiteCommand.Parameters.Add("freq", DbType.Int32);
				IEnumerator<TranslationModelVocabEntry> enumerator = vocab.GetEnumerator();
				while (enumerator.MoveNext())
				{
					sQLiteParameter.Value = enumerator.Current.Token;
					sQLiteParameter2.Value = enumerator.Current.Key;
					sQLiteParameter3.Value = enumerator.Current.Occurrences;
					sQLiteCommand.ExecuteNonQuery();
				}
				enumerator.Dispose();
			}
		}

		public void Flush()
		{
		}

		public DateTime? GetTranslationModelDate(int modelId)
		{
			CheckModelExists(modelId);
			return DbStorageBase.ParseDateParam(GetParameter("TranslationModelDate"), "TranslationModelDate");
		}

		public void SetTranslationModelDate(int modelId, DateTime? dateTime)
		{
			CheckModelExists(modelId);
			if (dateTime.HasValue)
			{
				SetParameter("TranslationModelDate", DbStorageBase.CreateDateParam(DbStorageBase.Normalize(dateTime.Value)));
				using (SQLiteCommand sQLiteCommand = CreateCommand("ANALYZE", beginTransaction: true))
				{
					sQLiteCommand.ExecuteNonQuery();
				}
			}
			else
			{
				SetParameter("TranslationModelDate", null);
			}
		}

		public int GetSampleCount(int modelId)
		{
			CheckModelExists(modelId);
			string parameter = GetParameter("TranslationModelSampleCount");
			if (!string.IsNullOrEmpty(parameter))
			{
				return int.Parse(parameter);
			}
			return 0;
		}

		public void SetSampleCount(int modelId, int sampleCount)
		{
			CheckModelExists(modelId);
			SetParameter("TranslationModelSampleCount", sampleCount.ToString());
			SetParameter("TranslationModelVersion", 2.ToString());
		}

		private void CheckModelExists(int modelId)
		{
			if (GetParameter("TranslationModelName") != null && modelId == 1)
			{
				return;
			}
			throw new Exception("Model not found with id " + modelId.ToString());
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

		private SQLiteCommand CreateCommand(string cmdText, bool beginTransaction)
		{
			SQLiteCommand sQLiteCommand = string.IsNullOrEmpty(cmdText) ? new SQLiteCommand() : new SQLiteCommand(cmdText);
			InitializeCommand(sQLiteCommand, beginTransaction);
			return sQLiteCommand;
		}

		private static string TranslationModelMatrixTableName(bool isReversedMatrix)
		{
			if (!isReversedMatrix)
			{
				return "trans_model";
			}
			return "trans_model_rev";
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

		private static string VocabTableName(bool target)
		{
			if (!target)
			{
				return "vocab_src";
			}
			return "vocab_trg";
		}

		public void CreateTranslationModelContainerSchema()
		{
		}

		public void DropTranslationModelContainerSchema()
		{
		}
	}
}
