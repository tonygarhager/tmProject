using Sdl.FileTypeSupport.Framework.IntegrationApi;

namespace Sdl.FileTypeSupport.Framework.Integration
{
	public class ApplicationPreviewType : PreviewType, IApplicationPreviewType, IPreviewType
	{
		private PreviewApplicationId? _singleFilePreviewApplicationId;

		private PreviewApplicationId? _sourceAndTargetPreviewApplicationId;

		public PreviewApplicationId? SingleFilePreviewApplicationId
		{
			get
			{
				return _singleFilePreviewApplicationId;
			}
			set
			{
				_singleFilePreviewApplicationId = value;
			}
		}

		public PreviewApplicationId? SourceAndTargetPreviewApplicationId
		{
			get
			{
				return _sourceAndTargetPreviewApplicationId;
			}
			set
			{
				_sourceAndTargetPreviewApplicationId = value;
			}
		}
	}
}
