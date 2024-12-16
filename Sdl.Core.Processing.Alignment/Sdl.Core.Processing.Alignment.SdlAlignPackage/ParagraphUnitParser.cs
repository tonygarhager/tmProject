using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using System;
using System.Globalization;

namespace Sdl.Core.Processing.Alignment.SdlAlignPackage
{
	internal class ParagraphUnitParser : AbstractBilingualParser
	{
		private IParagraphUnit _paragraphUnit;

		private readonly CultureInfo _sourceLanguage;

		private readonly CultureInfo _targetLanguage;

		internal ParagraphUnitParser(CultureInfo sourceLanguage, CultureInfo targetLanguage)
		{
			_sourceLanguage = sourceLanguage;
			_targetLanguage = targetLanguage;
		}

		public override bool ParseNext()
		{
			InitializeOutput();
			base.Output.ProcessParagraphUnit(_paragraphUnit);
			CompleteOutput();
			return false;
		}

		public void AddParagraphUnit(IParagraphUnit paragraphUnit)
		{
			_paragraphUnit = paragraphUnit;
		}

		private void InitializeOutput()
		{
			IFileExtractor fileExtractor = base.Output as IFileExtractor;
			if (fileExtractor == null)
			{
				throw new Exception("Output must implement IFileExtractor");
			}
			IDocumentProperties documentInfo = fileExtractor.DocumentInfo;
			documentInfo.SourceLanguage = new Language(_sourceLanguage);
			documentInfo.TargetLanguage = new Language(_targetLanguage);
			base.Output.Initialize(documentInfo);
			IFileProperties fileProperties = ItemFactory.CreateFileProperties();
			base.Output.SetFileProperties(fileProperties);
		}

		private void CompleteOutput()
		{
			base.Output.FileComplete();
			base.Output.Complete();
		}
	}
}
