using System.Globalization;
using System.Threading;

namespace Sdl.Core.Bcm.BcmConverters.FromBilingualApi.Helpers
{
	internal class IdGenerator
	{
		private int _id;

		public string GetNextNumericalId()
		{
			Interlocked.Increment(ref _id);
			return _id.ToString("d", CultureInfo.InvariantCulture);
		}
	}
}
