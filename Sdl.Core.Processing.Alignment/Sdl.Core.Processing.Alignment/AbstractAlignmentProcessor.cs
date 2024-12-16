using Sdl.Core.Processing.Alignment.Core;
using Sdl.Core.Processing.Alignment.Core.Anchoring;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;

namespace Sdl.Core.Processing.Alignment
{
	internal class AbstractAlignmentProcessor : AbstractBilingualContentProcessor
	{
		protected GapAligner Aligner;

		protected readonly ConfirmedAlignmentsAnchorStrategy ConfirmedAlignmentsAnchorStrategy = new ConfirmedAlignmentsAnchorStrategy();

		public bool OverrideParagraphUnitIdForConfirmedAlignments
		{
			set
			{
				Aligner.OverrideParagraphUnitIdForConfirmedAlignments = value;
			}
		}

		public bool CancelProcessing => Aligner.CancelProcessing;

		public List<AlignmentData> Alignments => Aligner.Alignments;

		public IList<AlignmentData> ConfirmedAlignments
		{
			get
			{
				return ConfirmedAlignmentsAnchorStrategy.ConfirmedAlignments;
			}
			set
			{
				ConfirmedAlignmentsAnchorStrategy.ConfirmedAlignments = value;
			}
		}

		public event EventHandler<ProgressEventArgs> OnCurrentParagraphUnitProgress
		{
			add
			{
				Aligner.OnCurrentParagraphUnitProgress += value;
			}
			remove
			{
				Aligner.OnCurrentParagraphUnitProgress -= value;
			}
		}

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			Aligner.ProcessParagraphUnit(paragraphUnit);
			base.ProcessParagraphUnit(paragraphUnit);
		}
	}
}
