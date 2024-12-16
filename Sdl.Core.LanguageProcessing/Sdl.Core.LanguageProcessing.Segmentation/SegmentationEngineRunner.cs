using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Collections.Generic;
using System.Text;

namespace Sdl.Core.LanguageProcessing.Segmentation
{
	public class SegmentationEngineRunner
	{
		private bool _isWordStop;

		private SegmentationEngine _segmentationEngine;

		private SegmentationHint _topFirstParent = SegmentationHint.MayExclude;

		private bool _wasInsideTagStopContainer;

		private int _hashIndex;

		private SegmentationInfo _previousInfo;

		private Dictionary<int, SegmentationInfo> ParagraphSegmentationInfo;

		private Node _lastSegmentStarter;

		private Chunk _updateChunk;

		private bool _updated;

		private Node _lastText;

		private Node _textBeforeLastText;

		private string _updatedChunkTextToBreak = string.Empty;

		private Tree Tree
		{
			get;
			set;
		}

		public virtual void RunSegmentationEngine(SegmentationEngine engine, Tree tree)
		{
			Tree = tree;
			_segmentationEngine = engine;
			_wasInsideTagStopContainer = false;
			_hashIndex = 0;
			ParagraphSegmentationInfo = new Dictionary<int, SegmentationInfo>();
			PrepareTextHints();
		}

		public void PrepareTextHints()
		{
			_isWordStop = false;
			TreeIterator treeIterator = new TreeIterator(Tree.Root.FirstChild);
			_topFirstParent = SegmentationHint.MayExclude;
			SegmentationInfo lastTextSegmentationInfo = null;
			if (treeIterator.CurrentNode == null)
			{
				return;
			}
			do
			{
				if (!treeIterator.CurrentNode.IsInsideTagContainer && !treeIterator.CurrentNode.IsInsideRevisionContainer && !treeIterator.CurrentNode.IsInsideCommentContainer)
				{
					_topFirstParent = SegmentationHint.MayExclude;
				}
				Node currentNode = treeIterator.CurrentNode;
				if (currentNode == null)
				{
					continue;
				}
				if (currentNode.IsSegment)
				{
					if (currentNode.HasNext)
					{
						treeIterator = new TreeIterator(currentNode.Next);
						continue;
					}
					break;
				}
				if (currentNode.IsPlaceholderTag)
				{
					DeterminePlaceholderSegmentationInfo(currentNode, treeIterator.CurrentNode.IsInsideTagAnyIncludeContainer, ref lastTextSegmentationInfo);
				}
				else if (currentNode.IsTagPair)
				{
					DetermineTagPairSegmentationInfo(currentNode, lastTextSegmentationInfo, treeIterator.CurrentNode.IsInsideIncludeRevisionOrMayExcludeContainer);
				}
				else if (currentNode.IsText)
				{
					Node text = currentNode;
					bool flag = currentNode.UniqueId > 0;
					DetermineTextSegmentationInfo(ref text, treeIterator.CurrentNode.IsInsideTagStopContainer, ref lastTextSegmentationInfo);
					_isWordStop = false;
					_wasInsideTagStopContainer = treeIterator.CurrentNode.IsInsideTagStopContainer;
					if (!flag)
					{
						_textBeforeLastText = _lastText;
						_lastText = text;
					}
				}
				else if (currentNode.IsCommentMarker)
				{
					DetermineCommentSegmentationInfo(currentNode);
				}
				else if (currentNode.IsRevisionMarker)
				{
					DetermineRevisionSegmentationInfo(currentNode);
				}
				else if (currentNode.IsLockedContent)
				{
					DetermineLockedContentSegmentationInfo(currentNode);
				}
				treeIterator.Next();
			}
			while (treeIterator.CurrentNode != null);
		}

		private void DetermineTextSegmentationInfo(ref Node text, bool isInsideTagStop, ref SegmentationInfo lastTextSegmentationInfo)
		{
			if (text.UniqueId > 0)
			{
				return;
			}
			SegmentationInfo segmInfoData = new SegmentationInfo
			{
				PrecededTagSpace = _isWordStop,
				IsInlineElement = true,
				SegmentationHintFinal = SegmentationHint.Include
			};
			if (text.Text == string.Empty)
			{
				text.UniqueId = ++_hashIndex;
				segmInfoData.IsInlineElement = false;
				ParagraphSegmentationInfo.Add(text.UniqueId, segmInfoData);
				return;
			}
			if (lastTextSegmentationInfo != null && !isInsideTagStop && _wasInsideTagStopContainer)
			{
				lastTextSegmentationInfo.FollowedTagSpace = true;
			}
			_updated = false;
			if (lastTextSegmentationInfo != null && _lastText != null && !lastTextSegmentationInfo.IsSegmentEnder)
			{
				UpdatePreviousSegmentationInfo(ref text, ref lastTextSegmentationInfo);
				_updated = true;
			}
			SplitTextItem(ref text, ref segmInfoData, lastTextSegmentationInfo);
			lastTextSegmentationInfo = segmInfoData;
			if (_lastSegmentStarter == null && segmInfoData.IsInlineElement && !segmInfoData.MayExclude && !text.IsWhitespace)
			{
				_lastSegmentStarter = text;
			}
			if (lastTextSegmentationInfo.IsSegmentEnder)
			{
				_lastSegmentStarter = null;
			}
		}

		private Node SplitOneTextItem(ref Node text, int splitPosition, ref SegmentationInfo segmInfoData)
		{
			if (splitPosition < 0)
			{
				return text;
			}
			Node node = text.SplitText(splitPosition, leadingSpaces: false);
			segmInfoData.FollowedTagSpace = false;
			text.UniqueId = ++_hashIndex;
			_previousInfo = segmInfoData;
			ParagraphSegmentationInfo.Add(text.UniqueId, segmInfoData);
			segmInfoData = segmInfoData.Clone();
			segmInfoData.IsSegmentEnder = false;
			segmInfoData.PrecededTagSpace = false;
			Node result = text;
			text = node;
			return result;
		}

		private void SplitTextItem(ref Node text, ref SegmentationInfo segmInfoData, SegmentationInfo lastTextSegmentationInfo)
		{
			if (text.UniqueId > 0)
			{
				return;
			}
			bool flag = false;
			string text2 = text.Text;
			bool flag2 = false;
			int num = -1;
			int num2 = (_updateChunk == null) ? (-1) : ((_textBeforeLastText != null) ? (_updateChunk.Length - _textBeforeLastText.TextCount) : _updateChunk.Length);
			if (lastTextSegmentationInfo != null && !lastTextSegmentationInfo.IsSegmentEnder && !string.IsNullOrEmpty(_lastText.Text) && _lastText.Text.Trim().Length != 0)
			{
				if (segmInfoData.PrecededTagSpace)
				{
					text2 = " " + text2;
				}
				text2 = _lastText.Text + text2;
				flag2 = true;
				flag = true;
				num = text2.Length;
			}
			segmInfoData.IsInlineElement = true;
			string text3 = text2;
			text2 = text2.TrimStart();
			string text4 = (text3.Length > text2.Length) ? text3.Substring(0, text3.Length - text2.Length) : string.Empty;
			Chunk chunk = _segmentationEngine.GetNextChunk(text2, 0, assumeEof: false, segmInfoData.FollowedTagSpace);
			bool flag3 = true;
			if (chunk != null && flag2 && _updateChunk != null && num2 > chunk.Length + text4.Length)
			{
				text2 = text.Text;
				text3 = text2;
				text2 = text2.TrimStart();
				text4 = ((text3.Length > text2.Length) ? text3.Substring(0, text3.Length - text2.Length) : string.Empty);
				flag2 = false;
				chunk = _segmentationEngine.GetNextChunk(text2, 0, assumeEof: false, segmInfoData.FollowedTagSpace);
			}
			if (chunk != null && chunk.Length == 1 && _updateChunk == null && _updated)
			{
				flag3 = false;
			}
			if ((chunk != null && chunk.Length != text2.TrimEnd().Length) & flag3)
			{
				text2 = text4 + text2;
				if (flag)
				{
					chunk.Length += text4.Length;
					chunk.Index = 0;
					if (segmInfoData.PrecededTagSpace)
					{
						chunk.Length--;
					}
					if (chunk.Length < 0)
					{
						chunk = null;
					}
				}
				int num3 = (_lastText == null || string.IsNullOrWhiteSpace(_lastText.Text) || !flag2) ? (chunk.Index + chunk.Length + text4.Length) : (chunk.Index + (chunk.Length + text4.Length - (_lastText.Text.Length + text4.Length)));
				while (chunk != null)
				{
					if (num3 == -1)
					{
						num3 = chunk.Index + chunk.Length + text4.Length;
					}
					if (num3 == text2.Length || num3 <= 0 || num3 == text.Text.Length)
					{
						break;
					}
					if (chunk.ChunkType == ChunkType.Whitespace)
					{
						text2 = text.Text;
						text3 = text2;
						text2 = text2.TrimStart();
						text4 = ((text3.Length > text2.Length) ? text3.Substring(0, text3.Length - text2.Length) : string.Empty);
						chunk = _segmentationEngine.GetNextChunk(text2, num3, assumeEof: false, segmInfoData.FollowedTagSpace);
						text2 = text4 + text2;
					}
					else
					{
						segmInfoData.IsSegmentEnder = true;
						_lastText = SplitOneTextItem(ref text, num3, ref segmInfoData);
						num3 = -1;
						text2 = text.Text;
						text3 = text2;
						text2 = text2.TrimStart();
						text4 = ((text3.Length > text2.Length) ? text3.Substring(0, text3.Length - text2.Length) : string.Empty);
						chunk = _segmentationEngine.GetNextChunk(text2, 0, assumeEof: false, followedByWordBreakTag: false);
						text2 = text4 + text2;
					}
				}
			}
			if (IsTextSegmentEnder(text))
			{
				segmInfoData.IsSegmentEnder = true;
			}
			text.UniqueId = ++_hashIndex;
			_previousInfo = segmInfoData;
			ParagraphSegmentationInfo.Add(text.UniqueId, segmInfoData);
		}

		private bool IsTextSegmentEnder(Node text)
		{
			Node parent = text.Parent;
			if (text.IsWhitespace)
			{
				return false;
			}
			if (text.BranchIndex != text.BranchCount - 1)
			{
				return false;
			}
			while (!parent.IsRoot)
			{
				if (ParagraphSegmentationInfo[parent.UniqueId].SegmentationHintFinal == SegmentationHint.Exclude)
				{
					return true;
				}
				Node node = parent;
				if (node.BranchIndex != node.BranchCount - 1)
				{
					return false;
				}
				parent = parent.Parent;
			}
			return false;
		}

		private void UpdatePreviousSegmentationInfo(ref Node currentText, ref SegmentationInfo previousSegmentationInfo)
		{
			_updateChunk = null;
			int leadingWhitespaceCount = _lastText.LeadingWhitespaceCount;
			if (leadingWhitespaceCount == _lastText.Text.Length && _textBeforeLastText == null)
			{
				return;
			}
			if (_isWordStop)
			{
				previousSegmentationInfo.FollowedTagSpace = true;
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (_textBeforeLastText != null && !ParagraphSegmentationInfo[_textBeforeLastText.UniqueId].IsSegmentEnder)
			{
				stringBuilder.Append(_textBeforeLastText.Text);
				if (previousSegmentationInfo.PrecededTagSpace)
				{
					stringBuilder.Append(" ");
				}
			}
			stringBuilder.Append(_lastText.Text);
			if (previousSegmentationInfo.FollowedTagSpace)
			{
				stringBuilder.Append(" ");
			}
			stringBuilder.Append(currentText.Text);
			string text = _updatedChunkTextToBreak = stringBuilder.ToString().TrimStart();
			_updateChunk = _segmentationEngine.GetNextChunk(text, 0, assumeEof: false, previousSegmentationInfo.FollowedTagSpace);
			if (_updateChunk == null)
			{
				return;
			}
			int length = _lastText.Text.TrimStart().Length;
			if (_lastText.Text.EndsWith("\r\n"))
			{
				string text2 = _lastText.Text;
				char[] trimChars = new char[3]
				{
					'\r',
					'\n',
					' '
				};
				text2 = text2.TrimEnd(trimChars);
				text2 = text2.TrimStart();
				length = text2.Length;
			}
			else if (_lastText.Text.EndsWith("\n"))
			{
				string text3 = _lastText.Text;
				char[] trimChars2 = new char[2]
				{
					'\n',
					' '
				};
				text3 = text3.TrimEnd(trimChars2);
				text3 = text3.TrimStart();
				length = text3.Length;
			}
			string text4 = _lastText.Text;
			string text5 = _lastText.Text.TrimStart();
			string text6 = (text4.Length > text5.Length) ? text4.Substring(0, text4.Length - text5.Length) : string.Empty;
			if (_updateChunk.Length > 1 && _updateChunk.Length <= text.Length - ((!currentText.IsWhitespace) ? currentText.Text.Length : 0))
			{
				if (length > _updateChunk.Length)
				{
					int num = _updateChunk.Length + text6.Length;
					if (char.IsWhiteSpace(_lastText.Text[0]))
					{
						num++;
					}
					SegmentationInfo segmInfoData = new SegmentationInfo
					{
						IsInlineElement = true,
						IsSegmentEnder = false
					};
					Node node = SplitOneTextItem(ref _lastText, num, ref segmInfoData);
					if (_lastText.UniqueId == 0)
					{
						_lastText.UniqueId = ++_hashIndex;
					}
					ParagraphSegmentationInfo.Add(_lastText.UniqueId, segmInfoData);
					ParagraphSegmentationInfo[node.UniqueId].IsSegmentEnder = true;
				}
				else if (_lastText.IsWhitespace && _lastText.UniqueId > 1)
				{
					ParagraphSegmentationInfo[_lastText.UniqueId - 1].IsSegmentEnder = true;
				}
				else
				{
					previousSegmentationInfo.IsSegmentEnder = true;
				}
			}
			else if ((_updateChunk.Length - text.Length + currentText.Text.Length == currentText.LeadingWhitespaceCount + 1 && _updateChunk.Length != text.Length) || _updateChunk.Length == 1)
			{
				SegmentationInfo segmInfoData2 = previousSegmentationInfo = new SegmentationInfo
				{
					IsInlineElement = true,
					IsSegmentEnder = true
				};
				_lastText = SplitOneTextItem(ref currentText, _updateChunk.Length - text.Length + currentText.Text.Length, ref segmInfoData2);
			}
		}

		private void DetermineLockedContentSegmentationInfo(Node content)
		{
			if (content.Parent.IsRoot && content.BranchCount == 1)
			{
				SegmentationInfo value = new SegmentationInfo
				{
					IsInlineElement = false
				};
				ParagraphSegmentationInfo.Add(content.UniqueId, value);
				return;
			}
			SegmentationInfo segmentationInfo = new SegmentationInfo
			{
				IsInlineElement = true,
				SegmentationHintFinal = content.SegmentationHint
			};
			segmentationInfo.IsAfterSegmentEnder = (segmentationInfo.SegmentationHintFinal == SegmentationHint.Exclude);
			content.UniqueId = ++_hashIndex;
			_previousInfo = segmentationInfo;
			ParagraphSegmentationInfo.Add(content.UniqueId, segmentationInfo);
		}

		private void DetermineRevisionSegmentationInfo(Node revisionMarker)
		{
			if (_topFirstParent == SegmentationHint.MayExclude)
			{
				_topFirstParent = SegmentationHint.Include;
			}
			if (revisionMarker.UniqueId == 0)
			{
				SegmentationInfo segmentationInfo = new SegmentationInfo
				{
					IsInlineElement = true,
					SegmentationHintFinal = SegmentationHint.Include
				};
				revisionMarker.UniqueId = ++_hashIndex;
				_previousInfo = segmentationInfo;
				ParagraphSegmentationInfo.Add(revisionMarker.UniqueId, segmentationInfo);
			}
		}

		private void DetermineCommentSegmentationInfo(Node commentMarker)
		{
			if (_topFirstParent == SegmentationHint.MayExclude)
			{
				_topFirstParent = SegmentationHint.Include;
			}
			if (commentMarker.UniqueId == 0)
			{
				SegmentationInfo segmentationInfo = new SegmentationInfo
				{
					IsInlineElement = true,
					SegmentationHintFinal = SegmentationHint.Include
				};
				commentMarker.UniqueId = ++_hashIndex;
				_previousInfo = segmentationInfo;
				ParagraphSegmentationInfo.Add(commentMarker.UniqueId, segmentationInfo);
			}
		}

		private void DetermineTagPairSegmentationInfo(Node tagPair, SegmentationInfo lastTextSegmentationInfo, bool isInsideIncludeRevisionOrMayExcludeContainer)
		{
			if (tagPair.UniqueId > 0)
			{
				return;
			}
			if (tagPair.IsWordStop)
			{
				_isWordStop = true;
				if (lastTextSegmentationInfo != null)
				{
					lastTextSegmentationInfo.FollowedTagSpace = true;
				}
			}
			SegmentationInfo segmentationInfo = new SegmentationInfo
			{
				MayExclude = tagPair.MayExclude
			};
			tagPair.UniqueId = ++_hashIndex;
			_previousInfo = segmentationInfo;
			ParagraphSegmentationInfo.Add(tagPair.UniqueId, segmentationInfo);
			segmentationInfo.SegmentationHintFinal = tagPair.SegmentationHint;
			if (_topFirstParent == SegmentationHint.MayExclude)
			{
				_topFirstParent = segmentationInfo.SegmentationHintFinal;
			}
			if (tagPair.SegmentationHint == SegmentationHint.MayExclude)
			{
				if (_previousInfo.IsSegmentEnder)
				{
					segmentationInfo.IsAfterSegmentEnder = true;
				}
				segmentationInfo.IsSegmentEnder = false;
			}
			else if ((tagPair.SegmentationHint == SegmentationHint.Exclude && isInsideIncludeRevisionOrMayExcludeContainer) || _topFirstParent == SegmentationHint.Include)
			{
				segmentationInfo.SegmentationHintFinal = SegmentationHint.Include;
			}
			segmentationInfo.IsAfterSegmentEnder = (segmentationInfo.SegmentationHintFinal == SegmentationHint.Exclude);
			segmentationInfo.IsInlineElement = (segmentationInfo.IsAfterSegmentEnder || segmentationInfo.SegmentationHintFinal == SegmentationHint.Include || segmentationInfo.SegmentationHintFinal == SegmentationHint.IncludeWithText);
			if (segmentationInfo.SegmentationHintFinal == SegmentationHint.IncludeWithText && tagPair.Parent != null && !tagPair.Parent.IsRoot && tagPair.BranchCount == 1 && ParagraphSegmentationInfo[tagPair.Parent.UniqueId].MayExclude)
			{
				segmentationInfo.IsInlineElement = false;
			}
		}

		private void DeterminePlaceholderSegmentationInfo(Node placeholderTag, bool insideIncludeParent, ref SegmentationInfo lastTextSegmentationInfo)
		{
			SegmentationHint segmentationHint = placeholderTag.SegmentationHint;
			if (placeholderTag.UniqueId > 0)
			{
				return;
			}
			bool flag = placeholderTag.BranchIndex != 0 && placeholderTag.BranchIndex != placeholderTag.Parent.BranchCount - 1;
			SegmentationInfo segmentationInfo = new SegmentationInfo
			{
				MayExclude = (segmentationHint == SegmentationHint.MayExclude)
			};
			if (placeholderTag.IsWordStop)
			{
				_isWordStop = true;
				if (lastTextSegmentationInfo != null)
				{
					lastTextSegmentationInfo.FollowedTagSpace = true;
				}
			}
			segmentationInfo.SegmentationHintFinal = segmentationHint;
			if (insideIncludeParent && segmentationInfo.SegmentationHintFinal == SegmentationHint.Exclude)
			{
				segmentationInfo.SegmentationHintFinal = SegmentationHint.Include;
			}
			switch (segmentationHint)
			{
			case SegmentationHint.Exclude:
				if ((_topFirstParent == SegmentationHint.Include || placeholderTag.Parent.IsRevisionMarker || placeholderTag.Parent.IsCommentMarker) & flag)
				{
					segmentationInfo.SegmentationHintFinal = SegmentationHint.Include;
				}
				break;
			case SegmentationHint.MayExclude:
				segmentationInfo.SegmentationHintFinal = DeterminePlaceholderMayExcludeHint(placeholderTag);
				break;
			}
			bool flag2 = false;
			if (segmentationHint == SegmentationHint.Exclude && flag && placeholderTag.Parent.MayExclude)
			{
				segmentationInfo.SegmentationHintFinal = SegmentationHint.Include;
				flag2 = true;
			}
			if (_hashIndex == 1 && segmentationInfo.SegmentationHintFinal == SegmentationHint.Exclude && _topFirstParent == SegmentationHint.Include)
			{
				LeadingPeExclude(placeholderTag);
			}
			if (CheckIfTrailingPe(placeholderTag, segmentationInfo, _previousInfo))
			{
				TrailingPeExclude(placeholderTag);
			}
			segmentationInfo.IsAfterSegmentEnder = (segmentationInfo.SegmentationHintFinal == SegmentationHint.Exclude);
			segmentationInfo.IsInlineElement = (!flag2 && (segmentationInfo.IsAfterSegmentEnder || segmentationInfo.SegmentationHintFinal == SegmentationHint.Include || segmentationInfo.SegmentationHintFinal == SegmentationHint.IncludeWithText));
			if (segmentationInfo.SegmentationHintFinal == SegmentationHint.IncludeWithText)
			{
				segmentationInfo.IsInlineElement = !placeholderTag.IsAloneInMayExcludeContainer;
			}
			if ((segmentationInfo.SegmentationHintFinal == SegmentationHint.Include || segmentationInfo.SegmentationHintFinal == SegmentationHint.IncludeWithText) && _lastSegmentStarter == null && !segmentationInfo.MayExclude)
			{
				_lastSegmentStarter = placeholderTag;
			}
			if (segmentationInfo.SegmentationHintFinal == SegmentationHint.Include && _lastSegmentStarter == null && segmentationInfo.MayExclude)
			{
				segmentationInfo.SegmentationHintFinal = SegmentationHint.Exclude;
			}
			placeholderTag.UniqueId = ++_hashIndex;
			_previousInfo = segmentationInfo;
			ParagraphSegmentationInfo.Add(placeholderTag.UniqueId, segmentationInfo);
		}

		private static bool CheckIfTrailingPe(Node placeholderTag, SegmentationInfo segmentationInfo, SegmentationInfo previousSegmInfo)
		{
			if (previousSegmInfo != null && !previousSegmInfo.IsSegmentEnder)
			{
				return false;
			}
			if (segmentationInfo.SegmentationHintFinal == SegmentationHint.Exclude && placeholderTag.BranchIndex == placeholderTag.Parent.BranchCount - 1)
			{
				return placeholderTag.BranchIndex > 0;
			}
			return false;
		}

		private void TrailingPeExclude(Node placeholderTag)
		{
			if (placeholderTag.UniqueId <= 0)
			{
				Node node = placeholderTag;
				SegmentationInfo segmentationInfo = new SegmentationInfo
				{
					IsInlineElement = true,
					IsAfterSegmentEnder = true
				};
				Node previous;
				while (node.BranchIndex - 1 > 0 && (previous = node.Previous) != null && previous.Exclude)
				{
					node = previous;
				}
				Node node2 = node.Split();
				node2.UniqueId = ++_hashIndex;
				_previousInfo = segmentationInfo;
				ParagraphSegmentationInfo.Add(node2.UniqueId, segmentationInfo);
			}
		}

		private void LeadingPeExclude(Node placeholderTag)
		{
			if (placeholderTag.Parent.IsRoot || placeholderTag.Parent.BranchCount == 1)
			{
				return;
			}
			Node node = placeholderTag;
			Node next;
			while (node.Parent.BranchCount > node.BranchIndex + 1 && (next = node.Next) != null)
			{
				next.UniqueId = ++_hashIndex;
				ParagraphSegmentationInfo.Add(next.UniqueId, new SegmentationInfo
				{
					IsInlineElement = false
				});
				if (!next.Exclude)
				{
					break;
				}
				node = next;
			}
			Node parent = node.Parent;
			node.Split();
			ParagraphSegmentationInfo[parent.UniqueId].IsAfterSegmentEnder = true;
			ParagraphSegmentationInfo[parent.UniqueId].IsInlineElement = true;
		}

		private SegmentationHint DeterminePlaceholderMayExcludeHint(Node placeholderTag)
		{
			SegmentationHint result = SegmentationHint.Include;
			if (placeholderTag.BranchIndex < placeholderTag.Parent.BranchCount - 1)
			{
				Node next = placeholderTag.Parent.Next;
				if (next != null && (next.IsTagPair || next.IsPlaceholderTag) && next.Exclude)
				{
					result = SegmentationHint.Exclude;
				}
			}
			if (_previousInfo == null)
			{
				result = SegmentationHint.Exclude;
			}
			if (_previousInfo != null && _previousInfo.SegmentationHintFinal == SegmentationHint.Exclude)
			{
				result = SegmentationHint.Exclude;
			}
			if (placeholderTag.BranchIndex == placeholderTag.Parent.BranchCount - 1)
			{
				result = SegmentationHint.Exclude;
			}
			return result;
		}

		public virtual void ApplySegmentationInfo()
		{
			TreeIterator treeIterator = new TreeIterator(Tree.Root.FirstChild);
			while (treeIterator.CurrentNode != null)
			{
				Node currentNode = treeIterator.CurrentNode;
				if (ParagraphSegmentationInfo != null && ParagraphSegmentationInfo.ContainsKey(currentNode.UniqueId))
				{
					currentNode.IsSegmentEnder = ParagraphSegmentationInfo[currentNode.UniqueId].IsSegmentEnder;
				}
				treeIterator.Next();
			}
		}
	}
}
