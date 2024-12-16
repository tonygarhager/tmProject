using System;

namespace Sdl.Common.Licensing.Provider.Core.Models
{
	public class ActivatedLicense
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

		public string LicenseName
		{
			get;
			set;
		}

		public string LicenseVersion
		{
			get;
			set;
		}

		public string ProductKey
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

		public int ActivatedQuantity
		{
			get;
			set;
		}

		public bool IsQuantityUnlimited
		{
			get;
			set;
		}

		public string LicenseString
		{
			get;
			set;
		}
	}
}
