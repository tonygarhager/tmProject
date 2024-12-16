using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Core.Processing.Alignment.ReverseAlignment
{
	public class ParagraphUnitIdPair
	{
		public ParagraphUnitId Left
		{
			get;
			set;
		}

		public ParagraphUnitId Right
		{
			get;
			set;
		}

		public ParagraphUnitIdPair(ParagraphUnitId left, ParagraphUnitId right)
		{
			Left = left;
			Right = right;
		}

		public override string ToString()
		{
			return $"{Left.ToString()} => {Right.ToString()}";
		}
	}
}
