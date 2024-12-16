using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using System;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	internal class AnnotatedTranslationUnit
	{
		public TranslationUnit TranslationUnit
		{
			get;
		}

		public AnnotatedSegment Source
		{
			get;
		}

		public AnnotatedSegment Target
		{
			get;
		}

		public AnnotatedTranslationUnit(AnnotatedTranslationMemory tm, TranslationUnit tu, bool keepTokens, bool keepPeripheralWhitespace)
		{
			if (tu == null)
			{
				throw new ArgumentNullException("tu");
			}
			tu.CheckAndComputeTagAssociations();
			if (!keepTokens)
			{
				Clean(tu.SourceSegment);
				Clean(tu.TargetSegment);
			}
			TranslationUnit = tu;
			Source = new AnnotatedSegment(tm, tu.SourceSegment, isTargetSegment: false, keepTokens, keepPeripheralWhitespace);
			Target = ((tu.TargetSegment == null) ? null : new AnnotatedSegment(tm, tu.TargetSegment, isTargetSegment: true, keepTokens, keepPeripheralWhitespace));
		}

		private static void Clean(Segment segment)
		{
			if (segment != null)
			{
				segment.Tokens = null;
				if (segment.Elements != null)
				{
					SegmentEditor.CleanSegment(segment);
				}
			}
		}

		public override string ToString()
		{
			string arg = (Source != null) ? Source.ToString() : "null";
			int valueOrDefault = (TranslationUnit?.ResourceId?.Id).GetValueOrDefault();
			return $"{valueOrDefault}-{arg}";
		}
	}
}
