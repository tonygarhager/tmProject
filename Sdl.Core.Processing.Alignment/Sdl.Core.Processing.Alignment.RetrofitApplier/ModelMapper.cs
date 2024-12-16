using Sdl.FileTypeSupport.Framework.BilingualApi;
using System.Collections.Generic;

namespace Sdl.Core.Processing.Alignment.RetrofitApplier
{
	public class ModelMapper
	{
		public Dictionary<int, ITagPair> UpdateTag
		{
			get;
			set;
		}

		public Dictionary<int, IPlaceholderTag> UpdatePh
		{
			get;
			set;
		}

		public Dictionary<int, ICommentMarker> UpdateComment
		{
			get;
			set;
		}

		public Dictionary<int, ITagPair> OriginalTag
		{
			get;
			set;
		}

		public Dictionary<int, IPlaceholderTag> OriginalPh
		{
			get;
			set;
		}

		public Dictionary<int, ICommentMarker> OriginalComment
		{
			get;
			set;
		}
	}
}
