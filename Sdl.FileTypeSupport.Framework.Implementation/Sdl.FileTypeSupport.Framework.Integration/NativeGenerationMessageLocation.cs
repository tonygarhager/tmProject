using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.FileTypeSupport.Framework.Integration
{
	public class NativeGenerationMessageLocation : BilingualMessageLocation
	{
		private LocationMarkerId _markerId;

		private LocationInfo _info;

		private INativeGenerationBilingualContentLocator _contentLocator;

		private INativeLocationInfoProvider _locationInfoProvider;

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

		public INativeLocationInfoProvider LocationInfoProvider
		{
			get
			{
				return _locationInfoProvider;
			}
			set
			{
				_locationInfoProvider = value;
			}
		}

		public INativeGenerationBilingualContentLocator ContentLocator
		{
			get
			{
				return _contentLocator;
			}
			set
			{
				_contentLocator = value;
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

		public override ParagraphUnitId? ParagrahUnitId
		{
			get
			{
				EnsureInitialized();
				return base.ParagrahUnitId;
			}
			set
			{
				base.ParagrahUnitId = value;
			}
		}

		public override TextLocation ParagraphLocation
		{
			get
			{
				EnsureInitialized();
				return base.ParagraphLocation;
			}
			set
			{
				base.ParagraphLocation = value;
			}
		}

		public override SegmentId? SegmentId
		{
			get
			{
				EnsureInitialized();
				return base.SegmentId;
			}
			set
			{
				base.SegmentId = value;
			}
		}

		public override ContentRestriction SourceOrTarget
		{
			get
			{
				EnsureInitialized();
				return base.SourceOrTarget;
			}
			set
			{
				base.SourceOrTarget = value;
			}
		}

		public NativeGenerationMessageLocation(LocationMarkerId markerId)
		{
			_markerId = markerId;
		}

		private void EnsureInitialized()
		{
			if (_locationInfoProvider == null || (_info != null && base.ParagraphLocation != null))
			{
				return;
			}
			_info = _locationInfoProvider.GetLocationInfo(_markerId);
			if (_info == null)
			{
				return;
			}
			bool flag = false;
			if (_contentLocator != null)
			{
				IMessageLocation messageLocation = _contentLocator.FindLocation(_info);
				if (messageLocation != null)
				{
					base.ParagraphLocation = messageLocation.ParagraphLocation;
					base.ParagrahUnitId = messageLocation.ParagrahUnitId;
					base.SegmentId = messageLocation.SegmentId;
					base.CharactersIntoParagraph = messageLocation.CharactersIntoParagraph;
					base.CharactersIntoSegment = messageLocation.CharactersIntoSegment;
					base.SourceOrTarget = messageLocation.SourceOrTarget;
					flag = true;
				}
			}
			if (!flag)
			{
				base.ParagraphLocation = null;
				base.ParagrahUnitId = _info.ParagraphUnitId;
				if (_info.ParagraphUnitId.HasValue)
				{
					base.CharactersIntoParagraph = _info.CharactersAfterParagraphStart;
				}
				base.SegmentId = _info.SegmentId;
				base.SourceOrTarget = _info.SourceOrTarget;
			}
		}
	}
}
