namespace Sdl.Core.LanguageProcessing.Segmentation.Serialization
{
	internal class StartRevisionMarkerToken : Token
	{
		public int RevisionMarkerTokenId
		{
			get;
		}

		public StartRevisionMarkerToken(int revisionMarkerTokenId)
		{
			RevisionMarkerTokenId = revisionMarkerTokenId;
		}
	}
}
