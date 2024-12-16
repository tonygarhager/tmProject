using Sdl.LanguagePlatform.Core;
using System;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	public class SystemFields
	{
		private DateTime _changeDate;

		private DateTime _useDate;

		private DateTime _creationDate;

		[DataMember]
		public DateTime CreationDate
		{
			get
			{
				return _creationDate;
			}
			set
			{
				_creationDate = DateTimeUtilities.Normalize(value);
			}
		}

		[DataMember]
		public DateTime ChangeDate
		{
			get
			{
				return _changeDate;
			}
			set
			{
				_changeDate = DateTimeUtilities.Normalize(value);
			}
		}

		[DataMember]
		public DateTime UseDate
		{
			get
			{
				return _useDate;
			}
			set
			{
				_useDate = DateTimeUtilities.Normalize(value);
			}
		}

		[DataMember]
		public string UseUser
		{
			get;
			set;
		}

		[DataMember]
		public int UseCount
		{
			get;
			set;
		}

		[DataMember]
		public string CreationUser
		{
			get;
			set;
		}

		[DataMember]
		public string ChangeUser
		{
			get;
			set;
		}

		public string CreationDisplayUsername
		{
			get;
			set;
		}

		public string ChangeDisplayUsername
		{
			get;
			set;
		}

		public string UseDisplayUsername
		{
			get;
			set;
		}

		public SystemFields()
		{
		}

		public SystemFields(SystemFields other)
		{
			_changeDate = other._changeDate;
			ChangeUser = (other.ChangeUser ?? string.Empty);
			_creationDate = other._creationDate;
			CreationUser = (other.CreationUser ?? string.Empty);
			_useDate = other._useDate;
			UseUser = (other.UseUser ?? string.Empty);
			UseCount = other.UseCount;
		}
	}
}
