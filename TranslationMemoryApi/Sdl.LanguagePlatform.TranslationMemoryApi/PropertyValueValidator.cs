using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Class used to Validate a property value.s
	/// </summary>
	public static class PropertyValueValidator
	{
		/// <summary>
		/// Validates a property value.
		/// </summary>
		/// <param name="instance">The instance of the object the property belongs to.</param>
		/// <param name="value">The value to validate.</param>
		public static void Validate(object instance, object value)
		{
			StackTrace stackTrace = new StackTrace();
			MethodBase method = stackTrace.GetFrame(1).GetMethod();
			if (method.IsSpecialName && (method.Name.StartsWith("set_") || method.Name.StartsWith("get_")))
			{
				string name = method.Name.Substring(4);
				PropertyInfo property = method.DeclaringType.GetProperty(name);
				object[] customAttributes = property.GetCustomAttributes(typeof(ValidationAttribute), inherit: false);
				object[] array = customAttributes;
				for (int i = 0; i < array.Length; i++)
				{
					ValidationAttribute validationAttribute = (ValidationAttribute)array[i];
					validationAttribute.Validate(value, name);
				}
			}
		}
	}
}
