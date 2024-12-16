using System;
using System.Runtime.Serialization;

namespace Sdl.Enterprise2.Platform.Contracts.AsyncLoading
{
	[DataContract(Name = "ResultSetSelector", Namespace = "http://sdl.com/enterprise/platform/2011", IsReference = true)]
	public class ResultSetSelector
	{
		[DataMember]
		public Guid? ResultSetId
		{
			get;
			set;
		}

		[DataMember]
		public int StartIndex
		{
			get;
			set;
		}

		[DataMember]
		public int Count
		{
			get;
			set;
		}
	}
}
