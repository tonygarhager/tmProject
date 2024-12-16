using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Sdl.Core.Bcm.BcmModel
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum ConfirmationLevel
	{
		NotTranslated,
		Draft,
		Translated,
		RejectedTranslation,
		ApprovedTranslation,
		RejectedSignOff,
		ApprovedSignOff
	}
}
