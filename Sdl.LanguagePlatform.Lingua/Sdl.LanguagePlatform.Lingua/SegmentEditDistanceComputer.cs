using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.EditDistance;
using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.UI;

namespace Sdl.LanguagePlatform.Lingua
{
	public class SegmentEditDistanceComputer
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

		private const double InsertDeleteCosts = 1.0;

		private const double MoveCosts = 1.1;

		private const double MoveSimThreshold = 0.95;

		private const double InvalidAssignmentCosts = 100000.0;

		private const bool UseStringEditDistance = false;

		private bool _charactersNormalizeSafely = true;

		private bool _applySmallChangeAdjustment = true;

		private readonly bool _computeMoves;

		public static bool CanComputeEditDistance(int sourceTokenCount, int targetTokenCount)
		{
			return sourceTokenCount * targetTokenCount <= 4194304;
		}

		public SegmentEditDistanceComputer()
		{
			_computeMoves = true;
		}

		public SegmentEditDistanceComputer(bool computeMoves)
		{
			_computeMoves = computeMoves;
		}

		public Sdl.LanguagePlatform.Core.EditDistance.EditDistance ComputeEditDistance(IList<Token> sourceTokens, IList<Token> targetTokens, bool computeDiagonalOnly, BuiltinRecognizers disabledAutoSubstitutions, out TagAssociations alignedTags, bool charactersNormalizeSafely = true, bool applySmallChangeAdjustment = true)
		{
			return ComputeEditDistance(sourceTokens, targetTokens, computeDiagonalOnly, disabledAutoSubstitutions, out alignedTags, charactersNormalizeSafely, applySmallChangeAdjustment, diagonalOnly: false);
		}

		public Sdl.LanguagePlatform.Core.EditDistance.EditDistance ComputeEditDistance(IList<Token> sourceTokens, IList<Token> targetTokens, bool computeDiagonalOnly, BuiltinRecognizers disabledAutoSubstitutions, out TagAssociations alignedTags, bool charactersNormalizeSafely, bool applySmallChangeAdjustment, bool diagonalOnly)
		{
			_charactersNormalizeSafely = charactersNormalizeSafely;
			_applySmallChangeAdjustment = applySmallChangeAdjustment;
			return ComputeEditDistanceImpl_Original(sourceTokens, targetTokens, disabledAutoSubstitutions, out alignedTags, diagonalOnly);
		}

		private static bool VerifyEditDistance(IEnumerable<EditDistanceItem> ed, int sourceObjectCount, int targetObjectCount)
		{
			bool[] array = new bool[sourceObjectCount];
			bool[] array2 = new bool[targetObjectCount];
			foreach (EditDistanceItem item in ed)
			{
				switch (item.Operation)
				{
				case EditOperation.Identity:
				case EditOperation.Change:
				case EditOperation.Move:
					if (array[item.Source])
					{
						return false;
					}
					if (array2[item.Target])
					{
						return false;
					}
					array[item.Source] = true;
					array2[item.Target] = true;
					break;
				case EditOperation.Insert:
					if (array2[item.Target])
					{
						return false;
					}
					array2[item.Target] = true;
					break;
				case EditOperation.Delete:
					if (array[item.Source])
					{
						return false;
					}
					array[item.Source] = true;
					break;
				default:
					throw new Exception("Unexpected case");
				}
			}
			for (int i = 0; i < sourceObjectCount; i++)
			{
				if (!array[i])
				{
					return false;
				}
			}
			for (int j = 0; j < targetObjectCount; j++)
			{
				if (!array2[j])
				{
					return false;
				}
			}
			return true;
		}

		private static void ComputeEditDistanceMatrix_Full(MatrixItem[,] matrix, SimilarityMatrix sim, TagAssociations alignedTags)
		{
			for (int i = 1; i <= sim.SourceTokens.Count; i++)
			{
				for (int j = 1; j <= sim.TargetTokens.Count; j++)
				{
					double num = sim[i - 1, j - 1];
					double num2 = (num < 0.0) ? 100000.0 : (matrix[i - 1, j - 1].Score + (1.0 - num));
					double num3 = matrix[i, j - 1].Score + 1.0;
					double num4 = matrix[i - 1, j].Score + 1.0;
					double num5 = Math.Min(Math.Min(num2, num4), num3);
					EditOperation editOperation = EditOperation.Undefined;
					if (num5 == num4)
					{
						editOperation = EditOperation.Delete;
					}
					else if (num5 == num3)
					{
						editOperation = EditOperation.Insert;
					}
					else if (num5 == num2)
					{
						editOperation = ((num != 1.0) ? EditOperation.Change : EditOperation.Identity);
					}
					if (alignedTags != null && alignedTags.Count > 0)
					{
						EditOperation operationBySourcePosition = alignedTags.GetOperationBySourcePosition(i - 1);
						EditOperation operationByTargetPosition = alignedTags.GetOperationByTargetPosition(j - 1);
						if ((operationBySourcePosition == EditOperation.Insert || operationBySourcePosition == EditOperation.Delete) && editOperation != operationBySourcePosition)
						{
							editOperation = operationBySourcePosition;
						}
						else if ((operationByTargetPosition == EditOperation.Insert || operationByTargetPosition == EditOperation.Delete) && editOperation != operationByTargetPosition)
						{
							editOperation = operationByTargetPosition;
						}
					}
					matrix[i, j].Similarity = num;
					matrix[i, j].Operation = editOperation;
					switch (editOperation)
					{
					case EditOperation.Delete:
						matrix[i, j].Score = num4;
						break;
					case EditOperation.Insert:
						matrix[i, j].Score = num3;
						break;
					default:
						matrix[i, j].Score = num2;
						break;
					}
				}
			}
		}

		private static EditOperation GetOperation(double changeCosts, double insertCosts, double deleteCosts, double similarity)
		{
			double num = Math.Min(changeCosts, Math.Min(insertCosts, deleteCosts));
			if (num == changeCosts)
			{
				if (similarity != 1.0)
				{
					return EditOperation.Change;
				}
				return EditOperation.Identity;
			}
			if (num != deleteCosts)
			{
				return EditOperation.Insert;
			}
			return EditOperation.Delete;
		}

		private void ComputeCell(MatrixItem[,] matrix, SimilarityMatrix sim, int i, int j)
		{
			if (matrix[i, j].Operation != EditOperation.Undefined)
			{
				return;
			}
			ComputeCell(matrix, sim, i - 1, j - 1);
			double num = sim[i - 1, j - 1];
			double num2 = (num < 0.0) ? 100000.0 : (matrix[i - 1, j - 1].Score + (1.0 - num));
			double num3 = 0.0;
			double num4 = 0.0;
			EditOperation editOperation;
			if (i < j)
			{
				ComputeCell(matrix, sim, i, j - 1);
				num3 = matrix[i, j - 1].Score + 1.0;
				if (num3 >= num2 || num2 == 100000.0)
				{
					ComputeCell(matrix, sim, i - 1, j);
					num4 = matrix[i - 1, j].Score + 1.0;
					editOperation = GetOperation(num2, num3, num4, num);
				}
				else
				{
					editOperation = ((!(num3 < num2)) ? EditOperation.Change : EditOperation.Insert);
				}
			}
			else
			{
				ComputeCell(matrix, sim, i - 1, j);
				num4 = matrix[i - 1, j].Score + 1.0;
				if (num4 >= num2 || num2 == 100000.0)
				{
					ComputeCell(matrix, sim, i, j - 1);
					num3 = matrix[i, j - 1].Score + 1.0;
					editOperation = GetOperation(num2, num3, num4, num);
				}
				else
				{
					editOperation = ((!(num4 < num2)) ? EditOperation.Change : EditOperation.Delete);
				}
			}
			matrix[i, j].Similarity = num;
			matrix[i, j].Operation = editOperation;
			switch (editOperation)
			{
			case EditOperation.Delete:
				matrix[i, j].Score = num4;
				break;
			case EditOperation.Insert:
				matrix[i, j].Score = num3;
				break;
			default:
				matrix[i, j].Score = num2;
				break;
			}
		}

		private void ComputeEditDistanceMatrix_Lazy(MatrixItem[,] matrix, SimilarityMatrix sim)
		{
			ComputeCell(matrix, sim, sim.SourceTokens.Count, sim.TargetTokens.Count);
		}

		private static MatrixItem[,] CreateEditDistanceMatrix(ICollection<Token> sourceTokens, ICollection<Token> targetTokens)
		{
			MatrixItem[,] array = new MatrixItem[sourceTokens.Count + 1, targetTokens.Count + 1];
			array[0, 0] = new MatrixItem(0.0, EditOperation.Identity, 0.0);
			for (int i = 1; i <= sourceTokens.Count; i++)
			{
				array[i, 0] = new MatrixItem((double)i * 1.0, EditOperation.Delete, 0.0);
			}
			for (int j = 1; j <= targetTokens.Count; j++)
			{
				array[0, j] = new MatrixItem((double)j * 1.0, EditOperation.Insert, 0.0);
			}
			for (int i = 1; i <= sourceTokens.Count; i++)
			{
				for (int j = 1; j <= targetTokens.Count; j++)
				{
					array[i, j] = new MatrixItem(0.0, EditOperation.Undefined, 0.0);
				}
			}
			return array;
		}

		private Sdl.LanguagePlatform.Core.EditDistance.EditDistance ComputeEditDistanceImpl_Original(IList<Token> sourceTokens, IList<Token> targetTokens, BuiltinRecognizers disabledAutoSubstitutions, out TagAssociations alignedTags, bool diagonalOnly)
		{
			if (sourceTokens == null)
			{
				throw new ArgumentNullException("sourceTokens");
			}
			if (targetTokens == null)
			{
				throw new ArgumentNullException("targetTokens");
			}
			if (diagonalOnly && sourceTokens.Count != targetTokens.Count)
			{
				throw new Exception("SegmentEditDistanceComputer: diagonalOnly == true but sourceTokens.Count != targetTokens.Count");
			}
			alignedTags = null;
			Sdl.LanguagePlatform.Core.EditDistance.EditDistance editDistance = new Sdl.LanguagePlatform.Core.EditDistance.EditDistance(sourceTokens.Count, targetTokens.Count, 0.0);
			SimilarityMatrix similarityMatrix = new SimilarityMatrix(sourceTokens, targetTokens, useStringEditDistance: false, disabledAutoSubstitutions, _charactersNormalizeSafely, _applySmallChangeAdjustment);
			MatrixItem[,] array = CreateEditDistanceMatrix(sourceTokens, targetTokens);
			alignedTags = TagAligner.AlignPairedTags(sourceTokens, targetTokens, similarityMatrix);
			if (alignedTags != null && alignedTags.Count > 0)
			{
				PatchSimilarityMatrix(similarityMatrix, sourceTokens, targetTokens, alignedTags);
				ComputeEditDistanceMatrix_Full(array, similarityMatrix, alignedTags);
			}
			else if (diagonalOnly)
			{
				for (int i = 0; i <= sourceTokens.Count; i++)
				{
					array[i, i].Operation = EditOperation.Change;
					array[i, i].Similarity = 0.0;
				}
			}
			else
			{
				ComputeEditDistanceMatrix_Lazy(array, similarityMatrix);
			}
			int num = sourceTokens.Count;
			int num2 = targetTokens.Count;
			editDistance.Distance = array[num, num2].Score;
			while (num > 0 || num2 > 0)
			{
				EditDistanceItem editDistanceItem = default(EditDistanceItem);
				editDistanceItem.Resolution = EditDistanceResolution.None;
				EditDistanceItem item = editDistanceItem;
				MatrixItem matrixItem = array[num, num2];
				item.Operation = matrixItem.Operation;
				switch (item.Operation)
				{
				case EditOperation.Identity:
					item.Costs = 0.0;
					num--;
					num2--;
					break;
				case EditOperation.Change:
					item.Costs = 1.0 - SimilarityComputers.GetTokenSimilarity(sourceTokens[num - 1], targetTokens[num2 - 1], useStringEditDistance: true, disabledAutoSubstitutions, _charactersNormalizeSafely, _applySmallChangeAdjustment);
					if (diagonalOnly)
					{
						if (item.Costs < 0.001)
						{
							item.Operation = EditOperation.Identity;
						}
						else
						{
							editDistance.Distance += 1.0;
						}
					}
					num--;
					num2--;
					break;
				case EditOperation.Insert:
					item.Costs = 1.0;
					num2--;
					break;
				case EditOperation.Delete:
					item.Costs = 1.0;
					num--;
					break;
				case EditOperation.Undefined:
					throw new Exception("Internal ED computation error");
				}
				item.Source = num;
				item.Target = num2;
				editDistance.AddAtStart(item);
			}
			if (alignedTags != null && alignedTags.Count > 0)
			{
				FixTagActions(sourceTokens, targetTokens, editDistance, alignedTags);
			}
			if (_computeMoves)
			{
				int num3 = DetectMoves(editDistance, array);
				if (num3 > 0)
				{
					editDistance.Distance -= (double)num3 * 2.0;
					editDistance.Distance += (double)num3 * 1.1;
				}
			}
			return editDistance;
		}

		private static void PrintTagAlignments(string filenamePath, IList<Token> sourceTokens, IList<Token> targetTokens, TagAssociations alignedTags, Sdl.LanguagePlatform.Core.EditDistance.EditDistance result)
		{
			TextWriter textWriter = new StreamWriter(filenamePath, append: false, Encoding.UTF8);
			textWriter.WriteLine("Source objects:");
			for (int i = 0; i < sourceTokens.Count; i++)
			{
				textWriter.WriteLine("\t{0}:\t{1}", i, sourceTokens[i]);
			}
			textWriter.WriteLine();
			textWriter.WriteLine("Target objects:");
			for (int j = 0; j < targetTokens.Count; j++)
			{
				textWriter.WriteLine("\t{0}:\t{1}", j, targetTokens[j]);
			}
			textWriter.WriteLine();
			textWriter.WriteLine();
			if (alignedTags != null)
			{
				textWriter.WriteLine("Tag Alignment:");
				foreach (TagAssociation alignedTag in alignedTags)
				{
					textWriter.WriteLine("\t{0}", alignedTag);
				}
				textWriter.WriteLine();
				textWriter.WriteLine();
			}
			result.Dump(textWriter, "Final ED");
			textWriter.Close();
			textWriter.Dispose();
		}

		private static void PrintMatrixAndCheapestPath(string filenamePath, MatrixItem[,] matrix, IList<Token> sourceTokens, IList<Token> targetTokens, HashSet<string> resultPathItems, Sdl.LanguagePlatform.Core.EditDistance.EditDistance result, SimilarityMatrix sim)
		{
			StreamWriter writer = new StreamWriter(filenamePath, append: false, Encoding.UTF8);
			Html32TextWriter html32TextWriter = new Html32TextWriter(writer);
			html32TextWriter.WriteFullBeginTag("html");
			html32TextWriter.WriteFullBeginTag("body");
			PrintEdMatrix(html32TextWriter, matrix, sourceTokens, targetTokens, resultPathItems);
			html32TextWriter.WriteFullBeginTag("div style=\"float: left;\"");
			html32TextWriter.Write("================");
			html32TextWriter.WriteEndTag("div");
			PrintSimilarityMatrix(html32TextWriter, sim);
			html32TextWriter.WriteFullBeginTag("div style=\"clear: both;\"");
			html32TextWriter.WriteFullBeginTag("h2");
			html32TextWriter.Write("Result");
			html32TextWriter.WriteEndTag("h2");
			html32TextWriter.Write("Score = {0}", result.Distance);
			PrintEdResults(sourceTokens, targetTokens, result, html32TextWriter);
			html32TextWriter.WriteEndTag("div");
			html32TextWriter.WriteEndTag("body");
			html32TextWriter.WriteEndTag("html");
			html32TextWriter.Close();
		}

		private static void PrintEdMatrix(HtmlTextWriter htmlWriter, MatrixItem[,] matrix, IList<Token> sourceTokens, IList<Token> targetTokens, ICollection<string> resultPathItems)
		{
			htmlWriter.WriteFullBeginTag("table border=\"1\" style=\"float: left;\"");
			htmlWriter.WriteFullBeginTag("caption");
			htmlWriter.Write("Edit Distance Matrix");
			htmlWriter.WriteEndTag("caption");
			for (int i = -1; i <= sourceTokens.Count; i++)
			{
				htmlWriter.WriteFullBeginTag("tr");
				for (int j = -1; j <= targetTokens.Count; j++)
				{
					bool flag = resultPathItems.Contains($"{i},{j}") || (i == 0 && j == 0);
					htmlWriter.WriteFullBeginTag(flag ? "td bgcolor=\"Red\"" : "td");
					if (i < 0)
					{
						if (j >= 0)
						{
							htmlWriter.Write("t={0}", j);
							if (j > 0)
							{
								htmlWriter.WriteBreak();
								htmlWriter.WriteFullBeginTag("b");
								htmlWriter.Write(targetTokens[j - 1].ToString());
								htmlWriter.WriteEndTag("b");
							}
						}
					}
					else if (j < 0)
					{
						htmlWriter.Write("s={0}", i);
						if (i > 0)
						{
							htmlWriter.WriteBreak();
							htmlWriter.WriteFullBeginTag("b");
							htmlWriter.Write(sourceTokens[i - 1].ToString());
							htmlWriter.WriteEndTag("b");
						}
					}
					else
					{
						htmlWriter.Write("d={0}", matrix[i, j].Score);
						htmlWriter.WriteBreak();
						htmlWriter.Write("s={0}", matrix[i, j].Similarity);
						htmlWriter.WriteBreak();
						htmlWriter.Write("o={0}", matrix[i, j].Operation.ToString());
					}
					htmlWriter.WriteEndTag("td");
				}
				htmlWriter.WriteEndTag("tr");
			}
			htmlWriter.WriteEndTag("table");
		}

		private static void PrintSimilarityMatrix(HtmlTextWriter htmlWriter, SimilarityMatrix sim)
		{
			htmlWriter.WriteFullBeginTag("table border=\"1\" style=\"float: left;\"");
			htmlWriter.WriteFullBeginTag("caption");
			htmlWriter.Write("Similarity Matrix");
			htmlWriter.WriteEndTag("caption");
			for (int i = -1; i < sim.SourceTokens.Count; i++)
			{
				htmlWriter.WriteFullBeginTag("tr");
				for (int j = -1; j < sim.TargetTokens.Count; j++)
				{
					htmlWriter.WriteFullBeginTag("td WIDTH=50 HEIGHT=50 align=\"center\"");
					if (i < 0)
					{
						htmlWriter.Write("t={0}", j);
						if (j >= 0)
						{
							htmlWriter.WriteBreak();
							htmlWriter.WriteFullBeginTag("b");
							htmlWriter.Write(sim.TargetTokens[j].ToString());
							htmlWriter.WriteEndTag("b");
						}
					}
					else if (j < 0)
					{
						htmlWriter.Write("s={0}", i);
						if (i >= 0)
						{
							htmlWriter.WriteBreak();
							htmlWriter.WriteFullBeginTag("b");
							htmlWriter.Write(sim.SourceTokens[i].ToString());
							htmlWriter.WriteEndTag("b");
						}
					}
					else
					{
						htmlWriter.Write(sim[i, j]);
					}
					htmlWriter.WriteEndTag("td");
				}
				htmlWriter.WriteEndTag("tr");
			}
			htmlWriter.WriteEndTag("table");
		}

		private static void PrintEdResults(IList<Token> sourceTokens, IList<Token> targetTokens, Sdl.LanguagePlatform.Core.EditDistance.EditDistance result, HtmlTextWriter htmlWriter)
		{
			htmlWriter.WriteFullBeginTag("table border=\"1\"");
			htmlWriter.WriteFullBeginTag("caption");
			htmlWriter.Write("Edit Distance results");
			htmlWriter.WriteEndTag("caption");
			for (int i = -1; i < 2; i++)
			{
				htmlWriter.WriteFullBeginTag("tr");
				for (int j = -1; j < result.Items.Count; j++)
				{
					htmlWriter.WriteFullBeginTag("td");
					if (i < 0)
					{
						if (j >= 0)
						{
							htmlWriter.Write("{0}({1},{2})", result.Items[j].Operation, result.Items[j].Source, result.Items[j].Target);
						}
					}
					else if (j < 0)
					{
						switch (i)
						{
						case 0:
							htmlWriter.Write("source tokens");
							break;
						case 1:
							htmlWriter.Write("target tokens");
							break;
						}
					}
					else
					{
						switch (i)
						{
						case 0:
							if (result.Items[j].Operation != EditOperation.Insert)
							{
								htmlWriter.Write(sourceTokens[result.Items[j].Source]);
							}
							break;
						case 1:
							if (result.Items[j].Operation != EditOperation.Delete)
							{
								htmlWriter.Write(targetTokens[result.Items[j].Target]);
							}
							break;
						}
					}
					htmlWriter.WriteEndTag("td");
				}
				htmlWriter.WriteEndTag("tr");
			}
			htmlWriter.WriteEndTag("table");
		}

		private static void PatchSimilarityMatrix(SimilarityMatrix sim, IList<Token> srcTokens, IList<Token> trgTokens, TagAssociations tagAlignment)
		{
			if (tagAlignment == null || tagAlignment.Count == 0)
			{
				return;
			}
			for (int i = 0; i < srcTokens.Count; i++)
			{
				if (!(srcTokens[i] is TagToken))
				{
					continue;
				}
				Tag tag = ((TagToken)srcTokens[i]).Tag;
				if (tag.Type != TagType.Start && tag.Type != TagType.End)
				{
					continue;
				}
				for (int j = 0; j < trgTokens.Count; j++)
				{
					if ((!sim.IsAssigned(i, j) || !(sim[i, j] < 0.0)) && trgTokens[j] is TagToken)
					{
						Tag tag2 = ((TagToken)trgTokens[j]).Tag;
						if ((tag2.Type == TagType.Start || tag2.Type == TagType.End) && !tagAlignment.AreAssociated(i, j))
						{
							sim[i, j] = -1.0;
						}
					}
				}
			}
		}

		private void FixTagActions(IList<Token> sourceTokens, IList<Token> targetTokens, Sdl.LanguagePlatform.Core.EditDistance.EditDistance result, TagAssociations tagAlignment)
		{
		}

		private int DetectMoves(Sdl.LanguagePlatform.Core.EditDistance.EditDistance result, MatrixItem[,] matrix)
		{
			int num = 0;
			for (int i = 0; i < result.Items.Count; i++)
			{
				EditOperation operation = result[i].Operation;
				if (operation != EditOperation.Delete && operation != EditOperation.Insert)
				{
					continue;
				}
				int source = 0;
				int target = 0;
				int moveSourceTarget = 0;
				int moveTargetSource = 0;
				int j;
				for (j = i + 1; j < result.Items.Count; j++)
				{
					if (operation == EditOperation.Delete && result[j].Operation == EditOperation.Insert && matrix[result[i].Source + 1, result[j].Target + 1].Similarity >= 0.95)
					{
						source = result[i].Source;
						moveSourceTarget = result[i].Target;
						target = result[j].Target;
						moveTargetSource = result[j].Source;
						break;
					}
					if (operation == EditOperation.Insert && result[j].Operation == EditOperation.Delete && matrix[result[j].Source + 1, result[i].Target + 1].Similarity >= 0.95)
					{
						source = result[j].Source;
						moveSourceTarget = result[j].Target;
						target = result[i].Target;
						moveTargetSource = result[i].Source;
						break;
					}
				}
				if (j < result.Items.Count)
				{
					EditDistanceItem value = result[i];
					value.Operation = EditOperation.Move;
					value.Source = source;
					value.Target = target;
					value.MoveSourceTarget = moveSourceTarget;
					value.MoveTargetSource = moveTargetSource;
					result.Items[i] = value;
					result.Items.RemoveAt(j);
					num++;
				}
			}
			return num;
		}
	}
}
