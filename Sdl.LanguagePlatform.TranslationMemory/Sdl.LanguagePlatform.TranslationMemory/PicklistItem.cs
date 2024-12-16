using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	public class PicklistItem
	{
		[DataMember]
		public int? ID
		{
			get;
			set;
		}

		[DataMember]
		public string Name
		{
			get;
			set;
		}

		public PicklistItem()
		{
		}

		public PicklistItem(string name)
		{
			Name = name;
		}

		public PicklistItem(PicklistItem other)
		{
			if (other.Name != null)
			{
				Name = other.Name;
			}
			if (other.ID.HasValue)
			{
				ID = other.ID;
			}
		}
	}
}
