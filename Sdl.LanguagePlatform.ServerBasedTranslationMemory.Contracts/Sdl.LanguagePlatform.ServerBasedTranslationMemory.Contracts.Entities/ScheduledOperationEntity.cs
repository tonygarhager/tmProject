using System;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities
{
	[DataContract(Namespace = "http://sdl.com/languageplatform/2010", Name = "ScheduledOperation")]
	public class ScheduledOperationEntity : IExtensibleDataObject
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
		public Guid? WorkItemUniqueId
		{
			get;
			set;
		}

		[DataMember]
		public int Status
		{
			get;
			set;
		}

		[DataMember]
		public string ErrorMessage
		{
			get;
			set;
		}

		[DataMember]
		public int PercentComplete
		{
			get;
			set;
		}

		[DataMember]
		public DateTime? QueuedOn
		{
			get;
			set;
		}

		[DataMember]
		public DateTime? StartedOn
		{
			get;
			set;
		}

		[DataMember]
		public DateTime? CompletedOn
		{
			get;
			set;
		}

		[DataMember]
		public DateTime? CancelledOn
		{
			get;
			set;
		}
	}
}
