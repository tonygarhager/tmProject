using Sdl.Core.Settings.Implementation.Xml;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Sdl.Core.Settings.Implementation
{
	internal class ArraySetting<T> : XmlSettingImpl<T>
	{
		private Type _elementType;

		public override T Value
		{
			get
			{
				XElement xElement = base.Xml.Element(XName.Get("Setting"));
				IEnumerable<XElement> enumerable = xElement.Elements("Value");
				List<object> list = new List<object>();
				foreach (XElement item in enumerable)
				{
					if (_elementType == typeof(string))
					{
						list.Add(item.Value);
					}
					else
					{
						list.Add(TypeConverterUtil.ConvertFromInvariantString(item.Value, _elementType));
					}
				}
				if (list.Count > 0 || xElement.Value != "null")
				{
					Array array = Array.CreateInstance(_elementType, list.Count);
					for (int i = 0; i < list.Count; i++)
					{
						array.SetValue(list[i], i);
					}
					return (T)(object)array;
				}
				return default(T);
			}
			set
			{
				SetValueCore(value);
				OnXmlUpdated();
			}
		}

		public ArraySetting(SettingsGroup settingsGroup, Setting setting, bool inherited)
			: base(settingsGroup, setting, inherited)
		{
			_elementType = typeof(T).GetElementType();
		}

		public ArraySetting(SettingsGroup settingsGroup, string settingId, T value, bool inherited)
			: base(settingsGroup, settingId, inherited)
		{
			_elementType = typeof(T).GetElementType();
			SetValueCore(value);
		}

		private void SetValueCore(T value)
		{
			XElement root = base.Xml.Root;
			while (root.FirstNode != null)
			{
				root.FirstNode.Remove();
			}
			if (value != null)
			{
				Array array = (Array)(object)value;
				foreach (object item in array)
				{
					XElement xElement = new XElement(XName.Get("Value"));
					if (_elementType == typeof(string))
					{
						xElement.Add(item);
					}
					else
					{
						xElement.Add(TypeConverterUtil.ConvertToInvariantString(item));
					}
					root.Add(xElement);
				}
			}
			else
			{
				root.Add("null");
			}
		}
	}
}
