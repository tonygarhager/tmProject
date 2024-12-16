using System;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.Core.Bcm.BcmModel.Tokenization
{
	internal class PhysicalUnitDescription
	{
		private Unit _Unit;

		private UnitType _UnitType;

		private UnitSystem _UnitSystem;

		private bool _IsCanonical;

		private int _Magnitude;

		private double _ConversionFactor;

		private Unit _ConversionUnit;

		private string _Name;

		private string _Abbreviations;

		private readonly string _PreferredAbbreviation;

		private List<Tuple<string, string>> _CultureSpecificAbbreviations;

		public Unit Unit
		{
			get
			{
				return _Unit;
			}
			set
			{
				_Unit = value;
			}
		}

		public UnitType UnitType
		{
			get
			{
				return _UnitType;
			}
			set
			{
				_UnitType = value;
			}
		}

		public UnitSystem UnitSystem
		{
			get
			{
				return _UnitSystem;
			}
			set
			{
				_UnitSystem = value;
			}
		}

		public bool IsCanonical
		{
			get
			{
				return _IsCanonical;
			}
			set
			{
				_IsCanonical = value;
			}
		}

		public int Magnitude
		{
			get
			{
				return _Magnitude;
			}
			set
			{
				_Magnitude = value;
			}
		}

		public double ConversionFactor
		{
			get
			{
				return _ConversionFactor;
			}
			set
			{
				_ConversionFactor = value;
			}
		}

		public Unit ConversionUnit
		{
			get
			{
				return _ConversionUnit;
			}
			set
			{
				_ConversionUnit = value;
			}
		}

		public string Name
		{
			get
			{
				return _Name;
			}
			set
			{
				_Name = value;
			}
		}

		public string Abbreviations
		{
			get
			{
				return _Abbreviations;
			}
			set
			{
				_Abbreviations = value;
			}
		}

		public List<Tuple<string, string>> CultureSpecificAbbreviations
		{
			get
			{
				return _CultureSpecificAbbreviations;
			}
			set
			{
				_CultureSpecificAbbreviations = value;
			}
		}

		public PhysicalUnitDescription()
		{
			_Unit = Unit.NoUnit;
			_UnitSystem = UnitSystem.NoSystem;
			_UnitType = UnitType.NoType;
			_ConversionUnit = Unit.NoUnit;
			_CultureSpecificAbbreviations = null;
			_Name = null;
			_Abbreviations = null;
		}

		public PhysicalUnitDescription(PhysicalUnitDescription other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			_Unit = other._Unit;
			_UnitType = other._UnitType;
			_UnitSystem = other._UnitSystem;
			_IsCanonical = other._IsCanonical;
			_Magnitude = other._Magnitude;
			_ConversionFactor = other._ConversionFactor;
			_ConversionUnit = other._ConversionUnit;
			_Name = other._Name;
			_Abbreviations = other._Abbreviations;
			_PreferredAbbreviation = other._PreferredAbbreviation;
			if (other._CultureSpecificAbbreviations != null)
			{
				_CultureSpecificAbbreviations = new List<Tuple<string, string>>();
				foreach (Tuple<string, string> cultureSpecificAbbreviation in other._CultureSpecificAbbreviations)
				{
					_CultureSpecificAbbreviations.Add(new Tuple<string, string>(cultureSpecificAbbreviation.Item1, cultureSpecificAbbreviation.Item2));
				}
			}
		}

		public PhysicalUnitDescription(Unit u, UnitType ut, UnitSystem us, bool isCanonical, int magnitude, double conversionFactor, Unit conversionUnit, string name, string abbreviation)
			: this(u, ut, us, isCanonical, magnitude, conversionFactor, conversionUnit, name, abbreviation, null)
		{
		}

		public PhysicalUnitDescription(Unit u, UnitType ut, UnitSystem us, bool isCanonical, int magnitude, double conversionFactor, Unit conversionUnit, string name, string abbreviation, string preferredAbbreviation)
		{
			_Unit = u;
			_UnitType = ut;
			_UnitSystem = us;
			_IsCanonical = isCanonical;
			_Magnitude = magnitude;
			_ConversionFactor = conversionFactor;
			_ConversionUnit = conversionUnit;
			_Name = name;
			_Abbreviations = abbreviation;
			if (preferredAbbreviation != null)
			{
				_PreferredAbbreviation = preferredAbbreviation;
				return;
			}
			if (_Abbreviations == null)
			{
				_PreferredAbbreviation = null;
				return;
			}
			int num = _Abbreviations.LastIndexOf('|');
			if (num < 0)
			{
				_PreferredAbbreviation = _Abbreviations;
			}
			else
			{
				_PreferredAbbreviation = _Abbreviations.Substring(num + 1);
			}
		}

		public string GetPreferredAbbreviation(CultureInfo culture)
		{
			return _PreferredAbbreviation;
		}
	}
}
