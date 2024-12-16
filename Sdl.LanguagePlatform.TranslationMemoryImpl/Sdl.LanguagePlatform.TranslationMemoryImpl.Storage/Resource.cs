using Sdl.LanguagePlatform.Core.Resources;
using Sdl.LanguagePlatform.TranslationMemory;
using System;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl.Storage
{
	internal class Resource : DbObject
	{
		public LanguageResourceType Type
		{
			get;
			set;
		}

		public string Language
		{
			get;
			set;
		}

		public byte[] Data
		{
			get;
			set;
		}

		public Resource(LanguageResourceType type, string language)
			: base(0, Guid.NewGuid())
		{
			Type = type;
			Language = language;
		}

		internal Resource(LanguageResource rsc)
			: base(rsc.ResourceId.Id, rsc.ResourceId.Guid)
		{
			Type = rsc.Type;
			Language = rsc.Culture?.Name;
			Data = rsc.Data;
		}

		internal Resource(int id, Guid guid, LanguageResourceType type, string language, byte[] data)
			: this(type, language)
		{
			base.Id = id;
			base.Guid = guid;
			Data = data;
		}

		internal LanguageResource ToLanguageResource()
		{
			return new LanguageResource
			{
				CultureName = Language,
				Type = Type,
				Data = Data,
				ResourceId = new PersistentObjectToken(base.Id, base.Guid)
			};
		}
	}
}
