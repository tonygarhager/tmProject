namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public interface IFileTypeDefinitionFactory
	{
		IFileTypeDefinition CreateFromFile(string filePath);

		IFileTypeDefinition CreateFromResource(string path);

		void ApplyFileTypeInformation(IFileTypeDefinition definition, FileTypeProfile fileTypeProfile);
	}
}
