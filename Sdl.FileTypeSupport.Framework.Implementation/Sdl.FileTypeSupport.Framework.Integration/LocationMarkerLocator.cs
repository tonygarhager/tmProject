using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.Integration
{
	public class LocationMarkerLocator : AbstractBilingualContentProcessor, INativeExtractionBilingualContentLocator
	{
		private Dictionary<LocationMarkerId, ILocationMarker> _locationMarkerDictionary = new Dictionary<LocationMarkerId, ILocationMarker>();

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			foreach (IAbstractMarkupData allSubItem in paragraphUnit.Source.AllSubItems)
			{
				ILocationMarker locationMarker = allSubItem as ILocationMarker;
				if (locationMarker != null)
				{
					_locationMarkerDictionary[locationMarker.MarkerId] = locationMarker;
				}
			}
			base.ProcessParagraphUnit(paragraphUnit);
		}

		public ILocationMarker FindLocationMarker(LocationMarkerId markerId)
		{
			if (_locationMarkerDictionary.TryGetValue(markerId, out ILocationMarker value))
			{
				return value;
			}
			return null;
		}
	}
}
