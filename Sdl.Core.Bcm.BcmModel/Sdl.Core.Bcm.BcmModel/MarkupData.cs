using Newtonsoft.Json;
using Sdl.Core.Bcm.BcmModel.Common;
using Sdl.Core.Bcm.BcmModel.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmModel
{
	[DataContract]
	[JsonConverter(typeof(MarkupDataCreator))]
	public abstract class MarkupData : MetadataContainer, ICloneable<MarkupData>, IEquatable<MarkupData>
	{
		private MarkupDataContainer _parent;

		private Fragment _parentFragment;

		[JsonProperty(PropertyName = "id", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, Order = int.MinValue)]
		public string Id
		{
			get;
			set;
		}

		[JsonProperty("type")]
		protected abstract string Type
		{
			get;
			set;
		}

		public MarkupDataContainer Parent
		{
			get
			{
				return _parent;
			}
			set
			{
				_parent = value;
			}
		}

		public Fragment ParentFragment
		{
			get
			{
				if (_parentFragment != null)
				{
					return _parentFragment;
				}
				for (MarkupDataContainer parent = Parent; parent != null; parent = parent.Parent)
				{
					if (parent.ParentFragment != null)
					{
						return parent.ParentFragment;
					}
				}
				return null;
			}
			internal set
			{
				_parentFragment = value;
			}
		}

		public Paragraph ParentParagraph
		{
			get
			{
				MarkupData markupData = this;
				while (markupData != null && !(markupData is Paragraph))
				{
					markupData = markupData.Parent;
				}
				return markupData as Paragraph;
			}
		}

		public virtual ParagraphUnit ParentParagraphUnit
		{
			get
			{
				return ParentParagraph?.ParentParagraphUnit;
			}
			set
			{
				Paragraph parentParagraph = ParentParagraph;
				if (parentParagraph != null)
				{
					parentParagraph.ParentParagraphUnit = value;
				}
			}
		}

		public IEnumerable<MarkupDataContainer> Ancestors
		{
			get
			{
				for (MarkupDataContainer p = Parent; p != null; p = p.Parent)
				{
					yield return p;
				}
			}
		}

		public Segment ParentSegment => Ancestors.OfType<Segment>().FirstOrDefault();

		public string FrameworkId => GetMetadata("frameworkOriginalTagId");

		public bool IsContainer => this is MarkupDataContainer;

		public int IndexInParent
		{
			get
			{
				if (Parent == null)
				{
					return -1;
				}
				return Parent.IndexOf(this);
			}
		}

		protected MarkupData(string id)
		{
			Id = id;
		}

		protected MarkupData()
		{
			GenerateId();
		}

		public abstract void AcceptVisitor(BcmVisitor visitor);

		public void RemoveFromParent()
		{
			Parent?.Remove(this);
		}

		protected void CopyPropertiesTo(MarkupData target)
		{
			CopyPropertiesTo((MetadataContainer)target);
			target.Id = Id;
		}

		public virtual MarkupData Clone()
		{
			MarkupData markupData = (MarkupData)MemberwiseClone();
			markupData.ReplaceMetadataWith(base.Metadata);
			return markupData;
		}

		public virtual bool Equals(MarkupData other)
		{
			if (other == null)
			{
				return false;
			}
			if (this == other)
			{
				return true;
			}
			if (Equals((MetadataContainer)other))
			{
				return string.Equals(Id, other.Id);
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
			return (hashCode * 397) ^ ((Id != null) ? Id.GetHashCode() : 0);
		}

		public virtual MarkupData UniqueClone()
		{
			MarkupData markupData = Clone();
			markupData.GenerateId();
			return markupData;
		}

		private void GenerateId()
		{
			Id = Guid.NewGuid().ToString();
		}
	}
}
