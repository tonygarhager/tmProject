using Sdl.LanguagePlatform.Lingua.Alignment;
using System;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.Lingua.TermRecognition
{
	internal class SubstringAlignmentDisambiguator : IExtensionDisambiguator
	{
		public AlignedSubstring PickExtension(List<AlignedSubstring> path, List<AlignedSubstring> candidates)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (candidates == null)
			{
				throw new ArgumentNullException("candidates");
			}
			if (candidates.Count == 0)
			{
				return null;
			}
			if (candidates.Count == 1 || path.Count == 0)
			{
				return candidates[0];
			}
			AlignedSubstring alignedSubstring = null;
			int num = 0;
			foreach (AlignedSubstring candidate in candidates)
			{
				int num2 = ComputeCosts(path, candidate);
				if (alignedSubstring == null || num2 < num)
				{
					num = num2;
					alignedSubstring = candidate;
				}
			}
			return alignedSubstring;
		}

		private int ComputeCosts(IEnumerable<AlignedSubstring> path, AlignedSubstring candidate)
		{
			if (candidate == null)
			{
				throw new ArgumentNullException("candidate");
			}
			int num = -1;
			foreach (AlignedSubstring item in path)
			{
				int val = (item.Source.Start >= candidate.Source.Start) ? (item.Source.Start - candidate.Source.Start - candidate.Source.Length) : (candidate.Source.Start - item.Source.Start - item.Source.Length);
				int val2 = (item.Target.Start >= candidate.Target.Start) ? (item.Target.Start - candidate.Target.Start - candidate.Target.Length) : (candidate.Target.Start - item.Target.Start - item.Target.Length);
				int num2 = Math.Max(val, val2);
				if (num < 0 || num2 < num)
				{
					num = num2;
				}
			}
			return num;
		}
	}
}
