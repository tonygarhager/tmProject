using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Collections;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.BilingualApi
{
	public interface ISerializableMarkupDataContainer : IAbstractMarkupDataContainer, IList<IAbstractMarkupData>, ICollection<IAbstractMarkupData>, IEnumerable<IAbstractMarkupData>, IEnumerable, ISupportsUniqueId
	{
		IDocumentProperties DocProperties
		{
			get;
			set;
		}

		IFileProperties FileProperties
		{
			get;
			set;
		}
	}
}
