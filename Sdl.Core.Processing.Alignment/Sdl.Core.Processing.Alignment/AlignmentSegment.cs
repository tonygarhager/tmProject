using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using System;

namespace Sdl.Core.Processing.Alignment
{
	[Serializable]
	internal class AlignmentSegment : Segment
	{
		public string ContextPath
		{
			get;
			private set;
		}

		public AlignmentSegment(ISegment segment, string contextPath)
			: base((Segment)segment)
		{
			ContextPath = contextPath;
		}
	}
}
