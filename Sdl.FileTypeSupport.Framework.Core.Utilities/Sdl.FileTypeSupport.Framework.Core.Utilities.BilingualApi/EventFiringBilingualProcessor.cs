using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi
{
	public class EventFiringBilingualProcessor : AbstractBilingualContentProcessor
	{
		public event InitializeHandler BeforeInitialize;

		public event InitializeHandler AfterInitialize;

		public event CompleteHandler BeforeComplete;

		public event CompleteHandler AfterComplete;

		public event SetFilePropertiesHandler BeforeSetFileProperties;

		public event SetFilePropertiesHandler AfterSetFileProperties;

		public event FileCompleteHandler BeforeFileComplete;

		public event FileCompleteHandler AfterFileComplete;

		public event ProcessParagraphUnitHandler BeforeProcessParagraphUnit;

		public event ProcessParagraphUnitHandler AfterProcessParagraphUnit;

		public override void Initialize(IDocumentProperties documentInfo)
		{
			if (this.BeforeInitialize != null)
			{
				this.BeforeInitialize(documentInfo);
			}
			base.Initialize(documentInfo);
			if (this.AfterInitialize != null)
			{
				this.AfterInitialize(documentInfo);
			}
		}

		public override void Complete()
		{
			if (this.BeforeComplete != null)
			{
				this.BeforeComplete();
			}
			base.Complete();
			if (this.AfterComplete != null)
			{
				this.AfterComplete();
			}
		}

		public override void SetFileProperties(IFileProperties fileInfo)
		{
			if (this.BeforeSetFileProperties != null)
			{
				this.BeforeSetFileProperties(fileInfo);
			}
			base.SetFileProperties(fileInfo);
			if (this.AfterSetFileProperties != null)
			{
				this.AfterSetFileProperties(fileInfo);
			}
		}

		public override void FileComplete()
		{
			if (this.BeforeFileComplete != null)
			{
				this.BeforeFileComplete();
			}
			base.FileComplete();
			if (this.AfterFileComplete != null)
			{
				this.AfterFileComplete();
			}
		}

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			if (this.BeforeProcessParagraphUnit != null)
			{
				this.BeforeProcessParagraphUnit(paragraphUnit);
			}
			base.ProcessParagraphUnit(paragraphUnit);
			if (this.AfterProcessParagraphUnit != null)
			{
				this.AfterProcessParagraphUnit(paragraphUnit);
			}
		}
	}
}
