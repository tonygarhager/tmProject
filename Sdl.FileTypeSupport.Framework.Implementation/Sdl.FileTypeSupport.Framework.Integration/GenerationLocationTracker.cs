using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.FileTypeSupport.Framework.Integration
{
	public class GenerationLocationTracker : LineNumberTracker, INativeLocationInfoProvider, INativeTextLocationInfoProvider, INativeOutputSettingsAware
	{
		private INativeOutputFileProperties _outputProperties;

		private IParagraphUnitProperties _paragraphProperties;

		private ISegmentPairProperties _segmentProperties;

		private Dictionary<LocationMarkerId, LocationInfo> _locationMarkerDictionary = new Dictionary<LocationMarkerId, LocationInfo>();

		private int _charactersSinceStartOfParagraph;

		private int _charactersSinceStartOfSegment;

		private int _charactersSinceEndOfSegment;

		private SegmentId? _lastSegmentId;

		private SortedList<NativeTextLocation, LocationInfo> _boundaries = new SortedList<NativeTextLocation, LocationInfo>();

		private List<int> _charactersPerLine = new List<int>(1000);

		public GenerationLocationTracker()
		{
			_charactersPerLine.Add(0);
		}

		protected override void IncrementLineAndOffsetNumbers(string text)
		{
			if (_paragraphProperties != null)
			{
				int length = text.Length;
				_charactersSinceStartOfParagraph += length;
				if (_segmentProperties != null)
				{
					_charactersSinceStartOfSegment += length;
				}
				else if (_lastSegmentId.HasValue)
				{
					_charactersSinceEndOfSegment += length;
				}
			}
			int num = base.Line;
			int num2 = base.Offset - 1;
			bool flag = base.LastCharacterWasCR;
			for (int i = 0; i < text.Length; i++)
			{
				switch (text[i])
				{
				case '\r':
					_charactersPerLine.Add(num2 + 1);
					num++;
					num2 = 0;
					flag = true;
					break;
				case '\n':
					if (flag)
					{
						_charactersPerLine[num - 1]++;
					}
					else
					{
						_charactersPerLine.Add(num2 + 1);
						num++;
						num2 = 0;
					}
					flag = false;
					break;
				default:
					num2++;
					flag = false;
					break;
				}
			}
			base.IncrementLineAndOffsetNumbers(text);
		}

		public override void ParagraphUnitStart(IParagraphUnitProperties properties)
		{
			_paragraphProperties = properties;
			_charactersSinceStartOfParagraph = 0;
			_boundaries[GetCurrentLocation()] = GetCurrentLocationInfo();
			base.ParagraphUnitStart(properties);
		}

		public override void ParagraphUnitEnd()
		{
			_paragraphProperties = null;
			_lastSegmentId = null;
			base.ParagraphUnitEnd();
		}

		public override void SegmentStart(ISegmentPairProperties properties)
		{
			_segmentProperties = properties;
			_charactersSinceStartOfSegment = 0;
			_boundaries[GetCurrentLocation()] = GetCurrentLocationInfo();
			base.SegmentStart(properties);
		}

		public override void SegmentEnd()
		{
			if (_segmentProperties != null)
			{
				_lastSegmentId = _segmentProperties.Id;
			}
			_segmentProperties = null;
			_charactersSinceEndOfSegment = 0;
			_boundaries[GetCurrentLocation()] = GetCurrentLocationInfo();
			base.SegmentEnd();
		}

		public override void LocationMark(LocationMarkerId markerId)
		{
			if (!_locationMarkerDictionary.ContainsKey(markerId))
			{
				LocationInfo currentLocationInfo = GetCurrentLocationInfo();
				_locationMarkerDictionary.Add(markerId, currentLocationInfo);
			}
			base.LocationMark(markerId);
		}

		public LocationInfo GetCurrentLocationInfo()
		{
			LocationInfo locationInfo = new LocationInfo();
			if (_outputProperties != null)
			{
				locationInfo.SourceOrTarget = _outputProperties.ContentRestriction;
			}
			locationInfo.LinesFromStartOfFile = base.Line;
			locationInfo.OffsetFromStartOfLine = base.Offset;
			if (_paragraphProperties != null)
			{
				locationInfo.ParagraphUnitId = _paragraphProperties.ParagraphUnitId;
				locationInfo.CharactersAfterParagraphStart = _charactersSinceStartOfParagraph;
			}
			if (_segmentProperties != null)
			{
				locationInfo.IsInSegment = true;
				locationInfo.SegmentId = _segmentProperties.Id;
				locationInfo.CharactersAfterSegmentStart = _charactersSinceStartOfSegment;
			}
			else
			{
				locationInfo.IsInSegment = false;
				locationInfo.SegmentId = _lastSegmentId;
				locationInfo.CharactersAfterSegment = _charactersSinceEndOfSegment;
			}
			return locationInfo;
		}

		public void SetOutputProperties(INativeOutputFileProperties properties)
		{
			_outputProperties = properties;
		}

		public void GetProposedOutputFileInfo(IPersistentFileConversionProperties fileProperties, IOutputFileInfo proposedFileInfo)
		{
		}

		public LocationInfo GetLocationInfo(LocationMarkerId marker)
		{
			if (!_locationMarkerDictionary.TryGetValue(marker, out LocationInfo value))
			{
				return null;
			}
			return value;
		}

		public LocationInfo GetLocationInfo(NativeTextLocation nativeLocation)
		{
			if (nativeLocation == null)
			{
				return null;
			}
			KeyValuePair<NativeTextLocation, LocationInfo> keyValuePair = _boundaries.LastOrDefault((KeyValuePair<NativeTextLocation, LocationInfo> loc) => !loc.Key.IsAfter(nativeLocation));
			LocationInfo locationInfo = (LocationInfo)keyValuePair.Value.Clone();
			locationInfo.LinesFromStartOfFile = nativeLocation.Line;
			locationInfo.OffsetFromStartOfLine = nativeLocation.Offset;
			if (keyValuePair.Equals(default(KeyValuePair<NativeTextLocation, LocationInfo>)))
			{
				return locationInfo;
			}
			int num = 0;
			if (keyValuePair.Key.Line == nativeLocation.Line)
			{
				num = nativeLocation.Offset - keyValuePair.Key.Offset;
			}
			else
			{
				for (int i = keyValuePair.Key.Line; i < nativeLocation.Line && _charactersPerLine.Count < i; i++)
				{
					num += _charactersPerLine[i];
				}
			}
			if (locationInfo.IsInSegment)
			{
				locationInfo.CharactersAfterSegmentStart += num;
			}
			else if (locationInfo.SegmentId.HasValue)
			{
				locationInfo.CharactersAfterSegment += num;
			}
			if (locationInfo.ParagraphUnitId.HasValue)
			{
				locationInfo.CharactersAfterParagraphStart += num;
			}
			return locationInfo;
		}
	}
}
