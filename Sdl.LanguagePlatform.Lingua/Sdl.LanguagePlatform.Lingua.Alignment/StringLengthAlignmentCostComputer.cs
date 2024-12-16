namespace Sdl.LanguagePlatform.Lingua.Alignment
{
	public class StringLengthAlignmentCostComputer : IAlignmentCostComputer<string>
	{
		private readonly LengthAlignmentCostComputer _costComputer;

		public StringLengthAlignmentCostComputer(double expansionFactor)
		{
			_costComputer = new LengthAlignmentCostComputer(expansionFactor);
		}

		public int GetSubstitutionCosts(string s, string t)
		{
			return _costComputer.GetSubstitutionCosts(s.Length, t.Length);
		}

		public int GetDeletionCosts(string s)
		{
			return _costComputer.GetDeletionCosts(s.Length);
		}

		public int GetInsertionCosts(string t)
		{
			return _costComputer.GetInsertionCosts(t.Length);
		}

		public int GetContractionCosts(string s1, string s2, string t)
		{
			return _costComputer.GetContractionCosts(s1.Length, s2.Length, t.Length);
		}

		public int GetExpansionCosts(string s, string t1, string t2)
		{
			return _costComputer.GetExpansionCosts(s.Length, t1.Length, t2.Length);
		}

		public int GetMeldingCosts(string s1, string s2, string t1, string t2)
		{
			return _costComputer.GetMeldingCosts(s1.Length, s2.Length, t1.Length, t2.Length);
		}
	}
}
