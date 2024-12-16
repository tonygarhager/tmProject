using Newtonsoft.Json;
using System.Collections.Generic;

namespace Sdl.Core.Globalization.LanguageRegistry
{
	public class LanguageRegistry
	{
		[JsonProperty(Order = 2)]
		public IList<Language> AllLanguages;

		[JsonProperty(Order = 1)]
		public string Version => "1.4.4";
	}
}
