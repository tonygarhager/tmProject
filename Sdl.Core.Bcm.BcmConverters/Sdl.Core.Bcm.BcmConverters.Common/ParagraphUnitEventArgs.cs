using Sdl.Core.Bcm.BcmModel;
using System;

namespace Sdl.Core.Bcm.BcmConverters.Common
{
	public class ParagraphUnitEventArgs : EventArgs
	{
		public ParagraphUnit ParagraphUnit
		{
			get;
			set;
		}

		public ParagraphUnitEventArgs(ParagraphUnit paragraphUnit)
		{
			ParagraphUnit = paragraphUnit;
		}
	}
}
