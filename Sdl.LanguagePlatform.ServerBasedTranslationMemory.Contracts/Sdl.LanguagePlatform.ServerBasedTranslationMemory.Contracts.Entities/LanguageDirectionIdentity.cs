using Sdl.Core.Api.DataAccess;
using System;

namespace Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities
{
	public struct LanguageDirectionIdentity
	{
		public Identity Identity
		{
			get;
			set;
		}

		public Guid TranslationMemoryId
		{
			get;
			set;
		}

		public string Source
		{
			get;
			set;
		}

		public string Target
		{
			get;
			set;
		}
	}
}
