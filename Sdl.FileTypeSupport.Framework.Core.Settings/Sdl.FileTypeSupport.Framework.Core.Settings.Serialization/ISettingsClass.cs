using System;

namespace Sdl.FileTypeSupport.Framework.Core.Settings.Serialization
{
	public interface ISettingsClass : ICloneable, IEquatable<ISettingsClass>
	{
		void Read(IValueGetter valueGetter);

		void Save(IValueProcessor valueProcessor);

		void ResetToDefaults();
	}
}
