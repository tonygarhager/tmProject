using System;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public interface IAbstractTagProperties : IAbstractBasicTagProperties, IMetaDataContainer, ICloneable
	{
		IEnumerable<ISubSegmentProperties> LocalizableContent
		{
			get;
		}

		bool HasLocalizableContent
		{
			get;
		}

		TagId TagId
		{
			get;
			set;
		}

		void AddSubSegment(ISubSegmentProperties localizableContent);

		void AddSubSegments(IEnumerable<ISubSegmentProperties> localizableContent);

		void RemoveSubSegment(ISubSegmentProperties localizableContent);

		void ClearSubSegments();

		void SortSubSegments();
	}
}
