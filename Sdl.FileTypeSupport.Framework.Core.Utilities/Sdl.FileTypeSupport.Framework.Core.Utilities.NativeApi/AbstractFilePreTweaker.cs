using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.IO;

namespace Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi
{
	public abstract class AbstractFilePreTweaker : IFilePreTweaker, IFileTweaker
	{
		private bool _enabled;

		private bool _requireValidEncoding;

		private INativeTextLocationMessageReporter _messageReporter;

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

		protected virtual string Name => "File Pre Tweaker";

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

		protected AbstractFilePreTweaker()
		{
			Enabled = true;
			RequireValidEncoding = false;
		}

		protected virtual void Tweak(IPersistentFileConversionProperties properties)
		{
		}

		protected virtual bool WillFileBeTweaked(string filePath)
		{
			return true;
		}

		public virtual void TweakFilePreParsing(IPersistentFileConversionProperties properties, IPropertiesFactory propertiesFactory)
		{
			try
			{
				if (WillFileBeTweaked(properties.OriginalFilePath))
				{
					bool flag = false;
					if (RequireValidEncoding && properties.FileSnifferInfo != null && properties.FileSnifferInfo.DetectedEncoding != null && properties.FileSnifferInfo.DetectedEncoding.Second == DetectionLevel.Unknown)
					{
						MessageReporter.ReportMessage(this, Name, ErrorLevel.Error, "Unable to tweak file (" + properties.OriginalFilePath + ") before parsing. Unknown Encoding", "");
						flag = true;
					}
					if (!flag)
					{
						properties.InputFilePath = GenerateInputFilePath(properties.OriginalFilePath);
						IDependencyFileProperties dependencyFileProperties = propertiesFactory.CreateDependencyFileProperties(properties.InputFilePath);
						dependencyFileProperties.DisposableObject = new FileJanitor(properties.InputFilePath);
						properties.DependencyFiles.Add(dependencyFileProperties);
						Tweak(properties);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Error when pre-tweaking file: " + ex.Message, ex);
			}
		}

		private string GenerateInputFilePath(string originalFilePath)
		{
			string extension = Path.GetExtension(originalFilePath);
			string randomFileName = Path.GetRandomFileName();
			randomFileName = Path.ChangeExtension(randomFileName, extension);
			return Path.Combine(Path.GetTempPath(), randomFileName);
		}
	}
}
