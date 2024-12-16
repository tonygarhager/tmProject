using Sdl.FileTypeSupport.Framework.Formatting;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.FileTypeSupport.Framework.Native
{
	[Serializable]
	public class StartTagProperties : AbstractTagProperties, IStartTagProperties, IAbstractTagProperties, IAbstractBasicTagProperties, IMetaDataContainer, ICloneable, IAbstractInlineTagProperties
	{
		private IFormattingGroup _Formatting;

		private SegmentationHint _segmentationHint = SegmentationHint.MayExclude;

		public IFormattingGroup Formatting
		{
			get
			{
				return _Formatting;
			}
			set
			{
				_Formatting = value;
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

		public StartTagProperties()
		{
			base.IsWordStop = false;
		}

		protected StartTagProperties(StartTagProperties other)
			: base(other)
		{
			if (other._Formatting != null)
			{
				_Formatting = (IFormattingGroup)other._Formatting.Clone();
			}
			_segmentationHint = other._segmentationHint;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			StartTagProperties startTagProperties = (StartTagProperties)obj;
			if (!base.Equals((object)startTagProperties))
			{
				return false;
			}
			if (Formatting == null != (startTagProperties.Formatting == null))
			{
				return false;
			}
			if (Formatting != null && !Formatting.Equals(startTagProperties.Formatting))
			{
				return false;
			}
			if (_segmentationHint != startTagProperties._segmentationHint)
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ ((Formatting != null) ? Formatting.GetHashCode() : 0) ^ _segmentationHint.GetHashCode();
		}

		public override object Clone()
		{
			return new StartTagProperties(this);
		}
	}
}
