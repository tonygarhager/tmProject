using Newtonsoft.Json;
using Sdl.Core.Bcm.BcmModel.Common;
using System;
using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmModel
{
	public class RevisionContainer : MarkupDataContainer
	{
		[DataMember(Name = "revisionType")]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public RevisionType RevisionType
		{
			get;
			set;
		}

		[DataMember(Name = "author")]
		public string Author
		{
			get;
			set;
		}

		[DataMember(Name = "timestamp")]
		public DateTime? Timestamp
		{
			get;
			set;
		}

		[JsonProperty("type")]
		protected override string Type
		{
			get
			{
				return "revision";
			}
			set
			{
			}
		}

		public RevisionContainer()
		{
		}

		public RevisionContainer(RevisionType revisionType)
		{
			base.Id = Guid.NewGuid().ToString();
			RevisionType = revisionType;
		}

		public override void AcceptVisitor(BcmVisitor visitor)
		{
			visitor.VisitRevisionContainer(this);
		}

		public override bool Equals(MarkupData other)
		{
			if (other == null)
			{
				return false;
			}
			if (this == other)
			{
				return true;
			}
			RevisionContainer revisionContainer = other as RevisionContainer;
			if (revisionContainer == null)
			{
				return false;
			}
			if (base.Equals(revisionContainer) && RevisionType == revisionContainer.RevisionType && string.Equals(Author, revisionContainer.Author))
			{
				return Timestamp.Equals(revisionContainer.Timestamp);
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (this == obj)
			{
				return true;
			}
			if (obj.GetType() != GetType())
			{
				return false;
			}
			return Equals((MarkupData)obj);
		}

		public override int GetHashCode()
		{
			int hashCode = base.GetHashCode();
			hashCode = ((hashCode * 397) ^ (int)RevisionType);
			hashCode = ((hashCode * 397) ^ ((Author != null) ? Author.GetHashCode() : 0));
			return (hashCode * 397) ^ Timestamp.GetHashCode();
		}

		public new RevisionContainer Clone()
		{
			return base.Clone() as RevisionContainer;
		}

		public new RevisionContainer UniqueClone()
		{
			return base.UniqueClone() as RevisionContainer;
		}
	}
}
