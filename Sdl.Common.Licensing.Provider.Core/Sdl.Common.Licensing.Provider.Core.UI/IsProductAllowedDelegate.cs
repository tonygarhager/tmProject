using System.Text;

namespace Sdl.Common.Licensing.Provider.Core.UI
{
	public delegate bool IsProductAllowedDelegate(IProductLicense productLicense, StringBuilder message);
}
