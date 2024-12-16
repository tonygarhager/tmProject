using System;
using System.Runtime.Serialization;

namespace Sdl.Enterprise2.Platform.Contracts.ApplicationDiscoveryService
{
	[DataContract(Name = "EndpointInfo", Namespace = "http://sdl.com/applicationdiscoveryservice/2014")]
	public class ApplicationServiceEndpointInfo
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
	}
}
