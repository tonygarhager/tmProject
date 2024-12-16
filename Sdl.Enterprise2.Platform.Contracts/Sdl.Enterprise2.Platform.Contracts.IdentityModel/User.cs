using System;
using System.Runtime.Serialization;

namespace Sdl.Enterprise2.Platform.Contracts.IdentityModel
{
	[DataContract(Name = "User", Namespace = "http://sdl.com/identity/2012", IsReference = true)]
	public class User : IExtensibleDataObject
	{
		private ExtensionDataObject _extensionDataObject;

		private Guid _id;

		private string _name;

		private string _displayName;

		private string _desc;

		private string _email;

		private string _phone;

		private string _locale;

		private Guid _organizationId;

		private MembershipEntry[] _membershipEntries;

		private bool _isProtected;

		private UserLanguageDirection[] _languageDirections;

		public ExtensionDataObject ExtensionData
		{
			get
			{
				return _extensionDataObject;
			}
			set
			{
				_extensionDataObject = value;
			}
		}

		[DataMember(Name = "UniqueId")]
		public Guid UniqueId
		{
			get
			{
				return _id;
			}
			set
			{
				_id = value;
			}
		}

		[DataMember(Name = "Name")]
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		[DataMember(Name = "DisplayName")]
		public string DisplayName
		{
			get
			{
				return _displayName;
			}
			set
			{
				_displayName = value;
			}
		}

		[DataMember(Name = "Description", EmitDefaultValue = false)]
		public string Description
		{
			get
			{
				return _desc;
			}
			set
			{
				_desc = value;
			}
		}

		[DataMember(Name = "EmailAddress")]
		public string EmailAddress
		{
			get
			{
				return _email;
			}
			set
			{
				_email = value;
			}
		}

		[DataMember(Name = "PhoneNumber")]
		public string PhoneNumber
		{
			get
			{
				return _phone;
			}
			set
			{
				_phone = value;
			}
		}

		[DataMember(Name = "Locale")]
		public string Locale
		{
			get
			{
				return _locale;
			}
			set
			{
				_locale = value;
			}
		}

		[DataMember(Name = "OrganizationId")]
		public Guid OrganizationId
		{
			get
			{
				return _organizationId;
			}
			set
			{
				_organizationId = value;
			}
		}

		[DataMember(Name = "MembershipEntries")]
		public MembershipEntry[] MembershipEntries
		{
			get
			{
				return _membershipEntries;
			}
			set
			{
				_membershipEntries = value;
			}
		}

		[DataMember(Name = "UserType")]
		public UserType UserType
		{
			get;
			set;
		}

		[DataMember(Name = "Protected")]
		public bool Protected
		{
			get
			{
				return _isProtected;
			}
			set
			{
				_isProtected = value;
			}
		}

		[DataMember(Name = "LanguageDirections")]
		public UserLanguageDirection[] LanguageDirections
		{
			get
			{
				return _languageDirections;
			}
			set
			{
				_languageDirections = value;
			}
		}

		public static implicit operator User2011(User user)
		{
			if (user == null)
			{
				return null;
			}
			return new User2011
			{
				Description = user.Description,
				DisplayName = user.DisplayName,
				EmailAddress = user.EmailAddress,
				ExtensionData = user.ExtensionData,
				IsWindowsUser = (user.UserType == UserType.WindowsUser),
				Locale = user.Locale,
				MembershipEntries = user.MembershipEntries,
				Name = user.Name,
				OrganizationId = user.OrganizationId,
				PhoneNumber = user.PhoneNumber,
				Protected = user.Protected,
				UniqueId = user.UniqueId
			};
		}

		public static implicit operator User(User2011 user2011)
		{
			if (user2011 == null)
			{
				return null;
			}
			return new User
			{
				Description = user2011.Description,
				DisplayName = user2011.DisplayName,
				EmailAddress = user2011.EmailAddress,
				ExtensionData = user2011.ExtensionData,
				UserType = (user2011.IsWindowsUser ? UserType.WindowsUser : UserType.SDLUser),
				Locale = user2011.Locale,
				MembershipEntries = user2011.MembershipEntries,
				Name = user2011.Name,
				OrganizationId = user2011.OrganizationId,
				PhoneNumber = user2011.PhoneNumber,
				Protected = user2011.Protected,
				UniqueId = user2011.UniqueId,
				LanguageDirections = new UserLanguageDirection[0]
			};
		}
	}
}
