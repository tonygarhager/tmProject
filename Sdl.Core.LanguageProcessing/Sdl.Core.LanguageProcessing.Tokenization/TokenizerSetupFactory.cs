using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Resources;
using Sdl.LanguagePlatform.Core.Tokenization;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Sdl.Core.LanguageProcessing.Tokenization
{
	public class TokenizerSetupFactory
	{
		public static TokenizerSetup Create(CultureInfo culture)
		{
			return Create(culture, BuiltinRecognizers.RecognizeDates | BuiltinRecognizers.RecognizeTimes | BuiltinRecognizers.RecognizeNumbers | BuiltinRecognizers.RecognizeAcronyms | BuiltinRecognizers.RecognizeMeasurements | BuiltinRecognizers.RecognizeAlphaNumeric, TokenizerFlags.DefaultFlags);
		}

		public static TokenizerSetup Create(CultureInfo culture, BuiltinRecognizers recognizers)
		{
			return Create(culture, recognizers, TokenizerFlags.DefaultFlags);
		}

		public static TokenizerSetup Create(CultureInfo culture, BuiltinRecognizers recognizers, TokenizerFlags flags)
		{
			return new TokenizerSetup
			{
				BreakOnWhitespace = CultureInfoExtensions.UseBlankAsWordSeparator(culture),
				Culture = culture,
				CreateWhitespaceTokens = false,
				BuiltinRecognizers = recognizers,
				TokenizerFlags = flags
			};
		}

		public static void Write(TokenizerSetup setup, Stream output)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(setup.GetType());
			xmlSerializer.Serialize(output, setup);
		}

		public static void Write(TokenizerSetup setup, TextWriter output)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(setup.GetType());
			xmlSerializer.Serialize(output, setup);
		}

		public static void Write(TokenizerSetup setup, string path)
		{
			using (StreamWriter output = new StreamWriter(path, append: false, Encoding.UTF8))
			{
				Write(setup, output);
			}
		}

		public static TokenizerSetup Load(Stream input)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(TokenizerSetup));
			return xmlSerializer.Deserialize(input) as TokenizerSetup;
		}

		public static TokenizerSetup Load(StreamReader input)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(TokenizerSetup));
			return xmlSerializer.Deserialize(input) as TokenizerSetup;
		}

		public static TokenizerSetup Load(IResourceDataAccessor accessor, CultureInfo culture)
		{
			using (Stream input = accessor.ReadResourceData(culture, LanguageResourceType.TokenizerSettings, fallback: true))
			{
				return Load(input);
			}
		}

		public static TokenizerSetup Load(string path)
		{
			using (StreamReader input = new StreamReader(path, Encoding.UTF8, detectEncodingFromByteOrderMarks: true))
			{
				return Load(input);
			}
		}
	}
}
