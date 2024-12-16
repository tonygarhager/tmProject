using System;
using System.Collections.Generic;
using System.Threading;

namespace Sdl.Core.FineGrainedAlignment
{
	public interface IFineGrainedAligner
	{
		bool BulkMode
		{
			get;
			set;
		}

		bool Align(IEnumerable<IAlignableContentPair> pairs);

		bool Align(IEnumerable<IAlignableContentPair> pairs, CancellationToken token, IProgress<int> progress);

		AlignResult[] AlignEx(IEnumerable<IAlignableContentPair> pairs);

		AlignResult[] AlignEx(IEnumerable<IAlignableContentPair> pairs, CancellationToken token, IProgress<int> progress);

		bool[] CanAlign(IEnumerable<IAlignableContentPair> pairs);

		void SetErrorLogger(Action<Exception, string> logger);
	}
}
