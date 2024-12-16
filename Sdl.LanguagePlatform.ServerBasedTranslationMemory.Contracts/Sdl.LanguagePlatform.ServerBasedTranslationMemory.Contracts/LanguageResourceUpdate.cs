using Sdl.LanguagePlatform.Core.Resources;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts
{
	[DataContract(Namespace = "http://sdl.com/languageplatform/2010", Name = "LanguageResourceUpdate")]
	public class LanguageResourceUpdate : IExtensibleDataObject
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
		public LanguageResourceUpdateType UpdateType
		{
			get;
			set;
		}

		[DataMember]
		public string CultureName
		{
			get;
			set;
		}

		[DataMember]
		public LanguageResourceType Type
		{
			get;
			set;
		}

		[DataMember]
		public byte[] Data
		{
			get;
			set;
		}
	}
}
