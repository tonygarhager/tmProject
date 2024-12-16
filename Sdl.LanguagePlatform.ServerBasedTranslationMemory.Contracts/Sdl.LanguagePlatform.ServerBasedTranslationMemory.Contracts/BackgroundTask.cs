using System;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts
{
	[DataContract(Namespace = "http://sdl.com/languageplatform/2010", Name = "BackgroundTask")]
	public class BackgroundTask : IEquatable<BackgroundTask>, IExtensibleDataObject
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
		public Guid UniqueId
		{
			get;
			set;
		}

		[DataMember]
		public string TaskType
		{
			get;
			set;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			return Equals(obj as BackgroundTask);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public bool Equals(BackgroundTask other)
		{
			if (other == null)
			{
				return false;
			}
			if (UniqueId == other.UniqueId)
			{
				return TaskType.Equals(other.TaskType, StringComparison.OrdinalIgnoreCase);
			}
			return false;
		}
	}
}
