namespace Sdl.FileTypeSupport.Framework.PreviewControls
{
	/// <summary>
	/// An interface that allows adding a TempFileProperties property to an existing class and allows these
	/// properties to be accessed using a consistent way.
	/// </summary>
	public interface ITempFileProperties
	{
		/// <summary>
		/// A TempFileProperties property that can be accessed or set in a standard way.
		/// </summary>
		TempFileProperties TempFileProperties
		{
			get;
			set;
		}
	}
}
