using System;
using System.Runtime.Serialization;

namespace Sdl.Enterprise2.Platform.Contracts.IdentityModel
{
	[DataContract(Name = "User", Namespace = "http://sdl.com/identity/2010", IsReference = true)]
	public class User2011 : IExtensibleDataObject
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

		private bool _isWindows;

		private bool _isProtected;

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

		[DataMember(Name = "IsWindowsUser")]
		public bool IsWindowsUser
		{
			get
			{
				return _isWindows;
			}
			set
			{
				_isWindows = value;
			}
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
	}
}
