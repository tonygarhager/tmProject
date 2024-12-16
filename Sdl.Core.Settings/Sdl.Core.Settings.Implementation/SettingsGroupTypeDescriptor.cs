using Sdl.Core.Settings.Implementation.Xml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Sdl.Core.Settings.Implementation
{
	public class SettingsGroupTypeDescriptor : CustomTypeDescriptor
	{
		private class SettingPropertyDescriptor : PropertyDescriptor
		{
			private SettingsGroup _settingsGroup;

			private PropertyInfo _property;

			public override Type ComponentType => _settingsGroup.GetType();

			public override bool IsReadOnly => false;

			public override Type PropertyType => _property.PropertyType.GetGenericArguments()[0];

			public event EventHandler ValueChanged;

			public SettingPropertyDescriptor(SettingsGroup settingsGroup, PropertyInfo property)
				: base(property.Name, null)
			{
				_settingsGroup = settingsGroup;
				_property = property;
			}

			public override bool CanResetValue(object component)
			{
				return true;
			}

			public override object GetValue(object component)
			{
				SettingsGroup settingsGroup = (SettingsGroup)component;
				Setting xmlSetting = settingsGroup.ResolveXmlSetting(Name);
				return SettingFactory.DeserializeSettingValue(PropertyType, xmlSetting);
			}

			public override void ResetValue(object component)
			{
				SettingsGroup obj = (SettingsGroup)component;
				object value = _property.GetValue(obj, null);
				value.GetType().InvokeMember("Reset", BindingFlags.InvokeMethod, null, value, null);
			}

			public override void SetValue(object component, object value)
			{
				SettingsGroup obj = (SettingsGroup)component;
				object value2 = _property.GetValue(obj, null);
				value2.GetType().InvokeMember("Value", BindingFlags.SetProperty, null, value2, new object[1]
				{
					value
				});
			}

			public override bool ShouldSerializeValue(object component)
			{
				return false;
			}

			public override void AddValueChanged(object component, EventHandler handler)
			{
				EnsureValueChangedAttached();
				ValueChanged += handler;
			}

			public override void RemoveValueChanged(object component, EventHandler handler)
			{
				ValueChanged -= handler;
				EnsureValueChangedDetached();
			}

			private void EnsureValueChangedAttached()
			{
				if (this.ValueChanged == null)
				{
					_settingsGroup.PropertyChanged += _settingsGroup_PropertyChanged;
				}
			}

			private void EnsureValueChangedDetached()
			{
				if (this.ValueChanged == null)
				{
					_settingsGroup.PropertyChanged += _settingsGroup_PropertyChanged;
				}
			}

			private void _settingsGroup_PropertyChanged(object sender, PropertyChangedEventArgs e)
			{
				if (e.PropertyName == Name && this.ValueChanged != null)
				{
					this.ValueChanged(this, EventArgs.Empty);
				}
			}
		}

		private class SettingDefinedPropertyDescriptor : PropertyDescriptor
		{
			private SettingsGroup _settingsGroup;

			private PropertyInfo _property;

			public override Type ComponentType => _settingsGroup.GetType();

			public override bool IsReadOnly => true;

			public override Type PropertyType => typeof(bool);

			public event EventHandler ValueChanged;

			public SettingDefinedPropertyDescriptor(SettingsGroup settingsGroup, PropertyInfo property)
				: base(property.Name + "Defined", null)
			{
				_settingsGroup = settingsGroup;
				_property = property;
			}

			public override bool CanResetValue(object component)
			{
				return true;
			}

			public override object GetValue(object component)
			{
				SettingsGroup settingsGroup = (SettingsGroup)component;
				bool inherited;
				Setting setting = settingsGroup.ResolveXmlSetting(_property.Name, out inherited);
				return !inherited;
			}

			public override bool ShouldSerializeValue(object component)
			{
				return false;
			}

			public override void ResetValue(object component)
			{
				throw new InvalidOperationException();
			}

			public override void SetValue(object component, object value)
			{
				throw new InvalidOperationException();
			}

			public override void AddValueChanged(object component, EventHandler handler)
			{
				EnsureValueChangedAttached();
				ValueChanged += handler;
			}

			public override void RemoveValueChanged(object component, EventHandler handler)
			{
				ValueChanged -= handler;
				EnsureValueChangedDetached();
			}

			private void EnsureValueChangedAttached()
			{
				if (this.ValueChanged == null)
				{
					_settingsGroup.PropertyChanged += _settingsGroup_PropertyChanged;
				}
			}

			private void EnsureValueChangedDetached()
			{
				if (this.ValueChanged == null)
				{
					_settingsGroup.PropertyChanged += _settingsGroup_PropertyChanged;
				}
			}

			private void _settingsGroup_PropertyChanged(object sender, PropertyChangedEventArgs e)
			{
				if (e.PropertyName == _property.Name && this.ValueChanged != null)
				{
					this.ValueChanged(this, EventArgs.Empty);
				}
			}
		}

		private SettingsGroup _settingsGroup;

		private PropertyDescriptorCollection _lazySettingsProperties;

		public SettingsGroupTypeDescriptor(SettingsGroup settingsGroup)
		{
			_settingsGroup = settingsGroup;
		}

		public override PropertyDescriptorCollection GetProperties()
		{
			if (_lazySettingsProperties == null)
			{
				PropertyInfo[] properties = GetType().GetProperties();
				List<PropertyDescriptor> list = new List<PropertyDescriptor>();
				PropertyInfo[] array = properties;
				foreach (PropertyInfo propertyInfo in array)
				{
					Type propertyType = propertyInfo.PropertyType;
					if (propertyType.IsGenericType)
					{
						Type genericTypeDefinition = propertyType.GetGenericTypeDefinition();
						if (genericTypeDefinition.Name.StartsWith("Setting"))
						{
							list.Add(new SettingPropertyDescriptor(_settingsGroup, propertyInfo));
							list.Add(new SettingDefinedPropertyDescriptor(_settingsGroup, propertyInfo));
						}
					}
				}
				_lazySettingsProperties = new PropertyDescriptorCollection(list.ToArray());
			}
			return _lazySettingsProperties;
		}
	}
}
