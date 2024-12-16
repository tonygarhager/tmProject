namespace Sdl.FileTypeSupport.Framework.BilingualApi
{
	public interface IBilingualContentHandler
	{
		void Initialize(IDocumentProperties documentInfo);

		void Complete();

		void SetFileProperties(IFileProperties fileInfo);

		void FileComplete();

		void ProcessParagraphUnit(IParagraphUnit paragraphUnit);
	}
}
