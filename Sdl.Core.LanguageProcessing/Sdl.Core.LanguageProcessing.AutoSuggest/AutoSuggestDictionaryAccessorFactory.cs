using System;
using System.Globalization;

namespace Sdl.Core.LanguageProcessing.AutoSuggest
{
	public static class AutoSuggestDictionaryAccessorFactory
	{
		public static IAutoSuggestDictionaryAccessor Create(Uri uri, CultureInfo sourceCulture, CultureInfo targetCulture)
		{
			if (uri.IsFile)
			{
				return new FileBasedAutoSuggestDictionaryAccessor(uri.LocalPath, sourceCulture, targetCulture);
			}
			throw new ArgumentException("Type of URI is not supported");
		}
	}
}
