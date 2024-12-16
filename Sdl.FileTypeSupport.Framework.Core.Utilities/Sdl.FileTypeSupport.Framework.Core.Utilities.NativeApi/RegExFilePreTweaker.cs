using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Collections.Generic;
using System.Text;

namespace Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi
{
	public class RegExFilePreTweaker : AbstractFilePreTweaker
	{
		private RegExFileHelper helper = new RegExFileHelper();

		public List<Pair<string, string>> ReplaceInfo
		{
			get
			{
				return helper.ReplaceInfo;
			}
			set
			{
				helper.ReplaceInfo = value;
			}
		}

		public RegExFilePreTweaker()
		{
		}

		public RegExFilePreTweaker(string regExMatch, string regExReplace)
		{
			helper.StoreReplaceInfo(regExMatch, regExReplace);
		}

		public RegExFilePreTweaker(List<string> regExMatchList, List<string> regExReplaceList)
		{
			helper.StoreReplaceInfo(regExMatchList, regExReplaceList);
		}

		protected override void Tweak(IPersistentFileConversionProperties properties)
		{
			Encoding outputEncoding = null;
			if (properties.FileSnifferInfo != null && properties.FileSnifferInfo.DetectedEncoding != null)
			{
				Codepage first = properties.FileSnifferInfo.DetectedEncoding.First;
				if (first.IsSupported && first.IsValid)
				{
					outputEncoding = first.Encoding;
				}
			}
			helper.ApplyRegExsToFile(properties.OriginalFilePath, properties.InputFilePath, outputEncoding);
			base.Tweak(properties);
		}

		protected override bool WillFileBeTweaked(string filePath)
		{
			return helper.RegExsWillApplyToFile(filePath);
		}
	}
}
