using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Sdl.FileTypeSupport.Framework.Bilingual
{
	[Serializable]
	public class Paragraph : MarkupDataContainer, IParagraph, IAbstractMarkupDataContainer, IList<IAbstractMarkupData>, ICollection<IAbstractMarkupData>, IEnumerable<IAbstractMarkupData>, IEnumerable, ISupportsUniqueId, ICloneable, ISupportsPersistenceId
	{
		private IParagraphUnit _ParagraphUnit;

		private Direction _TextDirection;

		[NonSerialized]
		private int _persistenceId;

		public override bool CanBeSplit => false;

		public bool IsSource
		{
			get
			{
				if (_ParagraphUnit != null)
				{
					return this == _ParagraphUnit.Source;
				}
				return false;
			}
		}

		public bool IsTarget
		{
			get
			{
				if (_ParagraphUnit != null)
				{
					return this == _ParagraphUnit.Target;
				}
				return false;
			}
		}

		public IParagraphUnit Parent
		{
			get
			{
				return _ParagraphUnit;
			}
			set
			{
				if (_ParagraphUnit != value)
				{
					if (_ParagraphUnit != null && (_ParagraphUnit.Source == this || _ParagraphUnit.Target == this))
					{
						throw new ArgumentException(StringResources.Paragraph_WrongPUError);
					}
					if (value != null && value.Source != this && value.Target != this)
					{
						throw new ArgumentException(StringResources.Paragraph_WrongPUParentError);
					}
					_ParagraphUnit = value;
				}
			}
		}

		public Direction TextDirection
		{
			get
			{
				return _TextDirection;
			}
			set
			{
				_TextDirection = value;
			}
		}

		[XmlIgnore]
		public int PersistenceId
		{
			get
			{
				return _persistenceId;
			}
			set
			{
				_persistenceId = value;
			}
		}

		public Paragraph()
		{
			_TextDirection = Direction.Inherit;
		}

		protected Paragraph(Paragraph other)
			: base(other)
		{
			_TextDirection = other._TextDirection;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			Paragraph obj2 = (Paragraph)obj;
			if (!base.Equals((object)obj2))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override IAbstractMarkupDataContainer Split(int splitBeforeItemIndex)
		{
			throw new NotSplittableException(StringResources.Paragraph_NoSplitError);
		}

		public override object Clone()
		{
			return new Paragraph(this);
		}
	}
}
