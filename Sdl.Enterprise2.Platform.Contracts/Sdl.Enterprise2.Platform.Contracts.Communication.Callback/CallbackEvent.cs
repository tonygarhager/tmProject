using System.Runtime.Serialization;

namespace Sdl.Enterprise2.Platform.Contracts.Communication.Callback
{
	[DataContract]
	public class CallbackEvent
	{
		[DataMember]
		public CallbackId CallbackContextId
		{
			get;
			set;
		}

		[DataMember]
		public string SerialisedEventArgs
		{
			get;
			set;
		}
	}
}
