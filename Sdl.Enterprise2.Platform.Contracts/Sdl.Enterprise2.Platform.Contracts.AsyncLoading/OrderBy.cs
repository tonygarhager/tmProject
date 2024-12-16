using System.Runtime.Serialization;

namespace Sdl.Enterprise2.Platform.Contracts.AsyncLoading
{
	[DataContract(Name = "OrderBy", Namespace = "http://sdl.com/enterprise/platform/2011", IsReference = true)]
	public class OrderBy
	{
		[DataMember]
		public string Field
		{
			get;
			set;
		}

		[DataMember]
		public OrderByDirection Direction
		{
			get;
			set;
		}
	}
}
