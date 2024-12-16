using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi
{
	public class LineNumberTracker : AbstractNativeExtractionGenerationContentProcessor, INativeLocationTracker
	{
		public int Line
		{
			get;
			set;
		} = 1;


		public int Offset
		{
			get;
			set;
		} = 1;


		public bool LastCharacterWasCR
		{
			get;
			set;
		}

		protected int LineBeforeIncrement
		{
			get;
			set;
		} = 1;


		protected int OffsetBeforeIncrement
		{
			get;
			set;
		} = 1;


		public NativeTextLocation GetCurrentLocation()
		{
			return new NativeTextLocation(Line, Offset);
		}

		public NativeTextLocation GetLocationBeforeCurrentContent()
		{
			return new NativeTextLocation(LineBeforeIncrement, OffsetBeforeIncrement);
		}

		public NativeTextLocation GetLocationAfterCurrentContent()
		{
			return new NativeTextLocation(Line, Offset);
		}

		public override void Text(ITextProperties textInfo)
		{
			IncrementLineAndOffsetNumbers(textInfo.Text);
			base.Text(textInfo);
		}

		public override void InlineStartTag(IStartTagProperties tagInfo)
		{
			IncrementLineAndOffsetNumbers(tagInfo.TagContent);
			base.InlineStartTag(tagInfo);
		}

		public override void InlineEndTag(IEndTagProperties tagInfo)
		{
			IncrementLineAndOffsetNumbers(tagInfo.TagContent);
			base.InlineEndTag(tagInfo);
		}

		public override void InlinePlaceholderTag(IPlaceholderTagProperties tagInfo)
		{
			IncrementLineAndOffsetNumbers(tagInfo.TagContent);
			base.InlinePlaceholderTag(tagInfo);
		}

		public override void StructureTag(IStructureTagProperties tagInfo)
		{
			IncrementLineAndOffsetNumbers(tagInfo.TagContent);
			base.StructureTag(tagInfo);
		}

		public virtual Pair<int, int> ParseText(string text)
		{
			int num = 0;
			int num2 = 0;
			bool flag = LastCharacterWasCR;
			for (int i = 0; i < text.Length; i++)
			{
				switch (text[i])
				{
				case '\r':
					num++;
					num2 = 0;
					flag = true;
					break;
				case '\n':
					if (!flag)
					{
						num++;
						num2 = 0;
					}
					flag = false;
					break;
				default:
					num2++;
					flag = false;
					break;
				}
			}
			return new Pair<int, int>(num, num2);
		}

		protected virtual void IncrementLineAndOffsetNumbers(string text)
		{
			LineBeforeIncrement = Line;
			OffsetBeforeIncrement = Offset;
			Pair<int, int> pair = ParseText(text);
			if (pair.First > 0)
			{
				Line += pair.First;
				Offset = pair.Second;
				LastCharacterWasCR = text.EndsWith("\r");
			}
			else
			{
				Offset += pair.Second;
			}
		}
	}
}
