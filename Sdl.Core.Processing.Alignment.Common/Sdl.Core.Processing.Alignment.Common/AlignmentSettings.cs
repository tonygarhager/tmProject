using Sdl.LanguagePlatform.Core.Resources;
using System;
using System.Globalization;

namespace Sdl.Core.Processing.Alignment.Common
{
	public class AlignmentSettings
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

		public AlignmentMode AlignmentMode
		{
			get;
			set;
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

		public IBilingualDictionary BilingualDictionary
		{
			get;
			set;
		}

		public bool UpdateBilingualDictionary
		{
			get;
			set;
		}

		public string TmPath
		{
			get;
			set;
		}

		public AlignmentSettings(CultureInfo leftCulture, CultureInfo rightCulture)
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
