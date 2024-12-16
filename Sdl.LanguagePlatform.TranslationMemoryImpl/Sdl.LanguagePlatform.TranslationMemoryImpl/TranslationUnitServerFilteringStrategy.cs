using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryImpl.Storage;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	internal class TranslationUnitServerFilteringStrategy : ITranslationUnitFilteringStrategy
	{
		private readonly CallContext _Context;

		internal TranslationUnitServerFilteringStrategy(CallContext context)
		{
			_Context = context;
		}

		List<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit> ITranslationUnitFilteringStrategy.GetTusFiltered(PersistentObjectToken translationMemoryId, RegularIterator iter, FieldDefinitions fieldDefinitions, bool includeContextContent, TextContextMatchType textContextMatchType, CultureInfo sourceCulture, CultureInfo targetCulture, bool usesIdContextMatch)
		{
			int num = iter.Forward ? (iter.PositionFrom = iter.PositionTo) : (iter.PositionTo = iter.PositionFrom);
			iter.ScannedTranslationUnits = 0;
			List<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit> list = new List<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit>();
			List<Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit> tusFiltered = _Context.Storage.GetTusFiltered(translationMemoryId.Id, iter.Filter, iter.PositionFrom, iter.MaxCount, iter.Forward, usesIdContextMatch, includeContextContent, textContextMatchType, sourceCulture, targetCulture);
			foreach (Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit item in tusFiltered)
			{
				int num2 = ++iter.ProcessedTranslationUnits;
				num2 = ++iter.ScannedTranslationUnits;
				if (iter.Forward)
				{
					iter.PositionTo = item.Id;
				}
				else
				{
					iter.PositionFrom = item.Id - 1;
				}
				list.Add(_Context.ResourceManager.GetTranslationUnit(item, fieldDefinitions, sourceCulture, targetCulture));
			}
			if (tusFiltered.Count > 0)
			{
				num = (iter.Forward ? (iter.PositionTo = tusFiltered[tusFiltered.Count - 1].Id) : (iter.PositionFrom = tusFiltered[0].Id - 1));
			}
			if (iter.ScannedTranslationUnits == 0)
			{
				int num2 = iter.PositionFrom = (iter.PositionTo = num);
			}
			return list;
		}

		public List<PersistentObjectToken> DeleteTusFiltered(PersistentObjectToken tmId, RegularIterator iter)
		{
			Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationMemory tm = _Context.Storage.GetTm(tmId.Id);
			return _Context.Storage.DeleteTusFiltered(tmId.Id, iter.Filter, iter.PositionFrom, iter.MaxCount, iter.Forward, tm.TextContextMatchType, tm.DataVersion > 0);
		}
	}
}
