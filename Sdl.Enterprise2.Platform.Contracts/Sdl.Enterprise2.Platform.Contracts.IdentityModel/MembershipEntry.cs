using System;
using System.Runtime.Serialization;

namespace Sdl.Enterprise2.Platform.Contracts.IdentityModel
{
	[CLSCompliant(true)]
	[DataContract(Namespace = "http://sdl.com/identity/2010", Name = "MembershipEntry", IsReference = true)]
	public class MembershipEntry : IExtensibleDataObject
	{
		private ExtensionDataObject _extensionDataObject;

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

		[DataMember(Order = 0)]
		public Guid ResourceGroupId
		{
			get;
			set;
		}

		[DataMember(Order = 1)]
		public Guid RoleId
		{
			get;
			set;
		}

		[DataMember(Order = 2)]
		public Guid UserId
		{
			get;
			set;
		}
	}
}
