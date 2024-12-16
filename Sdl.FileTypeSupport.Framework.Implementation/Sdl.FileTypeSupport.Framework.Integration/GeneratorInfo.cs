using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.IntegrationApi;

namespace Sdl.FileTypeSupport.Framework.Integration
{
	public class GeneratorInfo : IGeneratorInfo
	{
		public GeneratorId Id
		{
			get;
			set;
		}

		public LocalizableString Description
		{
			get;
			set;
		}
	}
}
