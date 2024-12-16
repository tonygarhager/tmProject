using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Sdl.Core.Bcm.BcmModel
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum FeedbackType
	{
		Added = 1,
		Deleted,
		Comment
	}
}
