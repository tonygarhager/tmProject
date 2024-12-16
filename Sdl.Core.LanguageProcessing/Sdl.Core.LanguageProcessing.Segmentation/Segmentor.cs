using Sdl.Core.LanguageProcessing.Segmentation.Serialization;
using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.LanguagePlatform.Core.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Sdl.Core.LanguageProcessing.Segmentation
{
	public class Segmentor : AbstractBilingualFileTypeComponent, IBilingualContentHandler
	{
		private SegmentationEngine _segmentationEngine;

		private IFileProperties _fileInfo;

		private readonly IResourceDataAccessor _resourceDataAccessor;

		public double SegmentationDuration
		{
			get;
			private set;
		}

		public Settings Settings
		{
			get;
		}

		public int SegmentCount
		{
			get;
			private set;
		}

		public int ParagraphCount
		{
			get;
			private set;
		}

		public Segmentor(Settings settings)
			: this(settings, null)
		{
		}

		public Segmentor(Settings settings, IResourceDataAccessor resourceDataAccessor)
		{
			Settings = (settings ?? new Settings());
			_resourceDataAccessor = resourceDataAccessor;
		}

		public void Initialize(IDocumentProperties documentInfo)
		{
			_segmentationEngine = SegmentationEngineFactory.CreateSegmentationEngine(_resourceDataAccessor, documentInfo.SourceLanguage.CultureInfo);
			int num3 = ParagraphCount = (SegmentCount = 0);
		}

		public void Initialize(CultureInfo culture, string ruleFilePath)
		{
			_segmentationEngine = SegmentationEngineFactory.CreateSegmentationEngine(culture, _resourceDataAccessor, ruleFilePath);
			int num3 = ParagraphCount = (SegmentCount = 0);
		}

		public void Complete()
		{
		}

		public void SetFileProperties(IFileProperties fileInfo)
		{
			_fileInfo = fileInfo;
		}

		public void FileComplete()
		{
		}

		public void ProcessParagraphUnit(IParagraphUnit transUnit)
		{
			if (transUnit.IsStructure || transUnit.Properties.LockType != 0)
			{
				return;
			}
			int count = transUnit.Target.Count;
			if (Settings.DontSegmentIfTargetExists && count > 0)
			{
				return;
			}
			int num = ++ParagraphCount;
			if (transUnit.Source != null && transUnit.Source.Count > 0)
			{
				ProcessParagraph(transUnit.Source);
			}
			transUnit.Target?.Clear();
			if (Settings.TargetSegmentCreationMode == TargetSegmentCreationMode.CreateEmptyTarget || Settings.TargetSegmentCreationMode == TargetSegmentCreationMode.CopySource)
			{
				CopySource(transUnit);
				if (Settings.TargetSegmentCreationMode == TargetSegmentCreationMode.CreateEmptyTarget)
				{
					ClearSegments(transUnit.Target);
				}
				RemoveCommentMarkers(transUnit.Target);
				if (Settings.TargetSegmentCreationMode == TargetSegmentCreationMode.CopySource)
				{
					AcceptTrackChanges(transUnit.Target);
				}
			}
		}

		private static void AcceptTrackChanges(IAbstractMarkupDataContainer paragraph)
		{
			List<IRevisionMarker> list = paragraph.AllSubItems.OfType<IRevisionMarker>().ToList();
			foreach (IRevisionMarker item in list)
			{
				switch (item.Properties.RevisionType)
				{
				case RevisionType.Insert:
				case RevisionType.Unchanged:
					item.MoveAllItemsTo(item.Parent, item.IndexInParent);
					item.RemoveFromParent();
					break;
				case RevisionType.Delete:
					item.RemoveFromParent();
					break;
				}
			}
		}

		private static void CopySource(IParagraphUnit transUnit)
		{
			foreach (IAbstractMarkupData item2 in transUnit.Source)
			{
				IAbstractMarkupData item = (IAbstractMarkupData)item2.Clone();
				transUnit.Target.Add(item);
			}
		}

		private void ProcessParagraph(IAbstractMarkupDataContainer paragraph)
		{
			switch (Settings.Mode)
			{
			case Mode.Disabled:
				break;
			case Mode.AllContentAsOneSegment:
			{
				InsertSegment(paragraph, 0, paragraph.Count);
				int num = ++SegmentCount;
				break;
			}
			case Mode.SentenceSegmentation:
				ConcatenateTextChains(paragraph);
				CreateSegments(paragraph);
				ConcatenateTextChains(paragraph);
				break;
			case Mode.ParagraphSegmentation:
				throw new NotImplementedException();
			}
		}

		private static void ConcatenateTextChains(IAbstractMarkupDataContainer paragraph)
		{
			Location startLocation = new Location(new LevelLocation(paragraph, 0));
			ContentIterator contentIterator = new ContentIterator(startLocation, null, skipsegments: false);
			do
			{
				if (contentIterator.CurrentData is Text)
				{
					IText text = (IText)contentIterator.CurrentData;
					for (IText sibblingOnSameLevel = GetSibblingOnSameLevel(text); sibblingOnSameLevel != null; sibblingOnSameLevel = GetSibblingOnSameLevel(text))
					{
						MergeTexts(text, sibblingOnSameLevel);
					}
				}
			}
			while (contentIterator.Next());
		}

		private static void MergeTexts(IText text, IText nexttext)
		{
			text.Properties.Text = text.Properties.Text + nexttext.Properties.Text;
			text.Parent.RemoveAt(nexttext.IndexInParent);
		}

		private static IText GetSibblingOnSameLevel(IAbstractMarkupData text)
		{
			IAbstractMarkupDataContainer parent = text.Parent;
			if (text.IndexInParent == parent.Count - 1)
			{
				return null;
			}
			return parent[text.IndexInParent + 1] as IText;
		}

		private void CreateSegments(IAbstractMarkupDataContainer content, SegmentorUtility segmentorUtility = null)
		{
			if (segmentorUtility == null)
			{
				BomSegmentationTreeManager bomSegmentationTreeManager = new BomSegmentationTreeManager();
				bomSegmentationTreeManager.PopulateTree(content);
				if (bomSegmentationTreeManager.Tree.Root.LastChild == null || !bomSegmentationTreeManager.Tree.Root.LastChild.IsInsideSegment)
				{
					bomSegmentationTreeManager.RunSegmentationEngine(_segmentationEngine);
					bomSegmentationTreeManager.ApplySegmentationInfo();
					bomSegmentationTreeManager.SetLastEnder();
					bomSegmentationTreeManager.SetRemainingEnders();
					bomSegmentationTreeManager.SetStarters();
					bomSegmentationTreeManager.GetInitialIncludesWithStartersAndEnders();
					bomSegmentationTreeManager.ProcessLeadingSpaces(moveOutsideContainer: true);
					bomSegmentationTreeManager.HandleContainerSplits();
					bomSegmentationTreeManager.ProcessTrailingSpaces(moveOutsideContainer: true);
					bomSegmentationTreeManager.DoFinalPromotion();
					bomSegmentationTreeManager.GetIncludesWithStartersAndEnders();
					bomSegmentationTreeManager.FinalizeWhitespace();
					RemoveNestedSegments(bomSegmentationTreeManager);
					CreateSegments(bomSegmentationTreeManager);
				}
			}
		}

		private void CreateSegments(BomSegmentationTreeManager manager)
		{
			IAbstractMarkupData abstractMarkupData = manager.FindNextStarter();
			bool flag = false;
			while (abstractMarkupData != null)
			{
				IAbstractMarkupData abstractMarkupData2 = manager.FindNextEnder();
				if (abstractMarkupData2 == null)
				{
					abstractMarkupData2 = ((BomNode)manager.Tree.Root.LastChild).Item;
					flag = true;
				}
				bool flag2 = true;
				if (abstractMarkupData == abstractMarkupData2)
				{
					flag2 = manager.MoveToNextBoundary();
				}
				IAbstractMarkupDataContainer parent = abstractMarkupData.Parent;
				int indexInParent = abstractMarkupData.IndexInParent;
				int indexInParent2 = abstractMarkupData2.IndexInParent;
				int numberOfItems = indexInParent2 - indexInParent + 1;
				InsertSegment(parent, indexInParent, numberOfItems);
				if (!(!flag2 | flag))
				{
					abstractMarkupData = manager.FindNextStarter();
					continue;
				}
				break;
			}
		}

		private void RemoveNestedSegments(BomSegmentationTreeManager manager)
		{
			Node node = manager.Tree.Root.FirstChild;
			while (true)
			{
				if (node != null)
				{
					if (node.IsSegmentStarter && node.DescendantStarters > 0)
					{
						break;
					}
					node = node.Next;
					continue;
				}
				return;
			}
			node.ResetStartersAndEndersInContainer();
			manager.Tree.Root.FirstChild.IsSegmentStarter = true;
			manager.Tree.Root.LastChild.IsSegmentEnder = true;
		}

		private static void ClearSegments(IAbstractMarkupDataContainer paragraph)
		{
			Location location = new Location(new LevelLocation(paragraph, 0));
			while (location.IsValid)
			{
				if (location.BottomLevel.IsAtEndOfParent)
				{
					location.Levels.Remove(location.BottomLevel);
					if (!location.IsValid)
					{
						break;
					}
					int num = ++location.BottomLevel.Index;
					continue;
				}
				IAbstractMarkupData itemAtLocation = location.ItemAtLocation;
				ISegment segment = itemAtLocation as ISegment;
				if (segment == null)
				{
					IAbstractMarkupDataContainer abstractMarkupDataContainer = itemAtLocation as IAbstractMarkupDataContainer;
					if (abstractMarkupDataContainer == null)
					{
						int num = ++location.BottomLevel.Index;
					}
					else
					{
						location.Levels.Add(new LevelLocation(abstractMarkupDataContainer, 0));
					}
				}
				else
				{
					segment.Clear();
					int num = ++location.BottomLevel.Index;
				}
			}
		}

		private static void RemoveCommentMarkers(IAbstractMarkupDataContainer paragraph)
		{
			IEnumerable<ICommentMarker> enumerable = paragraph.AllSubItems.OfType<ICommentMarker>().ToList();
			foreach (ICommentMarker item in enumerable)
			{
				item.MoveAllItemsTo(item.Parent, item.IndexInParent);
				item.RemoveFromParent();
			}
		}

		private void InsertSegment(IAbstractMarkupDataContainer content, int firstItem, int numberOfItems)
		{
			if (numberOfItems > 0 && content.Count >= numberOfItems)
			{
				ISegmentPairProperties properties = ItemFactory.CreateSegmentPairProperties();
				ISegment segment = ItemFactory.CreateSegment(properties);
				content.MoveItemsTo(segment, firstItem, numberOfItems);
				content.Insert(firstItem, segment);
			}
		}

		private void PrintParagraph(string messageType, IAbstractMarkupDataContainer content)
		{
			Serializer serializer = new Serializer();
			int num = 0;
			if (_fileInfo?.FileConversionProperties == null)
			{
				return;
			}
			string contentString = serializer.GetContentString(content);
			string name = new DirectoryInfo(_fileInfo.FileConversionProperties.OriginalFilePath).Name;
			StreamWriter streamWriter = new StreamWriter("c:\\temp\\new\\" + name + ".txt", append: true);
			string text = messageType + "=>[" + contentString + "]";
			streamWriter.WriteLine(text);
			if (messageType.Contains("output") && text.IndexOf("<s>", StringComparison.Ordinal) > 0)
			{
				streamWriter.WriteLine("-----");
				string text2 = text;
				int num2 = 0;
				int num3 = 1;
				do
				{
					int num4 = text2.IndexOf("<s>", StringComparison.Ordinal);
					if (num3 == 1 && num4 > 30)
					{
						streamWriter.WriteLine("Possible content outside segment!!!");
					}
					streamWriter.WriteLine("index s " + num4.ToString());
					if (num4 < num2)
					{
						streamWriter.WriteLine("Segmentation error!!!");
					}
					num2 = text2.IndexOf("</s>", StringComparison.Ordinal);
					streamWriter.WriteLine("index /s " + num2.ToString());
					text2 = text2.Remove(num4, 3);
					num2 -= 3;
					text2 = text2.Remove(num2, 4);
					num3++;
				}
				while (text2.Contains("<s>"));
			}
			try
			{
				if (messageType.Contains("input") && text.IndexOf("<tm>", StringComparison.Ordinal) > 0)
				{
					string text3 = text;
					num = 0;
					do
					{
						num++;
						int startIndex = text3.IndexOf("<tm>", StringComparison.Ordinal);
						text3 = text3.Remove(startIndex, 4);
					}
					while (text3.Contains("<tm>"));
				}
				if (messageType.Contains("output") && text.IndexOf("<tm>", StringComparison.Ordinal) > 0)
				{
					string text2 = text;
					int num5 = 0;
					do
					{
						num5++;
						int startIndex2 = text2.IndexOf("<tm>", StringComparison.Ordinal);
						text2 = text2.Remove(startIndex2, 4);
					}
					while (text2.Contains("<tm>"));
					if (num5 > num)
					{
						streamWriter.WriteLine("Extra tu-s");
					}
				}
			}
			catch (Exception ex)
			{
				streamWriter.WriteLine("some exception " + ex?.ToString());
			}
			streamWriter.WriteLine("");
			streamWriter.WriteLine("");
			streamWriter.Close();
		}
	}
}
