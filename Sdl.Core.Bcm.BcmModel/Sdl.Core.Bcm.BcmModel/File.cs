using Newtonsoft.Json;
using Sdl.Core.Bcm.BcmModel.Collections;
using Sdl.Core.Bcm.BcmModel.Common;
using Sdl.Core.Bcm.BcmModel.Skeleton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmModel
{
	[DataContract]
	public class File : MetadataContainer, ICloneable<File>, IEquatable<File>
	{
		[JsonProperty(PropertyName = "id", Order = int.MinValue)]
		public string Id
		{
			get;
			set;
		}

		[DataMember(Name = "originalEncoding", EmitDefaultValue = false)]
		public string OriginalEncoding
		{
			get;
			set;
		}

		[DataMember(Name = "preferredTargetEncoding", EmitDefaultValue = false)]
		public string PreferredTargetEncoding
		{
			get;
			set;
		}

		[DataMember(Name = "originalFileName", EmitDefaultValue = false)]
		public string OriginalFileName
		{
			get;
			set;
		}

		[DataMember(Name = "fileTypeDefinitionId", EmitDefaultValue = false)]
		public string FileTypeDefinitionId
		{
			get;
			set;
		}

		[DataMember(Name = "dependencyFiles", EmitDefaultValue = false)]
		public List<DependencyFile> DependencyFiles
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

		[DataMember(Name = "skeleton", EmitDefaultValue = false)]
		public FileSkeleton Skeleton
		{
			get;
			set;
		}

		[DataMember(Name = "paragraphUnits", EmitDefaultValue = false)]
		public ParagraphUnitCollection ParagraphUnits
		{
			get;
			set;
		}

		public Document Parent
		{
			get;
			set;
		}

		public IdGenerator IdGenerator
		{
			get;
			set;
		}

		public File()
		{
			Id = Guid.NewGuid().ToString();
			ParagraphUnits = new ParagraphUnitCollection
			{
				ParentFile = this
			};
			Skeleton = new FileSkeleton
			{
				FileId = Id,
				ParentFile = this
			};
			IdGenerator = new IdGenerator();
			DependencyFiles = new List<DependencyFile>();
			CommentDefinitionIds = new List<int>();
		}

		public File Clone()
		{
			File result = (File)MemberwiseClone();
			result.ReplaceMetadataWith(base.Metadata);
			if (DependencyFiles != null)
			{
				result.DependencyFiles = new List<DependencyFile>();
				DependencyFiles.ForEach(delegate(DependencyFile dep)
				{
					result.DependencyFiles.Add(dep.Clone());
				});
			}
			if (CommentDefinitionIds != null)
			{
				result.CommentDefinitionIds = CommentDefinitionIds.ToList();
			}
			if (Skeleton != null)
			{
				result.Skeleton = Skeleton.Clone();
			}
			if (ParagraphUnits != null)
			{
				result.ParagraphUnits = new ParagraphUnitCollection();
				ParagraphUnits.ForEach(delegate(ParagraphUnit pu)
				{
					result.ParagraphUnits.Add(pu.Clone());
				});
			}
			return result;
		}

		[OnDeserialized]
		internal void OnDeserializedMethod(StreamingContext context)
		{
			foreach (ParagraphUnit paragraphUnit in ParagraphUnits)
			{
				paragraphUnit.ParentFile = this;
				paragraphUnit.ParentFileId = Id;
			}
		}

		public bool Equals(File other)
		{
			if (other == null)
			{
				return false;
			}
			if (this == other)
			{
				return true;
			}
			if (Equals((MetadataContainer)other) && string.Equals(Id, other.Id) && string.Equals(OriginalEncoding, other.OriginalEncoding) && string.Equals(PreferredTargetEncoding, other.PreferredTargetEncoding) && string.Equals(OriginalFileName, other.OriginalFileName) && string.Equals(FileTypeDefinitionId, other.FileTypeDefinitionId) && DependencyFiles.IsSequenceEqual(other.DependencyFiles) && CommentDefinitionIds.IsSequenceEqual(other.CommentDefinitionIds) && object.Equals(Skeleton, other.Skeleton))
			{
				return ParagraphUnits.Equals(other.ParagraphUnits);
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
			return Equals((File)obj);
		}

		public override int GetHashCode()
		{
			int hashCode = base.GetHashCode();
			hashCode = ((hashCode * 397) ^ ((Id != null) ? Id.GetHashCode() : 0));
			hashCode = ((hashCode * 397) ^ ((OriginalEncoding != null) ? OriginalEncoding.GetHashCode() : 0));
			hashCode = ((hashCode * 397) ^ ((PreferredTargetEncoding != null) ? PreferredTargetEncoding.GetHashCode() : 0));
			hashCode = ((hashCode * 397) ^ ((OriginalFileName != null) ? OriginalFileName.GetHashCode() : 0));
			hashCode = ((hashCode * 397) ^ ((FileTypeDefinitionId != null) ? FileTypeDefinitionId.GetHashCode() : 0));
			hashCode = ((hashCode * 397) ^ ((DependencyFiles != null) ? DependencyFiles.GetHashCode() : 0));
			hashCode = ((hashCode * 397) ^ ((CommentDefinitionIds != null) ? CommentDefinitionIds.GetHashCode() : 0));
			hashCode = ((hashCode * 397) ^ ((Skeleton != null) ? Skeleton.GetHashCode() : 0));
			return (hashCode * 397) ^ ((ParagraphUnits != null) ? ParagraphUnits.GetHashCode() : 0);
		}

		public bool ShouldSerializeCommentDefinitionIds()
		{
			if (CommentDefinitionIds != null)
			{
				return CommentDefinitionIds.Any();
			}
			return false;
		}

		public bool ShouldSerializeDependencyFiles()
		{
			if (DependencyFiles != null)
			{
				return DependencyFiles.Any();
			}
			return false;
		}
	}
}
