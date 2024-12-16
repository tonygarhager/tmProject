using System;
using System.Collections;
using System.Globalization;

namespace Sdl.Enterprise2.Studio.Platform.Client.IdentityModel
{
	public sealed class UserCredentials : IEquatable<UserCredentials>
	{
		public class SsoData
		{
			public string SamlToken
			{
				get;
				set;
			}

			public string AuthToken
			{
				get;
				set;
			}

			public DateTime ExpirationDate
			{
				get;
				set;
			}

			public bool HasExpired
			{
				get
				{
					if (!string.IsNullOrEmpty(AuthToken))
					{
						return ExpirationDate < DateTime.UtcNow;
					}
					return true;
				}
			}
		}

		private string userName;

		private string password;

		private UserManagerTokenType userType;

		private SsoData ssoCredentials;

		public string UserName
		{
			get
			{
				return userName;
			}
			set
			{
				userName = FormatUserName(value);
			}
		}

		public string Password
		{
			get
			{
				return password;
			}
			set
			{
				password = value;
			}
		}

		public UserManagerTokenType UserType
		{
			get
			{
				return userType;
			}
			set
			{
				userType = value;
			}
		}

		public SsoData SsoCredentials
		{
			get
			{
				return ssoCredentials;
			}
			set
			{
				ssoCredentials = value;
			}
		}

		public UserCredentials()
		{
		}

		public UserCredentials(string userName, string password, UserManagerTokenType userType)
		{
			if (userType == UserManagerTokenType.CustomUser || userType == UserManagerTokenType.CustomWindowsUser)
			{
				if (string.IsNullOrEmpty(userName))
				{
					throw new ArgumentNullException("userName");
				}
				if (string.IsNullOrEmpty(password))
				{
					throw new ArgumentNullException("password");
				}
				UserName = userName;
				Password = password;
			}
			if (userType == UserManagerTokenType.Saml2User)
			{
				if (string.IsNullOrEmpty(userName))
				{
					throw new ArgumentNullException("userName");
				}
				UserName = userName;
			}
			this.userType = userType;
		}

		public UserCredentials(string userName, SsoData ssoCredentials, UserManagerTokenType userType)
		{
			if (string.IsNullOrEmpty(userName))
			{
				throw new ArgumentNullException("userName");
			}
			UserName = userName;
			SsoCredentials = ssoCredentials;
			UserType = userType;
		}

		public static UserCredentials Create(string credentialString)
		{
			if (string.IsNullOrEmpty(credentialString))
			{
				throw new ArgumentNullException("credentialString");
			}
			string value = null;
			string value2 = null;
			UserManagerTokenType userManagerTokenType = UserManagerTokenType.CustomUser;
			string[] array = credentialString.Split(';');
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (text.StartsWith("user", StringComparison.OrdinalIgnoreCase))
				{
					value = text.Split('=')[1];
				}
				else if (text.StartsWith("pass", StringComparison.OrdinalIgnoreCase))
				{
					value2 = text.Split('=')[1];
				}
				else if (text.StartsWith("type", StringComparison.OrdinalIgnoreCase))
				{
					userManagerTokenType = (UserManagerTokenType)Enum.Parse(typeof(UserManagerTokenType), text.Split('=')[1], ignoreCase: true);
				}
			}
			if (userManagerTokenType == UserManagerTokenType.CustomUser && (string.IsNullOrEmpty(value) || !string.IsNullOrEmpty(value2)))
			{
				throw new InvalidOperationException("A username and password must be supplied when specifying a custom user.");
			}
			if (userManagerTokenType == UserManagerTokenType.CustomWindowsUser && (string.IsNullOrEmpty(value) || !string.IsNullOrEmpty(value2)))
			{
				throw new InvalidOperationException("A username and password must be supplied when specifying a custom windows user.");
			}
			UserCredentials userCredentials = new UserCredentials
			{
				UserType = userManagerTokenType
			};
			if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(value2))
			{
				userCredentials.UserName = value;
				userCredentials.Password = value2;
			}
			return userCredentials;
		}

		public static bool Equals(UserCredentials left, UserCredentials right)
		{
			if (left == right)
			{
				return true;
			}
			return left?.Equals(right) ?? right.Equals(left);
		}

		public override bool Equals(object obj)
		{
			return (obj as UserCredentials)?.Equals(this) ?? false;
		}

		public override int GetHashCode()
		{
			return GetHashCode(UserType, UserName, Password);
		}

		private static int GetHashCode(params object[] items)
		{
			return GetHashCode((IEnumerable)items);
		}

		private static int GetHashCode(IEnumerable items)
		{
			int num = 0;
			int num2 = 0;
			foreach (object item in items)
			{
				num ^= item.GetHashCode() * (int)Math.Pow(29.0, num2);
				num2++;
			}
			return num;
		}

		public bool Equals(UserCredentials other)
		{
			if (other != null && string.Compare(other.UserName, UserName, StringComparison.OrdinalIgnoreCase) == 0 && string.Compare(other.Password, Password, StringComparison.OrdinalIgnoreCase) == 0 && other.UserType == UserType && EqualsSso(other.ssoCredentials))
			{
				return true;
			}
			return false;
		}

		private bool EqualsSso(SsoData other)
		{
			if ((SsoCredentials == null && other != null) || (SsoCredentials != null && other == null))
			{
				return false;
			}
			if (SsoCredentials == null || other == null)
			{
				return true;
			}
			if (SsoCredentials.SamlToken.Equals(other.SamlToken) && SsoCredentials.AuthToken.Equals(other.AuthToken))
			{
				return SsoCredentials.ExpirationDate == other.ExpirationDate;
			}
			return false;
		}

		private static string FormatUserName(string userName)
		{
			if (string.IsNullOrEmpty(userName))
			{
				throw new ArgumentNullException("userName");
			}
			string[] array = userName.Split('\\');
			if (array.Length == 1)
			{
				return userName.ToLowerInvariant();
			}
			if (array.Length == 2)
			{
				string text = array[0].Split('.')[0];
				return $"{text.ToUpperInvariant()}\\{array[1].ToLowerInvariant()}";
			}
			throw new InvalidOperationException(string.Format(CultureInfo.CurrentUICulture, "Unsupported username format {0}.", userName));
		}
	}
}
