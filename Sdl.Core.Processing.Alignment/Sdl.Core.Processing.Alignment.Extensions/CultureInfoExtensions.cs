using System.Globalization;

namespace Sdl.Core.Processing.Alignment.Extensions
{
	internal static class CultureInfoExtensions
	{
		internal static CultureInfo GetCultureInfoFromName(this string cultureInfoName)
		{
			return CultureInfo.GetCultureInfo(cultureInfoName);
		}
	}
}
