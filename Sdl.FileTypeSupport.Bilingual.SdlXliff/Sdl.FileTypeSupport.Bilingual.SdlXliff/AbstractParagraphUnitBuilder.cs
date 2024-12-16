#define TRACE
using Oasis.Xliff12;
using Sdl.FileTypeSupport.Framework;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Diagnostics;
using System.Text;

namespace Sdl.FileTypeSupport.Bilingual.SdlXliff
{
	public abstract class AbstractParagraphUnitBuilder : ITransUnitVisitor
	{
		private IDocumentItemFactory _ItemFactory;

		private IBilingualContentMessageReporter _MessageReporter;

		public IBilingualContentMessageReporter MessageReporter
		{
			get
			{
				return _MessageReporter;
			}
			set
			{
				_MessageReporter = value;
			}
		}

		public IPropertiesFactory PropertiesFactory
		{
			get
			{
				if (_ItemFactory != null)
				{
					return _ItemFactory.PropertiesFactory;
				}
				return null;
			}
		}

		public IDocumentItemFactory ItemFactory
		{
			get
			{
				return _ItemFactory;
			}
			set
			{
				_ItemFactory = value;
			}
		}

		protected AbstractParagraphUnitBuilder()
		{
		}

		protected AbstractParagraphUnitBuilder(IDocumentItemFactory itemFactory)
		{
			_ItemFactory = itemFactory;
		}

		protected virtual void CheckOutput()
		{
			if (_ItemFactory == null || PropertiesFactory == null)
			{
				throw new FileTypeSupportException("ItemFactory and/or PropetiesFactory not set.");
			}
		}

		protected void SetTextAndSubSegmentsFromElementContent(object[] items, IAbstractTag tag)
		{
			tag.TagProperties.TagContent = GetElementTextAndSetSubSegments(items, tag);
			if (tag.HasSubSegmentReferences)
			{
				foreach (ISubSegmentReference subSegment in tag.SubSegments)
				{
					tag.TagProperties.AddSubSegment(subSegment.Properties);
				}
			}
		}

		protected string GetElementTextAndSetSubSegments(object[] items, IAbstractTag tag)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (object obj in items)
			{
				string text = obj as string;
				if (text != null)
				{
					stringBuilder.Append(text);
					continue;
				}
				sub sub = obj as sub;
				if (sub != null)
				{
					int length = stringBuilder.Length;
					stringBuilder.Append(GetElementTextAndSetSubSegments(((sub)obj).Items, tag));
					int length2 = stringBuilder.Length - length;
					ISubSegmentReference subSegmentReference = ItemFactory.CreateSubSegmentReference(PropertiesFactory.CreateSubSegmentProperties(length, length2), new ParagraphUnitId(sub.xid));
					tag.AddSubSegmentReference(subSegmentReference);
				}
				else
				{
					Trace.WriteLine($"Ignoring unknown element content: {obj}");
				}
			}
			return stringBuilder.ToString();
		}

		public virtual void Text(string text)
		{
			if (text == null)
			{
				throw new ArgumentNullException("text");
			}
			CheckOutput();
		}

		public virtual void PlaceholderTag(x x)
		{
			if (x == null)
			{
				throw new ArgumentNullException("x");
			}
			CheckOutput();
		}

		public virtual void StartPairedTag(g g)
		{
			if (g == null)
			{
				throw new ArgumentNullException("g");
			}
			CheckOutput();
		}

		public virtual void EndPairedTag(g g)
		{
			if (g == null)
			{
				throw new ArgumentNullException("g");
			}
			CheckOutput();
		}

		public virtual void SegmentStart(mrk mrk)
		{
			if (mrk == null)
			{
				throw new ArgumentNullException("mrk");
			}
			CheckOutput();
		}

		public virtual void SegmentEnd(mrk mrk)
		{
			if (mrk == null)
			{
				throw new ArgumentNullException("mrk");
			}
			CheckOutput();
		}

		public virtual void LocationMarker(mrk mrk)
		{
			if (mrk == null)
			{
				throw new ArgumentNullException("mrk");
			}
			CheckOutput();
		}

		public virtual void CommentsMarkerStart(mrk mrk)
		{
			if (mrk == null)
			{
				throw new ArgumentNullException("mrk");
			}
			CheckOutput();
		}

		public virtual void CommentsMarkerEnd(mrk mrk)
		{
			if (mrk == null)
			{
				throw new ArgumentNullException("mrk");
			}
			CheckOutput();
		}

		public virtual void RevisionMarkerStart(mrk mrk)
		{
			if (mrk == null)
			{
				throw new ArgumentNullException("mrk");
			}
			CheckOutput();
		}

		public virtual void RevisionMarkerEnd(mrk mrk)
		{
			if (mrk == null)
			{
				throw new ArgumentNullException("mrk");
			}
			CheckOutput();
		}

		public virtual void MarkStart(mrk mrk)
		{
			if (mrk == null)
			{
				throw new ArgumentNullException("mrk");
			}
			CheckOutput();
		}

		public virtual void MarkEnd(mrk mrk)
		{
			if (mrk == null)
			{
				throw new ArgumentNullException("mrk");
			}
			CheckOutput();
		}

		public virtual void UnknownContent(object element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			CheckOutput();
		}
	}
}
