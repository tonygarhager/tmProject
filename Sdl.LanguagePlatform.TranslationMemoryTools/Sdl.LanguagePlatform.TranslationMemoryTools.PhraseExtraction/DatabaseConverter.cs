using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Stat;
using System;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Text;

namespace Sdl.LanguagePlatform.TranslationMemoryTools.PhraseExtraction
{
	public class DatabaseConverter
	{
		private static bool IsCjCulture(CultureInfo ci)
		{
			switch (ci.Name.Substring(0, 2).ToLower())
			{
			case "ja":
			case "zh":
				return true;
			default:
				return false;
			}
		}

		public static void Convert(string flatFilePath, string dbFilePath)
		{
			if (!File.Exists(flatFilePath))
			{
				throw new FileNotFoundException("File not found", flatFilePath);
			}
			DataFileInfo dataFileInfo = new DataFileInfo(flatFilePath);
			BilingualDictionaryFile bilingualDictionaryFile = new BilingualDictionaryFile(dataFileInfo.Location, dataFileInfo.SourceCulture, dataFileInfo.TargetCulture);
			bilingualDictionaryFile.Load();
			bool applyCjSeparatorCheck = IsCjCulture(dataFileInfo.SourceCulture);
			bool applyCjSeparatorCheck2 = IsCjCulture(dataFileInfo.TargetCulture);
			VocabularyFile vocabulary = dataFileInfo.Location.GetVocabulary(dataFileInfo.SourceCulture);
			vocabulary.Load();
			VocabularyFile vocabulary2 = dataFileInfo.Location.GetVocabulary(dataFileInfo.TargetCulture);
			vocabulary2.Load();
			using (SQLiteConnection sQLiteConnection = CreateBiligualPhraseFile(dbFilePath))
			{
				SQLiteCommand sQLiteCommand = new SQLiteCommand("insert into C (k, v) values (@k, @v)", sQLiteConnection);
				sQLiteCommand.Parameters.Add(new SQLiteParameter("@k", DbType.String));
				sQLiteCommand.Parameters.Add(new SQLiteParameter("@v", DbType.String));
				sQLiteCommand.Parameters["@k"].Value = "s";
				sQLiteCommand.Parameters["@v"].Value = dataFileInfo.SourceCulture.Name;
				sQLiteCommand.ExecuteNonQuery();
				sQLiteCommand.Parameters["@k"].Value = "t";
				sQLiteCommand.Parameters["@v"].Value = dataFileInfo.TargetCulture.Name;
				sQLiteCommand.ExecuteNonQuery();
				SQLiteTransaction sQLiteTransaction = sQLiteConnection.BeginTransaction();
				for (int i = 0; i < bilingualDictionaryFile.Count; i++)
				{
					InsertPhrasePair(bilingualDictionaryFile[i], sQLiteConnection);
				}
				sQLiteTransaction.Commit();
				sQLiteTransaction = sQLiteConnection.BeginTransaction();
				for (int j = 0; j < bilingualDictionaryFile.TargetPhrases.Count; j++)
				{
					InsertTargetPhrase(bilingualDictionaryFile.TargetPhrases[j], vocabulary2, sQLiteConnection, applyCjSeparatorCheck2);
				}
				sQLiteTransaction.Commit();
				SQLiteCommand sQLiteCommand2 = new SQLiteCommand("insert into S (k, s, f, d) values (@k, @s, @f, @d)", sQLiteConnection);
				SQLiteParameter sQLiteParameter = new SQLiteParameter("@k", DbType.Int64);
				SQLiteParameter sQLiteParameter2 = new SQLiteParameter("@s", DbType.Int64);
				SQLiteParameter sQLiteParameter3 = new SQLiteParameter("@f", DbType.Int32);
				SQLiteParameter sQLiteParameter4 = new SQLiteParameter("@d", DbType.String);
				sQLiteCommand2.Parameters.Add(sQLiteParameter);
				sQLiteCommand2.Parameters.Add(sQLiteParameter2);
				sQLiteCommand2.Parameters.Add(sQLiteParameter3);
				sQLiteCommand2.Parameters.Add(sQLiteParameter4);
				SQLiteCommand sQLiteCommand3 = new SQLiteCommand("insert into I (k, s, o) values (@k, @s, @o)", sQLiteConnection);
				SQLiteParameter sQLiteParameter5 = new SQLiteParameter("@k", DbType.Int64);
				SQLiteParameter sQLiteParameter6 = new SQLiteParameter("@s", DbType.Int64);
				SQLiteParameter sQLiteParameter7 = new SQLiteParameter("@o", DbType.Int32);
				sQLiteCommand3.Parameters.Add(sQLiteParameter5);
				sQLiteCommand3.Parameters.Add(sQLiteParameter6);
				sQLiteCommand3.Parameters.Add(sQLiteParameter7);
				for (int k = 0; k < bilingualDictionaryFile.SourcePhrases.Count; k += 10000)
				{
					sQLiteTransaction = sQLiteConnection.BeginTransaction();
					for (int l = k; l < Math.Min(bilingualDictionaryFile.SourcePhrases.Count, k + 10000); l++)
					{
						Phrase phrase = bilingualDictionaryFile.SourcePhrases[l];
						string s;
						bool start = GetStart(phrase, vocabulary, out s);
						int num = phrase.Keys[0];
						if (start)
						{
							num = vocabulary.Add(s);
						}
						sQLiteParameter.Value = phrase.ID;
						sQLiteParameter2.Value = num;
						sQLiteParameter3.Value = phrase.Count;
						sQLiteParameter4.Value = GetString(phrase, vocabulary, applyCjSeparatorCheck);
						sQLiteCommand2.ExecuteNonQuery();
						sQLiteParameter6.Value = phrase.ID;
						int num2 = (!start) ? 1 : 2;
						for (int m = num2; m < phrase.Keys.Length; m++)
						{
							sQLiteParameter5.Value = phrase.Keys[m];
							sQLiteParameter7.Value = m - num2 + 1;
							sQLiteCommand3.ExecuteNonQuery();
						}
					}
					sQLiteTransaction.Commit();
				}
				sQLiteTransaction = sQLiteConnection.BeginTransaction();
				SQLiteCommand sQLiteCommand4 = new SQLiteCommand("insert into W (k, w) values (@k, @w)", sQLiteConnection);
				sQLiteParameter = new SQLiteParameter("@k", DbType.Int64);
				SQLiteParameter sQLiteParameter8 = new SQLiteParameter("@w", DbType.String);
				sQLiteCommand4.Parameters.Add(sQLiteParameter);
				sQLiteCommand4.Parameters.Add(sQLiteParameter8);
				for (int n = 0; n < vocabulary.Count; n++)
				{
					string text = vocabulary.Lookup(n);
					if (text != null)
					{
						sQLiteParameter.Value = n;
						sQLiteParameter8.Value = text;
						sQLiteCommand4.ExecuteNonQuery();
					}
				}
				sQLiteTransaction.Commit();
			}
		}

		private static SQLiteConnection CreateBiligualPhraseFile(string path)
		{
			string connectionString = "Data Source=" + path.Replace('\\', '/') + ";New=True;UTF8Encoding=True;Version=3";
			if (File.Exists(path))
			{
				File.Delete(path);
			}
			SQLiteConnection sQLiteConnection = new SQLiteConnection(connectionString);
			sQLiteConnection.Open();
			new SQLiteCommand("PRAGMA page_size=4096;create table C (k nvarchar(50), v nvarchar(255));create table P (s integer, t integer, f integer, constraint pkp primary key (s, t));create table S (k integer primary key autoincrement, s integer, f integer, d ntext(1024));create table T (k integer primary key autoincrement, f integer, d ntext(1024));create table W (k integer primary key autoincrement, w ntext(255));create table I (k integer, s integer, o integer, constraint pki primary key (k, s, o));create index is1 on S (s, f);create index iks on I (s, k);", sQLiteConnection).ExecuteNonQuery();
			return sQLiteConnection;
		}

		private static void InsertPhrasePair(PhrasePair pair, SQLiteConnection connection)
		{
			SQLiteCommand sQLiteCommand = new SQLiteCommand("insert into P (s, t, f) values (@s, @t, @f)", connection);
			SQLiteParameter parameter = new SQLiteParameter("@s", DbType.Int64)
			{
				Value = pair.SourcePhraseKey
			};
			sQLiteCommand.Parameters.Add(parameter);
			parameter = new SQLiteParameter("@t", DbType.Int64)
			{
				Value = pair.TargetPhraseKey
			};
			sQLiteCommand.Parameters.Add(parameter);
			parameter = new SQLiteParameter("@f", DbType.Int32)
			{
				Value = pair.Count
			};
			sQLiteCommand.Parameters.Add(parameter);
			sQLiteCommand.ExecuteNonQuery();
		}

		private static string GetString(Phrase phrase, IKeyToStringMapper vocabularyFile, bool applyCjSeparatorCheck)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string text = null;
			int[] keys = phrase.Keys;
			foreach (int key in keys)
			{
				bool flag = text != null;
				string text2 = vocabularyFile.Lookup(key);
				if (applyCjSeparatorCheck && text != null && text2 != null && text.Length > 0 && text2.Length > 0)
				{
					char c = text[text.Length - 1];
					bool flag2 = CharacterProperties.IsCJKChar(c);
					if (!flag2 && TMXDataEncoder.IsJaLongVowelMarker(c))
					{
						flag2 = true;
					}
					if (CharacterProperties.IsCJKChar(text2[0]) && flag2)
					{
						flag = false;
					}
				}
				if (flag)
				{
					stringBuilder.Append(" ");
				}
				stringBuilder.Append(text2);
				text = text2;
			}
			return stringBuilder.ToString();
		}

		private static bool GetStart(Phrase phrase, IKeyToStringMapper vocabularyFile, out string s)
		{
			s = vocabularyFile.Lookup(phrase.Keys[0]);
			if (s.Length >= 4 || phrase.Keys.Length <= 1)
			{
				return false;
			}
			s += " ";
			s += vocabularyFile.Lookup(phrase.Keys[1]);
			return true;
		}

		private static void InsertTargetPhrase(Phrase phrase, IKeyToStringMapper vocabularyFile, SQLiteConnection connection, bool applyCjSeparatorCheck)
		{
			string @string = GetString(phrase, vocabularyFile, applyCjSeparatorCheck);
			SQLiteCommand sQLiteCommand = new SQLiteCommand("insert into T (k, f, d) values (@k, @f, @d)", connection);
			SQLiteParameter parameter = new SQLiteParameter("@k", DbType.Int32)
			{
				Value = phrase.ID
			};
			sQLiteCommand.Parameters.Add(parameter);
			parameter = new SQLiteParameter("@f", DbType.Int32)
			{
				Value = phrase.Count
			};
			sQLiteCommand.Parameters.Add(parameter);
			parameter = new SQLiteParameter("@d", DbType.String)
			{
				Value = @string
			};
			sQLiteCommand.Parameters.Add(parameter);
			sQLiteCommand.ExecuteNonQuery();
		}
	}
}
