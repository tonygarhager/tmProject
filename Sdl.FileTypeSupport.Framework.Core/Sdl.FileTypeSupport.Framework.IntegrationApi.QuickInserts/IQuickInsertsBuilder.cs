using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi.QuickInserts
{
	public interface IQuickInsertsBuilder
	{
		List<QuickInsertIds> BuildQuickInsertIdList();
	}
}
