using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Sdl.Core.Bcm.BcmModel.Tokenization
{
	public static class PhysicalUnit
	{
		private static readonly List<PhysicalUnitDescription> _Units;

		private static Dictionary<string, int> AnonymousGroups;

		static PhysicalUnit()
		{
			_Units = new List<PhysicalUnitDescription>();
			_Units.Add(new PhysicalUnitDescription(Unit.Mmm, UnitType.Length, UnitSystem.Metric, isCanonical: false, 1, 0.001, Unit.Mm, "millimeter", "mm"));
			_Units.Add(new PhysicalUnitDescription(Unit.Mcm, UnitType.Length, UnitSystem.Metric, isCanonical: false, 2, 0.01, Unit.Mm, "centimeter", "cm"));
			_Units.Add(new PhysicalUnitDescription(Unit.Mdm, UnitType.Length, UnitSystem.Metric, isCanonical: false, 4, 0.1, Unit.Mm, "decimeter", "dm"));
			_Units.Add(new PhysicalUnitDescription(Unit.Mm, UnitType.Length, UnitSystem.Metric, isCanonical: true, 4, 1.0, Unit.Mm, "meter", "m"));
			_Units.Add(new PhysicalUnitDescription(Unit.Mkm, UnitType.Length, UnitSystem.Metric, isCanonical: false, 5, 1000.0, Unit.Mm, "kilometer", "km"));
			_Units.Add(new PhysicalUnitDescription(Unit.Mmm2, UnitType.Area, UnitSystem.Metric, isCanonical: false, 1, 1E-06, Unit.Mm2, "square millimeter", "mm²"));
			_Units.Add(new PhysicalUnitDescription(Unit.Mcm2, UnitType.Area, UnitSystem.Metric, isCanonical: false, 1, 0.0001, Unit.Mm2, "square centimeter", "cm²"));
			_Units.Add(new PhysicalUnitDescription(Unit.Mm2, UnitType.Area, UnitSystem.Metric, isCanonical: true, 3, 1.0, Unit.Mm2, "square meter", "m²"));
			_Units.Add(new PhysicalUnitDescription(Unit.Ma, UnitType.Area, UnitSystem.Metric, isCanonical: false, 4, 100.0, Unit.Mm2, "are", "a"));
			_Units.Add(new PhysicalUnitDescription(Unit.Mha, UnitType.Area, UnitSystem.Metric, isCanonical: false, 4, 10000.0, Unit.Mm2, "hectare", "ha"));
			_Units.Add(new PhysicalUnitDescription(Unit.Mkm2, UnitType.Area, UnitSystem.Metric, isCanonical: false, 5, 1000000.0, Unit.Mm2, "square kilometer", "km²"));
			_Units.Add(new PhysicalUnitDescription(Unit.Mmg, UnitType.Mass, UnitSystem.Metric, isCanonical: false, 1, 0.001, Unit.Mg, "milligram", "mg"));
			_Units.Add(new PhysicalUnitDescription(Unit.Mg, UnitType.Mass, UnitSystem.Metric, isCanonical: true, 1, 1.0, Unit.Mg, "gram", "g"));
			_Units.Add(new PhysicalUnitDescription(Unit.Mkg, UnitType.Mass, UnitSystem.Metric, isCanonical: false, 2, 1000.0, Unit.Mg, "kilogram", "kg"));
			_Units.Add(new PhysicalUnitDescription(Unit.Mt, UnitType.Mass, UnitSystem.Metric, isCanonical: false, 4, 1000000.0, Unit.Mg, "metric ton", "t"));
			_Units.Add(new PhysicalUnitDescription(Unit.Mml, UnitType.Capacity, UnitSystem.Metric, isCanonical: false, 1, 0.001, Unit.Ml, "milliliter", "ml"));
			_Units.Add(new PhysicalUnitDescription(Unit.Mcm3, UnitType.Capacity, UnitSystem.Metric, isCanonical: false, 1, 0.001, Unit.Ml, "cubic centimeter", "cm³"));
			_Units.Add(new PhysicalUnitDescription(Unit.Mcl, UnitType.Capacity, UnitSystem.Metric, isCanonical: false, 2, 0.01, Unit.Ml, "centiliter", "cl"));
			_Units.Add(new PhysicalUnitDescription(Unit.Mdl, UnitType.Capacity, UnitSystem.Metric, isCanonical: false, 3, 0.1, Unit.Ml, "deciliter", "dl"));
			_Units.Add(new PhysicalUnitDescription(Unit.Ml, UnitType.Capacity, UnitSystem.Metric, isCanonical: true, 4, 1.0, Unit.Ml, "liter", "l"));
			_Units.Add(new PhysicalUnitDescription(Unit.Mm3, UnitType.Capacity, UnitSystem.Metric, isCanonical: false, 5, 1000.0, Unit.Ml, "cubic meter", "m³"));
			_Units.Add(new PhysicalUnitDescription(Unit.Mcentigrade, UnitType.Temperature, UnitSystem.NoSystem, isCanonical: false, 0, 0.0, Unit.Mcentigrade, "degrees centigrade", "°C|℃"));
			_Units.Add(new PhysicalUnitDescription(Unit.Mfahrenheit, UnitType.Temperature, UnitSystem.NoSystem, isCanonical: false, 0, 0.0, Unit.Mcentigrade, "degrees fahrenheit", "°F|℉"));
			_Units.Add(new PhysicalUnitDescription(Unit.Mkelvin, UnitType.Temperature, UnitSystem.NoSystem, isCanonical: true, 0, 0.0, Unit.Mcentigrade, "degrees kelvin", "°K|K"));
			_Units.Add(new PhysicalUnitDescription(Unit.Mpercent, UnitType.Other, UnitSystem.NoSystem, isCanonical: true, 0, 0.0, Unit.Mpercent, "percent", "%|٪|﹪|％", "%"));
			_Units.Add(new PhysicalUnitDescription(Unit.Mdegree, UnitType.Other, UnitSystem.NoSystem, isCanonical: true, 0, 0.0, Unit.Mdegree, "degrees", "°"));
			_Units.Add(new PhysicalUnitDescription(Unit.BISin, UnitType.Length, UnitSystem.BIS, isCanonical: false, 1, 25.4, Unit.Mmm, "inch", "inch|inches"));
			_Units.Add(new PhysicalUnitDescription(Unit.BISft, UnitType.Length, UnitSystem.BIS, isCanonical: false, 3, 12.0, Unit.BISin, "foot", "feet|foot|ft"));
			_Units.Add(new PhysicalUnitDescription(Unit.BISyd, UnitType.Length, UnitSystem.BIS, isCanonical: false, 4, 3.0, Unit.BISft, "yard", "yard|yards|yd"));
			_Units.Add(new PhysicalUnitDescription(Unit.BISfurlong, UnitType.Length, UnitSystem.BIS, isCanonical: false, 5, 220.0, Unit.BISyd, "furlong", "furlong"));
			_Units.Add(new PhysicalUnitDescription(Unit.BISmi, UnitType.Length, UnitSystem.BIS, isCanonical: false, 5, 1760.0, Unit.BISyd, "mile", "mile|miles|mi"));
			_Units.Add(new PhysicalUnitDescription(Unit.BISin2, UnitType.Area, UnitSystem.BIS, isCanonical: false, 1, 645.16, Unit.Mmm2, "square inch", "sq in|in²"));
			_Units.Add(new PhysicalUnitDescription(Unit.BISft2, UnitType.Area, UnitSystem.BIS, isCanonical: false, 2, 144.0, Unit.BISin2, "square foot", "sq ft|ft²"));
			_Units.Add(new PhysicalUnitDescription(Unit.BISyd2, UnitType.Area, UnitSystem.BIS, isCanonical: false, 3, 9.0, Unit.BISft2, "square yard", "sq yd|yd²"));
			_Units.Add(new PhysicalUnitDescription(Unit.BISacre, UnitType.Area, UnitSystem.BIS, isCanonical: false, 4, 4840.0, Unit.BISyd2, "acre", "acre|acres"));
			_Units.Add(new PhysicalUnitDescription(Unit.BISmi2, UnitType.Area, UnitSystem.BIS, isCanonical: false, 5, 640.0, Unit.BISacre, "square mile", "sq mi|mi²"));
			_Units.Add(new PhysicalUnitDescription(Unit.BISoz, UnitType.Mass, UnitSystem.BIS, isCanonical: false, 1, 28.34952, Unit.Mg, "avoirdupois ounce", "oz avdp|oz"));
			_Units.Add(new PhysicalUnitDescription(Unit.BISlb, UnitType.Mass, UnitSystem.BIS, isCanonical: false, 2, 16.0, Unit.BISoz, "avoirdupois pound", "lb avdp|#|lb"));
			_Units.Add(new PhysicalUnitDescription(Unit.BISstone, UnitType.Mass, UnitSystem.BIS, isCanonical: false, 3, 14.0, Unit.BISlb, "stone", null));
			_Units.Add(new PhysicalUnitDescription(Unit.BISshortHW, UnitType.Mass, UnitSystem.BIS, isCanonical: false, 3, 100.0, Unit.BISlb, "short hundredweight", "short cwt"));
			_Units.Add(new PhysicalUnitDescription(Unit.BISlongHW, UnitType.Mass, UnitSystem.BIS, isCanonical: false, 3, 112.0, Unit.BISlb, "long hundredweight", "long cwt"));
			_Units.Add(new PhysicalUnitDescription(Unit.BISshortTon, UnitType.Mass, UnitSystem.BIS, isCanonical: false, 4, 2000.0, Unit.BISlb, "short ton", null));
			_Units.Add(new PhysicalUnitDescription(Unit.BISlongTon, UnitType.Mass, UnitSystem.BIS, isCanonical: false, 4, 2240.0, Unit.BISlb, "long ton", null));
			_Units.Add(new PhysicalUnitDescription(Unit.BISflozUK, UnitType.Capacity, UnitSystem.BISUK, isCanonical: false, 1, 28.412, Unit.Mcm3, "UK fluid ounce", "fl oz"));
			_Units.Add(new PhysicalUnitDescription(Unit.BISptUK, UnitType.Capacity, UnitSystem.BISUK, isCanonical: false, 3, 20.0, Unit.BISflozUK, "UK pint", "pt"));
			_Units.Add(new PhysicalUnitDescription(Unit.BISqtUK, UnitType.Capacity, UnitSystem.BISUK, isCanonical: false, 4, 2.0, Unit.BISptUK, "UK quart", "qt"));
			_Units.Add(new PhysicalUnitDescription(Unit.BISbuUK, UnitType.Capacity, UnitSystem.BISUK, isCanonical: false, 4, 64.0, Unit.BISptUK, "UK bushel", "bu"));
			_Units.Add(new PhysicalUnitDescription(Unit.BISgalUK, UnitType.Capacity, UnitSystem.BISUK, isCanonical: false, 4, 8.0, Unit.BISptUK, "UK gallon", "gal"));
			_Units.Add(new PhysicalUnitDescription(Unit.BISflozUS, UnitType.Capacity, UnitSystem.BISUSFluid, isCanonical: false, 1, 29.57353, Unit.Mcm3, "US fluid ounce", "fl oz"));
			_Units.Add(new PhysicalUnitDescription(Unit.BISptUS, UnitType.Capacity, UnitSystem.BISUSFluid, isCanonical: false, 3, 16.0, Unit.BISflozUS, "US fluid pint", "pt"));
			_Units.Add(new PhysicalUnitDescription(Unit.BISgalUS, UnitType.Capacity, UnitSystem.BISUSFluid, isCanonical: false, 4, 128.0, Unit.BISflozUS, "US fluid gallon", "gal"));
			_Units.Add(new PhysicalUnitDescription(Unit.BISptUSDry, UnitType.Capacity, UnitSystem.BISUSDry, isCanonical: false, 1, 0.5506, Unit.Ml, "US dry pint", "pt"));
			_Units.Add(new PhysicalUnitDescription(Unit.BISbuUSDry, UnitType.Capacity, UnitSystem.BISUSDry, isCanonical: false, 4, 35.239, Unit.Ml, "US dry bushel", "bu"));
			AddAnonymousUnits(_Units);
			AddCultureSpecificAbbreviations(_Units);
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

		public static bool AreUnitTypesCompatible(Unit a, Unit b)
		{
			if (a == b)
			{
				return true;
			}
			UnitType unitType = GetUnitType(a);
			UnitType unitType2 = GetUnitType(b);
			if (unitType == unitType2 && unitType <= UnitType.Temperature)
			{
				return unitType2 <= UnitType.Temperature;
			}
			return false;
		}

		public static string GetAbbreviations(Unit unit)
		{
			return Find(unit)?.Abbreviations;
		}

		public static string GetPreferredAbbreviation(Unit unit, CultureInfo culture)
		{
			PhysicalUnitDescription physicalUnitDescription = Find(unit);
			if (physicalUnitDescription == null || physicalUnitDescription.Abbreviations == null)
			{
				return null;
			}
			return physicalUnitDescription.GetPreferredAbbreviation(culture);
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
				if (type == UnitType.Capacity)
				{
					if (regionInfo.ThreeLetterISORegionName.Equals("SGP"))
					{
						return UnitSystem.BISUK;
					}
					return UnitSystem.BISUSFluid;
				}
				return UnitSystem.BIS;
			}
			catch
			{
				return UnitSystem.Metric;
			}
		}

		private static PhysicalUnitDescription CreateAnonymousUnit(string unit)
		{
			return new PhysicalUnitDescription(Unit.NoUnit, UnitType.NoType, UnitSystem.NoSystem, isCanonical: false, 1, 1.0, Unit.NoUnit, unit, unit);
		}

		private static void AddAnonymousUnitUnlessPresent(List<PhysicalUnitDescription> units, HashSet<string> addedUnits, string u)
		{
			if (!addedUnits.Contains(u))
			{
				units.Add(CreateAnonymousUnit(u));
				addedUnits.Add(u);
			}
		}

		private static void AddAnonymousUnits(List<PhysicalUnitDescription> units)
		{
			HashSet<string> addedUnits = new HashSet<string>();
			AnonymousGroups = new Dictionary<string, int>();
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
				if (!AnonymousGroups.ContainsKey(text))
				{
					AnonymousGroups.Add(text, num);
				}
			}
			string[] array2 = "V|Hz|A|J|PS|N|Nm|K|cal|B|W".Split(separator, StringSplitOptions.RemoveEmptyEntries);
			foreach (string text2 in array2)
			{
				num++;
				if (!AnonymousGroups.ContainsKey(text2))
				{
					AnonymousGroups.Add(text2, num);
				}
				AddAnonymousUnitUnlessPresent(units, addedUnits, text2);
				string[] array3 = "m|M|k|K|G".Split(separator, StringSplitOptions.RemoveEmptyEntries);
				foreach (string str in array3)
				{
					AddAnonymousUnitUnlessPresent(units, addedUnits, str + text2);
					if (!AnonymousGroups.ContainsKey(str + text2))
					{
						AnonymousGroups.Add(str + text2, num);
					}
				}
			}
			string[] array4 = "dl|kg|l|m²|ml".Split(separator, StringSplitOptions.RemoveEmptyEntries);
			foreach (string text3 in array4)
			{
				num++;
				if (!AnonymousGroups.ContainsKey("g/" + text3))
				{
					AnonymousGroups.Add("g/" + text3, num);
				}
				AddAnonymousUnitUnlessPresent(units, addedUnits, "g/" + text3);
				string[] array5 = "m|µ|n|p|k".Split(separator, StringSplitOptions.RemoveEmptyEntries);
				foreach (string str2 in array5)
				{
					AddAnonymousUnitUnlessPresent(units, addedUnits, str2 + "g/" + text3);
					if (!AnonymousGroups.ContainsKey(str2 + "g/" + text3))
					{
						AnonymousGroups.Add(str2 + "g/" + text3, num);
					}
				}
			}
		}

		public static bool AreUnitsSameCategory(string UnitA, string UnitB)
		{
			if (UnitA == UnitB)
			{
				return true;
			}
			if (AnonymousGroups.ContainsKey(UnitA) && AnonymousGroups.ContainsKey(UnitB))
			{
				return AnonymousGroups[UnitA].Equals(AnonymousGroups[UnitB]);
			}
			return false;
		}

		private static void AddCultureSpecificAbbreviations(List<PhysicalUnitDescription> units)
		{
			string[] array = new string[56]
			{
				"µg",
				"мкг",
				"ng",
				"нг",
				"pg",
				"пг",
				"KB",
				"кБ|КБ",
				"MB",
				"МБ",
				"GB",
				"ГБ",
				"TB",
				"ТБ",
				"mm",
				"мм",
				"cm",
				"см",
				"dm",
				"дм",
				"m",
				"м",
				"km",
				"км",
				"mm²",
				"мм²",
				"cm²",
				"см²",
				"m²",
				"м²",
				"a",
				"ар",
				"ha",
				"га",
				"km²",
				"км²",
				"mg",
				"мг",
				"g",
				"г",
				"kg",
				"кг",
				"t",
				"т",
				"ml",
				"мл",
				"cm³",
				"см³",
				"cl",
				"сл",
				"dl",
				"дл",
				"l",
				"л",
				"m³",
				"м³"
			};
			for (int i = 0; i < array.Length; i += 2)
			{
			}
		}

		private static Unit CanonicalUnit(UnitType type)
		{
			foreach (PhysicalUnitDescription unit in _Units)
			{
				if (unit != null && unit.UnitType == type && unit.IsCanonical)
				{
					return unit.Unit;
				}
			}
			return Unit.NoUnit;
		}

		internal static PhysicalUnitDescription Find(Unit unit)
		{
			return _Units.FirstOrDefault((PhysicalUnitDescription u) => u != null && u.Unit == unit);
		}
	}
}
