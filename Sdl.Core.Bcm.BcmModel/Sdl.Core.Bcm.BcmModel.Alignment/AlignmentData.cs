using System;
using System.Collections.Generic;

namespace Sdl.Core.Bcm.BcmModel.Alignment
{
	public class AlignmentData : ExtensionDataContainer
	{
		public int TuId
		{
			get;
			set;
		}

		public DateTime ContentInsertDate
		{
			get;
			set;
		}

		public DateTime? ModelDate
		{
			get;
			set;
		}

		public List<SpanPairNode> SpanPairSet
		{
			get;
			set;
		}

		public List<SpanPairNode> IncompatibleSpanPairSet
		{
			get;
			set;
		}
	}
}
