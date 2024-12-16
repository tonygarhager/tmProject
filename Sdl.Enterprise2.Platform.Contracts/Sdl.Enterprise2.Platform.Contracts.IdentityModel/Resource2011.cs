using System;
using System.Runtime.Serialization;

namespace Sdl.Enterprise2.Platform.Contracts.IdentityModel
{
	[DataContract(Namespace = "http://sdl.com/identity/2010", Name = "Resource", IsReference = true)]
	public class Resource2011 : IExtensibleDataObject
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
		public Guid UniqueId
		{
			get;
			set;
		}

		[DataMember(Order = 1)]
		public string Name
		{
			get;
			set;
		}

		[DataMember(Order = 2, EmitDefaultValue = false)]
		public string Description
		{
			get;
			set;
		}

		[DataMember(Order = 3)]
		public string ResourceType
		{
			get;
			set;
		}

		[DataMember(Order = 4)]
		public PermissionCollection Permissions
		{
			get;
			set;
		}

		[DataMember(Order = 5)]
		public ResourceGroup2011 ParentResourceGroup
		{
			get;
			set;
		}

		[DataMember(Order = 6)]
		public ResourceGroup2011[] LinkedResourceGroups
		{
			get;
			set;
		}
	}
}
