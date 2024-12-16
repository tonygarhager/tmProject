using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.EditDistance;
using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sdl.Core.Processing.Alignment.RetrofitApplier
{
	public class EdUtilities
	{
		public static void PrepareEditDistance(EdApplierContext context, bool enablelogs, string dirLog, string edFileLogName)
		{
			ReplaceMoveOperations(context);
			OrderEditDistance(context);
			MergeConsecutiveInsertDeleteTagsAsChanges(context.AllEditDistanceItems, context.UpdatedLinguaSegment.Tokens, context.OriginalLinguaSegment.Tokens);
			MarkWhiteSpaceBetweenTextChangesAsChange(context);
			TagChangesConvertToEquality(context);
			PhChangesConvertToEquality(context);
			CommentChangesConvertToEquality(context);
			SetDistance0(context);
			if (enablelogs)
			{
				LogEditDistance(context.AllEditDistanceItems, context.OriginalLinguaSegment.Tokens, context.UpdatedLinguaSegment.Tokens, dirLog, edFileLogName);
			}
		}

		private static void MarkWhiteSpaceBetweenTextChangesAsChange(EdApplierContext context)
		{
			TextChanges textChanges = TextChanges.noText;
			int i = 0;
			for (int j = 0; j < context.AllEditDistanceItems.Count; j++)
			{
				EditDistanceItem ed = context.AllEditDistanceItems[j];
				switch (textChanges)
				{
				case TextChanges.noText:
					if (IsTextChange(context, ed))
					{
						textChanges = TextChanges.Text;
					}
					break;
				case TextChanges.Text:
					if (IsWhiteSpaceIdentity(context, ed))
					{
						i = j;
						textChanges = TextChanges.TextSpace;
					}
					else
					{
						textChanges = TextChanges.noText;
					}
					break;
				case TextChanges.TextSpace:
					textChanges = (IsTextChange(context, ed) ? TextChanges.TextSpaceText : TextChanges.noText);
					break;
				case TextChanges.TextSpaceText:
					ReplaceEditDistanceItem(context, i, EditOperation.Change);
					textChanges = TextChanges.Text;
					break;
				}
			}
		}

		private static void TagChangesConvertToEquality(EdApplierContext context)
		{
			for (int i = 0; i < context.AllEditDistanceItems.Count; i++)
			{
				EditDistanceItem ed = context.AllEditDistanceItems[i];
				if (IsTagChange(context, ed))
				{
					TagToken tagToken = context.OriginalLinguaSegment.Tokens[ed.Source] as TagToken;
					ITagPair tagPair = context.Map.OriginalTag[tagToken.Tag.Anchor];
					TagToken tagToken2 = context.UpdatedLinguaSegment.Tokens[ed.Target] as TagToken;
					ITagPair tagPair2 = context.Map.UpdateTag[tagToken2.Tag.Anchor];
					if (tagPair != null && tagPair2 != null && AreTagsEqual(tagPair, tagPair2))
					{
						ReplaceEditDistanceItem(context, i, EditOperation.Identity);
					}
				}
			}
		}

		private static void CommentChangesConvertToEquality(EdApplierContext context)
		{
			for (int i = 0; i < context.AllEditDistanceItems.Count; i++)
			{
				EditDistanceItem ed = context.AllEditDistanceItems[i];
				if (IsCommentChange(context, ed))
				{
					TagToken tagToken = context.OriginalLinguaSegment.Tokens[ed.Source] as TagToken;
					CommentMarker commentMarker = context.Map.OriginalComment[tagToken.Tag.Anchor] as CommentMarker;
					TagToken tagToken2 = context.UpdatedLinguaSegment.Tokens[ed.Target] as TagToken;
					CommentMarker commentMarker2 = context.Map.UpdateComment[tagToken2.Tag.Anchor] as CommentMarker;
					if (commentMarker != null && commentMarker2 != null && AreCommentEqual(commentMarker, commentMarker2))
					{
						ReplaceEditDistanceItem(context, i, EditOperation.Identity);
					}
				}
			}
		}

		private static void PhChangesConvertToEquality(EdApplierContext context)
		{
			for (int i = 0; i < context.AllEditDistanceItems.Count; i++)
			{
				EditDistanceItem ed = context.AllEditDistanceItems[i];
				if (IsPhChange(context, ed))
				{
					TagToken tagToken = context.OriginalLinguaSegment.Tokens[ed.Source] as TagToken;
					IPlaceholderTag placeholderTag = context.Map.OriginalPh[tagToken.Tag.Anchor];
					TagToken tagToken2 = context.UpdatedLinguaSegment.Tokens[ed.Target] as TagToken;
					IPlaceholderTag placeholderTag2 = context.Map.UpdatePh[tagToken2.Tag.Anchor];
					if (placeholderTag != null && placeholderTag2 != null && ArePhsEqual(placeholderTag, placeholderTag2))
					{
						ReplaceEditDistanceItem(context, i, EditOperation.Identity);
					}
				}
			}
		}

		private static void SetDistance0(EdApplierContext context)
		{
			if (context.AllEditDistanceItems.All((EditDistanceItem x) => x.Operation == EditOperation.Identity))
			{
				context.EditDistance.Distance = 0.0;
			}
		}

		public static bool AreTagsEqual(ITagPair source, ITagPair target)
		{
			return source.TagProperties.TagContent.ToLowerInvariant().Equals(target.TagProperties.TagContent.ToLowerInvariant());
		}

		private static bool AreCommentEqual(CommentMarker source, CommentMarker target)
		{
			return source.Comments.Equals(target.Comments);
		}

		public static bool ArePhsEqual(IPlaceholderTag source, IPlaceholderTag target)
		{
			return source.Properties.DisplayText.Equals(target.Properties.DisplayText);
		}

		public static void LogEditDistance(List<EditDistanceItem> allEditDistanceItems, List<Token> sourceTokens, List<Token> targetTokens, string dirLog, string edFileLogName)
		{
			string text = ";";
			if (!Directory.Exists(dirLog))
			{
				Directory.CreateDirectory(dirLog);
			}
			EditDistanceItem editDistanceItem = default(EditDistanceItem);
			editDistanceItem.Source = -1;
			EditDistanceItem editDistanceItem2 = editDistanceItem;
			string str = "LIndex; Source; Op&Index; Target; RIndex" + Environment.NewLine;
			string text2 = string.Empty;
			string text3 = string.Empty;
			for (int i = 0; i < allEditDistanceItems.Count; i++)
			{
				EditDistanceItem editDistanceItem3 = allEditDistanceItems[i];
				if (editDistanceItem2.Source != -1 && editDistanceItem3.Operation != EditOperation.Move)
				{
					string text4 = "";
					string text5 = "";
					if (editDistanceItem3.Source - editDistanceItem2.Source == 2)
					{
						text4 = (editDistanceItem3.Source - 1).ToString();
						text2 = sourceTokens[editDistanceItem3.Source - 1].Text;
						text3 = "_";
						string str2 = text4 + text + text2 + text + "halfMove" + text + text3 + text + text5;
						str = str + str2 + Environment.NewLine;
					}
					if (editDistanceItem3.Target - editDistanceItem2.Target == 2)
					{
						text5 = (editDistanceItem3.Target - 1).ToString();
						text2 = "_";
						text3 = targetTokens[editDistanceItem3.Target - 1].Text;
						string str3 = text4 + text + " " + text2 + text + " halfMove" + text + " " + text3 + text + " " + text5;
						str = str + str3 + Environment.NewLine;
					}
				}
				string empty = string.Empty;
				switch (editDistanceItem3.Operation)
				{
				case EditOperation.Identity:
					text2 = ((editDistanceItem3.Source < sourceTokens.Count) ? sourceTokens[editDistanceItem3.Source].Text : "_");
					text3 = ((editDistanceItem3.Target < targetTokens.Count) ? targetTokens[editDistanceItem3.Target].Text : "_");
					break;
				case EditOperation.Delete:
					text2 = sourceTokens[editDistanceItem3.Source].Text;
					text3 = "_";
					break;
				case EditOperation.Insert:
					text3 = targetTokens[editDistanceItem3.Target].Text;
					text2 = "_";
					break;
				case EditOperation.Change:
					text2 = sourceTokens[editDistanceItem3.Source].Text;
					text3 = targetTokens[editDistanceItem3.Target].Text;
					break;
				case EditOperation.Move:
					text2 = sourceTokens[editDistanceItem3.Source].Text;
					text3 = targetTokens[editDistanceItem3.Target].Text;
					break;
				}
				if (text2 == " ")
				{
					text2 = "space";
				}
				if (text3 == " ")
				{
					text3 = "space";
				}
				string str4 = $"{editDistanceItem3.Source}{text} {text2}{text} {i}{editDistanceItem3.ToString()}{text} {text3}{text}{editDistanceItem3.Target}";
				str = str + str4 + Environment.NewLine;
				editDistanceItem2 = editDistanceItem3;
			}
			File.AppendAllText(dirLog + "\\" + edFileLogName, str + Environment.NewLine);
		}

		private static bool IsWhiteSpaceIdentity(EdApplierContext context, EditDistanceItem ed)
		{
			if (ed.Operation == EditOperation.Identity)
			{
				return context.OriginalLinguaSegment.Tokens[ed.Source].Text == " ";
			}
			return false;
		}

		private static bool IsTextChange(EdApplierContext context, EditDistanceItem ed)
		{
			if (ed.Operation == EditOperation.Change)
			{
				if (context.OriginalLinguaSegment.Tokens[ed.Source] is TagToken)
				{
					return !(context.UpdatedLinguaSegment.Tokens[ed.Target] is TagToken);
				}
				return true;
			}
			return false;
		}

		private static bool IsTagOrPhChange(EdApplierContext context, EditDistanceItem ed)
		{
			if (ed.Operation != EditOperation.Change)
			{
				return false;
			}
			if (ed.Source >= context.OriginalLinguaSegment.Tokens.Count || ed.Target >= context.UpdatedLinguaSegment.Tokens.Count)
			{
				return false;
			}
			TagToken tagToken = context.OriginalLinguaSegment.Tokens[ed.Source] as TagToken;
			TagToken tagToken2 = context.UpdatedLinguaSegment.Tokens[ed.Target] as TagToken;
			if (tagToken == null || tagToken2 == null)
			{
				return false;
			}
			return true;
		}

		private static bool IsPhChange(EdApplierContext context, EditDistanceItem ed)
		{
			if (!IsTagOrPhChange(context, ed))
			{
				return false;
			}
			TagToken tagToken = context.OriginalLinguaSegment.Tokens[ed.Source] as TagToken;
			TagToken tagToken2 = context.UpdatedLinguaSegment.Tokens[ed.Target] as TagToken;
			return tagToken.Tag.Type == TagType.Standalone && tagToken2.Tag.Type == TagType.Standalone;
		}

		private static bool IsTagChange(EdApplierContext context, EditDistanceItem ed)
		{
			if (!IsTagOrPhChange(context, ed))
			{
				return false;
			}
			TagToken tagToken = context.OriginalLinguaSegment.Tokens[ed.Source] as TagToken;
			TagToken tagToken2 = context.UpdatedLinguaSegment.Tokens[ed.Target] as TagToken;
			bool flag = (tagToken.Tag.Type == TagType.Start || tagToken.Tag.Type == TagType.End) && (tagToken2.Tag.Type == TagType.Start || tagToken2.Tag.Type == TagType.End);
			bool flag2 = tagToken.Tag.TagID.StartsWith("c") && tagToken2.Tag.TagID.StartsWith("c");
			if (flag)
			{
				return !flag2;
			}
			return false;
		}

		private static bool IsCommentChange(EdApplierContext context, EditDistanceItem ed)
		{
			if (!IsTagOrPhChange(context, ed))
			{
				return false;
			}
			TagToken tagToken = context.OriginalLinguaSegment.Tokens[ed.Source] as TagToken;
			TagToken tagToken2 = context.UpdatedLinguaSegment.Tokens[ed.Target] as TagToken;
			bool flag = (tagToken.Tag.Type == TagType.Start || tagToken.Tag.Type == TagType.End) && (tagToken2.Tag.Type == TagType.Start || tagToken2.Tag.Type == TagType.End);
			bool flag2 = tagToken.Tag.TagID.StartsWith("c") && tagToken2.Tag.TagID.StartsWith("c");
			return flag && flag2;
		}

		private static void ReplaceSingleMoveForwardOperation(IList<EditDistanceItem> list, int i, int insertOffset)
		{
			int target = list[i].Target;
			EditDistanceItem value = list.ElementAt(i);
			value.Operation = EditOperation.Delete;
			value.Target = list[i].MoveSourceTarget;
			list[i] = value;
			int j;
			for (j = target + insertOffset; list[j].Target < target; j++)
			{
			}
			EditDistanceItem editDistanceItem = default(EditDistanceItem);
			editDistanceItem.Target = target;
			editDistanceItem.Source = list.ElementAt(j).Source;
			editDistanceItem.Operation = EditOperation.Insert;
			EditDistanceItem item = editDistanceItem;
			list.Insert(j, item);
		}

		private static void ReplaceSingleMoveBackwardOperation(IList<EditDistanceItem> list, int i, int insertOffset)
		{
			int moveSourceTarget = list[i].MoveSourceTarget;
			int source = list[i].Source;
			EditDistanceItem value = list.ElementAt(i);
			value.Operation = EditOperation.Insert;
			value.Source = list[i].MoveTargetSource;
			list[i] = value;
			EditDistanceItem editDistanceItem = default(EditDistanceItem);
			editDistanceItem.Target = moveSourceTarget;
			editDistanceItem.Source = source;
			editDistanceItem.Operation = EditOperation.Delete;
			EditDistanceItem item = editDistanceItem;
			int j;
			for (j = moveSourceTarget + insertOffset; j < list.Count && list[j].Source < source; j++)
			{
			}
			list.Insert(j, item);
		}

		private static void ReplaceMoveOperations(EdApplierContext context)
		{
			bool flag = true;
			int num = 0;
			while (flag)
			{
				flag = false;
				for (int i = 0; i < context.AllEditDistanceItems.Count; i++)
				{
					if (context.AllEditDistanceItems[i].Operation == EditOperation.Move)
					{
						if (context.AllEditDistanceItems[i].Source < context.AllEditDistanceItems[i].Target)
						{
							ReplaceSingleMoveForwardOperation(context.AllEditDistanceItems, i, num++);
						}
						else
						{
							ReplaceSingleMoveBackwardOperation(context.AllEditDistanceItems, i, num++);
						}
						flag = true;
						break;
					}
				}
			}
		}

		private static void OrderEditDistance(EdApplierContext context)
		{
			for (int i = 0; i < context.AllEditDistanceItems.Count - 1; i++)
			{
				EditDistanceItem editDistanceItem = context.AllEditDistanceItems[i];
				EditDistanceItem editDistanceItem2 = context.AllEditDistanceItems[i + 1];
				if (editDistanceItem.Source > editDistanceItem2.Source || editDistanceItem.Target > editDistanceItem2.Target)
				{
					SwapEditDistanceItem(context, i);
				}
			}
		}

		private static void RevertEditDistance(EdApplierContext context)
		{
			for (int i = 0; i < context.AllEditDistanceItems.Count; i++)
			{
				EditDistanceItem editDistanceItem = context.AllEditDistanceItems[i];
				switch (editDistanceItem.Operation)
				{
				case EditOperation.Delete:
					ReplaceAndRevertEditDistanceItem(context, i, EditOperation.Insert);
					break;
				case EditOperation.Insert:
					ReplaceAndRevertEditDistanceItem(context, i, EditOperation.Delete);
					break;
				case EditOperation.Identity:
				case EditOperation.Change:
					ReplaceAndRevertEditDistanceItem(context, i, editDistanceItem.Operation);
					break;
				}
			}
		}

		private static void ReplaceAndRevertEditDistanceItem(EdApplierContext context, int i, EditOperation operation)
		{
			EditDistanceItem editDistanceItem = default(EditDistanceItem);
			editDistanceItem.Target = context.AllEditDistanceItems.ElementAt(i).Source;
			editDistanceItem.Source = context.AllEditDistanceItems.ElementAt(i).Target;
			editDistanceItem.Operation = operation;
			EditDistanceItem item = editDistanceItem;
			context.AllEditDistanceItems.RemoveAt(i);
			context.AllEditDistanceItems.Insert(i, item);
		}

		private static void MergeConsecutiveInsertDeleteTagsAsChanges(List<EditDistanceItem> allEditDistanceItems, List<Token> sourceTokens, List<Token> targetTokens)
		{
			for (int i = 0; i < allEditDistanceItems.Count - 1; i++)
			{
				EditDistanceItem editDistanceItem = allEditDistanceItems[i];
				EditDistanceItem editDistanceItem2 = allEditDistanceItems[i + 1];
				bool flag = editDistanceItem2.Source < sourceTokens.Count && sourceTokens[editDistanceItem2.Source] is TagToken && editDistanceItem2.Target < targetTokens.Count && targetTokens[editDistanceItem.Target] is TagToken;
				if ((editDistanceItem.Operation == EditOperation.Insert && editDistanceItem2.Operation == EditOperation.Delete) & flag)
				{
					MergeEditDistanceItem(allEditDistanceItems, i, editDistanceItem2.Source, editDistanceItem.Target);
				}
				else if ((editDistanceItem.Operation == EditOperation.Delete && editDistanceItem2.Operation == EditOperation.Insert) & flag)
				{
					MergeEditDistanceItem(allEditDistanceItems, i, editDistanceItem.Source, editDistanceItem2.Target);
				}
			}
		}

		private static void SwapEditDistanceItem(EdApplierContext context, int i)
		{
			EditDistanceItem editDistanceItem = default(EditDistanceItem);
			editDistanceItem.Source = context.AllEditDistanceItems.ElementAt(i + 1).Source;
			editDistanceItem.Target = context.AllEditDistanceItems.ElementAt(i + 1).Target;
			editDistanceItem.Operation = context.AllEditDistanceItems.ElementAt(i + 1).Operation;
			EditDistanceItem item = editDistanceItem;
			context.AllEditDistanceItems.RemoveAt(i + 1);
			context.AllEditDistanceItems.Insert(i, item);
		}

		private static void MergeEditDistanceItem(List<EditDistanceItem> allEditDistanceItems, int i, int source, int target)
		{
			EditDistanceItem editDistanceItem = default(EditDistanceItem);
			editDistanceItem.Operation = EditOperation.Change;
			editDistanceItem.Source = source;
			editDistanceItem.Target = target;
			EditDistanceItem item = editDistanceItem;
			allEditDistanceItems.Insert(i, item);
			allEditDistanceItems.RemoveAt(i + 1);
			allEditDistanceItems.RemoveAt(i + 1);
		}

		private static void ReplaceEditDistanceItem(EdApplierContext context, int i, EditOperation operation)
		{
			EditDistanceItem editDistanceItem = default(EditDistanceItem);
			editDistanceItem.Source = context.AllEditDistanceItems.ElementAt(i).Source;
			editDistanceItem.Target = context.AllEditDistanceItems.ElementAt(i).Target;
			editDistanceItem.Operation = operation;
			EditDistanceItem item = editDistanceItem;
			context.AllEditDistanceItems.RemoveAt(i);
			context.AllEditDistanceItems.Insert(i, item);
		}
	}
}
