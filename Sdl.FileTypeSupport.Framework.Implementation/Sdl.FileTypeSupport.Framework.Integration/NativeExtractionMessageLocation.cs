using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.FileTypeSupport.Framework.Integration
{
	public class NativeExtractionMessageLocation : BilingualMessageLocation
	{
		private LocationMarkerId _markerId;

		private ILocationMarker _bilingualMarker;

		private INativeExtractionBilingualContentLocator _bilingualContentLocator;

		public LocationMarkerId MarkerId
		{
			get
			{
				return _markerId;
			}
			set
			{
				_markerId = value;
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

		public override FileId? FileId
		{
			get
			{
				EnsureInitialized();
				return base.FileId;
			}
			set
			{
				base.FileId = value;
			}
		}

		public override ContentRestriction SourceOrTarget
		{
			get
			{
				return ContentRestriction.Source;
			}
			set
			{
				if (value != ContentRestriction.Source)
				{
					throw new ArgumentException("NativeExtractionMessageLocation only supports locations in the source content.");
				}
			}
		}

		public override int CharactersIntoParagraph
		{
			get
			{
				EnsureInitialized();
				return base.CharactersIntoParagraph;
			}
			set
			{
				base.CharactersIntoParagraph = value;
			}
		}

		public override TextLocation ParagraphLocation
		{
			get
			{
				EnsureInitialized();
				if (_bilingualMarker != null && _bilingualMarker.ParentParagraph != null)
				{
					base.ParagraphLocation = new TextLocation(new Location(_bilingualMarker.ParentParagraph, _bilingualMarker), 0);
				}
				return base.ParagraphLocation;
			}
			set
			{
				base.ParagraphLocation = value;
			}
		}

		public NativeExtractionMessageLocation(LocationMarkerId markerId)
		{
			_markerId = markerId;
		}

		public override bool Equals(object obj)
		{
			if (!base.Equals(obj))
			{
				return false;
			}
			NativeExtractionMessageLocation nativeExtractionMessageLocation = (NativeExtractionMessageLocation)obj;
			if (_markerId == null != (nativeExtractionMessageLocation._markerId != null))
			{
				return false;
			}
			if (_markerId != null && !_markerId.Equals(nativeExtractionMessageLocation._markerId))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ ((_markerId != null) ? _markerId.GetHashCode() : 0);
		}

		private void EnsureInitialized()
		{
			if (_bilingualMarker != null && _markerId.Equals(_bilingualMarker.MarkerId))
			{
				return;
			}
			if (_markerId == null)
			{
				throw new FileTypeSupportException("Marker ID not set!");
			}
			if (_bilingualContentLocator == null)
			{
				throw new FileTypeSupportException("Bilingual content locator not set!");
			}
			_bilingualMarker = _bilingualContentLocator.FindLocationMarker(_markerId);
			if (_bilingualMarker == null)
			{
				return;
			}
			base.ParagrahUnitId = _bilingualMarker.ParentParagraph.Parent.Properties.ParagraphUnitId;
			IAbstractMarkupData bilingualMarker = _bilingualMarker;
			IAbstractMarkupDataContainer parent = bilingualMarker.Parent;
			ISegment segment = parent as ISegment;
			while (segment == null && parent != null)
			{
				bilingualMarker = (parent as IAbstractMarkupData);
				if (bilingualMarker == null)
				{
					break;
				}
				parent = bilingualMarker.Parent;
				segment = (parent as ISegment);
			}
			if (segment != null)
			{
				base.SegmentId = segment.Properties.Id;
			}
		}
	}
}
