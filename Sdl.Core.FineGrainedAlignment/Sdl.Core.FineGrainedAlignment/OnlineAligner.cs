using System;
using System.Collections.Generic;
using System.Threading;

namespace Sdl.Core.FineGrainedAlignment
{
	internal class OnlineAligner : IFineGrainedAligner
	{
		public bool BulkMode
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public OnlineAligner(string url, string username, string pwd)
		{
		}

		public bool Align(IEnumerable<IAlignableContentPair> pairs)
		{
			throw new NotImplementedException();
		}

		public bool Align(IEnumerable<IAlignableContentPair> pairs, CancellationToken token, IProgress<int> progress)
		{
			throw new NotImplementedException();
		}

		public AlignResult[] AlignEx(IEnumerable<IAlignableContentPair> pairs)
		{
			throw new NotImplementedException();
		}

		public AlignResult[] AlignEx(IEnumerable<IAlignableContentPair> pairs, CancellationToken token, IProgress<int> progress)
		{
			throw new NotImplementedException();
		}

		public bool[] CanAlign(IEnumerable<IAlignableContentPair> pairs)
		{
			throw new NotImplementedException();
		}

		public void SetErrorLogger(Action<Exception, string> logger)
		{
			throw new NotImplementedException();
		}
	}
}
