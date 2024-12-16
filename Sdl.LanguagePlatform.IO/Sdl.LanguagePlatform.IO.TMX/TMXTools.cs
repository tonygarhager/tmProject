using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.IO.Streams;
using Sdl.LanguagePlatform.TranslationMemory;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.LanguagePlatform.IO.TMX
{
	public static class TMXTools
	{
		public static LanguagePair GetLanguageDirection(string fileName, out CultureInfo headerSourceLanguage)
		{
			headerSourceLanguage = null;
			try
			{
				TMXReaderSettings tMXReaderSettings = new TMXReaderSettings();
				tMXReaderSettings.ValidateAgainstSchema = false;
				TMXReader tMXReader = new TMXReader(fileName, tMXReaderSettings);
				LanguagePair languagePair = new LanguagePair((CultureInfo)null, (CultureInfo)null);
				int num = 0;
				Event @event;
				while ((@event = tMXReader.Read()) != null && (languagePair.SourceCulture == null || languagePair.TargetCulture == null) && num < 100)
				{
					if (@event is TMXStartOfInputEvent)
					{
						TMXStartOfInputEvent tMXStartOfInputEvent = @event as TMXStartOfInputEvent;
						headerSourceLanguage = tMXStartOfInputEvent.SourceCulture;
					}
					else if (@event is TUEvent)
					{
						TranslationUnit translationUnit = (@event as TUEvent).TranslationUnit;
						if (translationUnit.SourceSegment != null)
						{
							if (languagePair.SourceCulture == null)
							{
								languagePair.SourceCulture = translationUnit.SourceSegment.Culture;
							}
							else if (!object.Equals(translationUnit.SourceSegment.Culture, languagePair.SourceCulture))
							{
								languagePair.SourceCulture = CultureInfo.InvariantCulture;
							}
						}
						if (translationUnit.TargetSegment != null)
						{
							if (languagePair.TargetCulture == null)
							{
								languagePair.TargetCulture = translationUnit.TargetSegment.Culture;
							}
							else if (!object.Equals(translationUnit.TargetSegment.Culture, languagePair.TargetCulture))
							{
								languagePair.TargetCulture = CultureInfo.InvariantCulture;
							}
						}
					}
					else if (!(@event is EndOfInputEvent))
					{
						throw new LanguagePlatformException(ErrorCode.TMXUnexpectedInputData);
					}
				}
				return languagePair;
			}
			catch
			{
				return null;
			}
		}

		public static IList<CultureInfo> GetLanguages(string fileName, int limit, out int scannedTUs)
		{
			HashSet<CultureInfo> hashSet = new HashSet<CultureInfo>();
			using (TMXReader tMXReader = new TMXReader(fileName, null))
			{
				scannedTUs = 0;
				Event @event;
				while ((@event = tMXReader.Read()) != null)
				{
					if (!(@event is TMXStartOfInputEvent))
					{
						if (@event is TUEvent)
						{
							TranslationUnit translationUnit = ((TUEvent)@event).TranslationUnit;
							if (translationUnit.SourceSegment != null && translationUnit.TargetSegment != null)
							{
								CultureInfo culture = translationUnit.SourceSegment.Culture;
								CultureInfo culture2 = translationUnit.TargetSegment.Culture;
								hashSet.Add(culture);
								hashSet.Add(culture2);
								scannedTUs++;
								if (limit > 0 && scannedTUs >= limit)
								{
									break;
								}
							}
						}
						else if (!(@event is EndOfInputEvent))
						{
							throw new LanguagePlatformException(ErrorCode.TMXUnexpectedInputData);
						}
					}
				}
			}
			return new List<CultureInfo>(hashSet);
		}
	}
}
