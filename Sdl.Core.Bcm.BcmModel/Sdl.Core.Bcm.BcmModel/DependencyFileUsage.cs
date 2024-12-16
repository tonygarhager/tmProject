using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Sdl.Core.Bcm.BcmModel
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum DependencyFileUsage
	{
		None,
		Extraction,
		Generation,
		Final
	}
}
