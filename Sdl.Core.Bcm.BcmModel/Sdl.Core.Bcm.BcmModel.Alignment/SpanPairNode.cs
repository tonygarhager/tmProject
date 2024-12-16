namespace Sdl.Core.Bcm.BcmModel.Alignment
{
	public class SpanPairNode : ExtensionDataContainer
	{
		public LiftSpan SourceSpan
		{
			get;
			set;
		}

		public LiftSpan TargetSpan
		{
			get;
			set;
		}

		public float Confidence
		{
			get;
			set;
		}

		public int Provenance
		{
			get;
			set;
		}
	}
}
