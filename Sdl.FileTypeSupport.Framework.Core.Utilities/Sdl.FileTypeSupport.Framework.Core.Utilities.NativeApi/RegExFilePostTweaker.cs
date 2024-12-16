using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi
{
	public class RegExFilePostTweaker : AbstractFilePostTweaker
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

		public RegExFilePostTweaker()
		{
		}

		public RegExFilePostTweaker(string regExMatch, string regExReplace)
		{
			helper.StoreReplaceInfo(regExMatch, regExReplace);
		}

		public RegExFilePostTweaker(List<string> regExMatchList, List<string> regExReplaceList)
		{
			helper.StoreReplaceInfo(regExMatchList, regExReplaceList);
		}

		protected override void Tweak(INativeOutputFileProperties outputFileProperties)
		{
			helper.ApplyRegExsToFile(outputFileProperties.OutputFilePath, outputFileProperties.OutputFilePath, outputFileProperties.Encoding.Encoding);
			base.Tweak(outputFileProperties);
		}
	}
}
