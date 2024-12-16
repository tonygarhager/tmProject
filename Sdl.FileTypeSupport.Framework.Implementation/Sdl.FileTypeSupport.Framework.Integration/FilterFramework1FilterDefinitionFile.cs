using Sdl.FileTypeSupport.Framework.IntegrationApi;
using System;

namespace Sdl.FileTypeSupport.Framework.Integration
{
	[Serializable]
	public class FilterFramework1FilterDefinitionFile : IFilterFramework1FilterDefinitionFile
	{
		private string _FilterFramework1FilterDefinitionFilePath;

		private string _FilterFramework1FilterDefinitionFileContent;

		public string FilterFramework1FilterDefinitionFilePath
		{
			get
			{
				return _FilterFramework1FilterDefinitionFilePath;
			}
			set
			{
				_FilterFramework1FilterDefinitionFilePath = value;
			}
		}

		public string FilterFramework1FilterDefinitionFileContent
		{
			get
			{
				return _FilterFramework1FilterDefinitionFileContent;
			}
			set
			{
				_FilterFramework1FilterDefinitionFileContent = value;
			}
		}
	}
}
