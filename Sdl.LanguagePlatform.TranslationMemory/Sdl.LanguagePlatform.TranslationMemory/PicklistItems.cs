using Sdl.LanguagePlatform.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	public class PicklistItems : IEnumerable<PicklistItem>, IEnumerable
	{
		[DataMember]
		private List<PicklistItem> _Items;

		public PicklistItems()
		{
			_Items = new List<PicklistItem>();
		}

		public PicklistItems(PicklistItems other)
		{
			if (other == null)
			{
				throw new ArgumentNullException();
			}
			_Items = new List<PicklistItem>();
			foreach (PicklistItem item in other._Items)
			{
				_Items.Add(new PicklistItem(item));
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj.GetType() != GetType())
			{
				return false;
			}
			PicklistItems other = obj as PicklistItems;
			if (other != null && _Items.Count == other._Items.Count)
			{
				return _Items.All((PicklistItem pli) => other.Contains(pli.Name));
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public PicklistItem Lookup(string name)
		{
			return _Items.FirstOrDefault((PicklistItem item) => item.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
		}

		public bool Contains(string name)
		{
			return Lookup(name) != null;
		}

		public bool Contains(PicklistItem item)
		{
			return Contains(item.Name);
		}

		public PicklistItem Lookup(int id)
		{
			return _Items.FirstOrDefault((PicklistItem item) => item.ID == id);
		}

		public PicklistItem Add(string itemName)
		{
			if (Lookup(itemName) != null)
			{
				throw new LanguagePlatformException(ErrorCode.TMPicklistValueAlreadyExists, itemName);
			}
			PicklistItem picklistItem = new PicklistItem(itemName);
			_Items.Add(picklistItem);
			return picklistItem;
		}

		public void Add(PicklistItem pli)
		{
			Add(pli, ignoreDups: false);
		}

		public void Add(PicklistItem pli, bool ignoreDups)
		{
			if (Lookup(pli.Name) != null)
			{
				if (!ignoreDups)
				{
					throw new LanguagePlatformException(ErrorCode.TMPicklistValueAlreadyExists, pli.Name);
				}
			}
			else if (pli.ID.HasValue && Lookup(pli.ID.Value) != null)
			{
				if (!ignoreDups)
				{
					throw new LanguagePlatformException(ErrorCode.TMPicklistValueAlreadyExists, "Item# " + pli.ID.Value.ToString());
				}
			}
			else
			{
				_Items.Add(pli);
			}
		}

		public void Clear()
		{
			_Items.Clear();
		}

		public List<string> GetNames()
		{
			return _Items.Select((PicklistItem pli) => pli.Name).ToList();
		}

		public IEnumerator<PicklistItem> GetEnumerator()
		{
			return _Items.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _Items.GetEnumerator();
		}
	}
}
