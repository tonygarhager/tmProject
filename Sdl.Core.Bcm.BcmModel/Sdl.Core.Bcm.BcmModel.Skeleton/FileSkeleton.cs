using Sdl.Core.Bcm.BcmModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmModel.Skeleton
{
	[DataContract]
	public class FileSkeleton : MetadataContainer, ICloneable<FileSkeleton>, IEquatable<FileSkeleton>
	{
		private IdGenerator _idGenerator;

		[DataMember(Name = "fileId", EmitDefaultValue = false)]
		public string FileId
		{
			get;
			set;
		}

		[DataMember(Name = "commentDefinitions")]
		public SkeletonCollection<CommentDefinition> CommentDefinitions
		{
			get;
			set;
		}

		[DataMember(Name = "contextDefinitions")]
		public SkeletonCollection<ContextDefinition> ContextDefinitions
		{
			get;
			set;
		}

		[DataMember(Name = "contexts")]
		public SkeletonCollection<Context> Contexts
		{
			get;
			set;
		}

		[DataMember(Name = "formattingGroups")]
		public SkeletonCollection<FormattingGroup> FormattingGroups
		{
			get;
			set;
		}

		[DataMember(Name = "structureTagDefinitions")]
		public SkeletonCollection<StructureTagDefinition> StructureTagDefinitions
		{
			get;
			set;
		}

		[DataMember(Name = "tagPairDefinitions")]
		public SkeletonCollection<TagPairDefinition> TagPairDefinitions
		{
			get;
			set;
		}

		[DataMember(Name = "placeholderTagDefinitions")]
		public SkeletonCollection<PlaceholderTagDefinition> PlaceholderTagDefinitions
		{
			get;
			set;
		}

		[DataMember(Name = "terminologyData")]
		public SkeletonCollection<TerminologyData> TerminologyData
		{
			get;
			set;
		}

		[DataMember(Name = "quickInsertIds", EmitDefaultValue = false)]
		public List<string> QuickInsertIds
		{
			get;
			set;
		}

		public List<string> SubContentPUs
		{
			get;
			set;
		}

		public File ParentFile
		{
			get;
			set;
		}

		public IdGenerator IdGenerator
		{
			get
			{
				IdGenerator idGenerator = ParentFile?.IdGenerator;
				if (idGenerator == null)
				{
					if (_idGenerator == null)
					{
						_idGenerator = new IdGenerator();
						_idGenerator.UpdateFrom(this);
					}
					idGenerator = _idGenerator;
				}
				return idGenerator;
			}
		}

		public FileSkeleton()
		{
			CommentDefinitions = new SkeletonCollection<CommentDefinition>
			{
				ParentSkeleton = this
			};
			ContextDefinitions = new SkeletonCollection<ContextDefinition>
			{
				ParentSkeleton = this
			};
			Contexts = new SkeletonCollection<Context>
			{
				ParentSkeleton = this
			};
			FormattingGroups = new SkeletonCollection<FormattingGroup>
			{
				ParentSkeleton = this
			};
			StructureTagDefinitions = new SkeletonCollection<StructureTagDefinition>
			{
				ParentSkeleton = this
			};
			TagPairDefinitions = new SkeletonCollection<TagPairDefinition>
			{
				ParentSkeleton = this
			};
			PlaceholderTagDefinitions = new SkeletonCollection<PlaceholderTagDefinition>
			{
				ParentSkeleton = this
			};
			TerminologyData = new SkeletonCollection<TerminologyData>
			{
				ParentSkeleton = this
			};
			SubContentPUs = new List<string>();
			QuickInsertIds = new List<string>();
		}

		public FileSkeleton Clone()
		{
			FileSkeleton fileSkeleton = (FileSkeleton)MemberwiseClone();
			fileSkeleton.AddMetadataFrom(base.Metadata);
			if (CommentDefinitions != null)
			{
				fileSkeleton.CommentDefinitions = CloneCollection(CommentDefinitions);
			}
			if (ContextDefinitions != null)
			{
				fileSkeleton.ContextDefinitions = CloneCollection(ContextDefinitions);
			}
			if (Contexts != null)
			{
				fileSkeleton.Contexts = CloneCollection(Contexts);
			}
			if (FormattingGroups != null)
			{
				fileSkeleton.FormattingGroups = CloneCollection(FormattingGroups);
			}
			if (StructureTagDefinitions != null)
			{
				fileSkeleton.StructureTagDefinitions = CloneCollection(StructureTagDefinitions);
			}
			if (TagPairDefinitions != null)
			{
				fileSkeleton.TagPairDefinitions = CloneCollection(TagPairDefinitions);
			}
			if (PlaceholderTagDefinitions != null)
			{
				fileSkeleton.PlaceholderTagDefinitions = CloneCollection(PlaceholderTagDefinitions);
			}
			if (TerminologyData != null)
			{
				fileSkeleton.TerminologyData = CloneCollection(TerminologyData);
			}
			if (SubContentPUs != null)
			{
				fileSkeleton.SubContentPUs = SubContentPUs.ToList();
			}
			if (QuickInsertIds != null)
			{
				fileSkeleton.QuickInsertIds = QuickInsertIds.ToList();
			}
			return fileSkeleton;
		}

		public bool Equals(FileSkeleton other)
		{
			if (other == null)
			{
				return false;
			}
			if (this == other)
			{
				return true;
			}
			if (Equals((MetadataContainer)other) && string.Equals(FileId, other.FileId) && CommentDefinitions.Equals(other.CommentDefinitions) && ContextDefinitions.Equals(other.ContextDefinitions) && Contexts.Equals(other.Contexts) && FormattingGroups.Equals(other.FormattingGroups) && StructureTagDefinitions.Equals(other.StructureTagDefinitions) && TagPairDefinitions.Equals(other.TagPairDefinitions) && PlaceholderTagDefinitions.Equals(other.PlaceholderTagDefinitions) && TerminologyData.Equals(other.TerminologyData) && SubContentPUs.IsSequenceEqual(other.SubContentPUs))
			{
				return QuickInsertIds.IsSequenceEqual(other.QuickInsertIds);
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
			return Equals((FileSkeleton)obj);
		}

		public override int GetHashCode()
		{
			int hashCode = base.GetHashCode();
			hashCode = ((hashCode * 397) ^ ((FileId != null) ? FileId.GetHashCode() : 0));
			hashCode = ((hashCode * 397) ^ ((CommentDefinitions != null) ? CommentDefinitions.GetHashCode() : 0));
			hashCode = ((hashCode * 397) ^ ((ContextDefinitions != null) ? ContextDefinitions.GetHashCode() : 0));
			hashCode = ((hashCode * 397) ^ ((Contexts != null) ? Contexts.GetHashCode() : 0));
			hashCode = ((hashCode * 397) ^ ((FormattingGroups != null) ? FormattingGroups.GetHashCode() : 0));
			hashCode = ((hashCode * 397) ^ ((StructureTagDefinitions != null) ? StructureTagDefinitions.GetHashCode() : 0));
			hashCode = ((hashCode * 397) ^ ((TagPairDefinitions != null) ? TagPairDefinitions.GetHashCode() : 0));
			hashCode = ((hashCode * 397) ^ ((PlaceholderTagDefinitions != null) ? PlaceholderTagDefinitions.GetHashCode() : 0));
			hashCode = ((hashCode * 397) ^ ((TerminologyData != null) ? TerminologyData.GetHashCode() : 0));
			hashCode = ((hashCode * 397) ^ ((SubContentPUs != null) ? SubContentPUs.GetHashCode() : 0));
			return (hashCode * 397) ^ ((QuickInsertIds != null) ? QuickInsertIds.GetHashCode() : 0);
		}

		private SkeletonCollection<T> CloneCollection<T>(IEnumerable<T> collection) where T : SkeletonItem, ICloneable<T>
		{
			SkeletonCollection<T> skeletonCollection = new SkeletonCollection<T>();
			foreach (T item in collection)
			{
				skeletonCollection.Add(item.Clone());
			}
			return skeletonCollection;
		}

		public bool ShouldSerializeCommentDefinitions()
		{
			if (CommentDefinitions != null)
			{
				return CommentDefinitions.Any();
			}
			return false;
		}

		public bool ShouldSerializeContextDefinitions()
		{
			if (ContextDefinitions != null)
			{
				return ContextDefinitions.Any();
			}
			return false;
		}

		public bool ShouldSerializeContexts()
		{
			if (Contexts != null)
			{
				return Contexts.Any();
			}
			return false;
		}

		public bool ShouldSerializeFormattingGroups()
		{
			if (FormattingGroups != null)
			{
				return FormattingGroups.Any();
			}
			return false;
		}

		public bool ShouldSerializeStructureTagDefinitions()
		{
			if (StructureTagDefinitions != null)
			{
				return StructureTagDefinitions.Any();
			}
			return false;
		}

		public bool ShouldSerializeTagPairDefinitions()
		{
			if (TagPairDefinitions != null)
			{
				return TagPairDefinitions.Any();
			}
			return false;
		}

		public bool ShouldSerializePlaceholderTagDefinitions()
		{
			if (PlaceholderTagDefinitions != null)
			{
				return PlaceholderTagDefinitions.Any();
			}
			return false;
		}

		public bool ShouldSerializeTerminologyData()
		{
			if (TerminologyData != null)
			{
				return TerminologyData.Any();
			}
			return false;
		}

		public bool ShouldSerializeSubContentPUs()
		{
			if (SubContentPUs != null)
			{
				return SubContentPUs.Any();
			}
			return false;
		}

		public bool ShouldSerializeQuickInsertIds()
		{
			if (QuickInsertIds != null)
			{
				return QuickInsertIds.Any();
			}
			return false;
		}
	}
}
