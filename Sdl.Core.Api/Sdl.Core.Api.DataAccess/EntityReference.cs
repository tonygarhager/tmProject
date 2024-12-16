using System;
using System.Runtime.Serialization;

namespace Sdl.Core.Api.DataAccess
{
	[DataContract(IsReference = true)]
	[CLSCompliant(false)]
	public abstract class EntityReference : IEntityReference
	{
		[DataContract]
		public class StateInfo
		{
			[DataMember]
			public bool Dirty
			{
				get;
				set;
			}
		}

		protected Identity _foreignKey;

		protected Entity _relatedEntity;

		private bool _isDirty;

		public Identity ForeignKey
		{
			get
			{
				if (_foreignKey != null)
				{
					return _foreignKey;
				}
				if (_relatedEntity != null)
				{
					return _relatedEntity.Id;
				}
				return null;
			}
			set
			{
				_isDirty = (_isDirty || (_foreignKey != null && !_foreignKey.Equals(value)));
				_foreignKey = value;
			}
		}

		Entity IEntityReference.Entity
		{
			get
			{
				return _relatedEntity;
			}
			set
			{
				_relatedEntity = value;
				_isDirty = (_isDirty || (_relatedEntity != null && _relatedEntity.Id != null && !_relatedEntity.Id.Equals(_foreignKey)));
			}
		}

		public bool IsLoaded
		{
			get
			{
				if (!(_relatedEntity != null) && _foreignKey != null)
				{
					return _foreignKey.Value == null;
				}
				return true;
			}
		}

		public bool IsDirty
		{
			get
			{
				if (!_isDirty)
				{
					if (_relatedEntity != null && _relatedEntity.Id != null)
					{
						return !_relatedEntity.Id.Equals(_foreignKey);
					}
					return false;
				}
				return true;
			}
		}

		[DataMember]
		public StateInfo SerializationState
		{
			get;
			set;
		}

		public void MarkAsClean()
		{
			_isDirty = false;
		}

		[OnSerializing]
		public void BeforeDataContractSerialization(StreamingContext context)
		{
			SerializationState = new StateInfo
			{
				Dirty = _isDirty
			};
		}

		[OnDeserialized]
		public void AfterDataContractDeserialization(StreamingContext context)
		{
			_isDirty = SerializationState.Dirty;
		}
	}
	[DataContract(IsReference = true)]
	[CLSCompliant(false)]
	public class EntityReference<T> : EntityReference where T : Entity, new()
	{
		[DataMember(Order = 0)]
		public T Entity
		{
			get
			{
				return (T)_relatedEntity;
			}
			set
			{
				((IEntityReference)this).Entity = value;
				if (((IEntityReference)this).Entity != null)
				{
					base.ForeignKey = ((IEntityReference)this).Entity.Id;
				}
				else
				{
					base.ForeignKey = null;
				}
			}
		}

		[DataMember(Order = 1)]
		public new Identity ForeignKey
		{
			get
			{
				return base.ForeignKey;
			}
			set
			{
				base.ForeignKey = value;
			}
		}

		public EntityReference()
		{
		}

		public EntityReference(T entity)
		{
			_relatedEntity = entity;
		}

		public EntityReference(Identity foreignKey)
		{
			ForeignKey = foreignKey;
		}

		public EntityReference(Guid foreignKey)
			: this(new Identity(foreignKey))
		{
		}

		public EntityReference(int foreignKey)
			: this(new Identity(foreignKey))
		{
		}
	}
}
