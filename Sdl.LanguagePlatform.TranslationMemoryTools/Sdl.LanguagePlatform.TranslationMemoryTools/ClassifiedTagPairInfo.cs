using Sdl.LanguagePlatform.Core.EditDistance;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemoryTools
{
	public class ClassifiedTagPairInfo
	{
		public List<EditDistanceItem> Subitems
		{
			get;
			set;
		}

		public EditDistanceItem? Start
		{
			get;
			set;
		}

		public int StartTokenPosition
		{
			get;
			set;
		}

		public EditDistanceItem? End
		{
			get;
			set;
		}

		public int EndTokenPosition
		{
			get;
			set;
		}

		public int SubitemsCount
		{
			get
			{
				List<EditDistanceItem> subitems = Subitems;
				if (subitems == null)
				{
					return 0;
				}
				return subitems.Count;
			}
		}
	}
}
