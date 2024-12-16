using Newtonsoft.Json;
using Sdl.Core.Bcm.BcmModel.Common;
using Sdl.Core.Bcm.BcmModel.Skeleton;
using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmModel.Annotations
{
	[DataContract]
	public class TerminologyAnnotationContainer : AnnotationContainer, ISkeletonItemReference<TerminologyData>
	{
		[DataMember(Name = "terminologyDataId")]
		public int TerminologyDataId
		{
			get;
			set;
		}

		public TerminologyData Definition => ParentParagraphUnit.ParentFile.Skeleton.TerminologyData[SkeletonCollectionKey.From(TerminologyDataId)];

		[JsonProperty("type")]
		protected override string Type
		{
			get
			{
				return "terminology";
			}
			set
			{
			}
		}

		public override void AcceptVisitor(BcmVisitor visitor)
		{
			visitor.VisitTerminologyContainer(this);
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
			TerminologyAnnotationContainer terminologyAnnotationContainer = other as TerminologyAnnotationContainer;
			if (terminologyAnnotationContainer == null)
			{
				return false;
			}
			if (base.Equals(terminologyAnnotationContainer))
			{
				return TerminologyDataId == terminologyAnnotationContainer.TerminologyDataId;
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
			return (base.GetHashCode() * 397) ^ TerminologyDataId;
		}
	}
}
