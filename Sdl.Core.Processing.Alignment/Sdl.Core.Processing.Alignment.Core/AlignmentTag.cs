using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Core.Processing.Alignment.Core
{
	internal class AlignmentTag : Tag
	{
		public IAbstractTag BilingualTag
		{
			get;
			private set;
		}

		public AlignmentTag(TagType type, string tagId, int anchor, IAbstractTag bilingualTag)
			: base(type, tagId, anchor)
		{
			BilingualTag = bilingualTag;
		}
	}
}
