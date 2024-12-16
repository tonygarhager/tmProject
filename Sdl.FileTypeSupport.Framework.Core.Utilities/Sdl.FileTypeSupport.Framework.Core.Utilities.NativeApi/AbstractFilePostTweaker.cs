using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi
{
	public abstract class AbstractFilePostTweaker : IFilePostTweaker, IFileTweaker
	{
		private INativeTextLocationMessageReporter _messageReporter;

		private bool _enabled;

		private bool _requireValidEncoding;

		public bool RequireValidEncoding
		{
			get
			{
				return _requireValidEncoding;
			}
			set
			{
				_requireValidEncoding = value;
			}
		}

		protected virtual string Name => "File Post Tweaker";

		public INativeTextLocationMessageReporter MessageReporter
		{
			get
			{
				return _messageReporter;
			}
			set
			{
				_messageReporter = value;
			}
		}

		public bool Enabled
		{
			get
			{
				return _enabled;
			}
			set
			{
				_enabled = value;
			}
		}

		protected AbstractFilePostTweaker()
		{
			Enabled = true;
			RequireValidEncoding = true;
		}

		protected virtual void Tweak(INativeOutputFileProperties outputFileProperties)
		{
		}

		public virtual void TweakFilePostWriting(INativeOutputFileProperties outputFileProperties)
		{
			bool flag = false;
			if (RequireValidEncoding && (outputFileProperties.Encoding == null || !outputFileProperties.Encoding.IsValid))
			{
				string message = "Unable to tweak file (" + outputFileProperties.OutputFilePath + ") after parsing. Unknown Encoding";
				if (MessageReporter == null)
				{
					throw new Exception(message);
				}
				MessageReporter.ReportMessage(this, Name, ErrorLevel.Error, message, "");
				flag = true;
			}
			if (!flag)
			{
				Tweak(outputFileProperties);
			}
		}
	}
}
