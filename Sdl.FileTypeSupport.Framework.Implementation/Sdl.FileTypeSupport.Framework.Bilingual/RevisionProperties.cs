using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Native;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Xml.Serialization;

namespace Sdl.FileTypeSupport.Framework.Bilingual
{
	[Serializable]
	public class RevisionProperties : AbstractMetaDataContainer, IRevisionProperties, IMetaDataContainer, ICloneable, ISupportsPersistenceId
	{
		private RevisionType _RevisionType = RevisionType.Unchanged;

		private DateTime? _Date;

		private string _Author;

		[NonSerialized]
		private int _persistenceId;

		public RevisionType RevisionType
		{
			get
			{
				return _RevisionType;
			}
			set
			{
				if (value == RevisionType.FeedbackComment || value == RevisionType.FeedbackAdded || value == RevisionType.FeedbackDeleted)
				{
					throw new RevisionTypeNotSupportedException("RevisionProperties do not support Feedback types");
				}
				_RevisionType = value;
			}
		}

		public DateTime? Date
		{
			get
			{
				return _Date;
			}
			set
			{
				_Date = value;
			}
		}

		public string Author
		{
			get
			{
				return _Author;
			}
			set
			{
				_Author = value;
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

		public RevisionProperties()
		{
		}

		protected RevisionProperties(RevisionProperties other)
		{
			_RevisionType = other._RevisionType;
			_Date = other._Date;
			_Author = other._Author;
			ReplaceMetaDataWithCloneOf(other._MetaData);
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			RevisionProperties revisionProperties = (RevisionProperties)obj;
			if (!base.Equals((object)revisionProperties))
			{
				return false;
			}
			if (revisionProperties._Author != _Author)
			{
				return false;
			}
			if (revisionProperties._Date != _Date)
			{
				return false;
			}
			if (revisionProperties._RevisionType != _RevisionType)
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			if (_Author == null)
			{
				return 0 ^ (_Date.HasValue ? _Date.GetHashCode() : 0) ^ _RevisionType.GetHashCode();
			}
			return base.GetHashCode() ^ _Author.GetHashCode();
		}

		public object Clone()
		{
			return new RevisionProperties(this);
		}
	}
}
