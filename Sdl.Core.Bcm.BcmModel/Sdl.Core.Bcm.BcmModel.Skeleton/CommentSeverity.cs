using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmModel.Skeleton
{
	[DataContract]
	[JsonConverter(typeof(StringEnumConverter))]
	public enum CommentSeverity
	{
		Low = 1,
		Medium,
		High
	}
}
