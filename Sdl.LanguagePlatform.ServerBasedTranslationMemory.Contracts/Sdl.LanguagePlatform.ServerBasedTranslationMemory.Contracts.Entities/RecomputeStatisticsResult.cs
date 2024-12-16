using System;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities
{
	[DataContract(Namespace = "http://sdl.com/languageplatform/2010", Name = "RecomputeStatisticsResult")]
	public class RecomputeStatisticsResult : IExtensibleDataObject
	{
		private ExtensionDataObject _extensionDataObject;

		public ExtensionDataObject ExtensionData
		{
			get
			{
				return _extensionDataObject;
			}
			set
			{
				_extensionDataObject = value;
			}
		}

		[DataMember]
		public DateTime LastRecomputeDate
		{
			get;
			set;
		}

		[DataMember]
		public int LastRecomputeSize
		{
			get;
			set;
		}
	}
}
