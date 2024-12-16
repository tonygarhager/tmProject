namespace Sdl.Core.Processing.Alignment.Core
{
	internal interface IAlignmentArray
	{
		int Width
		{
			get;
		}

		int Height
		{
			get;
		}

		AlignmentArrayElement this[int x, int y]
		{
			get;
			set;
		}

		bool IsValid(int x, int y);

		bool IsValidAndDefined(int x, int y);
	}
}
