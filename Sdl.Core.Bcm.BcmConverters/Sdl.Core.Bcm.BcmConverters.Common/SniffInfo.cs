using Sdl.Core.Bcm.BcmModel;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmConverters.Common
{
	[DataContract]
	internal class SniffInfo : MetadataContainer
	{
		[DataMember(Name = "isSupported")]
		public bool IsSupported
		{
			get;
			set;
		}

		[DataMember(Name = "detectedEncoding")]
		public Tuple<string, DetectionLevel> DetectedEncoding
		{
			get;
			set;
		}

		[DataMember(Name = "detectedSourceLanguage")]
		public Tuple<string, DetectionLevel> DetectedSourceLanguage
		{
			get;
			set;
		}

		[DataMember(Name = "detectedTargetLanguage")]
		public Tuple<string, DetectionLevel> DetectedTargetLanguage
		{
			get;
			set;
		}

		[DataMember(Name = "suggestedTargetEncoding")]
		public EncodingCategory SuggestedTargetEncoding
		{
			get;
			set;
		}
	}
}
