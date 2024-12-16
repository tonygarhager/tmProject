using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Sdl.Enterprise2.Platform.Contracts.IdentityModel
{
	[CollectionDataContract(Namespace = "http://sdl.com/identity/2010", Name = "Permissions", IsReference = true)]
	public class PermissionCollection : ICollection<Permission>, IEnumerable<Permission>, IEnumerable, IPermissionCheck
	{
		private List<Permission> _permissions = new List<Permission>();

		public int Count => _permissions.Count;

		public bool IsReadOnly => false;

		public PermissionCollection()
		{
		}

		public PermissionCollection(IList<Permission> list)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			_permissions.AddRange(list);
		}

		public bool HasPermission(string permission)
		{
			if (string.IsNullOrEmpty(permission))
			{
				throw new ArgumentNullException("permission");
			}
			return _permissions.Any((Permission p) => p.Name.Equals(permission, StringComparison.OrdinalIgnoreCase));
		}

		public void Add(Permission item)
		{
			_permissions.Add(item);
		}

		public void Clear()
		{
			_permissions.Clear();
		}

		public bool Contains(Permission item)
		{
			return _permissions.Contains(item);
		}

		public void CopyTo(Permission[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public bool Remove(Permission item)
		{
			return _permissions.Remove(item);
		}

		public IEnumerator<Permission> GetEnumerator()
		{
			return _permissions.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<Permission>)this).GetEnumerator();
		}
	}
}
