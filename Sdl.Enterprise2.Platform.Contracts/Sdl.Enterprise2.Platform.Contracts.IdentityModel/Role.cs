using System;
using System.Runtime.Serialization;

namespace Sdl.Enterprise2.Platform.Contracts.IdentityModel
{
	[DataContract(Name = "Role", Namespace = "http://sdl.com/identity/2012", IsReference = true)]
	public class Role
	{
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

		[DataMember(Order = 3)]
		public bool Protected
		{
			get;
			set;
		}

		public static implicit operator Role2009(Role role2012)
		{
			if (role2012 == null)
			{
				return null;
			}
			return new Role2009
			{
				Name = role2012.Name,
				Permissions = role2012.Permissions,
				UniqueId = role2012.UniqueId
			};
		}

		public static implicit operator Role(Role2009 role)
		{
			if (role == null)
			{
				return null;
			}
			return new Role
			{
				Name = role.Name,
				Permissions = role.Permissions,
				UniqueId = role.UniqueId
			};
		}
	}
}
