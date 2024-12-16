using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Resources;
using Sdl.LanguagePlatform.TranslationMemory;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	[DataContract]
	public class LanguageResource : PersistentObject
	{
		private CultureInfo _lazyCulture;

		private string _cultureName;

		[DataMember]
		public byte[] Data
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

		[XmlIgnore]
		public CultureInfo Culture
		{
			get
			{
				if (_lazyCulture == null && !string.IsNullOrEmpty(_cultureName))
				{
					_lazyCulture = CultureInfoExtensions.GetCultureInfo(_cultureName);
				}
				return _lazyCulture;
			}
			set
			{
				_lazyCulture = value;
				if (_lazyCulture != null)
				{
					_cultureName = _lazyCulture.Name;
				}
			}
		}

		[DataMember]
		public string CultureName
		{
			get
			{
				return _cultureName;
			}
			set
			{
				_cultureName = value;
				_lazyCulture = null;
			}
		}

		public LanguageResource()
		{
			Type = LanguageResourceType.Undefined;
		}
	}
}
