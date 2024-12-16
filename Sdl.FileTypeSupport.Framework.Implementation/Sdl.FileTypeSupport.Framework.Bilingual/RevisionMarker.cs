using Sdl.FileTypeSupport.Framework.BilingualApi;
using System;

namespace Sdl.FileTypeSupport.Framework.Bilingual
{
	[Serializable]
	public class RevisionMarker : AbstractRevisionBase
	{
		public RevisionMarker()
		{
		}

		protected RevisionMarker(RevisionMarker other)
			: base(other)
		{
		}

		protected RevisionMarker(RevisionMarker other, int splitBeforeItemIndex)
			: base(other, splitBeforeItemIndex)
		{
		}

		public override void AcceptVisitor(IMarkupDataVisitor visitor)
		{
			if (visitor == null)
			{
				throw new ArgumentNullException("visitor");
			}
			visitor.VisitRevisionMarker(this);
		}

		public override IAbstractMarkupDataContainer Split(int splitBeforeItemIndex)
		{
			return new RevisionMarker(this, splitBeforeItemIndex);
		}

		public override object Clone()
		{
			return new RevisionMarker(this);
		}
	}
}
