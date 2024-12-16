using System;
using System.Runtime.Serialization;

namespace Sdl.Core.Api.DataAccess
{
	[DataContract(Name = "Entity", Namespace = "http://schemas.sdl.com/2008/common", IsReference = true)]
	public abstract class Entity : IEquatable<Entity>
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

		private Identity _id;

		private bool _isDirty;

		[DataMember(IsRequired = false, Order = 0)]
		public Identity Id
		{
			get
			{
				return _id;
			}
			set
			{
				_isDirty = (_isDirty || (_id != null && !_id.Equals(value)));
				_id = value;
			}
		}

		[IgnoreEntityMember]
		public bool IsDirty => _isDirty;

		[DataMember]
		[IgnoreEntityMember]
		public StateInfo SerializationState
		{
			get;
			set;
		}

		public void MarkAsDirty()
		{
			_isDirty = true;
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

		public bool Equals(Entity other)
		{
			if (other == null)
			{
				return false;
			}
			if ((object)other == this)
			{
				return true;
			}
			if (Id == null || other.Id == null)
			{
				return false;
			}
			Type type = GetType();
			Type type2 = other.GetType();
			if (type == type2)
			{
				return Id.Equals(other.Id);
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			Entity entity = obj as Entity;
			if (entity != null)
			{
				return Equals(entity);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(Entity entity1, Entity entity2)
		{
			return object.Equals(entity1, entity2);
		}

		public static bool operator !=(Entity entity1, Entity entity2)
		{
			return !(entity1 == entity2);
		}
	}
}
