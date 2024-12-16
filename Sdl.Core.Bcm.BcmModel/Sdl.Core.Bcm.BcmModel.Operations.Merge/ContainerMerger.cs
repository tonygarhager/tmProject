using System.Linq;

namespace Sdl.Core.Bcm.BcmModel.Operations.Merge
{
	internal class ContainerMerger
	{
		private MarkupDataContainer _first;

		private MarkupDataContainer _second;

		public ContainerMerger(MarkupDataContainer first, MarkupDataContainer second)
		{
			_first = first;
			_second = second;
		}

		public MarkupDataContainer Merge()
		{
			if ((_first.Count > 0 || _second.Count > 0) && !_first.Parent.Equals(_second.Parent))
			{
				MarkupDataContainer commonAncestor = GetCommonAncestor(_first, _second);
				PushDownAncestorsOf(ref _first, commonAncestor);
				PushDownAncestorsOf(ref _second, commonAncestor);
			}
			MergeBetweenSegments();
			_second.RemoveFromParent();
			_first.AddRange(_second.Children);
			ContainerCleaner containerCleaner = new ContainerCleaner(_first);
			containerCleaner.PerformCleanup(moveLeftOutside: false, moveRightOutside: false);
			_first.Validate();
			return _first;
		}

		private MarkupDataContainer GetCommonAncestor(MarkupDataContainer first, MarkupDataContainer second)
		{
			for (MarkupDataContainer parent = first.Parent; parent != null; parent = parent.Parent)
			{
				for (MarkupDataContainer parent2 = second.Parent; parent2 != null; parent2 = parent2.Parent)
				{
					if (parent.Equals(parent2))
					{
						return parent;
					}
				}
			}
			return null;
		}

		private void MergeBetweenSegments()
		{
			if (_second.Count != 0)
			{
				foreach (MarkupData item in _first.GetElementsInBetween(_second))
				{
					item.RemoveFromParent();
					_first.Add(item);
				}
			}
		}

		private static void PushDownAncestorsOf(ref MarkupDataContainer container, MarkupDataContainer commonAncestor)
		{
			MarkupDataContainer root = ContainerParentSplitter.SplitContainerParents(container, commonAncestor);
			PushDownAncestors(ref container, root, commonAncestor);
		}

		private static void PushDownAncestors(ref MarkupDataContainer item, MarkupDataContainer root, MarkupDataContainer commonAncestor)
		{
			bool flag = false;
			MarkupDataContainer current = item;
			while (!current.Equals(root) && !(current is Paragraph) && !(current.Parent is Paragraph) && (commonAncestor == null || !current.Equals(commonAncestor)) && PushDown(ref current))
			{
				current = current.Parent;
				flag = true;
			}
			if (flag)
			{
				string id = item.Id;
				item = current.AllSubItems.OfType<MarkupDataContainer>().First((MarkupDataContainer x) => x.Id == id);
			}
		}

		private static bool PushDown(ref MarkupDataContainer current)
		{
			MarkupDataContainer markupDataContainer = current.Clone() as MarkupDataContainer;
			MarkupDataContainer parent = current.Parent;
			MarkupDataContainer parent2 = parent.Parent;
			int indexInParent = parent.IndexInParent;
			parent2[indexInParent] = markupDataContainer;
			parent.Clear();
			markupDataContainer.InsertAt(0, parent);
			while (markupDataContainer.Count > 1)
			{
				MarkupData markupData = markupDataContainer[1];
				markupData.RemoveFromParent();
				parent.Add(markupData);
			}
			current = markupDataContainer;
			return true;
		}

		private void RemoveAutoclonedMetadata()
		{
			_first.RemoveMetadata("SDL:AutoCloned");
			_second.RemoveMetadata("SDL:AutoCloned");
		}
	}
}
