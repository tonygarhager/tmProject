using Sdl.Core.Bcm.BcmModel.Collections;
using Sdl.Core.Bcm.BcmModel.Skeleton;
using Sdl.FileTypeSupport.Framework.Formatting;
using System.Collections.Generic;

namespace Sdl.Core.Bcm.BcmConverters.FromBilingualApi.Helpers
{
	internal static class FormattingGroupHelper
	{
		internal static int AddFormatting(FileSkeleton fileSkeleton, IFormattingGroup formatting)
		{
			FormattingGroup formattingGroup = CreateFormatting(formatting);
			return AddFormatting(fileSkeleton, formattingGroup);
		}

		internal static int AddFormatting(FileSkeleton fileSkeleton, FormattingGroup formattingGroup)
		{
			return fileSkeleton.FormattingGroups.GetOrAdd(formattingGroup)?.Id ?? formattingGroup.Id;
		}

		internal static FormattingGroup CreateFormatting(IFormattingGroup formatting)
		{
			FormattingGroup formattingGroup = new FormattingGroup
			{
				Items = new DictionaryEx<string, string>()
			};
			foreach (KeyValuePair<string, IFormattingItem> item in formatting)
			{
				formattingGroup.Items.Add(item.Value.FormattingName, item.Value.StringValue);
			}
			return formattingGroup;
		}
	}
}
