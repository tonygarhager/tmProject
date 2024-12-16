using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.Integration
{
	public class SharedObjects : ISharedObjects
	{
		private Dictionary<string, object> _sharedObjects = new Dictionary<string, object>();

		public IEnumerable<string> SharedObjectIds => _sharedObjects.Keys;

		IEnumerable<object> ISharedObjects.SharedObjects => _sharedObjects.Values;

		public IEnumerable<KeyValuePair<string, object>> SharedObjectsWithIds => _sharedObjects;

		public event EventHandler<SharedObjectPublishedEventArgs> SharedObjectPublished;

		public virtual void OnSharedObjectPublished(object sender, SharedObjectPublishedEventArgs args)
		{
			if (this.SharedObjectPublished != null)
			{
				this.SharedObjectPublished(sender, args);
			}
		}

		public T GetSharedObject<T>(string id)
		{
			if (!_sharedObjects.TryGetValue(id, out object value))
			{
				return default(T);
			}
			return (T)value;
		}

		public void PublishSharedObject(string id, object toBeShared, IdConflictResolution conflictingIdResolution)
		{
			if (_sharedObjects.ContainsKey(id) && conflictingIdResolution == IdConflictResolution.ThrowException)
			{
				ConflictingIdException ex = new ConflictingIdException();
				ex.Id = id;
				throw ex;
			}
			_sharedObjects[id] = toBeShared;
			OnSharedObjectPublished(this, new SharedObjectPublishedEventArgs(id, toBeShared));
		}
	}
}
