using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Xml.Serialization;

namespace Sdl.FileTypeSupport.Framework.Native
{
	[Serializable]
	public class LockedContentProperties : ILockedContentProperties, ICloneable, ISupportsPersistenceId
	{
		private LockTypeFlags _LockType;

		[NonSerialized]
		private int _persistenceId;

		public LockTypeFlags LockType
		{
			get
			{
				return _LockType;
			}
			set
			{
				_LockType = value;
			}
		}

		[XmlIgnore]
		public int PersistenceId
		{
			get
			{
				return _persistenceId;
			}
			set
			{
				_persistenceId = value;
			}
		}

		public LockedContentProperties()
		{
			_LockType = LockTypeFlags.Manual;
		}

		protected LockedContentProperties(LockedContentProperties other)
		{
			_LockType = other._LockType;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			LockedContentProperties lockedContentProperties = (LockedContentProperties)obj;
			if (LockType != lockedContentProperties.LockType)
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return LockType.ToString().GetHashCode();
		}

		public object Clone()
		{
			return new LockedContentProperties(this);
		}
	}
}
