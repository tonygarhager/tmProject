using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.FileTypeSupport.Framework.Native
{
	internal class MergeableObject
	{
		private IStartTagProperties _StartTagProperties;

		private ILockedContentProperties _LockedContentProperties;

		private ParagraphUnitId _paragraphUnitId;

		public IStartTagProperties StartTagProperties => _StartTagProperties;

		public ParagraphUnitId ParagraphUnitId => _paragraphUnitId;

		public ILockedContentProperties LockedContentProperties => _LockedContentProperties;

		public MergeableObject(IStartTagProperties startTagProperties, ParagraphUnitId paragraphUnitId)
		{
			_StartTagProperties = startTagProperties;
			_paragraphUnitId = paragraphUnitId;
		}

		public MergeableObject(ILockedContentProperties lockedContentProperties)
		{
			_LockedContentProperties = lockedContentProperties;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			MergeableObject mergeableObject = obj as MergeableObject;
			if (mergeableObject != null)
			{
				if (_StartTagProperties != null)
				{
					return _StartTagProperties.Equals(mergeableObject._StartTagProperties);
				}
				if (_LockedContentProperties != null)
				{
					return _LockedContentProperties.Equals(mergeableObject._LockedContentProperties);
				}
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ ((_StartTagProperties != null) ? _StartTagProperties.GetHashCode() : 0) ^ ((_LockedContentProperties != null) ? _LockedContentProperties.GetHashCode() : 0);
		}
	}
}
