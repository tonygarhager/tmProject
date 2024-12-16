using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Native;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Xml.Serialization;

namespace Sdl.FileTypeSupport.Framework.Bilingual
{
	[Serializable]
	public class FeedbackProperties : AbstractMetaDataContainer, IRevisionProperties, IMetaDataContainer, ICloneable, ISupportsPersistenceId
	{
		private RevisionType _RevisionType = RevisionType.Unchanged;

		private DateTime? _Date;

		private string _Author;

		private IComment _FeedbackComment;

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
				if (value != RevisionType.FeedbackComment && value != RevisionType.FeedbackAdded && value != RevisionType.FeedbackDeleted)
				{
					throw new RevisionTypeNotSupportedException("Unsupported Feedback type");
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

		public IComment FeedbackComment
		{
			get
			{
				return _FeedbackComment;
			}
			set
			{
				_FeedbackComment = value;
			}
		}

		public string DocumentCategory
		{
			get;
			set;
		}

		public string FeedbackCategory
		{
			get;
			set;
		}

		public string FeedbackSeverity
		{
			get;
			set;
		}

		public string ReplacementId
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

		public FeedbackProperties()
		{
		}

		protected FeedbackProperties(FeedbackProperties other)
		{
			_RevisionType = other._RevisionType;
			_Date = other._Date;
			_Author = other._Author;
			ReplaceMetaDataWithCloneOf(other._MetaData);
			if (other.FeedbackComment != null)
			{
				_FeedbackComment = (IComment)other._FeedbackComment.Clone();
			}
			DocumentCategory = other.DocumentCategory;
			FeedbackCategory = other.FeedbackCategory;
			FeedbackSeverity = other.FeedbackSeverity;
			ReplacementId = other.ReplacementId;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			FeedbackProperties feedbackProperties = (FeedbackProperties)obj;
			if (!base.Equals((object)feedbackProperties))
			{
				return false;
			}
			if (feedbackProperties._Author != _Author)
			{
				return false;
			}
			if (feedbackProperties._Date != _Date)
			{
				return false;
			}
			if (feedbackProperties._RevisionType != _RevisionType)
			{
				return false;
			}
			if (feedbackProperties.DocumentCategory != DocumentCategory)
			{
				return false;
			}
			if (feedbackProperties.FeedbackCategory != FeedbackCategory)
			{
				return false;
			}
			if (feedbackProperties.FeedbackSeverity != FeedbackSeverity)
			{
				return false;
			}
			if (FeedbackComment != null && feedbackProperties.FeedbackComment == null)
			{
				return false;
			}
			if (FeedbackComment == null && feedbackProperties.FeedbackComment != null)
			{
				return false;
			}
			if (FeedbackComment != null && feedbackProperties.FeedbackComment != null && !feedbackProperties.FeedbackComment.Equals(FeedbackComment))
			{
				return false;
			}
			if (ReplacementId == null && feedbackProperties.ReplacementId != null)
			{
				return false;
			}
			if (ReplacementId != null && feedbackProperties.ReplacementId != null && !feedbackProperties.ReplacementId.Equals(ReplacementId))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			if (_Author == null)
			{
				return 0 ^ (_Date.HasValue ? _Date.GetHashCode() : 0) ^ _RevisionType.GetHashCode() ^ ((DocumentCategory != null) ? DocumentCategory.GetHashCode() : 0) ^ ((FeedbackCategory != null) ? FeedbackCategory.GetHashCode() : 0) ^ ((FeedbackSeverity != null) ? FeedbackSeverity.GetHashCode() : 0) ^ ((FeedbackComment != null) ? FeedbackComment.GetHashCode() : 0) ^ ((ReplacementId != null) ? ReplacementId.GetHashCode() : 0);
			}
			return base.GetHashCode() ^ _Author.GetHashCode();
		}

		public object Clone()
		{
			return new FeedbackProperties(this);
		}
	}
}
