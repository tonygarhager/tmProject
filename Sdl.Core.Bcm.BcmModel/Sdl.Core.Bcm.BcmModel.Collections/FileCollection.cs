using Sdl.Core.Bcm.BcmModel.Skeleton;

namespace Sdl.Core.Bcm.BcmModel.Collections
{
	public class FileCollection : KeyBasedCollection<string, File>
	{
		public IdGenerator IdGenerator
		{
			get;
			set;
		}

		public FileCollection()
		{
			KeySelector = ((File file) => file.Id);
		}

		protected override void InsertItem(int index, File item)
		{
			if (IdGenerator != null)
			{
				item.IdGenerator = IdGenerator;
			}
			base.InsertItem(index, item);
		}

		protected override void SetItem(int index, File item)
		{
			if (IdGenerator != null)
			{
				item.IdGenerator = IdGenerator;
			}
			base.SetItem(index, item);
		}
	}
}
