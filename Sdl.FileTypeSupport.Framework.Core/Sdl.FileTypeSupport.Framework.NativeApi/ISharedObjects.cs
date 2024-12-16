using System;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public interface ISharedObjects
	{
		IEnumerable<object> SharedObjects
		{
			get;
		}

		IEnumerable<string> SharedObjectIds
		{
			get;
		}

		IEnumerable<KeyValuePair<string, object>> SharedObjectsWithIds
		{
			get;
		}

		event EventHandler<SharedObjectPublishedEventArgs> SharedObjectPublished;

		T GetSharedObject<T>(string id);

		void PublishSharedObject(string id, object toBeShared, IdConflictResolution conflictingIdResolution);
	}
}
