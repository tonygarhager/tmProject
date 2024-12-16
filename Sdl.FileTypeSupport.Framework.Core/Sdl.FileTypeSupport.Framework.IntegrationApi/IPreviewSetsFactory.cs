using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public interface IPreviewSetsFactory : IFileTypeDefinitionAware
	{
		bool IsFileAgnostic
		{
			get;
		}

		IPreviewSets GetPreviewSets(IFileProperties fileProperties);

		IPreviewSet CreatePreviewSet();

		IPreviewType CreatePreviewType<T>();
	}
}
