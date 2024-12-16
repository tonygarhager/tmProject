using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.LanguagePlatform.Core;
using System;
using System.Globalization;

namespace Sdl.Core.Processing.Alignment.Core
{
	internal class AlignmentElement
	{
		private readonly Segment _content;

		private string _textContent;

		private readonly DocumentSegmentId _id;

		public string TextContent => _textContent;

		public Segment Content => _content;

		public DocumentSegmentId Id => _id;

		public int Order => _id.Order;

		public string TextBetweenSegment
		{
			get;
			set;
		}

		public string DocumentStructurePath
		{
			get;
			private set;
		}

		public AlignmentElement(string textContent)
			: this(new ParagraphUnitId(""), new SegmentId(""), textContent, null, 0)
		{
		}

		public AlignmentElement(ParagraphUnitId paragraphUnitId, SegmentId segmentId, string textContent, int order)
			: this(paragraphUnitId, segmentId, textContent, null, order)
		{
		}

		public AlignmentElement(ParagraphUnitId paragraphUnitId, SegmentId segmentId, string textContent, string documentStructurePath, int order)
		{
			_id = new DocumentSegmentId(paragraphUnitId, segmentId, order);
			_textContent = (textContent ?? string.Empty);
			_content = new Segment(CultureInfo.InvariantCulture);
			_content.Add(_textContent);
			DocumentStructurePath = (documentStructurePath ?? string.Empty);
		}

		public AlignmentElement(ParagraphUnitId paragraphUnitId, SegmentId segmentId, string textContent, Segment segment, string documentStructurePath, int order)
		{
			_id = new DocumentSegmentId(paragraphUnitId, segmentId, order);
			_textContent = (textContent ?? string.Empty);
			_content = segment;
			_content.Add(_textContent);
			DocumentStructurePath = (documentStructurePath ?? string.Empty);
		}

		public override string ToString()
		{
			return _textContent;
		}

		public AlignmentElement Merge(AlignmentElement elementToMergeWith)
		{
			if (elementToMergeWith != null)
			{
				int order = Math.Min(Order, elementToMergeWith.Order);
				return new AlignmentElement(default(ParagraphUnitId), default(SegmentId), _content + " " + elementToMergeWith._content, order);
			}
			throw new Exception("Alignment elements cannot be merged!");
		}

		public override bool Equals(object obj)
		{
			AlignmentElement alignmentElement = obj as AlignmentElement;
			if (alignmentElement != null && _id.Equals(alignmentElement._id))
			{
				return _content.Equals(alignmentElement._content);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (_content.GetHashCode() + 173) * _id.GetHashCode();
		}
	}
}
