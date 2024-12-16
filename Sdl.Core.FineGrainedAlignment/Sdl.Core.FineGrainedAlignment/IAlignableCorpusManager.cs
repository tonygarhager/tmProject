namespace Sdl.Core.FineGrainedAlignment
{
	public interface IAlignableCorpusManager
	{
		IAlignableCorpus GetAlignableCorpus(AlignableCorpusId id);
	}
}
