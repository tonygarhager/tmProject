using System;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public class ProgressEventArgs : EventArgs
	{
		private byte _value;

		public byte ProgressValue => _value;

		public ProgressEventArgs(byte value)
		{
			if (value > 100)
			{
				value = 100;
			}
			_value = value;
		}

		public override string ToString()
		{
			return $"{_value}%";
		}
	}
}
