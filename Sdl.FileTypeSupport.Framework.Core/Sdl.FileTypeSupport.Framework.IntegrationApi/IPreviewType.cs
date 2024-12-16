namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public interface IPreviewType
	{
		GeneratorId? SourceGeneratorId
		{
			get;
			set;
		}

		GeneratorId TargetGeneratorId
		{
			get;
			set;
		}
	}
}
