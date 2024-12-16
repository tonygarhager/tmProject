using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.Text;

namespace Sdl.Core.LanguageProcessing.AutoSuggest
{
	internal class FileBasedAutoSuggestDictionaryAccessor : AbstractAutoSuggestDictionaryAccessor
	{
		private SQLiteConnection _connection;

		private readonly string _filePath;

		public FileBasedAutoSuggestDictionaryAccessor(string fileName, CultureInfo sourceCulture, CultureInfo targetCulture)
			: base(sourceCulture, targetCulture)
		{
			_filePath = fileName;
		}

		private void Open()
		{
			if (_connection == null)
			{
				_connection = new SQLiteConnection(GetConnectionString(_filePath));
				_connection.Open();
			}
		}

		protected override void EnsureOpenConnection()
		{
			Open();
		}

		private void Close()
		{
			if (_connection != null)
			{
				_connection.Close();
				_connection.Dispose();
				_connection = null;
			}
		}

		private static string GetConnectionString(string filePath)
		{
			SQLiteConnectionStringBuilder sQLiteConnectionStringBuilder = new SQLiteConnectionStringBuilder
			{
				DataSource = filePath,
				Enlist = false,
				FailIfMissing = true,
				ReadOnly = true
			};
			return sQLiteConnectionStringBuilder.ConnectionString;
		}

		public override void Dispose()
		{
			if (_connection != null)
			{
				Close();
			}
		}

		protected override void AddMappings(IList<string> words, IList<string> mergedWords, PhraseMappingPairs mappings, bool hasSpecialTokens, IList<Token> sourceTokens)
		{
			AddMappings(_connection, words, mergedWords, mappings, hasSpecialTokens, sourceTokens);
		}

		private void AddMappings(SQLiteConnection connection, IList<string> words, IList<string> mergedWords, PhraseMappingPairs mappings, bool hasSpecialTokens, IList<Token> sourceTokens)
		{
			if (connection != null)
			{
				IList<int> sourceWordsIndex = GetSourceWordsIndex(connection, words);
				if (sourceWordsIndex.Count > 0)
				{
					IList<int> sourceWordsIndex2 = GetSourceWordsIndex(connection, mergedWords);
					string mappingsSql = GetMappingsSql(sourceWordsIndex, sourceWordsIndex2);
					AddMappings(connection, mappingsSql, mappings, hasSpecialTokens, sourceTokens);
				}
			}
		}

		private static string GetMappingsSql(IList<int> index, IEnumerable<int> mergedIndex)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("select P.f,  S.d, T.d  from S join P on P.s = S.k join T on P.t = T.k where S.s in (");
			foreach (int item in index)
			{
				stringBuilder.Append(item);
				stringBuilder.Append(",");
			}
			foreach (int item2 in mergedIndex)
			{
				stringBuilder.Append(item2);
				stringBuilder.Append(",");
			}
			stringBuilder.Length--;
			stringBuilder.Append(")");
			stringBuilder.Append(" and not exists (select 1 from I where I.s = S.k and I.k not in (");
			foreach (int item3 in index)
			{
				stringBuilder.Append(item3);
				stringBuilder.Append(",");
			}
			stringBuilder.Length--;
			stringBuilder.Append(") limit 1)");
			stringBuilder.Append(" order by P.f desc");
			return stringBuilder.ToString();
		}

		private void AddMappings(SQLiteConnection connection, string sql, PhraseMappingPairs mappings, bool hasSpecialTokens, IList<Token> sourceTokens)
		{
			using (SQLiteCommand sQLiteCommand = new SQLiteCommand(sql, connection))
			{
				using (SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader())
				{
					while (sQLiteDataReader.Read())
					{
						int @int = sQLiteDataReader.GetInt32(0);
						string @string = sQLiteDataReader.GetString(1);
						Segment segment = CreateSegment(_sourceCulture, @string, SourceCliticsInformation);
						string string2 = sQLiteDataReader.GetString(2);
						Segment segment2 = CreateSegment(_targetCulture, string2, TargetCliticsInformation);
						if (hasSpecialTokens)
						{
							List<Pair<Segment>> list = SubstituteSpecialTokens(sourceTokens, segment, segment2);
							if (list != null)
							{
								foreach (Pair<Segment> item in list)
								{
									mappings.Add(item.Left, item.Right, @int);
								}
							}
							else
							{
								mappings.Add(segment, segment2, @int);
							}
						}
						else
						{
							mappings.Add(segment, segment2, @int);
						}
					}
				}
			}
		}

		private IList<int> GetSourceWordsIndex(SQLiteConnection connection, IList<string> words)
		{
			IList<int> list = new List<int>();
			if (connection == null || words == null || words.Count <= 0)
			{
				return list;
			}
			int nonNull;
			string sourceWordsIndexSql = GetSourceWordsIndexSql(words, out nonNull);
			if (nonNull <= 0)
			{
				return list;
			}
			using (SQLiteCommand sQLiteCommand = new SQLiteCommand(sourceWordsIndexSql, connection))
			{
				using (SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader())
				{
					while (sQLiteDataReader.Read())
					{
						int @int = sQLiteDataReader.GetInt32(0);
						list.Add(@int);
					}
					return list;
				}
			}
		}

		private string GetSourceWordsIndexSql(IEnumerable<string> words, out int nonNull)
		{
			nonNull = 0;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("select k from W where w in (");
			foreach (string word in words)
			{
				if (word != null)
				{
					string encodedLiteralWord = GetEncodedLiteralWord(word);
					stringBuilder.Append(encodedLiteralWord);
					stringBuilder.Append(',');
					nonNull++;
				}
			}
			stringBuilder.Length--;
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}
	}
}
