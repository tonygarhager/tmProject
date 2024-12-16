using System;
using System.Runtime.Serialization;

namespace Sdl.Enterprise2.Platform.Contracts.IdentityModel
{
	[DataContract(Name = "Role", Namespace = "http://sdl.com/identity/2010", IsReference = true)]
	public class Role2009 : IExtensibleDataObject
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

		[DataMember(Order = 2)]
		public Permission[] Permissions
		{
			get;
			set;
		}
	}
}
