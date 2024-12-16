using System.Collections.Generic;

namespace Sdl.Common.Licensing.Provider.Core
{
	public interface INativeLicense
	{
		IList<string> GetNativeFeaturesIds();
	}
}
