using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;

namespace Sdl.FileTypeSupport.Framework.Integration
{
	public class AgnosticQuickTagsFactory : AbstractFileTypeDefinitionComponent, IQuickTagsFactory, IFileTypeDefinitionAware
	{
		private IQuickTags _quickTags;

		public IQuickTags QuickTags
		{
			get
			{
				return _quickTags;
			}
			set
			{
				_quickTags = value;
			}
		}

		public bool IsFileAgnostic => true;

		public AgnosticQuickTagsFactory()
		{
			_quickTags = new QuickTags();
		}

		public AgnosticQuickTagsFactory(IQuickTags quickTags)
		{
			_quickTags = quickTags;
		}

		public IQuickTags GetQuickTags(IFileProperties fileProperties)
		{
			return _quickTags;
		}
	}
}
