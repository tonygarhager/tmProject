using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Sdl.FileTypeSupport.Framework.Formatting
{
	public interface IFormattingGroup : IDictionary<string, IFormattingItem>, ICollection<KeyValuePair<string, IFormattingItem>>, IEnumerable<KeyValuePair<string, IFormattingItem>>, IEnumerable, ICloneable, IXmlSerializable
	{
		new IFormattingItem this[string formattingName]
		{
			get;
		}

		void Add(IFormattingItem formatting);

		bool Contains(string formattingName);

		bool Contains(IFormattingItem formatting);

		void OverrideWith(IFormattingGroup otherFormatting);

		void UnderrideWith(IFormattingGroup otherFormatting);
	}
}
