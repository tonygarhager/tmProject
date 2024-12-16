namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public interface IFileTypeDefinitionConfiguringControl
	{
		FileTypeDefinitionId FileTypeDefintionId
		{
			set;
		}

		bool Save();
	}
}
