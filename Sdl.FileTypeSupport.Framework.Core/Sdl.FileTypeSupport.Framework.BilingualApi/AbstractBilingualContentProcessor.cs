namespace Sdl.FileTypeSupport.Framework.BilingualApi
{
	public abstract class AbstractBilingualContentProcessor : AbstractBilingualContentHandler, IBilingualContentProcessor, IBilingualContentHandler
	{
		private IBilingualContentHandler _output;

		public virtual IBilingualContentHandler Output
		{
			get
			{
				return _output;
			}
			set
			{
				_output = value;
			}
		}

		public override void Initialize(IDocumentProperties documentInfo)
		{
			base.Initialize(documentInfo);
			if (Output != null)
			{
				Output.Initialize(documentInfo);
			}
		}

		public override void SetFileProperties(IFileProperties fileInfo)
		{
			base.SetFileProperties(fileInfo);
			if (Output != null)
			{
				Output.SetFileProperties(fileInfo);
			}
		}

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			base.ProcessParagraphUnit(paragraphUnit);
			if (Output != null)
			{
				Output.ProcessParagraphUnit(paragraphUnit);
			}
		}

		public override void FileComplete()
		{
			base.FileComplete();
			if (Output != null)
			{
				Output.FileComplete();
			}
		}

		public override void Complete()
		{
			base.Complete();
			if (Output != null)
			{
				Output.Complete();
			}
		}
	}
}
