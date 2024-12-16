using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public class GenericCredentials
	{
		private const string _userString = "user";

		private const string _passwordString = "password";

		private string _userName;

		private string _password;

		private Dictionary<string, string> _properties = new Dictionary<string, string>();

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

		public string[] PropertyKeys => _properties.Keys.ToArray();

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

		public GenericCredentials(string userName, string password)
		{
			_userName = userName;
			_password = password;
		}

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

		public override bool Equals(object obj)
		{
			GenericCredentials genericCredentials = obj as GenericCredentials;
			if (genericCredentials == null)
			{
				return false;
			}
			return object.Equals(ToCredentialString(), genericCredentials.ToCredentialString());
		}

		public override int GetHashCode()
		{
			return ToCredentialString().GetHashCode();
		}
	}
}
