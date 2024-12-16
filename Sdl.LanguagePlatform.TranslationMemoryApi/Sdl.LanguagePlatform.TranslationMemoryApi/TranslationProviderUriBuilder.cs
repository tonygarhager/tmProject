using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public class TranslationProviderUriBuilder
	{
		private string _type;

		private string _protocol;

		private string _userName;

		private string _password;

		private string _hostName;

		private int _port;

		private string _resource;

		private Dictionary<string, string> _paramerters;

		private Uri _builtUri;

		public Uri Uri
		{
			get
			{
				if (_builtUri == null)
				{
					_builtUri = BuildUri();
				}
				return _builtUri;
			}
		}

		public string Type
		{
			get
			{
				if (!string.IsNullOrEmpty(_type))
				{
					return _type;
				}
				return null;
			}
			set
			{
				_type = value;
				_builtUri = null;
			}
		}

		public string Protocol
		{
			get
			{
				if (!string.IsNullOrEmpty(_protocol))
				{
					return _protocol;
				}
				return null;
			}
			set
			{
				_protocol = value;
				_builtUri = null;
			}
		}

		public string UserName
		{
			get
			{
				if (!string.IsNullOrEmpty(_userName))
				{
					return _userName;
				}
				return null;
			}
			set
			{
				_userName = value;
				_builtUri = null;
			}
		}

		public string Password
		{
			get
			{
				if (!string.IsNullOrEmpty(_password))
				{
					return _password;
				}
				return null;
			}
			set
			{
				_password = value;
				_builtUri = null;
			}
		}

		public string HostName
		{
			get
			{
				if (!string.IsNullOrEmpty(_hostName))
				{
					return _hostName;
				}
				return null;
			}
			set
			{
				_hostName = value;
				_builtUri = null;
			}
		}

		public int Port
		{
			get
			{
				return _port;
			}
			set
			{
				_port = value;
				_builtUri = null;
			}
		}

		public string Resource
		{
			get
			{
				if (string.IsNullOrEmpty(_resource))
				{
					return null;
				}
				if (!object.Equals(_protocol, "file"))
				{
					return _resource;
				}
				return _resource.Replace("/", "\\");
			}
			set
			{
				_resource = value;
				_builtUri = null;
			}
		}

		public string[] ParameterKeys => _paramerters.Keys.ToArray();

		public string this[string propertyName]
		{
			get
			{
				string value = null;
				if (_paramerters.TryGetValue(propertyName, out value))
				{
					return value;
				}
				return null;
			}
			set
			{
				if (value == null)
				{
					_paramerters.Remove(propertyName);
				}
				else
				{
					_paramerters[propertyName] = value;
				}
				_builtUri = null;
			}
		}

		public TranslationProviderUriBuilder()
			: this(null, null)
		{
		}

		public TranslationProviderUriBuilder(string type)
			: this(type, null)
		{
		}

		public TranslationProviderUriBuilder(string type, string protocol)
		{
			Initialise();
			if (string.IsNullOrEmpty(type))
			{
				throw new ArgumentNullException("type");
			}
			if (type.IndexOf(".") != -1)
			{
				throw new ArgumentException("TranslationMemoryUriBuilder: type cannot contain a '.' character.  Specify a 'protocol' parameter if requried");
			}
			if (type == "file")
			{
				throw new ArgumentException("TranslationMemoryUriBuilder: type cannot be set to \"file\" as this is not compatible with the Translation Provider Uri Schema");
			}
			_type = type;
			_protocol = protocol;
		}

		public TranslationProviderUriBuilder(Uri uri)
		{
			ParseUri(uri);
		}

		private void Initialise()
		{
			_type = null;
			_protocol = null;
			_userName = null;
			_password = null;
			_hostName = null;
			_port = 0;
			_paramerters = new Dictionary<string, string>();
			_builtUri = null;
		}

		public void ParseUri(Uri uri)
		{
			Initialise();
			if (uri == null)
			{
				throw new ArgumentException("Uri");
			}
			int num = uri.Scheme.IndexOf('.');
			if (num > 0 && num + 1 < uri.Scheme.Length)
			{
				_type = uri.Scheme.Substring(0, num);
				_protocol = uri.Scheme.Substring(num + 1);
			}
			else
			{
				_type = uri.Scheme;
			}
			int num2 = uri.UserInfo.IndexOf(':');
			if (num2 != -1)
			{
				_userName = HttpUtility.UrlDecode(uri.UserInfo.Substring(0, num2));
				_password = HttpUtility.UrlDecode(uri.UserInfo.Substring(num2 + 1));
			}
			_hostName = uri.Host;
			_port = ((uri.Port != -1) ? uri.Port : 0);
			string text = MyFileDecode(uri.LocalPath);
			if (!uri.IsFile && text.Length > 0 && text[0] == '/')
			{
				if (text.Length > 1)
				{
					_resource = text.Substring(1);
				}
			}
			else if (!string.IsNullOrEmpty(text))
			{
				_resource = text;
			}
			string[] array = uri.Query.Split(new char[2]
			{
				'?',
				'&'
			}, StringSplitOptions.RemoveEmptyEntries);
			string[] array2 = array;
			foreach (string text2 in array2)
			{
				string[] array3 = text2.Split('=');
				if ((array3.Length == 1 || array3.Length == 2) && !string.IsNullOrEmpty(array3[0]))
				{
					_paramerters.Add(HttpUtility.UrlDecode(array3[0]), HttpUtility.UrlDecode(array3[1]));
				}
			}
		}

		private Uri BuildUri()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(_type);
			if (!string.IsNullOrEmpty(_protocol))
			{
				stringBuilder.Append('.');
				stringBuilder.Append(_protocol);
			}
			stringBuilder.Append("://");
			if (!string.IsNullOrEmpty(_userName))
			{
				stringBuilder.Append(HttpUtility.UrlEncode(_userName) + ":");
				stringBuilder.Append(HttpUtility.UrlEncode(_password) + "@");
			}
			stringBuilder.Append(_hostName);
			if (_port > 0)
			{
				stringBuilder.Append(":" + _port.ToString());
			}
			if (!string.IsNullOrEmpty(_resource))
			{
				stringBuilder.Append("/" + MyFileEncode(_resource));
			}
			int num = 0;
			foreach (KeyValuePair<string, string> paramerter in _paramerters)
			{
				if (paramerter.Value != null)
				{
					stringBuilder.Append(((num++ == 0) ? "?" : "&") + HttpUtility.UrlEncode(paramerter.Key) + "=" + HttpUtility.UrlEncode(paramerter.Value));
				}
			}
			Uri uri = new Uri(stringBuilder.ToString());
			TranslationProviderUriBuilder build = new TranslationProviderUriBuilder(uri);
			if (!EqualProperties(this, build))
			{
				throw new InvalidOperationException("The current parameters are not compatible with the Translation Provider Uri Schema");
			}
			return uri;
		}

		private bool EqualProperties(TranslationProviderUriBuilder build1, TranslationProviderUriBuilder build2)
		{
			if (!object.Equals(build1.Type, build2.Type) || !object.Equals(build1.Protocol, build2.Protocol) || !object.Equals(build1.UserName, build2.UserName) || !object.Equals(build1.Password, build2.Password) || !object.Equals(build1.HostName, build2.HostName) || !object.Equals(build1.Port, build2.Port) || !object.Equals(build1.Resource, build2.Resource))
			{
				return false;
			}
			if (!object.Equals(build1.ParameterKeys.Length, build2.ParameterKeys.Length))
			{
				return false;
			}
			string[] parameterKeys = build1.ParameterKeys;
			foreach (string propertyName in parameterKeys)
			{
				if (!object.Equals(build1[propertyName], build2[propertyName]))
				{
					return false;
				}
			}
			return true;
		}

		private string MyFileDecode(string value)
		{
			return value;
		}

		private string MyFileEncode(string value)
		{
			string text = value.Replace("%", "%25");
			return text.Replace("#", "%23");
		}

		public override string ToString()
		{
			return Uri.AbsoluteUri;
		}

		public override bool Equals(object obj)
		{
			TranslationProviderUriBuilder translationProviderUriBuilder = obj as TranslationProviderUriBuilder;
			if (translationProviderUriBuilder == null)
			{
				return false;
			}
			return object.Equals(Uri, translationProviderUriBuilder.Uri);
		}

		public override int GetHashCode()
		{
			return Uri.GetHashCode();
		}
	}
}
