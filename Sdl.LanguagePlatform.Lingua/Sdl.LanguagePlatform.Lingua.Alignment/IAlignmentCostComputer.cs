namespace Sdl.LanguagePlatform.Lingua.Alignment
{
	public interface IAlignmentCostComputer<in T>
	{
		int GetSubstitutionCosts(T s, T t);

		int GetDeletionCosts(T s);

		int GetInsertionCosts(T t);

		int GetContractionCosts(T s1, T s2, T t);

		int GetExpansionCosts(T s, T t1, T t2);

		int GetMeldingCosts(T s1, T s2, T t1, T t2);
	}
}
