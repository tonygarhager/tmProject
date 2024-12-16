using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Sdl.Core.Bcm.BcmModel
{
	public abstract class ExtensionDataContainer
	{
		[JsonExtensionData]
		public IDictionary<string, JToken> ExtensionData
		{
			get;
			set;
		}
	}
}
