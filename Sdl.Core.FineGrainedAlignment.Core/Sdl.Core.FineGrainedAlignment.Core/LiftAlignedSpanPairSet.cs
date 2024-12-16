using System;
using System.Collections.Generic;
using System.IO;

namespace Sdl.Core.FineGrainedAlignment.Core
{
	public class LiftAlignedSpanPairSet
	{
		private class AlignedSpanNode
		{
			public LiftAlignedSpanPair AlignedSpanPair;

			public readonly List<AlignedSpanNode> children = new List<AlignedSpanNode>();

			private bool Strict => true;

			public void DumpAlignmentInfo(Func<short, short, string> srcRenderFunc, Func<short, short, string> trgRenderFunc, int count, bool includeConfidence = false)
			{
				List<AlignedSpanNode> list = new List<AlignedSpanNode>(children);
				list.Sort((AlignedSpanNode a, AlignedSpanNode b) => a.AlignedSpanPair.SourceStartIndex.CompareTo(b.AlignedSpanPair.SourceStartIndex));
				foreach (AlignedSpanNode item in list)
				{
					item.DumpAlignmentInfo(srcRenderFunc, trgRenderFunc, count + 1, includeConfidence);
				}
			}

			public void Validate()
			{
				AlignedSpanPair.Validate();
				for (int i = 0; i < children.Count; i++)
				{
					children[i].Validate();
				}
				for (int j = 0; j < children.Count - 1; j++)
				{
					for (int k = j + 1; k < children.Count; k++)
					{
					}
				}
			}

			public SimpleTree<LiftAlignedSpanPair> GetTree()
			{
				SimpleTree<LiftAlignedSpanPair> simpleTree = new SimpleTree<LiftAlignedSpanPair>(AlignedSpanPair, new List<SimpleTree<LiftAlignedSpanPair>>());
				foreach (AlignedSpanNode child in children)
				{
					simpleTree.Value.Add(child.GetTree());
				}
				return simpleTree;
			}

			public void GetAlignedSpanPairsByDepth(int depth, List<LiftAlignedSpanPair> list)
			{
				if (Depth() == depth)
				{
					list.Add(AlignedSpanPair);
				}
				else
				{
					foreach (AlignedSpanNode child in children)
					{
						child.GetAlignedSpanPairsByDepth(depth, list);
					}
				}
			}

			public int Depth()
			{
				int num = 0;
				foreach (AlignedSpanNode child in children)
				{
					num = Math.Max(num, child.Depth());
				}
				return 1 + num;
			}

			public void GetAlignedSpanPairsCoveredBySpan(LiftSpan span, bool searchTargetText, int maxExcess, List<LiftAlignedSpanPair> list)
			{
				if (span.Covers(AlignedSpanPair.Span(searchTargetText), maxExcess))
				{
					GetAllAlignedSpanPairs(list);
				}
				else
				{
					foreach (AlignedSpanNode child in children)
					{
						child.GetAlignedSpanPairsCoveredBySpan(span, searchTargetText, maxExcess, list);
					}
				}
			}

			public void Reduce(AlignedSpanNode parent)
			{
				List<AlignedSpanNode> list = new List<AlignedSpanNode>(children);
				foreach (AlignedSpanNode item in list)
				{
					item.Reduce(this);
				}
				if (parent != null)
				{
					LiftSpanMerger liftSpanMerger = new LiftSpanMerger(AlignedSpanPair.SourceEndIndex + 1);
					LiftSpanMerger liftSpanMerger2 = new LiftSpanMerger(AlignedSpanPair.TargetEndIndex + 1);
					foreach (AlignedSpanNode child in children)
					{
						liftSpanMerger.AddSpan(child.AlignedSpanPair.SourceSpan, 1.0);
						liftSpanMerger2.AddSpan(child.AlignedSpanPair.TargetSpan, 1.0);
					}
					if (liftSpanMerger.GetInverseSpans(AlignedSpanPair.SourceStartIndex, AlignedSpanPair.SourceLength).Count == 0 && liftSpanMerger2.GetInverseSpans(AlignedSpanPair.TargetStartIndex, AlignedSpanPair.TargetLength).Count == 0)
					{
						parent.children.Remove(this);
						foreach (AlignedSpanNode child2 in children)
						{
							parent.children.Add(child2);
						}
					}
				}
			}

			private static double AdjustDeducedConfidence(double sourceToTargTokenRatio, double confidence, int sourceLength, int targetLength)
			{
				if (sourceLength < 3 && targetLength < 3)
				{
					return confidence;
				}
				double num = (double)sourceLength * 1.0 / (double)targetLength;
				double num2 = (num > sourceToTargTokenRatio) ? (sourceToTargTokenRatio / num) : (num / sourceToTargTokenRatio);
				return confidence * num2;
			}

			internal void DeduceFurtherAlignments(bool includesPunc, double sourceToTargTokenRatio, Func<short, bool> srcIsPunc, Func<short, bool> trgIsPunc, Func<short, short, string> srcRenderFunc, Func<short, short, string> trgRenderFunc)
			{
				if (children.Count == 0)
				{
					return;
				}
				LiftSpanMerger liftSpanMerger = new LiftSpanMerger();
				foreach (AlignedSpanNode child in children)
				{
					child.MergeSpans(liftSpanMerger, source: true);
				}
				List<LiftSpan> inverseSpans = liftSpanMerger.GetInverseSpans(AlignedSpanPair.SourceStartIndex, AlignedSpanPair.SourceLength);
				if (inverseSpans.Count == 1)
				{
					LiftSpanMerger liftSpanMerger2 = new LiftSpanMerger();
					foreach (AlignedSpanNode child2 in children)
					{
						child2.MergeSpans(liftSpanMerger2, source: false);
					}
					List<LiftSpan> inverseSpans2 = liftSpanMerger2.GetInverseSpans(AlignedSpanPair.TargetStartIndex, AlignedSpanPair.TargetLength);
					if (inverseSpans2.Count == 1)
					{
						LiftAlignedSpanPair liftAlignedSpanPair = new LiftAlignedSpanPair();
						liftAlignedSpanPair.Provenance = 2;
						liftAlignedSpanPair.SourceSpan.StartIndex = inverseSpans[0].StartIndex;
						liftAlignedSpanPair.SourceSpan.Length = inverseSpans[0].Length;
						liftAlignedSpanPair.TargetSpan.StartIndex = inverseSpans2[0].StartIndex;
						liftAlignedSpanPair.TargetSpan.Length = inverseSpans2[0].Length;
						bool flag = liftAlignedSpanPair.SourceStartIndex == AlignedSpanPair.SourceStartIndex;
						bool flag2 = liftAlignedSpanPair.TargetStartIndex == AlignedSpanPair.TargetStartIndex;
						bool flag3 = liftAlignedSpanPair.SourceEndIndex == AlignedSpanPair.SourceEndIndex;
						bool flag4 = liftAlignedSpanPair.TargetEndIndex == AlignedSpanPair.TargetEndIndex;
						bool flag5 = flag ? flag2 : ((!flag3) ? (!flag4 && !flag2) : flag4);
						bool flag6 = true;
						if (includesPunc && flag5)
						{
							flag6 = CheckDeductionPunctuation(liftAlignedSpanPair, srcIsPunc, trgIsPunc);
						}
						if (flag5 && flag6)
						{
							List<LiftSpan> spans = liftSpanMerger2.GetSpans();
							double num = liftSpanMerger2.GetConfidence(spans[0].StartIndex, spans[0].Length);
							if (spans.Count == 2)
							{
								double confidence = liftSpanMerger2.GetConfidence(spans[1].StartIndex, spans[1].Length);
								num = (num * (double)spans[0].Length + confidence * (double)spans[1].Length) / (double)(spans[0].Length + spans[1].Length);
							}
							List<LiftSpan> spans2 = liftSpanMerger.GetSpans();
							double num2 = liftSpanMerger.GetConfidence(spans2[0].StartIndex, spans2[0].Length);
							if (spans2.Count == 2)
							{
								double confidence2 = liftSpanMerger.GetConfidence(spans2[1].StartIndex, spans2[1].Length);
								num2 = (num2 * (double)spans2[0].Length + confidence2 * (double)spans2[1].Length) / (double)(spans2[0].Length + spans2[1].Length);
							}
							liftAlignedSpanPair.Confidence = (float)((double)AlignedSpanPair.Confidence * (num2 + num) / 2.0);
							liftAlignedSpanPair.Confidence = (float)AdjustDeducedConfidence(sourceToTargTokenRatio, liftAlignedSpanPair.Confidence, liftAlignedSpanPair.SourceLength, liftAlignedSpanPair.TargetLength);
							liftAlignedSpanPair.Provenance = 2;
							AddChildSpan(liftAlignedSpanPair);
						}
					}
				}
				bool flag7;
				do
				{
					LiftAlignedSpanPair liftAlignedSpanPair2 = null;
					flag7 = false;
					foreach (AlignedSpanNode child3 in children)
					{
						bool flag8 = child3.AlignedSpanPair.SourceStartIndex == AlignedSpanPair.SourceStartIndex;
						bool flag9 = child3.AlignedSpanPair.SourceEndIndex == AlignedSpanPair.SourceEndIndex;
						if (flag8 || flag9)
						{
							bool flag10 = child3.AlignedSpanPair.TargetStartIndex == AlignedSpanPair.TargetStartIndex;
							bool flag11 = child3.AlignedSpanPair.TargetEndIndex == AlignedSpanPair.TargetEndIndex;
							if ((flag10 || flag11) && (!flag8 || flag10) && (!flag9 || flag11))
							{
								LiftAlignedSpanPair liftAlignedSpanPair3 = new LiftAlignedSpanPair();
								liftAlignedSpanPair3.SourceSpan.StartIndex = (short)((child3.AlignedSpanPair.SourceStartIndex == AlignedSpanPair.SourceStartIndex) ? (child3.AlignedSpanPair.SourceStartIndex + child3.AlignedSpanPair.SourceLength) : AlignedSpanPair.SourceStartIndex);
								liftAlignedSpanPair3.SourceSpan.Length = (short)(AlignedSpanPair.SourceLength - child3.AlignedSpanPair.SourceLength);
								liftAlignedSpanPair3.TargetSpan.StartIndex = (short)((child3.AlignedSpanPair.TargetStartIndex == AlignedSpanPair.TargetStartIndex) ? (child3.AlignedSpanPair.TargetStartIndex + child3.AlignedSpanPair.TargetLength) : AlignedSpanPair.TargetStartIndex);
								liftAlignedSpanPair3.TargetSpan.Length = (short)(AlignedSpanPair.TargetLength - child3.AlignedSpanPair.TargetLength);
								liftAlignedSpanPair3.Confidence = child3.AlignedSpanPair.Confidence * AlignedSpanPair.Confidence;
								if (FindNodeWithMatchingSpan(children, liftAlignedSpanPair3.SourceSpan, searchTarget: false) == null && FindNodeWithMatchingSpan(children, liftAlignedSpanPair3.TargetSpan, searchTarget: true) == null)
								{
									liftAlignedSpanPair2 = liftAlignedSpanPair3;
									liftAlignedSpanPair2.Provenance = 2;
									break;
								}
							}
						}
					}
					if (liftAlignedSpanPair2 != null && includesPunc && !CheckDeductionPunctuation(liftAlignedSpanPair2, srcIsPunc, trgIsPunc))
					{
						liftAlignedSpanPair2 = null;
					}
					if (liftAlignedSpanPair2 == null)
					{
						break;
					}
					liftAlignedSpanPair2.Confidence = (float)AdjustDeducedConfidence(sourceToTargTokenRatio, liftAlignedSpanPair2.Confidence, liftAlignedSpanPair2.SourceLength, liftAlignedSpanPair2.TargetLength);
					liftAlignedSpanPair2.Provenance = 2;
					bool contradictory;
					AlignedSpanNode smallestContainingSpanPair = GetSmallestContainingSpanPair(liftAlignedSpanPair2, out contradictory, repetitionIsContradiction: true);
					if (smallestContainingSpanPair != this)
					{
						liftAlignedSpanPair2 = null;
					}
					if (!contradictory && liftAlignedSpanPair2 != null)
					{
						AddChildSpan(liftAlignedSpanPair2);
						flag7 = true;
					}
				}
				while (flag7);
				foreach (AlignedSpanNode child4 in children)
				{
					child4.DeduceFurtherAlignments(includesPunc, sourceToTargTokenRatio, srcIsPunc, trgIsPunc, srcRenderFunc, trgRenderFunc);
				}
			}

			private int LeadingPunctuationCount(LiftSpan span, Func<short, bool> isPunc, int max)
			{
				int num = 0;
				for (short num2 = span.StartIndex; num2 <= span.EndIndex; num2 = (short)(num2 + 1))
				{
					if (!isPunc(num2))
					{
						return num;
					}
					num++;
					if (max != -1 && num >= max)
					{
						return num;
					}
				}
				return num;
			}

			private int TrailingPunctuationCount(LiftSpan span, Func<short, bool> isPunc, int max)
			{
				int num = 0;
				for (short num2 = span.EndIndex; num2 >= span.StartIndex; num2 = (short)(num2 - 1))
				{
					if (!isPunc(num2))
					{
						return num;
					}
					num++;
					if (max != -1 && num >= max)
					{
						return num;
					}
				}
				return num;
			}

			private bool IsPunctuation(LiftSpan span, Func<short, bool> isPunc)
			{
				for (short num = span.StartIndex; num <= span.EndIndex; num = (short)(num + 1))
				{
					if (!isPunc(num))
					{
						return false;
					}
				}
				return true;
			}

			private AlignedSpanNode FindNodeWithMatchingSpan(List<AlignedSpanNode> nodes, LiftSpan span, bool searchTarget)
			{
				foreach (AlignedSpanNode node in nodes)
				{
					if (searchTarget)
					{
						if (node.AlignedSpanPair.TargetSpan.SpanEquals(span))
						{
							return node;
						}
					}
					else if (node.AlignedSpanPair.SourceSpan.SpanEquals(span))
					{
						return node;
					}
				}
				return null;
			}

			private bool CheckDeductionPunctuation(LiftAlignedSpanPair deducedAlignedSpanPair, Func<short, bool> srcIsPunc, Func<short, bool> trgIsPunc)
			{
				int num = 0;
				while (true)
				{
					bool flag = false;
					bool flag2 = srcIsPunc(deducedAlignedSpanPair.SourceStartIndex);
					bool flag3 = trgIsPunc(deducedAlignedSpanPair.TargetStartIndex);
					bool flag4 = srcIsPunc((short)(deducedAlignedSpanPair.SourceStartIndex + deducedAlignedSpanPair.SourceLength - 1));
					bool flag5 = trgIsPunc((short)(deducedAlignedSpanPair.TargetStartIndex + deducedAlignedSpanPair.TargetLength - 1));
					bool flag6 = false;
					bool flag7 = false;
					if (flag2 && flag4)
					{
						flag6 = IsPunctuation(deducedAlignedSpanPair.SourceSpan, srcIsPunc);
					}
					if (flag3 && flag5)
					{
						flag7 = IsPunctuation(deducedAlignedSpanPair.TargetSpan, trgIsPunc);
					}
					if (flag6 && flag7)
					{
						return true;
					}
					if (flag6 | flag7)
					{
						return false;
					}
					int num2 = 5;
					if (flag2 && LeadingPunctuationCount(deducedAlignedSpanPair.SourceSpan, srcIsPunc, num2) == num2)
					{
						return false;
					}
					if (flag3 && LeadingPunctuationCount(deducedAlignedSpanPair.TargetSpan, trgIsPunc, num2) == num2)
					{
						return false;
					}
					if (flag4 && TrailingPunctuationCount(deducedAlignedSpanPair.SourceSpan, srcIsPunc, num2) == num2)
					{
						return false;
					}
					if (flag5 && TrailingPunctuationCount(deducedAlignedSpanPair.TargetSpan, trgIsPunc, num2) == num2)
					{
						return false;
					}
					if (flag4 && deducedAlignedSpanPair.SourceSpan.Length > 1)
					{
						deducedAlignedSpanPair.SourceSpan.Length = (short)(deducedAlignedSpanPair.SourceSpan.Length - 1);
						flag = true;
					}
					if (flag5 && deducedAlignedSpanPair.TargetSpan.Length > 1)
					{
						deducedAlignedSpanPair.TargetSpan.Length = (short)(deducedAlignedSpanPair.TargetSpan.Length - 1);
						flag = true;
					}
					if (flag2 && deducedAlignedSpanPair.SourceSpan.Length > 1)
					{
						deducedAlignedSpanPair.SourceSpan.StartIndex++;
						deducedAlignedSpanPair.SourceSpan.Length--;
						flag = true;
					}
					if (flag3 && deducedAlignedSpanPair.TargetSpan.Length > 1)
					{
						deducedAlignedSpanPair.TargetSpan.StartIndex++;
						deducedAlignedSpanPair.TargetSpan.Length--;
						flag = true;
					}
					if (!flag)
					{
						break;
					}
					num++;
					if (num > 100)
					{
						throw new Exception("Deduction logic error");
					}
				}
				return true;
			}

			private void MergeSpans(LiftSpanMerger merger, bool source)
			{
				if (source)
				{
					merger.AddSpan(AlignedSpanPair.SourceSpan, AlignedSpanPair.Confidence);
				}
				else
				{
					merger.AddSpan(AlignedSpanPair.TargetSpan, AlignedSpanPair.Confidence);
				}
				foreach (AlignedSpanNode child in children)
				{
					child.MergeSpans(merger, source);
				}
			}

			public void GetAllAlignedSpanPairs(List<LiftAlignedSpanPair> list)
			{
				list.Add(AlignedSpanPair);
				foreach (AlignedSpanNode child in children)
				{
					child.GetAllAlignedSpanPairs(list);
				}
			}

			public static AlignedSpanNode Load(BinaryReader reader, ref int count, bool compactSzn)
			{
				AlignedSpanNode alignedSpanNode = new AlignedSpanNode();
				alignedSpanNode.AlignedSpanPair = LiftAlignedSpanPair.Load(reader, compactSzn);
				count++;
				byte b = reader.ReadByte();
				for (int i = 0; i < b; i++)
				{
					alignedSpanNode.children.Add(Load(reader, ref count, compactSzn));
				}
				return alignedSpanNode;
			}

			public void Save(BinaryWriter writer, bool compactSzn)
			{
				if (!SaveInternal(writer, compactSzn))
				{
					throw new Exception("AlignedSpanNode - too many children: " + children.Count.ToString());
				}
			}

			internal bool SaveInternal(BinaryWriter writer, bool compactSzn)
			{
				AlignedSpanPair.Save(writer, compactSzn);
				if (children.Count > 255)
				{
					return false;
				}
				writer.Write((byte)children.Count);
				foreach (AlignedSpanNode child in children)
				{
					if (!child.SaveInternal(writer, compactSzn))
					{
						return false;
					}
				}
				return true;
			}

			private bool CanContainSpan(LiftSpan span, out bool contradictory, bool searchTargetText)
			{
				contradictory = false;
				LiftSpan liftSpan = AlignedSpanPair.SourceSpan;
				if (searchTargetText)
				{
					liftSpan = AlignedSpanPair.TargetSpan;
				}
				if (liftSpan.Length == 0 && span.Length > 0)
				{
					contradictory = true;
					return false;
				}
				if (!liftSpan.Covers(span, 0))
				{
					if (span.Overlaps(liftSpan))
					{
						contradictory = !span.Covers(liftSpan, 0);
					}
					return false;
				}
				return true;
			}

			private bool CanContainSpanPair(LiftAlignedSpanPair otherAlignedSpanPair, out bool contradictory, bool repetitionIsContradiction)
			{
				bool flag = AlignedSpanPair.SourceSpan.Covers(otherAlignedSpanPair.SourceSpan, 0);
				bool flag2 = AlignedSpanPair.TargetSpan.Covers(otherAlignedSpanPair.TargetSpan, 0);
				contradictory = (flag != flag2);
				if (!flag && (AlignedSpanPair.SourceSpan.Overlaps(otherAlignedSpanPair.SourceSpan) || AlignedSpanPair.TargetSpan.Overlaps(otherAlignedSpanPair.TargetSpan)) && (!otherAlignedSpanPair.SourceSpan.Covers(AlignedSpanPair.SourceSpan, 0) || !otherAlignedSpanPair.TargetSpan.Covers(AlignedSpanPair.TargetSpan, 0)))
				{
					contradictory = true;
				}
				if (repetitionIsContradiction)
				{
					if (flag)
					{
						contradictory |= (AlignedSpanPair.SourceSpan.Length == otherAlignedSpanPair.SourceSpan.Length);
					}
					if (flag2)
					{
						contradictory |= (AlignedSpanPair.TargetSpan.Length == otherAlignedSpanPair.TargetSpan.Length);
					}
				}
				return flag;
			}

			public AlignedSpanNode GetSmallestContainingSpanPair(LiftAlignedSpanPair otherAlignedSpanPair, bool repetitionIsContradiction)
			{
				bool contradictory;
				return GetSmallestContainingSpanPair(otherAlignedSpanPair, out contradictory, repetitionIsContradiction);
			}

			public AlignedSpanNode FindParentSpan(LiftAlignedSpanPair alignedSpan, out AlignedSpanNode childNode)
			{
				childNode = null;
				foreach (AlignedSpanNode child in children)
				{
					if (child.AlignedSpanPair == alignedSpan)
					{
						childNode = child;
						return this;
					}
					AlignedSpanNode alignedSpanNode = child.FindParentSpan(alignedSpan, out childNode);
					if (alignedSpanNode != null)
					{
						return alignedSpanNode;
					}
				}
				return null;
			}

			public AlignedSpanNode GetSmallestContainingSpanPair(LiftSpan span, out bool contradictory, bool searchTargetText)
			{
				if (!CanContainSpan(span, out contradictory, searchTargetText))
				{
					return null;
				}
				if (contradictory)
				{
					return null;
				}
				foreach (AlignedSpanNode child in children)
				{
					AlignedSpanNode smallestContainingSpanPair = child.GetSmallestContainingSpanPair(span, out contradictory, searchTargetText);
					if (contradictory)
					{
						return null;
					}
					if (smallestContainingSpanPair != null)
					{
						return smallestContainingSpanPair;
					}
				}
				return this;
			}

			public AlignedSpanNode GetSmallestContainingSpanPair(LiftAlignedSpanPair otherAlignedSpanPair, out bool contradictory, bool repetitionIsContradiction)
			{
				if (!CanContainSpanPair(otherAlignedSpanPair, out contradictory, repetitionIsContradiction))
				{
					return null;
				}
				if (contradictory)
				{
					return null;
				}
				foreach (AlignedSpanNode child in children)
				{
					AlignedSpanNode smallestContainingSpanPair = child.GetSmallestContainingSpanPair(otherAlignedSpanPair, out contradictory, repetitionIsContradiction);
					if (contradictory)
					{
						return null;
					}
					if (smallestContainingSpanPair != null)
					{
						return smallestContainingSpanPair;
					}
				}
				return this;
			}

			public void AddChildSpan(LiftAlignedSpanPair childAlignedSpan)
			{
				childAlignedSpan.Validate();
				if (!AlignedSpanPair.SourceSpan.Covers(childAlignedSpan.SourceSpan, 0))
				{
					throw new Exception("Aligned span does not cover child");
				}
				if (!AlignedSpanPair.TargetSpan.Covers(childAlignedSpan.TargetSpan, 0))
				{
					throw new Exception("Aligned span does not cover child");
				}
				if (AlignedSpanPair.SourceSpan.Length == childAlignedSpan.SourceSpan.Length)
				{
					throw new Exception("Duplication source alignment");
				}
				if (AlignedSpanPair.TargetSpan.Length == childAlignedSpan.TargetSpan.Length)
				{
					throw new Exception("Duplication target alignment");
				}
				if (AlignedSpanPair.SourceSpan.Length < 1)
				{
					throw new Exception("Span with invalid length");
				}
				if (AlignedSpanPair.TargetSpan.Length < 1)
				{
					throw new Exception("Span with invalid length");
				}
				List<AlignedSpanNode> list = new List<AlignedSpanNode>();
				for (int i = 0; i < children.Count; i++)
				{
					AlignedSpanNode alignedSpanNode = children[i];
					if (childAlignedSpan.SourceSpan.Covers(alignedSpanNode.AlignedSpanPair.SourceSpan, 0))
					{
						if (!childAlignedSpan.TargetSpan.Covers(alignedSpanNode.AlignedSpanPair.TargetSpan, 0))
						{
							throw new Exception("Inconsistent aligned spans");
						}
						list.Add(alignedSpanNode);
					}
					else if (Strict)
					{
						if (alignedSpanNode.AlignedSpanPair.SourceSpan.Overlaps(childAlignedSpan.SourceSpan))
						{
							throw new Exception("New child overlaps existing children");
						}
						if (alignedSpanNode.AlignedSpanPair.TargetSpan.Overlaps(childAlignedSpan.TargetSpan))
						{
							throw new Exception("New child overlaps existing children");
						}
					}
				}
				AlignedSpanNode alignedSpanNode2 = new AlignedSpanNode();
				alignedSpanNode2.AlignedSpanPair = childAlignedSpan;
				foreach (AlignedSpanNode item in list)
				{
					children.Remove(item);
				}
				alignedSpanNode2.children.AddRange(list);
				children.Add(alignedSpanNode2);
			}
		}

		private AlignedSpanNode _root;

		private readonly List<LiftAlignedSpanPair> _incompatiblePairs = new List<LiftAlignedSpanPair>();

		private readonly HashSet<short> _sourceAlignedToNull = new HashSet<short>();

		private readonly HashSet<short> _targetAlignedToNull = new HashSet<short>();

		private int _count;

		private byte[] _serializedAlignmentData;

		private const byte SERIALIZATION_VERSION = 1;

		private bool _empty;

		public bool IsEmpty
		{
			get
			{
				EnsureDeserializedIfNeeded();
				return _empty;
			}
		}

		public int Count
		{
			get
			{
				EnsureDeserializedIfNeeded();
				return _count;
			}
		}

		public void Validate()
		{
			if (!IsEmpty)
			{
				_root.Validate();
			}
		}

		public LiftAlignedSpanPairSet(short sourceLength, short targetLength)
		{
			_empty = false;
			LiftAlignedSpanPair alignedSpanPair = new LiftAlignedSpanPair
			{
				SourceSpan = 
				{
					Length = sourceLength
				},
				TargetSpan = 
				{
					Length = targetLength
				},
				Confidence = 1f,
				Provenance = 5
			};
			_root = new AlignedSpanNode();
			_root.AlignedSpanPair = alignedSpanPair;
			_count = 1;
		}

		private LiftAlignedSpanPairSet()
		{
			_empty = true;
		}

		public static LiftAlignedSpanPairSet CreateEmptyLiftAlignedSpanPairSet()
		{
			return new LiftAlignedSpanPairSet();
		}

		public LiftAlignedSpanPairSet(byte[] serializedAlignmentData)
		{
			if (serializedAlignmentData == null)
			{
				throw new ArgumentNullException("serializedAlignmentData");
			}
			_serializedAlignmentData = serializedAlignmentData;
		}

		private void EnsureDeserializedIfNeeded()
		{
			if (_serializedAlignmentData != null)
			{
				using (MemoryStream memoryStream = new MemoryStream(_serializedAlignmentData))
				{
					using (BinaryReader binaryReader = new BinaryReader(memoryStream))
					{
						Load(binaryReader);
						binaryReader.Close();
						memoryStream.Close();
						_serializedAlignmentData = null;
					}
				}
			}
		}

		private void Load(BinaryReader reader)
		{
			_count = 0;
			byte b = reader.ReadByte();
			if (b > 1)
			{
				throw new Exception("Unsupported LiftAlignedSpanPairSet serialization version: " + b.ToString());
			}
			_empty = reader.ReadBoolean();
			if (!_empty)
			{
				bool flag = reader.ReadBoolean();
				_root = AlignedSpanNode.Load(reader, ref _count, flag);
				for (int num = flag ? reader.ReadUInt16() : reader.ReadInt32(); num > 0; num--)
				{
					_incompatiblePairs.Add(LiftAlignedSpanPair.Load(reader, flag));
				}
			}
		}

		private void CheckNotEmpty()
		{
			if (_empty)
			{
				throw new Exception("The LiftAlignedSpanPairSet is empty.");
			}
		}

		public byte[] Save()
		{
			EnsureDeserializedIfNeeded();
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Write((byte)1);
					binaryWriter.Write(_empty);
					if (!_empty)
					{
						bool flag = CompactSerialization();
						binaryWriter.Write(flag);
						if (!_root.SaveInternal(binaryWriter, flag))
						{
							LiftAlignedSpanPairSet liftAlignedSpanPairSet = CreateEmptyLiftAlignedSpanPairSet();
							return liftAlignedSpanPairSet.Save();
						}
						if (flag)
						{
							binaryWriter.Write((ushort)_incompatiblePairs.Count);
						}
						else
						{
							binaryWriter.Write(_incompatiblePairs.Count);
						}
						foreach (LiftAlignedSpanPair incompatiblePair in _incompatiblePairs)
						{
							incompatiblePair.Save(binaryWriter, flag);
						}
					}
					binaryWriter.Close();
					memoryStream.Close();
					return memoryStream.ToArray();
				}
			}
		}

		private bool CompactSerialization()
		{
			if (_root.AlignedSpanPair.SourceSpan.Length <= 255 && _root.AlignedSpanPair.TargetSpan.Length <= 255)
			{
				return _incompatiblePairs.Count <= 65535;
			}
			return false;
		}

		public SimpleTree<LiftAlignedSpanPair> GetTree()
		{
			EnsureDeserializedIfNeeded();
			CheckNotEmpty();
			return _root.GetTree();
		}

		public int Depth()
		{
			EnsureDeserializedIfNeeded();
			return _root.Depth();
		}

		public LiftAlignedSpanPair Root()
		{
			EnsureDeserializedIfNeeded();
			CheckNotEmpty();
			return _root.AlignedSpanPair;
		}

		public List<LiftAlignedSpanPair> GetAlignedSpanPairsByDepth(int depth)
		{
			EnsureDeserializedIfNeeded();
			CheckNotEmpty();
			List<LiftAlignedSpanPair> list = new List<LiftAlignedSpanPair>();
			_root.GetAlignedSpanPairsByDepth(depth, list);
			return list;
		}

		public List<LiftAlignedSpanPair> GetAllAlignedSpanPairs(bool includeIncompatible)
		{
			EnsureDeserializedIfNeeded();
			CheckNotEmpty();
			List<LiftAlignedSpanPair> list = new List<LiftAlignedSpanPair>();
			_root.GetAllAlignedSpanPairs(list);
			if (includeIncompatible)
			{
				list.AddRange(_incompatiblePairs);
			}
			return list;
		}

		public List<LiftAlignedSpanPair> GetIncompatibleAlignedSpanPairs()
		{
			EnsureDeserializedIfNeeded();
			CheckNotEmpty();
			return new List<LiftAlignedSpanPair>(_incompatiblePairs);
		}

		public List<LiftAlignedSpanPair> GetAlignedSpanPairsCoveredBySpan(LiftSpan span, bool searchTargetText, int maxExcess)
		{
			EnsureDeserializedIfNeeded();
			CheckNotEmpty();
			List<LiftAlignedSpanPair> list = new List<LiftAlignedSpanPair>();
			_root.GetAlignedSpanPairsCoveredBySpan(span, searchTargetText, maxExcess, list);
			return list;
		}

		public void Clear()
		{
			EnsureDeserializedIfNeeded();
			CheckNotEmpty();
			_root.children.Clear();
		}

		private bool CrossesNeighbours(LiftAlignedSpanPair alignedSpan, bool useTarget)
		{
			if (Contradicts(alignedSpan, repetitionIsContradiction: false))
			{
				throw new Exception("CrossesNeighbours: alignedSpan contradicts span pair set");
			}
			bool contradictory;
			AlignedSpanNode smallestContainingSpanPair = _root.GetSmallestContainingSpanPair(alignedSpan, out contradictory, repetitionIsContradiction: false);
			AlignedSpanNode alignedSpanNode = null;
			List<AlignedSpanNode> list = new List<AlignedSpanNode>(smallestContainingSpanPair.children);
			list.Sort((AlignedSpanNode a, AlignedSpanNode b) => a.AlignedSpanPair.StartIndex(useTarget).CompareTo(b.AlignedSpanPair.StartIndex(useTarget)));
			foreach (AlignedSpanNode item in list)
			{
				if (item.AlignedSpanPair.EndIndex(useTarget) >= alignedSpan.StartIndex(useTarget))
				{
					break;
				}
				alignedSpanNode = item;
			}
			AlignedSpanNode alignedSpanNode2 = null;
			List<AlignedSpanNode> list2 = new List<AlignedSpanNode>(smallestContainingSpanPair.children);
			list2.Sort((AlignedSpanNode b, AlignedSpanNode a) => a.AlignedSpanPair.StartIndex(useTarget).CompareTo(b.AlignedSpanPair.StartIndex(useTarget)));
			foreach (AlignedSpanNode item2 in list2)
			{
				if (item2.AlignedSpanPair.StartIndex(useTarget) <= alignedSpan.EndIndex(useTarget))
				{
					break;
				}
				alignedSpanNode2 = item2;
			}
			int num = -1;
			int num2 = -1;
			if (alignedSpanNode != null)
			{
				num = alignedSpanNode.AlignedSpanPair.StartIndex(!useTarget) + 1;
			}
			if (alignedSpanNode2 != null)
			{
				num2 = alignedSpanNode2.AlignedSpanPair.EndIndex(!useTarget) - 1;
			}
			if (alignedSpanNode != null && alignedSpanNode2 != null)
			{
				num = Math.Min(alignedSpanNode.AlignedSpanPair.StartIndex(!useTarget) + 1, alignedSpanNode2.AlignedSpanPair.StartIndex(!useTarget) + 1);
				num2 = Math.Max(alignedSpanNode2.AlignedSpanPair.EndIndex(!useTarget) - 1, alignedSpanNode.AlignedSpanPair.EndIndex(!useTarget) - 1);
			}
			if (alignedSpan.StartIndex(!useTarget) < num)
			{
				return true;
			}
			if (num2 != -1 && alignedSpan.EndIndex(!useTarget) > num2)
			{
				return true;
			}
			return false;
		}

		public bool CrossesNeighbours(LiftAlignedSpanPair alignedSpan)
		{
			EnsureDeserializedIfNeeded();
			CheckNotEmpty();
			if (CrossesNeighbours(alignedSpan, useTarget: false))
			{
				return true;
			}
			if (CrossesNeighbours(alignedSpan, useTarget: true))
			{
				return true;
			}
			return false;
		}

		public bool Contradicts(LiftAlignedSpanPair alignedSpan, bool repetitionIsContradiction)
		{
			EnsureDeserializedIfNeeded();
			CheckNotEmpty();
			alignedSpan.Validate();
			bool contradictory;
			AlignedSpanNode smallestContainingSpanPair = _root.GetSmallestContainingSpanPair(alignedSpan, out contradictory, repetitionIsContradiction);
			if (contradictory)
			{
				return true;
			}
			if (smallestContainingSpanPair == null)
			{
				return true;
			}
			return false;
		}

		public bool Coheres(LiftAlignedSpanPair alignedSpan)
		{
			List<LiftAlignedSpanPair> list = new List<LiftAlignedSpanPair>();
			_root.GetAlignedSpanPairsCoveredBySpan(alignedSpan.SourceSpan, searchTargetText: false, 0, list);
			foreach (LiftAlignedSpanPair item in list)
			{
				if (!alignedSpan.TargetSpan.Covers(item.TargetSpan, 0))
				{
					return false;
				}
			}
			list.Clear();
			_root.GetAlignedSpanPairsCoveredBySpan(alignedSpan.TargetSpan, searchTargetText: true, 0, list);
			foreach (LiftAlignedSpanPair item2 in list)
			{
				if (!alignedSpan.SourceSpan.Covers(item2.SourceSpan, 0))
				{
					return false;
				}
			}
			return true;
		}

		public LiftAlignedSpanPair GetSmallestContainingPair(LiftSpan span, bool searchTargetText, bool exceptionIfContradicts)
		{
			EnsureDeserializedIfNeeded();
			CheckNotEmpty();
			bool contradictory;
			AlignedSpanNode smallestContainingSpanPair = _root.GetSmallestContainingSpanPair(span, out contradictory, searchTargetText);
			if (contradictory)
			{
				if (exceptionIfContradicts)
				{
					throw new Exception("span contradicts AlignedSpanSet");
				}
				return null;
			}
			if (smallestContainingSpanPair == null)
			{
				throw new Exception("span does not fit in AlignedSpanSet");
			}
			return smallestContainingSpanPair.AlignedSpanPair;
		}

		public LiftAlignedSpanPair GetSmallestContainingPair(LiftAlignedSpanPair alignedSpan, bool repetitionIsContradiction)
		{
			EnsureDeserializedIfNeeded();
			CheckNotEmpty();
			bool contradictory;
			AlignedSpanNode smallestContainingSpanPair = _root.GetSmallestContainingSpanPair(alignedSpan, out contradictory, repetitionIsContradiction);
			if (contradictory)
			{
				throw new Exception("Aligned span contradicts AlignedSpanSet");
			}
			if (smallestContainingSpanPair == null)
			{
				throw new Exception("Aligned span does not fit in AlignedSpanSet");
			}
			return smallestContainingSpanPair.AlignedSpanPair;
		}

		public bool Exists(LiftAlignedSpanPair alignedSpan)
		{
			EnsureDeserializedIfNeeded();
			CheckNotEmpty();
			bool contradictory;
			AlignedSpanNode smallestContainingSpanPair = _root.GetSmallestContainingSpanPair(alignedSpan, out contradictory, repetitionIsContradiction: false);
			if (contradictory)
			{
				return false;
			}
			if (smallestContainingSpanPair == null)
			{
				return false;
			}
			if (smallestContainingSpanPair.AlignedSpanPair.SourceLength == alignedSpan.SourceLength)
			{
				return smallestContainingSpanPair.AlignedSpanPair.TargetLength == alignedSpan.TargetLength;
			}
			return false;
		}

		public void Add(LiftAlignedSpanPair alignedSpan)
		{
			EnsureDeserializedIfNeeded();
			CheckNotEmpty();
			alignedSpan.Validate();
			AlignedSpanNode smallestContainingSpanPair = _root.GetSmallestContainingSpanPair(alignedSpan, repetitionIsContradiction: true);
			if (smallestContainingSpanPair == null)
			{
				throw new Exception("Aligned span does not fit in AlignedSpanSet");
			}
			AddInternal(alignedSpan, smallestContainingSpanPair);
		}

		public void AddNullAlignment(short index, bool target)
		{
			if (target)
			{
				_targetAlignedToNull.Add(index);
			}
			else
			{
				_sourceAlignedToNull.Add(index);
			}
		}

		private void AddInternal(LiftAlignedSpanPair alignedSpan, AlignedSpanNode parent)
		{
			parent.AddChildSpan(alignedSpan);
			_count++;
		}

		public void ArbitraryAdd(LiftAlignedSpanPair alignedSpan)
		{
			EnsureDeserializedIfNeeded();
			CheckNotEmpty();
			alignedSpan.Validate();
			AlignedSpanNode smallestContainingSpanPair = _root.GetSmallestContainingSpanPair(alignedSpan, repetitionIsContradiction: true);
			if (smallestContainingSpanPair == null)
			{
				if (Exists(alignedSpan))
				{
					throw new Exception("The aligned span pair is already in the alignment set");
				}
				if (alignedSpan.SourceEndIndex > _root.AlignedSpanPair.SourceEndIndex || alignedSpan.TargetEndIndex > _root.AlignedSpanPair.TargetEndIndex)
				{
					throw new Exception("The aligned span pair exceeds the data size");
				}
				if (IncompatibleSpanPairIndex(alignedSpan) != -1)
				{
					throw new Exception("The aligned span pair is already in the incompatible list");
				}
				_incompatiblePairs.Add(alignedSpan);
			}
			else
			{
				AddInternal(alignedSpan, smallestContainingSpanPair);
			}
		}

		private int IncompatibleSpanPairIndex(LiftAlignedSpanPair spanPair)
		{
			for (int i = 0; i < _incompatiblePairs.Count; i++)
			{
				LiftAlignedSpanPair liftAlignedSpanPair = _incompatiblePairs[i];
				if (liftAlignedSpanPair.SourceSpan.SpanEquals(spanPair.SourceSpan) && liftAlignedSpanPair.TargetSpan.SpanEquals(spanPair.TargetSpan))
				{
					return i;
				}
			}
			return -1;
		}

		public void Remove(LiftAlignedSpanPair alignedSpan)
		{
			EnsureDeserializedIfNeeded();
			CheckNotEmpty();
			int num = IncompatibleSpanPairIndex(alignedSpan);
			if (num != -1)
			{
				_incompatiblePairs.RemoveAt(num);
				return;
			}
			if (_root.AlignedSpanPair == alignedSpan)
			{
				throw new Exception("Attempt to remove root from LiftAlignedSpanPairSet");
			}
			AlignedSpanNode childNode;
			AlignedSpanNode alignedSpanNode = _root.FindParentSpan(alignedSpan, out childNode);
			if (alignedSpanNode == null)
			{
				throw new Exception("Attempt to remove non-member from LiftAlignedSpanPairSet");
			}
			alignedSpanNode.children.Remove(childNode);
			alignedSpanNode.children.AddRange(childNode.children);
			_count--;
		}

		public void Reduce()
		{
			EnsureDeserializedIfNeeded();
			CheckNotEmpty();
			_root.Reduce(null);
		}

		public void DeduceFurtherAlignments(bool includesPunc, Func<short, bool> srcIsPunc, Func<short, bool> trgIsPunc)
		{
			EnsureDeserializedIfNeeded();
			CheckNotEmpty();
			if (includesPunc)
			{
				if (srcIsPunc == null)
				{
					throw new ArgumentNullException("srcIsPunc");
				}
				if (trgIsPunc == null)
				{
					throw new ArgumentNullException("trgIsPunc");
				}
			}
			double sourceToTargTokenRatio = (double)_root.AlignedSpanPair.SourceLength * 1.0 / (double)_root.AlignedSpanPair.TargetLength;
			_root.DeduceFurtherAlignments(includesPunc, sourceToTargTokenRatio, srcIsPunc, trgIsPunc, null, null);
		}
	}
}
