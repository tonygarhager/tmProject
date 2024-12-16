using System;
using System.Runtime.Serialization;

namespace Sdl.Enterprise2.Platform.Contracts.Communication.Callback
{
	[DataContract]
	public class CallbackId : IEquatable<CallbackId>
	{
		[DataMember]
		public Guid ClientId
		{
			get;
			set;
		}

		[DataMember]
		public Guid EventId
		{
			get;
			set;
		}

		public bool Equals(CallbackId other)
		{
			if (other.ClientId == ClientId)
			{
				return other.EventId == EventId;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			CallbackId callbackId = obj as CallbackId;
			if (callbackId != null)
			{
				return Equals(callbackId);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return ClientId.GetHashCode() ^ (EventId.GetHashCode() * 29);
		}

		public override string ToString()
		{
			return "CallbackId { ClientId=" + ClientId.ToString() + ",EventId=" + EventId.ToString() + "}";
		}
	}
}
