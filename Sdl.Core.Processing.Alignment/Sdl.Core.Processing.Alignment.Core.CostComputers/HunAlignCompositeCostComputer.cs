using Sdl.Core.Processing.Alignment.Common;
using Sdl.LanguagePlatform.Core.Resources;
using System.Collections.Generic;

namespace Sdl.Core.Processing.Alignment.Core.CostComputers
{
	internal class HunAlignCompositeCostComputer : CompositeCostComputer
	{
		public HunAlignCompositeCostComputer(IBilingualDictionary bilingualDictionary)
			: this(bilingualDictionary, null)
		{
		}

		public HunAlignCompositeCostComputer(IBilingualDictionary bilingualDictionary, IResourceDataAccessor resourceDataAccessor)
			: base(new List<CompositeCostComponent>
			{
				new CompositeCostComponent(new HunAlignWordCostComputer(bilingualDictionary, resourceDataAccessor), 5.0),
				new CompositeCostComponent(new HunAlignLengthCostComputer(), 5.0),
				new CompositeCostComponent(new HunAlignAlignmentTypeCostComputer(), 8.0)
			})
		{
		}
	}
}
