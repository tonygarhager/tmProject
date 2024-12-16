using System;
using System.Runtime.Serialization;

namespace Sdl.Enterprise2.Platform.Contracts.Communication
{
	[DataContract(Namespace = "http://sdl.com/studioserver/platform/2012")]
	public class ServiceFault
	{
		[DataMember]
		public Guid ErrorId
		{
			get;
			set;
		}

		[DataMember]
		public DateTime ServerTime
		{
			get;
			set;
		}

		[DataMember]
		public object Data
		{
			get;
			set;
		}
	}
}
