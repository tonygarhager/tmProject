using Sdl.FileTypeSupport.Framework.BilingualApi;
using System.Collections.Generic;
using System.Text;

namespace Sdl.Core.LanguageProcessing.Segmentation
{
	internal class WordStopParagraph
	{
		private class WordStopVisitor : IMarkupDataVisitor
		{
			private readonly List<IAbstractMarkupData> _flatMarkupList = new List<IAbstractMarkupData>();

			public void VisitParagraph(IAbstractMarkupDataContainer paragraph)
			{
				VisitChildren(paragraph);
			}

			public void VisitCommentMarker(ICommentMarker commentMarker)
			{
				VisitChildren(commentMarker);
			}

			public void VisitLocationMarker(ILocationMarker location)
			{
			}

			public void VisitLockedContent(ILockedContent lockedContent)
			{
				VisitChildren(lockedContent.Content);
			}

			public void VisitOtherMarker(IOtherMarker otherMarker)
			{
				VisitChildren(otherMarker);
			}

			public void VisitPlaceholderTag(IPlaceholderTag placeholderTag)
			{
				_flatMarkupList.Add(placeholderTag);
			}

			public void VisitRevisionMarker(IRevisionMarker revisionMarker)
			{
				_flatMarkupList.Add(revisionMarker);
				VisitChildren(revisionMarker);
				_flatMarkupList.Add(revisionMarker);
			}

			public void VisitSegment(ISegment segment)
			{
				VisitChildren(segment);
			}

			public void VisitTagPair(ITagPair tagPair)
			{
				_flatMarkupList.Add(tagPair);
				VisitChildren(tagPair);
				_flatMarkupList.Add(tagPair);
			}

			public void VisitText(IText text)
			{
				_flatMarkupList.Add(text);
			}

			private void VisitChildren(IAbstractMarkupDataContainer container)
			{
				foreach (IAbstractMarkupData item in container)
				{
					item.AcceptVisitor(this);
				}
			}

			public bool HasWordStopBefore(IText text)
			{
				return HasWordStop(text, -1);
			}

			public bool HasWordStopAfter(IText text)
			{
				return HasWordStop(text, 1);
			}

			private bool HasWordStop(IText text, int direction)
			{
				int num = _flatMarkupList.FindIndex((IAbstractMarkupData flatMarkupData) => flatMarkupData == text);
				if (num <= -1)
				{
					return false;
				}
				for (int i = num + direction; i >= 0 && i < _flatMarkupList.Count; i += direction)
				{
					IAbstractMarkupData abstractMarkupData = _flatMarkupList[i];
					if (!(abstractMarkupData is IText))
					{
						IAbstractTag abstractTag = abstractMarkupData as IAbstractTag;
						IRevisionMarker revisionMarker;
						if (abstractTag != null)
						{
							if (IsWordStopTag(abstractTag))
							{
								goto IL_0087;
							}
							revisionMarker = (abstractMarkupData as IRevisionMarker);
							if (revisionMarker == null)
							{
								continue;
							}
						}
						else
						{
							revisionMarker = (abstractMarkupData as IRevisionMarker);
							if (revisionMarker == null)
							{
								continue;
							}
						}
						if (!IsWordStopRevisionMarker(revisionMarker, direction))
						{
							continue;
						}
						goto IL_0087;
					}
					return false;
					IL_0087:
					return true;
				}
				return false;
			}

			private static bool IsWordStopTag(IAbstractTag abstractTag)
			{
				ITagPair tagPair = abstractTag as ITagPair;
				if (tagPair == null || tagPair.StartTagProperties == null || !tagPair.StartTagProperties.IsWordStop)
				{
					IPlaceholderTag placeholderTag = abstractTag as IPlaceholderTag;
					if (placeholderTag != null && placeholderTag.Properties != null)
					{
						return placeholderTag.Properties.IsWordStop;
					}
					return false;
				}
				return true;
			}

			private static bool IsWordStopRevisionMarker(IAbstractMarkupDataContainer revisionMarker, int direction)
			{
				string containerText = GetContainerText(revisionMarker);
				if (string.IsNullOrEmpty(containerText))
				{
					return false;
				}
				int index = (direction <= 0) ? (containerText.Length - 1) : 0;
				char c = containerText[index];
				return char.IsWhiteSpace(c);
			}

			private static string GetContainerText(IAbstractMarkupDataContainer container)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (IAbstractMarkupData allSubItem in container.AllSubItems)
				{
					IText text = allSubItem as IText;
					if (text != null)
					{
						stringBuilder.Append(text.Properties.Text);
					}
				}
				return stringBuilder.ToString();
			}
		}

		private readonly WordStopVisitor _wordStopVisitor;

		public WordStopParagraph(IAbstractMarkupDataContainer paragraph)
		{
			_wordStopVisitor = new WordStopVisitor();
			_wordStopVisitor.VisitParagraph(paragraph);
		}

		public bool HasWordStopBefore(IText text)
		{
			return _wordStopVisitor.HasWordStopBefore(text);
		}

		public bool HasWordStopAfter(IText text)
		{
			return _wordStopVisitor.HasWordStopAfter(text);
		}
	}
}
