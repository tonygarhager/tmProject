using Newtonsoft.Json;
using Sdl.Core.Bcm.BcmModel.Common;
using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmModel
{
	[DataContract]
	public class Paragraph : MarkupDataContainer
	{
		public override ParagraphUnit ParentParagraphUnit
		{
			get;
			set;
		}

		[JsonProperty("type")]
		protected override string Type
		{
			get
			{
				return "paragraph";
			}
			set
			{
			}
		}

		public Paragraph()
		{
		}

		public Paragraph(string id)
			: base(id)
		{
		}

		public Paragraph(Segment segment)
		{
			Add(segment);
		}

		public override void AcceptVisitor(BcmVisitor visitor)
		{
			visitor.VisitParagraph(this);
		}

		public new Paragraph Clone()
		{
			return base.Clone() as Paragraph;
		}

		public new Paragraph UniqueClone()
		{
			return base.UniqueClone() as Paragraph;
		}
	}
}
