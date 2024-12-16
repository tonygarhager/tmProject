using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryImpl.Storage;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	internal class TranslationUnitClientFilteringStrategy : ITranslationUnitFilteringStrategy
	{
		private int _queueBatchSizeMultiplier = 5;

		private int _evaluatorThreadCount = 3;

		private readonly bool _fileBased;

		private readonly CallContext _Context;

		internal TranslationUnitClientFilteringStrategy(CallContext context, bool fileBased)
		{
			_fileBased = fileBased;
			if (_fileBased)
			{
				_queueBatchSizeMultiplier = 1;
				_evaluatorThreadCount = 1;
			}
			_Context = context;
		}

		public List<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit> GetTusFiltered(PersistentObjectToken translationMemoryId, RegularIterator iter, FieldDefinitions fieldDefinitions, bool includeContextContent, TextContextMatchType textContextMatchType, CultureInfo sourceCulture, CultureInfo targetCulture, bool usesIdContextMatch)
		{
			SortedList<int, Sdl.LanguagePlatform.TranslationMemory.TranslationUnit> result = new SortedList<int, Sdl.LanguagePlatform.TranslationMemory.TranslationUnit>();
			bool done = false;
			int pos;
			if (iter.Forward)
			{
				iter.PositionFrom = (pos = iter.PositionTo);
			}
			else
			{
				iter.PositionTo = (pos = iter.PositionFrom);
			}
			iter.ScannedTranslationUnits = 0;
			if (iter.Filter == null)
			{
				_queueBatchSizeMultiplier = 1;
				_evaluatorThreadCount = 1;
			}
			ConcurrentQueue<Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit> queue = new ConcurrentQueue<Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit>();
			Task task = Task.Run(async delegate
			{
				while (true)
				{
					if (queue.Count >= iter.MaxCount * _queueBatchSizeMultiplier)
					{
						await Task.Delay(1);
					}
					else
					{
						List<Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit> tus = _Context.Storage.GetTus(translationMemoryId.Id, pos, iter.MaxCount * _queueBatchSizeMultiplier, iter.Forward, usesIdContextMatch, includeContextContent, textContextMatchType, sourceCulture, targetCulture);
						if (tus.Count > 0)
						{
							if (iter.Forward)
							{
								pos = tus[tus.Count - 1].Id;
							}
							else
							{
								pos = tus[tus.Count - 1].Id - 1;
							}
						}
						else
						{
							done = true;
						}
						foreach (Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit item in tus)
						{
							if (iter.Forward)
							{
								iter.PositionTo = item.Id;
							}
							else
							{
								iter.PositionFrom = item.Id - 1;
							}
							queue.Enqueue(item);
						}
						if (_fileBased || iter.Filter == null)
						{
							break;
						}
					}
					if (done)
					{
						return;
					}
				}
				done = true;
			});
			List<Task> list = new List<Task>();
			for (int i = 0; i < _evaluatorThreadCount; i++)
			{
				list.Add(Task.Run(async delegate
				{
					while (!done || queue.Count > 0)
					{
						if (queue.TryDequeue(out Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit result2))
						{
							Sdl.LanguagePlatform.TranslationMemory.TranslationUnit translationUnit = _Context.ResourceManager.GetTranslationUnit(result2, fieldDefinitions, sourceCulture, targetCulture);
							lock (result)
							{
								int num4 = ++iter.ProcessedTranslationUnits;
								num4 = ++iter.ScannedTranslationUnits;
								if (iter.Filter == null || iter.Filter.Evaluate(translationUnit))
								{
									result.Add(result2.Id, translationUnit);
								}
								if (result.Count >= iter.MaxCount)
								{
									done = true;
								}
							}
						}
						else
						{
							await Task.Delay(10);
						}
					}
				}));
			}
			task.Wait();
			foreach (Task item2 in list)
			{
				item2.Wait();
			}
			if (iter.ScannedTranslationUnits == 0 && result.Count == 0)
			{
				int num3 = iter.PositionFrom = (iter.PositionTo = pos);
			}
			if (result.Count <= 0)
			{
				return new List<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit>();
			}
			if (iter.Forward)
			{
				List<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit> list2 = result.Values.Take(iter.MaxCount).ToList();
				iter.PositionTo = list2.Last().ResourceId.Id;
				return list2;
			}
			List<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit> list3 = result.Values.Reverse().Take(iter.MaxCount).Reverse()
				.ToList();
			iter.PositionFrom = list3.First().ResourceId.Id - 1;
			return list3;
		}

		public List<PersistentObjectToken> DeleteTusFiltered(PersistentObjectToken tmId, RegularIterator iter)
		{
			Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationMemory tm = _Context.Storage.GetTm(tmId.Id);
			if (iter.PositionFrom == 0 && _Context.IsFilebasedTm)
			{
				OptimizeStorageForExport();
			}
			List<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit> tusFiltered = GetTusFiltered(tmId, iter, null, includeContextContent: false, TextContextMatchType.PrecedingAndFollowingSource, tm.LanguageDirection.SourceCulture, tm.LanguageDirection.TargetCulture, tm.IdContextMatch);
			if (tusFiltered == null || tusFiltered.Count <= 0)
			{
				return new List<PersistentObjectToken>();
			}
			return _Context.Storage.DeleteTus(tmId.Id, tusFiltered.Select((Sdl.LanguagePlatform.TranslationMemory.TranslationUnit tu) => tu.ResourceId).ToList(), tm.TextContextMatchType, tm.DataVersion > 0);
		}

		private void OptimizeStorageForExport()
		{
			(_Context.Storage as SqliteStorage)?.Optimize();
		}
	}
}
