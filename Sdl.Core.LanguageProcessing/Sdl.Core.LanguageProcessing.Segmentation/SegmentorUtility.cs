using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Core.LanguageProcessing.Segmentation
{
	public class SegmentorUtility
	{
		public static bool IsTextWhiteSpace(IAbstractMarkupData item)
		{
			IText text = item as IText;
			if (text == null)
			{
				return false;
			}
			int leadingWhitespacesCount = GetLeadingWhitespacesCount(text.Properties.Text, 0);
			return leadingWhitespacesCount == text.Properties.Text.Length;
		}

		public static int GetLeadingWhitespacesCount(string text, int startIndex)
		{
			int i;
			for (i = startIndex; i < text.Length && CharacterProperties.IsWhitespace(text[i]); i++)
			{
			}
			return i - startIndex;
		}

		public virtual int GetTrailingWhitespacesCount(string text)
		{
			int num = text.Length;
			while (num > 0 && CharacterProperties.IsWhitespace(text[num - 1]))
			{
				num--;
			}
			return text.Length - num;
		}
	}
}
