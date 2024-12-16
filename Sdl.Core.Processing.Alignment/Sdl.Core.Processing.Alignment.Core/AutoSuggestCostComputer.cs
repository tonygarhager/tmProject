using Sdl.Core.LanguageProcessing.AutoSuggest;
using Sdl.Core.Processing.Alignment.Core.CostComputers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Sdl.Core.Processing.Alignment.Core
{
	internal class AutoSuggestCostComputer : IAlignmentCostComputer
	{
		private IAutoSuggestDictionaryAccessor _accessor;

		public AutoSuggestCostComputer(string asdFileName, CultureInfo sourceCulture, CultureInfo targetCulture)
		{
			if (string.IsNullOrEmpty(asdFileName))
			{
				throw new ArgumentNullException("asdFileName must not be null or empty", "asdFileName");
			}
			if (!File.Exists(asdFileName))
			{
				throw new ArgumentException("File " + asdFileName + " does not exist");
			}
			_accessor = AutoSuggestDictionaryAccessorFactory.Create(new Uri(asdFileName), sourceCulture, targetCulture);
		}

		public AlignmentCost GetAlignmentCost(IEnumerable<string> leftStrings, IEnumerable<string> rightStrings)
		{
			if (leftStrings == null)
			{
				throw new ArgumentNullException("leftStrings");
			}
			if (rightStrings == null)
			{
				throw new ArgumentNullException("rightStrings");
			}
			string sourceText = Join(leftStrings, _accessor.SourceCulture);
			string targetText = Join(rightStrings, _accessor.TargetCulture);
			int droppedPhrasePairs;
			PhraseMappingPairs commonPhrases = _accessor.GetCommonPhrases(sourceText, targetText, out droppedPhrasePairs);
			if (commonPhrases.Count + droppedPhrasePairs == 0)
			{
				return new AlignmentCost(1.0);
			}
			return new AlignmentCost(1.0 - (double)commonPhrases.Count / (double)(commonPhrases.Count + droppedPhrasePairs));
		}

		private static string Join(IEnumerable<string> strings, CultureInfo culture)
		{
			return string.Join(" ", strings.ToArray());
		}

		public AlignmentCost GetAlignmentCost(IEnumerable<AlignmentElement> leftElements, IEnumerable<AlignmentElement> rightElements)
		{
			if (leftElements == null)
			{
				throw new ArgumentNullException("leftElements");
			}
			if (rightElements == null)
			{
				throw new ArgumentNullException("rightElements");
			}
			IEnumerable<string> leftStrings = leftElements.Select((AlignmentElement leftElement) => leftElement.TextContent);
			IEnumerable<string> rightStrings = rightElements.Select((AlignmentElement rightElement) => rightElement.TextContent);
			return GetAlignmentCost(leftStrings, rightStrings);
		}
	}
}
