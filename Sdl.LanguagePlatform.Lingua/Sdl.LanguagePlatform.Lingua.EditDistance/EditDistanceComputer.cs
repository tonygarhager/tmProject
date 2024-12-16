using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.EditDistance;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.LanguagePlatform.Lingua.EditDistance
{
	public class EditDistanceComputer<T>
	{
		private struct MatrixItem
		{
			public double Score;

			public EditOperation Operation;

			public double Similarity;

			public MatrixItem(double score, EditOperation op, double similarity)
			{
				Score = score;
				Operation = op;
				Similarity = similarity;
			}
		}

		private readonly SimilarityComputer<T> _similarityComputer;

		private double _insertDeleteCosts;

		private double _moveCosts;

		private readonly bool _applySmallChangeAdjustment = true;

		public double SimilarityThreshold
		{
			get;
			set;
		}

		public double InsertDeleteCosts
		{
			get
			{
				return _insertDeleteCosts;
			}
			set
			{
				if (value < 0.0)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				_insertDeleteCosts = value;
			}
		}

		public double MoveCosts
		{
			get
			{
				return _moveCosts;
			}
			set
			{
				if (value < 0.0)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				_moveCosts = value;
				ComputeMoveOperations = (_moveCosts > 0.0);
			}
		}

		public bool ComputeMoveOperations
		{
			get;
			set;
		}

		public EditDistanceComputer(SimilarityComputer<T> similarityComputer, double insertDeleteCosts, double moveCosts)
		{
			_similarityComputer = similarityComputer;
			if (insertDeleteCosts < 0.0)
			{
				throw new ArgumentOutOfRangeException("insertDeleteCosts");
			}
			if (moveCosts < 0.0)
			{
				throw new ArgumentOutOfRangeException("moveCosts");
			}
			_insertDeleteCosts = insertDeleteCosts;
			_moveCosts = moveCosts;
			SimilarityThreshold = 0.85;
			ComputeMoveOperations = (moveCosts > 0.0);
		}

		public EditDistanceComputer(SimilarityComputer<T> similarityComputer)
			: this(similarityComputer, 1.0, 0.0)
		{
		}

		public EditDistanceComputer(SimilarityComputer<T> similarityComputer, bool applySmallChangeAdjustment)
			: this(similarityComputer, 1.0, 0.0)
		{
			_applySmallChangeAdjustment = applySmallChangeAdjustment;
		}

		public Sdl.LanguagePlatform.Core.EditDistance.EditDistance ComputeEditDistance(IList<T> sourceObjects, IList<T> targetObjects)
		{
			return ComputeEditDistance(sourceObjects, targetObjects, null);
		}

		public Sdl.LanguagePlatform.Core.EditDistance.EditDistance ComputeEditDistance(IList<T> sourceObjects, IList<T> targetObjects, List<Pair<int>> precomputedAssociations)
		{
			if (sourceObjects == null)
			{
				throw new ArgumentNullException("sourceObjects");
			}
			if (targetObjects == null)
			{
				throw new ArgumentNullException("targetObjects");
			}
			if (precomputedAssociations != null && !SortAndValidate(precomputedAssociations, sourceObjects.Count, targetObjects.Count))
			{
				precomputedAssociations = null;
			}
			Sdl.LanguagePlatform.Core.EditDistance.EditDistance editDistance = new Sdl.LanguagePlatform.Core.EditDistance.EditDistance(sourceObjects.Count, targetObjects.Count, 0.0);
			MatrixItem[,] array = new MatrixItem[sourceObjects.Count + 1, targetObjects.Count + 1];
			bool flag = precomputedAssociations != null;
			array[0, 0] = new MatrixItem(0.0, EditOperation.Identity, 0.0);
			int i;
			for (i = 1; i <= sourceObjects.Count; i++)
			{
				array[i, 0] = new MatrixItem((double)i * _insertDeleteCosts, EditOperation.Delete, 0.0);
			}
			int j;
			for (j = 1; j <= targetObjects.Count; j++)
			{
				array[0, j] = new MatrixItem((double)j * _insertDeleteCosts, EditOperation.Insert, 0.0);
			}
			for (i = 1; i <= sourceObjects.Count; i++)
			{
				for (j = 1; j <= targetObjects.Count; j++)
				{
					array[i, j] = new MatrixItem(0.0, EditOperation.Identity, 0.0);
				}
			}
			for (i = 1; i <= sourceObjects.Count; i++)
			{
				T a = sourceObjects[i - 1];
				int num = flag ? GetSourcePreassignment(i - 1, precomputedAssociations) : (-1);
				for (j = 1; j <= targetObjects.Count; j++)
				{
					T b = targetObjects[j - 1];
					double num2 = (num >= 0 && num != j - 1) ? (-1.0) : _similarityComputer(a, b);
					double num3 = (num2 < 0.0) ? 100000.0 : (array[i - 1, j - 1].Score + (1.0 - num2));
					double num4 = array[i, j - 1].Score + _insertDeleteCosts;
					double num5 = array[i - 1, j].Score + _insertDeleteCosts;
					double num6 = Math.Min(Math.Min(num3, num5), num4);
					array[i, j].Score = num6;
					array[i, j].Similarity = num2;
					if (num6 == num5)
					{
						array[i, j].Operation = EditOperation.Delete;
					}
					else if (num6 == num4)
					{
						array[i, j].Operation = EditOperation.Insert;
					}
					else if (num6 == num3)
					{
						array[i, j].Operation = ((num2 != 1.0) ? EditOperation.Change : EditOperation.Identity);
					}
				}
			}
			i = sourceObjects.Count;
			j = targetObjects.Count;
			editDistance.Distance = array[i, j].Score;
			while (i > 0 || j > 0)
			{
				EditDistanceItem editDistanceItem = default(EditDistanceItem);
				editDistanceItem.Resolution = EditDistanceResolution.None;
				editDistanceItem.Operation = array[i, j].Operation;
				EditDistanceItem item = editDistanceItem;
				switch (item.Operation)
				{
				case EditOperation.Identity:
					i--;
					j--;
					item.Costs = 0.0;
					break;
				case EditOperation.Change:
					item.Costs = 1.0 - array[i, j].Similarity;
					i--;
					j--;
					break;
				case EditOperation.Insert:
					j--;
					item.Costs = _insertDeleteCosts;
					break;
				case EditOperation.Delete:
					item.Costs = _insertDeleteCosts;
					i--;
					break;
				}
				item.Source = i;
				item.Target = j;
				editDistance.AddAtStart(item);
			}
			if (_applySmallChangeAdjustment && sourceObjects.Count + targetObjects.Count <= 6 && (sourceObjects.Count - targetObjects.Count <= 1 || sourceObjects.Count - targetObjects.Count >= -1) && editDistance.Distance.Equals(1.0))
			{
				editDistance.Distance = 0.3;
			}
			int num7 = 0;
			if (ComputeMoveOperations)
			{
				for (int k = 0; k < editDistance.Items.Count; k++)
				{
					EditOperation operation = editDistance[k].Operation;
					if (operation != EditOperation.Delete && operation != EditOperation.Insert)
					{
						continue;
					}
					int source = 0;
					int target = 0;
					int moveSourceTarget = 0;
					int moveTargetSource = 0;
					int l;
					for (l = k + 1; l < editDistance.Items.Count; l++)
					{
						if (editDistance[l].Operation == EditOperation.Insert && operation == EditOperation.Delete && array[editDistance[k].Source + 1, editDistance[l].Target + 1].Similarity >= SimilarityThreshold)
						{
							source = editDistance[k].Source;
							moveSourceTarget = editDistance[k].Target;
							target = editDistance[l].Target;
							moveTargetSource = editDistance[l].Source;
							break;
						}
						if (editDistance[l].Operation == EditOperation.Delete && operation == EditOperation.Insert && array[editDistance[l].Source + 1, editDistance[k].Target + 1].Similarity >= SimilarityThreshold)
						{
							source = editDistance[l].Source;
							moveSourceTarget = editDistance[l].Target;
							target = editDistance[k].Target;
							moveTargetSource = editDistance[k].Source;
							break;
						}
					}
					if (l < editDistance.Items.Count)
					{
						EditDistanceItem value = editDistance[k];
						value.Operation = EditOperation.Move;
						value.Source = source;
						value.Target = target;
						value.MoveSourceTarget = moveSourceTarget;
						value.MoveTargetSource = moveTargetSource;
						editDistance.Items[k] = value;
						editDistance.Items.RemoveAt(l);
						num7++;
					}
				}
			}
			if (num7 > 0)
			{
				editDistance.Distance -= (double)num7 * (2.0 * _insertDeleteCosts);
				editDistance.Distance += (double)num7 * _moveCosts;
			}
			return editDistance;
		}

		private static int GetSourcePreassignment(int st, IReadOnlyCollection<Pair<int>> precomputedAssociations)
		{
			if (precomputedAssociations == null || precomputedAssociations.Count == 0)
			{
				return -1;
			}
			foreach (Pair<int> precomputedAssociation in precomputedAssociations)
			{
				if (precomputedAssociation.Left > st)
				{
					return -1;
				}
				if (precomputedAssociation.Left == st)
				{
					return precomputedAssociation.Right;
				}
			}
			return -1;
		}

		private static bool SortAndValidate(List<Pair<int>> precomputedAssociations, int srcObjCount, int trgObjCount)
		{
			bool flag = !precomputedAssociations.Any((Pair<int> p) => p.Left < 0 || p.Left >= srcObjCount || p.Right < 0 || p.Right >= trgObjCount);
			if (flag)
			{
				precomputedAssociations.Sort(delegate(Pair<int> a, Pair<int> b)
				{
					int num = a.Left - b.Left;
					if (num == 0)
					{
						num = a.Right - b.Right;
					}
					return num;
				});
			}
			return flag;
		}
	}
}
