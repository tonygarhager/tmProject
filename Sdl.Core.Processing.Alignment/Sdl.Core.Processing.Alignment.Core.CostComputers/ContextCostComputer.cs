using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sdl.Core.Processing.Alignment.Core.CostComputers
{
	internal class ContextCostComputer : IAlignmentCostComputer
	{
		private readonly Regex _pathChecker;

		public ContextCostComputer(char pathDelimeter, char structureElementMarker)
		{
			string text = (pathDelimeter != '\\') ? pathDelimeter.ToString() : "\\\\";
			_pathChecker = new Regex("^[" + structureElementMarker.ToString() + "]?((?<ElementName>[^\\n\\r" + text + "]+?)[" + text + "](?<Order>[0-9]+?)[" + text + "])+$", RegexOptions.Compiled);
		}

		public AlignmentCost GetAlignmentCost(IEnumerable<AlignmentElement> sourceElements, IEnumerable<AlignmentElement> targetElements)
		{
			if (sourceElements.Count() == 0 && targetElements.Count() == 0)
			{
				return AlignmentCost.MinValue;
			}
			if (sourceElements.Count() != 0 && targetElements.Count() != 0)
			{
				var source = from alignmentElement in sourceElements.Union(targetElements)
					let match = _pathChecker.Match(alignmentElement.DocumentStructurePath)
					let elementCaptures = match.Groups["ElementName"].Captures
					let orderElements = match.Groups["Order"].Captures
					select new
					{
						Path = alignmentElement.DocumentStructurePath,
						LastElement = ((elementCaptures.Count > 0) ? elementCaptures[elementCaptures.Count - 1].Value : string.Empty),
						Order = ((orderElements.Count > 0) ? int.Parse(orderElements[orderElements.Count - 1].Value) : 0)
					};
				string elem = source.First().LastElement;
				int order = source.First().Order;
				bool firstUsefulPath = IsUsefulPath(source.First().Path);
				if (!source.All(x => firstUsefulPath == IsUsefulPath(x.Path) && elem == x.LastElement && order == x.Order))
				{
					return AlignmentCost.MaxValue;
				}
				return AlignmentCost.MinValue;
			}
			return AlignmentCost.MaxValue;
		}

		private bool IsUsefulPath(string documentStructurePath)
		{
			return _pathChecker.IsMatch(documentStructurePath);
		}
	}
}
