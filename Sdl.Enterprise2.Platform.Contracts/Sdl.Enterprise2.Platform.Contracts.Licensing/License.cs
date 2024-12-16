using System;
using System.Runtime.Serialization;

namespace Sdl.Enterprise2.Platform.Contracts.Licensing
{
	[DataContract(Name = "License", Namespace = "http://sdl.com/enterprise/platform/2011", IsReference = true)]
	public class License
	{
		[DataMember]
		public Guid Id
		{
			get;
			private set;
		}

		[DataMember]
		public DateTime Expires
		{
			get;
			set;
		}

		public License()
		{
		}

		public License(DateTime expires)
		{
			Id = Guid.NewGuid();
			Expires = expires;
		}
	}
}
