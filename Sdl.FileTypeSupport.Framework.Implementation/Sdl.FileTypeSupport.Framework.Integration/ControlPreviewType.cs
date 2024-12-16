using Sdl.FileTypeSupport.Framework.IntegrationApi;

namespace Sdl.FileTypeSupport.Framework.Integration
{
	public class ControlPreviewType : PreviewType, IControlPreviewType, IPreviewType
	{
		private PreviewControlId? _singleFilePreviewControlId;

		private PreviewControlId? _sourceAndTargetPreviewControlId;

		public PreviewControlId? SingleFilePreviewControlId
		{
			get
			{
				return _singleFilePreviewControlId;
			}
			set
			{
				_singleFilePreviewControlId = value;
			}
		}

		public PreviewControlId? SourceAndTargetPreviewControlId
		{
			get
			{
				return _sourceAndTargetPreviewControlId;
			}
			set
			{
				_sourceAndTargetPreviewControlId = value;
			}
		}
	}
}
