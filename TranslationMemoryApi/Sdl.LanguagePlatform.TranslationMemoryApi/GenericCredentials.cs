using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// A generic class for building and parsing a credentials string.  This will usually contain a UserName and Password but may also contain
	/// other parameters using an indexer on this class.
	/// </summary>
	public class GenericCredentials
	{
		private const string _userString = "user";

		private const string _passwordString = "password";

		private string _userName;

		private string _password;

		private Dictionary<string, string> _properties = new Dictionary<string, string>();

		/// <summary>
		/// The UserName to be used in a credentials store - if null, then no username is specified.
		/// </summary>
		public string UserName
		{
			get
			{
				return _userName;
			}
			set
			{
				_userName = value;
			}
		}

		/// <summary>
		/// The Password to be used in a credentials store - if null, then no password is specified.
		/// </summary>
		public string Password
		{
			get
			{
				return _password;
			}
			set
			{
				_password = value;
			}
		}

		/// <summary>
		/// A string indexer that allows any number of key/value pairs to used in a credentials store.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public string this[string propertyName]
		{
			get
			{
				string value = null;
				if (propertyName == "user")
				{
					return _userName;
				}
				if (propertyName == "password")
				{
					return _password;
				}
				if (_properties.TryGetValue(propertyName, out value))
				{
					return value;
				}
				return null;
			}
			set
			{
				if (propertyName == "user")
				{
					_userName = value;
				}
				else if (propertyName == "password")
				{
					_password = value;
				}
				else
				{
					_properties[propertyName] = value;
				}
			}
		}

		/// <summary>
		/// Returns an array of property key names that have values defined for them.
		/// </summary>
		/// <remarks>
		/// This array does not contain entries for "userName" or "password" values.
		/// </remarks>
		public string[] PropertyKeys => _properties.Keys.ToArray();

		/// <summary>
		/// Constructs a <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.GenericCredentials" /> class from a credentials string previously created by another instance of this class.
		/// </summary>
		/// <param name="credentialString"></param>
		public GenericCredentials(string credentialString)
		{
			string[] array = credentialString.Split(';');
			string[] array2 = array;
			foreach (string text in array2)
			{
				string[] array3 = text.Split('=');
				array3[0] = HttpUtility.UrlDecode(array3[0]);
				array3[1] = HttpUtility.UrlDecode(array3[1]);
				if (array3.Length == 2 || string.IsNullOrEmpty(array3[0]) || string.IsNullOrEmpty(array3[1]))
				{
					if (array3[0] == "user")
					{
						_userName = array3[1];
					}
					else if (array3[0] == "password")
					{
						_password = array3[1];
					}
					else
					{
						_properties[array3[0]] = array3[1];
					}
				}
			}
		}

		/// <summary>
		/// Constructs a <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.GenericCredentials" /> class from a UserName and Password string.
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		public GenericCredentials(string userName, string password)
		{
			_userName = userName;
			_password = password;
		}

		/// <summary>
		/// Returns a string containing a UserName and Password and any other defined parameters that can be used in a credentials store.
		/// </summary>
		/// <returns>The credential string representation.</returns>
		public string ToCredentialString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (!string.IsNullOrEmpty(_userName))
			{
				AddParameterString(stringBuilder, "user", _userName);
			}
			if (!string.IsNullOrEmpty(_password))
			{
				AddParameterString(stringBuilder, "password", _password);
			}
			foreach (KeyValuePair<string, string> property in _properties)
			{
				AddParameterString(stringBuilder, property.Key, property.Value);
			}
			return stringBuilder.ToString();
		}

		/// <summary>
		/// Returns the string representation of the credentials. See <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.GenericCredentials.ToCredentialString" />.
		/// </summary>
		/// <returns>The credential string representation.</returns>
		public override string ToString()
		{
			return ToCredentialString();
		}

		private void AddParameterString(StringBuilder sb, string key, string value)
		{
			if (sb.Length > 0)
			{
				sb.Append(";");
			}
			sb.Append($"{HttpUtility.UrlEncode(key)}={HttpUtility.UrlEncode(value)}");
		}

		/// <summary>
		/// Determines whether the specified <see cref="T:System.Object" /> is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="T:System.Object" /> to compare with this instance.</param>
		/// <returns>
		/// 	<c>true</c> if the specified <see cref="T:System.Object" /> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		/// <exception cref="T:System.NullReferenceException">
		/// The <paramref name="obj" /> parameter is null.
		/// </exception>
		public override bool Equals(object obj)
		{
			GenericCredentials genericCredentials = obj as GenericCredentials;
			if (genericCredentials == null)
			{
				return false;
			}
			return object.Equals(ToCredentialString(), genericCredentials.ToCredentialString());
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode()
		{
			return ToCredentialString().GetHashCode();
		}
	}
}
