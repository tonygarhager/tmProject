using System.Runtime.Serialization;

namespace Sdl.Enterprise2.Platform.Contracts.Communication.Callback
{
	[DataContract]
	public class CallbackEventArgs
	{
		[DataMember]
		public bool Error
		{
			get;
			set;
		}

		[DataMember]
		public string ErrorMessage
		{
			get;
			set;
		}

		[DataMember]
		public string StackTrace
		{
			get;
			set;
		}

		[DataMember]
		public bool IsComplete
		{
			get;
			set;
		}

		[DataMember]
		public string Action
		{
			get;
			set;
		}

		[DataMember]
		public int? PercentComplete
		{
			get;
			set;
		}

		[DataMember]
		public int? Count
		{
			get;
			set;
		}
	}
	[DataContract]
	public class CallbackEventArgs<T> : CallbackEventArgs
	{
		[DataMember]
		public T UserData
		{
			get;
			set;
		}
	}
}
