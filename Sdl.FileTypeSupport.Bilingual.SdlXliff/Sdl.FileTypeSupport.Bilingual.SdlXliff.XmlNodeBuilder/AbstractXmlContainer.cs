using System.Collections;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Bilingual.SdlXliff.XmlNodeBuilder
{
	public abstract class AbstractXmlContainer : IXmlBuilderContainer, IList<IXmlBuilderNode>, ICollection<IXmlBuilderNode>, IEnumerable<IXmlBuilderNode>, IEnumerable
	{
		private List<IXmlBuilderNode> _nodes = new List<IXmlBuilderNode>();

		public IXmlBuilderNode this[int index]
		{
			get
			{
				return _nodes[index];
			}
			set
			{
				_nodes[index] = value;
			}
		}

		public int Count => _nodes.Count;

		public bool IsReadOnly => false;

		public int IndexOf(IXmlBuilderNode item)
		{
			return _nodes.IndexOf(item);
		}

		public void Insert(int index, IXmlBuilderNode item)
		{
			_nodes.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			_nodes.RemoveAt(index);
		}

		public void Add(IXmlBuilderNode item)
		{
			_nodes.Add(item);
		}

		public void Clear()
		{
			_nodes.Clear();
		}

		public bool Contains(IXmlBuilderNode item)
		{
			return _nodes.Contains(item);
		}

		public void CopyTo(IXmlBuilderNode[] array, int arrayIndex)
		{
			_nodes.CopyTo(array, arrayIndex);
		}

		public bool Remove(IXmlBuilderNode item)
		{
			return _nodes.Remove(item);
		}

		public IEnumerator<IXmlBuilderNode> GetEnumerator()
		{
			return _nodes.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _nodes.GetEnumerator();
		}
	}
}
