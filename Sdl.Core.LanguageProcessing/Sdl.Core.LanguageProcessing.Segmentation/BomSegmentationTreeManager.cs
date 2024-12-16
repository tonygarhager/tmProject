using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Core.LanguageProcessing.Segmentation
{
	public class BomSegmentationTreeManager : SegmentationTreeManager
	{
		private readonly BomTree _tree = new BomTree();

		private Node _currentNode;

		public override Tree Tree => _tree;

		public BomSegmentationTreeManager()
		{
			_segmentationEngineRunner = new SegmentationEngineRunner();
		}

		public void PopulateTree(IAbstractMarkupDataContainer rootContainer)
		{
			foreach (IAbstractMarkupData item in rootContainer)
			{
				if (item is IAbstractMarkupDataContainer)
				{
					PopulateContainer(item);
				}
				else
				{
					PopulateItem(item);
				}
			}
		}

		public void PopulateTreeWithRoot(IAbstractMarkupData root)
		{
			IAbstractMarkupDataContainer abstractMarkupDataContainer = root as IAbstractMarkupDataContainer;
			if (abstractMarkupDataContainer is IParagraph)
			{
				PopulateTree(abstractMarkupDataContainer);
			}
			if (abstractMarkupDataContainer != null)
			{
				PopulateContainer(abstractMarkupDataContainer as IAbstractMarkupData);
			}
			else
			{
				PopulateItem(root);
			}
		}

		private void PopulateItem(IAbstractMarkupData item)
		{
			if (_currentNode == null)
			{
				_currentNode = _tree.Root.AddChild(new BomNode
				{
					Item = item
				});
			}
			else if (_currentNode.IsContainer && !_currentNode.IsLockedContent)
			{
				_currentNode.AddChild(new BomNode
				{
					Item = item
				});
			}
			else
			{
				_currentNode.Add(new BomNode
				{
					Item = item
				});
			}
		}

		private void PopulateContainer(IAbstractMarkupData item)
		{
			if (_currentNode == null)
			{
				_currentNode = _tree.Root.AddChild(new BomNode
				{
					Item = item
				});
			}
			else if (_currentNode.IsContainer && !_currentNode.IsLockedContent)
			{
				_currentNode = _currentNode.AddChild(new BomNode
				{
					Item = item
				});
			}
			else
			{
				_currentNode = _currentNode.Add(new BomNode
				{
					Item = item
				});
			}
			IAbstractMarkupDataContainer abstractMarkupDataContainer = item as IAbstractMarkupDataContainer;
			if (abstractMarkupDataContainer != null)
			{
				foreach (IAbstractMarkupData item2 in abstractMarkupDataContainer)
				{
					if (item2 is IAbstractMarkupDataContainer)
					{
						PopulateContainer(item2);
					}
					else
					{
						PopulateItem(item2);
					}
				}
			}
			_currentNode = _currentNode.Parent;
		}

		public IAbstractMarkupData FindNextStarter()
		{
			return (FindNextStarterNode() as BomNode)?.Item;
		}

		public IAbstractMarkupData FindNextEnder()
		{
			return (FindNextEnderNode() as BomNode)?.Item;
		}
	}
}
