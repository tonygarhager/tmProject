using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.FileTypeSupport.Framework.Native
{
	[Serializable]
	public class PlaceholderTagProperties : AbstractTagProperties, IPlaceholderTagProperties, IAbstractTagProperties, IAbstractBasicTagProperties, IMetaDataContainer, ICloneable, IAbstractInlineTagProperties
	{
		private string _textEquivalent;

		private bool _isBreakableWhiteSpace;

		private SegmentationHint _segmentationHint = SegmentationHint.MayExclude;

		public string TextEquivalent
		{
			get
			{
				return _textEquivalent;
			}
			set
			{
				_textEquivalent = value;
			}
		}

		public bool HasTextEquivalent => !string.IsNullOrEmpty(TextEquivalent);

		public bool IsBreakableWhiteSpace
		{
			get
			{
				return _isBreakableWhiteSpace;
			}
			set
			{
				_isBreakableWhiteSpace = value;
			}
		}

		public SegmentationHint SegmentationHint
		{
			get
			{
				return _segmentationHint;
			}
			set
			{
				_segmentationHint = value;
			}
		}

		public PlaceholderTagProperties()
		{
			base.IsWordStop = false;
		}

		protected PlaceholderTagProperties(PlaceholderTagProperties other)
			: base(other)
		{
			_segmentationHint = other._segmentationHint;
			_textEquivalent = other._textEquivalent;
			_isBreakableWhiteSpace = other._isBreakableWhiteSpace;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			PlaceholderTagProperties placeholderTagProperties = (PlaceholderTagProperties)obj;
			if (!base.Equals((object)placeholderTagProperties))
			{
				return false;
			}
			if (_segmentationHint != placeholderTagProperties._segmentationHint)
			{
				return false;
			}
			if (HasTextEquivalent != placeholderTagProperties.HasTextEquivalent)
			{
				return false;
			}
			if (HasTextEquivalent && _textEquivalent != placeholderTagProperties._textEquivalent)
			{
				return false;
			}
			if (_isBreakableWhiteSpace != placeholderTagProperties._isBreakableWhiteSpace)
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ _segmentationHint.GetHashCode() ^ _isBreakableWhiteSpace.GetHashCode() ^ ((_textEquivalent != null) ? _textEquivalent.GetHashCode() : 0);
		}

		public override object Clone()
		{
			return new PlaceholderTagProperties(this);
		}
	}
}
