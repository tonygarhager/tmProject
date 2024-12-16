namespace Sdl.Core.PluginFramework
{
	public class PluginState
	{
		private bool _enabled;

		public bool Enabled
		{
			get
			{
				return _enabled;
			}
			set
			{
				_enabled = value;
			}
		}

		public PluginState(bool enabled)
		{
			_enabled = enabled;
		}
	}
}
