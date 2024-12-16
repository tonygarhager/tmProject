using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Collections.Generic;

namespace Sdl.Core.Processing.Alignment
{
	internal class DocumentStructureNode
	{
		public DocumentStructureNode Parent
		{
			get;
			private set;
		}

		public IList<DocumentStructureNode> Children
		{
			get;
			private set;
		}

		public string Id
		{
			get;
			private set;
		}

		public IStructureInfo StructureInfo
		{
			get;
			private set;
		}

		public DocumentStructureNode(string rootNodeId)
		{
			Children = new List<DocumentStructureNode>();
			Parent = null;
			Id = rootNodeId;
			StructureInfo = null;
		}

		public DocumentStructureNode(DocumentStructureNode parent, IStructureInfo structureInfo)
		{
			Children = new List<DocumentStructureNode>();
			Parent = parent;
			if (Parent != null)
			{
				Parent.Children.Add(this);
			}
			Id = structureInfo.Id;
			StructureInfo = structureInfo;
		}
	}
}
