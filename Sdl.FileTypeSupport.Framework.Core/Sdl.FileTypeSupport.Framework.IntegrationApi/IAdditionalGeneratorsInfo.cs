using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public interface IAdditionalGeneratorsInfo : IFileTypeDefinitionAware
	{
		IList<IGeneratorInfo> Generators
		{
			get;
		}
	}
}
