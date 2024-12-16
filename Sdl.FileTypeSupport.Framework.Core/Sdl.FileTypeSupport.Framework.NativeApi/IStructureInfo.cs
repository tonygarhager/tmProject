using System;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public interface IStructureInfo : ICloneable
	{
		IContextInfo ContextInfo
		{
			get;
			set;
		}

		bool MustUseDisplayName
		{
			get;
			set;
		}

		IStructureInfo ParentStructure
		{
			get;
			set;
		}

		string Id
		{
			get;
		}
	}
}
