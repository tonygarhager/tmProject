using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Xml.Serialization;

namespace Sdl.FileTypeSupport.Framework.Native
{
	[Serializable]
	public class SubSegmentProperties : ISubSegmentProperties, ICloneable, ISupportsPersistenceId
	{
		private int _StartOffset;

		private int _Length;

		[NonSerialized]
		private int _persistenceId;

		public int StartOffset
		{
			get
			{
				return _StartOffset;
			}
			set
			{
				_StartOffset = value;
			}
		}

		public int Length
		{
			get
			{
				return _Length;
			}
			set
			{
				_Length = value;
			}
		}

		public IContextProperties Contexts
		{
			get;
			set;
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

		public SubSegmentProperties()
		{
		}

		protected SubSegmentProperties(SubSegmentProperties other)
		{
			_StartOffset = other._StartOffset;
			_Length = other._Length;
			if (other.Contexts != null)
			{
				Contexts = (IContextProperties)other.Contexts.Clone();
			}
		}

		public virtual object Clone()
		{
			return new SubSegmentProperties(this);
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			SubSegmentProperties subSegmentProperties = (SubSegmentProperties)obj;
			if (subSegmentProperties._Length != _Length)
			{
				return false;
			}
			if (subSegmentProperties._StartOffset != _StartOffset)
			{
				return false;
			}
			if (subSegmentProperties.Contexts == null != (Contexts == null))
			{
				return false;
			}
			if (Contexts != null && !Contexts.Equals(subSegmentProperties.Contexts))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return _StartOffset.GetHashCode() ^ _Length.GetHashCode() ^ ((Contexts != null) ? Contexts.GetHashCode() : 0);
		}
	}
}
