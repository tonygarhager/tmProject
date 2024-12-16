using System;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public abstract class AbstractNativeFileParser : AbstractNativeFileTypeComponent, INativeFileParser, INativeFileTypeComponent, IParser, IDisposable
	{
		protected enum Stage
		{
			Initial,
			InProgress,
			Completed
		}

		private INativeExtractionContentHandler _Output;

		private Stage _CurrentStage;

		protected Stage CurrentStage
		{
			get
			{
				return _CurrentStage;
			}
			set
			{
				_CurrentStage = value;
			}
		}

		public virtual INativeExtractionContentHandler Output
		{
			get
			{
				return _Output;
			}
			set
			{
				_Output = value;
			}
		}

		public event EventHandler<ProgressEventArgs> Progress;

		public virtual void OnProgress(object sender, ProgressEventArgs args)
		{
			if (this.Progress != null)
			{
				this.Progress(sender, args);
			}
		}

		public void OnProgress(byte percent)
		{
			OnProgress(this, new ProgressEventArgs(percent));
		}

		public void OutputText(string text)
		{
			if (Output != null)
			{
				Output.Text(PropertiesFactory.CreateTextProperties(text));
			}
		}

		protected virtual void BeforeParsing()
		{
		}

		protected virtual bool DuringParsing()
		{
			return false;
		}

		protected virtual void AfterParsing()
		{
		}

		public virtual bool ParseNext()
		{
			switch (_CurrentStage)
			{
			case Stage.Initial:
				OnProgress(0);
				BeforeParsing();
				_CurrentStage = Stage.InProgress;
				goto case Stage.InProgress;
			case Stage.InProgress:
				if (DuringParsing())
				{
					break;
				}
				AfterParsing();
				OnProgress(100);
				_CurrentStage = Stage.Completed;
				goto case Stage.Completed;
			case Stage.Completed:
				return false;
			}
			return true;
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
		}

		~AbstractNativeFileParser()
		{
			Dispose(disposing: false);
		}
	}
}
