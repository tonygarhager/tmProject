using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.Core.Processing.Alignment.Core.Algorithms
{
	internal class PositionAlignmentAlgorithm : IAlignmentAlgorithm
	{
		public bool CancelProcessing
		{
			get;
			set;
		}

		public CultureInfo SourceCulture => CultureInfo.InvariantCulture;

		public CultureInfo TargetCulture => CultureInfo.InvariantCulture;

		public event EventHandler<ProgressEventArgs> OnProgress;

		public IList<AlignmentData> Align(IList<AlignmentElement> leftInputElements, IList<AlignmentElement> rightInputElements)
		{
			List<AlignmentData> list = new List<AlignmentData>();
			int num = Math.Min(leftInputElements.Count, rightInputElements.Count);
			int num2 = Math.Max(leftInputElements.Count, rightInputElements.Count);
			for (int i = 0; i < num; i++)
			{
				list.Add(new AlignmentData(new List<DocumentSegmentId>
				{
					leftInputElements[i].Id
				}, new List<DocumentSegmentId>
				{
					rightInputElements[i].Id
				}, AlignmentCost.MinValue));
				if (this.OnProgress != null)
				{
					this.OnProgress(this, new ProgressEventArgs((byte)(i / num2)));
				}
			}
			if (num < leftInputElements.Count)
			{
				for (int j = num; j < leftInputElements.Count; j++)
				{
					List<DocumentSegmentId> list2 = new List<DocumentSegmentId>();
					list2.Add(leftInputElements[j].Id);
					new AlignmentData(list2, new List<DocumentSegmentId>(), AlignmentCost.MinValue);
					if (this.OnProgress != null)
					{
						this.OnProgress(this, new ProgressEventArgs((byte)(j / num2)));
					}
				}
			}
			else if (num < rightInputElements.Count)
			{
				for (int k = num; k < rightInputElements.Count; k++)
				{
					new AlignmentData(new List<DocumentSegmentId>(), new List<DocumentSegmentId>
					{
						rightInputElements[k].Id
					}, AlignmentCost.MinValue);
					if (this.OnProgress != null)
					{
						this.OnProgress(this, new ProgressEventArgs((byte)(k / num2)));
					}
				}
			}
			return list;
		}

		public IList<AlignmentData> GetAnchors(IList<AlignmentElement> leftInputElements, IList<AlignmentElement> rightInputElements)
		{
			return new List<AlignmentData>();
		}
	}
}
