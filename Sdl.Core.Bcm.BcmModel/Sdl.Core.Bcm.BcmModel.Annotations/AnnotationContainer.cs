using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmModel.Annotations
{
	public abstract class AnnotationContainer : MarkupDataContainer
	{
		[DataMember(Name = "annotationId", EmitDefaultValue = false)]
		public int AnnotationId
		{
			get;
			set;
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
			AnnotationContainer annotationContainer = other as AnnotationContainer;
			if (annotationContainer == null)
			{
				return false;
			}
			if (base.Equals(annotationContainer))
			{
				return AnnotationId == annotationContainer.AnnotationId;
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
			if (obj.GetType() == GetType())
			{
				return Equals((MarkupData)obj);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (base.GetHashCode() * 397) ^ AnnotationId;
		}
	}
}
