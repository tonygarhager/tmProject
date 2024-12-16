using Sdl.Core.Bcm.BcmConverters.ToBilingualApi.Extensions;
using Sdl.Core.Bcm.BcmModel;
using Sdl.Core.Bcm.BcmModel.Skeleton;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Core.Bcm.BcmConverters.ToBilingualApi.MarkupDataConverters
{
	internal class StructureTagConverter : MarkupDataConverter
	{
		private readonly IPropertiesFactory _propertiesFactory;

		private readonly IDocumentItemFactory _documentItemFactory;

		private readonly FileSkeleton _fileSkeleton;

		public StructureTagConverter(IPropertiesFactory propertiesFactory, IDocumentItemFactory documentItemFactory, FileSkeleton fileSkeleton)
		{
			_propertiesFactory = propertiesFactory;
			_documentItemFactory = documentItemFactory;
			_fileSkeleton = fileSkeleton;
		}

		internal override IAbstractMarkupData Convert(MarkupData source)
		{
			StructureTag structureTag = source as StructureTag;
			if (structureTag == null)
			{
				return null;
			}
			StructureTagDefinition definition = GetDefinition(structureTag.StructureTagDefinitionId);
			if (definition == null)
			{
				return null;
			}
			IStructureTagProperties structureTagProperties = SetTagProperties(definition);
			RestoreFrameworkOriginalTagId(structureTagProperties, source);
			structureTagProperties.CopyMetadataFrom(structureTag.Metadata);
			IStructureTag tag = _documentItemFactory.CreateStructureTag(structureTagProperties);
			tag = (SubcontentConverter.AddSubsegmentReferences(tag, definition.SubContent, base.ConverterFactory.ContextTable, _propertiesFactory) as IStructureTag);
			definition.SubContent?.ForEach(delegate(LocalizableSubContent x)
			{
				_fileSkeleton.SubContentPUs.Add(x.ParagraphUnitId);
			});
			return tag;
		}

		private IStructureTagProperties SetTagProperties(StructureTagDefinition structTagDef)
		{
			IStructureTagProperties structureTagProperties = _propertiesFactory.CreateStructureTagProperties(structTagDef.TagContent);
			structureTagProperties.DisplayText = structTagDef.DisplayText;
			SubcontentConverter.AddLocalizableSubContent(structTagDef.SubContent, structureTagProperties, base.ConverterFactory.ContextTable, _propertiesFactory);
			return structureTagProperties;
		}

		private static void RestoreFrameworkOriginalTagId(IAbstractTagProperties props, MetadataContainer sourceMetadata)
		{
			string metadata = sourceMetadata.GetMetadata("frameworkOriginalTagId");
			sourceMetadata.RemoveMetadata("frameworkOriginalTagId");
			props.TagId = new TagId(metadata);
		}

		private StructureTagDefinition GetDefinition(int id)
		{
			return _fileSkeleton.StructureTagDefinitions.GetById(id);
		}
	}
}
