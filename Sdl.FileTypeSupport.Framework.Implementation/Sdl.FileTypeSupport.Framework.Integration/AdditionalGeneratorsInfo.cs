using Sdl.FileTypeSupport.Framework.IntegrationApi;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.Integration
{
	public class AdditionalGeneratorsInfo : AbstractFileTypeDefinitionComponent, IAdditionalGeneratorsInfo, IFileTypeDefinitionAware
	{
		private IList<IGeneratorInfo> _list = new List<IGeneratorInfo>();

		public IList<IGeneratorInfo> Generators => _list;

		public AdditionalGeneratorsInfo()
		{
		}

		public AdditionalGeneratorsInfo(params IGeneratorInfo[] list)
		{
			SetupAdditionalGeneratorsInfo(list);
		}

		private void SetupAdditionalGeneratorsInfo(IList<IGeneratorInfo> list)
		{
			_list = list;
		}
	}
}
