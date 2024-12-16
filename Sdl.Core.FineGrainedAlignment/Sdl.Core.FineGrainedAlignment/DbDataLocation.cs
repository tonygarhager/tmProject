using Sdl.LanguagePlatform.Stat;

namespace Sdl.Core.FineGrainedAlignment
{
	internal class DbDataLocation : DataLocation2
	{
		private readonly IChiSquaredTranslationModelStore _store;

		public IChiSquaredTranslationModelStore Store => _store;

		public DbDataLocation(string workingDir, IChiSquaredTranslationModelStore store)
			: base(workingDir)
		{
			_store = store;
		}
	}
}
