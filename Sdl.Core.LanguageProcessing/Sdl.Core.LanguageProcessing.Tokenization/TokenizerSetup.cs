using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using System.Globalization;
using System.Xml.Serialization;

namespace Sdl.Core.LanguageProcessing.Tokenization
{
	public class TokenizerSetup
	{
		[XmlAttribute("createWhitespaceTokens")]
		public bool CreateWhitespaceTokens;

		[XmlAttribute("breakOnWhitespace")]
		public bool BreakOnWhitespace;

		public bool SeparateClitics;

		public BuiltinRecognizers BuiltinRecognizers;

		public TokenizerFlags TokenizerFlags;

		[XmlAttribute("culture")]
		public string CultureName
		{
			get
			{
				return Culture.Name;
			}
			set
			{
				Culture = CultureInfoExtensions.GetCultureInfo(value);
			}
		}

		[XmlIgnore]
		public CultureInfo Culture
		{
			get;
			set;
		}
	}
}
