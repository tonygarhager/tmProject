using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.FileTypeSupport.Framework.BilingualApi
{
	public interface IAbstractMarkupData : ICloneable, ISupportsUniqueId
	{
		IAbstractMarkupDataContainer Parent
		{
			get;
			set;
		}

		int IndexInParent
		{
			get;
		}

		IParagraph ParentParagraph
		{
			get;
		}

		void AcceptVisitor(IMarkupDataVisitor visitor);

		void RemoveFromParent();
	}
}
