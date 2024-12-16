namespace Sdl.Core.FineGrainedAlignment
{
	public interface IAlignerBroker
	{
		IFineGrainedAligner GetAligner(AlignerDefinition definition);
	}
}
