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
	public class Document : MetadataContainer, ICloneable<Document>, IEquatable<Document>
	{
		private string _modelVersion = BcmConst.CurrentBcmModelVersion;

		[DataMember(Name = "id")]
		public string Id
		{
			get;
			set;
		}

		[DataMember(Name = "modelVersion")]
		public string ModelVersion
		{
			get
			{
				return _modelVersion;
			}
			private set
			{
				_modelVersion = value;
			}
		}

		[DataMember(Name = "name")]
		public string Name
		{
			get;
			set;
		}

		[DataMember(Name = "sourceLanguageCode")]
		public string SourceLanguageCode
		{
			get;
			set;
		}

		[DataMember(Name = "targetLanguageCode")]
		public string TargetLanguageCode
		{
			get;
			set;
		}

		[Obsolete("Will be removed in a future version. Use the LanguageRegistry instead for retrieving language information.")]
		[DataMember(Name = "sourceLanguageName", EmitDefaultValue = false)]
		public string SourceLanguageName
		{
			get;
			set;
		}

		[Obsolete("Will be removed in a future version. Use the LanguageRegistry instead for retrieving language information.")]
		[DataMember(Name = "targetLanguageName", EmitDefaultValue = false)]
		public string TargetLanguageName
		{
			get;
			set;
		}

		[DataMember(Name = "files")]
		public FileCollection Files
		{
			get;
			set;
		}

		public IdGenerator IdGenerator
		{
			get;
			private set;
		}

		public Document()
		{
			Id = Guid.NewGuid().ToString();
			IdGenerator = new IdGenerator();
			Files = new FileCollection
			{
				IdGenerator = IdGenerator
			};
		}

		public Document Clone()
		{
			Document result = (Document)MemberwiseClone();
			if (Files != null)
			{
				result.Files = new FileCollection();
				Files.ForEach(delegate(File file)
				{
					result.Files.Add(file.Clone());
				});
			}
			return result;
		}

		public void UpdateTargetSegment(string fileId, string paragraphUnitId, string segmentNumber, Segment newTargetSegment)
		{
			File file = Files.SingleOrDefault((File f) => f.Id == fileId);
			if (file == null)
			{
				throw new ArgumentException("File not found", "fileId");
			}
			ParagraphUnit paragraphUnit = file.ParagraphUnits.SingleOrDefault((ParagraphUnit pu) => pu.Id == paragraphUnitId);
			if (paragraphUnit == null)
			{
				throw new ArgumentException("Paragraph Unit not found", "paragraphUnitId");
			}
			SegmentPair segmentPair = paragraphUnit.SegmentPairs.SingleOrDefault((SegmentPair sp) => sp.Target.SegmentNumber == segmentNumber);
			if (segmentPair == null)
			{
				throw new ArgumentException("Segment not found", "segmentNumber");
			}
			Segment target = segmentPair.Target;
			UpdateTags(segmentPair.Source, newTargetSegment);
			ReplaceSegment(target, newTargetSegment);
		}

		private void UpdateTags(Segment sourceSegment, Segment targetSegment)
		{
			List<TagPair> list = sourceSegment.AllSubItems.OfType<TagPair>().ToList();
			List<TagPair> list2 = targetSegment.AllSubItems.OfType<TagPair>().ToList();
			for (int i = 0; i < Math.Min(list.Count, list2.Count); i++)
			{
				list2[i].TagPairDefinitionId = list[i].TagPairDefinitionId;
			}
			List<PlaceholderTag> list3 = sourceSegment.AllSubItems.OfType<PlaceholderTag>().ToList();
			List<PlaceholderTag> list4 = targetSegment.AllSubItems.OfType<PlaceholderTag>().ToList();
			for (int j = 0; j < Math.Min(list3.Count, list4.Count); j++)
			{
				list4[j].PlaceholderTagDefinitionId = list3[j].PlaceholderTagDefinitionId;
			}
			List<StructureTag> list5 = sourceSegment.AllSubItems.OfType<StructureTag>().ToList();
			List<StructureTag> list6 = targetSegment.AllSubItems.OfType<StructureTag>().ToList();
			for (int k = 0; k < Math.Min(list5.Count, list6.Count); k++)
			{
				list6[k].StructureTagDefinitionId = list5[k].StructureTagDefinitionId;
			}
		}

		private void ReplaceSegment(Segment segment, Segment replacement)
		{
			segment.Clear();
			foreach (MarkupData item in replacement)
			{
				segment.Add(item.Clone());
			}
			segment.ConfirmationLevel = replacement.ConfirmationLevel;
			segment.IsLocked = replacement.IsLocked;
			segment.TranslationOrigin = replacement.TranslationOrigin;
		}

		[OnDeserialized]
		internal void OnDeserializedMethod(StreamingContext context)
		{
			foreach (File file in Files)
			{
				file.Parent = this;
				file.IdGenerator = IdGenerator;
			}
			UpdateIdGenerator(this);
		}

		private void UpdateIdGenerator(Document document)
		{
			IdGenerator.UpdateFrom(document);
		}

		public bool Equals(Document other)
		{
			if (other == null)
			{
				return false;
			}
			if (this == other)
			{
				return true;
			}
			if (Equals((MetadataContainer)other) && string.Equals(ModelVersion, other.ModelVersion) && string.Equals(Id, other.Id) && string.Equals(Name, other.Name) && string.Equals(SourceLanguageCode, other.SourceLanguageCode) && string.Equals(TargetLanguageCode, other.TargetLanguageCode))
			{
				return Files.Equals(other.Files);
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
			return Equals((Document)obj);
		}

		public override int GetHashCode()
		{
			int hashCode = base.GetHashCode();
			hashCode = ((hashCode * 397) ^ ((ModelVersion != null) ? ModelVersion.GetHashCode() : 0));
			hashCode = ((hashCode * 397) ^ ((Id != null) ? Id.GetHashCode() : 0));
			hashCode = ((hashCode * 397) ^ ((Name != null) ? Name.GetHashCode() : 0));
			hashCode = ((hashCode * 397) ^ ((SourceLanguageCode != null) ? SourceLanguageCode.GetHashCode() : 0));
			hashCode = ((hashCode * 397) ^ ((TargetLanguageCode != null) ? TargetLanguageCode.GetHashCode() : 0));
			return (hashCode * 397) ^ ((Files != null) ? Files.GetHashCode() : 0);
		}
	}
}
