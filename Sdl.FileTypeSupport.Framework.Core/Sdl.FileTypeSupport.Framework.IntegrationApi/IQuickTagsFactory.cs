using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public interface IQuickTagsFactory : IFileTypeDefinitionAware
	{
		bool IsFileAgnostic
		{
			get;
		}

		IQuickTags GetQuickTags(IFileProperties fileProperties);
	}
}
