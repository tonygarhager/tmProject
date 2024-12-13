using Sdl.Core.Api.DataAccess;
using Sdl.Core.LanguageProcessing;
using Sdl.Core.LanguageProcessing.Tokenization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Resources;
using Sdl.LanguagePlatform.Core.Segmentation;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	internal class RecognizerFactory
	{
		public Wordlist CreateWordList(byte[] data, bool ignoreComments)
		{
			if (data == null)
			{
				return null;
			}
			Wordlist wordlist = new Wordlist();
			wordlist.Load(data, ignoreComments);
			return wordlist;
		}

		internal Wordlist GetAbbreviations(string languageCode, IEnumerable<LanguageResourceEntity> languageResourceEntities)
		{
			LanguageResourceEntity languageResourceEntity = languageResourceEntities.FirstOrDefault((LanguageResourceEntity entity) => string.Equals(entity.CultureName, languageCode, StringComparison.OrdinalIgnoreCase) && entity.Type == LanguageResourceType.Abbreviations);
			return CreateWordList(languageResourceEntity);
		}

		internal Wordlist GetAbbreviations(IEnumerable<LanguageResourceEntity> languageResourceEntities)
		{
			LanguageResourceEntity languageResourceEntity = languageResourceEntities.FirstOrDefault((LanguageResourceEntity entity) => entity.Type == LanguageResourceType.Abbreviations);
			return CreateWordList(languageResourceEntity);
		}

		internal List<CurrencyFormat> GetCurrencySettings(string languageCode, EntityCollection<LanguageResourceEntity> entities)
		{
			LanguageResourceEntity entity = entities.FirstOrDefault((LanguageResourceEntity e) => e.Type == LanguageResourceType.CurrencyFSTEx && string.Equals(e.CultureName, languageCode, StringComparison.OrdinalIgnoreCase));
			return CreateCurrencyFSTEx(entity);
		}

		internal List<string> GetDateTimeFormats(string languageCode, IEnumerable<LanguageResourceEntity> entities, LanguageResourceType type)
		{
			LanguageResourceEntity languageResourceEntity = entities.FirstOrDefault((LanguageResourceEntity e) => e.Type == type && e.CultureName.Equals(languageCode, StringComparison.OrdinalIgnoreCase));
			if (!(languageResourceEntity != null))
			{
				return null;
			}
			return DateTimeFSTEx.FromBinary(languageResourceEntity.Data).Patterns;
		}

		internal Dictionary<string, CustomUnitDefinition> GetMesurementUnits(string languageCode, EntityCollection<LanguageResourceEntity> entities)
		{
			LanguageResourceEntity languageResourceEntity = entities.FirstOrDefault((LanguageResourceEntity e) => e.Type == LanguageResourceType.MeasurementFSTEx && e.CultureName.Equals(languageCode, StringComparison.OrdinalIgnoreCase));
			if (!(languageResourceEntity != null))
			{
				return null;
			}
			return MeasureFSTEx.FromBinary(languageResourceEntity.Data).UnitDefinitions;
		}

		internal List<SeparatorCombination> GetNumbersSeparators(string languageCode, EntityCollection<LanguageResourceEntity> entities)
		{
			LanguageResourceEntity entity = entities.FirstOrDefault((LanguageResourceEntity e) => e.Type == LanguageResourceType.NumberFSTEx && string.Equals(e.CultureName, languageCode, StringComparison.OrdinalIgnoreCase));
			return CreateNumberFSTEx(entity);
		}

		internal Wordlist GetOrdinalFollowers(string languageCode, IEnumerable<LanguageResourceEntity> languageResourceEntities)
		{
			LanguageResourceEntity languageResourceEntity = languageResourceEntities.FirstOrDefault((LanguageResourceEntity entity) => string.Equals(entity.CultureName, languageCode, StringComparison.OrdinalIgnoreCase) && entity.Type == LanguageResourceType.OrdinalFollowers);
			return CreateWordList(languageResourceEntity);
		}

		internal Wordlist GetOrdinalFollowers(IEnumerable<LanguageResourceEntity> languageResourceEntities)
		{
			LanguageResourceEntity languageResourceEntity = languageResourceEntities.FirstOrDefault((LanguageResourceEntity entity) => entity.Type == LanguageResourceType.OrdinalFollowers);
			return CreateWordList(languageResourceEntity);
		}

		internal SegmentationRules GetSegmentationRules(string languageCode, IEnumerable<LanguageResourceEntity> languageResourceEntities)
		{
			LanguageResourceEntity languageResourceEntity = languageResourceEntities.FirstOrDefault((LanguageResourceEntity entity) => string.Equals(entity.CultureName, languageCode, StringComparison.OrdinalIgnoreCase) && entity.Type == LanguageResourceType.SegmentationRules);
			if (languageResourceEntity != null)
			{
				return GetSegmentationRules(languageResourceEntity.Data, languageCode);
			}
			return null;
		}

		internal SegmentationRules GetSegmentationRules(byte[] data, string languageCode)
		{
			if (data != null)
			{
				SegmentationRules segmentationRules = null;
				using (MemoryStream reader = new MemoryStream(RemoveTrailingNullCharacters(data)))
				{
					return SegmentationRules.Load(reader, languageCode, null, keepListReferences: true);
				}
			}
			return null;
		}

		internal Wordlist GetVariables(string languageCode, IEnumerable<LanguageResourceEntity> languageResourceEntities)
		{
			LanguageResourceEntity languageResourceEntity = languageResourceEntities.FirstOrDefault((LanguageResourceEntity entity) => string.Equals(entity.CultureName, languageCode, StringComparison.OrdinalIgnoreCase) && entity.Type == LanguageResourceType.Variables);
			return CreateWordList(languageResourceEntity);
		}

		internal Wordlist GetVariables(IEnumerable<LanguageResourceEntity> languageResourceEntities)
		{
			LanguageResourceEntity languageResourceEntity = languageResourceEntities.FirstOrDefault((LanguageResourceEntity entity) => entity.Type == LanguageResourceType.Variables);
			return CreateWordList(languageResourceEntity);
		}

		private List<CurrencyFormat> CreateCurrencyFSTEx(LanguageResourceEntity entity)
		{
			if (!(entity != null))
			{
				return null;
			}
			return CurrencyFSTEx.FromBinary(entity.Data).CurrencyFormats;
		}

		private List<SeparatorCombination> CreateNumberFSTEx(LanguageResourceEntity entity)
		{
			if (entity != null)
			{
				return NumberFSTEx.FromBinary(entity.Data).SeparatorCombinations;
			}
			return null;
		}

		private Wordlist CreateWordList(LanguageResourceEntity languageResourceEntity)
		{
			if (languageResourceEntity != null)
			{
				return CreateWordList(languageResourceEntity.Data, ignoreComments: false);
			}
			return null;
		}

		private int LastIndexOfNonNullCharacter(byte[] data)
		{
			for (int num = data.Length - 1; num >= 0; num--)
			{
				if (data[num] != 0)
				{
					return num;
				}
			}
			return -1;
		}

		private byte[] RemoveTrailingNullCharacters(byte[] data0)
		{
			if (data0 == null)
			{
				return data0;
			}
			int num = LastIndexOfNonNullCharacter(data0);
			if (num == data0.Length - 1)
			{
				return data0;
			}
			byte[] array = new byte[num + 1];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = data0[i];
			}
			return array;
		}
	}
}
