using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sdl.Core.Processing.Alignment
{
	internal class DocumentStructureBuilder
	{
		public const int FirstSegmentMaxPathLength = 1;

		private readonly char _pathDelimeter;

		private readonly char _structureElementMarker;

		private readonly DocumentStructureNode _rootNode = new DocumentStructureNode("RootNode");

		internal readonly Dictionary<string, DocumentStructureNode> PathNodes = new Dictionary<string, DocumentStructureNode>();

		internal readonly Dictionary<ISegment, string> GeneratedPaths = new Dictionary<ISegment, string>();

		public DocumentStructureBuilder(char pathDelimeter, char structureElementMarker, IEnumerable<IParagraphUnit> paragraphUnits)
		{
			_pathDelimeter = pathDelimeter;
			_structureElementMarker = structureElementMarker;
			PathNodes.Add(_rootNode.Id, _rootNode);
			BuildDocumentTree(paragraphUnits);
		}

		public string GetStructurePath(ISegment segment)
		{
			List<DocumentStructureNode> list = new List<DocumentStructureNode>();
			if (GeneratedPaths.ContainsKey(segment))
			{
				return GeneratedPaths[segment];
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (segment.ParentParagraphUnit != null)
			{
				IContextProperties contexts = segment.ParentParagraphUnit.Properties.Contexts;
				if (contexts != null && contexts.StructureInfo != null && PathNodes.TryGetValue(contexts.StructureInfo.Id, out DocumentStructureNode value))
				{
					do
					{
						list.Add(value);
						value = value.Parent;
					}
					while (value != _rootNode);
					for (int num = list.Count - 1; num >= 0; num--)
					{
						stringBuilder.Append(list[num].StructureInfo.ContextInfo.DisplayName).Append(_pathDelimeter).Append(list[num].Parent.Children.IndexOf(list[num]))
							.Append(_pathDelimeter);
					}
					if (list.Count <= 1 && !GeneratedPaths.Values.Contains(stringBuilder.ToString()))
					{
						GeneratedPaths.Add(segment, stringBuilder.ToString());
						stringBuilder.Insert(0, _structureElementMarker);
					}
				}
			}
			return stringBuilder.ToString();
		}

		private void BuildDocumentTree(IEnumerable<IParagraphUnit> paragraphUnits)
		{
			foreach (IParagraphUnit paragraphUnit in paragraphUnits)
			{
				IContextProperties contexts = paragraphUnit.Properties.Contexts;
				if (contexts != null && contexts.StructureInfo != null)
				{
					CreateNodes(contexts.StructureInfo);
				}
			}
		}

		private void CreateNodes(IStructureInfo structureInfo)
		{
			if (structureInfo != null && !PathNodes.ContainsKey(structureInfo.Id))
			{
				if (structureInfo.ParentStructure != null)
				{
					CreateNodes(structureInfo.ParentStructure);
					PathNodes.Add(structureInfo.Id, new DocumentStructureNode(PathNodes[structureInfo.ParentStructure.Id], structureInfo));
				}
				else
				{
					PathNodes.Add(structureInfo.Id, new DocumentStructureNode(_rootNode, structureInfo));
				}
			}
		}
	}
}
