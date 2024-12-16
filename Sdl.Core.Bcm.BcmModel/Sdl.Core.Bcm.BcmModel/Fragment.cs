using Sdl.Core.Bcm.BcmModel.Common;
using Sdl.Core.Bcm.BcmModel.Skeleton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Sdl.Core.Bcm.BcmModel
{
	[DataContract]
	public class Fragment : MetadataContainer, IEquatable<Fragment>, ICloneable<Fragment>
	{
		private MarkupData _sourceContent;

		private MarkupData _targetContent;

		[DataMember(Name = "sourceLanguageCode", EmitDefaultValue = false)]
		public string SourceLanguageCode
		{
			get;
			set;
		}

		[DataMember(Name = "targetLanguageCode", EmitDefaultValue = false)]
		public string TargetLanguageCode
		{
			get;
			set;
		}

		[DataMember(Name = "documentId", EmitDefaultValue = false)]
		public string DocumentId
		{
			get;
			set;
		}

		[DataMember(Name = "skeleton", EmitDefaultValue = false)]
		public FileSkeleton Skeleton
		{
			get;
			set;
		}

		[DataMember(Name = "sourceContent", EmitDefaultValue = false)]
		public MarkupData SourceContent
		{
			get
			{
				return _sourceContent;
			}
			set
			{
				_sourceContent = value;
				if (_sourceContent != null)
				{
					_sourceContent.ParentFragment = this;
				}
			}
		}

		[DataMember(Name = "targetContent", EmitDefaultValue = false)]
		public MarkupData TargetContent
		{
			get
			{
				return _targetContent;
			}
			set
			{
				_targetContent = value;
				if (_targetContent != null)
				{
					_targetContent.ParentFragment = this;
				}
			}
		}

		[DataMember(Name = "index", EmitDefaultValue = false)]
		public int? Index
		{
			get;
			set;
		}

		[DataMember(Name = "structureContextId", EmitDefaultValue = false)]
		public int StructureContextId
		{
			get;
			set;
		}

		[DataMember(Name = "contextList", EmitDefaultValue = false)]
		public IList<int> ContextList
		{
			get;
			set;
		}

		[DataMember(Name = "commentDefinitionIds", EmitDefaultValue = false)]
		public List<int> CommentDefinitionIds
		{
			get;
			set;
		}

		public Fragment()
		{
			ContextList = new List<int>();
			CommentDefinitionIds = new List<int>();
		}

		public bool ShouldSerializeSkeleton()
		{
			if (Skeleton != null)
			{
				if ((Skeleton.ContextDefinitions == null || !Skeleton.ContextDefinitions.Any()) && (Skeleton.Contexts == null || !Skeleton.Contexts.Any()) && (Skeleton.TagPairDefinitions == null || !Skeleton.TagPairDefinitions.Any()) && (Skeleton.PlaceholderTagDefinitions == null || !Skeleton.PlaceholderTagDefinitions.Any()) && (Skeleton.StructureTagDefinitions == null || !Skeleton.StructureTagDefinitions.Any()) && (Skeleton.CommentDefinitions == null || !Skeleton.CommentDefinitions.Any()) && (Skeleton.FormattingGroups == null || !Skeleton.FormattingGroups.Any()) && (Skeleton.SubContentPUs == null || !Skeleton.SubContentPUs.Any()))
				{
					if (Skeleton.TerminologyData != null)
					{
						return Skeleton.TerminologyData.Any();
					}
					return false;
				}
				return true;
			}
			return false;
		}

		public bool ShouldSerializeContextList()
		{
			if (ContextList != null)
			{
				return ContextList.Any();
			}
			return false;
		}

		public bool ShouldSerializeCommentDefinitionIds()
		{
			if (CommentDefinitionIds != null)
			{
				return CommentDefinitionIds.Any();
			}
			return false;
		}

		public Fragment Clone()
		{
			Fragment fragment = (Fragment)MemberwiseClone();
			fragment.ReplaceMetadataWith(base.Metadata);
			if (Skeleton != null)
			{
				fragment.Skeleton = Skeleton.Clone();
			}
			if (SourceContent != null)
			{
				fragment.SourceContent = SourceContent.Clone();
			}
			if (TargetContent != null)
			{
				fragment.TargetContent = TargetContent.Clone();
			}
			if (ContextList != null)
			{
				fragment.ContextList = ContextList.ToList();
			}
			if (CommentDefinitionIds != null)
			{
				fragment.CommentDefinitionIds = CommentDefinitionIds.ToList();
			}
			return fragment;
		}

		public bool Equals(Fragment other)
		{
			if (other == null)
			{
				return false;
			}
			if (this == other)
			{
				return true;
			}
			if (Equals((MetadataContainer)other) && string.Equals(DocumentId, other.DocumentId) && string.Equals(SourceLanguageCode, other.SourceLanguageCode) && string.Equals(TargetLanguageCode, other.TargetLanguageCode) && object.Equals(Skeleton, other.Skeleton) && object.Equals(Index, other.Index) && object.Equals(SourceContent, other.SourceContent) && object.Equals(TargetContent, other.TargetContent) && StructureContextId == other.StructureContextId && ContextList.IsSequenceEqual(other.ContextList))
			{
				return CommentDefinitionIds.IsSequenceEqual(other.CommentDefinitionIds);
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
			return Equals((Fragment)obj);
		}

		public override int GetHashCode()
		{
			int hashCode = base.GetHashCode();
			hashCode = ((hashCode * 397) ^ (DocumentId?.GetHashCode() ?? 0));
			hashCode = ((hashCode * 397) ^ (SourceLanguageCode?.GetHashCode() ?? 0));
			hashCode = ((hashCode * 397) ^ (TargetLanguageCode?.GetHashCode() ?? 0));
			hashCode = ((hashCode * 397) ^ (Skeleton?.GetHashCode() ?? 0));
			hashCode = ((hashCode * 397) ^ (SourceContent?.GetHashCode() ?? 0));
			hashCode = ((hashCode * 397) ^ (TargetContent?.GetHashCode() ?? 0));
			hashCode = ((hashCode * 397) ^ (Index.HasValue ? Index.GetHashCode() : 0));
			hashCode = ((hashCode * 397) ^ StructureContextId);
			hashCode = ((hashCode * 397) ^ ((ContextList != null) ? ContextList.GetHashCode() : 0));
			return (hashCode * 397) ^ ((CommentDefinitionIds != null) ? CommentDefinitionIds.GetHashCode() : 0);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("<fragment slc=\"{0}\" tlc=\"{1}\" docId = \"{2}\" index = \"{3}\">", SourceLanguageCode, TargetLanguageCode, DocumentId, Index);
			stringBuilder.Append("<source>");
			stringBuilder.Append(SourceContent);
			stringBuilder.Append("</source>");
			stringBuilder.Append("<target>");
			stringBuilder.Append(TargetContent);
			stringBuilder.Append("</target>");
			stringBuilder.Append(TargetContent);
			stringBuilder.Append("</fragment>");
			return stringBuilder.ToString();
		}
	}
}
