using System;
using System.Collections;
using System.Collections.Generic;

namespace Sdl.Core.Processing.Alignment.Core
{
	internal class AlignedPairsEnumerable<T> : IEnumerable<AlignedPair<T>>, IEnumerable
	{
		internal readonly IList<T> LeftObjects;

		internal readonly IList<T> RightObjects;

		internal readonly IList<AlignmentData> Alignments;

		internal readonly Func<IEnumerable<T>, T> Merger;

		private readonly Func<IEnumerable<DocumentSegmentId>, IEnumerable<T>, IEnumerable<T>> _locator;

		public AlignedPairsEnumerable(IList<T> leftObjects, IList<T> rightObjects, IList<AlignmentData> alignments, Func<IEnumerable<DocumentSegmentId>, IEnumerable<T>, IEnumerable<T>> locator, Func<IEnumerable<T>, T> merger)
		{
			if (leftObjects == null)
			{
				throw new ArgumentNullException("leftObjects");
			}
			if (rightObjects == null)
			{
				throw new ArgumentNullException("rightObjects");
			}
			if (alignments == null)
			{
				throw new ArgumentNullException("alignments");
			}
			if (locator == null)
			{
				throw new ArgumentNullException("locator");
			}
			if (merger == null)
			{
				throw new ArgumentNullException("merger");
			}
			LeftObjects = leftObjects;
			RightObjects = rightObjects;
			Alignments = alignments;
			Merger = merger;
			_locator = locator;
		}

		public IEnumerator<AlignedPair<T>> GetEnumerator()
		{
			foreach (AlignmentData alignment in Alignments)
			{
				T leftObject = Merger(_locator(alignment.LeftIds, LeftObjects));
				T rightObject = Merger(_locator(alignment.RightIds, RightObjects));
				yield return new AlignedPair<T>(leftObject, rightObject, alignment.AlignmentType);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
