using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.LanguagePlatform.TranslationMemoryTools
{
	public class MarkupDataPosition
	{
		public IAbstractMarkupData MarkupData
		{
			get;
		}

		public int CharacterOffset
		{
			get;
		}

		public MarkupDataPosition(IAbstractMarkupData markupData, int characterOffset)
		{
			MarkupData = markupData;
			CharacterOffset = characterOffset;
		}
	}
}
