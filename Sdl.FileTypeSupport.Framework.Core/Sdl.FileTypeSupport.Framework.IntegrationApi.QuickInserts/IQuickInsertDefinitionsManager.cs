using Sdl.FileTypeSupport.Framework.BilingualApi;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi.QuickInserts
{
	public interface IQuickInsertDefinitionsManager
	{
		IQuickInsert BuildQuickInsert(QuickInsertIds id);

		IQuickInsert BuildClonedQuickInsert(QuickInsertIds id);

		bool IsQuickInsert(IAbstractMarkupData item);

		List<QuickInsertIds> ParseQuickInsertIds(string quickInsertIds);

		bool TryParseQuickInsertId(string quickInsertIdString, out QuickInsertIds quickInsertId);
	}
}
