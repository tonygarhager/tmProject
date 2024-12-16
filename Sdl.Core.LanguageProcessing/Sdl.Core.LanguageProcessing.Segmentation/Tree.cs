namespace Sdl.Core.LanguageProcessing.Segmentation
{
	public class Tree
	{
		public Node Root
		{
			get;
			set;
		}

		public int Count
		{
			get
			{
				int num = 0;
				if (!Root.HasChild)
				{
					return num;
				}
				TreeIterator treeIterator = new TreeIterator(Root.FirstChild);
				Node node = treeIterator.CurrentNode;
				while (node != null)
				{
					node = treeIterator.Next();
					num++;
				}
				return num;
			}
		}

		public Tree()
		{
			Root = null;
		}
	}
}
