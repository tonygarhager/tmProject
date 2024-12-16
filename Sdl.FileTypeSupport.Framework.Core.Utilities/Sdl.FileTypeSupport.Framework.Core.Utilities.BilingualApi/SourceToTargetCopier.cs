using Sdl.FileTypeSupport.Framework.BilingualApi;
using System;

namespace Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi
{
	public class SourceToTargetCopier : AbstractBilingualContentProcessor
	{
		private ExistingContentHandling _HandleExistingTargetContent;

		private bool _copySegmentsAsEmpty;

		public ExistingContentHandling HandleExistingTargetContent
		{
			get
			{
				return _HandleExistingTargetContent;
			}
			set
			{
				_HandleExistingTargetContent = value;
			}
		}

		public bool CopySegmentsAsEmpty
		{
			get
			{
				return _copySegmentsAsEmpty;
			}
			set
			{
				_copySegmentsAsEmpty = value;
			}
		}

		public SourceToTargetCopier(ExistingContentHandling targetHandling)
		{
			_HandleExistingTargetContent = targetHandling;
		}

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			if (paragraphUnit.IsStructure)
			{
				base.ProcessParagraphUnit(paragraphUnit);
				return;
			}
			if (paragraphUnit == null)
			{
				base.ProcessParagraphUnit(paragraphUnit);
				return;
			}
			if (paragraphUnit.Target.Count != 0)
			{
				switch (_HandleExistingTargetContent)
				{
				case ExistingContentHandling.Replace:
					break;
				case ExistingContentHandling.Preserve:
					base.ProcessParagraphUnit(paragraphUnit);
					return;
				case ExistingContentHandling.ThrowException:
					throw new FileTypeSupportException("SourceToTargetCopier unexpectedly encountered existing target content while processing a paragraph unit.");
				default:
					throw new Exception("Unrecognized target content handling setting.");
				}
				paragraphUnit.Target.Clear();
			}
			CopySourceToTarget(paragraphUnit);
			base.ProcessParagraphUnit(paragraphUnit);
		}

		protected void CopySourceToTarget(IParagraphUnit paragraphUnit)
		{
			foreach (IAbstractMarkupData item in paragraphUnit.Source)
			{
				IAbstractMarkupData abstractMarkupData = (IAbstractMarkupData)item.Clone();
				paragraphUnit.Target.Add(abstractMarkupData);
				ISegment segment = abstractMarkupData as ISegment;
				if (segment != null)
				{
					ISegment segment2 = item as ISegment;
					segment.Properties = segment2.Properties;
					if (segment.Properties.TranslationOrigin == null)
					{
						segment.Properties.TranslationOrigin = ItemFactory.CreateTranslationOrigin();
					}
					segment.Properties.TranslationOrigin.OriginType = "source";
					if (CopySegmentsAsEmpty)
					{
						segment.Clear();
						segment.Properties.TranslationOrigin.OriginType = "not-translated";
					}
				}
			}
		}
	}
}
