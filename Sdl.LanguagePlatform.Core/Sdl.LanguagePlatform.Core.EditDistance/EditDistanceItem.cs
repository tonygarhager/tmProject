using System;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.Core.EditDistance
{
	[DataContract]
	public struct EditDistanceItem
	{
		[DataMember]
		public int Source
		{
			get;
			set;
		}

		[DataMember]
		public int Target
		{
			get;
			set;
		}

		[DataMember]
		public EditOperation Operation
		{
			get;
			set;
		}

		[DataMember]
		public EditDistanceResolution Resolution
		{
			get;
			set;
		}

		[DataMember]
		public double Costs
		{
			get;
			set;
		}

		[DataMember]
		public int MoveSourceTarget
		{
			get;
			set;
		}

		[DataMember]
		public int MoveTargetSource
		{
			get;
			set;
		}

		public override string ToString()
		{
			switch (Operation)
			{
			case EditOperation.Identity:
				return $"=({Source},{Target})";
			case EditOperation.Change:
				return $"c({Source},{Target})";
			case EditOperation.Move:
				return $"m({Source},{Target},{MoveSourceTarget},{MoveTargetSource})";
			case EditOperation.Insert:
				return $"i({Source},{Target})";
			case EditOperation.Delete:
				return $"d({Source},{Target})";
			default:
				throw new Exception("Invalid case label");
			}
		}
	}
}
