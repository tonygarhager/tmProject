using Sdl.FileTypeSupport.Framework.BilingualApi;
using System;

namespace Sdl.FileTypeSupport.Framework.Bilingual
{
	[Serializable]
	public class FeedbackMarker : AbstractRevisionBase
	{
		public FeedbackMarker()
		{
		}

		protected FeedbackMarker(FeedbackMarker other)
			: base(other)
		{
		}

		protected FeedbackMarker(FeedbackMarker other, int splitBeforeItemIndex)
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
			return new FeedbackMarker(this, splitBeforeItemIndex);
		}

		public override object Clone()
		{
			return new FeedbackMarker(this);
		}
	}
}
