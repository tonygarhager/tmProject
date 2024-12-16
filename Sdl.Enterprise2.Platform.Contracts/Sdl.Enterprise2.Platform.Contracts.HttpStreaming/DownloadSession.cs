using System;
using System.Runtime.Serialization;

namespace Sdl.Enterprise2.Platform.Contracts.HttpStreaming
{
	[DataContract(Namespace = "http://sdl.com/enterprise/platform/2010", Name = "DownloadSession")]
	public class DownloadSession
	{
		[DataMember]
		public Guid SessionId
		{
			get;
			set;
		}

		[DataMember]
		public long? Length
		{
			get;
			set;
		}
	}
}
