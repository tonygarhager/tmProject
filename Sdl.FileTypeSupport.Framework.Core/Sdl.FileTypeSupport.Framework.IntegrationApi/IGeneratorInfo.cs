using Sdl.Core.Globalization;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public interface IGeneratorInfo
	{
		GeneratorId Id
		{
			get;
			set;
		}

		LocalizableString Description
		{
			get;
			set;
		}
	}
}
