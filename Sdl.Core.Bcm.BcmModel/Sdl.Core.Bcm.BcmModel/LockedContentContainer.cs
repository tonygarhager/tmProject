using Newtonsoft.Json;
using Sdl.Core.Bcm.BcmModel.Common;
using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmModel
{
	[DataContract]
	public class LockedContentContainer : MarkupDataContainer
	{
		[JsonProperty("type")]
		protected override string Type
		{
			get
			{
				return "locked";
			}
			set
			{
			}
		}

		public override void AcceptVisitor(BcmVisitor visitor)
		{
			visitor.VisitLockedContentContainer(this);
		}

		public new LockedContentContainer Clone()
		{
			return base.Clone() as LockedContentContainer;
		}

		public new LockedContentContainer UniqueClone()
		{
			return base.UniqueClone() as LockedContentContainer;
		}
	}
}
