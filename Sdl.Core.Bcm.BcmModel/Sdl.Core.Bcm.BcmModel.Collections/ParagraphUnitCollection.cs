using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.Bcm.BcmModel.Collections
{
	public class ParagraphUnitCollection : KeyBasedCollection<string, ParagraphUnit>
	{
		private int? _segmentCount;

		public File ParentFile
		{
			get;
			set;
		}

		public IEnumerable<SegmentPair> AllSegmentPairs => base.Items.Where((ParagraphUnit p) => !p.IsStructure).SelectMany((ParagraphUnit p) => p.SegmentPairs);

		public int SegmentCount
		{
			get
			{
				if (!_segmentCount.HasValue)
				{
					_segmentCount = AllSegmentPairs.Count();
				}
				return _segmentCount.Value;
			}
		}

		public ParagraphUnitCollection()
		{
			KeySelector = ((ParagraphUnit paragraphUnit) => paragraphUnit.Id);
		}

		protected override void InsertItem(int index, ParagraphUnit item)
		{
			if (ParentFile != null)
			{
				item.ParentFile = ParentFile;
				item.ParentFileId = ParentFile.Id;
			}
			base.InsertItem(index, item);
		}
	}
}
