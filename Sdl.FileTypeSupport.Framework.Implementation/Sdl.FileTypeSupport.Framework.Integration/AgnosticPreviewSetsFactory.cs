using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;

namespace Sdl.FileTypeSupport.Framework.Integration
{
	public class AgnosticPreviewSetsFactory : AbstractFileTypeDefinitionComponent, IPreviewSetsFactory, IFileTypeDefinitionAware
	{
		private IPreviewSets _previewSets;

		public IPreviewSets PreviewSets
		{
			get
			{
				return _previewSets;
			}
			set
			{
				_previewSets = value;
			}
		}

		public bool IsFileAgnostic => true;

		public AgnosticPreviewSetsFactory()
		{
			_previewSets = new PreviewSets();
		}

		public AgnosticPreviewSetsFactory(IPreviewSets previewSets)
		{
			_previewSets = previewSets;
		}

		public IPreviewSets GetPreviewSets(IFileProperties fileProperties)
		{
			return _previewSets;
		}

		public IPreviewSet CreatePreviewSet()
		{
			return new PreviewSet();
		}

		public IPreviewType CreatePreviewType<T>()
		{
			if (typeof(T) == typeof(IApplicationPreviewType))
			{
				return new ApplicationPreviewType();
			}
			return new ControlPreviewType();
		}
	}
}
