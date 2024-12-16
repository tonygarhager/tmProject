namespace Sdl.Core.Processing.Alignment.Core
{
	internal class AlignmentArrayElement
	{
		public AlignmentCost ElementCost
		{
			get;
			set;
		}

		public double TotalCost
		{
			get;
			set;
		}

		public AlignmentOperation Operation
		{
			get;
			set;
		}

		public int ExpansionContractionFactor
		{
			get;
			set;
		}

		public AlignmentArrayElement(AlignmentCost elementCost, double totalCost, AlignmentOperation operation, int expansionContractionFactor = 2)
		{
			ElementCost = elementCost;
			TotalCost = totalCost;
			Operation = operation;
			ExpansionContractionFactor = expansionContractionFactor;
		}

		public override string ToString()
		{
			return $"{Operation.ToString()} elementCost={ElementCost.ToString()} total={TotalCost}";
		}
	}
}
