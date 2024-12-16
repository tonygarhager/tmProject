namespace Sdl.Core.Settings
{
	public abstract class Setting<T>
	{
		protected bool _inherited;

		public abstract string Id
		{
			get;
		}

		public virtual bool Inherited => _inherited;

		public abstract T Value
		{
			get;
			set;
		}

		public static implicit operator T(Setting<T> setting)
		{
			return setting.Value;
		}

		public abstract void Reset();
	}
}
