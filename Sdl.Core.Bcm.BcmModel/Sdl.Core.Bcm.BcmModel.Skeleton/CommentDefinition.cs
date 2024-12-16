using Newtonsoft.Json;
using Sdl.Core.Bcm.BcmModel.Common;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmModel.Skeleton
{
	[DataContract]
	public class CommentDefinition : SkeletonItem, ICloneable<CommentDefinition>, IEquatable<CommentDefinition>
	{
		[DataMember(Name = "text")]
		public string Text
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

		[DataMember(Name = "date", EmitDefaultValue = false)]
		public DateTime Date
		{
			get;
			set;
		}

		[DataMember(Name = "commentSeverity")]
		[DefaultValue(CommentSeverity.Medium)]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public CommentSeverity CommentSeverity
		{
			get;
			set;
		}

		public CommentDefinition()
		{
			CommentSeverity = CommentSeverity.Medium;
		}

		public CommentDefinition Clone()
		{
			return (CommentDefinition)MemberwiseClone();
		}

		public bool Equals(CommentDefinition other)
		{
			if (other == null)
			{
				return false;
			}
			if (this == other)
			{
				return true;
			}
			if (Equals((SkeletonItem)other) && string.Equals(Text, other.Text) && string.Equals(Author, other.Author) && Date.Equals(other.Date))
			{
				return CommentSeverity == other.CommentSeverity;
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
			return Equals((CommentDefinition)obj);
		}

		public override int GetHashCode()
		{
			int hashCode = base.GetHashCode();
			hashCode = ((hashCode * 397) ^ ((Text != null) ? Text.GetHashCode() : 0));
			hashCode = ((hashCode * 397) ^ ((Author != null) ? Author.GetHashCode() : 0));
			hashCode = ((hashCode * 397) ^ Date.GetHashCode());
			return (hashCode * 397) ^ (int)CommentSeverity;
		}
	}
}
