using Sdl.FileTypeSupport.Framework.IntegrationApi;

namespace Sdl.FileTypeSupport.Framework.Integration
{
	public class PreviewType : IPreviewType
	{
		private GeneratorId? _sourceGeneratorId;

		private GeneratorId _targetGeneratorId;

		public GeneratorId? SourceGeneratorId
		{
			get
			{
				return _sourceGeneratorId;
			}
			set
			{
				_sourceGeneratorId = value;
			}
		}

		public GeneratorId TargetGeneratorId
		{
			get
			{
				return _targetGeneratorId;
			}
			set
			{
				_targetGeneratorId = value;
			}
		}
	}
}
