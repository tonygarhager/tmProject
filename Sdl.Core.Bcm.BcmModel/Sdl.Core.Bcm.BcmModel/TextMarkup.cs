using Newtonsoft.Json;
using Sdl.Core.Bcm.BcmModel.Common;
using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmModel
{
	[DataContract]
	public class TextMarkup : MarkupData
	{
		[DataMember(Name = "text")]
		public string Text
		{
			get;
			set;
		}

		[JsonProperty("type")]
		protected override string Type
		{
			get
			{
				return "text";
			}
			set
			{
			}
		}

		public TextMarkup()
		{
		}

		public TextMarkup(string text)
		{
			Text = text;
		}

		public override void AcceptVisitor(BcmVisitor visitor)
		{
			visitor.VisitText(this);
		}

		public override string ToString()
		{
			return Text;
		}

		public override bool Equals(MarkupData other)
		{
			if (other == null)
			{
				return false;
			}
			if (this == other)
			{
				return true;
			}
			TextMarkup textMarkup = other as TextMarkup;
			if (textMarkup == null)
			{
				return false;
			}
			if (base.Equals(textMarkup))
			{
				return string.Equals(Text, textMarkup.Text);
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (this == obj)
			{
				return true;
			}
			if (obj.GetType() != GetType())
			{
				return false;
			}
			return Equals((MarkupData)obj);
		}

		public override int GetHashCode()
		{
			return (base.GetHashCode() * 397) ^ ((Text != null) ? Text.GetHashCode() : 0);
		}

		public new TextMarkup Clone()
		{
			return base.Clone() as TextMarkup;
		}
	}
}
