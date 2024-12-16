namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public interface IFileTypeComponentBuilderAdapter : IFileTypeComponentBuilder, IFileTypeDefinitionAware
	{
		IFileTypeComponentBuilder Original
		{
			get;
			set;
		}
	}
}
