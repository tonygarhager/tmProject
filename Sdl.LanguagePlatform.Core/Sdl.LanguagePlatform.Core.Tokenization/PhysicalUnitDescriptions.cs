using Sdl.LanguagePlatform.Core.Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Sdl.LanguagePlatform.Core.Tokenization
{
	[XmlRoot(Namespace = "http://www.sdl.com/PhysicalUnitDescriptions", ElementName = "PhysicalUnitDescriptions")]
	public class PhysicalUnitDescriptions : IEnumerable<PhysicalUnitDescription>, IEnumerable
	{
		public List<PhysicalUnitDescription> Units
		{
			get;
			set;
		}

		[XmlIgnore]
		public int Count
		{
			get
			{
				List<PhysicalUnitDescription> units = Units;
				if (units == null)
				{
					return 0;
				}
				return units.Count;
			}
		}

		public static PhysicalUnitDescriptions Load(string fileName)
		{
			using (Stream reader = File.OpenRead(fileName))
			{
				return Load(reader);
			}
		}

		public static PhysicalUnitDescriptions Load(CultureInfo culture, IResourceDataAccessor accessor)
		{
			using (Stream reader = accessor.ReadResourceData(culture, LanguageResourceType.PhysicalUnits, fallback: true))
			{
				return Load(reader);
			}
		}

		public static PhysicalUnitDescriptions Load(Stream reader)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(PhysicalUnitDescriptions));
			XmlReaderSettings settings = new XmlReaderSettings
			{
				CheckCharacters = false
			};
			XmlReader xmlReader = XmlReader.Create(reader, settings);
			return xmlSerializer.Deserialize(xmlReader) as PhysicalUnitDescriptions;
		}

		public void Save(string fileName)
		{
			using (FileStream writer = File.Create(fileName))
			{
				Save(writer);
			}
		}

		public void Save(Stream writer)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(PhysicalUnitDescriptions));
			xmlSerializer.Serialize(writer, this);
		}

		public PhysicalUnitDescriptions()
		{
			Units = new List<PhysicalUnitDescription>();
		}

		public PhysicalUnitDescriptions(PhysicalUnitDescriptions other)
		{
			if (other == null)
			{
				throw new ArgumentNullException();
			}
			if (other.Units != null)
			{
				Units = new List<PhysicalUnitDescription>();
				foreach (PhysicalUnitDescription unit in other.Units)
				{
					Units.Add(new PhysicalUnitDescription(unit));
				}
			}
		}

		public void Add(PhysicalUnitDescription d)
		{
			if (Units == null)
			{
				Units = new List<PhysicalUnitDescription>();
			}
			Units.Add(d);
		}

		public void Delete(PhysicalUnitDescription d)
		{
			throw new NotImplementedException();
		}

		public PhysicalUnitDescription FindFirst(string abbreviation)
		{
			throw new NotImplementedException();
		}

		public IEnumerator<PhysicalUnitDescription> GetEnumerator()
		{
			if (Units == null)
			{
				throw new InvalidOperationException("Object not initialized");
			}
			return Units.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			if (Units == null)
			{
				throw new InvalidOperationException("Object not initialized");
			}
			return Units.GetEnumerator();
		}
	}
}
