using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sdl.Core.Api.DataAccess
{
	public interface EntityCollection : IEnumerable
	{
		List<Identity> RemovedEntities
		{
			get;
			set;
		}

		bool IsLoaded
		{
			get;
			set;
		}

		void AddEntity(Entity entity);

		void RemoveLocal(Entity entity);
	}
	[DataContract(IsReference = true)]
	public class EntityCollection<T> : EntityCollection, IEnumerable, ICollection<T>, IEnumerable<T> where T : Entity, new()
	{
		private List<T> _entities;

		private List<Identity> _removedEntities = new List<Identity>();

		public T this[int index]
		{
			get
			{
				return Entities[index];
			}
			set
			{
				Entities[index] = value;
			}
		}

		[DataMember]
		public bool IsLoaded
		{
			get;
			set;
		}

		public int Count => Entities.Count;

		public bool IsReadOnly => false;

		[DataMember]
		public List<T> Entities
		{
			get
			{
				if (_entities == null)
				{
					_entities = new List<T>();
				}
				return _entities;
			}
			set
			{
				_entities = value;
			}
		}

		[DataMember]
		public List<Identity> RemovedEntities
		{
			get
			{
				return _removedEntities;
			}
			set
			{
				_removedEntities = value;
			}
		}

		public EntityCollection()
		{
		}

		public EntityCollection(ICollection<T> items)
		{
			foreach (T item in items)
			{
				Add(item);
			}
		}

		public void Add(T item)
		{
			Entities.Add(item);
		}

		public void Clear()
		{
			foreach (T entity in Entities)
			{
				OnEntityRemoved(entity);
			}
			Entities.Clear();
		}

		public bool Contains(T item)
		{
			return Entities.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			Entities.CopyTo(array, arrayIndex);
		}

		public bool Remove(T item)
		{
			if (Entities.Remove(item))
			{
				OnEntityRemoved(item);
				return true;
			}
			return false;
		}

		public void RemoveLocal(Entity entity)
		{
			Entities.Remove((T)entity);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return Entities.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)Entities).GetEnumerator();
		}

		public void AddEntity(Entity entity)
		{
			Add((T)entity);
		}

		private void OnEntityRemoved(T entity)
		{
			if (entity.Id != null)
			{
				_removedEntities.Add(entity.Id);
			}
		}
	}
}
