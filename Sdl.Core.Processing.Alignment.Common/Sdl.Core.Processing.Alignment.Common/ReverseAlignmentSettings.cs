using Sdl.LanguagePlatform.Core.Resources;
using System;
using System.Globalization;

namespace Sdl.Core.Processing.Alignment.Common
{
	public class ReverseAlignmentSettings
	{
		public CultureInfo LeftCulture
		{
			get;
			private set;
		}

		public CultureInfo RightCulture
		{
			get;
			private set;
		}

		public IResourceDataAccessor ResourceDataAccessor
		{
			get;
			set;
		}

		public byte MinimumAlignmentQuality
		{
			get;
			set;
		}

		public string ProjectId
		{
			get;
			set;
		}

		public ReverseAlignmentSettings(CultureInfo leftCulture, CultureInfo rightCulture)
		{
			if (leftCulture == null)
			{
				throw new ArgumentNullException("leftCulture");
			}
			if (rightCulture == null)
			{
				throw new ArgumentNullException("rightCulture");
			}
			LeftCulture = leftCulture;
			RightCulture = rightCulture;
		}
	}
}
