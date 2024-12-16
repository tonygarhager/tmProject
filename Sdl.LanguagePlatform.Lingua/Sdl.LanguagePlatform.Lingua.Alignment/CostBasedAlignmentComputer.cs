using System;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.Lingua.Alignment
{
	public class CostBasedAlignmentComputer<T>
	{
		private struct Cell
		{
			public int Cost;

			public AlignmentOperation Op;
		}

		private readonly IAlignmentCostComputer<T> _alignmentCostComputer;

		public CostBasedAlignmentComputer(IAlignmentCostComputer<T> alignmentCostComputer)
		{
			_alignmentCostComputer = (alignmentCostComputer ?? throw new ArgumentNullException());
		}

		public List<AlignmentItem> Align(IList<T> srcItems, IList<T> trgItems)
		{
			return Align(srcItems, trgItems, null);
		}

		private List<AlignmentItem> Align(IList<T> srcItems, IList<T> trgItems, List<AlignmentItem> fixpoints)
		{
			int count = srcItems.Count;
			int count2 = trgItems.Count;
			Cell[,] array = new Cell[count + 1, count2 + 1];
			int i;
			int j;
			if (fixpoints != null)
			{
				foreach (AlignmentItem fixpoint in fixpoints)
				{
					if (fixpoint.SourceFrom < 0 || fixpoint.SourceFrom >= count)
					{
						throw new ArgumentOutOfRangeException("fixpoints");
					}
					if (fixpoint.SourceUpto < 0 || fixpoint.SourceUpto > count || fixpoint.SourceUpto <= fixpoint.SourceFrom)
					{
						throw new ArgumentOutOfRangeException("fixpoints");
					}
					if (fixpoint.TargetFrom < 0 || fixpoint.TargetFrom >= count)
					{
						throw new ArgumentOutOfRangeException("fixpoints");
					}
					if (fixpoint.TargetUpto < 0 || fixpoint.TargetUpto > count || fixpoint.TargetUpto <= fixpoint.TargetFrom)
					{
						throw new ArgumentOutOfRangeException("fixpoints");
					}
					for (i = 0; i < count; i++)
					{
						for (j = 0; j < count2; j++)
						{
							if (i >= fixpoint.SourceFrom && i < fixpoint.SourceUpto && j >= fixpoint.TargetFrom && j < fixpoint.TargetUpto)
							{
								array[i + 1, j + 1].Op = AlignmentOperation.Substitute;
								array[i + 1, j + 1].Cost = _alignmentCostComputer.GetSubstitutionCosts(srcItems[i], trgItems[j]);
							}
							else
							{
								array[i + 1, j + 1].Op = AlignmentOperation.Invalid;
							}
						}
					}
				}
			}
			Cell cell = default(Cell);
			cell.Cost = 0;
			cell.Op = AlignmentOperation.None;
			array[0, 0] = cell;
			for (j = 0; j <= count2; j++)
			{
				for (i = 0; i <= count; i++)
				{
					if (array[i, j].Op != 0)
					{
						continue;
					}
					int num = int.MaxValue;
					AlignmentOperation op = AlignmentOperation.None;
					if (i == 0 && j == 0)
					{
						num = 0;
						op = AlignmentOperation.None;
					}
					if (i > 0 && j > 0 && array[i - 1, j - 1].Op != AlignmentOperation.Invalid)
					{
						int num2 = array[i - 1, j - 1].Cost + _alignmentCostComputer.GetSubstitutionCosts(srcItems[i - 1], trgItems[j - 1]);
						if (num2 < num)
						{
							num = num2;
							op = AlignmentOperation.Substitute;
						}
					}
					if (i > 0 && array[i - 1, j].Op != AlignmentOperation.Invalid)
					{
						int num2 = array[i - 1, j].Cost + _alignmentCostComputer.GetDeletionCosts(srcItems[i - 1]);
						if (num2 < num)
						{
							num = num2;
							op = AlignmentOperation.Delete;
						}
					}
					if (j > 0 && array[i, j - 1].Op != AlignmentOperation.Invalid)
					{
						int num2 = array[i, j - 1].Cost + _alignmentCostComputer.GetInsertionCosts(trgItems[j - 1]);
						if (num2 < num)
						{
							num = num2;
							op = AlignmentOperation.Insert;
						}
					}
					if (i > 1 && j > 0 && array[i - 2, j - 1].Op != AlignmentOperation.Invalid)
					{
						int num2 = array[i - 2, j - 1].Cost + _alignmentCostComputer.GetContractionCosts(srcItems[i - 2], srcItems[i - 1], trgItems[j - 1]);
						if (num2 < num)
						{
							num = num2;
							op = AlignmentOperation.Contract;
						}
					}
					if (i > 0 && j > 1 && array[i - 1, j - 2].Op != AlignmentOperation.Invalid)
					{
						int num2 = array[i - 1, j - 2].Cost + _alignmentCostComputer.GetExpansionCosts(srcItems[i - 1], trgItems[j - 2], trgItems[j - 1]);
						if (num2 < num)
						{
							num = num2;
							op = AlignmentOperation.Expand;
						}
					}
					if (i > 1 && j > 1 && array[i - 2, j - 2].Op != AlignmentOperation.Invalid)
					{
						int num2 = array[i - 2, j - 2].Cost + _alignmentCostComputer.GetMeldingCosts(srcItems[i - 2], srcItems[i - 1], trgItems[j - 2], trgItems[j - 1]);
						if (num2 < num)
						{
							num = num2;
							op = AlignmentOperation.Merge;
						}
					}
					cell.Cost = num;
					cell.Op = op;
					array[i, j] = cell;
				}
			}
			if (fixpoints != null)
			{
				throw new NotImplementedException("Alignment path readout does not yet work with fixpoints");
			}
			List<AlignmentItem> list = new List<AlignmentItem>();
			i = count;
			j = count2;
			AlignmentItem item = default(AlignmentItem);
			while (i > 0 || j > 0)
			{
				item.Op = array[i, j].Op;
				item.SourceUpto = i;
				item.TargetUpto = j;
				switch (array[i, j].Op)
				{
				case AlignmentOperation.Substitute:
					i--;
					j--;
					break;
				case AlignmentOperation.Insert:
					j--;
					break;
				case AlignmentOperation.Delete:
					i--;
					break;
				case AlignmentOperation.Contract:
					i -= 2;
					j--;
					break;
				case AlignmentOperation.Expand:
					i--;
					j -= 2;
					break;
				case AlignmentOperation.Merge:
					i -= 2;
					j -= 2;
					break;
				default:
					throw new Exception("Unexpected");
				}
				item.SourceFrom = i;
				item.TargetFrom = j;
				list.Insert(0, item);
			}
			return list;
		}
	}
}
