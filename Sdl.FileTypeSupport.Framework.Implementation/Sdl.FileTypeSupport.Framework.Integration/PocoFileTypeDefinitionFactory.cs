using Sdl.FileTypeSupport.Framework.IntegrationApi;
using System;

namespace Sdl.FileTypeSupport.Framework.Integration
{
	public class PocoFileTypeDefinitionFactory : IFileTypeDefinitionFactory
	{
		public void ApplyFileTypeInformation(IFileTypeDefinition definition, FileTypeProfile fileTypeProfile)
		{
			FileTypeProfile.ApplyProfile(definition, fileTypeProfile);
		}

		public IFileTypeDefinition CreateFromFile(string filePath)
		{
			throw new NotImplementedException();
		}

		public IFileTypeDefinition CreateFromResource(string path)
		{
			throw new NotImplementedException();
		}
	}
}
