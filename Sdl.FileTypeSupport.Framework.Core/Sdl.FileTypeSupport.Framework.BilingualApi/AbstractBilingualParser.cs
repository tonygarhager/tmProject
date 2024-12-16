using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.FileTypeSupport.Framework.BilingualApi
{
	public abstract class AbstractBilingualParser : AbstractBilingualFileTypeComponent, IBilingualParser, IParser, IDisposable, IBilingualFileTypeComponent
	{
		private IDocumentProperties _documentProperties;

		private IBilingualContentHandler _output;

		private Predicate<IPersistentFileConversionProperties> _fileRestriction;

		public IDocumentProperties DocumentProperties
		{
			get
			{
				return _documentProperties;
			}
			set
			{
				_documentProperties = value;
			}
		}

		public IBilingualContentHandler Output
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

		public Predicate<IPersistentFileConversionProperties> FileRestriction
		{
			get
			{
				return _fileRestriction;
			}
			set
			{
				_fileRestriction = value;
			}
		}

		public event EventHandler<ProgressEventArgs> Progress;

		public abstract bool ParseNext();

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
		}

		~AbstractBilingualParser()
		{
			Dispose(disposing: false);
		}

		public virtual void OnProgress(byte percent)
		{
			if (this.Progress != null)
			{
				this.Progress(this, new ProgressEventArgs(percent));
			}
		}
	}
}
