using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace Sdl.Core.Settings.Implementation
{
	internal static class TypeConverterUtil
	{
		public static string ConvertToInvariantString(object value)
		{
			TypeConverter converter = GetConverter(value.GetType());
			if (converter != null)
			{
				return converter.ConvertToString(value);
			}
			return (string)Convert.ChangeType(value, typeof(string), CultureInfo.InvariantCulture);
		}

		public static object ConvertFromInvariantString(string value, Type destinationtype)
		{
			try
			{
				TypeConverter converter = GetConverter(destinationtype);
				if (converter != null)
				{
					return converter.ConvertFromString(value);
				}
				return Convert.ChangeType(value, destinationtype, CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				return null;
			}
		}

		public static bool CanConvertToString(Type type)
		{
			if (GetConverter(type) != null)
			{
				return true;
			}
			return typeof(IConvertible).IsAssignableFrom(type);
		}

		private static TypeConverter GetConverter(Type type)
		{
			if (type.IsEnum)
			{
				return new EnumConverter(type);
			}
			if (type == typeof(Guid))
			{
				return new GuidConverter();
			}
			if (type == typeof(Rectangle))
			{
				return new RectangleConverter();
			}
			if (type == typeof(Point))
			{
				return new PointConverter();
			}
			if (type == typeof(Color))
			{
				return new ColorConverter();
			}
			return null;
		}
	}
}
