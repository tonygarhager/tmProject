using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi
{
	public class BilingualMessageLocation : IMessageLocation
	{
		private FileId? _fileId;

		private ParagraphUnitId? _paragraphUnitId;

		private SegmentId? _segmentId;

		private ContentRestriction _sourceOrTarget;

		private int _charactersIntoParagraph = -1;

		private int _charactersIntoSegment = -1;

		private string _locationDescription;

		private TextLocation _paragraphLocation;

		public virtual FileId? FileId
		{
			get
			{
				return _fileId;
			}
			set
			{
				_fileId = value;
			}
		}

		public virtual ParagraphUnitId? ParagrahUnitId
		{
			get
			{
				return _paragraphUnitId;
			}
			set
			{
				_paragraphUnitId = value;
			}
		}

		public virtual SegmentId? SegmentId
		{
			get
			{
				return _segmentId;
			}
			set
			{
				_segmentId = value;
			}
		}

		public virtual TextLocation ParagraphLocation
		{
			get
			{
				return _paragraphLocation;
			}
			set
			{
				_paragraphLocation = value;
			}
		}

		public virtual string LocationDescription
		{
			get
			{
				return _locationDescription;
			}
			set
			{
				_locationDescription = value;
			}
		}

		public virtual ContentRestriction SourceOrTarget
		{
			get
			{
				return _sourceOrTarget;
			}
			set
			{
				_sourceOrTarget = value;
			}
		}

		public virtual int CharactersIntoParagraph
		{
			get
			{
				return _charactersIntoParagraph;
			}
			set
			{
				_charactersIntoParagraph = value;
			}
		}

		public virtual int CharactersIntoSegment
		{
			get
			{
				return _charactersIntoSegment;
			}
			set
			{
				_charactersIntoSegment = value;
			}
		}

		public BilingualMessageLocation()
		{
		}

		public BilingualMessageLocation(TextLocation textLocation)
		{
			_paragraphLocation = textLocation;
			if (textLocation.Location != null && textLocation.Location.BottomLevel.ItemAtLocation is ISegment)
			{
				SetSegmentInformation(textLocation.Location.BottomLevel.ItemAtLocation as ISegment);
			}
			else
			{
				if (textLocation.Location == null)
				{
					return;
				}
				IAbstractMarkupDataContainer parent = textLocation.Location.BottomLevel.Parent;
				IParagraph paragraph;
				while (true)
				{
					if (parent != null)
					{
						ISegment segment = parent as ISegment;
						if (segment != null)
						{
							SetSegmentInformation(segment);
							return;
						}
						paragraph = (parent as IParagraph);
						if (paragraph != null)
						{
							break;
						}
						IAbstractMarkupData abstractMarkupData = parent as IAbstractMarkupData;
						if (abstractMarkupData != null)
						{
							parent = abstractMarkupData.Parent;
							continue;
						}
						return;
					}
					return;
				}
				SetParagraphUnitInformation(paragraph);
			}
		}

		private void SetSegmentInformation(ISegment segment)
		{
			_segmentId = segment.Properties.Id;
			_paragraphUnitId = segment.ParentParagraphUnit.Properties.ParagraphUnitId;
			_sourceOrTarget = ((!segment.ParentParagraph.IsTarget) ? ContentRestriction.Source : ContentRestriction.Target);
		}

		private void SetParagraphUnitInformation(IParagraph paragraph)
		{
			_paragraphUnitId = paragraph.Parent.Properties.ParagraphUnitId;
			_sourceOrTarget = ((!paragraph.IsTarget) ? ContentRestriction.Source : ContentRestriction.Target);
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			BilingualMessageLocation bilingualMessageLocation = (BilingualMessageLocation)obj;
			if (bilingualMessageLocation._sourceOrTarget != _sourceOrTarget)
			{
				return false;
			}
			if (bilingualMessageLocation._charactersIntoParagraph != _charactersIntoParagraph)
			{
				return false;
			}
			if (!bilingualMessageLocation._fileId.HasValue != !_fileId.HasValue)
			{
				return false;
			}
			if (_fileId.HasValue && !_fileId.Equals(bilingualMessageLocation._fileId))
			{
				return false;
			}
			if (!bilingualMessageLocation._paragraphUnitId.HasValue != !_paragraphUnitId.HasValue)
			{
				return false;
			}
			if (_paragraphUnitId.HasValue && !_paragraphUnitId.Equals(bilingualMessageLocation._paragraphUnitId))
			{
				return false;
			}
			if (!bilingualMessageLocation._segmentId.HasValue != !_segmentId.HasValue)
			{
				return false;
			}
			if (_segmentId.HasValue && !_segmentId.Equals(bilingualMessageLocation._segmentId))
			{
				return false;
			}
			if (bilingualMessageLocation._locationDescription == null != (_locationDescription == null))
			{
				return false;
			}
			if (_locationDescription != null && !_locationDescription.Equals(bilingualMessageLocation._locationDescription))
			{
				return false;
			}
			if (bilingualMessageLocation._paragraphLocation == null != (_paragraphLocation == null))
			{
				return false;
			}
			if (_paragraphLocation != null && !_paragraphLocation.Equals(bilingualMessageLocation._paragraphLocation))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return _sourceOrTarget.GetHashCode() ^ _charactersIntoParagraph.GetHashCode() ^ (_fileId.HasValue ? _fileId.GetHashCode() : 0) ^ (_paragraphUnitId.HasValue ? _paragraphUnitId.GetHashCode() : 0) ^ (_segmentId.HasValue ? _segmentId.GetHashCode() : 0) ^ ((_locationDescription != null) ? _locationDescription.GetHashCode() : 0) ^ ((_paragraphLocation != null) ? _paragraphLocation.GetHashCode() : 0);
		}
	}
}
