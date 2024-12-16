using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Xml.Serialization;

namespace Sdl.FileTypeSupport.Framework.Bilingual
{
	[Serializable]
	public abstract class AbstractMarkupData : IAbstractMarkupData, ICloneable, ISupportsUniqueId, ISupportsPersistenceId
	{
		private IAbstractMarkupDataContainer _Parent;

		[NonSerialized]
		private int _persistenceId;

		[NonSerialized]
		private int _uniqueId;

		public virtual IAbstractMarkupDataContainer Parent
		{
			get
			{
				return _Parent;
			}
			set
			{
				if (value != _Parent)
				{
					if (_Parent != null && -1 != _Parent.IndexOf(this))
					{
						throw new FileTypeSupportException(StringResources.AbstractMarkupData_ParentOtherCollectionError);
					}
					if (value != null && -1 == value.IndexOf(this))
					{
						throw new FileTypeSupportException(StringResources.AbstractMarkupData_WrongCollectionError);
					}
					_Parent = value;
				}
			}
		}

		public IParagraph ParentParagraph
		{
			get
			{
				IAbstractMarkupDataContainer parent = _Parent;
				IParagraph paragraph = parent as IParagraph;
				while (parent != null && paragraph == null)
				{
					IAbstractMarkupData abstractMarkupData = parent as IAbstractMarkupData;
					if (abstractMarkupData == null)
					{
						ILockedContainer lockedContainer = parent as ILockedContainer;
						if (lockedContainer == null)
						{
							return null;
						}
						abstractMarkupData = lockedContainer.LockedContent;
					}
					parent = abstractMarkupData.Parent;
					paragraph = (parent as IParagraph);
				}
				return paragraph;
			}
		}

		public int IndexInParent
		{
			get
			{
				if (Parent != null)
				{
					return Parent.IndexOf(this);
				}
				return -1;
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

		[XmlIgnore]
		int ISupportsUniqueId.UniqueId
		{
			get
			{
				return _uniqueId;
			}
			set
			{
				_uniqueId = value;
			}
		}

		protected AbstractMarkupData()
		{
		}

		protected AbstractMarkupData(AbstractMarkupData other)
		{
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			AbstractMarkupData abstractMarkupData = (AbstractMarkupData)obj;
			return true;
		}

		public override int GetHashCode()
		{
			return 42;
		}

		public abstract object Clone();

		public abstract void AcceptVisitor(IMarkupDataVisitor visitor);

		public void RemoveFromParent()
		{
			if (Parent != null)
			{
				Parent.Remove(this);
			}
		}
	}
}
