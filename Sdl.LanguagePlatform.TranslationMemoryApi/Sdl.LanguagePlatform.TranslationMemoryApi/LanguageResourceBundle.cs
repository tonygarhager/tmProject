using Sdl.Core.Api.DataAccess;
using Sdl.Core.LanguageProcessing;
using Sdl.Core.LanguageProcessing.Tokenization;
using Sdl.Core.LanguageProcessing.Tokenization.Transducer;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Resources;
using Sdl.LanguagePlatform.Core.Segmentation;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public class LanguageResourceBundle : INotifyPropertyChanged
	{
		private readonly RecognizerFactory _recognizerFactory;

		private Wordlist _lazyAbbreviations;

		private Wordlist _lazyOrdinalFollowers;

		private Wordlist _lazyVariables;

		private SegmentationRules _lazySegmentationRules;

		private List<SeparatorCombination> _lazyNumbersSeparators;

		private List<CurrencyFormat> _lazyCurrencyFormats;

		private Dictionary<string, CustomUnitDefinition> _lazyMeasurementUnits;

		private List<string> _lazyShortTimeFormats;

		private List<string> _lazyLongTimeFormats;

		private List<string> _lazyShortDateFormats;

		private List<string> _lazyLongDateFormats;

		public CultureInfo Language => CultureInfoExtensions.GetCultureInfo(LanguageCode);

		public string LanguageCode
		{
			get;
		}

		public Wordlist Abbreviations
		{
			get
			{
				if (_lazyAbbreviations == null && Entities != null)
				{
					_lazyAbbreviations = _recognizerFactory.GetAbbreviations(LanguageCode, Entities);
				}
				return _lazyAbbreviations;
			}
			set
			{
				_lazyAbbreviations = value;
				if (_lazyAbbreviations == null && Entities != null)
				{
					RemoveEntity(LanguageResourceType.Abbreviations);
				}
				OnPropertyChanged("Abbreviations");
			}
		}

		public List<CurrencyFormat> CurrencyFormats
		{
			get
			{
				if (_lazyCurrencyFormats == null && Entities != null)
				{
					_lazyCurrencyFormats = _recognizerFactory.GetCurrencySettings(LanguageCode, Entities);
				}
				return _lazyCurrencyFormats;
			}
			set
			{
				_lazyCurrencyFormats = value;
				if (_lazyCurrencyFormats == null && Entities != null)
				{
					RemoveEntity(LanguageResourceType.CurrencyFSTEx);
				}
				OnPropertyChanged("CurrencyFormats");
			}
		}

		public Wordlist OrdinalFollowers
		{
			get
			{
				if (_lazyOrdinalFollowers == null && Entities != null)
				{
					_lazyOrdinalFollowers = _recognizerFactory.GetOrdinalFollowers(LanguageCode, Entities);
				}
				return _lazyOrdinalFollowers;
			}
			set
			{
				_lazyOrdinalFollowers = value;
				if (_lazyOrdinalFollowers == null && Entities != null)
				{
					RemoveEntity(LanguageResourceType.OrdinalFollowers);
				}
				OnPropertyChanged("OrdinalFollowers");
			}
		}

		public Wordlist Variables
		{
			get
			{
				if (_lazyVariables == null && Entities != null)
				{
					_lazyVariables = _recognizerFactory.GetVariables(LanguageCode, Entities);
				}
				return _lazyVariables;
			}
			set
			{
				_lazyVariables = value;
				if (_lazyVariables == null && Entities != null)
				{
					RemoveEntity(LanguageResourceType.Variables);
				}
				OnPropertyChanged("Variables");
			}
		}

		public SegmentationRules SegmentationRules
		{
			get
			{
				if (_lazySegmentationRules == null && Entities != null)
				{
					_lazySegmentationRules = _recognizerFactory.GetSegmentationRules(LanguageCode, Entities);
				}
				return _lazySegmentationRules;
			}
			set
			{
				_lazySegmentationRules = value;
				if (_lazySegmentationRules == null && Entities != null)
				{
					RemoveEntity(LanguageResourceType.SegmentationRules);
				}
				OnPropertyChanged("SegmentationRules");
			}
		}

		public List<SeparatorCombination> NumbersSeparators
		{
			get
			{
				if (_lazyNumbersSeparators == null && Entities != null)
				{
					_lazyNumbersSeparators = _recognizerFactory.GetNumbersSeparators(LanguageCode, Entities);
				}
				return _lazyNumbersSeparators;
			}
			set
			{
				_lazyNumbersSeparators = value;
				if (_lazyNumbersSeparators == null && Entities != null)
				{
					RemoveEntities(LanguageResourceType.NumberFST, LanguageResourceType.NumberFSTEx);
				}
				OnPropertyChanged("NumbersSeparators");
			}
		}

		public List<string> ShortTimeFormats
		{
			get
			{
				if (_lazyShortTimeFormats == null && Entities != null)
				{
					_lazyShortTimeFormats = _recognizerFactory.GetDateTimeFormats(LanguageCode, Entities, LanguageResourceType.ShortTimeFSTEx);
				}
				return _lazyShortTimeFormats;
			}
			set
			{
				_lazyShortTimeFormats = value;
				if (_lazyShortTimeFormats == null && Entities != null)
				{
					RemoveEntities(LanguageResourceType.ShortTimeFST, LanguageResourceType.ShortTimeFSTEx);
				}
				OnPropertyChanged("ShortTimeFormats");
			}
		}

		public List<string> ShortDateFormats
		{
			get
			{
				if (_lazyShortDateFormats == null && Entities != null)
				{
					_lazyShortDateFormats = _recognizerFactory.GetDateTimeFormats(LanguageCode, Entities, LanguageResourceType.ShortDateFSTEx);
				}
				return _lazyShortDateFormats;
			}
			set
			{
				_lazyShortDateFormats = value;
				if (_lazyShortDateFormats == null && Entities != null)
				{
					RemoveEntities(LanguageResourceType.ShortDateFST, LanguageResourceType.ShortDateFSTEx);
				}
				OnPropertyChanged("ShortDateFormats");
			}
		}

		public List<string> LongTimeFormats
		{
			get
			{
				if (_lazyLongTimeFormats == null && Entities != null)
				{
					_lazyLongTimeFormats = _recognizerFactory.GetDateTimeFormats(LanguageCode, Entities, LanguageResourceType.LongTimeFSTEx);
				}
				return _lazyLongTimeFormats;
			}
			set
			{
				_lazyLongTimeFormats = value;
				if (_lazyLongTimeFormats == null && Entities != null)
				{
					RemoveEntities(LanguageResourceType.LongTimeFST, LanguageResourceType.LongTimeFSTEx);
				}
				OnPropertyChanged("LongTimeFormats");
			}
		}

		public List<string> LongDateFormats
		{
			get
			{
				if (_lazyLongDateFormats == null && Entities != null)
				{
					_lazyLongDateFormats = _recognizerFactory.GetDateTimeFormats(LanguageCode, Entities, LanguageResourceType.LongDateFSTEx);
				}
				return _lazyLongDateFormats;
			}
			set
			{
				_lazyLongDateFormats = value;
				if (_lazyLongDateFormats == null && Entities != null)
				{
					RemoveEntities(LanguageResourceType.LongDateFST, LanguageResourceType.LongDateFSTEx);
				}
				OnPropertyChanged("LongDateFormats");
			}
		}

		public Dictionary<string, CustomUnitDefinition> MeasurementUnits
		{
			get
			{
				if (_lazyMeasurementUnits == null && Entities != null)
				{
					_lazyMeasurementUnits = _recognizerFactory.GetMesurementUnits(LanguageCode, Entities);
				}
				return _lazyMeasurementUnits;
			}
			set
			{
				_lazyMeasurementUnits = value;
				if (_lazyMeasurementUnits == null && Entities != null)
				{
					RemoveEntities(LanguageResourceType.MeasurementFST, LanguageResourceType.MeasurementFSTEx);
				}
				OnPropertyChanged("MeasurementUnits");
			}
		}

		internal EntityCollection<LanguageResourceEntity> Entities
		{
			get;
			set;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public LanguageResourceBundle(CultureInfo language)
			: this(language.Name)
		{
		}

		internal LanguageResourceBundle(CultureInfo language, EntityCollection<LanguageResourceEntity> entities)
			: this(language)
		{
			Entities = entities;
		}

		public LanguageResourceBundle(string languageCode)
		{
			LanguageCode = languageCode;
			_recognizerFactory = new RecognizerFactory();
		}

		internal LanguageResourceBundle(string languageCode, EntityCollection<LanguageResourceEntity> entities)
			: this(languageCode)
		{
			Entities = entities;
		}

		public LanguageResourceBundle()
		{
		}

		public LanguageResourceBundle Clone()
		{
			List<CurrencyFormat> clonedCurrencyFormat = null;
			if (CurrencyFormats != null)
			{
				clonedCurrencyFormat = new List<CurrencyFormat>(CurrencyFormats.Count);
				CurrencyFormats.ForEach(delegate(CurrencyFormat item)
				{
					clonedCurrencyFormat.Add((CurrencyFormat)item.Clone());
				});
			}
			List<SeparatorCombination> clonedNumberSeparators = null;
			if (NumbersSeparators != null)
			{
				clonedNumberSeparators = new List<SeparatorCombination>(NumbersSeparators.Count);
				NumbersSeparators.ForEach(delegate(SeparatorCombination item)
				{
					clonedNumberSeparators.Add((SeparatorCombination)item.Clone());
				});
			}
			List<string> clonedShortTimeFormats = null;
			if (ShortTimeFormats != null)
			{
				clonedShortTimeFormats = new List<string>(ShortTimeFormats.Count);
				ShortTimeFormats.ForEach(delegate(string item)
				{
					clonedShortTimeFormats.Add(item.ToString());
				});
			}
			List<string> clonedLongTimeFormats = null;
			if (LongTimeFormats != null)
			{
				clonedLongTimeFormats = new List<string>(LongTimeFormats.Count);
				LongTimeFormats.ForEach(delegate(string item)
				{
					clonedLongTimeFormats.Add(item.ToString());
				});
			}
			List<string> clonedShortDateFormats = null;
			if (ShortDateFormats != null)
			{
				clonedShortDateFormats = new List<string>(ShortDateFormats.Count);
				ShortDateFormats.ForEach(delegate(string item)
				{
					clonedShortDateFormats.Add(item.ToString());
				});
			}
			List<string> clonedLongDateFormats = null;
			if (LongDateFormats != null)
			{
				clonedLongDateFormats = new List<string>(LongDateFormats.Count);
				LongDateFormats.ForEach(delegate(string item)
				{
					clonedLongDateFormats.Add(item.ToString());
				});
			}
			Dictionary<string, CustomUnitDefinition> dictionary = null;
			if (MeasurementUnits != null)
			{
				dictionary = new Dictionary<string, CustomUnitDefinition>(MeasurementUnits.Count);
				foreach (KeyValuePair<string, CustomUnitDefinition> measurementUnit in MeasurementUnits)
				{
					dictionary.Add(measurementUnit.Key.ToString(), (CustomUnitDefinition)(measurementUnit.Value?.Clone()));
				}
			}
			return new LanguageResourceBundle(LanguageCode)
			{
				SegmentationRules = (SegmentationRules)(SegmentationRules?.Clone()),
				Abbreviations = (Wordlist)(Abbreviations?.Clone()),
				OrdinalFollowers = (Wordlist)(OrdinalFollowers?.Clone()),
				Variables = (Wordlist)(Variables?.Clone()),
				NumbersSeparators = clonedNumberSeparators,
				CurrencyFormats = clonedCurrencyFormat,
				ShortTimeFormats = clonedShortTimeFormats,
				LongTimeFormats = clonedLongTimeFormats,
				ShortDateFormats = clonedShortDateFormats,
				LongDateFormats = clonedLongDateFormats,
				MeasurementUnits = dictionary
			};
		}

		public void ResetToDefaults()
		{
			SegmentationRules = null;
			Abbreviations = null;
			OrdinalFollowers = null;
			Variables = null;
			NumbersSeparators = null;
			CurrencyFormats = null;
			ShortTimeFormats = null;
			LongTimeFormats = null;
			ShortDateFormats = null;
			LongDateFormats = null;
			MeasurementUnits = null;
		}

		public bool IsCustomized()
		{
			if (SegmentationRules == null && Abbreviations == null && OrdinalFollowers == null && Variables == null && NumbersSeparators == null && CurrencyFormats == null && ShortTimeFormats == null && LongTimeFormats == null && ShortDateFormats == null && LongDateFormats == null)
			{
				return MeasurementUnits != null;
			}
			return true;
		}

		internal void DiscardChanges()
		{
			_lazyAbbreviations = null;
			_lazyVariables = null;
			_lazySegmentationRules = null;
			_lazyOrdinalFollowers = null;
			_lazyNumbersSeparators = null;
			_lazyCurrencyFormats = null;
			_lazyShortTimeFormats = null;
			_lazyLongTimeFormats = null;
			_lazyShortDateFormats = null;
			_lazyLongDateFormats = null;
			_lazyMeasurementUnits = null;
		}

		internal void SaveToEntities()
		{
			if (Entities == null)
			{
				throw new InvalidOperationException("This language resource bundle is not connected to an entity collection.");
			}
			SaveAbbreviations();
			SaveOrdinalFollowers();
			SaveVariables();
			SaveSegmentationRules();
			SaveNumeric();
			SaveDateTime();
		}

		internal void RemoveEntities()
		{
			if (Entities == null)
			{
				throw new InvalidOperationException("This language resource bundle is not connected to an entity collection.");
			}
			Entities.Clear();
		}

		private void SaveToEntity(byte[] data, LanguageResourceType type)
		{
			if (Entities == null)
			{
				throw new InvalidOperationException("This language resource bundle is not connected to an entity collection.");
			}
			LanguageResourceEntity languageResourceEntity = GetEntity(type);
			if (languageResourceEntity == null)
			{
				languageResourceEntity = new LanguageResourceEntity
				{
					UniqueId = Guid.NewGuid(),
					CultureName = LanguageCode,
					Type = type
				};
				Entities.Add(languageResourceEntity);
			}
			languageResourceEntity.Data = data;
		}

		private LanguageResourceEntity GetEntity(LanguageResourceType languageResourceType)
		{
			if (Entities == null)
			{
				throw new InvalidOperationException("This language resource bundle is not connected to an entity collection.");
			}
			return Entities.FirstOrDefault((LanguageResourceEntity entity) => string.Equals(entity.CultureName, LanguageCode, StringComparison.OrdinalIgnoreCase) && entity.Type == languageResourceType);
		}

		private void RemoveEntities(params LanguageResourceType[] types)
		{
			foreach (LanguageResourceType type in types)
			{
				RemoveEntity(type);
			}
		}

		private void RemoveEntity(LanguageResourceType type)
		{
			if (Entities == null)
			{
				throw new InvalidOperationException("This language resource bundle is not connected to an entity collection.");
			}
			LanguageResourceEntity entity = GetEntity(type);
			if (entity != null)
			{
				Entities.Remove(entity);
			}
		}

		private void SaveSegmentationRules()
		{
			if (_lazySegmentationRules != null)
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					_lazySegmentationRules.Save(memoryStream);
					SaveToEntity(memoryStream.ToArray(), LanguageResourceType.SegmentationRules);
				}
			}
		}

		private void SaveVariables()
		{
			if (_lazyVariables != null)
			{
				SaveToEntity(_lazyVariables.GetBytes(), LanguageResourceType.Variables);
			}
		}

		private void SaveOrdinalFollowers()
		{
			if (_lazyOrdinalFollowers != null)
			{
				SaveToEntity(_lazyOrdinalFollowers.GetBytes(), LanguageResourceType.OrdinalFollowers);
			}
		}

		private void SaveAbbreviations()
		{
			if (_lazyAbbreviations != null)
			{
				SaveToEntity(_lazyAbbreviations.GetBytes(), LanguageResourceType.Abbreviations);
			}
		}

		private void SaveDateTime()
		{
			Dictionary<LanguageResourceType, List<string>> dictionary = new Dictionary<LanguageResourceType, List<string>>
			{
				{
					LanguageResourceType.ShortTimeFSTEx,
					_lazyShortTimeFormats
				},
				{
					LanguageResourceType.LongTimeFSTEx,
					_lazyLongTimeFormats
				},
				{
					LanguageResourceType.ShortDateFSTEx,
					_lazyShortDateFormats
				},
				{
					LanguageResourceType.LongDateFSTEx,
					_lazyLongDateFormats
				}
			};
			if (dictionary.Any((KeyValuePair<LanguageResourceType, List<string>> r) => r.Value != null))
			{
				RemoveEntities(LanguageResourceType.ShortTimeFST, LanguageResourceType.ShortTimeFSTEx, LanguageResourceType.LongTimeFST, LanguageResourceType.LongTimeFSTEx, LanguageResourceType.ShortDateFST, LanguageResourceType.ShortDateFSTEx, LanguageResourceType.LongDateFST, LanguageResourceType.LongDateFSTEx);
				List<string> list = new List<string>();
				IEnumerable<List<string>> enumerable = dictionary.Values.Where((List<string> v) => v != null && v.Count() != 0);
				foreach (List<string> item5 in enumerable)
				{
					list.AddRange(item5);
				}
				if (list.Count != 0)
				{
					List<(LanguageResourceType, FST, LanguageResourceType, DateTimeFSTEx)> dateTimeLanguageResources = DateTimePatternComputer.GetDateTimeLanguageResources(Language, LanguageMetadata.GetMetadata(Language.Name), list, customOnly: true);
					foreach (var item6 in dateTimeLanguageResources)
					{
						LanguageResourceType item = item6.Item1;
						FST item2 = item6.Item2;
						LanguageResourceType item3 = item6.Item3;
						DateTimeFSTEx item4 = item6.Item4;
						if (dictionary[item3] != null)
						{
							SaveToEntity(item2.GetBinary(), item);
							SaveToEntity(item4.ToBinary(), item3);
						}
					}
				}
			}
		}

		private void SaveNumeric()
		{
			Dictionary<LanguageResourceType, bool> dictionary = new Dictionary<LanguageResourceType, bool>
			{
				{
					LanguageResourceType.NumberFSTEx,
					_lazyNumbersSeparators != null
				},
				{
					LanguageResourceType.CurrencyFSTEx,
					_lazyCurrencyFormats != null
				},
				{
					LanguageResourceType.MeasurementFSTEx,
					_lazyMeasurementUnits != null
				}
			};
			if (dictionary.Any((KeyValuePair<LanguageResourceType, bool> r) => r.Value))
			{
				List<SeparatorCombination> list = _lazyNumbersSeparators ?? new List<SeparatorCombination>();
				Dictionary<string, CustomUnitDefinition> dictionary2 = _lazyMeasurementUnits ?? new Dictionary<string, CustomUnitDefinition>();
				List<CurrencyFormat> currencyFormats = _lazyCurrencyFormats ?? new List<CurrencyFormat>();
				(FST, NumberFSTEx, FST, MeasureFSTEx, FST, CurrencyFSTEx) measureNumberAndCurrencyLanguageResources = NumberPatternComputer.GetMeasureNumberAndCurrencyLanguageResources(Language, LanguageMetadata.GetMetadata(Language.Name), list, dictionary2, currencyFormats, list.Count != 0, dictionary2.Count != 0);
				RemoveEntities(LanguageResourceType.NumberFST, LanguageResourceType.NumberFSTEx, LanguageResourceType.CurrencyFST, LanguageResourceType.CurrencyFSTEx, LanguageResourceType.MeasurementFST, LanguageResourceType.MeasurementFSTEx);
				SaveToEntity(measureNumberAndCurrencyLanguageResources.Item1.GetBinary(), LanguageResourceType.NumberFST);
				SaveToEntity(measureNumberAndCurrencyLanguageResources.Item5.GetBinary(), LanguageResourceType.CurrencyFST);
				SaveToEntity(measureNumberAndCurrencyLanguageResources.Item3.GetBinary(), LanguageResourceType.MeasurementFST);
				if (dictionary[LanguageResourceType.NumberFSTEx])
				{
					SaveToEntity(measureNumberAndCurrencyLanguageResources.Item2.ToBinary(), LanguageResourceType.NumberFSTEx);
				}
				if (dictionary[LanguageResourceType.CurrencyFSTEx])
				{
					SaveToEntity(measureNumberAndCurrencyLanguageResources.Item6.ToBinary(), LanguageResourceType.CurrencyFSTEx);
				}
				if (dictionary[LanguageResourceType.MeasurementFSTEx])
				{
					SaveToEntity(measureNumberAndCurrencyLanguageResources.Item4.ToBinary(), LanguageResourceType.MeasurementFSTEx);
				}
			}
		}

		private void OnPropertyChanged(string property)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
		}
	}
}
