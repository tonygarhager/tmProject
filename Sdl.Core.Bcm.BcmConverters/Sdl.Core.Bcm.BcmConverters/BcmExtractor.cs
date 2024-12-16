using Newtonsoft.Json;
using Sdl.Core.Bcm.BcmConverters.Common;
using Sdl.Core.Bcm.BcmConverters.FromBilingualApi;
using Sdl.Core.Bcm.BcmConverters.FromBilingualApi.Extensions;
using Sdl.Core.Bcm.BcmConverters.FromBilingualApi.Helpers;
using Sdl.Core.Bcm.BcmModel;
using Sdl.Core.Bcm.BcmModel.Operations;
using Sdl.Core.Bcm.BcmModel.Skeleton;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sdl.Core.Bcm.BcmConverters
{
	public class BcmExtractor : AbstractBilingualContentHandler
	{
		private readonly Sdl.Core.Bcm.BcmConverters.FromBilingualApi.Helpers.IdGenerator _punitIndexGenerator;

		private Document _document;

		private IFileProperties _fileInfo;

		private Sdl.Core.Bcm.BcmModel.File _currentFile;

		private ContextHierarchy _contextHierarchy;

		public Document OutputDocument => _document;

		public Language SourceLanguage
		{
			get;
			set;
		}

		public Language TargetLanguage
		{
			get;
			set;
		}

		public Action<DependencyFile> DependencyFileHandler
		{
			get;
			set;
		}

		public BcmExtractionSettings BcmExtractionSettings
		{
			get;
			set;
		}

		public event EventHandler ParagraphUnitProcessing;

		public event EventHandler<ParagraphUnitEventArgs> ParagraphUnitProcessed;

		public event Action<IFileProperties> FilePropertiesSet;

		public event EventHandler DependencyFilesAdded;

		public BcmExtractor()
		{
			DependencyFileHandler = delegate
			{
			};
			_punitIndexGenerator = new Sdl.Core.Bcm.BcmConverters.FromBilingualApi.Helpers.IdGenerator();
			BcmExtractionSettings = new BcmExtractionSettings();
		}

		public override void Initialize(IDocumentProperties documentInfo)
		{
			base.Initialize(documentInfo);
			_document = new Document
			{
				Name = Path.GetFileName(documentInfo.LastOpenedAsPath)
			};
		}

		public override void SetFileProperties(IFileProperties fileInfo)
		{
			_fileInfo = fileInfo;
			if (string.IsNullOrEmpty(_document.Name))
			{
				_document.Name = Path.GetFileName(fileInfo.FileConversionProperties.OriginalFilePath);
			}
			base.SetFileProperties(fileInfo);
			_currentFile = new Sdl.Core.Bcm.BcmModel.File();
			_document.Files.Add(_currentFile);
			_contextHierarchy = new ContextHierarchy(_currentFile.Skeleton);
			SetDocumentLanguages();
			SerializeSniffInfo(fileInfo.FileConversionProperties.FileSnifferInfo);
			OnFilePropertiesSet(fileInfo);
		}

		private void SerializeSniffInfo(Sdl.FileTypeSupport.Framework.NativeApi.SniffInfo fileSnifferInfo)
		{
			if (fileSnifferInfo != null)
			{
				Sdl.Core.Bcm.BcmConverters.Common.SniffInfo value = MarkupDataConverter.Convert(fileSnifferInfo);
				string value2 = JsonConvert.SerializeObject(value);
				string value3 = JsonConvert.SerializeObject(value2);
				_currentFile.SetMetadata("frameworkOriginalSniffInfo", value3);
			}
		}

		private void SetDocumentLanguages()
		{
			if (SourceLanguage == null)
			{
				SourceLanguage = _fileInfo.FileConversionProperties.SourceLanguage;
			}
			if (TargetLanguage == null)
			{
				TargetLanguage = _fileInfo.FileConversionProperties.TargetLanguage;
			}
			if (SourceLanguage != null)
			{
				_document.SourceLanguageCode = (SourceLanguage.IsValid ? SourceLanguage.IsoAbbreviation : null);
				_document.SourceLanguageName = (SourceLanguage.IsValid ? SourceLanguage.DisplayName : null);
			}
			if (TargetLanguage != null)
			{
				_document.TargetLanguageCode = (TargetLanguage.IsValid ? TargetLanguage.IsoAbbreviation : null);
				_document.TargetLanguageName = (TargetLanguage.IsValid ? TargetLanguage.DisplayName : null);
			}
		}

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			base.ProcessParagraphUnit(paragraphUnit);
			OnParagraphUnitProcessing();
			ParagraphUnit paragraphUnit2 = new ParagraphUnit
			{
				ParentFileId = _currentFile.Id,
				Index = int.Parse(_punitIndexGenerator.GetNextNumericalId()),
				IsStructure = paragraphUnit.IsStructure,
				IsLocked = (paragraphUnit.Properties.LockType != LockTypeFlags.Unlocked),
				Source = Convert(paragraphUnit.Source),
				Target = Convert(paragraphUnit.Target)
			};
			SetOriginalIdMetadata(paragraphUnit, paragraphUnit2);
			CopyParagraphUnitComments(paragraphUnit, paragraphUnit2);
			CopyParagraphUnitContexts(paragraphUnit, paragraphUnit2);
			_currentFile.ParagraphUnits.Add(paragraphUnit2);
			OnParagraphUnitProcessed(new ParagraphUnitEventArgs(paragraphUnit2));
		}

		private static void SetOriginalIdMetadata(IParagraphUnit paragraphUnit, ParagraphUnit result)
		{
			result.SetMetadata("frameworkOriginalParagraphUnitId", paragraphUnit.Properties.ParagraphUnitId.Id);
		}

		private void CopyParagraphUnitComments(IParagraphUnit sourceParagraph, ParagraphUnit targetParagraph)
		{
			if (sourceParagraph.Properties.Comments != null)
			{
				if (targetParagraph.CommentDefinitionIds == null)
				{
					targetParagraph.CommentDefinitionIds = new List<int>();
				}
				foreach (IComment comment in sourceParagraph.Properties.Comments.Comments)
				{
					CommentDefinition elem = MarkupDataConverter.Convert(comment);
					int id = _currentFile.Skeleton.CommentDefinitions.GetOrAdd(elem).Id;
					targetParagraph.CommentDefinitionIds.Add(id);
				}
			}
		}

		private void CopyParagraphUnitContexts(IParagraphUnit sourceParagraph, ParagraphUnit targetParagraph)
		{
			Tuple<int, IEnumerable<int>> tuple = _contextHierarchy.AddContextProperties(sourceParagraph.Properties.ParagraphUnitId.Id, sourceParagraph.Properties.Contexts, BcmExtractionSettings.GenerateContextsDependencyFile);
			int item = tuple.Item1;
			if (item != -1)
			{
				targetParagraph.StructureContextId = item;
			}
			if (tuple.Item2 != null)
			{
				targetParagraph.ContextList = tuple.Item2.ToList();
			}
		}

		public override void FileComplete()
		{
			FilePropertiesBuilder filePropertiesBuilder = new FilePropertiesBuilder(_fileInfo, _currentFile.Skeleton, _contextHierarchy)
			{
				DependencyFileHandler = DependencyFileHandler,
				DependencyFilesAdded = this.DependencyFilesAdded
			};
			filePropertiesBuilder.AddFileProperties(_currentFile);
			PopulateSkeletonQuickTagDefinitions();
			base.FileComplete();
		}

		public override void Complete()
		{
			_currentFile.CopyMetadataFrom(_fileInfo.FileConversionProperties);
			base.Complete();
		}

		private void PopulateSkeletonQuickTagDefinitions()
		{
			if (_currentFile.Skeleton?.QuickInsertIds != null)
			{
				FileSkeleton skeleton = _currentFile.Skeleton;
				if (skeleton == null || skeleton.QuickInsertIds.Count != 0)
				{
					foreach (string quickInsertId in _currentFile.Skeleton.QuickInsertIds)
					{
						QuickInsertHelper.AddQuickTagDefinitionToSkeleton(quickInsertId, _currentFile.Skeleton);
					}
				}
			}
		}

		private Paragraph Convert(IParagraph paragraph)
		{
			Paragraph paragraph2 = new Paragraph();
			MarkupDataContainerVisitorData data = new MarkupDataContainerVisitorData(_currentFile, BcmExtractionSettings);
			MarkupDataContainerVisitor markupDataContainerVisitor = new MarkupDataContainerVisitor(data);
			paragraph2.AddRange(markupDataContainerVisitor.Visit(paragraph));
			paragraph2.NormalizeTextItems();
			return paragraph2;
		}

		private void OnParagraphUnitProcessing()
		{
			this.ParagraphUnitProcessing?.Invoke(this, EventArgs.Empty);
		}

		private void OnParagraphUnitProcessed(ParagraphUnitEventArgs e)
		{
			this.ParagraphUnitProcessed?.Invoke(this, e);
		}

		private void OnFilePropertiesSet(IFileProperties obj)
		{
			this.FilePropertiesSet?.Invoke(obj);
		}
	}
}
