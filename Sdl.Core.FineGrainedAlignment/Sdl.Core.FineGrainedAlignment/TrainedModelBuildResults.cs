using Sdl.LanguagePlatform.Stat;

namespace Sdl.Core.FineGrainedAlignment
{
	public class TrainedModelBuildResults
	{
		public SparseMatrix<double> TranslationMatrix;

		public SparseMatrix<double> ReversedTranslationMatrix;
	}
}
