namespace Sdl.FileTypeSupport.Framework.BilingualApi
{
	public abstract class AbstractBilingualContentHandler : AbstractBilingualFileTypeComponent, IBilingualContentHandler
	{
		public virtual void Initialize(IDocumentProperties documentInfo)
		{
		}

		public virtual void Complete()
		{
		}

		public virtual void SetFileProperties(IFileProperties fileInfo)
		{
		}

		public virtual void FileComplete()
		{
		}

		public virtual void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
		}
	}
}
