using Sdl.Core.Bcm.BcmModel.Skeleton;
using Sdl.FileTypeSupport.Framework.Formatting;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.Bcm.BcmConverters.ToBilingualApi.MarkupDataConverters
{
	internal class FormattingGroupConverter
	{
		private readonly IFormattingItemFactory _formattingItemFactory;

		private readonly FileSkeleton _fileSkeleton;

		public FormattingGroupConverter(IFormattingItemFactory formattingItemFactory, FileSkeleton fileSkeleton)
		{
			_formattingItemFactory = formattingItemFactory;
			_fileSkeleton = fileSkeleton;
		}

		public IFormattingGroup Convert(int formattingGroupId)
		{
			if (formattingGroupId == 0)
			{
				return null;
			}
			FormattingGroup formattingGroup = GetFormattingGroup(formattingGroupId);
			IFormattingGroup formattingGroup2 = _formattingItemFactory.CreateFormatting();
			foreach (IFormattingItem item in formattingGroup.Items.Select((KeyValuePair<string, string> pair) => _formattingItemFactory.CreateFormattingItem(pair.Key, pair.Value)))
			{
				formattingGroup2.Add(item);
			}
			return formattingGroup2;
		}

		private FormattingGroup GetFormattingGroup(int id)
		{
			return _fileSkeleton.FormattingGroups.GetById(id);
		}
	}
}
