namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public interface ISubContentComponentBuilder
	{
		ISubContentExtractor BuildSubContentExtractor(string name);

		ISubContentGenerator BuildSubContentGenerator(string name);
	}
}
