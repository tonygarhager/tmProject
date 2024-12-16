namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public abstract class AbstractFileTypeDefinitionComponent : IFileTypeDefinitionAware
	{
		private IFileTypeDefinition _fileTypeDefinition;

		public IFileTypeDefinition FileTypeDefinition
		{
			get
			{
				lock (this)
				{
					return _fileTypeDefinition;
				}
			}
			set
			{
				lock (this)
				{
					_fileTypeDefinition = value;
				}
			}
		}
	}
}
