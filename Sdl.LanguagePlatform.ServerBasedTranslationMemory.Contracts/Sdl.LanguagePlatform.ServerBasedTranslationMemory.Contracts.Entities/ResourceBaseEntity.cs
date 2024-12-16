using Sdl.Core.Api.DataAccess;
using Sdl.Enterprise2.Platform.Contracts.IdentityModel;
using System;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities
{
	[DataContract(Name = "ResourceBase", Namespace = "http://sdl.com/languageplatform/2010", IsReference = true)]
	public class ResourceBaseEntity : Entity, IExtensibleDataObject
	{
		private ExtensionDataObject _extensionDataObject;

		private Guid? _uniqueId;

		[IgnoreEntityMember]
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

		[DataMember]
		[EntityColumn]
		public Guid? UniqueId
		{
			get
			{
				return _uniqueId;
			}
			set
			{
				if (!(_uniqueId == value))
				{
					_uniqueId = value;
				}
			}
		}

		[IgnoreEntityMember]
		[DataMember]
		public string ParentResourceGroupName
		{
			get;
			set;
		}

		[IgnoreEntityMember]
		[DataMember]
		public string ParentResourceGroupPath
		{
			get;
			set;
		}

		[IgnoreEntityMember]
		[DataMember]
		public string ParentResourceGroupDescription
		{
			get;
			set;
		}

		[IgnoreEntityMember]
		[DataMember]
		public string[] LinkedResourceGroupPaths
		{
			get;
			set;
		}

		[IgnoreEntityMember]
		[DataMember]
		public PermissionCollection Permissions
		{
			get;
			set;
		}

		[IgnoreEntityMember]
		public Guid ResourceGroupId
		{
			get;
			set;
		}
	}
}
