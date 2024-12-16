using System;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl.Storage
{
	internal class PickValue : DbObject, IComparable
	{
		private string _Value;

		public string Value
		{
			get
			{
				return _Value;
			}
			set
			{
				_Value = value;
			}
		}

		public PickValue(string value)
			: this(0, Guid.NewGuid(), value)
		{
		}

		internal PickValue(int id, Guid guid, string value)
			: base(id, guid)
		{
			Value = value;
		}

		public int CompareTo(object obj)
		{
			PickValue pickValue = (PickValue)obj;
			return string.Compare(Value, pickValue.Value, StringComparison.InvariantCultureIgnoreCase);
		}
	}
}
