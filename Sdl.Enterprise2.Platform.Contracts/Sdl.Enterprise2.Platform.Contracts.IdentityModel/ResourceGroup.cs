using System;
using System.Runtime.Serialization;

namespace Sdl.Enterprise2.Platform.Contracts.IdentityModel
{
	[DataContract(Namespace = "http://sdl.com/identity/2012", Name = "ResourceGroup", IsReference = true)]
	public class ResourceGroup : IExtensibleDataObject
	{
		private ExtensionDataObject _extensionDataObject;

		private string resourceType;

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
			get
			{
				if (resourceType == null)
				{
					return null;
				}
				return resourceType.ToUpperInvariant();
			}
			set
			{
				resourceType = value;
			}
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
		public ResourceGroup[] ChildResourceGroups
		{
			get;
			set;
		}

		public static implicit operator ResourceGroup2011(ResourceGroup resourceGroup)
		{
			if (resourceGroup == null)
			{
				return null;
			}
			return new ResourceGroup2011
			{
				BlockRoleInheritance = false,
				ChildResourceGroups = ((resourceGroup.ChildResourceGroups == null) ? null : Array.ConvertAll(resourceGroup.ChildResourceGroups, (Converter<ResourceGroup, ResourceGroup2011>)((ResourceGroup input) => input))),
				Description = resourceGroup.Description,
				ExtensionData = resourceGroup.ExtensionData,
				MembershipEntries = new MembershipEntry[0],
				Name = resourceGroup.Name,
				ParentResourceGroupId = resourceGroup.ParentResourceGroupId,
				Path = resourceGroup.Path,
				ResourceGroupType = resourceGroup.ResourceGroupType,
				UniqueId = resourceGroup.UniqueId
			};
		}

		public static implicit operator ResourceGroup(ResourceGroup2011 resourceGroup)
		{
			if (resourceGroup == null)
			{
				return null;
			}
			return new ResourceGroup
			{
				ChildResourceGroups = ((resourceGroup.ChildResourceGroups == null) ? null : Array.ConvertAll(resourceGroup.ChildResourceGroups, (Converter<ResourceGroup2011, ResourceGroup>)((ResourceGroup2011 input) => input))),
				Description = resourceGroup.Description,
				ExtensionData = resourceGroup.ExtensionData,
				Name = resourceGroup.Name,
				ParentResourceGroupId = resourceGroup.ParentResourceGroupId,
				Path = resourceGroup.Path,
				ResourceGroupType = resourceGroup.ResourceGroupType,
				UniqueId = resourceGroup.UniqueId
			};
		}
	}
}
