using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sdl.Core.Processing.Alignment.Core.CostComputers
{
	internal class DocumentStructureCostComputer : IAlignmentCostComputer
	{
		private readonly Regex _pathChecker;

		public DocumentStructureCostComputer(char pathDelimiter, char structureElementMarker)
		{
			string text = (pathDelimiter != '\\') ? pathDelimiter.ToString() : "\\\\";
			_pathChecker = new Regex("^[" + structureElementMarker.ToString() + "]((?<ElementName>[^\\n\\r" + text + "]+?)[" + text + "](?<Order>[0-9]+?)[" + text + "])+$", RegexOptions.Compiled);
		}

		public AlignmentCost GetAlignmentCost(IEnumerable<AlignmentElement> sourceElements, IEnumerable<AlignmentElement> targetElements)
		{
			if (sourceElements.Count() == 1 || targetElements.Count() == 1)
			{
				string documentStructurePath = sourceElements.First().DocumentStructurePath;
				string documentStructurePath2 = targetElements.First().DocumentStructurePath;
				if (IsUsefulPath(documentStructurePath) && IsUsefulPath(documentStructurePath2))
				{
					int order;
					string element = GetElement(documentStructurePath, out order);
					int order2;
					string element2 = GetElement(documentStructurePath2, out order2);
					if (!(element == element2) || order != order2)
					{
						return AlignmentCost.MaxValue;
					}
					return AlignmentCost.MinValue;
				}
			}
			return AlignmentCost.MaxValue;
		}

		private string GetElement(string path, out int order)
		{
			Match match = _pathChecker.Match(path);
			string value = match.Groups["ElementName"].Captures[0].Value;
			order = int.Parse(match.Groups["Order"].Captures[0].Value);
			return value;
		}

		private bool IsUsefulPath(string documentStructurePath)
		{
			return _pathChecker.IsMatch(documentStructurePath);
		}
	}
}
