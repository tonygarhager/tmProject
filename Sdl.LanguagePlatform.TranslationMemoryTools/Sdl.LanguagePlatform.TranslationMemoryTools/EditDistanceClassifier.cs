using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.EditDistance;
using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemoryTools
{
	public class EditDistanceClassifier
	{
		private readonly List<EditDistanceItem> _editDistanceItems;

		public List<ClassifiedTagPairInfo> AddedTagPairs
		{
			get;
			set;
		} = new List<ClassifiedTagPairInfo>();


		public List<ClassifiedTagPairInfo> DeletedTagPairs
		{
			get;
			set;
		} = new List<ClassifiedTagPairInfo>();


		public List<ClassifiedTagPairInfo> MovedTagPairs
		{
			get;
			set;
		} = new List<ClassifiedTagPairInfo>();


		public List<EditDistanceChangeSequence> ChangeSequences
		{
			get;
			set;
		} = new List<EditDistanceChangeSequence>();


		public List<EditDistanceItem> Other
		{
			get;
			set;
		} = new List<EditDistanceItem>();


		public Segment LinguaFromSegment
		{
			get;
			set;
		}

		public Segment LinguaToSegment
		{
			get;
			set;
		}

		public EditDistanceClassifier(List<EditDistanceItem> editDistanceItems)
		{
			_editDistanceItems = editDistanceItems;
		}

		private ClassifiedTagPairInfo ExtractTagPairInfo(EditDistanceItem? start, EditDistanceItem? end)
		{
			ClassifiedTagPairInfo classifiedTagPairInfo = new ClassifiedTagPairInfo
			{
				Start = start,
				End = end,
				StartTokenPosition = (start.HasValue ? _editDistanceItems.IndexOf(start.Value) : (-1)),
				EndTokenPosition = (end.HasValue ? _editDistanceItems.IndexOf(end.Value) : (-1))
			};
			if (classifiedTagPairInfo.StartTokenPosition == -1 || classifiedTagPairInfo.EndTokenPosition == -1)
			{
				return classifiedTagPairInfo;
			}
			int num = classifiedTagPairInfo.StartTokenPosition + 1;
			int num2 = classifiedTagPairInfo.EndTokenPosition - 1 - num + 1;
			if (num2 > 0)
			{
				classifiedTagPairInfo.Subitems = _editDistanceItems.GetRange(num, num2);
			}
			return classifiedTagPairInfo;
		}

		public void Execute()
		{
			Dictionary<EditDistanceItem, bool> dictionary = new Dictionary<EditDistanceItem, bool>();
			Dictionary<int, EditDistanceItem> dictionary2 = new Dictionary<int, EditDistanceItem>();
			for (int i = 0; i < _editDistanceItems.Count; i++)
			{
				EditDistanceItem editDistanceItem = _editDistanceItems[i];
				if (dictionary.ContainsKey(editDistanceItem))
				{
					continue;
				}
				switch (editDistanceItem.Operation)
				{
				case EditOperation.Identity:
					Other.Add(editDistanceItem);
					break;
				case EditOperation.Change:
					dictionary2[editDistanceItem.Source] = editDistanceItem;
					Other.Add(editDistanceItem);
					break;
				case EditOperation.Move:
				{
					Token token3 = LinguaFromSegment.Tokens[editDistanceItem.Source];
					if (!IsPairedTag(token3))
					{
						Other.Add(editDistanceItem);
						break;
					}
					TagToken tagToken = LinguaToSegment.Tokens[editDistanceItem.Target] as TagToken;
					if (tagToken == null)
					{
						Other.Add(editDistanceItem);
						break;
					}
					EditDistanceItem? editDistanceItem4 = FindOtherMovedTagEnd(tagToken, i);
					if (IsStartTag(token3))
					{
						ClassifiedTagPairInfo item5 = ExtractTagPairInfo(editDistanceItem, editDistanceItem4);
						MovedTagPairs.Add(item5);
					}
					else
					{
						ClassifiedTagPairInfo item6 = ExtractTagPairInfo(editDistanceItem4, editDistanceItem);
						MovedTagPairs.Add(item6);
					}
					dictionary.Add(editDistanceItem, value: true);
					if (editDistanceItem4.HasValue)
					{
						dictionary.Add(editDistanceItem4.Value, value: true);
					}
					break;
				}
				case EditOperation.Insert:
				{
					Token token2 = LinguaToSegment.Tokens[editDistanceItem.Target];
					if (!IsPairedTag(token2))
					{
						Other.Add(editDistanceItem);
						break;
					}
					EditDistanceItem? editDistanceItem3 = FindOtherInsertedTagEnd((TagToken)token2, i);
					EditDistanceItem? deleteOperation = FindDeletedTag(editDistanceItem, i);
					EditDistanceItem? deleteOperation2 = FindDeletedTag(editDistanceItem3, i);
					if (!editDistanceItem3.HasValue || deleteOperation.HasValue || deleteOperation2.HasValue)
					{
						EditDistanceItem? start2 = null;
						EditDistanceItem? end2 = null;
						if (deleteOperation.HasValue)
						{
							EditDistanceItem value3 = CreateMoveOperation(deleteOperation, editDistanceItem);
							if (!IsStartTag(token2))
							{
								end2 = value3;
							}
							else
							{
								start2 = value3;
							}
						}
						if (deleteOperation2.HasValue)
						{
							EditDistanceItem value4 = CreateMoveOperation(deleteOperation2, editDistanceItem3);
							if (!IsStartTag(token2))
							{
								start2 = value4;
							}
							else
							{
								end2 = value4;
							}
						}
						if (start2.HasValue && end2.HasValue)
						{
							ClassifiedTagPairInfo item3 = ExtractTagPairInfo(start2, end2);
							MovedTagPairs.Add(item3);
						}
						if (deleteOperation.HasValue)
						{
							dictionary.Add(deleteOperation.Value, value: true);
						}
						if (deleteOperation2.HasValue)
						{
							dictionary.Add(deleteOperation2.Value, value: true);
						}
					}
					else
					{
						ClassifiedTagPairInfo item4 = ExtractTagPairInfo(editDistanceItem, editDistanceItem3);
						AddedTagPairs.Add(item4);
					}
					dictionary.Add(editDistanceItem, value: true);
					if (editDistanceItem3.HasValue)
					{
						dictionary.Add(editDistanceItem3.Value, value: true);
					}
					break;
				}
				case EditOperation.Delete:
				{
					Token token = LinguaFromSegment.Tokens[editDistanceItem.Source];
					if (!IsPairedTag(token))
					{
						Other.Add(editDistanceItem);
						break;
					}
					EditDistanceItem? editDistanceItem2 = FindOtherDeletedTagEnd((TagToken)token, i);
					EditDistanceItem? insertOperation = FindInsertedTag(editDistanceItem, i);
					EditDistanceItem? insertOperation2 = FindInsertedTag(editDistanceItem2, i);
					if (!editDistanceItem2.HasValue || insertOperation.HasValue || insertOperation2.HasValue)
					{
						EditDistanceItem? start = null;
						EditDistanceItem? end = null;
						if (insertOperation.HasValue)
						{
							EditDistanceItem value = CreateMoveOperation(editDistanceItem, insertOperation);
							if (!IsStartTag(token))
							{
								end = value;
							}
							else
							{
								start = value;
							}
						}
						if (insertOperation2.HasValue)
						{
							EditDistanceItem value2 = CreateMoveOperation(editDistanceItem2, insertOperation2);
							if (!IsStartTag(token))
							{
								start = value2;
							}
							else
							{
								end = value2;
							}
						}
						if (start.HasValue || end.HasValue)
						{
							ClassifiedTagPairInfo item = ExtractTagPairInfo(start, end);
							MovedTagPairs.Add(item);
						}
						if (insertOperation.HasValue)
						{
							dictionary.Add(insertOperation.Value, value: true);
						}
						if (insertOperation2.HasValue)
						{
							dictionary.Add(insertOperation2.Value, value: true);
						}
					}
					else
					{
						ClassifiedTagPairInfo item2 = ExtractTagPairInfo(editDistanceItem, editDistanceItem2);
						DeletedTagPairs.Add(item2);
					}
					dictionary.Add(editDistanceItem, value: true);
					if (editDistanceItem2.HasValue)
					{
						dictionary.Add(editDistanceItem2.Value, value: true);
					}
					break;
				}
				}
			}
			int num = -1;
			EditDistanceChangeSequence editDistanceChangeSequence = null;
			foreach (KeyValuePair<int, EditDistanceItem> item7 in dictionary2)
			{
				if (num >= 0 && item7.Key == num + 1)
				{
					EditDistanceItem editDistanceItem5 = dictionary2[num];
					if (EditDistanceChangeSequence.AreItemsCompatible(editDistanceItem5, item7.Value))
					{
						if (editDistanceChangeSequence == null)
						{
							editDistanceChangeSequence = new EditDistanceChangeSequence();
							ChangeSequences.Add(editDistanceChangeSequence);
							editDistanceChangeSequence.Add(editDistanceItem5);
							Other.Remove(editDistanceItem5);
						}
						editDistanceChangeSequence.Add(item7.Value);
						Other.Remove(item7.Value);
					}
					else
					{
						editDistanceChangeSequence = null;
					}
				}
				else
				{
					editDistanceChangeSequence = null;
				}
				num = item7.Key;
			}
		}

		private static EditDistanceItem CreateMoveOperation(EditDistanceItem? deleteOperation, EditDistanceItem? insertOperation)
		{
			if (!deleteOperation.HasValue)
			{
				throw new ArgumentNullException("deleteOperation");
			}
			if (!insertOperation.HasValue)
			{
				throw new ArgumentNullException("insertOperation");
			}
			EditDistanceItem result = default(EditDistanceItem);
			result.Operation = EditOperation.Move;
			result.Source = deleteOperation.Value.Source;
			result.MoveSourceTarget = deleteOperation.Value.Target;
			result.MoveTargetSource = insertOperation.Value.Source;
			result.Target = insertOperation.Value.Target;
			return result;
		}

		private static bool IsPairedTag(Token token)
		{
			if (token == null)
			{
				return false;
			}
			if (token.Type != TokenType.Tag)
			{
				return false;
			}
			if (IsPlaceholderTag(token))
			{
				return false;
			}
			return true;
		}

		private static bool IsStartTag(Token token)
		{
			TagToken tagToken = token as TagToken;
			if (tagToken == null)
			{
				return false;
			}
			if (tagToken.Tag == null)
			{
				return false;
			}
			return tagToken.Tag.Type == TagType.Start;
		}

		private static bool IsEndTag(Token token)
		{
			TagToken tagToken = token as TagToken;
			if (tagToken == null)
			{
				return false;
			}
			if (tagToken.Tag == null)
			{
				return false;
			}
			return tagToken.Tag.Type == TagType.End;
		}

		private static bool IsPlaceholderTag(Token token)
		{
			TagToken tagToken = token as TagToken;
			if (tagToken == null)
			{
				return false;
			}
			if (tagToken.Tag == null)
			{
				return true;
			}
			if (tagToken.Tag.Type != TagType.Standalone && tagToken.Tag.Type != TagType.TextPlaceholder)
			{
				return tagToken.Tag.Type == TagType.LockedContent;
			}
			return true;
		}

		private EditDistanceItem? FindInsertedTag(EditDistanceItem? deleteOperation, int startFromIndex)
		{
			if (!deleteOperation.HasValue)
			{
				return null;
			}
			TagToken first = (TagToken)LinguaFromSegment.Tokens[deleteOperation.Value.Source];
			for (int i = startFromIndex; i < _editDistanceItems.Count; i++)
			{
				EditDistanceItem value = _editDistanceItems[i];
				if (value.Operation == EditOperation.Insert)
				{
					TagToken tagToken = LinguaToSegment.Tokens[value.Target] as TagToken;
					if (tagToken != null && IsCorrespondingTag(first, tagToken))
					{
						return value;
					}
				}
			}
			return null;
		}

		private EditDistanceItem? FindDeletedTag(EditDistanceItem? insertOperation, int startFromIndex)
		{
			if (!insertOperation.HasValue)
			{
				return null;
			}
			TagToken first = (TagToken)LinguaToSegment.Tokens[insertOperation.Value.Target];
			for (int i = startFromIndex; i < _editDistanceItems.Count; i++)
			{
				EditDistanceItem value = _editDistanceItems[i];
				if (value.Operation == EditOperation.Delete)
				{
					TagToken tagToken = LinguaFromSegment.Tokens[value.Source] as TagToken;
					if (tagToken != null && IsCorrespondingTag(first, tagToken))
					{
						return value;
					}
				}
			}
			return null;
		}

		private static bool IsCorrespondingTag(TagToken first, TagToken second)
		{
			if (first.Tag.Type != second.Tag.Type)
			{
				return false;
			}
			return first.Tag.TagID == second.Tag.TagID;
		}

		private EditDistanceItem? FindOtherMovedTagEnd(TagToken sourceToken, int startFromIndex)
		{
			for (int i = startFromIndex; i < _editDistanceItems.Count; i++)
			{
				EditDistanceItem value = _editDistanceItems[i];
				if (value.Operation == EditOperation.Move)
				{
					TagToken tagToken = LinguaToSegment.Tokens[value.Target] as TagToken;
					if (IsPairedTag(tagToken) && IsOtherTagEnd(sourceToken.Tag, tagToken.Tag))
					{
						return value;
					}
				}
			}
			return null;
		}

		private EditDistanceItem? FindOtherInsertedTagEnd(TagToken targetToken, int startFromIndex)
		{
			for (int i = startFromIndex; i < _editDistanceItems.Count; i++)
			{
				EditDistanceItem value = _editDistanceItems[i];
				if (value.Operation == EditOperation.Insert)
				{
					TagToken tagToken = LinguaToSegment.Tokens[value.Target] as TagToken;
					if (IsPairedTag(tagToken) && IsOtherTagEnd(targetToken.Tag, tagToken.Tag))
					{
						return value;
					}
				}
			}
			return null;
		}

		private static bool IsOtherTagEnd(Tag first, Tag second)
		{
			if ((first.Type == TagType.Start && second.Type == TagType.End) || (first.Type == TagType.End && second.Type == TagType.Start))
			{
				return first.Anchor == second.Anchor;
			}
			return false;
		}

		private EditDistanceItem? FindOtherDeletedTagEnd(TagToken sourceToken, int startFromIndex)
		{
			for (int i = startFromIndex; i < _editDistanceItems.Count; i++)
			{
				EditDistanceItem value = _editDistanceItems[i];
				if (value.Operation == EditOperation.Delete)
				{
					TagToken tagToken = LinguaFromSegment.Tokens[value.Source] as TagToken;
					if (IsPairedTag(tagToken) && IsOtherTagEnd(sourceToken.Tag, tagToken.Tag))
					{
						return value;
					}
				}
			}
			return null;
		}
	}
}
