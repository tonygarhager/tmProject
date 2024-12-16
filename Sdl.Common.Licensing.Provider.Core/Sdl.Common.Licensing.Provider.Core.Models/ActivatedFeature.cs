using System;

namespace Sdl.Common.Licensing.Provider.Core.Models
{
	public class ActivatedFeature
	{
		public DateTime ActivationDateTime
		{
			get;
			set;
		}

		public DateTime StartDate
		{
			get;
			set;
		}

		public DateTime EndDate
		{
			get;
			set;
		}

		public string AID
		{
			get;
			set;
		}

		public bool IsAdditive
		{
			get;
			set;
		}

		public string FeatureName
		{
			get;
			set;
		}

		public string Version
		{
			get;
			set;
		}

		public int Quantity
		{
			get;
			set;
		}

		public int TotalQuantity
		{
			get;
			set;
		}

		public int RemainingQuantity
		{
			get;
			set;
		}

		public string LicenseHash
		{
			get;
			set;
		}

		public string LicenseStorage
		{
			get;
			set;
		}

		public string Id
		{
			get;
			set;
		}

		public string ProductName
		{
			get;
			set;
		}

		public string ProductKey
		{
			get;
			set;
		}
	}
}
