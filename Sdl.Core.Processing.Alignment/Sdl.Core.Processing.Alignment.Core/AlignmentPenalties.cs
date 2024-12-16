using System;

namespace Sdl.Core.Processing.Alignment.Core
{
	internal class AlignmentPenalties
	{
		private const double SubstitutionProbability = 0.89;

		private const double ExpansionOrContractionProbability = 0.09;

		private const double InsertionOrDeletionProbability = 0.01;

		private const double MeldingProbability = 0.01;

		public int DeletionPenalty => (int)(-100.0 * Math.Log(0.011235955056179775));

		public int InsertionPenalty => (int)(-100.0 * Math.Log(0.011235955056179775));

		public int SubstitutionPenalty => 0;

		public int ExpansionPenalty => (int)(-100.0 * Math.Log(0.10112359550561797));

		public int ContractionPenalty => (int)(-100.0 * Math.Log(0.10112359550561797));

		public int MeldingPenalty => (int)(-100.0 * Math.Log(0.011235955056179775));
	}
}
