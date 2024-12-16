using Sdl.LanguagePlatform.Core.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Sdl.LanguagePlatform.Core.Segmentation
{
	[XmlRoot(Namespace = "http://www.sdl.com/SegmentationRuleSet", ElementName = "SegmentationRules")]
	[DataContract]
	public class SegmentationRules : ICloneable
	{
		[DataMember]
		public LocalizedString Description
		{
			get;
			set;
		}

		[XmlIgnore]
		public int Count => Rules.Count;

		[XmlIgnore]
		public bool ListReferencesResolved
		{
			get;
			private set;
		}

		[DataMember]
		public List<SegmentationRule> Rules
		{
			get;
			set;
		}

		public SegmentationRule this[int index] => Rules[index];

		[XmlIgnore]
		public CultureInfo Culture => CultureInfoExtensions.GetCultureInfo(CultureName);

		[XmlAttribute("cultureName")]
		[DataMember]
		public string CultureName
		{
			get;
			set;
		}

		public static SegmentationRules Load(string fileName, CultureInfo cultureOverride, IResourceDataAccessor accessor)
		{
			return Load(fileName, cultureOverride, accessor, keepListReferences: false);
		}

		public static SegmentationRules Load(string fileName, CultureInfo cultureOverride, IResourceDataAccessor accessor, bool keepListReferences)
		{
			using (Stream reader = File.OpenRead(fileName))
			{
				return Load(reader, cultureOverride, accessor, keepListReferences);
			}
		}

		public static SegmentationRules Load(Stream reader, CultureInfo cultureOverride, IResourceDataAccessor accessor)
		{
			return Load(reader, cultureOverride, accessor, keepListReferences: false);
		}

		public static SegmentationRules Load(Stream reader, CultureInfo cultureOverride, IResourceDataAccessor accessor, bool keepListReferences)
		{
			return LoadInternal(reader, cultureOverride?.Name, accessor, keepListReferences, useDataContractSerializer: false);
		}

		public static SegmentationRules Load(string fileName, string cultureOverride, IResourceDataAccessor accessor)
		{
			return Load(fileName, cultureOverride, accessor, keepListReferences: false);
		}

		public static SegmentationRules Load(string fileName, string cultureOverride, IResourceDataAccessor accessor, bool keepListReferences)
		{
			using (Stream reader = File.OpenRead(fileName))
			{
				return Load(reader, cultureOverride, accessor, keepListReferences);
			}
		}

		public static SegmentationRules Load(Stream reader, string cultureOverride, IResourceDataAccessor accessor)
		{
			return Load(reader, cultureOverride, accessor, keepListReferences: false);
		}

		public static SegmentationRules Load(Stream reader, string cultureOverride, IResourceDataAccessor accessor, bool keepListReferences)
		{
			return LoadInternal(reader, cultureOverride, accessor, keepListReferences, useDataContractSerializer: false);
		}

		public static SegmentationRules LoadUsingDataContractSerializer(Stream reader, CultureInfo cultureOverride, IResourceDataAccessor accessor, bool keepListReferences)
		{
			return LoadInternal(reader, cultureOverride.Name, accessor, keepListReferences, useDataContractSerializer: true);
		}

		public static SegmentationRules LoadUsingDataContractSerializer(Stream reader, string cultureOverride, IResourceDataAccessor accessor, bool keepListReferences)
		{
			return LoadInternal(reader, cultureOverride, accessor, keepListReferences, useDataContractSerializer: true);
		}

		internal static SegmentationRules LoadInternal(Stream reader, string cultureOverride, IResourceDataAccessor accessor, bool keepListReferences, bool useDataContractSerializer)
		{
			SegmentationRules segmentationRules;
			if (useDataContractSerializer)
			{
				DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(SegmentationRules));
				segmentationRules = (dataContractSerializer.ReadObject(reader) as SegmentationRules);
			}
			else
			{
				XmlReaderSettings settings = new XmlReaderSettings
				{
					CheckCharacters = false
				};
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(SegmentationRules));
				XmlReader xmlReader = XmlReader.Create(reader, settings);
				segmentationRules = (xmlSerializer.Deserialize(xmlReader) as SegmentationRules);
			}
			if (segmentationRules == null)
			{
				return null;
			}
			if (cultureOverride != null)
			{
				segmentationRules.CultureName = cultureOverride;
			}
			if (keepListReferences)
			{
				return segmentationRules;
			}
			segmentationRules.ResolvePatternReferences(accessor);
			segmentationRules.ListReferencesResolved = true;
			return segmentationRules;
		}

		public SegmentationRules()
		{
			Description = new LocalizedString();
			CultureName = CultureInfo.InvariantCulture.Name;
			Rules = new List<SegmentationRule>();
		}

		public SegmentationRules(SegmentationRules other)
		{
			if (other == null)
			{
				throw new ArgumentNullException();
			}
			Description = new LocalizedString(other.Description);
			CultureName = other.CultureName;
			if (other.Rules != null)
			{
				Rules = new List<SegmentationRule>();
				foreach (SegmentationRule rule in other.Rules)
				{
					Rules.Add(new SegmentationRule(rule));
				}
			}
		}

		public SegmentationRules(CultureInfo culture, string description)
		{
			Description = new LocalizedString();
			Description.SetText(CultureInfo.InvariantCulture, description);
			CultureName = culture?.Name;
			Rules = new List<SegmentationRule>();
		}

		public SegmentationRules(string cultureName, string description)
		{
			Description = new LocalizedString();
			Description.SetText(CultureInfo.InvariantCulture, description);
			CultureName = cultureName;
			Rules = new List<SegmentationRule>();
		}

		public override string ToString()
		{
			if (Description != null)
			{
				return Description.Text;
			}
			return "(null)";
		}

		public static void Save(SegmentationRules rules, Stream writer)
		{
			if (rules == null)
			{
				throw new ArgumentNullException("rules");
			}
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			rules.Save(writer);
		}

		public void Save(string fileName)
		{
			using (Stream stream = File.Create(fileName))
			{
				Save(stream);
				stream.Close();
			}
		}

		public void Save(Stream writer)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(GetType());
			xmlSerializer.Serialize(writer, this);
		}

		public void SaveUsingDataContractSerializer(Stream writer)
		{
			DataContractSerializer dataContractSerializer = new DataContractSerializer(GetType());
			dataContractSerializer.WriteObject(writer, this);
		}

		private void ResolvePatternReferences(IResourceDataAccessor accessor)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (accessor != null)
			{
				string value = LoadVariablePattern(accessor, LanguageResourceType.Abbreviations);
				dictionary.Add("$ABBREVIATIONS", value);
				value = LoadVariablePattern(accessor, LanguageResourceType.OrdinalFollowers);
				dictionary.Add("$ORDINALFOLLOWERS", value);
			}
			for (int num = Rules.Count - 1; num >= 0; num--)
			{
				SegmentationRule segmentationRule = Rules[num];
				if (segmentationRule.MatchingContext.Requires != null && (!dictionary.ContainsKey(segmentationRule.MatchingContext.Requires) || dictionary[segmentationRule.MatchingContext.Requires] == null))
				{
					Rules.RemoveAt(num);
				}
				else
				{
					ResolvePatternReferences(segmentationRule.MatchingContext, accessor, dictionary);
					if (segmentationRule.Exceptions != null)
					{
						for (int num2 = segmentationRule.Exceptions.Count - 1; num2 >= 0; num2--)
						{
							if (segmentationRule.Exceptions[num2].Requires != null && (!dictionary.ContainsKey(segmentationRule.Exceptions[num2].Requires) || dictionary[segmentationRule.Exceptions[num2].Requires] == null))
							{
								segmentationRule.Exceptions.RemoveAt(num2);
							}
						}
						foreach (SegmentationContext exception in segmentationRule.Exceptions)
						{
							ResolvePatternReferences(exception, accessor, dictionary);
						}
					}
				}
			}
		}

		private void ResolvePatternReferences(SegmentationContext sc, IResourceDataAccessor accessor, Dictionary<string, string> cachedPatterns)
		{
			if (sc != null)
			{
				if (sc.PrecedingContext != null)
				{
					ResolvePatternReferences(sc.PrecedingContext, accessor, cachedPatterns);
				}
				if (sc.FollowingContext != null)
				{
					ResolvePatternReferences(sc.FollowingContext, accessor, cachedPatterns);
				}
			}
		}

		private string LoadVariablePattern(IResourceDataAccessor accessor, LanguageResourceType rt)
		{
			if (accessor.GetResourceStatus(Culture, rt, fallback: true) == ResourceStatus.NotAvailable)
			{
				return null;
			}
			Wordlist wordlist = new Wordlist();
			wordlist.Load(accessor.ReadResourceData(Culture, rt, fallback: true));
			CharacterSet first;
			return wordlist.GetRegularExpression(out first);
		}

		private void ResolvePatternReferences(Context sc, IResourceDataAccessor accessor, Dictionary<string, string> cachedPatterns)
		{
			if (!string.IsNullOrEmpty(sc.Pattern))
			{
				StringBuilder stringBuilder = new StringBuilder(sc.Pattern);
				bool flag = false;
				foreach (string key in cachedPatterns.Keys)
				{
					if (sc.Pattern.IndexOf(key, StringComparison.Ordinal) >= 0)
					{
						string text = cachedPatterns[key];
						if (text == null)
						{
							throw new LanguagePlatformException(ErrorCode.SegmentationUnknownVariable, FaultStatus.Error, key + " (" + CultureName + ")");
						}
						stringBuilder = stringBuilder.Replace(key, text);
						flag = true;
					}
				}
				if (flag)
				{
					sc.Pattern = stringBuilder.ToString();
				}
			}
		}

		public void AddRule(SegmentationRule r)
		{
			if (r == null)
			{
				throw new ArgumentNullException();
			}
			if (Rules == null)
			{
				Rules = new List<SegmentationRule>();
			}
			Rules.Add(r);
		}

		public void Add(object o)
		{
			SegmentationRule segmentationRule = o as SegmentationRule;
			if (segmentationRule == null)
			{
				throw new ArgumentException("Illegal type");
			}
			AddRule(segmentationRule);
		}

		public IEnumerator<SegmentationRule> GetEnumerator()
		{
			return Rules.GetEnumerator();
		}

		public object Clone()
		{
			return new SegmentationRules(this);
		}
	}
}
