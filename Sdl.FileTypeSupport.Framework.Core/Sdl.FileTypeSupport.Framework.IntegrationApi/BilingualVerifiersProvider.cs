using Sdl.FileTypeSupport.Framework.BilingualApi;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public delegate IList<IBilingualVerifier> BilingualVerifiersProvider(FileTypeDefinitionId fileTypeDefinitionToUse);
}
