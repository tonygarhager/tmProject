using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.FileTypeSupport.Framework.BilingualApi
{
	public interface IRevisionProperties : IMetaDataContainer, ICloneable
	{
		RevisionType RevisionType
		{
			get;
			set;
		}

		DateTime? Date
		{
			get;
			set;
		}

		string Author
		{
			get;
			set;
		}
	}
}
