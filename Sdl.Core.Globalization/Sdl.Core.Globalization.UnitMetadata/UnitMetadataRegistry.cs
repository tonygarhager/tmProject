using Sdl.Core.Globalization.LanguageRegistry;
using Sdl.Core.Globalization.NumberMetadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.Globalization.UnitMetadata
{
	public class UnitMetadataRegistry
	{
		internal Dictionary<string, UnitMetadataSet> UnitMetadataSets = new Dictionary<string, UnitMetadataSet>();

		private static string[] _undesirableLabels = new string[10]
		{
			"H",
			"s",
			"C",
			"F",
			"S",
			"d",
			"h",
			"'",
			"\"",
			"″"
		};

		public UnitDefinitionSet UnitDefinitionSet
		{
			get;
			set;
		}

		public UnitMetadata UnitMetadataFromLabel(string label, string languageCode)
		{
			UnitMetadataSet value;
			return TryHierachy((string x) => UnitMetadataSets.TryGetValue(x, out value) ? value.UnitMetadataList.FirstOrDefault((UnitMetadata y) => y.LabelValueSetFromLabel(label) != null) : null, languageCode);
		}

		public List<UnitMetadata> AllUnitMetadataFromLabel(string label, string languageCode)
		{
			if (languageCode == null)
			{
				throw new ArgumentNullException("languageCode");
			}
			List<UnitMetadata> list = new List<UnitMetadata>();
			Language language = (languageCode.Length > 0) ? LanguageRegistryApi.Instance.GetLanguage(languageCode) : null;
			HashSet<string> hashSet = new HashSet<string>();
			while (true)
			{
				if (UnitMetadataSets.TryGetValue(languageCode, out UnitMetadataSet value))
				{
					foreach (UnitMetadata unitMetadata in value.UnitMetadataList)
					{
						if (!hashSet.Contains(unitMetadata.UnitKey))
						{
							if (unitMetadata.DoNotInherit)
							{
								hashSet.Add(unitMetadata.UnitKey);
							}
							if (unitMetadata.LabelValueSetFromLabel(label) != null)
							{
								list.Add(unitMetadata);
							}
						}
					}
				}
				if (languageCode.Length == 0)
				{
					break;
				}
				language = language?.ParentLanguage;
				languageCode = language?.LanguageCode;
				if (languageCode == null)
				{
					languageCode = string.Empty;
				}
			}
			return list;
		}

		private T TryHierachy<T>(Func<string, T> f, string languageCode) where T : class
		{
			if (languageCode == null)
			{
				throw new ArgumentNullException("languageCode");
			}
			if (languageCode.Length > 0)
			{
				Language language = LanguageRegistryApi.Instance.GetLanguage(languageCode);
				while (languageCode != null)
				{
					T val = f(language.LanguageCode);
					if (val != null)
					{
						return val;
					}
					language = language.ParentLanguage;
					languageCode = language?.LanguageCode;
				}
			}
			return f(string.Empty);
		}

		public List<UnitMetadata> GetAllMetadata(string languageCode)
		{
			if (languageCode == null)
			{
				throw new ArgumentNullException("languageCode");
			}
			List<UnitMetadata> list = new List<UnitMetadata>();
			Language language = (languageCode.Length > 0) ? LanguageRegistryApi.Instance.GetLanguage(languageCode) : null;
			HashSet<string> hashSet = new HashSet<string>();
			while (true)
			{
				if (UnitMetadataSets.TryGetValue(languageCode, out UnitMetadataSet value))
				{
					foreach (UnitMetadata unitMetadata in value.UnitMetadataList)
					{
						if (!hashSet.Contains(unitMetadata.UnitKey))
						{
							if (unitMetadata.DoNotInherit)
							{
								hashSet.Add(unitMetadata.UnitKey);
							}
							list.Add(unitMetadata);
						}
					}
				}
				if (languageCode.Length == 0)
				{
					break;
				}
				language = language?.ParentLanguage;
				languageCode = language?.LanguageCode;
				if (languageCode == null)
				{
					languageCode = string.Empty;
				}
			}
			return list;
		}

		public LabelValueSet GetPreferredLabelValueSet(string unitKey, string languageCode)
		{
			UnitMetadataSet value;
			return TryHierachy((string x) => UnitMetadataSets.TryGetValue(x, out value) ? value.GetMetadataByKey(unitKey)?.LabelValueSets.FirstOrDefault((LabelValueSet y) => y.Preferred) : null, languageCode);
		}

		public UnitMetadata UnitMetadataFromKey(string unitKey, string languageCode, out string languageCodeFound)
		{
			languageCodeFound = TryHierachy((string x) => (UnitMetadataSets.TryGetValue(x, out UnitMetadataSet value) && value.GetMetadataByKey(unitKey) != null) ? x : null, languageCode);
			if (languageCodeFound != null)
			{
				return UnitMetadataSets[languageCodeFound].GetMetadataByKey(unitKey);
			}
			return null;
		}

		internal void FixupNumberMetadata()
		{
			foreach (KeyValuePair<string, UnitMetadataSet> unitMetadataSet in UnitMetadataSets)
			{
				Sdl.Core.Globalization.NumberMetadata.NumberMetadata numberMetadata = NumberMetadataApi.Instance.Registry.FindMetadata(unitMetadataSet.Key);
				if (numberMetadata != null)
				{
					foreach (UnitMetadata unitMetadata in unitMetadataSet.Value.UnitMetadataList)
					{
						foreach (LabelValueSet labelValueSet in unitMetadata.LabelValueSets)
						{
							labelValueSet.NumberMetadata = numberMetadata;
						}
					}
				}
			}
		}

		internal void Validate()
		{
			if (!UnitMetadataSets.ContainsKey(""))
			{
				throw new UnitMetadataRegistryException("Top-level unit metadata is missing");
			}
			if (UnitDefinitionSet.UnitDefinitions.Count == 0)
			{
				throw new UnitMetadataRegistryException("No unit definitions were found");
			}
			List<UnitDefinition> list = UnitDefinitionSet.UnitDefinitions.Where((UnitDefinition x) => UnitMetadataSets[""].UnitMetadataList.Count((UnitMetadata y) => string.Compare(y.UnitKey, x.UnitKey) == 0) != 1).ToList();
			if (list.Count > 0)
			{
				throw new UnitMetadataRegistryException("Top-level unit metadata is missing labels for the following unit definitions: " + string.Join(",", list.Select((UnitDefinition x) => x.UnitKey)));
			}
			List<string> list2 = (from x in UnitMetadataSets[""].UnitMetadataList
				where x.LabelValueSets.Count((LabelValueSet y) => y.Preferred) != 1
				select x into z
				select z.UnitKey).ToList();
			if (list2.Count > 0)
			{
				throw new UnitMetadataRegistryException("Top-level unit metadata has label value sets without exactly one preferred label for the following unit defintions: " + string.Join(",", list2));
			}
			IEnumerable<string> topLevelKeys = UnitDefinitionSet.UnitDefinitions.Select((UnitDefinition x) => x.UnitKey);
			HashSet<string> keys = new HashSet<string>(topLevelKeys);
			if (keys.Count != topLevelKeys.Count())
			{
				IEnumerable<string> values = topLevelKeys.Where((string x) => topLevelKeys.Count((string y) => y == x) > 1);
				throw new UnitMetadataRegistryException("Unit metadata contains the following duplicate unit keys: " + string.Join(",", values));
			}
			foreach (UnitMetadata unitMetadata in UnitMetadataSets[""].UnitMetadataList)
			{
				foreach (LabelValueSet labelValueSet in unitMetadata.LabelValueSets)
				{
					foreach (LabelValueCondition labelValueCondition in labelValueSet.LabelValueConditions)
					{
						if (_undesirableLabels.Contains(labelValueCondition.Label))
						{
							throw new UnitMetadataRegistryException("Top-level unit metadata for " + unitMetadata.UnitKey + " contains undesirable label " + labelValueCondition.Label);
						}
					}
				}
			}
			foreach (KeyValuePair<string, UnitMetadataSet> unitMetadataSet in UnitMetadataSets)
			{
				Sdl.Core.Globalization.NumberMetadata.NumberMetadata numberMetadata = NumberMetadataApi.Instance.Registry.FindMetadata(unitMetadataSet.Key);
				List<string> list3 = (from x in unitMetadataSet.Value.UnitMetadataList
					where !keys.Contains(x.UnitKey)
					select x into y
					select y.UnitKey).ToList();
				if (list3.Count > 0)
				{
					throw new UnitMetadataRegistryException("Unit metadata for language code '" + unitMetadataSet.Key + "' contains the following unknown unit keys: " + string.Join(",", list3));
				}
				List<string> keysHere = unitMetadataSet.Value.UnitMetadataList.Select((UnitMetadata x) => x.UnitKey).ToList();
				HashSet<string> hashSet = new HashSet<string>(keysHere);
				if (hashSet.Count != keysHere.Count)
				{
					IEnumerable<string> values2 = keysHere.Where((string x) => keysHere.Count((string y) => y == x) > 1);
					throw new UnitMetadataRegistryException("Unit metadata for language code '" + unitMetadataSet.Key + "' contains the following duplicate unit keys: " + string.Join(",", values2));
				}
				var list4 = unitMetadataSet.Value.UnitMetadataList.SelectMany((UnitMetadata x) => x.LabelValueSets.Select((LabelValueSet y) => new
				{
					key = x.UnitKey,
					msg = y.Validate(numberMetadata)
				})).ToList();
				list4 = list4.FindAll(x => x.msg != null);
				if (list4.Count > 0)
				{
					throw new UnitMetadataRegistryException("Unit metadata for language code '" + unitMetadataSet.Key + "' contains the following LabelValueSet errors: " + string.Join("; ", list4.Select(x => x.key + " " + x.msg)));
				}
				List<string> list5 = (from x in unitMetadataSet.Value.UnitMetadataList
					where x.LabelValueSets.Count((LabelValueSet y) => y.Preferred) > 1
					select x.UnitKey).ToList();
				if (list5.Count > 0)
				{
					throw new UnitMetadataRegistryException("Unit metadata for language code '" + unitMetadataSet.Key + "' contains label sets with >1 preferred label for the following unknown unit keys: " + string.Join(",", list5));
				}
			}
		}
	}
}
