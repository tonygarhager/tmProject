using Sdl.Core.Globalization.UnitMetadata;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Sdl.LanguagePlatform.Core.Tokenization
{
	public static class PhysicalUnit
	{
		private static readonly List<PhysicalUnitDescription> Units;

		private static Dictionary<string, int> _anonymousGroups;

		private static HashSet<Unit> UntypedUnits;

		private static void LoadFromRegistry()
		{
			UnitMetadataRegistry registry = UnitMetadataApi.Instance.Registry;
			List<UnitDefinition> unitDefinitions = registry.UnitDefinitionSet.UnitDefinitions;
			foreach (UnitDefinition item3 in unitDefinitions)
			{
				Unit result = Unit.NoUnit;
				UnitType result2 = UnitType.Other;
				if (item3.UnitType != null && item3.UnitType.All((char x) => !char.IsDigit(x)))
				{
					Enum.TryParse(item3.UnitType, out result2);
				}
				string text = null;
				if (!Enum.TryParse(item3.UnitKey, out result))
				{
					result = Unit.NoUnit;
					text = item3.UnitKey;
				}
				UnitSystem us = UnitSystem.NoSystem;
				string languageCodeFound;
				UnitMetadata unitMetadata = registry.UnitMetadataFromKey(item3.UnitKey, "", out languageCodeFound);
				List<LabelValueSet> list = new List<LabelValueSet>(unitMetadata.LabelValueSets);
				LabelValueSet item = list.First((LabelValueSet x) => x.Preferred);
				list.Remove(item);
				list.Add(item);
				StringBuilder stringBuilder = new StringBuilder();
				foreach (LabelValueSet item4 in list)
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append("|");
					}
					stringBuilder.Append(item4.GetLabel(0));
				}
				PhysicalUnitDescription item2 = new PhysicalUnitDescription(result, result2, us, isCanonical: false, 0, 0.0, Unit.NoUnit, "", stringBuilder.ToString());
				Units.Add(item2);
			}
		}

		static PhysicalUnit()
		{
			Units = new List<PhysicalUnitDescription>();
			UntypedUnits = new HashSet<Unit>
			{
				Unit.NoUnit,
				Unit.Other,
				Unit.Currency
			};
			LoadFromRegistry();
		}

		public static UnitSystem GetSystem(Unit unit)
		{
			return Find(unit)?.UnitSystem ?? UnitSystem.NoSystem;
		}

		public static bool IsMetric(Unit unit)
		{
			return GetSystem(unit) == UnitSystem.Metric;
		}

		public static UnitType GetUnitType(Unit unit)
		{
			return Find(unit)?.UnitType ?? UnitType.NoType;
		}

		[Obsolete]
		public static bool AreUnitTypesCompatible(Unit a, Unit b)
		{
			return false;
		}

		public static string GetAbbreviations(Unit unit)
		{
			return Find(unit)?.Abbreviations;
		}

		public static string GetPreferredAbbreviation(Unit unit, CultureInfo culture)
		{
			return Find(unit)?.GetPreferredAbbreviation(culture);
		}

		public static string GetName(Unit unit)
		{
			return Find(unit)?.Name;
		}

		public static UnitSystem GetDefaultSystem(UnitType type, CultureInfo culture)
		{
			try
			{
				if (culture == null)
				{
					return UnitSystem.Metric;
				}
				RegionInfo regionInfo = new RegionInfo(culture.Name);
				if (regionInfo.IsMetric)
				{
					return UnitSystem.Metric;
				}
				if (type != UnitType.Capacity)
				{
					return UnitSystem.BIS;
				}
				if (regionInfo.ThreeLetterISORegionName.Equals("SGP"))
				{
					return UnitSystem.BISUK;
				}
				return UnitSystem.BISUSFluid;
			}
			catch
			{
				return UnitSystem.Metric;
			}
		}

		public static Unit Find(string abbreviation, CultureInfo culture)
		{
			return Find(abbreviation, GetDefaultSystem(UnitType.Capacity, culture), culture);
		}

		public static Unit Find(string abbreviation, UnitSystem preferredSystem, CultureInfo culture)
		{
			PhysicalUnitDescription physicalUnitDescription = null;
			if (preferredSystem == UnitSystem.BIS || preferredSystem == UnitSystem.NoSystem)
			{
				preferredSystem = GetDefaultSystem(UnitType.Capacity, culture);
			}
			bool flag = false;
			Unit unit = Unit.NoUnit;
			foreach (PhysicalUnitDescription unit2 in Units)
			{
				if (unit2 != null && unit2.HasAbbreviation(abbreviation, culture))
				{
					if (unit2.UnitType == UnitType.Capacity)
					{
						if (unit2.UnitSystem == preferredSystem)
						{
							flag = true;
						}
						else
						{
							unit = unit2.Unit;
						}
					}
					else
					{
						flag = true;
					}
					if (flag)
					{
						physicalUnitDescription = unit2;
						break;
					}
				}
			}
			return physicalUnitDescription?.Unit ?? unit;
		}

		private static PhysicalUnitDescription CreateAnonymousUnit(string unit)
		{
			return new PhysicalUnitDescription(Unit.NoUnit, UnitType.NoType, UnitSystem.NoSystem, isCanonical: false, 1, 1.0, Unit.NoUnit, unit, unit);
		}

		private static void AddAnonymousUnitUnlessPresent(ICollection<PhysicalUnitDescription> units, ISet<string> addedUnits, string u)
		{
			if (!addedUnits.Contains(u))
			{
				units.Add(CreateAnonymousUnit(u));
				addedUnits.Add(u);
			}
		}

		private static void AddAnonymousUnits(ICollection<PhysicalUnitDescription> units)
		{
			HashSet<string> addedUnits = new HashSet<string>();
			_anonymousGroups = new Dictionary<string, int>();
			string[] separator = new string[1]
			{
				"|"
			};
			int num = -1;
			string[] array = new string[38]
			{
				"/kg",
				"/l",
				"kcal/g",
				"l/kg",
				"ml/kg",
				"ml/min",
				"min",
				"mm³",
				"mg/l",
				"mg/ml",
				"/mm³",
				"mol/l",
				"mmol/l",
				"µmol/l",
				"nmol/l",
				"pmol/l",
				"mol/L",
				"mmol/L",
				"µmol/L",
				"nmol/L",
				"pmol/L",
				"mol/dl",
				"mmol/dl",
				"µmol/dl",
				"nmol/dl",
				"pmol/dl",
				"µg",
				"ng",
				"pg",
				"mol",
				"mmol",
				"µmol",
				"nmol",
				"pmol",
				"KB",
				"MB",
				"GB",
				"TB"
			};
			foreach (string text in array)
			{
				switch (text)
				{
				case "mol/l":
				case "µg":
				case "mol":
				case "KB":
					num++;
					break;
				}
				AddAnonymousUnitUnlessPresent(units, addedUnits, text);
				if (!_anonymousGroups.ContainsKey(text))
				{
					_anonymousGroups.Add(text, num);
				}
			}
			string[] array2 = "V|Hz|A|J|PS|N|Nm|K|cal|B|W".Split(separator, StringSplitOptions.RemoveEmptyEntries);
			foreach (string text2 in array2)
			{
				num++;
				if (!_anonymousGroups.ContainsKey(text2))
				{
					_anonymousGroups.Add(text2, num);
				}
				AddAnonymousUnitUnlessPresent(units, addedUnits, text2);
				string[] array3 = "m|M|k|K|G".Split(separator, StringSplitOptions.RemoveEmptyEntries);
				foreach (string str in array3)
				{
					AddAnonymousUnitUnlessPresent(units, addedUnits, str + text2);
					if (!_anonymousGroups.ContainsKey(str + text2))
					{
						_anonymousGroups.Add(str + text2, num);
					}
				}
			}
			string[] array4 = "dl|kg|l|m²|ml".Split(separator, StringSplitOptions.RemoveEmptyEntries);
			foreach (string text3 in array4)
			{
				num++;
				if (!_anonymousGroups.ContainsKey("g/" + text3))
				{
					_anonymousGroups.Add("g/" + text3, num);
				}
				AddAnonymousUnitUnlessPresent(units, addedUnits, "g/" + text3);
				string[] array5 = "m|µ|n|p|k".Split(separator, StringSplitOptions.RemoveEmptyEntries);
				foreach (string str2 in array5)
				{
					AddAnonymousUnitUnlessPresent(units, addedUnits, str2 + "g/" + text3);
					if (!_anonymousGroups.ContainsKey(str2 + "g/" + text3))
					{
						_anonymousGroups.Add(str2 + "g/" + text3, num);
					}
				}
			}
		}

		public static bool AreUnitsSameClass(CultureInfo ci, Unit u1, Unit u2)
		{
			if (UntypedUnits.Contains(u1) || UntypedUnits.Contains(u2))
			{
				return false;
			}
			return AreUnitKeysSameClass(ci, u1.ToString(), u2.ToString());
		}

		public static bool AreUnitsSameClass(CultureInfo ci, string unitA, string unitB)
		{
			if (ci == null)
			{
				return false;
			}
			UnitMetadataRegistry registry = UnitMetadataApi.Instance.Registry;
			UnitMetadata unitMetadata = registry.UnitMetadataFromLabel(unitA, ci.Name);
			UnitMetadata unitMetadata2 = registry.UnitMetadataFromLabel(unitB, ci.Name);
			if (unitMetadata == null || unitMetadata2 == null)
			{
				return false;
			}
			return AreUnitKeysSameClass(ci, unitMetadata.UnitKey, unitMetadata2.UnitKey);
		}

		public static bool AreUnitKeysSameClass(CultureInfo ci, string key1, string key2)
		{
			UnitMetadataRegistry registry = UnitMetadataApi.Instance.Registry;
			UnitDefinition definitionByKey = registry.UnitDefinitionSet.GetDefinitionByKey(key1);
			UnitDefinition definitionByKey2 = registry.UnitDefinitionSet.GetDefinitionByKey(key2);
			if (definitionByKey == null || definitionByKey2 == null)
			{
				return false;
			}
			if (string.IsNullOrWhiteSpace(definitionByKey.UnitType) || string.IsNullOrWhiteSpace(definitionByKey2.UnitType))
			{
				return false;
			}
			return string.CompareOrdinal(definitionByKey.UnitType, definitionByKey2.UnitType) == 0;
		}

		[Obsolete]
		public static bool AreUnitsSameCategory(string unitA, string unitB)
		{
			return false;
		}

		public static string GetUnitsRX(CultureInfo culture, bool includeDefaultAbbreviationsWithCultureAbbreviations)
		{
			Wordlist units = GetUnits(culture, includeDefaultAbbreviationsWithCultureAbbreviations);
			CharacterSet first;
			return units.GetRegularExpression(out first);
		}

		public static Wordlist GetUnits(CultureInfo culture, bool includeDefaultAbbreviationsWithCultureAbbreviations)
		{
			bool flag = false;
			if (culture != null)
			{
				flag = CultureInfoExtensions.UseFullWidth(culture);
			}
			Wordlist wordlist = new Wordlist();
			foreach (PhysicalUnitDescription unit in Units)
			{
				bool flag2 = false;
				if (culture != null && unit.CultureSpecificAbbreviations != null)
				{
					string cultureSpecificAbbreviations = unit.GetCultureSpecificAbbreviations(culture);
					if (!string.IsNullOrEmpty(cultureSpecificAbbreviations))
					{
						flag2 = true;
						string[] array = cultureSpecificAbbreviations.Split('|');
						foreach (string text in array)
						{
							wordlist.Add(text);
							if (flag)
							{
								wordlist.Add(StringUtilities.HalfWidthToFullWidth2(text));
							}
						}
					}
				}
				if ((!flag2 || includeDefaultAbbreviationsWithCultureAbbreviations) && unit != null && unit.Abbreviations != null)
				{
					string[] array2 = unit.Abbreviations.Split('|');
					foreach (string text2 in array2)
					{
						wordlist.Add(text2);
						if (flag)
						{
							wordlist.Add(StringUtilities.HalfWidthToFullWidth2(text2));
						}
					}
				}
			}
			return wordlist;
		}

		internal static PhysicalUnitDescription Find(Unit unit)
		{
			return Units.FirstOrDefault((PhysicalUnitDescription u) => u != null && u.Unit == unit);
		}
	}
}
