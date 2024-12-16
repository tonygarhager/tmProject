using System;
using System.Runtime.Serialization;

namespace Sdl.Enterprise2.Platform.Contracts.IdentityModel
{
	[DataContract(Namespace = "http://sdl.com/identity/2012", Name = "Resource", IsReference = true)]
	public class Resource : IExtensibleDataObject
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
		public ResourceGroup ParentResourceGroup
		{
			get;
			set;
		}

		[DataMember(Order = 6)]
		public ResourceGroup[] LinkedResourceGroups
		{
			get;
			set;
		}

		public static implicit operator Resource2011(Resource resource)
		{
			if (resource == null)
			{
				return null;
			}
			return new Resource2011
			{
				Description = resource.Description,
				ExtensionData = resource.ExtensionData,
				LinkedResourceGroups = Array.ConvertAll(resource.LinkedResourceGroups, (Converter<ResourceGroup, ResourceGroup2011>)((ResourceGroup input) => input)),
				Name = resource.Name,
				ParentResourceGroup = resource.ParentResourceGroup,
				Permissions = resource.Permissions,
				ResourceType = resource.ResourceType,
				UniqueId = resource.UniqueId
			};
		}

		public static implicit operator Resource(Resource2011 resource)
		{
			if (resource == null)
			{
				return null;
			}
			return new Resource
			{
				Description = resource.Description,
				ExtensionData = resource.ExtensionData,
				LinkedResourceGroups = Array.ConvertAll(resource.LinkedResourceGroups, (Converter<ResourceGroup2011, ResourceGroup>)((ResourceGroup2011 input) => input)),
				Name = resource.Name,
				ParentResourceGroup = resource.ParentResourceGroup,
				Permissions = resource.Permissions,
				ResourceType = resource.ResourceType,
				UniqueId = resource.UniqueId
			};
		}
	}
}
