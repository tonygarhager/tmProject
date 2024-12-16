using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Sdl.LanguagePlatform.Core.Tokenization
{
	public class PhysicalUnitDescription
	{
		private readonly string _PreferredAbbreviation;

		public Unit Unit
		{
			get;
			set;
		}

		public UnitType UnitType
		{
			get;
			set;
		}

		public UnitSystem UnitSystem
		{
			get;
			set;
		}

		public bool IsCanonical
		{
			get;
			set;
		}

		public int Magnitude
		{
			get;
			set;
		}

		public double ConversionFactor
		{
			get;
			set;
		}

		public Unit ConversionUnit
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public string Abbreviations
		{
			get;
			set;
		}

		public List<Pair<string>> CultureSpecificAbbreviations
		{
			get;
			set;
		}

		public string Category
		{
			get;
			set;
		}

		public PhysicalUnitDescription()
		{
			Unit = Unit.NoUnit;
			UnitSystem = UnitSystem.NoSystem;
			UnitType = UnitType.NoType;
			ConversionUnit = Unit.NoUnit;
			CultureSpecificAbbreviations = null;
			Name = null;
			Abbreviations = null;
		}

		public PhysicalUnitDescription(PhysicalUnitDescription other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			Unit = other.Unit;
			UnitType = other.UnitType;
			UnitSystem = other.UnitSystem;
			IsCanonical = other.IsCanonical;
			Magnitude = other.Magnitude;
			ConversionFactor = other.ConversionFactor;
			ConversionUnit = other.ConversionUnit;
			Name = other.Name;
			Abbreviations = other.Abbreviations;
			_PreferredAbbreviation = other._PreferredAbbreviation;
			if (other.CultureSpecificAbbreviations != null)
			{
				CultureSpecificAbbreviations = new List<Pair<string>>();
				foreach (Pair<string> cultureSpecificAbbreviation in other.CultureSpecificAbbreviations)
				{
					CultureSpecificAbbreviations.Add(new Pair<string>(cultureSpecificAbbreviation.Left, cultureSpecificAbbreviation.Right));
				}
			}
		}

		public PhysicalUnitDescription(Unit u, UnitType ut, UnitSystem us, bool isCanonical, int magnitude, double conversionFactor, Unit conversionUnit, string name, string abbreviation)
			: this(u, ut, us, isCanonical, magnitude, conversionFactor, conversionUnit, name, abbreviation, null)
		{
		}

		public PhysicalUnitDescription(Unit u, UnitType ut, UnitSystem us, bool isCanonical, int magnitude, double conversionFactor, Unit conversionUnit, string name, string abbreviation, string preferredAbbreviation)
		{
			Unit = u;
			UnitType = ut;
			UnitSystem = us;
			IsCanonical = isCanonical;
			Magnitude = magnitude;
			ConversionFactor = conversionFactor;
			ConversionUnit = conversionUnit;
			Name = name;
			Abbreviations = abbreviation;
			if (preferredAbbreviation != null)
			{
				_PreferredAbbreviation = preferredAbbreviation;
			}
			else if (Abbreviations == null)
			{
				_PreferredAbbreviation = null;
			}
			else
			{
				_PreferredAbbreviation = LastAbbreviation(Abbreviations);
			}
		}

		private static string LastAbbreviation(string entry)
		{
			int num = entry.LastIndexOf('|');
			if (num >= 0)
			{
				return entry.Substring(num + 1);
			}
			return entry;
		}

		public string GetPreferredAbbreviation(CultureInfo culture)
		{
			if (culture != null)
			{
				Pair<string> pair = CultureSpecificAbbreviations?.FirstOrDefault((Pair<string> x) => string.CompareOrdinal(culture.Name, x.Left) == 0);
				if (pair != null && pair.Right != null)
				{
					return LastAbbreviation(pair.Right);
				}
				return null;
			}
			return _PreferredAbbreviation;
		}

		public bool HasAbbreviation(string abbr)
		{
			if (abbr == null)
			{
				throw new ArgumentNullException();
			}
			return HasAbbreviation(abbr, null);
		}

		public string GetCultureSpecificAbbreviations(CultureInfo culture)
		{
			if (culture == null || CultureSpecificAbbreviations == null)
			{
				return null;
			}
			foreach (Pair<string> cultureSpecificAbbreviation in CultureSpecificAbbreviations)
			{
				if (ContainsCulture(cultureSpecificAbbreviation.Left, culture))
				{
					return cultureSpecificAbbreviation.Right;
				}
			}
			return null;
		}

		private static bool ContainsCulture(string cultureList, CultureInfo culture)
		{
			return (from cn in cultureList.Split('|')
				select CultureInfoExtensions.GetCultureInfo(cn, returnNullForUnknowns: true)).Any((CultureInfo ci) => ci != null && CultureInfoExtensions.AreCompatible(ci, culture));
		}

		public bool HasAbbreviation(string abbr, CultureInfo culture)
		{
			if (Abbreviations != null && Abbreviations.Split('|').Any((string a) => abbr.Equals(a, StringComparison.Ordinal)))
			{
				return true;
			}
			if (CultureSpecificAbbreviations == null || culture == null)
			{
				return false;
			}
			foreach (Pair<string> cultureSpecificAbbreviation in CultureSpecificAbbreviations)
			{
				if (ContainsCulture(cultureSpecificAbbreviation.Left, culture) && cultureSpecificAbbreviation.Right.Split('|').Any((string a) => abbr.Equals(a, StringComparison.Ordinal)))
				{
					return true;
				}
			}
			return false;
		}
	}
}
