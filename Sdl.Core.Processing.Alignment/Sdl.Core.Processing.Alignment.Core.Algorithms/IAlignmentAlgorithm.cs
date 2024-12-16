using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.Core.Processing.Alignment.Core.Algorithms
{
	internal interface IAlignmentAlgorithm
	{
		bool CancelProcessing
		{
			get;
			set;
		}

		CultureInfo SourceCulture
		{
			get;
		}

		CultureInfo TargetCulture
		{
			get;
		}

		event EventHandler<ProgressEventArgs> OnProgress;

		IList<AlignmentData> Align(IList<AlignmentElement> leftInputElements, IList<AlignmentElement> rightInputElements);
	}
}
