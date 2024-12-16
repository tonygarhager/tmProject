using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.BilingualApi
{
	public interface IAbstractTag : IAbstractDataContent, IAbstractMarkupData, ICloneable, ISupportsUniqueId
	{
		IEnumerable<ISubSegmentReference> SubSegments
		{
			get;
		}

		IAbstractTagProperties TagProperties
		{
			get;
		}

		bool HasSubSegmentReferences
		{
			get;
		}

		void AddSubSegmentReference(ISubSegmentReference subSegmentReference);

		void AddSubSegmentReferences(IEnumerable<ISubSegmentReference> subSegmentReferences);

		void RemoveSubSegmentReference(ISubSegmentReference subSegmentReference);

		void ClearSubSegmentReferences();
	}
}
