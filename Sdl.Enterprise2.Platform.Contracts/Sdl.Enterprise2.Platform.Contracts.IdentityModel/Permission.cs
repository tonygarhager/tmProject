using System;
using System.Runtime.Serialization;

namespace Sdl.Enterprise2.Platform.Contracts.IdentityModel
{
	[DataContract(Name = "Permission", Namespace = "http://sdl.com/identity/2010", IsReference = true)]
	public class Permission : IExtensibleDataObject
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
		public string DisplayName
		{
			get;
			set;
		}

		[DataMember(Order = 3)]
		public string Description
		{
			get;
			set;
		}

		public bool IsOfType(string resourceType)
		{
			if (string.IsNullOrEmpty(resourceType))
			{
				throw new ArgumentNullException("resourceType");
			}
			string[] array = Name.Split('.');
			if (array.Length == 2 && array[0].Equals(resourceType, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			return false;
		}
	}
}
