using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryTools;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	///
	/// </summary>
	public class AlignmentTMBilingualProcessor : AbstractBilingualContentProcessor
	{
		private readonly List<TranslationUnit> _translationUnitsList;

		private readonly List<ITranslationOrigin> _translationOriginsList = new List<ITranslationOrigin>();

		private readonly ITranslationMemory _translationMemory;

		private readonly LanguagePair _languagePair;

		private readonly ImportSettings _importSettings;

		private const string AlignmentQuality = "Quality";

		private const string SourcePath = "SourceFile";

		private const string TargetPath = "TargetFile";

		private bool _useAlignmentMetaData;

		/// <summary>
		///
		/// </summary>
		public string AlignSourcePath
		{
			get;
			set;
		}

		/// <summary>
		///
		/// </summary>
		public string AlignTargetPath
		{
			get;
			set;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="translationMemory"></param>
		/// <param name="languagePair"></param>
		/// <param name="importSettings"></param>
		public AlignmentTMBilingualProcessor(ITranslationMemory translationMemory, LanguagePair languagePair, ImportSettings importSettings)
		{
			_translationMemory = translationMemory;
			_languagePair = languagePair;
			_importSettings = importSettings;
			_translationUnitsList = new List<TranslationUnit>();
			_useAlignmentMetaData = true;
		}

		public AlignmentTMBilingualProcessor(ITranslationMemory translationMemory, LanguagePair languagePair, ImportSettings importSettings, bool useAlignmentMetaData)
		{
			_translationMemory = translationMemory;
			_languagePair = languagePair;
			_importSettings = importSettings;
			_translationUnitsList = new List<TranslationUnit>();
			_useAlignmentMetaData = useAlignmentMetaData;
		}

		public void Initialize()
		{
			if (_useAlignmentMetaData)
			{
				AddTMCustomFields();
			}
		}

		private void AddTMCustomFields()
		{
			bool flag = false;
			if (_translationMemory.FieldDefinitions.LookupIField("Quality") == null)
			{
				FieldDefinition item = new FieldDefinition("Quality", FieldValueType.Integer);
				_translationMemory.FieldDefinitions.Add(item);
				flag = true;
			}
			if (_translationMemory.FieldDefinitions.LookupIField("SourceFile") == null)
			{
				FieldDefinition item2 = new FieldDefinition("SourceFile", FieldValueType.SingleString);
				_translationMemory.FieldDefinitions.Add(item2);
				flag = true;
			}
			if (_translationMemory.FieldDefinitions.LookupIField("TargetFile") == null)
			{
				FieldDefinition item3 = new FieldDefinition("TargetFile", FieldValueType.SingleString);
				_translationMemory.FieldDefinitions.Add(item3);
				flag = true;
			}
			if (_importSettings.ProjectSettings != null)
			{
				foreach (FieldValue projectSetting in _importSettings.ProjectSettings)
				{
					if (_translationMemory.FieldDefinitions.LookupIField(projectSetting.Name) == null)
					{
						FieldDefinition item4 = new FieldDefinition(projectSetting.Name, projectSetting.ValueType);
						_translationMemory.FieldDefinitions.Add(item4);
						flag = true;
					}
				}
			}
			if (flag)
			{
				_translationMemory.Save();
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="paragraphUnit"></param>
		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			if (paragraphUnit.SegmentPairs != null)
			{
				foreach (ISegmentPair segmentPair in paragraphUnit.SegmentPairs)
				{
					if (segmentPair.Properties.TranslationOrigin != null && segmentPair.Properties.TranslationOrigin.MatchPercent >= _importSettings.AlignmentQuality)
					{
						TranslationUnit translationUnit = TUConverter.BuildLinguaTranslationUnit(_languagePair, segmentPair, paragraphUnit.Properties, _importSettings.PlainText, excludeTagsInLockedContentText: false);
						if (translationUnit != null)
						{
							_translationUnitsList.Add(translationUnit);
							_translationOriginsList.Add(segmentPair.Properties.TranslationOrigin);
							AddAlignmentDataToTUs(translationUnit, segmentPair);
							if (_importSettings.ProjectSettings != null)
							{
								foreach (FieldValue projectSetting in _importSettings.ProjectSettings)
								{
									if (projectSetting.ValueType == FieldValueType.SingleString)
									{
										FieldValue fv = (SingleStringFieldValue)projectSetting;
										translationUnit.FieldValues.Add(fv);
									}
									if (projectSetting.ValueType == FieldValueType.Integer)
									{
										FieldValue fv2 = (IntFieldValue)projectSetting;
										translationUnit.FieldValues.Add(fv2);
									}
									if (projectSetting.ValueType == FieldValueType.DateTime)
									{
										FieldValue fv3 = (DateTimeFieldValue)projectSetting;
										translationUnit.FieldValues.Add(fv3);
									}
									if (projectSetting.ValueType == FieldValueType.SinglePicklist)
									{
										FieldValue fv4 = (SinglePicklistFieldValue)projectSetting;
										translationUnit.FieldValues.Add(fv4);
									}
									if (projectSetting.ValueType == FieldValueType.MultiplePicklist)
									{
										FieldValue fv5 = (MultiplePicklistFieldValue)projectSetting;
										translationUnit.FieldValues.Add(fv5);
									}
								}
							}
						}
					}
				}
			}
			base.ProcessParagraphUnit(paragraphUnit);
		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ImportResult> AddTranslationUnitsInBundels()
		{
			List<ImportResult> list = new List<ImportResult>();
			List<TranslationUnit> list2 = new List<TranslationUnit>();
			for (int i = 0; i < _translationUnitsList.Count; i++)
			{
				if (_translationOriginsList[i] != null && _translationOriginsList[i].GetMetaData("PreviousSegmentId") != null)
				{
					list2.Add(_translationUnitsList[i]);
					continue;
				}
				if (list2.Count > 0)
				{
					list.AddRange(_translationMemory.GetLanguageDirection(_languagePair).AddTranslationUnits(list2.GetRange(0, list2.Count).ToArray(), _importSettings));
					list2.Clear();
				}
				list2.Add(_translationUnitsList[i]);
			}
			if (list2.Count > 0)
			{
				list.AddRange(_translationMemory.GetLanguageDirection(_languagePair).AddTranslationUnits(list2.GetRange(0, list2.Count).ToArray(), _importSettings));
			}
			return list.ToArray();
		}

		private void AddAlignmentDataToTUs(TranslationUnit tu, ISegmentPair segmentPair)
		{
			if (_useAlignmentMetaData)
			{
				FieldValue fv = new IntFieldValue("Quality", (segmentPair.Properties.TranslationOrigin != null) ? segmentPair.Properties.TranslationOrigin.MatchPercent : 0);
				FieldValue fv2 = new SingleStringFieldValue("SourceFile", AlignSourcePath);
				FieldValue fv3 = new SingleStringFieldValue("TargetFile", AlignTargetPath);
				tu.FieldValues.Add(fv);
				tu.FieldValues.Add(fv2);
				tu.FieldValues.Add(fv3);
			}
		}
	}
}
