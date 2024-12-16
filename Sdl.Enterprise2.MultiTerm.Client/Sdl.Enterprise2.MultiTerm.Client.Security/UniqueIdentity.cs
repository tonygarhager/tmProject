using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Security.Principal;

namespace Sdl.Enterprise2.MultiTerm.Client.Security
{
	[Serializable]
	public class UniqueIdentity : MarshalByRefObject, IIdentity, ISerializable
	{
		private string authenticationType;

		private Guid id = Guid.Empty;

		private readonly bool isAuthenticated;

		private string name;

		public Guid Id
		{
			get
			{
				return id;
			}
			internal set
			{
				id = value;
			}
		}

		public string AuthenticationType
		{
			get
			{
				return authenticationType;
			}
			internal set
			{
				authenticationType = value;
			}
		}

		public virtual bool IsAuthenticated => isAuthenticated;

		public string Name
		{
			get
			{
				return name;
			}
			internal set
			{
				name = value;
			}
		}

		public UniqueIdentity()
		{
			id = Guid.Empty;
			name = string.Empty;
			authenticationType = string.Empty;
		}

		private UniqueIdentity(SerializationInfo info, StreamingContext context)
		{
			authenticationType = info.GetString("authenticationType");
			id = new Guid(info.GetString("id"));
			isAuthenticated = info.GetBoolean("isAuthenticated");
			name = info.GetString("name");
		}

		internal UniqueIdentity(Guid userId, string userName)
			: this(userId, null, isAuthenticated: false, userName)
		{
		}

		internal UniqueIdentity(Guid uniqueId)
			: this(uniqueId, null, isAuthenticated: false, null)
		{
		}

		internal UniqueIdentity(Guid uniqueId, string authenticationType, bool isAuthenticated, string name)
			: this()
		{
			Id = uniqueId;
			if (!string.IsNullOrEmpty(authenticationType))
			{
				this.authenticationType = authenticationType;
			}
			this.isAuthenticated = isAuthenticated;
			if (!string.IsNullOrEmpty(name))
			{
				this.name = name;
			}
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("authenticationType", authenticationType);
			info.AddValue("id", id.ToString());
			info.AddValue("isAuthenticated", isAuthenticated);
			info.AddValue("name", name);
		}
	}
}
