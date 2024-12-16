using System;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Text;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	public abstract class DbStorageBase
	{
		protected DbConnection _conn;

		protected object _connectionLock = new object();

		protected bool _KeepConnection;

		private DbTransaction _Transaction;

		public static DateTime _MinDate = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public DbStorageBase()
		{
		}

		public DbStorageBase(DbStorageBase other)
		{
			_conn = other._conn;
			_KeepConnection = other._KeepConnection;
			_Transaction = other._Transaction;
			_connectionLock = other._connectionLock;
		}

		private void EnsureOpenConnection(bool beginTransaction)
		{
			lock (_connectionLock)
			{
				if (_conn.State != ConnectionState.Open)
				{
					_conn.Open();
				}
			}
			if (_Transaction == null && beginTransaction)
			{
				BeginTransaction();
			}
		}

		public void BeginTransaction()
		{
			_Transaction = _conn.BeginTransaction();
		}

		public void CommitTransaction()
		{
			if (_Transaction != null)
			{
				_Transaction.Commit();
				_Transaction.Dispose();
				_Transaction = null;
			}
		}

		protected void InitializeCommand(DbCommand cmd)
		{
			InitializeCommand(cmd, beginTransaction: true);
		}

		protected void InitializeCommand(DbCommand cmd, bool beginTransaction)
		{
			EnsureOpenConnection(beginTransaction);
			if (cmd.Connection == null)
			{
				cmd.Connection = _conn;
			}
			if (cmd.Transaction == null)
			{
				cmd.Transaction = _Transaction;
			}
			cmd.CommandTimeout = 180;
		}

		public void AbortTransaction()
		{
			if (_Transaction != null)
			{
				_Transaction.Rollback();
				_Transaction.Dispose();
				_Transaction = null;
			}
		}

		public void Dispose()
		{
			if (_conn != null)
			{
				if (_Transaction != null)
				{
					AbortTransaction();
				}
				if (_conn.State == ConnectionState.Open)
				{
					_conn.Close();
				}
				if (!_KeepConnection)
				{
					_conn.Dispose();
				}
			}
		}

		protected static byte[] StringToByteArray(string str)
		{
			byte[] array = new byte[str.Length * 2];
			Buffer.BlockCopy(str.ToCharArray(), 0, array, 0, array.Length);
			return array;
		}

		protected static string ByteArrayToString(byte[] bytes)
		{
			char[] array = new char[bytes.Length / 2];
			Buffer.BlockCopy(bytes, 0, array, 0, bytes.Length);
			return new string(array);
		}

		protected string[] ReadSchemaCommands(string schemaName, string separator, object objInContainingAssembly)
		{
			StreamReader streamReader = new StreamReader(objInContainingAssembly.GetType().Assembly.GetManifestResourceStream(schemaName) ?? throw new Exception("EmbeddedResourceNotFound " + schemaName), Encoding.GetEncoding(1252));
			string text = streamReader.ReadToEnd();
			streamReader.Close();
			return text.Split(new string[1]
			{
				separator
			}, StringSplitOptions.RemoveEmptyEntries);
		}

		public static string CreateDateParam(DateTime dt)
		{
			return dt.ToString("yyyy-MM-dd HH:mm:ss");
		}

		public static DateTime? ParseDateParam(string paramVal, string paramName)
		{
			if (string.IsNullOrEmpty(paramVal))
			{
				return null;
			}
			if (paramVal.Length < "yyyy-MM-dd HH-mm-ss".Length)
			{
				throw new Exception("Invalid date string: " + paramVal);
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(paramVal.Substring(0, 4));
			stringBuilder.Append("-");
			stringBuilder.Append(paramVal.Substring(5, 2));
			stringBuilder.Append("-");
			stringBuilder.Append(paramVal.Substring(8, 2));
			stringBuilder.Append(" ");
			stringBuilder.Append(paramVal.Substring(11, 2));
			stringBuilder.Append("-");
			stringBuilder.Append(paramVal.Substring(14, 2));
			stringBuilder.Append("-");
			stringBuilder.Append(paramVal.Substring(17, 2));
			string s = stringBuilder.ToString();
			if (!DateTime.TryParseExact(s, "yyyy-MM-dd HH-mm-ss", null, DateTimeStyles.None, out DateTime result))
			{
				throw new Exception("Invalid parameter value '" + paramVal + "' for parameter " + paramName);
			}
			return DateTime.SpecifyKind(result, DateTimeKind.Utc);
		}

		public static DateTime Normalize(DateTime val)
		{
			if (val < _MinDate)
			{
				val = _MinDate;
			}
			return DateTime.SpecifyKind(new DateTime(val.Year, val.Month, val.Day, val.Hour, val.Minute, val.Second), DateTimeKind.Utc);
		}
	}
}
