using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Core.Processing.Processors.Storage
{
	public class ParagraphUnitExtractor : AbstractBilingualContentProcessor
	{
		public ParagraphUnitStore ParagraphUnits
		{
			get;
		} = new ParagraphUnitStore();


		public IFileProperties FileProperties
		{
			get;
			set;
		}

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			ParagraphUnits.Add(paragraphUnit);
			base.ProcessParagraphUnit(paragraphUnit);
		}

		public override void SetFileProperties(IFileProperties fileInfo)
		{
			base.SetFileProperties(fileInfo);
			FileProperties = fileInfo;
		}
	}
}
