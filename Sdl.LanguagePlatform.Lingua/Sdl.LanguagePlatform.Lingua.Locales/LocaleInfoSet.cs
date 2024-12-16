using Sdl.LanguagePlatform.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Sdl.LanguagePlatform.Lingua.Locales
{
	public class LocaleInfoSet : IEnumerable<LocaleInfo>, IEnumerable
	{
		private delegate LocaleInfo ResourceLineParser(string line);

		private readonly List<LocaleInfo> _data;

		private readonly Dictionary<string, int> _codeIndex;

		private readonly Dictionary<string, int> _canonicalCodeIndex;

		private static readonly Dictionary<LocaleSource, LocaleInfoSet> Cache;

		public string Description
		{
			get;
			internal set;
		}

		public LocaleSource Source
		{
			get;
			internal set;
		}

		public IList<LocaleInfo> Data => _data;

		public int Count => _data.Count;

		public LocaleInfo this[int idx] => _data[idx];

		static LocaleInfoSet()
		{
			Cache = new Dictionary<LocaleSource, LocaleInfoSet>();
		}

		internal LocaleInfoSet(LocaleSource source, string description)
		{
			Source = source;
			Description = description;
			_data = new List<LocaleInfo>();
			_codeIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
			_canonicalCodeIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
		}

		public LocaleInfo LookupCode(string code)
		{
			if (code != null && _codeIndex.TryGetValue(code, out int value))
			{
				return _data[value];
			}
			return null;
		}

		public LocaleInfo LookupCanonicalCode(string canonicalCode)
		{
			if (canonicalCode == null)
			{
				return null;
			}
			if (!_canonicalCodeIndex.TryGetValue(canonicalCode, out int value))
			{
				return null;
			}
			return _data[value];
		}

		public bool IsSupported(CultureInfo culture)
		{
			if (culture == null)
			{
				throw new ArgumentNullException();
			}
			return LookupCanonicalCode(culture.Name) != null;
		}

		internal void Add(LocaleInfo item)
		{
			if (!_codeIndex.ContainsKey(item.Code))
			{
				int count = _data.Count;
				_data.Add(item);
				_codeIndex.Add(item.Code, count);
				string text = item.CanonicalCode ?? item.Code;
				if (!string.IsNullOrEmpty(text) && !_canonicalCodeIndex.ContainsKey(text))
				{
					_canonicalCodeIndex.Add(text, count);
				}
			}
		}

		public void Sort(FieldCode field, bool relaxedCodeEquality)
		{
			if (Data == null)
			{
				return;
			}
			switch (field)
			{
			case FieldCode.LCID:
				_data.Sort(LocaleInfo.CompareByLCIDThenByCode);
				break;
			case FieldCode.Name:
				_data.Sort((LocaleInfo x, LocaleInfo y) => string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase));
				break;
			case FieldCode.Code:
				if (relaxedCodeEquality)
				{
					_data.Sort(LocaleInfo.CompareByRelaxedCode);
				}
				else
				{
					_data.Sort((LocaleInfo x, LocaleInfo y) => string.Compare(x.Code, y.Code, StringComparison.OrdinalIgnoreCase));
				}
				break;
			default:
				throw new Exception("Invalid field");
			}
		}

		public static LocaleInfoSet GetLocaleInfoSet(LocaleSource source)
		{
			lock (Cache)
			{
				if (Cache.TryGetValue(source, out LocaleInfoSet value))
				{
					return value;
				}
				switch (source)
				{
				case LocaleSource.XP:
				case LocaleSource.Vista:
				case LocaleSource.Win7:
					value = ReadData(source, CultureDumpParser);
					break;
				case LocaleSource.Workbench:
					value = ReadData(source, WorkbenchDumpParser);
					break;
				case LocaleSource.SDLX:
					value = ReadData(source, SdlxDumpParser);
					break;
				case LocaleSource.TMS:
					value = ReadData(source, TmsDumpParser);
					break;
				default:
					throw new Exception("Invalid enum");
				}
				Cache.Add(source, value);
				return value;
			}
		}

		private static string GetResourceName(LocaleSource source)
		{
			switch (source)
			{
			case LocaleSource.XP:
				return "XPSP2-en.txt";
			case LocaleSource.Vista:
				return "vistahp-en.txt";
			case LocaleSource.Win7:
				return "win7-en.txt";
			case LocaleSource.Workbench:
				return "twb-vista.txt";
			case LocaleSource.SDLX:
				return "sdlx.txt";
			case LocaleSource.TMS:
				return "tms.txt";
			default:
				throw new Exception("Invalid enum");
			}
		}

		private static LocaleInfo Extract(string line, int expectedFieldCount, int codeColumn, int nameColumn, int lcidColumn, bool computeExtraFields)
		{
			string[] array = line.Split("\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			if (array.Length != expectedFieldCount)
			{
				return null;
			}
			int lcid = int.Parse(array[lcidColumn]);
			LocaleInfo localeInfo = new LocaleInfo(lcid, array[nameColumn], array[codeColumn]);
			if (!computeExtraFields)
			{
				return localeInfo;
			}
			localeInfo.RegionQualifiedCode = CultureInfoExtensions.GetRegionQualifiedCulture(localeInfo.Code);
			localeInfo.CanonicalCode = CultureInfoExtensions.GetMappedCultureCodeForLegacyCode(localeInfo.Code);
			return localeInfo;
		}

		private static LocaleInfo WorkbenchDumpParser(string line)
		{
			return Extract(line, 6, 4, 5, 1, computeExtraFields: true);
		}

		private static LocaleInfo SdlxDumpParser(string line)
		{
			return Extract(line, 3, 1, 0, 2, computeExtraFields: true);
		}

		private static LocaleInfo TmsDumpParser(string line)
		{
			return Extract(line, 3, 0, 1, 2, computeExtraFields: true);
		}

		private static LocaleInfo CultureDumpParser(string line)
		{
			return Extract(line, 4, 0, 1, 2, computeExtraFields: false);
		}

		private static LocaleInfoSet ReadData(LocaleSource source, ResourceLineParser parser)
		{
			string resourceName = GetResourceName(source);
			LocaleInfoSet localeInfoSet = new LocaleInfoSet(source, source.ToString() + " (" + resourceName + ")");
			using (Stream stream = typeof(LocaleInfoSet).Assembly.GetManifestResourceStream(typeof(LocaleInfoSet), resourceName))
			{
				if (stream != null)
				{
					using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true))
					{
						string text;
						while ((text = streamReader.ReadLine()) != null)
						{
							if (!string.IsNullOrEmpty(text) && !text.StartsWith("#"))
							{
								LocaleInfo localeInfo = parser(text);
								if (localeInfo != null)
								{
									localeInfoSet.Add(localeInfo);
								}
							}
						}
						return localeInfoSet;
					}
				}
				return localeInfoSet;
			}
		}

		public IEnumerator<LocaleInfo> GetEnumerator()
		{
			return _data.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _data.GetEnumerator();
		}
	}
}
