using Oasis.Xliff12;
using Sdl.FileTypeSupport.Framework;

namespace Sdl.FileTypeSupport.Bilingual.SdlXliff
{
	public class TransUnitIterator
	{
		private transunit _TransUnit;

		private ITransUnitVisitor _Output;

		public transunit TransUnit
		{
			get
			{
				return _TransUnit;
			}
			set
			{
				_TransUnit = value;
			}
		}

		public ITransUnitVisitor Output
		{
			get
			{
				return _Output;
			}
			set
			{
				_Output = value;
			}
		}

		public TransUnitIterator()
		{
		}

		public TransUnitIterator(transunit transUnit)
		{
			_TransUnit = transUnit;
		}

		public void Parse(bool source)
		{
			if (source)
			{
				if (_TransUnit.segsource != null)
				{
					if (_TransUnit.segsource.Items != null)
					{
						ParseContent(_TransUnit.segsource.Items);
					}
					return;
				}
				if (_TransUnit.source == null)
				{
					throw new FileTypeSupportException(string.Format(StringResources.CorruptFile_TransUnitWithoutSource, _TransUnit));
				}
				if (_TransUnit.source.Items != null)
				{
					ParseContent(_TransUnit.source.Items);
				}
			}
			else if (_TransUnit.target?.Items != null)
			{
				ParseContent(_TransUnit.target.Items);
			}
		}

		private void ParseContent(object[] content)
		{
			if (content == null)
			{
				return;
			}
			foreach (object obj in content)
			{
				string text = obj as string;
				if (text != null)
				{
					_Output.Text(text);
					continue;
				}
				x x = obj as x;
				if (x != null)
				{
					_Output.PlaceholderTag(x);
					continue;
				}
				g g = obj as g;
				if (g != null)
				{
					_Output.StartPairedTag(g);
					ParseContent(g.Items);
					_Output.EndPairedTag(g);
					continue;
				}
				mrk mrk = obj as mrk;
				if (mrk != null)
				{
					IterateOvreMark(mrk);
				}
				else
				{
					_Output.UnknownContent(obj);
				}
			}
		}

		private void IterateOvreMark(mrk mrk)
		{
			switch (mrk.mtype)
			{
			case "seg":
			{
				_Output.SegmentStart(mrk);
				object[] items3 = mrk.Items;
				if (items3 != null && items3.Length != 0)
				{
					ParseContent(mrk.Items);
				}
				_Output.SegmentEnd(mrk);
				break;
			}
			case "x-sdl-location":
			{
				_Output.LocationMarker(mrk);
				object[] items5 = mrk.Items;
				if (items5 != null && items5.Length != 0)
				{
					ParseContent(mrk.Items);
				}
				break;
			}
			case "x-sdl-comment":
			{
				_Output.CommentsMarkerStart(mrk);
				object[] items4 = mrk.Items;
				if (items4 != null && items4.Length != 0)
				{
					ParseContent(mrk.Items);
				}
				_Output.CommentsMarkerEnd(mrk);
				break;
			}
			case "x-sdl-added":
			case "x-sdl-deleted":
			case "x-sdl-feedback-comment":
			case "x-sdl-feedback-added":
			case "x-sdl-feedback-deleted":
			{
				_Output.RevisionMarkerStart(mrk);
				object[] items2 = mrk.Items;
				if (items2 != null && items2.Length != 0)
				{
					ParseContent(mrk.Items);
				}
				_Output.RevisionMarkerEnd(mrk);
				break;
			}
			default:
			{
				_Output.MarkStart(mrk);
				object[] items = mrk.Items;
				if (items != null && items.Length != 0)
				{
					ParseContent(mrk.Items);
				}
				_Output.MarkEnd(mrk);
				break;
			}
			}
		}
	}
}
