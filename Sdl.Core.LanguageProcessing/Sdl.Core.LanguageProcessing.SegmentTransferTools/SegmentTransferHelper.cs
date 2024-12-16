using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.Formatting;
using Sdl.FileTypeSupport.Framework.Integration.QuickInserts;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.LanguageProcessing.SegmentTransferTools
{
	public class SegmentTransferHelper
	{
		private readonly object _syncObject = new object();

		private const string ArtificialIdPrefix = "pm";

		public static int MergeSegmentsInParagraphUnit(ISegment firstSegment, int toMerge)
		{
			int indexInParent = firstSegment.IndexInParent;
			int num = 0;
			int num2 = toMerge;
			while (num2 > 0)
			{
				IAbstractMarkupData abstractMarkupData = firstSegment.Parent[indexInParent + 1];
				abstractMarkupData.RemoveFromParent();
				ISegment segment = abstractMarkupData as ISegment;
				ITagPair tagPair = abstractMarkupData as ITagPair;
				if (segment != null)
				{
					foreach (IAbstractMarkupData item in segment)
					{
						firstSegment.Insert(firstSegment.Count, (IAbstractMarkupData)item.Clone());
					}
					num2--;
				}
				else if (tagPair != null)
				{
					num2 -= 2;
					firstSegment.Insert(firstSegment.Count, abstractMarkupData);
				}
				else
				{
					num2--;
					firstSegment.Insert(firstSegment.Count, abstractMarkupData);
				}
				num++;
				firstSegment.Parent.Remove(abstractMarkupData);
			}
			return num;
		}

		public static void RenumberTagIds(ParagraphUnitId paragraphUnitId, IParagraph paragraph)
		{
			int num = 1;
			foreach (IAbstractMarkupData allSubItem in paragraph.AllSubItems)
			{
				AbstractTag abstractTag = allSubItem as AbstractTag;
				if (abstractTag != null)
				{
					IAbstractTagProperties tagProperties = abstractTag.TagProperties;
					ParagraphUnitId paragraphUnitId2 = paragraphUnitId;
					tagProperties.TagId = new TagId("rt" + paragraphUnitId2.ToString() + "_" + num++.ToString());
				}
			}
		}

		public void FixTags(IEnumerable<IAbstractMarkupData> targetContainer, TagContainer tagContainer, List<IAbstractTag> alreadyFoundTags = null)
		{
			if (alreadyFoundTags == null)
			{
				alreadyFoundTags = new List<IAbstractTag>();
			}
			if (targetContainer != null)
			{
				List<IAbstractMarkupData> list = targetContainer.ToList();
				foreach (IAbstractMarkupData item in list)
				{
					IPlaceholderTag placeholderTag = item as IPlaceholderTag;
					if (placeholderTag == null)
					{
						ITagPair tagPair = item as ITagPair;
						if (tagPair == null)
						{
							ILockedContent lockedContent = item as ILockedContent;
							if (lockedContent == null)
							{
								IAbstractMarkupDataContainer abstractMarkupDataContainer = item as IAbstractMarkupDataContainer;
								if (abstractMarkupDataContainer != null)
								{
									FixTags(abstractMarkupDataContainer, tagContainer, alreadyFoundTags);
								}
							}
							else
							{
								FixTags(lockedContent.Content, tagContainer, alreadyFoundTags);
							}
						}
						else
						{
							FixAbstractTagInContainer(tagContainer, alreadyFoundTags, tagPair, delegate(IAbstractTag x)
							{
								SetTagProperties(tagPair, (ITagPair)x);
							}, delegate
							{
								tagPair.TagProperties.TagId = new TagId(GenerateArtificalTagId(tagContainer));
							});
							FixTags(tagPair, tagContainer, alreadyFoundTags);
						}
					}
					else
					{
						FixAbstractTagInContainer(tagContainer, alreadyFoundTags, placeholderTag, delegate(IAbstractTag x)
						{
							SetTagProperties(placeholderTag, (IPlaceholderTag)x);
						}, delegate
						{
							placeholderTag.TagProperties.TagId = new TagId(GenerateArtificalTagId(tagContainer));
						});
					}
				}
			}
		}

		private static void FixAbstractTagInContainer(TagContainer tagContainer, ICollection<IAbstractTag> alreadyFoundTags, IAbstractTag item, Action<IAbstractTag> propertiesSetter, Action tagIdSetter)
		{
			QuickInsertDefinitionsManager quickInsertDefinitionsManager = new QuickInsertDefinitionsManager();
			bool isnewTag;
			IAbstractTag abstractTag = tagContainer.FindRelatedTag(item, alreadyFoundTags, out isnewTag);
			if (abstractTag != null)
			{
				if (isnewTag)
				{
					alreadyFoundTags.Add(abstractTag);
				}
				propertiesSetter(abstractTag);
				if (abstractTag.SubSegments != null && abstractTag.SubSegments.Any())
				{
					RestoreSubsegments(item, abstractTag);
				}
			}
			else
			{
				if (!quickInsertDefinitionsManager.IsQuickInsert(item))
				{
					tagIdSetter();
				}
				alreadyFoundTags.Add(item);
			}
		}

		private static void SetTagProperties(IPlaceholderTag itemAsPlaceholder, IPlaceholderTag sourcePlaceholderTag)
		{
			itemAsPlaceholder.Properties = (IPlaceholderTagProperties)sourcePlaceholderTag.Properties.Clone();
		}

		private static void SetTagProperties(ITagPair itemAsTagPair, ITagPair correspondingTagPair)
		{
			itemAsTagPair.StartTagProperties = (IStartTagProperties)correspondingTagPair.StartTagProperties.Clone();
			itemAsTagPair.EndTagProperties = (IEndTagProperties)correspondingTagPair.EndTagProperties.Clone();
		}

		public bool FixNotFoundTags(ISegment targetContainer, TagContainer tagContainer)
		{
			if (tagContainer == null)
			{
				throw new ArgumentNullException("tagContainer");
			}
			int i = 0;
			Stack<IPlaceholderTag> stack = new Stack<IPlaceholderTag>();
			DocumentItemFactory documentItemFactory = new DocumentItemFactory();
			bool result = false;
			for (; i < targetContainer.Count; i++)
			{
				IPlaceholderTag placeholderTag = targetContainer[i] as IPlaceholderTag;
				if (placeholderTag == null || !placeholderTag.Properties.TagId.Id.StartsWith("pm"))
				{
					continue;
				}
				if (stack.Count > 0 && placeholderTag.Properties.TagContent.StartsWith("</"))
				{
					IPlaceholderTag placeholderTag2 = stack.Peek();
					if (placeholderTag2.Properties.DisplayText == placeholderTag.Properties.DisplayText)
					{
						IStartTagProperties startTagProperties = documentItemFactory.PropertiesFactory.CreateStartTagProperties(placeholderTag2.Properties.TagContent);
						SetStartProperties(startTagProperties, placeholderTag2);
						IEndTagProperties endTagProperties = documentItemFactory.PropertiesFactory.CreateEndTagProperties(placeholderTag.Properties.TagContent);
						SetEndProperties(endTagProperties, placeholderTag);
						int num = targetContainer.IndexOf(placeholderTag2);
						ITagPair tagPair = documentItemFactory.CreateTagPair(startTagProperties, endTagProperties);
						for (int j = num + 1; j < i; j++)
						{
							IAbstractMarkupData item = targetContainer[j].Clone() as IAbstractMarkupData;
							tagPair.Add(item);
						}
						for (int k = num + 1; k < i; k++)
						{
							targetContainer.RemoveAt(num + 1);
						}
						targetContainer.RemoveAt(num);
						targetContainer.RemoveAt(num);
						i = num;
						stack.Pop();
						targetContainer.Insert(num, tagPair);
						result = true;
					}
					else if (!placeholderTag.TagProperties.TagContent.EndsWith("/>"))
					{
						stack.Push(placeholderTag);
					}
				}
				else if (!placeholderTag.TagProperties.TagContent.EndsWith("/>"))
				{
					stack.Push(placeholderTag);
				}
			}
			return result;
		}

		private static void SetStartProperties(IStartTagProperties startProps, IPlaceholderTag prevItem)
		{
			startProps.CanHide = prevItem.Properties.CanHide;
			startProps.DisplayText = prevItem.Properties.DisplayText;
			startProps.IsSoftBreak = prevItem.Properties.IsSoftBreak;
			startProps.IsWordStop = prevItem.Properties.IsWordStop;
			startProps.Formatting = new FormattingGroup();
			foreach (KeyValuePair<string, string> metaDatum in prevItem.Properties.MetaData)
			{
				startProps.SetMetaData(metaDatum.Key, metaDatum.Value);
			}
		}

		private static void SetEndProperties(IEndTagProperties endProps, IPlaceholderTag prevItem)
		{
			endProps.CanHide = prevItem.Properties.CanHide;
			endProps.DisplayText = prevItem.Properties.DisplayText;
			endProps.IsSoftBreak = prevItem.Properties.IsSoftBreak;
			endProps.IsWordStop = prevItem.Properties.IsWordStop;
			foreach (KeyValuePair<string, string> metaDatum in prevItem.Properties.MetaData)
			{
				endProps.SetMetaData(metaDatum.Key, metaDatum.Value);
			}
		}

		public void FixUpPlaceholderIds(ISegment targetContainer, TagContainer tagContainer)
		{
			FixUpPlaceholderIdsVisitor visitor = new FixUpPlaceholderIdsVisitor(tagContainer);
			targetContainer.AcceptVisitor(visitor);
		}

		internal string GenerateArtificalTagId(TagContainer tagContainer)
		{
			lock (_syncObject)
			{
				string text;
				while (true)
				{
					text = "pm" + Guid.NewGuid().ToString();
					if (tagContainer == null)
					{
						break;
					}
					if (!tagContainer.Contains(text))
					{
						return text;
					}
				}
				return text;
			}
		}

		private static void RestoreSubsegments(IAbstractTag tagToUpdate, IAbstractTag sourceTag)
		{
			tagToUpdate.ClearSubSegmentReferences();
			foreach (ISubSegmentReference subSegment in sourceTag.SubSegments)
			{
				tagToUpdate.AddSubSegmentReference((ISubSegmentReference)subSegment.Clone());
			}
		}
	}
}
