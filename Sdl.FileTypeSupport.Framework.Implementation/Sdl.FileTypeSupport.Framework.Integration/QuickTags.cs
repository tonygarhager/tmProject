using Sdl.FileTypeSupport.Framework.IntegrationApi;
using System.Collections;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.Integration
{
	public class QuickTags : IQuickTags, ICollection<IQuickTag>, IEnumerable<IQuickTag>, IEnumerable
	{
		private List<IQuickTag> _QuickTags = new List<IQuickTag>();

		public IList<IQuickTag> StandardQuickTags
		{
			set
			{
				foreach (IQuickTag item in value)
				{
					_QuickTags.Add(item);
				}
			}
		}

		public IQuickTag this[string commandId]
		{
			get
			{
				foreach (IQuickTag quickTag in _QuickTags)
				{
					if (quickTag.CommandId == commandId)
					{
						return quickTag;
					}
				}
				return null;
			}
		}

		public int Count => _QuickTags.Count;

		public bool IsReadOnly => ((ICollection<IQuickTag>)_QuickTags).IsReadOnly;

		public IEnumerable<IQuickTag> AllDisplayItems
		{
			get
			{
				foreach (IQuickTag quickTag in _QuickTags)
				{
					if (quickTag.DisplayOnToolBar)
					{
						yield return quickTag;
					}
				}
			}
		}

		public IEnumerable<IQuickTag> AllDefaultItems
		{
			get
			{
				foreach (IQuickTag quickTag in _QuickTags)
				{
					if (quickTag.IsDefaultQuickTag)
					{
						yield return quickTag;
					}
				}
			}
		}

		public IEnumerable<IQuickTag> AllNonDefaultItems
		{
			get
			{
				foreach (IQuickTag quickTag in _QuickTags)
				{
					if (!quickTag.IsDefaultQuickTag)
					{
						yield return quickTag;
					}
				}
			}
		}

		public QuickTags(params QuickTag[] quickTags)
		{
			CreateQuickTags(quickTags);
		}

		public QuickTags()
		{
		}

		public void SetStandardQuickTags(IList<IQuickTag> standardQuickTags)
		{
			StandardQuickTags = standardQuickTags;
		}

		private void CreateQuickTags(IList<QuickTag> quickTagList)
		{
			foreach (QuickTag quickTag in quickTagList)
			{
				_QuickTags.Add(quickTag);
			}
		}

		public void Add(IQuickTag item)
		{
			if (this[item.CommandId] != null)
			{
				throw new FileTypeSupportException(StringResources.QuickTags_DuplicateIDError);
			}
			_QuickTags.Add(item);
		}

		public void Clear()
		{
			_QuickTags.Clear();
		}

		public bool Contains(IQuickTag item)
		{
			if (this[item.CommandId] != null)
			{
				return true;
			}
			return false;
		}

		public void CopyTo(IQuickTag[] array, int arrayIndex)
		{
			_QuickTags.CopyTo(array, arrayIndex);
		}

		public bool Remove(IQuickTag item)
		{
			int num = _QuickTags.IndexOf(item);
			if (num != -1)
			{
				_QuickTags.RemoveAt(num);
				return true;
			}
			return false;
		}

		public IEnumerator<IQuickTag> GetEnumerator()
		{
			foreach (IQuickTag quickTag in _QuickTags)
			{
				yield return quickTag;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
