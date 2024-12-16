using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.FileTypeSupport.Framework.Integration
{
	public class NativeWriterMessagesProxy : INativeFileWriter, INativeGenerationContentHandler, IAbstractNativeContentHandler, INativeOutputSettingsAware, IDisposable, INativeContentCycleAware
	{
		private GenerationLocationTracker _locationTracker = new GenerationLocationTracker();

		private GenerationBilingualContentLocator _bilingualContentLocator;

		private INativeFileWriter _realWriter;

		private EventHandler<MessageEventArgs> _externalMessageReporter;

		private IPersistentFileConversionProperties _fileProperties;

		public INativeFileWriter RealWriter
		{
			get
			{
				return _realWriter;
			}
			set
			{
				_realWriter = value;
			}
		}

		public GenerationBilingualContentLocator BilingualContentLocator
		{
			get
			{
				return _bilingualContentLocator;
			}
			set
			{
				_bilingualContentLocator = value;
			}
		}

		public GenerationLocationTracker LocationTracker
		{
			get
			{
				return _locationTracker;
			}
			set
			{
				_locationTracker = value;
				if (_realWriter != null)
				{
					_realWriter.LocationTracker = value;
				}
			}
		}

		public EventHandler<MessageEventArgs> ExternalMessageReporter
		{
			get
			{
				return _externalMessageReporter;
			}
			set
			{
				_externalMessageReporter = value;
			}
		}

		public INativeTextLocationMessageReporter MessageReporter
		{
			get
			{
				if (_realWriter != null)
				{
					return _realWriter.MessageReporter;
				}
				return null;
			}
			set
			{
				if (_realWriter != null)
				{
					_realWriter.MessageReporter = value;
					return;
				}
				throw new FileTypeSupportException("Proxy has no real writer yet!");
			}
		}

		INativeLocationTracker INativeFileWriter.LocationTracker
		{
			get
			{
				if (_realWriter != null)
				{
					return _realWriter.LocationTracker;
				}
				return null;
			}
			set
			{
				if (_realWriter != null)
				{
					_realWriter.LocationTracker = value;
				}
			}
		}

		public NativeWriterMessagesProxy(INativeFileWriter realWriter, EventHandler<MessageEventArgs> externalMessageReporter, GenerationBilingualContentLocator bilingualContentLocator)
		{
			_realWriter = realWriter;
			_locationTracker.Output = _realWriter;
			_externalMessageReporter = externalMessageReporter;
			_bilingualContentLocator = bilingualContentLocator;
		}

		private void ReportException(Exception ex)
		{
			if (_externalMessageReporter != null)
			{
				LocationInfo currentLocationInfo = _locationTracker.GetCurrentLocationInfo();
				IMessageLocation fromLocation = _bilingualContentLocator.FindLocation(currentLocationInfo);
				string message = $"{ex.Message}";
				string filePath = null;
				if (_fileProperties != null)
				{
					filePath = _fileProperties.OriginalFilePath;
				}
				_externalMessageReporter(_realWriter, new MessageEventArgs(filePath, StringResources.NativeFileWriting, ErrorLevel.Error, message, fromLocation, null));
			}
		}

		public void ParagraphUnitStart(IParagraphUnitProperties properties)
		{
			try
			{
				_locationTracker.ParagraphUnitStart(properties);
			}
			catch (Exception ex)
			{
				ReportException(ex);
				throw;
			}
		}

		public void ParagraphUnitEnd()
		{
			try
			{
				_locationTracker.ParagraphUnitEnd();
			}
			catch (Exception ex)
			{
				ReportException(ex);
				throw;
			}
		}

		public void SegmentStart(ISegmentPairProperties properties)
		{
			try
			{
				_locationTracker.SegmentStart(properties);
			}
			catch (Exception ex)
			{
				ReportException(ex);
				throw;
			}
		}

		public void SegmentEnd()
		{
			try
			{
				_locationTracker.SegmentEnd();
			}
			catch (Exception ex)
			{
				ReportException(ex);
				throw;
			}
		}

		public void StructureTag(IStructureTagProperties tagInfo)
		{
			try
			{
				_locationTracker.StructureTag(tagInfo);
			}
			catch (Exception ex)
			{
				ReportException(ex);
				throw;
			}
		}

		public void InlineStartTag(IStartTagProperties tagInfo)
		{
			try
			{
				_locationTracker.InlineStartTag(tagInfo);
			}
			catch (Exception ex)
			{
				ReportException(ex);
				throw;
			}
		}

		public void InlineEndTag(IEndTagProperties tagInfo)
		{
			try
			{
				_locationTracker.InlineEndTag(tagInfo);
			}
			catch (Exception ex)
			{
				ReportException(ex);
				throw;
			}
		}

		public void InlinePlaceholderTag(IPlaceholderTagProperties tagInfo)
		{
			try
			{
				_locationTracker.InlinePlaceholderTag(tagInfo);
			}
			catch (Exception ex)
			{
				ReportException(ex);
				throw;
			}
		}

		public void Text(ITextProperties textInfo)
		{
			try
			{
				_locationTracker.Text(textInfo);
			}
			catch (Exception ex)
			{
				ReportException(ex);
				throw;
			}
		}

		public void ChangeContext(IContextProperties newContexts)
		{
			try
			{
				_locationTracker.ChangeContext(newContexts);
			}
			catch (Exception ex)
			{
				ReportException(ex);
				throw;
			}
		}

		public void CustomInfo(ICustomInfoProperties info)
		{
			try
			{
				_locationTracker.CustomInfo(info);
			}
			catch (Exception ex)
			{
				ReportException(ex);
				throw;
			}
		}

		public void LocationMark(LocationMarkerId markerId)
		{
			try
			{
				_locationTracker.LocationMark(markerId);
			}
			catch (Exception ex)
			{
				ReportException(ex);
				throw;
			}
		}

		public void LockedContentStart(ILockedContentProperties lockedContentInfo)
		{
			try
			{
				_locationTracker.LockedContentStart(lockedContentInfo);
			}
			catch (Exception ex)
			{
				ReportException(ex);
				throw;
			}
		}

		public void LockedContentEnd()
		{
			try
			{
				_locationTracker.LockedContentEnd();
			}
			catch (Exception ex)
			{
				ReportException(ex);
				throw;
			}
		}

		public void RevisionStart(IRevisionProperties revisionInfo)
		{
			try
			{
				_locationTracker.RevisionStart(revisionInfo);
			}
			catch (Exception ex)
			{
				ReportException(ex);
				throw;
			}
		}

		public void RevisionEnd()
		{
			try
			{
				_locationTracker.RevisionEnd();
			}
			catch (Exception ex)
			{
				ReportException(ex);
				throw;
			}
		}

		public void CommentStart(ICommentProperties commentInfo)
		{
			try
			{
				_locationTracker.CommentStart(commentInfo);
			}
			catch (Exception ex)
			{
				ReportException(ex);
				throw;
			}
		}

		public void CommentEnd()
		{
			try
			{
				_locationTracker.CommentEnd();
			}
			catch (Exception ex)
			{
				ReportException(ex);
				throw;
			}
		}

		public void ParagraphComments(ICommentProperties commentInfo)
		{
			try
			{
				_locationTracker.ParagraphComments(commentInfo);
			}
			catch (Exception ex)
			{
				ReportException(ex);
				throw;
			}
		}

		public void SetOutputProperties(INativeOutputFileProperties properties)
		{
			_locationTracker.SetOutputProperties(properties);
			if (_realWriter != null)
			{
				_realWriter.SetOutputProperties(properties);
			}
		}

		public void GetProposedOutputFileInfo(IPersistentFileConversionProperties fileProperties, IOutputFileInfo proposedFileInfo)
		{
			if (_realWriter != null)
			{
				_realWriter.GetProposedOutputFileInfo(fileProperties, proposedFileInfo);
			}
		}

		public void SetFileProperties(IFileProperties properties)
		{
			_fileProperties = properties.FileConversionProperties;
			(_locationTracker as INativeContentCycleAware)?.SetFileProperties(properties);
			(_realWriter as INativeContentCycleAware)?.SetFileProperties(properties);
		}

		public void StartOfInput()
		{
			try
			{
				(_locationTracker as INativeContentCycleAware)?.StartOfInput();
				(_realWriter as INativeContentCycleAware)?.StartOfInput();
			}
			catch (Exception ex)
			{
				ReportException(ex);
				throw;
			}
		}

		public void EndOfInput()
		{
			try
			{
				(_locationTracker as INativeContentCycleAware)?.EndOfInput();
				(_realWriter as INativeContentCycleAware)?.EndOfInput();
			}
			catch (Exception ex)
			{
				ReportException(ex);
				throw;
			}
		}

		public void Dispose()
		{
			if (_realWriter != null)
			{
				_realWriter.Dispose();
			}
		}
	}
}
