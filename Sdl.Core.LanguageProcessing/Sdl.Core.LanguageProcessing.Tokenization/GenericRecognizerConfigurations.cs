using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Sdl.Core.LanguageProcessing.Tokenization
{
	public class GenericRecognizerConfigurations
	{
		[XmlElement("GenericRecognizer")]
		public List<GenericRecognizerConfiguration> Configurations
		{
			get;
			set;
		}

		public GenericRecognizerConfigurations()
		{
			Configurations = new List<GenericRecognizerConfiguration>();
		}

		public void Add(GenericRecognizerConfiguration configuration)
		{
			if (configuration == null)
			{
				throw new ArgumentNullException();
			}
			if (Configurations == null)
			{
				Configurations = new List<GenericRecognizerConfiguration>();
			}
			Configurations.Add(configuration);
		}

		public void Save(string fileName)
		{
			using (TextWriter wtr = new StreamWriter(fileName, append: false, Encoding.UTF8))
			{
				Save(wtr);
			}
		}

		public void Save(TextWriter wtr)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(GetType());
			xmlSerializer.Serialize(wtr, this);
		}

		public static GenericRecognizerConfigurations Load(string fileName)
		{
			using (TextReader rdr = new StreamReader(fileName, Encoding.UTF8, detectEncodingFromByteOrderMarks: true))
			{
				return Load(rdr);
			}
		}

		public static GenericRecognizerConfigurations Load(TextReader rdr)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(GenericRecognizerConfigurations));
			return xmlSerializer.Deserialize(rdr) as GenericRecognizerConfigurations;
		}
	}
}
