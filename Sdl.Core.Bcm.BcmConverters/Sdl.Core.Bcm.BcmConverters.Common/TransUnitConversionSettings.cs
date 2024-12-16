using System;

namespace Sdl.Core.Bcm.BcmConverters.Common
{
	public class TransUnitConversionSettings
	{
		public TransUnitConversionType ConversionType
		{
			get;
			set;
		}

		[Obsolete("Do not use, will be removed.")]
		public bool ProcessComments
		{
			get;
			set;
		}

		public bool IncludeTokens
		{
			get;
			set;
		}

		public bool IncludeUserNameSystemFields
		{
			get;
			set;
		}

		public bool IncludeAlignmentData
		{
			get;
			set;
		}

		public TransUnitConversionSettings()
		{
			ConversionType = TransUnitConversionType.Bilingual;
			ProcessComments = true;
			IncludeTokens = false;
			IncludeAlignmentData = false;
			IncludeUserNameSystemFields = false;
		}

		internal static TransUnitConversionSettings BuildDefault()
		{
			return new TransUnitConversionSettings();
		}
	}
}
