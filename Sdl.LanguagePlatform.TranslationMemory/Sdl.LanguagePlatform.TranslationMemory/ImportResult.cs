using Sdl.LanguagePlatform.Core;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	public class ImportResult
	{
		[DataMember]
		public Action Action
		{
			get;
			set;
		}

		[DataMember]
		public ErrorCode ErrorCode
		{
			get;
			set;
		}

		[DataMember]
		public PersistentObjectToken TuId
		{
			get;
			set;
		}

		public ImportResult()
			: this(Action.Discard, ErrorCode.OK)
		{
		}

		public ImportResult(Action action)
		{
			Action = action;
			ErrorCode = ((action == Action.Error) ? ErrorCode.Other : ErrorCode.OK);
		}

		public ImportResult(Action action, ErrorCode errorCode)
		{
			Action = action;
			ErrorCode = errorCode;
		}

		public override string ToString()
		{
			return $"{Action}-{TuId}-{ErrorCode}";
		}
	}
}
