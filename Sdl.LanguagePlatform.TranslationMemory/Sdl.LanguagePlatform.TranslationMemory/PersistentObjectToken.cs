using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	public sealed class PersistentObjectToken
	{
		[DataMember]
		public int Id
		{
			get;
			set;
		}

		[DataMember]
		public Guid Guid
		{
			get;
			set;
		}

		public PersistentObjectToken()
			: this(0, Guid.Empty)
		{
		}

		public PersistentObjectToken(int id, Guid guid)
		{
			Id = id;
			Guid = guid;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj.GetType() != GetType())
			{
				return false;
			}
			PersistentObjectToken persistentObjectToken = obj as PersistentObjectToken;
			if (persistentObjectToken == null)
			{
				return false;
			}
			if (Guid.Equals(persistentObjectToken.Guid))
			{
				return Id == persistentObjectToken.Id;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (Guid.GetHashCode() << 5) ^ Id.GetHashCode();
		}

		public override string ToString()
		{
			return "(" + Id.ToString(CultureInfo.InvariantCulture) + ", " + Guid.ToString("", CultureInfo.InvariantCulture) + ")";
		}
	}
}
