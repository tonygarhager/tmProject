namespace Sdl.Core.LanguageProcessing.Segmentation
{
	public class BomTree : Tree
	{
		public BomTree()
		{
			base.Root = new BomNode();
		}
	}
}
