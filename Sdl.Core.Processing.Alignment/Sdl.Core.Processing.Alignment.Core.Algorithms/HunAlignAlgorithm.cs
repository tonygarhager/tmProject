using Sdl.Core.Processing.Alignment.Common;
using Sdl.Core.Processing.Alignment.Core.CostComputers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.Processing.Alignment.Core.Algorithms
{
	internal class HunAlignAlgorithm : AbstractAlignmentAlgorithm
	{
		internal IBilingualDictionary UpdateBilingualDictionary;

		internal IAlignmentCostComputer CostComputer;

		public HunAlignAlgorithm(AlignmentAlgorithmSettings alignmentAlgorithmSettings, IBilingualDictionary bilingualDictionary)
			: this(alignmentAlgorithmSettings, bilingualDictionary, updateBilingualDictionary: true)
		{
		}

		public HunAlignAlgorithm(AlignmentAlgorithmSettings alignmentAlgorithmSettings, IBilingualDictionary bilingualDictionary, bool updateBilingualDictionary)
			: this(alignmentAlgorithmSettings, bilingualDictionary, updateBilingualDictionary, new HunAlignCompositeCostComputer(bilingualDictionary, alignmentAlgorithmSettings.ResourceDataAccessor))
		{
		}

		public HunAlignAlgorithm(AlignmentAlgorithmSettings alignmentAlgorithmSettings, IBilingualDictionary bilingualDictionary, bool updateBilingualDictionary, IAlignmentCostComputer costComputer)
			: base(alignmentAlgorithmSettings)
		{
			if (bilingualDictionary == null)
			{
				throw new ArgumentNullException("bilingualDictionary");
			}
			UpdateBilingualDictionary = (updateBilingualDictionary ? bilingualDictionary : null);
			CostComputer = costComputer;
		}

		internal override IAlignmentCostComputer GetAlignmentCostComputer()
		{
			return CostComputer;
		}

		public override IList<AlignmentData> Align(IList<AlignmentElement> leftInputElements, IList<AlignmentElement> rightInputElements)
		{
			if (UpdateBilingualDictionary != null)
			{
				IList<AlignmentData> alignments = base.Align(leftInputElements, rightInputElements);
				WordForager wordForager = new WordForager(UpdateBilingualDictionary.SourceCulture, UpdateBilingualDictionary.TargetCulture);
				AlignedPairsEnumerable<AlignmentElement> alignedPairs = new AlignedPairsEnumerable<AlignmentElement>(leftInputElements, rightInputElements, alignments, (IEnumerable<DocumentSegmentId> ids, IEnumerable<AlignmentElement> alignmentElements) => ids.Select((DocumentSegmentId id) => alignmentElements.First((AlignmentElement el) => object.Equals(el.Id, id))), MergeAlignmentElements);
				DefaultBilingualDictionary sourceBilingualDictionary = wordForager.CreateBilingualDictionary(alignedPairs);
				AddEntries(sourceBilingualDictionary, UpdateBilingualDictionary);
			}
			return base.Align(leftInputElements, rightInputElements);
		}

		public void AddEntries(IBilingualDictionary sourceBilingualDictionary, IBilingualDictionary targetBilingualDictionary)
		{
			foreach (string sourceWord in sourceBilingualDictionary.GetSourceWords())
			{
				foreach (string targetWord in sourceBilingualDictionary.GetTargetWords(sourceWord))
				{
					if (targetBilingualDictionary.CanAddEntry(sourceWord, targetWord))
					{
						targetBilingualDictionary.AddEntry(sourceWord, targetWord);
					}
				}
			}
		}

		private static AlignmentElement MergeAlignmentElements(IEnumerable<AlignmentElement> alignmentElements)
		{
			AlignmentElement[] source = (alignmentElements as AlignmentElement[]) ?? alignmentElements.ToArray();
			if (source.Any())
			{
				return source.Aggregate((AlignmentElement elem0, AlignmentElement elem1) => elem0.Merge(elem1));
			}
			return null;
		}
	}
}
