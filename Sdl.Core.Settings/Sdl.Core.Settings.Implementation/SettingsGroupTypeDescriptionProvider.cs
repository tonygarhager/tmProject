using System;
using System.ComponentModel;

namespace Sdl.Core.Settings.Implementation
{
	public class SettingsGroupTypeDescriptionProvider : TypeDescriptionProvider
	{
		public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
		{
			SettingsGroup settingsGroup = instance as SettingsGroup;
			if (settingsGroup != null)
			{
				return new SettingsGroupTypeDescriptor(settingsGroup);
			}
			return base.GetTypeDescriptor(objectType, instance);
		}
	}
}
