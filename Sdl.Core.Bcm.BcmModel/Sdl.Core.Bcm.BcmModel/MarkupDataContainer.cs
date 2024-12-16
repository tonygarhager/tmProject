using Sdl.Core.Bcm.BcmModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Sdl.Core.Bcm.BcmModel
{
	[DataContract]
	public abstract class MarkupDataContainer : MarkupData
	{
		public const string AutoClonedTagPairKey = "SDL:AutoCloned";

		private List<MarkupData> _children = new List<MarkupData>();

		[DataMember(Name = "children", Order = int.MaxValue)]
		public IList<MarkupData> Children => _children;

		public IEnumerable<MarkupData> AllSubItems => GetAllChildren((MarkupData x) => true);

		public int Count => _children.Count;

		public MarkupData this[int index]
		{
			get
			{
				return _children[index];
			}
			set
			{
				RemoveAt(index);
				InsertAt(index, value);
			}
		}

		protected MarkupDataContainer()
		{
		}

		protected MarkupDataContainer(string id)
			: base(id)
		{
		}

		public IEnumerable<MarkupData> GetAllChildren(Func<MarkupData, bool> condition)
		{
			foreach (MarkupData markupData in Children)
			{
				if (condition(markupData))
				{
					yield return markupData;
				}
				MarkupDataContainer markupDataContainer = markupData as MarkupDataContainer;
				if (markupDataContainer != null)
				{
					foreach (MarkupData allChild in markupDataContainer.GetAllChildren(condition))
					{
						yield return allChild;
					}
				}
			}
		}

		public IEnumerable<T> GetAllChildren<T>(Func<T, bool> condition) where T : MarkupData
		{
			return AllSubItems.OfType<T>().Where(condition);
		}

		public T First<T>(Func<T, bool> condition) where T : MarkupData
		{
			return GetAllChildren(condition).First();
		}

		public T FirstOrDefault<T>(Func<T, bool> condition) where T : MarkupData
		{
			IEnumerable<T> allChildren = GetAllChildren(condition);
			IList<T> source = (allChildren as IList<T>) ?? allChildren.ToList();
			if (source.Any())
			{
				return source.First();
			}
			return null;
		}

		private void SetParentToThis(IEnumerable<MarkupData> range)
		{
			foreach (MarkupData item in range)
			{
				item.Parent = this;
			}
		}

		public void ForEach(Action<MarkupData> action)
		{
			foreach (MarkupData child in _children)
			{
				action(child);
			}
		}

		public MarkupDataContainer Add(MarkupData markupData)
		{
			markupData.Parent = this;
			_children.Add(markupData);
			return this;
		}

		public MarkupDataContainer AddRange(IEnumerable<MarkupData> range)
		{
			IList<MarkupData> list = (range as IList<MarkupData>) ?? range.ToList();
			SetParentToThis(list);
			_children.AddRange(list);
			return this;
		}

		public MarkupDataContainer InsertAt(int index, MarkupData item)
		{
			item.Parent = this;
			_children.Insert(index, item);
			return this;
		}

		public MarkupDataContainer InsertRange(int index, IEnumerable<MarkupData> range)
		{
			IList<MarkupData> list = (range as IList<MarkupData>) ?? range.ToList();
			SetParentToThis(list);
			_children.InsertRange(index, list);
			return this;
		}

		public void Clear()
		{
			foreach (MarkupData child in _children)
			{
				child.Parent = null;
			}
			_children.Clear();
		}

		public virtual void ClearTagPairs()
		{
			List<MarkupDataContainer> list = _children.OfType<MarkupDataContainer>().ToList();
			foreach (MarkupDataContainer item in list)
			{
				item.ClearTagPairs();
			}
		}

		internal void Reset()
		{
			_children = new List<MarkupData>();
		}

		public MarkupDataContainer Remove(MarkupData markupData)
		{
			int num = IndexOf(markupData);
			if (num == -1)
			{
				return this;
			}
			RemoveAt(num);
			return this;
		}

		public MarkupDataContainer RemoveAt(int index)
		{
			_children[index].Parent = null;
			_children.RemoveAt(index);
			return this;
		}

		public MarkupDataContainer RemoveRange(int index, int count)
		{
			int num = index;
			while (num < Count && num - index < count)
			{
				RemoveAt(num++);
			}
			return this;
		}

		public MarkupDataContainer SplitAt(int index)
		{
			MarkupDataContainer markupDataContainer = base.Clone() as MarkupDataContainer;
			if (markupDataContainer == null)
			{
				return null;
			}
			markupDataContainer.Reset();
			markupDataContainer.Id = Guid.NewGuid().ToString();
			SetMetadata("SDL:AutoCloned", true.ToString());
			markupDataContainer.SetMetadata("SDL:AutoCloned", true.ToString());
			MoveToContainer(markupDataContainer, index, 0, _children.Count - index);
			return markupDataContainer;
		}

		public void MoveToContainer(MarkupDataContainer target, int sourceIndex, int targetIndex, int count)
		{
			List<MarkupData> range = _children.GetRange(sourceIndex, count);
			target.InsertRange(targetIndex, range);
			_children.RemoveRange(sourceIndex, count);
		}

		public bool Contains(MarkupData markupData)
		{
			return _children.Contains(markupData);
		}

		public int IndexOf(MarkupData markupData)
		{
			return _children.IndexOf(markupData);
		}

		public int FindIndex(Predicate<MarkupData> match)
		{
			return _children.FindIndex(match);
		}

		public IEnumerator<MarkupData> GetEnumerator()
		{
			return _children.GetEnumerator();
		}

		protected void CopyPropertiesTo(MarkupDataContainer target)
		{
			CopyPropertiesTo((MarkupData)target);
		}

		public override MarkupData Clone()
		{
			return CloneContainer((MarkupData data) => data.Clone(), base.Clone);
		}

		public override bool Equals(MarkupData other)
		{
			MarkupDataContainer markupDataContainer = other as MarkupDataContainer;
			if (markupDataContainer == null)
			{
				return false;
			}
			return Equals(markupDataContainer);
		}

		public virtual bool Equals(MarkupDataContainer other)
		{
			if (other == null)
			{
				return false;
			}
			if (this == other)
			{
				return true;
			}
			if (base.Equals(other))
			{
				return Children.IsSequenceEqual(other.Children);
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
			return Equals((MarkupDataContainer)obj);
		}

		public override int GetHashCode()
		{
			return (base.GetHashCode() * 397) ^ ((Children != null) ? Children.GetHashCode() : 0);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("<{0}>", Type);
			foreach (MarkupData child in Children)
			{
				stringBuilder.Append(child);
			}
			stringBuilder.AppendFormat("</{0}>", Type);
			return stringBuilder.ToString();
		}

		[OnDeserialized]
		internal void OnDeserializedMethod(StreamingContext context)
		{
			foreach (MarkupData child in _children)
			{
				child.Parent = this;
			}
		}

		public override MarkupData UniqueClone()
		{
			return CloneContainer((MarkupData data) => data.UniqueClone(), base.UniqueClone);
		}

		internal virtual MarkupDataContainer CloneWithoutChildren()
		{
			MarkupDataContainer markupDataContainer = base.Clone() as MarkupDataContainer;
			markupDataContainer?.Reset();
			return markupDataContainer;
		}

		private MarkupData CloneContainer(Func<MarkupData, MarkupData> cloneAction, Func<MarkupData> baseAction)
		{
			MarkupDataContainer markupDataContainer = baseAction() as MarkupDataContainer;
			if (markupDataContainer == null)
			{
				return null;
			}
			markupDataContainer.Reset();
			foreach (MarkupData child in Children)
			{
				markupDataContainer.Add(cloneAction(child));
			}
			return markupDataContainer;
		}
	}
}
