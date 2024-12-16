using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmModel
{
	[DataContract]
	[JsonConverter(typeof(StringEnumConverter))]
	public enum RevisionType
	{
		Inserted = 1,
		Deleted,
		Unchanged
	}
}
