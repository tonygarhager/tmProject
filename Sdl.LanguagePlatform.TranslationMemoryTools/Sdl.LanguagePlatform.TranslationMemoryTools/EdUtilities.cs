using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.EditDistance;
using Sdl.LanguagePlatform.Core.Tokenization;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.LanguagePlatform.TranslationMemoryTools
{
	internal class EdUtilities
	{
		internal static void MarkWhiteSpaceBetweenTextChangesAsChange(Segment originalSegment, Segment updatedSegment, List<EditDistanceItem> editDistanceItems)
		{
			int num = 0;
			int i = 0;
			for (int j = 0; j < editDistanceItems.Count; j++)
			{
				EditDistanceItem ed = editDistanceItems[j];
				switch (num)
				{
				case 0:
					if (IsTextChange(originalSegment, updatedSegment, ed))
					{
						num++;
					}
					break;
				case 1:
					if (IsWhiteSpaceIdentity(originalSegment, ed))
					{
						i = j;
						num++;
					}
					else
					{
						num = 0;
					}
					break;
				case 2:
					if (IsTextChange(originalSegment, updatedSegment, ed))
					{
						ReplaceEditDistanceItem(editDistanceItems, i, EditOperation.Change);
						num = 1;
					}
					else
					{
						num = 0;
					}
					break;
				}
			}
		}

		private static bool IsTextChange(Segment originalSegment, Segment updatedSegment, EditDistanceItem ed)
		{
			if (ed.Operation == EditOperation.Change)
			{
				if (originalSegment.Tokens[ed.Source] is TagToken)
				{
					return !(updatedSegment.Tokens[ed.Target] is TagToken);
				}
				return true;
			}
			return false;
		}

		private static bool IsWhiteSpaceIdentity(Segment originalSegment, EditDistanceItem ed)
		{
			if (ed.Operation == EditOperation.Identity && originalSegment.Tokens[ed.Source].Text.Length == 1)
			{
				return char.IsWhiteSpace(originalSegment.Tokens[ed.Source].Text[0]);
			}
			return false;
		}

		private static void ReplaceEditDistanceItem(List<EditDistanceItem> editDistanceItems, int i, EditOperation operation)
		{
			EditDistanceItem editDistanceItem = default(EditDistanceItem);
			editDistanceItem.Source = editDistanceItems.ElementAt(i).Source;
			editDistanceItem.Target = editDistanceItems.ElementAt(i).Target;
			editDistanceItem.Operation = operation;
			EditDistanceItem item = editDistanceItem;
			editDistanceItems.RemoveAt(i);
			editDistanceItems.Insert(i, item);
		}
	}
}
