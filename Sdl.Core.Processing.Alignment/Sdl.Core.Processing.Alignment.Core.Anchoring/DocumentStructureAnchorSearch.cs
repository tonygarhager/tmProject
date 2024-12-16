using Sdl.Core.Processing.Alignment.Core.CostComputers;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;

namespace Sdl.Core.Processing.Alignment.Core.Anchoring
{
	internal class DocumentStructureAnchorSearch : AbstractAnchoringStrategy
	{
		public const double AcceptanceTreshold = 0.2;

		public const char StructureElementMarker = '#';

		public const char PathDelimiter = '\\';

		public bool CancelProcessing
		{
			get;
			set;
		}

		public event EventHandler<ProgressEventArgs> OnProgress;

		public override IList<AlignmentData> GetAnchors(IList<AlignmentElement> leftInputElements, IList<AlignmentElement> rightInputElements)
		{
			if (leftInputElements == null)
			{
				throw new ArgumentNullException("leftInputElements");
			}
			if (rightInputElements == null)
			{
				throw new ArgumentNullException("rightInputElements");
			}
			List<AlignmentData> list = new List<AlignmentData>();
			DocumentStructureCostComputer documentStructureCostComputer = new DocumentStructureCostComputer('\\', '#');
			int num = 0;
			int num2 = -1;
			while (num < leftInputElements.Count)
			{
				AlignmentElement alignmentElement = leftInputElements[num++];
				int num3 = num2 + 1;
				while (num3 < rightInputElements.Count)
				{
					AlignmentElement alignmentElement2 = rightInputElements[num3++];
					AlignmentCost alignmentCost = documentStructureCostComputer.GetAlignmentCost(new AlignmentElement[1]
					{
						alignmentElement
					}, new AlignmentElement[1]
					{
						alignmentElement2
					});
					if ((double)alignmentCost <= 0.2)
					{
						list.Add(new AlignmentData(new List<DocumentSegmentId>
						{
							alignmentElement.Id
						}, new List<DocumentSegmentId>
						{
							alignmentElement2.Id
						}, alignmentCost));
						num2 = num3;
						break;
					}
				}
				if (this.OnProgress != null)
				{
					this.OnProgress(this, new ProgressEventArgs(0));
				}
				if (CancelProcessing)
				{
					return new List<AlignmentData>();
				}
			}
			return list;
		}

		public static bool SupportsAlignmentAlgorithm(AlignmentAlgorithmType alignmentAlgorithmType)
		{
			if ((uint)(alignmentAlgorithmType - 2) <= 4u)
			{
				return true;
			}
			return false;
		}
	}
}
