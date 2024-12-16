using System;
using System.Runtime.Serialization;

namespace Sdl.Enterprise2.Platform.Contracts.AsyncLoading
{
	[DataContract(Name = "PartialResultSet", Namespace = "http://sdl.com/enterprise/platform/2011", IsReference = true)]
	public class PartialResultSet<T>
	{
		[DataMember]
		public Guid ResultSetId
		{
			get;
			set;
		}

		[DataMember]
		public T[] Results
		{
			get;
			set;
		}

		[DataMember]
		public int FirstResultIndex
		{
			get;
			set;
		}

		[DataMember]
		public int TotalResultCount
		{
			get;
			set;
		}
	}
}
