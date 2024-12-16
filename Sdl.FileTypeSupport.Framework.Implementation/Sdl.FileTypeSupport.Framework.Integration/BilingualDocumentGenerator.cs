using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.Native;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.Integration
{
	public class BilingualDocumentGenerator : AbstractBilingualProcessorContainer, IBilingualDocumentGenerator, IBilingualProcessorContainer, IAbstractGenerator, IFileTypeDefinitionAware
	{
		private List<IFilePostTweaker> _FileTweakers = new List<IFilePostTweaker>();

		private INativeOutputFileProperties _nativeOutputProperties = new NativeOutputFileProperties();

		public IBilingualContentHandler Input => this;

		public IBilingualDocumentWriter Writer
		{
			get
			{
				return Output as IBilingualDocumentWriter;
			}
			set
			{
				Output = value;
			}
		}

		public IEnumerable<object> AllComponents
		{
			get
			{
				yield return GetBilingualProcessors();
				if (Writer != null)
				{
					yield return Writer;
				}
			}
		}

		public IEnumerable<IFilePostTweaker> FileTweakers => _FileTweakers;

		public event EventHandler<MessageEventArgs> Message;

		public BilingualDocumentGenerator()
		{
		}

		public BilingualDocumentGenerator(IBilingualDocumentFileWriter writer)
		{
			Writer = writer;
		}

		public void AddFileTweaker(IFilePostTweaker tweaker)
		{
			_FileTweakers.Add(tweaker);
		}

		public void RemoveFileTweaker(IFilePostTweaker tweaker)
		{
			_FileTweakers.Remove(tweaker);
		}

		public override void FileComplete()
		{
			base.FileComplete();
			RunPostWritingOperations();
		}

		public virtual void OnMessage(object sender, MessageEventArgs args)
		{
			if (this.Message != null)
			{
				if (args != null && string.IsNullOrEmpty(args.FilePath))
				{
					args.FilePath = _nativeOutputProperties.OutputFilePath;
				}
				this.Message(sender, args);
			}
		}

		private void RunPostWritingOperations()
		{
			PostGenerationMessageReporterAdapter messageReporter = new PostGenerationMessageReporterAdapter(OnMessage, null, null);
			foreach (IFilePostTweaker fileTweaker in FileTweakers)
			{
				if (fileTweaker.Enabled)
				{
					fileTweaker.MessageReporter = messageReporter;
					fileTweaker.TweakFilePostWriting(_nativeOutputProperties);
					fileTweaker.MessageReporter = null;
				}
			}
		}
	}
}
