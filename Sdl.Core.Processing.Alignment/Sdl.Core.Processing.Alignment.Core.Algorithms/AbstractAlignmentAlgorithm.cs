using Sdl.Core.Processing.Alignment.Core.CostComputers;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Sdl.Core.Processing.Alignment.Core.Algorithms
{
	internal abstract class AbstractAlignmentAlgorithm : IAlignmentAlgorithm
	{
		protected readonly AlignmentAlgorithmSettings AlignmentAlgorithmSettingsField;

		private const int NoOverride = 0;

		private bool _cancelProcessing;

		private byte _percentageProgress;

		public AlignmentAlgorithmSettings AlignmentAlgorithmSettings => AlignmentAlgorithmSettingsField;

		public CultureInfo SourceCulture => AlignmentAlgorithmSettingsField.LeftCulture;

		public CultureInfo TargetCulture => AlignmentAlgorithmSettingsField.RightCulture;

		public virtual int DiagonalThicknessOverride
		{
			get;
			private set;
		}

		public bool CancelProcessing
		{
			get
			{
				return _cancelProcessing;
			}
			set
			{
				_cancelProcessing = value;
			}
		}

		public event EventHandler<ProgressEventArgs> OnProgress;

		protected AbstractAlignmentAlgorithm(AlignmentAlgorithmSettings alignmentAlgorithmSettings)
		{
			AlignmentAlgorithmSettingsField = alignmentAlgorithmSettings;
			DiagonalThicknessOverride = 0;
		}

		internal abstract IAlignmentCostComputer GetAlignmentCostComputer();

		public virtual IList<AlignmentData> Align(IList<AlignmentElement> leftInputElements, IList<AlignmentElement> rightInputElements)
		{
			_percentageProgress = 0;
			IAlignmentCostComputer alignmentCostComputer = GetAlignmentCostComputer();
			IAlignmentArray initialElements = GetInitialElements(leftInputElements, rightInputElements, alignmentCostComputer);
			for (int i = 1; i < initialElements.Width; i++)
			{
				for (int j = 1; j < initialElements.Height; j++)
				{
					if (_cancelProcessing)
					{
						return new List<AlignmentData>();
					}
					if (initialElements.IsValid(i, j))
					{
						AlignmentArrayElement elementWithMinimumCost = null;
						elementWithMinimumCost = (initialElements[i, j] = GetElementWithMinimumCost(leftInputElements, rightInputElements, initialElements, i, j, alignmentCostComputer, elementWithMinimumCost));
					}
					ReportProgress(ProgressEstimator.CalculateProgressInArray(j, i, initialElements.Width, initialElements.Height));
				}
			}
			ReportProgress(100);
			return GetAlignmentsFromElements(initialElements, leftInputElements, rightInputElements);
		}

		internal virtual bool IsOperationAllowed(IAlignmentArray elements, int i, int j, AlignmentOperation op)
		{
			switch (op)
			{
			case AlignmentOperation.Deletion:
				return elements.IsValidAndDefined(i, j - 1);
			case AlignmentOperation.Insertion:
				return elements.IsValidAndDefined(i - 1, j);
			case AlignmentOperation.Substitution:
				return elements.IsValidAndDefined(i - 1, j - 1);
			case AlignmentOperation.Expansion:
				if (j > 1)
				{
					return elements.IsValidAndDefined(i - 1, j - 2);
				}
				return false;
			case AlignmentOperation.Contraction:
				if (i > 1)
				{
					return elements.IsValidAndDefined(i - 2, j - 1);
				}
				return false;
			case AlignmentOperation.Melding:
				if (i > 1 && j > 1)
				{
					return elements.IsValidAndDefined(i - 2, j - 2);
				}
				return false;
			case AlignmentOperation.MeldingCross:
				return false;
			default:
				throw new InvalidOperationException("Unknown enum");
			}
		}

		private IAlignmentArray GetInitialElements(IList<AlignmentElement> leftInputElements, IList<AlignmentElement> rightInputElements, IAlignmentCostComputer alignmentCostComputer)
		{
			int num = leftInputElements.Count + 1;
			int num2 = rightInputElements.Count + 1;
			IAlignmentArray alignmentArray = AlignmentArrayFactory.CreateAlignmentElementArray(num, num2, DiagonalThicknessOverride);
			alignmentArray[0, 0] = new AlignmentArrayElement(AlignmentCost.MinValue, 0.0, AlignmentOperation.Substitution);
			for (int i = 1; i < num; i++)
			{
				if (alignmentArray.IsValid(i, 0))
				{
					AlignmentCost alignmentCost = alignmentCostComputer.GetAlignmentCost(new List<AlignmentElement>
					{
						leftInputElements[i - 1]
					}, new List<AlignmentElement>());
					double totalCost = alignmentArray[i - 1, 0].TotalCost + (double)alignmentCost;
					AlignmentArrayElement alignmentArrayElement2 = alignmentArray[i, 0] = new AlignmentArrayElement(alignmentCost, totalCost, AlignmentOperation.Insertion);
				}
			}
			for (int j = 1; j < num2; j++)
			{
				if (alignmentArray.IsValid(0, j))
				{
					AlignmentCost alignmentCost2 = alignmentCostComputer.GetAlignmentCost(new List<AlignmentElement>(), new List<AlignmentElement>
					{
						rightInputElements[j - 1]
					});
					double totalCost2 = alignmentArray[0, j - 1].TotalCost + (double)alignmentCost2;
					AlignmentArrayElement alignmentArrayElement4 = alignmentArray[0, j] = new AlignmentArrayElement(alignmentCost2, totalCost2, AlignmentOperation.Deletion);
				}
			}
			return alignmentArray;
		}

		internal virtual AlignmentArrayElement GetElementWithMinimumCost(IList<AlignmentElement> leftInputElements, IList<AlignmentElement> rightInputElements, IAlignmentArray elements, int i, int j, IAlignmentCostComputer alignmentCostComputer, AlignmentArrayElement elementWithMinimumCost)
		{
			if (IsOperationAllowed(elements, i, j, AlignmentOperation.Deletion))
			{
				AlignmentCost alignmentCost = alignmentCostComputer.GetAlignmentCost(new List<AlignmentElement>(), new List<AlignmentElement>
				{
					rightInputElements[j - 1]
				});
				double totalCost = elements[i, j - 1].TotalCost + (double)alignmentCost;
				elementWithMinimumCost = new AlignmentArrayElement(alignmentCost, totalCost, AlignmentOperation.Deletion);
			}
			if (IsOperationAllowed(elements, i, j, AlignmentOperation.Insertion))
			{
				AlignmentCost alignmentCost2 = alignmentCostComputer.GetAlignmentCost(new List<AlignmentElement>
				{
					leftInputElements[i - 1]
				}, new List<AlignmentElement>());
				double num = elements[i - 1, j].TotalCost + (double)alignmentCost2;
				if (elementWithMinimumCost == null || elementWithMinimumCost.TotalCost > num)
				{
					elementWithMinimumCost = new AlignmentArrayElement(alignmentCost2, num, AlignmentOperation.Insertion);
				}
			}
			if (IsOperationAllowed(elements, i, j, AlignmentOperation.Substitution))
			{
				AlignmentCost alignmentCost3 = alignmentCostComputer.GetAlignmentCost(new List<AlignmentElement>
				{
					leftInputElements[i - 1]
				}, new List<AlignmentElement>
				{
					rightInputElements[j - 1]
				});
				double num2 = elements[i - 1, j - 1].TotalCost + (double)alignmentCost3;
				if (elementWithMinimumCost == null || elementWithMinimumCost.TotalCost > num2)
				{
					elementWithMinimumCost = new AlignmentArrayElement(alignmentCost3, num2, AlignmentOperation.Substitution);
				}
			}
			if (IsOperationAllowed(elements, i, j, AlignmentOperation.Expansion))
			{
				CalculateExpansionMinimumCost(leftInputElements, rightInputElements, elements, i, j, alignmentCostComputer, ref elementWithMinimumCost);
			}
			if (IsOperationAllowed(elements, i, j, AlignmentOperation.Contraction))
			{
				CalculateContractionMinimumCost(leftInputElements, rightInputElements, elements, i, j, alignmentCostComputer, ref elementWithMinimumCost);
			}
			if (IsOperationAllowed(elements, i, j, AlignmentOperation.Melding))
			{
				CalculateMeldingMinimumCost(leftInputElements, rightInputElements, elements, i, j, alignmentCostComputer, ref elementWithMinimumCost);
			}
			if (IsOperationAllowed(elements, i, j, AlignmentOperation.MeldingCross))
			{
				CalculateMeldingCrossMinimumCost(leftInputElements, rightInputElements, elements, i, j, alignmentCostComputer, ref elementWithMinimumCost);
			}
			return elementWithMinimumCost;
		}

		internal virtual void CalculateContractionMinimumCost(IList<AlignmentElement> leftInputElements, IList<AlignmentElement> rightInputElements, IAlignmentArray elements, int i, int j, IAlignmentCostComputer alignmentCostComputer, ref AlignmentArrayElement elementWithMinimumCost)
		{
			AlignmentCost alignmentCost = alignmentCostComputer.GetAlignmentCost(new List<AlignmentElement>
			{
				leftInputElements[i - 2],
				leftInputElements[i - 1]
			}, new List<AlignmentElement>
			{
				rightInputElements[j - 1]
			});
			double num = elements[i - 2, j - 1].TotalCost + (double)alignmentCost;
			if (elementWithMinimumCost == null || elementWithMinimumCost.TotalCost > num)
			{
				elementWithMinimumCost = new AlignmentArrayElement(alignmentCost, num, AlignmentOperation.Contraction);
			}
		}

		internal virtual void CalculateExpansionMinimumCost(IList<AlignmentElement> leftInputElements, IList<AlignmentElement> rightInputElements, IAlignmentArray elements, int i, int j, IAlignmentCostComputer alignmentCostComputer, ref AlignmentArrayElement elementWithMinimumCost)
		{
			AlignmentCost alignmentCost = alignmentCostComputer.GetAlignmentCost(new List<AlignmentElement>
			{
				leftInputElements[i - 1]
			}, new List<AlignmentElement>
			{
				rightInputElements[j - 2],
				rightInputElements[j - 1]
			});
			double num = elements[i - 1, j - 2].TotalCost + (double)alignmentCost;
			if (elementWithMinimumCost == null || elementWithMinimumCost.TotalCost > num)
			{
				elementWithMinimumCost = new AlignmentArrayElement(alignmentCost, num, AlignmentOperation.Expansion);
			}
		}

		internal virtual void CalculateMeldingMinimumCost(IList<AlignmentElement> leftInputElements, IList<AlignmentElement> rightInputElements, IAlignmentArray elements, int i, int j, IAlignmentCostComputer alignmentCostComputer, ref AlignmentArrayElement elementWithMinimumCost)
		{
			AlignmentCost alignmentCost = alignmentCostComputer.GetAlignmentCost(new List<AlignmentElement>
			{
				leftInputElements[i - 2],
				leftInputElements[i - 1]
			}, new List<AlignmentElement>
			{
				rightInputElements[j - 2],
				rightInputElements[j - 1]
			});
			double num = elements[i - 2, j - 2].TotalCost + (double)alignmentCost;
			if (elementWithMinimumCost == null || elementWithMinimumCost.TotalCost > num)
			{
				elementWithMinimumCost = new AlignmentArrayElement(alignmentCost, num, AlignmentOperation.Melding);
			}
		}

		internal virtual void CalculateMeldingCrossMinimumCost(IList<AlignmentElement> leftInputElements, IList<AlignmentElement> rightInputElements, IAlignmentArray elements, int i, int j, IAlignmentCostComputer alignmentCostComputer, ref AlignmentArrayElement elementWithMinimumCost)
		{
			throw new NotSupportedException("aligment cross is not supported for classic alignment");
		}

		internal virtual List<AlignmentData> GetAlignmentsFromElements(IAlignmentArray costMatrix, IList<AlignmentElement> leftElements, IList<AlignmentElement> rightElements)
		{
			LinkedList<AlignmentData> linkedList = new LinkedList<AlignmentData>();
			int i = costMatrix.Width - 1;
			int j = costMatrix.Height - 1;
			while (i != 0 || j != 0)
			{
				AlignmentOperation operation = costMatrix[i, j].Operation;
				AlignmentCost elementCost = costMatrix[i, j].ElementCost;
				int expansionContractionFactor = costMatrix[i, j].ExpansionContractionFactor;
				switch (operation)
				{
				case AlignmentOperation.Deletion:
				{
					AlignmentData alignmentData4 = new AlignmentData();
					alignmentData4.RightIds.Add(rightElements[j - 1].Id);
					alignmentData4.Cost = elementCost;
					linkedList.AddFirst(alignmentData4);
					j--;
					break;
				}
				case AlignmentOperation.Insertion:
				{
					AlignmentData alignmentData3 = new AlignmentData();
					alignmentData3.LeftIds.Add(leftElements[i - 1].Id);
					alignmentData3.Cost = elementCost;
					linkedList.AddFirst(alignmentData3);
					i--;
					break;
				}
				case AlignmentOperation.Substitution:
				{
					AlignmentData alignmentData2 = new AlignmentData();
					alignmentData2.LeftIds.Add(leftElements[i - 1].Id);
					alignmentData2.RightIds.Add(rightElements[j - 1].Id);
					alignmentData2.Cost = elementCost;
					linkedList.AddFirst(alignmentData2);
					i--;
					j--;
					break;
				}
				case AlignmentOperation.Expansion:
					AddExpansionAlignment(leftElements, rightElements, elementCost, linkedList, ref i, ref j, expansionContractionFactor);
					break;
				case AlignmentOperation.Contraction:
					AddContractionAlignment(leftElements, rightElements, elementCost, linkedList, ref i, ref j, expansionContractionFactor);
					break;
				case AlignmentOperation.Melding:
				{
					AlignmentData alignmentData = new AlignmentData();
					alignmentData.LeftIds.Add(leftElements[i - 2].Id);
					alignmentData.LeftIds.Add(leftElements[i - 1].Id);
					alignmentData.RightIds.Add(rightElements[j - 2].Id);
					alignmentData.RightIds.Add(rightElements[j - 1].Id);
					alignmentData.Cost = elementCost;
					linkedList.AddFirst(alignmentData);
					i -= 2;
					j -= 2;
					break;
				}
				case AlignmentOperation.MeldingCross:
					AddMeldingCrossAlignment(leftElements, rightElements, elementCost, linkedList, ref i, ref j);
					break;
				}
			}
			return linkedList.ToList();
		}

		internal virtual void AddExpansionAlignment(IList<AlignmentElement> leftElements, IList<AlignmentElement> rightElements, AlignmentCost elementCost, LinkedList<AlignmentData> alignments, ref int i, ref int j, int factor)
		{
			AlignmentData alignmentData = new AlignmentData();
			alignmentData.LeftIds.Add(leftElements[i - 1].Id);
			alignmentData.RightIds.Add(rightElements[j - 2].Id);
			alignmentData.RightIds.Add(rightElements[j - 1].Id);
			alignmentData.Cost = elementCost;
			alignments.AddFirst(alignmentData);
			i--;
			j -= 2;
		}

		internal virtual void AddContractionAlignment(IList<AlignmentElement> leftElements, IList<AlignmentElement> rightElements, AlignmentCost elementCost, LinkedList<AlignmentData> alignments, ref int i, ref int j, int factor)
		{
			AlignmentData alignmentData = new AlignmentData();
			alignmentData.LeftIds.Add(leftElements[i - 2].Id);
			alignmentData.LeftIds.Add(leftElements[i - 1].Id);
			alignmentData.RightIds.Add(rightElements[j - 1].Id);
			alignmentData.Cost = elementCost;
			alignments.AddFirst(alignmentData);
			i -= 2;
			j--;
		}

		internal virtual void AddMeldingCrossAlignment(IList<AlignmentElement> leftElements, IList<AlignmentElement> rightElements, AlignmentCost elementCost, LinkedList<AlignmentData> alignments, ref int i, ref int j)
		{
			throw new NotSupportedException("aligment cross is not supported for classic alignment");
		}

		private void ReportProgress(byte progress)
		{
			if (this.OnProgress != null && _percentageProgress < progress)
			{
				_percentageProgress = progress;
				this.OnProgress(this, new ProgressEventArgs(progress));
			}
		}
	}
}
