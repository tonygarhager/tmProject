using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	[Serializable]
	public class SegmentReference
	{
		private SegmentId _segmentId;

		private ParagraphUnitId _paragraphUnitId;

		private FileId _fileId;

		public FileId FileId
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

		public ParagraphUnitId ParagraphUnitId
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

		public SegmentId SegmentId
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

		public SegmentReference(FileId fileId, ParagraphUnitId paraId, SegmentId segId)
		{
			_fileId = fileId;
			_paragraphUnitId = paraId;
			_segmentId = segId;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			SegmentReference segmentReference = (SegmentReference)obj;
			if (!_fileId.Equals(segmentReference._fileId))
			{
				return false;
			}
			if (!_paragraphUnitId.Equals(segmentReference._paragraphUnitId))
			{
				return false;
			}
			if (!_segmentId.Equals(segmentReference._segmentId))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return _fileId.GetHashCode() ^ _paragraphUnitId.GetHashCode() ^ _segmentId.GetHashCode();
		}
	}
}
