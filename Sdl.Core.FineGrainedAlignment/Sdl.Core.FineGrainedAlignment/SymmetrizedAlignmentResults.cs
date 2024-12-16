using System.Collections.Generic;

namespace Sdl.Core.FineGrainedAlignment
{
	internal class SymmetrizedAlignmentResults
	{
		public List<WordAlignment> SymmetrizedAlignments = new List<WordAlignment>();

		public List<WordAlignment> AsymmetricalSourceToTargetAlignments = new List<WordAlignment>();

		public List<WordAlignment> AsymmetricalTargetToSourceAlignments = new List<WordAlignment>();
	}
}
