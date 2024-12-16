using System;
using System.Runtime.Serialization;

namespace Sdl.Enterprise2.Platform.Contracts.HttpStreaming
{
	[DataContract(Namespace = "http://sdl.com/enterprise/platform/2010", Name = "UploadSession")]
	public class UploadSession
	{
		[DataMember]
		public Guid SessionId
		{
			get;
			set;
		}
	}
}
