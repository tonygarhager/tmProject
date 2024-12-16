namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public interface IApplicationPreviewType : IPreviewType
	{
		PreviewApplicationId? SingleFilePreviewApplicationId
		{
			get;
			set;
		}

		PreviewApplicationId? SourceAndTargetPreviewApplicationId
		{
			get;
			set;
		}
	}
}
