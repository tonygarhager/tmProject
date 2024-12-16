using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public interface IVerifierCollection : IFileTypeDefinitionAware
	{
		List<INativeFileVerifier> NativeVerifiers
		{
			get;
			set;
		}

		List<IBilingualVerifier> BilingualVerifiers
		{
			get;
			set;
		}
	}
}
