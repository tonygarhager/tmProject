using System.Globalization;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	/// <summary>
	/// Represents a language within a legacy translation memory. Provides a way to keep track of the original language code
	/// and name, as well as the associated <see cref="!:CultureInfo" /> object.
	/// </summary>
	public class LegacyLanguage
	{
		private string _NativeCode;

		private string _NativeName;

		private CultureInfo _Culture;

		/// <summary>
		/// Gets or sets the native language code, as used by the legacy translation memory.
		/// </summary>
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

		/// <summary>
		/// Gets or sets the native language name, as used by the legacy translation memory.
		/// </summary>
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

		/// <summary>
		/// Gets or sets the <see cref="!:CultureInfo" /> object that corresponds to this legacy language.
		/// </summary>
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

		/// <summary>
		/// Initializes a new instance with default values.
		/// </summary>
		public LegacyLanguage()
		{
			_NativeCode = (_NativeName = null);
			_Culture = null;
		}

		/// <summary>
		/// Constructor with culture.
		/// </summary>
		/// <param name="ci"></param>
		public LegacyLanguage(CultureInfo ci)
		{
			_Culture = ci;
		}

		/// <summary>
		/// <see cref="M:System.Object.Equals(object)" />
		/// </summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>true if the specified object is equal to the current object;
		/// otherwise, false.
		/// </returns>
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

		/// <summary>
		/// <see cref="M:System.Object.GetHashCode" />
		/// </summary>
		/// <returns>A hash code for this object</returns>
		public override int GetHashCode()
		{
			int num = 29;
			return num + ((_Culture != null) ? (171 * _Culture.GetHashCode()) : 0);
		}
	}
}
