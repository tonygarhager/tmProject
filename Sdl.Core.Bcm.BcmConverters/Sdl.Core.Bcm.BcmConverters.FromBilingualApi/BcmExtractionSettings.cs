namespace Sdl.Core.Bcm.BcmConverters.FromBilingualApi
{
	public class BcmExtractionSettings
	{
		public bool ProcessComments
		{
			get;
			set;
		}

		public bool GenerateContextsDependencyFile
		{
			get;
			set;
		}

		public BcmExtractionSettings()
		{
			ProcessComments = true;
		}
	}
}
