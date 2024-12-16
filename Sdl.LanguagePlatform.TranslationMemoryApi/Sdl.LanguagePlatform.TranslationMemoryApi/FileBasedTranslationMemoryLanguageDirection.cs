using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemory.EditScripts;
using Sdl.LanguagePlatform.TranslationMemoryApi.Helpers;
using Sdl.LanguagePlatform.TranslationMemoryImpl;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.ServiceModel;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public class FileBasedTranslationMemoryLanguageDirection : ISubsegmentTranslationMemoryLanguageDirection, ITranslationMemoryLanguageDirection, ITranslationProviderLanguageDirection, IAdvancedContextTranslationMemoryLanguageDirection
	{
		private AbstractLocalTranslationMemory _tm;

		public ITranslationMemory TranslationProvider => _tm;

		ITranslationProvider ITranslationProviderLanguageDirection.TranslationProvider => _tm;

		public LanguagePair LanguageDirection => _tm.Setup.LanguageDirection;

		public bool CanReverseLanguageDirection => _tm.Setup.CanReverseLanguageDirection;

		public CultureInfo SourceLanguage => LanguageDirection.SourceCulture;

		public CultureInfo TargetLanguage => LanguageDirection.TargetCulture;

		internal FileBasedTranslationMemoryLanguageDirection(AbstractLocalTranslationMemory tm)
		{
			_tm = tm;
		}

		public int DeleteAllTranslationUnits()
		{
			try
			{
				return _tm.Service.DeleteAllTranslationUnits(_tm.Container, _tm.Setup.ResourceId);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		public bool DeleteTranslationUnit(PersistentObjectToken translationUnitId)
		{
			try
			{
				return _tm.Service.DeleteTranslationUnit(_tm.Container, _tm.Setup.ResourceId, translationUnitId);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		public int DeleteTranslationUnits(PersistentObjectToken[] translationUnitIds)
		{
			try
			{
				return _tm.Service.DeleteTranslationUnits(_tm.Container, _tm.Setup.ResourceId, translationUnitIds);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		public int DeleteTranslationUnitsWithIterator(ref RegularIterator iterator)
		{
			if (iterator == null)
			{
				throw new ArgumentNullException("iterator");
			}
			try
			{
				return _tm.Service.DeleteTranslationUnitsWithIterator(_tm.Container, _tm.Setup.ResourceId, ref iterator);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		public int EditTranslationUnits(EditScript editScript, EditUpdateMode updateMode)
		{
			return EditTranslationUnits(editScript, updateMode, (FilterExpression)null);
		}

		public int EditTranslationUnits(EditScript editScript, EditUpdateMode updateMode, FilterExpression filter)
		{
			if (editScript == null)
			{
				throw new ArgumentNullException("editScript");
			}
			try
			{
				int num = 0;
				RegularIterator iterator = new RegularIterator();
				iterator.Filter = filter;
				iterator.Forward = true;
				int num2 = 0;
				do
				{
					num2 = _tm.Service.EditTranslationUnitsWithIterator(_tm.Container, _tm.Setup.ResourceId, editScript, updateMode, ref iterator);
					num += num2;
				}
				while (num2 > 0);
				return num;
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		public int EditTranslationUnitsWithIterator(EditScript editScript, EditUpdateMode updateMode, ref RegularIterator iterator)
		{
			if (editScript == null)
			{
				throw new ArgumentNullException("editScript");
			}
			if (iterator == null)
			{
				throw new ArgumentNullException("iterator");
			}
			try
			{
				return _tm.Service.EditTranslationUnitsWithIterator(_tm.Container, _tm.Setup.ResourceId, editScript, updateMode, ref iterator);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		public bool ReindexTranslationUnits(ref RegularIterator iterator)
		{
			try
			{
				return _tm.Service.ReindexTranslationUnits(_tm.Container, _tm.Setup.ResourceId, ref iterator);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		public TranslationUnit[] PreviewEditTranslationUnits(EditScript editScript, FilterExpression filter)
		{
			if (editScript == null)
			{
				throw new ArgumentNullException("editScript");
			}
			try
			{
				TranslationUnit[] array = null;
				RegularIterator iterator = new RegularIterator();
				iterator.Filter = filter;
				iterator.Forward = true;
				int num = 0;
				do
				{
					array = _tm.Service.PreviewEditTranslationUnitsWithIterator(_tm.Container, _tm.Setup.ResourceId, editScript, ref iterator);
				}
				while (array == null || array.Length == 0);
				return array;
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		public int EditTranslationUnits(EditScript editScript, EditUpdateMode updateMode, PersistentObjectToken[] translationUnitIds)
		{
			if (editScript == null)
			{
				throw new ArgumentNullException("editScript");
			}
			try
			{
				return _tm.Service.EditTranslationUnits(_tm.Container, _tm.Setup.ResourceId, editScript, updateMode, translationUnitIds);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		public ImportResult AddTranslationUnit(TranslationUnit translationUnit, ImportSettings settings)
		{
			try
			{
				return _tm.Service.AddTranslationUnit(_tm.Container, _tm.Setup.ResourceId, translationUnit, settings);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		public ImportResult[] AddTranslationUnits(TranslationUnit[] translationUnits, ImportSettings settings)
		{
			try
			{
				return _tm.Service.AddTranslationUnits(_tm.Container, _tm.Setup.ResourceId, translationUnits, settings);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		public ImportResult[] AddOrUpdateTranslationUnits(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings)
		{
			try
			{
				return _tm.Service.AddOrUpdateTranslationUnits(_tm.Container, _tm.Setup.ResourceId, translationUnits, previousTranslationHashes, settings);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		public ImportResult[] AddTranslationUnitsMasked(TranslationUnit[] translationUnits, ImportSettings settings, bool[] mask)
		{
			try
			{
				return _tm.Service.AddTranslationUnitsMasked(_tm.Container, _tm.Setup.ResourceId, translationUnits, settings, mask);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		public ImportResult[] AddOrUpdateTranslationUnitsMasked(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings, bool[] mask)
		{
			try
			{
				return _tm.Service.AddOrUpdateTranslationUnitsMasked(_tm.Container, _tm.Setup.ResourceId, translationUnits, previousTranslationHashes, settings, mask);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		public TranslationUnit GetTranslationUnit(PersistentObjectToken translationUnitId)
		{
			try
			{
				return _tm.Service.GetTranslationUnit(_tm.Container, _tm.Setup.ResourceId, translationUnitId);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		public TranslationUnit[] GetTranslationUnits(ref RegularIterator iterator)
		{
			try
			{
				return _tm.Service.GetTranslationUnits(_tm.Container, _tm.Setup.ResourceId, ref iterator);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		public TranslationUnit[] GetTranslationUnitsWithContextContent(ref RegularIterator iterator)
		{
			try
			{
				return _tm.Service.GetTranslationUnitsWithContextContent(_tm.Container, _tm.Setup.ResourceId, ref iterator);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		public TranslationUnit[] GetDuplicateTranslationUnits(ref DuplicateIterator iterator)
		{
			try
			{
				return _tm.Service.GetDuplicateTranslationUnits(_tm.Container, _tm.Setup.ResourceId, ref iterator);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		public int GetTranslationUnitCount()
		{
			try
			{
				return _tm.Service.GetTuCount(_tm.Container, _tm.Setup.ResourceId);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		public SearchResults SearchSegment(SearchSettings settings, Segment segment)
		{
			try
			{
				return _tm.Service.SearchSegment(_tm.Container, _tm.Setup.ResourceId, settings, segment);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		public SegmentAndSubsegmentSearchResults SearchSegment(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, Segment segment)
		{
			try
			{
				return (_tm.Service as ISubsegmentTranslationMemoryService)?.SearchSegment(_tm.Container, _tm.Setup.ResourceId, settings, subsegmentSettings, condition, segment);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		public SubsegmentSearchResultsCollection SubsegmentSearchSegment(SubsegmentSearchSettings settings, Segment segment)
		{
			try
			{
				return (_tm.Service as ISubsegmentTranslationMemoryService)?.SubsegmentSearchSegment(_tm.Container, _tm.Setup.ResourceId, settings, segment);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		public SubsegmentSearchResultsCollection[] SubsegmentSearchSegments(SubsegmentSearchSettings settings, Segment[] segments)
		{
			try
			{
				return (_tm.Service as ISubsegmentTranslationMemoryService)?.SubsegmentSearchSegments(_tm.Container, _tm.Setup.ResourceId, settings, segments);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		public SearchResults[] SearchSegments(SearchSettings settings, Segment[] segments)
		{
			return SearchSegmentsMasked(settings, segments, null);
		}

		public SegmentAndSubsegmentSearchResults[] SearchSegments(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, Segment[] segments)
		{
			return SearchSegmentsMasked(settings, subsegmentSettings, condition, segments, null);
		}

		public SearchResults[] SearchSegmentsMasked(SearchSettings settings, Segment[] segments, bool[] mask)
		{
			try
			{
				return _tm.Service.SearchSegmentsMasked(_tm.Container, _tm.Setup.ResourceId, settings, segments, mask);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		public SegmentAndSubsegmentSearchResults[] SearchSegmentsMasked(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, Segment[] segments, bool[] mask)
		{
			try
			{
				return (_tm.Service as ISubsegmentTranslationMemoryService)?.SearchSegmentsMasked(_tm.Container, _tm.Setup.ResourceId, settings, subsegmentSettings, condition, segments, mask);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		public SearchResults SearchText(SearchSettings settings, string segment)
		{
			try
			{
				return _tm.Service.SearchText(_tm.Container, _tm.Setup.ResourceId, settings, segment);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		public SearchResults SearchTranslationUnit(SearchSettings settings, TranslationUnit translationUnit)
		{
			try
			{
				return _tm.Service.SearchTranslationUnit(_tm.Container, _tm.Setup.ResourceId, settings, translationUnit);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		public SegmentAndSubsegmentSearchResults SearchTranslationUnit(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, TranslationUnit translationUnit)
		{
			try
			{
				return (_tm.Service as ISubsegmentTranslationMemoryService)?.SearchTranslationUnit(_tm.Container, _tm.Setup.ResourceId, settings, subsegmentSettings, condition, translationUnit);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		public SearchResults[] SearchTranslationUnits(SearchSettings settings, TranslationUnit[] translationUnits)
		{
			return SearchTranslationUnitsMasked(settings, translationUnits, null);
		}

		public SegmentAndSubsegmentSearchResults[] SearchTranslationUnits(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, TranslationUnit[] translationUnits)
		{
			return SearchTranslationUnitsMasked(settings, subsegmentSettings, condition, translationUnits, null);
		}

		public virtual SearchResults[] SearchTranslationUnitsMasked(SearchSettings settings, TranslationUnit[] translationUnits, bool[] mask)
		{
			try
			{
				return _tm.Service.SearchTranslationUnitsMasked(_tm.Container, _tm.Setup.ResourceId, settings, translationUnits, mask);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		public virtual SegmentAndSubsegmentSearchResults[] SearchTranslationUnitsMasked(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, TranslationUnit[] translationUnits, bool[] mask)
		{
			try
			{
				ISubsegmentTranslationMemoryService subsegmentTranslationMemoryService = _tm.Service as ISubsegmentTranslationMemoryService;
				if (subsegmentTranslationMemoryService == null)
				{
					return null;
				}
				SearchSettings settings2 = FileBasedTMSettingsCloner.CloneSettings(settings);
				return subsegmentTranslationMemoryService.SearchTranslationUnitsMasked(_tm.Container, _tm.Setup.ResourceId, settings2, subsegmentSettings, condition, translationUnits, mask);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		public ImportResult UpdateTranslationUnit(TranslationUnit translationUnit)
		{
			try
			{
				return _tm.Service.UpdateTranslationUnit(_tm.Container, _tm.Setup.ResourceId, translationUnit);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		public ImportResult[] UpdateTranslationUnits(TranslationUnit[] translationUnits)
		{
			try
			{
				return _tm.Service.UpdateTranslationUnits(_tm.Container, _tm.Setup.ResourceId, translationUnits);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		public ImportResult[] UpdateTranslationUnitsMasked(TranslationUnit[] translationUnits, bool[] mask)
		{
			try
			{
				return _tm.Service.UpdateTranslationUnitsMasked(_tm.Container, _tm.Setup.ResourceId, translationUnits, mask);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		public bool ApplyFieldsToTranslationUnit(FieldValues values, bool overwrite, PersistentObjectToken translationUnitId)
		{
			try
			{
				return _tm.Service.ApplyFieldsToTranslationUnit(_tm.Container, _tm.Setup.ResourceId, values, overwrite, translationUnitId);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		public int ApplyFieldsToTranslationUnits(FieldValues values, bool overwrite, PersistentObjectToken[] translationUnitIds)
		{
			try
			{
				return _tm.Service.ApplyFieldsToTranslationUnits(_tm.Container, _tm.Setup.ResourceId, values, overwrite, translationUnitIds);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		public TranslationUnit[] PreviewEditTranslationUnitsWithIterator(EditScript editScript, ref RegularIterator iterator)
		{
			try
			{
				return _tm.Service.PreviewEditTranslationUnitsWithIterator(_tm.Container, _tm.Setup.ResourceId, editScript, ref iterator);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		public override bool Equals(object obj)
		{
			FileBasedTranslationMemoryLanguageDirection fileBasedTranslationMemoryLanguageDirection = obj as FileBasedTranslationMemoryLanguageDirection;
			if (fileBasedTranslationMemoryLanguageDirection == null)
			{
				return false;
			}
			if (fileBasedTranslationMemoryLanguageDirection.TranslationProvider.Uri.Equals(TranslationProvider.Uri))
			{
				return fileBasedTranslationMemoryLanguageDirection.LanguageDirection.Equals(LanguageDirection);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return TranslationProvider.Uri.GetHashCode();
		}

		public List<SubsegmentMatchType> SupportedSubsegmentMatchTypes()
		{
			try
			{
				if (_tm.Service is ISubsegmentTranslationMemoryService)
				{
					ISubsegmentTranslationMemoryService subsegmentTranslationMemoryService = _tm.Service as ISubsegmentTranslationMemoryService;
					return subsegmentTranslationMemoryService.SupportedSubsegmentMatchTypes(_tm.Container, _tm.Setup.ResourceId);
				}
				return new List<SubsegmentMatchType>();
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}
	}
}
