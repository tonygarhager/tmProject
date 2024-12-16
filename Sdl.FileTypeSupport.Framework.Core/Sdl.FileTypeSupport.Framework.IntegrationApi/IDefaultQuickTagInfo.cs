using Sdl.FileTypeSupport.Framework.Formatting;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public interface IDefaultQuickTagInfo
	{
		QuickTagDefaultId DefaultId
		{
			get;
		}

		string CommandID
		{
			get;
		}

		string CommandName
		{
			get;
		}

		string ImageResource
		{
			get;
		}

		string ImagePath
		{
			get;
		}

		string Description
		{
			get;
		}

		IFormattingGroup Formatting
		{
			get;
		}
	}
}
