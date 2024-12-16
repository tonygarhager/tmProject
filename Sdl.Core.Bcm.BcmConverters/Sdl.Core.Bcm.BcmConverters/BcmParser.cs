using Sdl.Core.Bcm.BcmConverters.ToBilingualApi;
using Sdl.Core.Bcm.BcmModel;
using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.Core.Bcm.BcmConverters
{
	public class BcmParser : IBilingualParser, IParser, IDisposable, IBilingualFileTypeComponent, INativeContentCycleAware
	{
		private readonly Document _document;

		private File _currentFile;

		public IDocumentItemFactory ItemFactory
		{
			get;
			set;
		}

		public IBilingualContentMessageReporter MessageReporter
		{
			get;
			set;
		}

		public IDocumentProperties DocumentProperties
		{
			get;
			set;
		}

		public IBilingualContentHandler Output
		{
			get;
			set;
		}

		public event EventHandler<ProgressEventArgs> Progress;

		public BcmParser(Document document)
		{
			_document = document;
		}

		public bool ParseNext()
		{
			Output.Initialize(DocumentProperties);
			foreach (File file2 in _document.Files)
			{
				File file = _currentFile = file2;
				FileProperties properties = BcmReaderUtil.CreateFileProperties(file, DocumentProperties);
				BcmReaderUtil.SetProperties(_currentFile, Output, DocumentProperties, properties);
				BcmToBilingualConverter bcmToBilingualConverter = new BcmToBilingualConverter(file, null);
				foreach (Sdl.Core.Bcm.BcmModel.ParagraphUnit paragraphUnit2 in file.ParagraphUnits)
				{
					IParagraphUnit paragraphUnit = bcmToBilingualConverter.Convert(paragraphUnit2);
					Output.ProcessParagraphUnit(paragraphUnit);
				}
			}
			return false;
		}

		public void SetFileProperties(IFileProperties properties)
		{
		}

		public void Dispose()
		{
		}

		public void StartOfInput()
		{
		}

		public void EndOfInput()
		{
			Output.FileComplete();
			Output.Complete();
		}

		protected virtual void OnProgress(ProgressEventArgs e)
		{
			this.Progress?.Invoke(this, e);
		}
	}
}
