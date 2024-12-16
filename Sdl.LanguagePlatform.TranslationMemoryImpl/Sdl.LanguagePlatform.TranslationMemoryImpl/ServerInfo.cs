using Sdl.LanguagePlatform.Core;
using System;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	[DataContract]
	public class ServerInfo
	{
		public enum ServerStatus
		{
			Unknown,
			Running,
			Stopped
		}

		private DateTime _Servertime;

		private DateTime _servertimeUtc;

		[DataMember]
		public string Version
		{
			get;
			set;
		}

		[DataMember]
		public string StatusMessage
		{
			get;
			set;
		}

		[DataMember]
		public ServerStatus Status
		{
			get;
			set;
		}

		[DataMember]
		public DateTime ServerTime
		{
			get
			{
				return _Servertime;
			}
			set
			{
				_Servertime = DateTimeUtilities.Normalize(value);
			}
		}

		[DataMember]
		public DateTime ServerTimeUTC
		{
			get
			{
				return _servertimeUtc;
			}
			set
			{
				_servertimeUtc = DateTimeUtilities.Normalize(value);
			}
		}

		[DataMember]
		public string UserName
		{
			get;
			set;
		}

		public ServerInfo(string version, string msg, ServerStatus status, string userName)
		{
			Version = version;
			StatusMessage = msg;
			Status = status;
			_Servertime = DateTime.Now;
			_servertimeUtc = DateTimeUtilities.Normalize(_Servertime);
			UserName = userName;
		}
	}
}
