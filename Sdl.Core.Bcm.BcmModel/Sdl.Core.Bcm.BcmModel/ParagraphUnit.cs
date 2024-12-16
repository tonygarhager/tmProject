using Newtonsoft.Json;
using Sdl.Core.Bcm.BcmModel.Collections;
using Sdl.Core.Bcm.BcmModel.Common;
using Sdl.Core.Bcm.BcmModel.Operations;
using Sdl.Core.Bcm.BcmModel.Operations.Merge;
using Sdl.Core.Bcm.BcmModel.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmModel
{
	[DataContract]
	public class ParagraphUnit : MetadataContainer, ICloneable<ParagraphUnit>, IEquatable<ParagraphUnit>
	{
		private Paragraph _source;

		private Paragraph _target;

		[DataMember(Name = "id")]
		public string Id
		{
			get;
			set;
		}

		[DataMember(Name = "parentFileId")]
		public string ParentFileId
		{
			get;
			set;
		}

		[DataMember(Name = "isStructure", EmitDefaultValue = false)]
		[DefaultValue(false)]
		public bool IsStructure
		{
			get;
			set;
		}

		[DataMember(Name = "isLocked", EmitDefaultValue = false)]
		[DefaultValue(false)]
		public bool IsLocked
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

		[DataMember(Name = "index")]
		[JsonConverter(typeof(FloatToIntConverter))]
		public int Index
		{
			get;
			set;
		}

		[DataMember(Name = "source")]
		public Paragraph Source
		{
			get
			{
				return _source;
			}
			set
			{
				_source = value;
				_source.ParentParagraphUnit = this;
			}
		}

		[DataMember(Name = "target")]
		public Paragraph Target
		{
			get
			{
				return _target;
			}
			set
			{
				_target = value;
				_target.ParentParagraphUnit = this;
			}
		}

		[DataMember(Name = "commentDefinitionIds", EmitDefaultValue = false)]
		public List<int> CommentDefinitionIds
		{
			get;
			set;
		}

		public File ParentFile
		{
			get;
			set;
		}

		public SegmentPairCollection SegmentPairs
		{
			get
			{
				IEnumerable<SegmentPair> enumerable = from srcSegment in Source.AllSubItems.OfType<Segment>()
					join targetSegment in Target.AllSubItems.OfType<Segment>() on srcSegment.SegmentNumber equals targetSegment.SegmentNumber into targets
					from target in targets.DefaultIfEmpty()
					select new SegmentPair(srcSegment, target);
				return new SegmentPairCollection(enumerable);
			}
		}

		public ParagraphUnit()
		{
			Id = Guid.NewGuid().ToString();
			IsStructure = false;
			IsLocked = false;
		}

		public ParagraphUnit Clone()
		{
			ParagraphUnit paragraphUnit = (ParagraphUnit)MemberwiseClone();
			paragraphUnit.ReplaceMetadataWith(base.Metadata);
			if (ContextList != null)
			{
				paragraphUnit.ContextList = ContextList.ToList();
			}
			if (Source != null)
			{
				paragraphUnit.Source = Source.Clone();
			}
			if (Target != null)
			{
				paragraphUnit.Target = Target.Clone();
			}
			if (CommentDefinitionIds != null)
			{
				paragraphUnit.CommentDefinitionIds = CommentDefinitionIds.ToList();
			}
			return paragraphUnit;
		}

		public Segment SplitSegment(string segmentNumber, string locationId, int splitIndex = -1)
		{
			Segment segment = Source.AllSubItems.OfType<Segment>().SingleOrDefault((Segment s) => s.SegmentNumber == segmentNumber);
			if (segment == null)
			{
				throw new InvalidOperationException("Segment not found");
			}
			MarkupData markupData = segment.GetAllChildren((MarkupData md) => md.Id == locationId).SingleOrDefault();
			if (markupData == null)
			{
				throw new InvalidOperationException("Invalid split location");
			}
			SegmentSplitter segmentSplitter = new SegmentSplitter(segment, markupData, splitIndex);
			return segmentSplitter.Split();
		}

		public void MergeSegments(string firstSegmentNumber, string secondSegmentNumber)
		{
			SegmentPair segmentPair = SegmentPairs.SingleOrDefault((SegmentPair sp) => sp.Source.SegmentNumber == firstSegmentNumber);
			if (segmentPair == null)
			{
				throw new ArgumentException("Segment number not found", "firstSegmentNumber");
			}
			if (segmentPair.Source.IsLocked || segmentPair.Target.IsLocked)
			{
				throw new InvalidOperationException("Cannot merge locked segments");
			}
			SegmentPair segmentPair2 = SegmentPairs.SingleOrDefault((SegmentPair sp) => sp.Source.SegmentNumber == secondSegmentNumber);
			if (segmentPair2 == null)
			{
				throw new ArgumentException("Segment number not found", "secondSegmentNumber");
			}
			if (segmentPair2.Source.IsLocked || segmentPair2.Target.IsLocked)
			{
				throw new InvalidOperationException("Cannot merge locked segments");
			}
			ContainerMerger containerMerger = new ContainerMerger(segmentPair.Source, segmentPair2.Source);
			containerMerger.Merge();
			containerMerger = new ContainerMerger(segmentPair.Target, segmentPair2.Target);
			containerMerger.Merge();
		}

		public override string ToString()
		{
			return $"{Source} {Target}";
		}

		public bool Equals(ParagraphUnit other)
		{
			if (other == null)
			{
				return false;
			}
			if (this == other)
			{
				return true;
			}
			if (Equals((MetadataContainer)other) && string.Equals(Id, other.Id) && string.Equals(ParentFileId, other.ParentFileId) && IsStructure.Equals(other.IsStructure) && IsLocked.Equals(other.IsLocked) && StructureContextId == other.StructureContextId && ContextList.IsSequenceEqual(other.ContextList) && Index.Equals(other.Index) && object.Equals(Source, other.Source) && object.Equals(Target, other.Target))
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
			return Equals((ParagraphUnit)obj);
		}

		public override int GetHashCode()
		{
			int hashCode = base.GetHashCode();
			hashCode = ((hashCode * 397) ^ ((Id != null) ? Id.GetHashCode() : 0));
			hashCode = ((hashCode * 397) ^ ((ParentFileId != null) ? ParentFileId.GetHashCode() : 0));
			hashCode = ((hashCode * 397) ^ IsStructure.GetHashCode());
			hashCode = ((hashCode * 397) ^ IsLocked.GetHashCode());
			hashCode = ((hashCode * 397) ^ StructureContextId);
			hashCode = ((hashCode * 397) ^ ((ContextList != null) ? ContextList.GetHashCode() : 0));
			hashCode = ((hashCode * 397) ^ Index.GetHashCode());
			hashCode = ((hashCode * 397) ^ ((Source != null) ? Source.GetHashCode() : 0));
			hashCode = ((hashCode * 397) ^ ((Target != null) ? Target.GetHashCode() : 0));
			return (hashCode * 397) ^ ((CommentDefinitionIds != null) ? CommentDefinitionIds.GetHashCode() : 0);
		}
	}
}
