using Sdl.Core.Bcm.BcmConverters.Common;
using Sdl.Core.Bcm.BcmConverters.FromBilingualApi.Extensions;
using Sdl.Core.Bcm.BcmConverters.FromBilingualApi.Helpers;
using Sdl.Core.Bcm.BcmConverters.ToBilingualApi.Extensions;
using Sdl.Core.Bcm.BcmModel.Skeleton;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.Bcm.BcmConverters.FromBilingualApi
{
	internal sealed class ContextHierarchy
	{
		private readonly FileSkeleton _fileSkeleton;

		private readonly ContextTable _fullContextTable;

		private IContextProperties _localCtxProps;

		internal bool HasFullContexts => _fullContextTable.ContextMapping.Any();

		internal ContextHierarchy(FileSkeleton fileSkeleton)
		{
			_fileSkeleton = fileSkeleton;
			_fullContextTable = new ContextTable();
		}

		internal Tuple<int, IEnumerable<int>> AddContextProperties(string paragraphUnitId, IContextProperties contextProperties, bool generateContextsFile)
		{
			if (generateContextsFile)
			{
				_fullContextTable.AddContextProperties(paragraphUnitId, contextProperties);
			}
			if (contextProperties == null)
			{
				return new Tuple<int, IEnumerable<int>>(-1, Enumerable.Empty<int>());
			}
			_localCtxProps = (contextProperties.Clone() as IContextProperties);
			int item = AddStructureInfoContexts(_localCtxProps.StructureInfo);
			IEnumerable<int> item2 = AddNonStructureContextsList(_localCtxProps.Contexts);
			return new Tuple<int, IEnumerable<int>>(item, item2);
		}

		private int AddStructureInfoContexts(IStructureInfo structureInfo)
		{
			List<IContextInfo> list = new List<IContextInfo>();
			while (structureInfo != null)
			{
				list.Add(structureInfo.ContextInfo);
				structureInfo = structureInfo.ParentStructure;
			}
			int num = list.Count - 1;
			int num2 = 0;
			while (num >= 0)
			{
				Context context = AddContextInfoToSkeleton(list[num], num2);
				num2 = context.Id;
				RemoveFromContextList(list[num]);
				num--;
			}
			if (num2 != 0)
			{
				return num2;
			}
			return -1;
		}

		private void RemoveFromContextList(IContextInfo ctxInfo)
		{
			IContextInfo contextInfo = _localCtxProps.Contexts.FirstOrDefault((IContextInfo x) => x.ContextType == ctxInfo.ContextType && x.Description == ctxInfo.Description && x.DisplayCode == ctxInfo.DisplayCode && x.DisplayColor == ctxInfo.DisplayColor && x.Purpose == ctxInfo.Purpose);
			if (contextInfo != null)
			{
				_localCtxProps.Contexts.Remove(contextInfo);
			}
		}

		private IEnumerable<int> AddNonStructureContextsList(IEnumerable<IContextInfo> contexts)
		{
			List<int> list = new List<int>();
			if (contexts == null)
			{
				return list;
			}
			foreach (IContextInfo item in contexts.Where(IsContextDisplayable))
			{
				ContextDefinition orCreateContextDefinition = GetOrCreateContextDefinition(item, isStructureContext: false);
				Context orCreateContext = GetOrCreateContext(orCreateContextDefinition.Id, 0);
				list.Add(orCreateContext.Id);
			}
			if (list.Count != 0)
			{
				return list;
			}
			return null;
		}

		private static bool IsContextDisplayable(IContextInfo contextInfo)
		{
			if (contextInfo.Purpose != 0 && contextInfo.Purpose != ContextPurpose.Match)
			{
				return contextInfo.Purpose == ContextPurpose.Location;
			}
			return true;
		}

		private Context AddContextInfoToSkeleton(IContextInfo contextInfo, int currentParentContextId)
		{
			ContextDefinition orCreateContextDefinition = GetOrCreateContextDefinition(contextInfo, isStructureContext: true);
			return GetOrCreateContext(orCreateContextDefinition.Id, currentParentContextId);
		}

		private Context GetOrCreateContext(int contextDefId, int currentParentContextId)
		{
			Context elem = new Context
			{
				ContextDefinitionId = contextDefId,
				ParentContextId = currentParentContextId
			};
			return _fileSkeleton.Contexts.GetOrAdd(elem);
		}

		private ContextDefinition GetOrCreateContextDefinition(IContextInfo contextInfo, bool isStructureContext)
		{
			ContextDefinition contextDefinition = new ContextDefinition
			{
				Description = contextInfo.Description,
				DisplayCode = contextInfo.DisplayCode,
				DisplayColor = contextInfo.DisplayColor.ToRgbColorString(),
				DisplayName = contextInfo.DisplayName,
				IsStructureContext = isStructureContext,
				IsTmContext = (contextInfo.Purpose == ContextPurpose.Match),
				TypeId = contextInfo.ContextType
			};
			if (contextInfo.DefaultFormatting != null && contextInfo.DefaultFormatting.Any())
			{
				contextDefinition.FormattingGroupId = FormattingGroupHelper.AddFormatting(_fileSkeleton, contextInfo.DefaultFormatting);
			}
			contextDefinition.CopyMetadataFrom(contextInfo);
			return _fileSkeleton.ContextDefinitions.GetOrAdd(contextDefinition);
		}

		internal string SerializeFullContextsToFile()
		{
			return _fullContextTable.SerializeFullContextsToFile();
		}
	}
}
