using System.Collections;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public interface IQuickTags : ICollection<IQuickTag>, IEnumerable<IQuickTag>, IEnumerable
	{
		IQuickTag this[string commandId]
		{
			get;
		}

		IEnumerable<IQuickTag> AllDisplayItems
		{
			get;
		}

		IEnumerable<IQuickTag> AllDefaultItems
		{
			get;
		}

		IEnumerable<IQuickTag> AllNonDefaultItems
		{
			get;
		}

		void SetStandardQuickTags(IList<IQuickTag> standardQuickTags);
	}
}
