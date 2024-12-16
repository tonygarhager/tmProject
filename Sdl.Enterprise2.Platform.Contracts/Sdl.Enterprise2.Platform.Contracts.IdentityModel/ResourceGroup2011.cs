using System;
using System.Runtime.Serialization;

namespace Sdl.Enterprise2.Platform.Contracts.IdentityModel
{
	[CLSCompliant(true)]
	[DataContract(Namespace = "http://sdl.com/identity/2010", Name = "ResourceGroup", IsReference = true)]
	public class ResourceGroup2011 : IExtensibleDataObject
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
		public string ResourceGroupType
		{
			get;
			set;
		}

		[DataMember(Order = 2)]
		public string Name
		{
			get;
			set;
		}

		[DataMember(Order = 3, EmitDefaultValue = false)]
		public string Description
		{
			get;
			set;
		}

		[DataMember(Order = 4)]
		public string Path
		{
			get;
			set;
		}

		[DataMember(Order = 5)]
		public Guid ParentResourceGroupId
		{
			get;
			set;
		}

		[DataMember(Order = 6)]
		public ResourceGroup2011[] ChildResourceGroups
		{
			get;
			set;
		}

		[DataMember(Order = 7)]
		public MembershipEntry[] MembershipEntries
		{
			get;
			set;
		}

		[DataMember(Order = 8)]
		public bool BlockRoleInheritance
		{
			get;
			set;
		}
	}
}
