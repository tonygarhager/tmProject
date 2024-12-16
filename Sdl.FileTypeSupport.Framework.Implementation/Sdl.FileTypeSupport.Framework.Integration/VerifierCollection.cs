using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.Integration
{
	public class VerifierCollection : AbstractFileTypeDefinitionComponent, IVerifierCollection, IFileTypeDefinitionAware
	{
		private List<IBilingualVerifier> _bilingualVerifiers = new List<IBilingualVerifier>();

		private List<INativeFileVerifier> _nativeVerifiers = new List<INativeFileVerifier>();

		public List<IBilingualVerifier> BilingualVerifiers
		{
			get
			{
				return _bilingualVerifiers;
			}
			set
			{
				_bilingualVerifiers = value;
			}
		}

		public List<INativeFileVerifier> NativeVerifiers
		{
			get
			{
				return _nativeVerifiers;
			}
			set
			{
				_nativeVerifiers = value;
			}
		}
	}
}
