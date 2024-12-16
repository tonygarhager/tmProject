using Sdl.Core.Globalization;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public interface IQuickTag
	{
		IQuickTagContent MarkupDataContent
		{
			get;
		}

		string CommandId
		{
			get;
			set;
		}

		LocalizableString CommandName
		{
			get;
			set;
		}

		LocalizableString Description
		{
			get;
			set;
		}

		IconDescriptor Icon
		{
			get;
			set;
		}

		bool DisplayOnToolBar
		{
			get;
			set;
		}

		bool IsDefaultQuickTag
		{
			get;
		}
	}
}
