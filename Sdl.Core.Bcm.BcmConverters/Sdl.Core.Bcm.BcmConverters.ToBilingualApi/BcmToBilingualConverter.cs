using Sdl.Core.Bcm.BcmConverters.Common;
using Sdl.Core.Bcm.BcmConverters.ToBilingualApi.Extensions;
using Sdl.Core.Bcm.BcmConverters.ToBilingualApi.MarkupDataConverters;
using Sdl.Core.Bcm.BcmModel;
using Sdl.Core.Bcm.BcmModel.Skeleton;
using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.Formatting;
using Sdl.FileTypeSupport.Framework.Formatting;
using Sdl.FileTypeSupport.Framework.Native;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sdl.Core.Bcm.BcmConverters.ToBilingualApi
{
	internal class BcmToBilingualConverter
	{
		private readonly Sdl.Core.Bcm.BcmModel.File _file;

		private readonly DocumentItemFactory _documentItemFactory;

		private readonly PropertiesFactory _propertiesFactory;

		private IContextProperties _lastContextProperties;

		private readonly ContextTable _fullContextTable;

		private ContextPropertiesItem _lastCtxInfoItem;

		private readonly MarkupDataConverterFactory _dataConverterFactory;

		private readonly FileSkeleton _skeleton;

		public BcmToBilingualConverter(Sdl.Core.Bcm.BcmModel.File file, IFileProperties properties)
		{
			_file = file;
			_propertiesFactory = new PropertiesFactory();
			_documentItemFactory = new DocumentItemFactory();
			_dataConverterFactory = new MarkupDataConverterFactory(_propertiesFactory, _documentItemFactory, _file.Skeleton);
			_skeleton = _file.Skeleton;
			DependencyFile dependencyFile = _file.DependencyFiles.FirstOrDefault((DependencyFile x) => x.Id == "fullContextsTable");
			if (dependencyFile != null && System.IO.File.Exists(dependencyFile.FileName))
			{
				_fullContextTable = ContextTable.DeserializeFullContexts(dependencyFile.FileName);
			}
			if (dependencyFile != null)
			{
				properties?.FileConversionProperties.SetMetaData("SDL:ContextFileLocation", dependencyFile.Location);
			}
			_dataConverterFactory.ContextTable = _fullContextTable;
		}

		public IParagraphUnit Convert(Sdl.Core.Bcm.BcmModel.ParagraphUnit pu)
		{
			LockTypeFlags flags = pu.IsStructure ? LockTypeFlags.Structure : LockTypeFlags.Unlocked;
			IParagraphUnit paragraphUnit = _documentItemFactory.CreateParagraphUnit(flags);
			string metadata = pu.GetMetadata("frameworkOriginalParagraphUnitId");
			paragraphUnit.Properties.ParagraphUnitId = new ParagraphUnitId(metadata);
			pu.RemoveMetadata("frameworkOriginalParagraphUnitId");
			if (_fullContextTable == null)
			{
				SetParagraphUnitContexts(pu, paragraphUnit);
			}
			else
			{
				SetParagraphUnitContexts(metadata, paragraphUnit);
			}
			paragraphUnit.Properties.Comments = Convert(pu.CommentDefinitionIds);
			paragraphUnit.Source.ConvertAndAddChildren(pu.Source.Children, _dataConverterFactory);
			paragraphUnit.Target.ConvertAndAddChildren(pu.Target.Children, _dataConverterFactory);
			_dataConverterFactory.ResetSegmentProperties(_documentItemFactory);
			return paragraphUnit;
		}

		private void SetParagraphUnitContexts(string punitId, IParagraphUnit result)
		{
			result.Properties.Contexts = _lastContextProperties;
			if (_fullContextTable == null)
			{
				return;
			}
			bool changeContext;
			Tuple<ContextPropertiesItem, IContextProperties> contextProperties = _fullContextTable.GetContextProperties(punitId, _propertiesFactory, ref _lastCtxInfoItem, out changeContext);
			if (changeContext)
			{
				if (contextProperties == null)
				{
					result.Properties.Contexts = null;
				}
				else if (!_file.Skeleton.SubContentPUs.Contains(punitId))
				{
					_lastCtxInfoItem = contextProperties.Item1;
					_lastContextProperties = contextProperties.Item2;
					result.Properties.Contexts = _lastContextProperties;
				}
				else
				{
					result.Properties.Contexts = contextProperties.Item2;
				}
			}
		}

		private void SetParagraphUnitContexts(Sdl.Core.Bcm.BcmModel.ParagraphUnit bcmParagraphUnit, IParagraphUnit result)
		{
			if (result.Properties.Contexts == null)
			{
				result.Properties.Contexts = new ContextProperties();
			}
			IList<IContextInfo> contexts = result.Properties.Contexts.Contexts;
			if (bcmParagraphUnit.ContextList != null)
			{
				foreach (int context in bcmParagraphUnit.ContextList)
				{
					Context byId = _file.Skeleton.Contexts.GetById(context);
					ContextDefinition byId2 = _file.Skeleton.ContextDefinitions.GetById(byId.ContextDefinitionId);
					contexts.Add(Convert(byId2));
				}
			}
			if (bcmParagraphUnit.StructureContextId > 0)
			{
				IStructureInfo structureInfo = GetStructureInfo(_file, bcmParagraphUnit.StructureContextId);
				result.Properties.Contexts.StructureInfo = structureInfo;
			}
		}

		private IContextInfo Convert(ContextDefinition bcmContext)
		{
			PropertiesFactory propertiesFactory = new PropertiesFactory();
			IContextInfo contextInfo = propertiesFactory.CreateContextInfo(bcmContext.TypeId);
			contextInfo.Description = bcmContext.Description;
			contextInfo.DisplayCode = bcmContext.DisplayCode;
			contextInfo.DisplayName = bcmContext.DisplayName;
			contextInfo.DisplayColor = bcmContext.DisplayColor.Convert();
			FormattingGroupConverter formattingGroupConverter = new FormattingGroupConverter(new FormattingItemFactory(), _file.Skeleton);
			IFormattingGroup formattingGroup2 = contextInfo.DefaultFormatting = formattingGroupConverter.Convert(bcmContext.FormattingGroupId);
			contextInfo.CopyMetadataFrom(bcmContext.Metadata);
			return contextInfo;
		}

		private IStructureInfo GetStructureInfo(Sdl.Core.Bcm.BcmModel.File file, int contextDefinitionId)
		{
			Context referenceId = file?.Skeleton?.Contexts.FirstOrDefault((Context c) => c.Id == contextDefinitionId);
			if (referenceId == null)
			{
				return null;
			}
			ContextDefinition bcmContext = _file.Skeleton.ContextDefinitions.First((ContextDefinition c) => c.Id == referenceId.ContextDefinitionId);
			IStructureInfo structureInfo = new StructureInfo
			{
				ContextInfo = Convert(bcmContext)
			};
			IStructureInfo structureInfo2 = structureInfo;
			for (referenceId = file.Skeleton?.Contexts.FirstOrDefault((Context c) => c.Id == referenceId.ParentContextId); referenceId != null; referenceId = file.Skeleton?.Contexts.FirstOrDefault((Context c) => c.Id == referenceId.ParentContextId))
			{
				bcmContext = _file.Skeleton.ContextDefinitions.First((ContextDefinition c) => c.Id == referenceId.ContextDefinitionId);
				StructureInfo structureInfo4 = (StructureInfo)(structureInfo2.ParentStructure = new StructureInfo
				{
					ContextInfo = Convert(bcmContext)
				});
				structureInfo2 = structureInfo4;
			}
			return structureInfo;
		}

		private ICommentProperties Convert(IEnumerable<int> commentDefinitionIds)
		{
			if (commentDefinitionIds == null)
			{
				return null;
			}
			ICommentProperties commentProperties = _propertiesFactory.CreateCommentProperties();
			foreach (int commentDefinitionId in commentDefinitionIds)
			{
				CommentDefinition commentDefinition = _skeleton.CommentDefinitions[SkeletonCollectionKey.From(commentDefinitionId)];
				IComment comment = _propertiesFactory.CreateComment(commentDefinition.Text, commentDefinition.Author, commentDefinition.CommentSeverity.Convert());
				comment.Date = commentDefinition.Date;
				comment.CopyMetadataFrom(commentDefinition.Metadata);
				commentProperties.Add(comment);
			}
			return commentProperties;
		}
	}
}
