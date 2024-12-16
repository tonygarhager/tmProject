namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public interface IControlPreviewType : IPreviewType
	{
		PreviewControlId? SingleFilePreviewControlId
		{
			get;
			set;
		}

		PreviewControlId? SourceAndTargetPreviewControlId
		{
			get;
			set;
		}
	}
}
