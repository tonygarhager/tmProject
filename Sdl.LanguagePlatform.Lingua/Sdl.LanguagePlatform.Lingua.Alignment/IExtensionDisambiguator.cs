using System.Collections.Generic;

namespace Sdl.LanguagePlatform.Lingua.Alignment
{
	public interface IExtensionDisambiguator
	{
		AlignedSubstring PickExtension(List<AlignedSubstring> path, List<AlignedSubstring> candidates);
	}
}
