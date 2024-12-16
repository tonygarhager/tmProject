using Sdl.LanguagePlatform.Core.EditDistance;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemoryTools
{
	public class EditDistanceChangeSequence
	{
		public List<EditDistanceItem> Items
		{
			get;
		} = new List<EditDistanceItem>();


		public void Add(EditDistanceItem item)
		{
			Items.Add(item);
		}

		public static bool AreItemsCompatible(EditDistanceItem first, EditDistanceItem second)
		{
			if (first.Operation != second.Operation)
			{
				return false;
			}
			return first.Resolution == second.Resolution;
		}
	}
}
