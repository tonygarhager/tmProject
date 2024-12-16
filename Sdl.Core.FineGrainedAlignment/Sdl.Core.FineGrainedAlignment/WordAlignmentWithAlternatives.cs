using System.Collections.Generic;

namespace Sdl.Core.FineGrainedAlignment
{
	internal class WordAlignmentWithAlternatives : WordAlignment
	{
		public bool StopWord;

		public int Freq;

		public List<WordAlignment> allAlternatives = new List<WordAlignment>();

		public List<WordAlignment> storedAlternatives = new List<WordAlignment>();

		public WordAlignmentWithAlternatives(AlignmentDirection dir, bool stopWord, int freq)
			: base(dir)
		{
			StopWord = stopWord;
			Freq = freq;
		}

		public void StoreAlternatives()
		{
			storedAlternatives.Clear();
			storedAlternatives.AddRange(allAlternatives);
		}

		public void RestoreAlternatives()
		{
			allAlternatives.Clear();
			allAlternatives.AddRange(storedAlternatives);
		}

		public bool IsAmbiguous()
		{
			if (allAlternatives.Count < 2)
			{
				return false;
			}
			double num = allAlternatives[0].confidence - allAlternatives[1].confidence;
			if (num / allAlternatives[0].confidence < 0.01)
			{
				return true;
			}
			return false;
		}

		public void UseBest()
		{
			otherWordIndex = allAlternatives[0].otherWordIndex;
			confidence = allAlternatives[0].confidence;
		}
	}
}
