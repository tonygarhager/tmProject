using System;
using System.Runtime.Serialization;

namespace Sdl.Enterprise2.Platform.Contracts.Discovery
{
	[DataContract(Name = "EndpointInfo", Namespace = "http://sdl.com/router/2010")]
	public class ServiceEndpointInfo
	{
		[DataMember]
		public string Name
		{
			get;
			set;
		}

		[DataMember]
		public Uri Address
		{
			get;
			set;
		}

		[DataMember]
		public string Contract
		{
			get;
			set;
		}

		[DataMember]
		public string Binding
		{
			get;
			set;
		}

		[DataMember]
		public string BindingConfiguration
		{
			get;
			set;
		}

		[DataMember]
		public string SecurityMode
		{
			get;
			set;
		}

		[DataMember]
		public string MessageCredentialType
		{
			get;
			set;
		}
	}
}
