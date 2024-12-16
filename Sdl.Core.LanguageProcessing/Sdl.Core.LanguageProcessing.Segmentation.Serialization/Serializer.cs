using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Sdl.Core.LanguageProcessing.Segmentation.Serialization
{
	public class Serializer
	{
		private readonly IDictionary<int, IRevisionProperties> _revisionTokenIdProperties = new Dictionary<int, IRevisionProperties>();

		public IParagraphUnit GetParagraphUnitFromContentString(string contentString, IDocumentItemFactory documentItemFactory, IDocumentProperties documentProperties)
		{
			IParagraphUnit paragraphUnit = documentItemFactory.CreateParagraphUnit(LockTypeFlags.Unlocked);
			TokenEnumerator tokenEnumerator = new TokenEnumerator(contentString);
			if (!tokenEnumerator.MoveNext())
			{
				return paragraphUnit;
			}
			IEnumerable<IAbstractMarkupData> enumerable = ParseContent(tokenEnumerator, documentItemFactory, documentProperties);
			foreach (IAbstractMarkupData item in enumerable)
			{
				paragraphUnit.Source.Add(item);
			}
			return paragraphUnit;
		}

		public string GetContentStringFromParagraphUnit(IParagraphUnit paragraphUnit)
		{
			return GetContentString(paragraphUnit.Source);
		}

		private IEnumerable<IAbstractMarkupData> ParseContent(TokenEnumerator contentTokens, IDocumentItemFactory documentItemFactory, IDocumentProperties documentProperties)
		{
			IList<IAbstractMarkupData> list = new List<IAbstractMarkupData>();
			do
			{
				Token current = contentTokens.Current;
				if (!(current is StartTagPairToken))
				{
					if (current is EndTagPairToken)
					{
						contentTokens.MovePrevious();
						return list;
					}
					if (!(current is StartLockedToken))
					{
						if (current is EndLockedToken)
						{
							contentTokens.MovePrevious();
							return list;
						}
						if (!(current is StartRevisionMarkerToken))
						{
							if (current is EndRevisionMarkerToken)
							{
								contentTokens.MovePrevious();
								return list;
							}
							if (!(current is StartComentMarker))
							{
								if (current is EndComentMarker)
								{
									contentTokens.MovePrevious();
									return list;
								}
								if (!(current is PlaceholderTagToken))
								{
									if (current is TextToken)
									{
										list.Add(ParseContentText(contentTokens, documentItemFactory));
									}
								}
								else
								{
									list.Add(ParseContentPlaceholder(contentTokens, documentItemFactory));
								}
							}
							else
							{
								list.Add(ParseContentCommentMarker(contentTokens, documentItemFactory, documentProperties));
							}
						}
						else
						{
							list.Add(ParseContentRevision(contentTokens, documentItemFactory, documentProperties));
						}
					}
					else
					{
						list.Add(ParseContentLocked(contentTokens, documentItemFactory, documentProperties));
					}
				}
				else
				{
					list.Add(ParseContentTagPair(contentTokens, documentItemFactory, documentProperties));
				}
			}
			while (contentTokens.MoveNext());
			return list;
		}

		private ITagPair ParseContentTagPair(TokenEnumerator contentTokens, IDocumentItemFactory documentItemFactory, IDocumentProperties documentProperties)
		{
			ITagPair tagPair = documentItemFactory.CreateTagPair(documentItemFactory.PropertiesFactory.CreateStartTagProperties("<t>"), documentItemFactory.PropertiesFactory.CreateEndTagProperties("</t>"));
			tagPair.StartTagProperties.SegmentationHint = ((StartTagPairToken)contentTokens.Current).SegmentationHint;
			tagPair.StartTagProperties.IsWordStop = ((StartTagPairToken)contentTokens.Current).IsWordStop;
			tagPair.StartTagProperties.CanHide = ((StartTagPairToken)contentTokens.Current).CanHide;
			contentTokens.MoveNext();
			IEnumerable<IAbstractMarkupData> enumerable = ParseContent(contentTokens, documentItemFactory, documentProperties);
			foreach (IAbstractMarkupData item in enumerable)
			{
				tagPair.Add(item);
			}
			contentTokens.MoveNext();
			return tagPair;
		}

		private ILockedContent ParseContentLocked(TokenEnumerator contentTokens, IDocumentItemFactory documentItemFactory, IDocumentProperties documentProperties)
		{
			ILockedContent lockedContent = documentItemFactory.CreateLockedContent(documentItemFactory.PropertiesFactory.CreateLockedContentProperties(LockTypeFlags.Manual));
			contentTokens.MoveNext();
			IEnumerable<IAbstractMarkupData> enumerable = ParseContent(contentTokens, documentItemFactory, documentProperties);
			foreach (IAbstractMarkupData item in enumerable)
			{
				lockedContent.Content.Add(item);
			}
			contentTokens.MoveNext();
			return lockedContent;
		}

		private ICommentMarker ParseContentCommentMarker(TokenEnumerator contentTokens, IDocumentItemFactory documentItemFactory, IDocumentProperties documentProperties)
		{
			ICommentMarker commentMarker = documentItemFactory.CreateCommentMarker(documentItemFactory.PropertiesFactory.CreateCommentProperties());
			contentTokens.MoveNext();
			IEnumerable<IAbstractMarkupData> enumerable = ParseContent(contentTokens, documentItemFactory, documentProperties);
			foreach (IAbstractMarkupData item in enumerable)
			{
				commentMarker.Add(item);
			}
			contentTokens.MoveNext();
			return commentMarker;
		}

		private IRevisionMarker ParseContentRevision(TokenEnumerator contentTokens, IDocumentItemFactory documentItemFactory, IDocumentProperties documentProperties)
		{
			int revisionMarkerTokenId = ((StartRevisionMarkerToken)contentTokens.Current).RevisionMarkerTokenId;
			IRevisionMarker revisionMarker = documentItemFactory.CreateRevision(GetRevisionProperties(revisionMarkerTokenId));
			contentTokens.MoveNext();
			IEnumerable<IAbstractMarkupData> enumerable = ParseContent(contentTokens, documentItemFactory, documentProperties);
			foreach (IAbstractMarkupData item in enumerable)
			{
				revisionMarker.Add(item);
			}
			contentTokens.MoveNext();
			return revisionMarker;
		}

		private static IPlaceholderTag ParseContentPlaceholder(TokenEnumerator contentTokens, IDocumentItemFactory documentItemFactory)
		{
			IPlaceholderTag placeholderTag = documentItemFactory.CreatePlaceholderTag(documentItemFactory.PropertiesFactory.CreatePlaceholderTagProperties("<p/>"));
			placeholderTag.Properties.SegmentationHint = ((PlaceholderTagToken)contentTokens.Current).SegmentationHint;
			placeholderTag.Properties.IsWordStop = ((PlaceholderTagToken)contentTokens.Current).IsWordStop;
			return placeholderTag;
		}

		private static IText ParseContentText(TokenEnumerator contentTokens, IDocumentItemFactory documentItemFactory)
		{
			string text = ((TextToken)contentTokens.Current).Text;
			return documentItemFactory.CreateText(documentItemFactory.PropertiesFactory.CreateTextProperties(text));
		}

		public string GetContentString(IAbstractMarkupDataContainer container, bool showOtherDetails = false)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (IAbstractMarkupData item in container)
			{
				ISegment segment = item as ISegment;
				if (segment == null)
				{
					ITagPair tagPair = item as ITagPair;
					if (tagPair == null)
					{
						ILockedContent lockedContent = item as ILockedContent;
						if (lockedContent == null)
						{
							ICommentMarker commentMarker = item as ICommentMarker;
							if (commentMarker == null)
							{
								IRevisionMarker revisionMarker = item as IRevisionMarker;
								if (revisionMarker == null)
								{
									IText text = item as IText;
									if (text != null)
									{
										bool flag = IsNextItemText(text) || IspreviousItemText(text);
										if (flag)
										{
											stringBuilder.Append("<text>");
										}
										stringBuilder.Append((text.Properties.Text == string.Empty) ? "<0>" : text.Properties.Text);
										if (flag)
										{
											stringBuilder.Append("</text>");
										}
										continue;
									}
								}
								else
								{
									string str = GetRevisionMarkerTokenId(revisionMarker.Properties).ToString(CultureInfo.InvariantCulture);
									if (showOtherDetails)
									{
										switch (revisionMarker.Properties.RevisionType)
										{
										case RevisionType.Insert:
											str = "i";
											break;
										case RevisionType.Delete:
											str = "d";
											break;
										}
									}
									stringBuilder.Append("<r" + str + ">");
									stringBuilder.Append(GetContentString(revisionMarker, showOtherDetails));
									stringBuilder.Append("</r" + str + ">");
								}
							}
							else
							{
								stringBuilder.Append("<c>");
								stringBuilder.Append(GetContentString(commentMarker, showOtherDetails));
								stringBuilder.Append("</c>");
							}
						}
						else
						{
							stringBuilder.Append("<l>");
							stringBuilder.Append(GetContentString(lockedContent.Content, showOtherDetails));
							stringBuilder.Append("</l>");
						}
						IPlaceholderTag placeholderTag = item as IPlaceholderTag;
						if (placeholderTag != null)
						{
							char controlChar = GetControlChar(placeholderTag.Properties.SegmentationHint, placeholderTag.Properties.IsWordStop);
							if (showOtherDetails)
							{
								stringBuilder.Append(placeholderTag.Properties.TagContent);
							}
							else
							{
								stringBuilder.Append("<p");
								stringBuilder.Append(controlChar);
								stringBuilder.Append("/>");
							}
						}
					}
					else
					{
						char controlChar2 = GetControlChar(tagPair.StartTagProperties.SegmentationHint, tagPair.StartTagProperties.IsWordStop);
						stringBuilder.Append("<t");
						stringBuilder.Append(controlChar2);
						if (tagPair.StartTagProperties.CanHide)
						{
							stringBuilder.Append("h");
						}
						stringBuilder.Append(">");
						stringBuilder.Append(GetContentString(tagPair, showOtherDetails));
						stringBuilder.Append("</t");
						stringBuilder.Append(controlChar2);
						if (tagPair.StartTagProperties.CanHide)
						{
							stringBuilder.Append("h");
						}
						stringBuilder.Append(">");
					}
				}
				else
				{
					stringBuilder.Append("<s>");
					stringBuilder.Append(GetContentString(segment, showOtherDetails));
					stringBuilder.Append("</s>");
				}
			}
			return stringBuilder.ToString();
		}

		private static bool IsNextItemText(IAbstractMarkupData text)
		{
			IAbstractMarkupDataContainer parent = text.Parent;
			if (text.IndexInParent == parent.Count - 1)
			{
				return false;
			}
			return parent[text.IndexInParent + 1] is IText;
		}

		private static bool IspreviousItemText(IAbstractMarkupData text)
		{
			IAbstractMarkupDataContainer parent = text.Parent;
			if (text.IndexInParent == 0)
			{
				return false;
			}
			return parent[text.IndexInParent - 1] is IText;
		}

		private static char GetControlChar(SegmentationHint segmentationHint, bool isWordStop)
		{
			if (isWordStop)
			{
				return 'w';
			}
			switch (segmentationHint)
			{
			case SegmentationHint.Exclude:
				return 'e';
			case SegmentationHint.Include:
				return 'i';
			case SegmentationHint.IncludeWithText:
				return 't';
			case SegmentationHint.MayExclude:
				return 'm';
			default:
				return 'u';
			}
		}

		internal void CreateRevisionProperties(IDocumentItemFactory documentItemFactory)
		{
			_revisionTokenIdProperties.Clear();
			for (int i = 0; i < 10; i++)
			{
				_revisionTokenIdProperties.Add(i, documentItemFactory.PropertiesFactory.CreateRevisionProperties(RevisionType.Insert));
			}
		}

		private IRevisionProperties GetRevisionProperties(int revisionMarkerTokenId)
		{
			return _revisionTokenIdProperties[revisionMarkerTokenId];
		}

		private int GetRevisionMarkerTokenId(IRevisionProperties revisionProperties)
		{
			if (_revisionTokenIdProperties == null || _revisionTokenIdProperties.Count == 0)
			{
				return 0;
			}
			return (from revisionTokenIdPropertiesPair in _revisionTokenIdProperties
				where revisionTokenIdPropertiesPair.Value == revisionProperties
				select revisionTokenIdPropertiesPair.Key).First();
		}
	}
}
