using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.Integration
{
	public class PreExtractionMessageReporterAdapter : INativeTextLocationMessageReporter, IBasicMessageReporter
	{
		private EventHandler<MessageEventArgs> _messageHandler;

		private INativeExtractionBilingualContentLocator _bilingualContentLocator;

		private string _filePath;

		private List<NativeExtractionMessageLocation> _messageLocations = new List<NativeExtractionMessageLocation>();

		private List<Pair<LocationMarkerId, NativeTextLocation>> _nativeLocations = new List<Pair<LocationMarkerId, NativeTextLocation>>();

		public List<NativeExtractionMessageLocation> MessageLocations
		{
			get
			{
				return _messageLocations;
			}
			set
			{
				_messageLocations = value;
			}
		}

		public List<Pair<LocationMarkerId, NativeTextLocation>> NativeLocations
		{
			get
			{
				return _nativeLocations;
			}
			set
			{
				_nativeLocations = value;
			}
		}

		public string FilePath
		{
			get
			{
				return _filePath;
			}
			set
			{
				_filePath = value;
			}
		}

		public INativeExtractionBilingualContentLocator BilingualContentLocator
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

		public PreExtractionMessageReporterAdapter(EventHandler<MessageEventArgs> messageHandler)
		{
			_messageHandler = messageHandler;
		}

		public virtual void ReportMessage(object source, string origin, ErrorLevel level, string message, NativeTextLocation fromLocation, NativeTextLocation uptoLocation)
		{
			if (_messageHandler != null)
			{
				LocationMarkerId locationMarkerId = new LocationMarkerId();
				NativeExtractionMessageLocation nativeExtractionMessageLocation = new NativeExtractionMessageLocation(locationMarkerId);
				nativeExtractionMessageLocation.BilingualContentLocator = _bilingualContentLocator;
				_messageLocations.Add(nativeExtractionMessageLocation);
				_nativeLocations.Add(new Pair<LocationMarkerId, NativeTextLocation>(locationMarkerId, fromLocation));
				NativeExtractionMessageLocation nativeExtractionMessageLocation2 = null;
				if (uptoLocation != null)
				{
					LocationMarkerId locationMarkerId2 = new LocationMarkerId();
					nativeExtractionMessageLocation2 = new NativeExtractionMessageLocation(locationMarkerId2);
					nativeExtractionMessageLocation2.BilingualContentLocator = _bilingualContentLocator;
					_messageLocations.Add(nativeExtractionMessageLocation2);
					_nativeLocations.Add(new Pair<LocationMarkerId, NativeTextLocation>(locationMarkerId2, uptoLocation));
				}
				_messageHandler(source, new MessageEventArgs(_filePath, origin, level, message, nativeExtractionMessageLocation, nativeExtractionMessageLocation2));
			}
		}

		public virtual void ReportMessage(object source, string origin, ErrorLevel level, string message, string locationDescription)
		{
			if (_messageHandler != null)
			{
				BilingualMessageLocation bilingualMessageLocation = new BilingualMessageLocation();
				bilingualMessageLocation.LocationDescription = locationDescription;
				_messageHandler(source, new MessageEventArgs(_filePath, origin, level, message, bilingualMessageLocation, null));
			}
		}
	}
}
