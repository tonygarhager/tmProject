using System;
using System.Xml.Serialization;

namespace Sdl.Core.LanguageProcessing.Tokenization
{
	public class GenericRecognizerConfiguration
	{
		[XmlAttribute("enabled")]
		public bool Enabled
		{
			get;
			set;
		}

		[XmlAttribute("priority")]
		public int Priority
		{
			get;
			set;
		}

		[XmlAttribute("ignorecase")]
		public bool IgnoreCase
		{
			get;
			set;
		}

		[XmlAttribute("autosubstitutable")]
		public bool AutoSubstitutable
		{
			get;
			set;
		}

		[XmlAttribute("trailingcontext")]
		public TrailingContext TrailingContext
		{
			get;
			set;
		}

		[XmlElement("tokenclass")]
		public string TokenClass
		{
			get;
			set;
		}

		[XmlElement("cultures")]
		public string Cultures
		{
			get;
			set;
		}

		[XmlElement("firstset")]
		public string First
		{
			get;
			set;
		}

		[XmlElement("regex")]
		public string Regex
		{
			get;
			set;
		}

		public GenericRecognizerConfiguration()
		{
		}

		public GenericRecognizerConfiguration(bool enabled, int priority, bool ignoreCase, bool autoSubstitutable, TrailingContext tc, string tokenClass, string firstSet, string rx)
		{
			if (string.IsNullOrEmpty(tokenClass))
			{
				throw new ArgumentNullException("tokenClass");
			}
			if (string.IsNullOrEmpty(rx))
			{
				throw new ArgumentNullException("rx");
			}
			Enabled = enabled;
			Priority = priority;
			IgnoreCase = ignoreCase;
			AutoSubstitutable = autoSubstitutable;
			TrailingContext = tc;
			TokenClass = tokenClass;
			Regex = rx;
			First = firstSet;
			Cultures = null;
		}
	}
}
