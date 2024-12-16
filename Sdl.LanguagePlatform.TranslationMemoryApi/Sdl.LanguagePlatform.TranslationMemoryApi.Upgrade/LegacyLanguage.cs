using System.Globalization;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	public class LegacyLanguage
	{
		private string _NativeCode;

		private string _NativeName;

		private CultureInfo _Culture;

		public string NativeCode
		{
			get
			{
				return _NativeCode;
			}
			set
			{
				_NativeCode = value;
			}
		}

		public string NativeName
		{
			get
			{
				return _NativeName;
			}
			set
			{
				_NativeName = value;
			}
		}

		public CultureInfo Culture
		{
			get
			{
				return _Culture;
			}
			set
			{
				_Culture = value;
			}
		}

		public LegacyLanguage()
		{
			_NativeCode = (_NativeName = null);
			_Culture = null;
		}

		public LegacyLanguage(CultureInfo ci)
		{
			_Culture = ci;
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
			LegacyLanguage legacyLanguage = obj as LegacyLanguage;
			if (legacyLanguage != null)
			{
				return object.Equals(_Culture, legacyLanguage._Culture);
			}
			return false;
		}

		public override int GetHashCode()
		{
			int num = 29;
			return num + ((_Culture != null) ? (171 * _Culture.GetHashCode()) : 0);
		}
	}
}
