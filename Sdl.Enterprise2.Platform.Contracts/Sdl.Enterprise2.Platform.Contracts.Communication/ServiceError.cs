using System;
using System.Runtime.Serialization;

namespace Sdl.Enterprise2.Platform.Contracts.Communication
{
	[DataContract(Namespace = "http://sdl.com/enterprise/platform/2010")]
	public class ServiceError
	{
		[DataMember]
		public int ErrorCode
		{
			get;
			set;
		}

		[DataMember]
		public string Source
		{
			get;
			set;
		}

		[DataMember]
		public DateTime ErrorTime
		{
			get;
			set;
		}

		[DataMember]
		public string AssemblyQualifiedName
		{
			get;
			set;
		}
	}
}
