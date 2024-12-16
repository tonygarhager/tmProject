using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Formatting;
using System;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi.QuickInserts
{
	public interface IQuickInsert : ICloneable
	{
		QuickInsertIds Id
		{
			get;
		}

		string Name
		{
			get;
		}

		string Description
		{
			get;
		}

		IAbstractMarkupDataContainer MarkupData
		{
			get;
		}

		IFormattingGroup Formatting
		{
			get;
		}

		bool DisplayOnToolBar
		{
			get;
		}
	}
}
