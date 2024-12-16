using System;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.Core
{
	[DataContract]
	public class Text : SegmentElement
	{
		[DataMember]
		public string Value
		{
			get;
			set;
		}

		public Text()
		{
		}

		public Text(string text)
		{
			Value = text;
		}

		public override SegmentElement Duplicate()
		{
			Text text = new Text();
			if (Value != null)
			{
				text.Value = string.Copy(Value);
			}
			return text;
		}

		public override Similarity GetSimilarity(SegmentElement other)
		{
			Text text = other as Text;
			if (text == null || Value == null || text.Value == null)
			{
				return Similarity.None;
			}
			if (!Value.Equals(text.Value, StringComparison.Ordinal))
			{
				return Similarity.None;
			}
			return Similarity.IdenticalValueAndType;
		}

		public override string ToString()
		{
			return Value;
		}

		public override void AcceptSegmentElementVisitor(ISegmentElementVisitor visitor)
		{
			if (visitor == null)
			{
				throw new ArgumentNullException();
			}
			visitor.VisitText(this);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj.GetType() != GetType())
			{
				return false;
			}
			Text text = (Text)obj;
			return string.Equals(Value, text.Value, StringComparison.Ordinal);
		}

		public override int GetHashCode()
		{
			if (Value != null)
			{
				return Value.GetPlatformAgnosticHashCode();
			}
			return 0;
		}

		public override int GetWeakHashCode()
		{
			return GetHashCode();
		}
	}
}
