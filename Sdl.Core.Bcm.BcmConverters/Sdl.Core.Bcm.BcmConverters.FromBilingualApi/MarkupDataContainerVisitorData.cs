using Sdl.Core.Bcm.BcmModel;
using Sdl.Core.Bcm.BcmModel.Annotations;
using System.Collections.Generic;

namespace Sdl.Core.Bcm.BcmConverters.FromBilingualApi
{
	internal class MarkupDataContainerVisitorData
	{
		public File File
		{
			get;
			set;
		}

		public BcmExtractionSettings BcmExtractionSettings
		{
			get;
			set;
		}

		public bool IsInSegment
		{
			get;
			set;
		}

		public Queue<CommentContainer> BufferedComments
		{
			get;
			set;
		}

		public MarkupDataContainerVisitorData(File file, BcmExtractionSettings bcmExtractionSettings)
		{
			IsInSegment = false;
			BufferedComments = new Queue<CommentContainer>();
			File = file;
			BcmExtractionSettings = bcmExtractionSettings;
		}

		internal MarkupDataContainerVisitorData(File file)
			: this(file, new BcmExtractionSettings())
		{
		}
	}
}
