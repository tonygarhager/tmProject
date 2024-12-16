namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public interface IAbstractInlineTagProperties
	{
		bool IsWordStop
		{
			get;
			set;
		}

		bool IsSoftBreak
		{
			get;
			set;
		}

		bool CanHide
		{
			get;
			set;
		}
	}
}
