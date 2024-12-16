namespace Sdl.Core.FineGrainedAlignment
{
	public class AlignedPhrase
	{
		private readonly int _srcStartPosition;

		private readonly int _srcLength;

		private readonly int _trgStartPosition;

		private readonly int _trgLength;

		private readonly float _association;

		public int SrcStartPosition => _srcStartPosition;

		public int SrcLength => _srcLength;

		public int TrgStartPosition => _trgStartPosition;

		public int TrgLength => _trgLength;

		public float Association => _association;

		public AlignedPhrase(int srcStartPosition, int srcLength, int trgStartPosition, int trgLength, float association)
		{
			_srcStartPosition = srcStartPosition;
			_srcLength = srcLength;
			_trgStartPosition = trgStartPosition;
			_trgLength = trgLength;
			_association = association;
		}
	}
}
