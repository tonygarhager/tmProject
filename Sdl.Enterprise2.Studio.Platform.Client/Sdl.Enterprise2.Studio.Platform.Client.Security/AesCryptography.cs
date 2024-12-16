using System;
using System.Data.Common;
using System.IO;
using System.Security.Cryptography;

namespace Sdl.Enterprise2.Studio.Platform.Client.Security
{
	public class AesCryptography
	{
		private readonly byte[] key;

		private readonly byte[] iv;

		public AesCryptography(byte[] key, byte[] iv)
		{
			this.key = key;
			this.iv = iv;
		}

		public string Encrypt(string plainText)
		{
			if (string.IsNullOrEmpty(plainText))
			{
				throw new ArgumentNullException("plainText");
			}
			string text = null;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (AesManaged aesManaged = new AesManaged
				{
					Key = key,
					IV = iv
				})
				{
					using (CryptoStream stream = new CryptoStream(memoryStream, aesManaged.CreateEncryptor(aesManaged.Key, aesManaged.IV), CryptoStreamMode.Write))
					{
						using (StreamWriter streamWriter = new StreamWriter(stream))
						{
							streamWriter.Write(plainText);
							aesManaged.Clear();
						}
					}
				}
				return Convert.ToBase64String(memoryStream.ToArray());
			}
		}

		public string Decrypt(string encryptedText)
		{
			if (string.IsNullOrEmpty(encryptedText))
			{
				throw new ArgumentNullException("encryptedText");
			}
			byte[] buffer = Convert.FromBase64String(encryptedText);
			string text = null;
			using (MemoryStream stream = new MemoryStream(buffer))
			{
				using (AesManaged aesManaged = new AesManaged
				{
					Key = key,
					IV = iv
				})
				{
					using (CryptoStream stream2 = new CryptoStream(stream, aesManaged.CreateDecryptor(aesManaged.Key, aesManaged.IV), CryptoStreamMode.Read))
					{
						using (StreamReader streamReader = new StreamReader(stream2))
						{
							text = streamReader.ReadToEnd();
							aesManaged.Clear();
							return text;
						}
					}
				}
			}
		}

		public string EncryptConnectionString(string connectionString)
		{
			if (string.IsNullOrEmpty(connectionString))
			{
				throw new ArgumentNullException("connectionString");
			}
			DbConnectionStringBuilder dbConnectionStringBuilder = new DbConnectionStringBuilder
			{
				ConnectionString = connectionString
			};
			if (dbConnectionStringBuilder.ContainsKey("Password"))
			{
				dbConnectionStringBuilder["Password"] = Encrypt((string)dbConnectionStringBuilder["Password"]);
			}
			return dbConnectionStringBuilder.ConnectionString;
		}

		public string DecryptConnectionString(string connectionString)
		{
			if (string.IsNullOrEmpty(connectionString))
			{
				throw new ArgumentNullException("connectionString");
			}
			DbConnectionStringBuilder dbConnectionStringBuilder = new DbConnectionStringBuilder
			{
				ConnectionString = connectionString
			};
			if (dbConnectionStringBuilder.ContainsKey("Password"))
			{
				dbConnectionStringBuilder["Password"] = Decrypt((string)dbConnectionStringBuilder["Password"]);
			}
			return dbConnectionStringBuilder.ConnectionString;
		}
	}
}
