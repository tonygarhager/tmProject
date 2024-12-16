using System.ComponentModel;

namespace Sdl.Desktop.IntegrationApi.Extensions
{
	public abstract class AbstractLayoutConfiguration
	{
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string Id
		{
			get;
			protected set;
		}
	}
}
